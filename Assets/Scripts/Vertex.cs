using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VoronoiNS;

public class Vertex : MapObject
{
    public List<Cell> cells = new List<Cell>();
    public List<Edge> edges = new List<Edge>();

    public VoronoiCellVertex VoronVertex { get; private set; }

    public Vertex(VoronoiCellVertex vrnVert)
    {
        VoronVertex = vrnVert;
    }

}
