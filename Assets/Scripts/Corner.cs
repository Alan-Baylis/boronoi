using System;
using System.Collections.Generic;
using Assets.Helpers;
using UnityEngine;

namespace Assets.Scripts
{
    public class Corner : MapObject
    {
        public Vector3 Point { get; set; }

        public Dictionary<Vector3, Center> Touches { get; set; }
        public Dictionary<Vector3, Edge> Protrudes { get; set; }
        public Dictionary<Vector3, Corner> Adjacents { get; set; }

        public Vector3 Normal { get; set; }
        public float Flow { get; set; }
        public float Moisture { get; set; }

        public Vector3 HighestNeighbour { get; set; }
        public Vector3 LowestNeighbour { get; set; }

        public Corner(Vector3 p)
        {
            Point = p;
            Touches = new Dictionary<Vector3, Center>(new Vector3Comparer());
            Protrudes = new Dictionary<Vector3, Edge>(new Vector3Comparer());
            Adjacents = new Dictionary<Vector3, Corner>(new Vector3Comparer());
            Props.Add(ObjectProp.Water);
        }
    }
}
