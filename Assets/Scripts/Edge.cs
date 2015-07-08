using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class Edge : IMapItem
    {
        public int Index { get; set; }
        public Vector3 Midpoint { get; set; }
        public Center DelaunayStart { get; set; }
        public Center DelaunayEnd { get; set; }// Delaunay edge
        public Corner VoronoiStart { get; set; }
        public Corner VoronoiEnd { get; set; }// Voronoi edge
        public Corner[] Corners { get { return new Corner[] { VoronoiStart, VoronoiEnd }; } }

        public Edge(Corner begin, Corner end, Center left, Center right)
        {
            VoronoiStart = begin;
            VoronoiEnd = end;
            DelaunayStart = left;
            DelaunayEnd = right;
        }

        public Edge(int index, Corner begin, Corner end)
        {
            VoronoiStart = begin;
            VoronoiEnd = end;
        }
    }
}
