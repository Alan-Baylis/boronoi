using System;
using Assets.Helpers;
using UnityEngine;

namespace Assets.Scripts
{
    public class Edge : MapObject
    {
        public int Index { get; set; }
        public Vector3 Midpoint { get; private set; }
        public Center DelaunayStart { get; private set; }
        public Center DelaunayEnd { get; private set; }// Delaunay edge
        public Corner VoronoiStart { get; private set; }
        public Corner VoronoiEnd { get; private set; }// Voronoi edge
        public Corner[] Corners { get { return new[] { VoronoiStart, VoronoiEnd }; } }

        public float Flow { get; set; }

        public Edge(Corner begin, Corner end, Center left, Center right)
        {
            VoronoiStart = begin;
            VoronoiEnd = end;
            DelaunayStart = left;
            DelaunayEnd = right;
            Midpoint = (begin.Point + end.Point) / 2f;
            Props.Add(ObjectProp.Water);
        }

        public Edge(int index, Corner begin, Corner end)
        {
            VoronoiStart = begin;
            VoronoiEnd = end;
            Props.Add(ObjectProp.Water);
        }
    }
}
