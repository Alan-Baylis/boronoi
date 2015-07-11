using System;
using System.Collections.Generic;
using Assets.Helpers;
using UnityEngine;

namespace Assets.Scripts
{
    public class Corner : MapObject
    {
        public Vector3 Point { get; set; }

        public HashSet<Center> Touches { get; set; }
        public HashSet<Edge> Protrudes { get; set; }
        public HashSet<Corner> Adjacents { get; set; }

        public Vector3 Normal { get; set; }

        public Corner(Vector3 p)
        {
            Point = p;
            Touches = new HashSet<Center>(new CenterComparer());
            Protrudes = new HashSet<Edge>(new EdgeComparer());
            Adjacents = new HashSet<Corner>(new CornerComparer());
            Props.Add(ObjectProp.Water);
        }
    }
}
