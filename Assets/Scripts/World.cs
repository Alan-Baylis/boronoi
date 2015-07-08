using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Assets.Helpers;
using Assets.Scripts;
using Delaunay;
using Delaunay.Geo;
using UnityEngine;

namespace Assets
{
    public class World : MonoBehaviour
    {
        public int SiteCount = 300;
        public int Width = 100;
        public int Height = 100;

        private Map _map;
        private DataFactory _factory;


        void Awake()
        {
            _map = new Map();
            _factory = new DataFactory(_map);

            var colors = new List<uint>();
            var points = new List<Vector2>();

            for (int i = 0; i < SiteCount; i++)
            {
                colors.Add(0);
                points.Add(new Vector2(
                        UnityEngine.Random.Range(0, Width),
                        UnityEngine.Random.Range(0, Height))
                );
            }

            var start = DateTime.Now;
            var v = new Voronoi(points, colors, new Rect(0, 0, Width, Height));
            var edges = v.VoronoiDiagram();
            Debug.Log("voronoi generation took [" + (DateTime.Now - start).Milliseconds + "] milisecs");

            foreach (var voronEdge in v.Edges())
            {
                if (voronEdge.leftSite == null
                    || voronEdge.rightSite == null
                    || voronEdge.leftVertex == null
                    || voronEdge.rightVertex == null)
                {
                    continue;
                }

                var centerLeft = _factory.CenterFactory(voronEdge.leftSite.Coord);
                var centerRight = _factory.CenterFactory(voronEdge.rightSite.Coord);
                var cornerLeft = _factory.CornerFactory(voronEdge.leftVertex.Coord);
                var cornerRight = _factory.CornerFactory(voronEdge.rightVertex.Coord);
                _factory.EdgeFactory(cornerLeft, cornerRight, centerLeft, centerRight);
            }

            foreach (var edge in _map.Edges.Values)
            {
                edge.VoronoiStart.Protrudes.Add(edge);
                edge.VoronoiEnd.Protrudes.Add(edge);
                edge.DelaunayStart.Borders.Add(edge);
                edge.DelaunayEnd.Borders.Add(edge);
            }

            foreach (var corner in _map.Corners.Values)
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
                }
            }

            foreach (var center in _map.Centers.Values)
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
                }
            }
        }

        void Update()
        {
            //foreach (var edge in _map.Edges.Values)
            //{
            //    Debug.DrawLine(edge.Corners[0].Point, edge.Corners[1].Point);
            //}

            //foreach (var corner in _map.Corners.Values)
            //{
            //    foreach (var edge in corner.Protrudes)
            //    {
            //        Debug.DrawLine(edge.Corners[0].Point, edge.Corners[1].Point);
            //    }
            //}

            //foreach (var center in _map.Centers.Values)
            //{
            //    foreach (var edge in center.Borders)
            //    {
            //        Debug.DrawLine(edge.Corners[0].Point, edge.Corners[1].Point);
            //    }
            //}
        }
    }
}
