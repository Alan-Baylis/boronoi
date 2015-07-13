using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Assets.Scripts;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace Assets.Helpers
{
    public static class EnumerationExtensions
    {
        #region StateFlags
        public static bool Has(this ObjectProp type, ObjectProp value)
        {
            return (type & value) == value;
        }

        public static bool Is(this ObjectProp type, ObjectProp value)
        {
            return type == value;
        }

        public static void Add(this ObjectProp type, ObjectProp value)
        {
             type |= value;
        }

        public static void Remove(this ObjectProp type, ObjectProp value)
        {
            type &= ~value;
        }
        #endregion

        #region Map
        public static void CreateMesh(this Map map)
        {
            var vertices = new List<Vector3>();
            var colors = new List<Color>();
            var triangles = new List<int>();
            var normals = new List<Vector3>();

            foreach (var center in map.Centers.Values.Where(x => x.Props.Has(ObjectProp.Land) || x.Props.Has(ObjectProp.ShallowWater)))
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
                var color = center.Biome.Color;
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
            
            // Instantiating things
            var go = new GameObject("Island");
            var rend = go.AddComponent<MeshRenderer>();
            rend.material = Resources.Load<Material>("UnityVC/VertexTerrain");
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
                if (corner.Props.Has(ObjectProp.Shore))
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
                foreach (var a in c.Adjacents.Where(x => !x.Props.Has(ObjectProp.Water) && !x.Props.Has(ObjectProp.Shore)))
                {
                    var newElevation = (float)(!a.Props.Has(ObjectProp.Water)
                        ? c.Point.y * 1.1 + Vector3.Distance(c.Point, a.Point) / 25
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
            var riverCount = Random.Range(5, 10);

            for (int i = 0; i < riverCount; i++)
            {
                // Select a random non-shore corner
                // TODO ATIL: Can be selected a little further inland
                Corner randomCorner;
                do
                {
                    randomCorner = map.Corners.Values.ElementAt(Random.Range(0, map.Corners.Count));
                }
                while (!randomCorner.Props.Has(ObjectProp.Land) || randomCorner.Props.Has(ObjectProp.Shore));

                //GameObject.CreatePrimitive(PrimitiveType.Sphere).transform.position = randomCorner.Point;

                // Percolate down until a shore is found
                for (Corner c = randomCorner, minAdj; !c.IsShore(); c = minAdj)
                {
                    // Find min among adjecents
                    minAdj = c.Adjacents.Aggregate((curMin, x) => x.Point.y < curMin.Point.y ? x : curMin);

                    var riverEdge = map.Edges[(c.Point + minAdj.Point).ToVector3xz() / 2f];
                    
                    riverEdge.Props.Add(ObjectProp.River);
                    riverEdge.DelaunayStart.Props.Add(ObjectProp.River);
                    riverEdge.DelaunayEnd.Props.Add(ObjectProp.River);
                    riverEdge.VoronoiStart.Props.Add(ObjectProp.River);
                    riverEdge.VoronoiEnd.Props.Add(ObjectProp.River);

                    riverEdge.VoronoiStart.Flow += 1f;
                    riverEdge.VoronoiEnd.Flow += 1f;
                }
            }
        }

        public static void GenerateMoisture(this Map map)
        {
            var queue = new Queue<Corner>();
            foreach (Corner q in map.Corners.Values.Where(q => 
                q.Props.Has(ObjectProp.Shore) || 
                q.Props.Has(ObjectProp.Water) ||
                q.Props.Has(ObjectProp.River)))
            {

                q.Moisture = (float) (q.Props.Has(ObjectProp.Shore) || q.Props.Has(ObjectProp.Water)
                    ? 0.5f
                    : (q.Flow > 0 ? Math.Max(1.0, (0.4 * q.Flow)) : 0.1));
                queue.Enqueue(q);
            }

            while (queue.Any())
            {
                var q = queue.Dequeue();

                foreach (Corner r in q.Adjacents.Where(x => x.Props.Has(ObjectProp.Land)))
                {
                    var newMoisture = q.Moisture * 
                        0.90f;
                    if (newMoisture > r.Moisture)
                    {
                        r.Moisture = newMoisture;
                        queue.Enqueue(r);
                    }
                }
            }

            foreach (var center in map.Centers.Values)
            {
                var sum = center.Corners.Sum(x => x.Moisture);
                center.Moisture = sum / center.Corners.Count;
            }
        }

        public static void GenerateBiome(this Map map)
        {
            foreach (var center in map.Centers.Values.Where(x => x.Props.Has(ObjectProp.Land)))
            {
                var bio = BiomeTypes.BiomeSelector(center.Props, 5000, center.Point.y, center.Moisture);
                center.Biome = bio;
            }
        }
        #endregion
    }
}
