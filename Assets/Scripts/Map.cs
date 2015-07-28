using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class Map
    {
        public Dictionary<Vector3, Center> Centers { get; set; }
        public Dictionary<Vector3, Corner> Corners { get; set; }
        public Dictionary<Vector3, Edge> Edges { get; set; }

        public List<Edge> Shoreline { get; set; }

        public Map()
        {
            Centers = new Dictionary<Vector3, Center>(new Vector3Comparer());
            Corners = new Dictionary<Vector3, Corner>(new Vector3Comparer());
            Edges = new Dictionary<Vector3, Edge>(new Vector3Comparer());

            Shoreline = new List<Edge>();
        }

    }
}
