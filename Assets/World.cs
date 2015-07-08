using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Assets.Helpers;
using Assets.Scripts;
using Delaunay;
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

        public World()
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
            var v = new Voronoi(points, colors, new Rect(0, 0, Width, Height));
            var edges = v.VoronoiDiagram();

            foreach (var site in v.Sites())
            {
                var s = _factory.CenterFactory(site.Coord.ToVector3xz());
                foreach (var edge in site.edges)
                {
                    //var e = _factory.EdgeFactory(edge.)       
                }
            }
        }
    }
}
