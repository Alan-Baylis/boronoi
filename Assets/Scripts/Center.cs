using System;
using System.Collections.Generic;
using Assets.Helpers;
using UnityEngine;

namespace Assets.Scripts
{
    public class Center : MapObject
    {
        public Vector3 Point { get; set; }  

        public HashSet<Center> Neighbours { get; set; }
        public HashSet<Edge> Borders { get; set; }
        public HashSet<Corner> Corners { get; set; }

        public Vector3 Normal { get; set; }
        public float Moisture { get; set; }
        public Biome Biome { get; set; }

        public Center(Vector3 p)
        {
            Point = p;
            Neighbours = new HashSet<Center>(new CenterComparer());
            Borders = new HashSet<Edge>(new EdgeComparer());
            Corners = new HashSet<Corner>(new CornerComparer());
            Props.Add(ObjectProp.Water);
        }
    }
}
