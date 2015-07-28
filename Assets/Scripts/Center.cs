using System;
using System.Collections.Generic;
using Assets.Helpers;
using UnityEngine;

namespace Assets.Scripts
{
    public class Center : MapObject
    {
        public Vector3 Point { get; set; }  

        public Dictionary<Vector3, Center> Neighbours { get; set; }
        public Dictionary<Vector3, Edge> Borders { get; set; }
        public Dictionary<Vector3, Corner> Corners { get; set; }

        public Vector3 Normal { get; set; }
        public float Moisture { get; set; }
        public Biome Biome { get; set; }

        public Center(Vector3 p)
        {
            Point = p;
            Neighbours = new Dictionary<Vector3, Center>(new Vector3Comparer());
            Borders = new Dictionary<Vector3, Edge>(new Vector3Comparer());
            Corners = new Dictionary<Vector3, Corner>(new Vector3Comparer());
            Props.Add(ObjectProp.Water);
        }

        public override string ToString()
        {
            return "Center: " + Point + "\n" +
                   "Corners/Edges/Neigh: " + Corners.Count + "/" + Borders.Count + "/" + Neighbours.Count  + "\n" +
                   "Moisture: " + Moisture + "\n" +
                   "Biome: " + Biome.Name + "\n";
        }
    }
}
