using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Assets.Scripts;
using UnityEngine;

namespace Assets.Helpers
{
    public static class EnumerationExtensions
    {
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

        public static void CreateMesh(this Map map)
        {
            var vertices = new List<Vector3>();
            var colors = new List<Color>();
            var triangles = new List<int>();

            foreach (var center in map.Centers.Values.Where(x => x.States.Has(StateFlags.Land) || x.States.Has(StateFlags.ShallowWater)))
            {
                //fuck this
                center.OrderCorners();

                var startingIndex = vertices.Count;
                vertices.Add(center.Point);
                vertices.AddRange(center.Corners.Select((t, i) => center.Corners.ElementAt(i)).Select(corner => corner.Point));
                var lastVertex = 0;
                var color = center.GetBiomeColor();

                for (int i = startingIndex; i < startingIndex + center.Corners.Count; i++)
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

            var go = new GameObject();
            var rend = go.AddComponent<MeshRenderer>();
            rend.material.shader = Resources.Load<Shader>("VertexColor");
            var mesh = new Mesh { name = "HexMesh" };
            var mcomp = go.AddComponent<MeshFilter>();
            mcomp.mesh = mesh;
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.colors = colors.ToArray();
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

            foreach (var center in map.Centers.Values)
            {
                center.Point = new Vector3(center.Point.x, center.Corners.Sum(x => x.Point.y) / center.Corners.Count, center.Point.z);
            }
        }
    }
}
