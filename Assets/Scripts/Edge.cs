using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VoronoiNS;

public class Edge : MapObject
{
    public List<Cell> cells = new List<Cell>();
    public List<Vertex> vertices = new List<Vertex>();

    public VoronoiCellEdge VoronEdge { get; private set; }

    public Edge(VoronoiCellEdge vrnEdge)
    {
        VoronEdge = vrnEdge;
    }
}
