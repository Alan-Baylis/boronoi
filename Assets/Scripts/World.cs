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

        private List<LineSegment> edges;

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

            var start = System.DateTime.Now;
            var v = new Voronoi(points, colors, new Rect(0, 0, Width, Height));
            edges = v.VoronoiDiagram();
            Debug.Log("voronoi generation took [" + (DateTime.Now - start).Milliseconds + "] milisecs");

            foreach (var site in v.Sites())
            {
                var s = _factory.CenterFactory(site.Coord.ToVector3xz());
                foreach (var edge in site.edges)
                {
                    //var e = _factory.EdgeFactory(edge.) 
                }
            }
        }

        void Update()
        {
            foreach (var lineSegment in edges)
            {
                if (lineSegment.p0 != null & lineSegment.p1 != null)
                {
                    Debug.DrawLine((Vector3)lineSegment.p0, (Vector3)lineSegment.p1);
                }
            }
        }
    }
}
