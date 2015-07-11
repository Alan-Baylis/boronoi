using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Assets.Scripts;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Helpers
{
    public static class EnumerationExtensions
    {
        #region StateFlags
        public static bool Has(this StateFlags type, StateFlags value)
        {
            return (type & value) == value;
        }

        public static bool Is(this StateFlags type, StateFlags value)
        {
            return type == value;
        }

        public static StateFlags Add(this StateFlags type, StateFlags value)
        {
            return type | value;
        }

        public static StateFlags Remove(this StateFlags type, StateFlags value)
        {
            return type & ~value;
        }
        #endregion

        #region Map
        public static void CreateMesh(this Map map)
        {
            var vertices = new List<Vector3>();
            var colors = new List<Color>();
            var triangles = new List<int>();
            var normals = new List<Vector3>();

            foreach (var center in map.Centers.Values.Where(x => x.States.Has(StateFlags.Land) || x.States.Has(StateFlags.ShallowWater)))
            {
                //fuck this
                center.OrderCorners();

                var startingIndex = vertices.Count;
                vertices.Add(center.Point);
                normals.Add(center.Normal);
                foreach (var corner in center.Corners)
                {
                    vertices.Add(corner.Point);
                    normals.Add(corner.Normal);
                }

                var lastVertex = 0;
                var color = center.GetBiomeColor();
                //center color
                colors.Add(color);

                for (int i = startingIndex + 1; i < startingIndex + center.Corners.Count; i++)
                {
                    triangles.Add(startingIndex);
                    triangles.Add(i + 1);
                    triangles.Add(i);
                    lastVertex = i + 1;

                    colors.Add(color);
                }
                
                //last vertex color
                colors.Add(color);

                //last tri
                triangles.Add(startingIndex);
                triangles.Add(startingIndex + 1);
                triangles.Add(lastVertex);
            }
            
            // Rivers
            // Each river edge is made up with 2 tris
            const float riverWidth = 2f; // In world units
            var riverEdges = map.Edges.Values.Where(x => x.States.Has(StateFlags.River)).ToList();
            foreach (var riverEdge in riverEdges)
            {
                var upperCorner = riverEdge.VoronoiStart.Point.y > riverEdge.VoronoiEnd.Point.y
                    ? riverEdge.VoronoiStart
                    : riverEdge.VoronoiEnd;

                var lowerCorner = riverEdge.VoronoiStart.Point.y < riverEdge.VoronoiEnd.Point.y
                    ? riverEdge.VoronoiStart
                    : riverEdge.VoronoiEnd;

                var edgeDir = (lowerCorner.Point - upperCorner.Point).normalized;
                var leftNudge = Quaternion.Euler(Vector3.up * 90) * edgeDir * riverWidth + Vector3.up;
                var rightNudge = Quaternion.Euler(Vector3.up * -90) * edgeDir * riverWidth + Vector3.up;

                vertices.AddRange(new List<Vector3>
                {
                    upperCorner.Point + leftNudge,
                    upperCorner.Point + rightNudge,
                    lowerCorner.Point + leftNudge,
                    lowerCorner.Point + rightNudge
                });

                normals.AddRange(new List<Vector3>
                {
                    upperCorner.Normal,
                    upperCorner.Normal,
                    lowerCorner.Normal,
                    lowerCorner.Normal,
                });

                triangles.AddRange(new List<int>
                {
                    vertices.Count - 4, vertices.Count - 3, vertices.Count - 2, // upLeft, upRight, lowLeft
                    vertices.Count - 2, vertices.Count - 3, vertices.Count - 1, // lowLeft, upRight, lowRight
                });

                colors.AddRange(new List<Color> { Color.blue, Color.blue, Color.blue, Color.blue });
            }

            // Instantiating things
            var go = new GameObject("Island");
            var rend = go.AddComponent<MeshRenderer>();
            rend.material.shader = Resources.Load<Shader>("VertexColor");
            var mesh = new Mesh { name = "HexMesh" };
            var mcomp = go.AddComponent<MeshFilter>();
            mcomp.mesh = mesh;
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.colors = colors.ToArray();
            mesh.normals = normals.ToArray();
        }

        public static void GenerateElevation(this Map map)
        {
            var lands = new Queue<Corner>();

            foreach (var corner in map.Corners.Values)
            {
                if (corner.States.Has(StateFlags.Shore))
                {
                    lands.Enqueue(corner);
                }
                else
                {
                    corner.Point = new Vector3(corner.Point.x, 999, corner.Point.z);
                }
            }
            
            var mapMaxHeight = 0f;
            while (lands.Any())
            {
                var c = lands.Dequeue();
                foreach (var a in c.Adjacents.Where(x => !x.States.Has(StateFlags.Water) && !x.States.Has(StateFlags.Shore)))
                {
                    var newElevation = (float)(!a.States.Has(StateFlags.Water)
                        ? c.Point.y * 1.07 + 1
                        : c.Point.y);

                    if (newElevation < a.Point.y)
                    {
                        a.Point = new Vector3(a.Point.x, (float)newElevation, a.Point.z);
                        if (newElevation > mapMaxHeight)
                            mapMaxHeight = newElevation;
                        lands.Enqueue(a);
                    }
                }
            }

            foreach (var center in map.Centers.Values)
            {
                foreach (var corner in center.Corners)
                {
                    corner.Point = new Vector3(
                        corner.Point.x, 
                        (corner.Point.y / mapMaxHeight) * 20, 
                        corner.Point.z);
                }
            }
            
            foreach (var corner in map.Corners.Values)
            {
                var sum = Vector3.zero;
                var count = 0;
                foreach (var ce in corner.Touches)
                {
                    foreach (var c in ce.Corners)
                    {
                        if (corner.Adjacents.Contains(c))
                        {
                            if (ce.Point.Area(corner.Point, c.Point) < 0)
                            {
                                sum += ce.Point.TriangleNormal(corner.Point, c.Point).normalized;
                            }
                            else
                            {
                                sum += ce.Point.TriangleNormal(c.Point, corner.Point).normalized;
                            }

                            count++;
                        }
                    }
                }
                corner.Normal = (sum/count).normalized;
            }

            foreach (var center in map.Centers.Values)
            {
                center.Point = new Vector3(center.Point.x, center.Corners.Sum(x => x.Point.y) / center.Corners.Count, center.Point.z);

                //TODO really need an vector3 average function
                var sum = Vector3.zero;
                foreach (var corner in center.Corners)
                {
                    sum += corner.Normal;
                }
                var ave = sum/center.Corners.Count;
                center.Normal = ave.normalized;
            }
        }

        public static void GenerateRivers(this Map map)
        {
            var riverCount = Random.Range(2, 4);

            for (int i = 0; i < riverCount; i++)
            {
                // Select a random non-shore corner
                // TODO ATIL: Can be selected a little further inland
                Corner randomCorner;
                do
                {
                    randomCorner = map.Corners.Values.ElementAt(Random.Range(0, map.Corners.Count));
                }
                while (!randomCorner.States.Has(StateFlags.Land) || randomCorner.States.Has(StateFlags.Shore));

                GameObject.CreatePrimitive(PrimitiveType.Sphere).transform.position = randomCorner.Point;

                // Percolate down until a shore is found
                for (Corner c = randomCorner, minAdj; !c.IsShore(); c = minAdj)
                {
                    // Find min among adjecents
                    minAdj = c.Adjacents.Aggregate((curMin, x) => x.Point.y < curMin.Point.y ? x : curMin);

                    var riverEdge = map.Edges[(c.Point + minAdj.Point).ToVector3xz() / 2f];
                    riverEdge.States = riverEdge.States.Add(StateFlags.River);
                    riverEdge.Flow += 1f;

                }
            }
        }
        #endregion
    }
}
