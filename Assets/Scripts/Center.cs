using System;
using System.Collections.Generic;
using Assets.Helpers;
using UnityEngine;

namespace Assets.Scripts
{
    [Flags]
    public enum StateFlags
    {
        Land = 1,
        Water = 2,
        ShallowWater = 4,
        Shore = 8,
        River = 16
    }

    public class Center : IMapItem
    {
        public Vector3 Point { get; set; }  

        public HashSet<Center> Neighbours { get; set; }
        public HashSet<Edge> Borders { get; set; }
        public HashSet<Corner> Corners { get; set; }

        public StateFlags States { get; set; }

        public Center(Vector3 p)
        {
            Point = p;
            Neighbours = new HashSet<Center>(new CenterComparer());
            Borders = new HashSet<Edge>(new EdgeComparer());
            Corners = new HashSet<Corner>(new CornerComparer());
            States = States.Add(StateFlags.Water);
        }
    }
}
