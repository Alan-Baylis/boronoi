using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using VoronoiNS;

public class BoronoiDiagram 
{
    public readonly VoronoiCell[] cells;
    public readonly VoronoiCellEdge[] edges;
    public readonly VoronoiCellVertex[] corners;

    public BoronoiDiagram(VoronoiDiagram voron)
    {
        this.cells = voron.cells;
        this.edges = voron.edges;

        List<VoronoiCellVertex> verts = new List<VoronoiCellVertex>();
        foreach (var edge in voron.edges)
        {
            verts.Add(edge.edgeVertexA);
            verts.Add(edge.edgeVertexB);
        }

        corners = verts.Distinct().ToArray();
    }
}
