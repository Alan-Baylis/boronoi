using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Assets.Helpers;
using Assets.Scripts;
using Delaunay;
using Delaunay.Geo;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace Assets
{
    public class World : MonoBehaviour
    {
        public int Seed = 4;
        public int SiteCount = 300;
        public int Width = 100;
        public int Height = 100;
        public int SmoothingFactor;
        private DataFactory _factory;
        private Map _map;

        void Awake()
        {
            var v = GetVoronoi(Width, Height, Seed, SmoothingFactor);
            _map = CreateDataStructure(v);
            
            foreach (var tup in _map.Centers.Where(x => InLand(x.Key,Width,Height,Seed)))
            {
                var center = tup.Value;
                center.Props.Add(ObjectProp.Land);
                center.Props.Remove(ObjectProp.Water);
                foreach (var c in center.Corners)
                {
                    c.Props.Add(ObjectProp.Land);
                    c.Props.Remove(ObjectProp.Water);
                }
                foreach (var c in center.Borders)
                {
                    c.Props.Add(ObjectProp.Land);
                    c.Props.Remove(ObjectProp.Water);
                }
            }
            
            foreach (var edge in _map.Edges.Values)
            {
                //edge.IsShore uses site water/land flags
                if (edge.IsShore())
                {
                    _map.Shoreline.Add(edge);
                    edge.Props.Add(ObjectProp.Shore);
                    edge.VoronoiStart.Props.Add(ObjectProp.Shore);
                    edge.VoronoiEnd.Props.Add(ObjectProp.Shore);
                    edge.DelaunayStart.Props.Add(ObjectProp.Shore);
                    edge.DelaunayEnd.Props.Add(ObjectProp.Shore);
                }
            }

            _map.GenerateElevation();
            _map.GenerateRivers();
            _map.CreateMesh();
        }


        private bool InLand(Vector3 p, int w, int h, int s)
        {
            return IsLandShape(new Vector3((float) (2*(p.x/w - 0.5)), 0, (float) (2*(p.z/h - 0.5))), s);
        }

        private bool IsLandShape(Vector3 v, int seed)
        {
            const double islandFactor = 1.02;
            Random.seed = seed;
            int bumps = Random.Range(1, 6);
            double startAngle = Random.Range(0f,1f) * 2 * Math.PI;
            double dipAngle = Random.Range(0f, 1f) * 2 * Math.PI;
            double dipWidth = Random.Range(2f, 7f) / 10;

            double angle = Math.Atan2(v.z, v.x);
            double length = 0.5 * (Math.Max(Math.Abs(v.x), Math.Abs(v.z)) + v.magnitude);

            double r1 = 0.5 + 0.40 * Math.Sin(startAngle + bumps * angle + Math.Cos((bumps + 3) * angle));
            double r2 = 0.7 - 0.20 * Math.Sin(startAngle + bumps * angle - Math.Sin((bumps + 2) * angle));
            if (Math.Abs(angle - dipAngle) < dipWidth
                || Math.Abs(angle - dipAngle + 2 * Math.PI) < dipWidth
                || Math.Abs(angle - dipAngle - 2 * Math.PI) < dipWidth)
            {
                r1 = r2 = 0.2;
            }
            return (length < r1 || (length > r1 * islandFactor && length < r2));
        }

        private Map CreateDataStructure(Voronoi v)
        {
            var map = new Map();
            _factory = new DataFactory(map);
            foreach (var voronEdge in v.Edges())
            {
                if (voronEdge.leftSite == null
                    || voronEdge.rightSite == null
                    || voronEdge.leftVertex == null
                    || voronEdge.rightVertex == null)
                {
                    continue;
                }

                var centerLeft = _factory.CenterFactory(voronEdge.leftSite.Coord.ToVector3xz());
                var centerRight = _factory.CenterFactory(voronEdge.rightSite.Coord.ToVector3xz());
                var cornerLeft = _factory.CornerFactory(voronEdge.leftVertex.Coord.ToVector3xz());
                var cornerRight = _factory.CornerFactory(voronEdge.rightVertex.Coord.ToVector3xz());
                _factory.EdgeFactory(cornerLeft, cornerRight, centerLeft, centerRight);
            }

            foreach (var edge in map.Edges.Values)
            {
                edge.VoronoiStart.Protrudes.Add(edge);
                edge.VoronoiEnd.Protrudes.Add(edge);
                edge.DelaunayStart.Borders.Add(edge);
                edge.DelaunayEnd.Borders.Add(edge);
            }

            foreach (var corner in map.Corners.Values)
            {
                foreach (var edge in corner.Protrudes)
                {
                    if (edge.VoronoiStart != corner)
                    {
                        corner.Adjacents.Add(edge.VoronoiStart);
                    }
                    if (edge.VoronoiEnd != corner)
                    {
                        corner.Adjacents.Add(edge.VoronoiEnd);
                    }

                    // Adding one side of every protruder will make it all
                    corner.Touches.Add(edge.DelaunayStart);
                    corner.Touches.Add(edge.DelaunayEnd);
                }
            }

            foreach (var center in map.Centers.Values)
            {
                foreach (var edge in center.Borders)
                {
                    if (edge.DelaunayStart != center)
                    {
                        center.Neighbours.Add(edge.DelaunayStart);
                    }
                    if (edge.DelaunayStart != center)
                    {
                        center.Neighbours.Add(edge.DelaunayStart);
                    }

                    // Adding one side of every border will make it all
                    center.Corners.Add(edge.VoronoiStart);
                    center.Corners.Add(edge.VoronoiEnd);
                }
            }
            return map;
        }

        private Voronoi GetVoronoi(int width, int height, int seed, int smoothingFactor)
        {
            var sw = new Stopwatch();
            sw.Start();

            var points = new List<Vector2>();
            Random.seed = seed;

            for (int i = 0; i < SiteCount; i++)
            {
                points.Add(new Vector2(
                    Random.Range(0f, width),
                    Random.Range(0f, height))
                    );
            }
            var v = new Voronoi(points, null, new Rect(0, 0, width, height));
            
            for (int i = 0; i < smoothingFactor; i++)
            {
                points = new List<Vector2>();
                foreach (var site in v.Sites())
                {
                    //brnkhy - voices are telling me that, this should use circumference, not average
                    var sum = Vector2.zero;
                    var count = 0;
                    foreach (var r in site.edges)
                    {
                        if (r.leftVertex != null)
                            sum += r.leftVertex.Coord;
                        if (r.rightVertex != null)
                            sum += r.rightVertex.Coord;
                        count += 2;
                    }
                    points.Add(sum/count);
                }

                v = new Voronoi(points, null, new Rect(0, 0, width, height));
            }
            sw.Stop();
            Debug.Log(string.Format("Voronoi generation took [{0}] milisecs with {1} smoothing iterations", sw.ElapsedMilliseconds, smoothingFactor));
            return v;
        }

        void Update()
        {
            //foreach (var center in _map.Centers.Values.Where(x => (x.States & StateFlags.Land) != 0))
            //{
            //    foreach (var edge in center.Borders)
            //    {
            //        Debug.DrawLine(edge.Corners[0].Point, edge.Corners[1].Point);
            //    }
            //}

            //foreach (var edge in _map.Edges.Values)
            //{
            //    if(edge.States.Has(StateFlags.Shore))
            //        Debug.DrawLine(edge.VoronoiStart.Point, edge.VoronoiEnd.Point, Color.red);

            //}
        }
    }
}
