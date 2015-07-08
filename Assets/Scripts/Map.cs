﻿using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class Map
    {
        public Dictionary<Vector3,Center> Centers { get; set; }
        public Dictionary<Vector3, Corner> Corners { get; set; }
        public Dictionary<Vector3, Edge> Edges { get; set; }

        public Map()
        {
            Centers = new Dictionary<Vector3, Center>();
            Corners = new Dictionary<Vector3, Corner>();
            Edges = new Dictionary<Vector3, Edge>();
        }

    }
}