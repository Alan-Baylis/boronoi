using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Assets.Scripts;
using LibNoise.Generator;
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
        public static GameObject CreateDiscreteMesh(this Map map)
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
                foreach (var corner in center.Corners.Values)
                {
                    vertices.Add(corner.Point);
                    normals.Add(corner.Normal);
                }

                var lastVertex = 0;
                var color = center.Biome != null ? center.Biome.Color : Color.green;
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
                if (vertices.Count > 50000)
                {
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

                    vertices.Clear();
                    triangles.Clear();
                    colors.Clear();
                    normals.Clear();
                }
            }

            // Instantiating things
            var go2 = new GameObject("Island");
            var rend2 = go2.AddComponent<MeshRenderer>();
            rend2.material = Resources.Load<Material>("UnityVC/VertexTerrain");
            var mesh2 = new Mesh { name = "HexMesh" };
            var mcomp2 = go2.AddComponent<MeshFilter>();
            mcomp2.mesh = mesh2;
            mesh2.vertices = vertices.ToArray();
            mesh2.triangles = triangles.ToArray();
            mesh2.colors = colors.ToArray();
            mesh2.normals = normals.ToArray();

            go2.AddComponent<MeshCollider>();
            return go2;
        }

        public static GameObject CreateContinuousMesh(this Map map)
        {
            var vertices = new List<Vector3>();
            var colors = new List<Color>();
            var triangles = new List<int>();
            var normals = new List<Vector3>();
            var uvs = new List<Vector2>();

            var vertdic = new Dictionary<Vector3, int>();

            foreach (var center in map.Centers.Values)
            {
                center.OrderCorners();

                var vertindexes = new List<int>();
                vertindexes.Add(vertices.Count);
                vertices.Add(center.Point);
                normals.Add(center.Normal);
                uvs.Add(new Vector2(center.Point.x, center.Point.z));

                var color = center.Biome != null ? center.Biome.Color : Color.green;
                colors.Add(color);

                foreach (var corner in center.Corners.Values)
                {
                    if (!vertdic.ContainsKey(corner.Point))
                    {
                        vertdic.Add(corner.Point, vertices.Count);
                        vertindexes.Add(vertices.Count);
                        vertices.Add(corner.Point);
                        normals.Add(corner.Normal);
                        uvs.Add(new Vector2(corner.Point.x, corner.Point.z));
                        colors.Add(color);
                    }
                    else
                    {
                        vertindexes.Add(vertdic[corner.Point]);
                    }
                }

                for (int i = 0; i < vertindexes.Count - 1; i++)
                {
                    triangles.Add(vertindexes[0]);
                    triangles.Add(vertindexes[i+1]);
                    triangles.Add(vertindexes[i]);
                }

                triangles.Add(vertindexes[0]);
                triangles.Add(vertindexes[1]);
                triangles.Add(vertindexes[vertindexes.Count - 1]);

                if (vertices.Count > 50000)
                {
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
                    mesh.uv = uvs.ToArray();

                    vertices.Clear();
                    triangles.Clear();
                    colors.Clear();
                    normals.Clear();
                    uvs.Clear();
                    vertdic.Clear();
                }
            }

            // Instantiating things
            var go2 = new GameObject("Island");
            var rend2 = go2.AddComponent<MeshRenderer>();
            rend2.material = Resources.Load<Material>("UnityVC/VertexTerrain");
            var mesh2 = new Mesh { name = "HexMesh" };
            var mcomp2 = go2.AddComponent<MeshFilter>();
            mcomp2.mesh = mesh2;
            mesh2.vertices = vertices.ToArray();
            mesh2.triangles = triangles.ToArray();
            mesh2.colors = colors.ToArray();
            mesh2.normals = normals.ToArray();
            mesh2.uv = uvs.ToArray();

            go2.AddComponent<MeshCollider>();
            return go2;
        }

        public static void GenerateElevation(this Map map)
        {
            //var perlin = new Perlin();
            //perlin.OctaveCount = 1;
            //perlin.Frequency = 3;
            //perlin.Lacunarity = 2;
            var lands = new Queue<Corner>();
            var oceans = new Queue<Corner>();

            foreach (var corner in map.Corners.Values)
            {
                if (corner.Props.Has(ObjectProp.Shore))
                {
                    lands.Enqueue(corner);
                    oceans.Enqueue(corner);
                }
                else if (corner.Props.Has(ObjectProp.Land))
                {
                    //if (Random.Range(0f, 1f) > 0.97)
                    //{
                    //    corner.Point = new Vector3(corner.Point.x, (float)(perlin.GetValue(corner.Point) + 1) * 20, corner.Point.z);
                    //    lands.Enqueue(corner);
                    //}
                    //else
                    //{
                        corner.Point = new Vector3(corner.Point.x, 999, corner.Point.z);    
                    //}
                }
            }

            var mapMax = 0f;
            while (lands.Any())
            {
                var c = lands.Dequeue();
                foreach (var a in c.Adjacents.Values.Where(x => !x.Props.Has(ObjectProp.Water) && !x.Props.Has(ObjectProp.Shore)))
                {
                    var newElevation = (float)(a.Props.Has(ObjectProp.Land)
                        ? c.Point.y * (1.02 + Random.Range(0f,1f) / 20f) + Vector3.Distance(c.Point.ToVector2xz(), a.Point.ToVector2xz()) / 50
                        : c.Point.y);

                    if (newElevation < a.Point.y)
                    {
                        if (newElevation > mapMax)
                            mapMax = newElevation;
                        a.Point = new Vector3(a.Point.x, (float)newElevation, a.Point.z);
                        lands.Enqueue(a);
                    }
                }
            }
            
            foreach (var corner in map.Corners.Values)
            {
                var sum = Vector3.zero;
                var count = 0;
                foreach (var ce in corner.Touches.Values)
                {
                    foreach (var c in ce.Corners.Values)
                    {
                        if (corner.Adjacents.ContainsKey(c.Point))
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
                center.Point = new Vector3(center.Point.x, center.Corners.Values.Sum(x => x.Point.y) / center.Corners.Count, center.Point.z);

                //TODO really need an vector3 average function
                var sum = Vector3.zero;
                foreach (var corner in center.Corners.Values)
                {
                    sum += corner.Normal;
                }
                var ave = sum/center.Corners.Count;
                center.Normal = ave.normalized;
            }
        }

        public static void GenerateRivers(this Map map, DataFactory factory)
        {
            var riverCount = Random.Range(5, 10);

            for (int i = 0; i < riverCount; i++)
            {
                // Select a random non-shore corner
                // TODO ATIL: Can be selected a little further inland
                Corner randomCorner;
                var rivers = new List<Edge>();
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
                    minAdj = c.Adjacents.Values.Aggregate((curMin, x) => x.Point.y < curMin.Point.y ? x : curMin);
                    if (minAdj.Point.y > c.Point.y)
                        break;
                    
                    c.LowestNeighbour = minAdj.Point;
                    minAdj.HighestNeighbour = c.Point;

                    var riverEdge = map.Edges[(c.Point + minAdj.Point) / 2f];
                    rivers.Add(riverEdge);
                    riverEdge.Props.Add(ObjectProp.River);
                    riverEdge.DelaunayStart.Props.Add(ObjectProp.River);
                    riverEdge.DelaunayEnd.Props.Add(ObjectProp.River);
                    riverEdge.VoronoiStart.Props.Add(ObjectProp.River);
                    riverEdge.VoronoiEnd.Props.Add(ObjectProp.River);

                    riverEdge.VoronoiStart.Flow += 1f;
                    riverEdge.VoronoiEnd.Flow += 1f;
                }

                foreach (var river in rivers)
                {
                    river.DelaunayStart.PushEdge(river, factory);
                    river.DelaunayEnd.PushEdge(river, factory);
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

                foreach (Corner r in q.Adjacents.Values.Where(x => x.Props.Has(ObjectProp.Land)))
                {
                    var newMoisture = q.Moisture * 
                        (0.95f + (Random.Range(0f,1f) - 0.5) / 10);
                    if (newMoisture > r.Moisture)
                    {
                        r.Moisture = (float) newMoisture;
                        queue.Enqueue(r);
                    }
                }
            }

            foreach (var center in map.Centers.Values)
            {
                var sum = center.Corners.Values.Sum(x => x.Moisture);
                center.Moisture = sum / center.Corners.Count;
            }
        }

        public static void GenerateBiome(this Map map)
        {
            foreach (var center in map.Centers.Values.Where(x => x.Props.Has(ObjectProp.Land)))
            {
                var bio = BiomeTypes.BiomeSelector(center.Props, 1000, center.Point.y, center.Moisture);
                center.Biome = bio;
            }
        }
        #endregion
    }
}
