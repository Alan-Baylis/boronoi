using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VoronoiNS;

public class Cell : MapObject
{
    public List<Edge> edges = new List<Edge>();
    public List<Vertex> vertices = new List<Vertex>();

    public VoronoiCell VoronCell { get; private set; }

    public Cell(VoronoiCell vrnCell)
    {
        VoronCell = vrnCell;
    }
}
