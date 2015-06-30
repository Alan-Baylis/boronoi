using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using VoronoiNS;

public class Map
{
    public List<Cell> cells = new List<Cell>();
    public List<Edge> edges = new List<Edge>();
    public List<Vertex> vertices = new List<Vertex>();

    public Map(VoronoiDiagram voronDiag)
    {
        var voronCells = voronDiag.cells.Where(c => c.serializedEdgeList != null).ToList();
        var voronEdges = voronDiag.edges.ToList();
        
        var voronVerts = new List<VoronoiCellVertex>();
        foreach (var edge in voronEdges)
        {
            if (edge.edgeVertexA != null && edge.edgeVertexB != null)
            {
                voronVerts.Add(edge.edgeVertexA);
                voronVerts.Add(edge.edgeVertexB);
            }
        }
        voronVerts = voronVerts.Distinct().ToList();

        voronCells.ForEach(vCell => cells.Add(new Cell(vCell)));
        voronEdges.ForEach(vEdge => edges.Add(new Edge(vEdge)));
        voronVerts.ForEach(vVert => vertices.Add(new Vertex(vVert)));

        edges.ForEach(edge =>
        {
            edge.cells = new List<Cell>()
            {
                cells.Find(c => c.VoronCell == edge.VoronEdge.cell1),
                cells.Find(c => c.VoronCell == edge.VoronEdge.cell2)
            };

            edge.vertices = new List<Vertex>()
            {
                vertices.Find(v => v.VoronVertex == edge.VoronEdge.edgeVertexA),
                vertices.Find(v => v.VoronVertex == edge.VoronEdge.edgeVertexB)
            };
        });

        vertices.ForEach(vertex =>
        {
            var a = vertex;

            vertex.cells = new List<Cell>()
            {
                cells.Find(c => c.VoronCell == vertex.VoronVertex.cell1),
                cells.Find(c => c.VoronCell == vertex.VoronVertex.cell2),
                cells.Find(c => c.VoronCell == vertex.VoronVertex.cell3),
            };

            vertex.edges = new List<Edge>()
            {
                edges.Find(e => e.VoronEdge == vertex.VoronVertex.edge1),
                edges.Find(e => e.VoronEdge == vertex.VoronVertex.edge2),
                edges.Find(e => e.VoronEdge == vertex.VoronVertex.edge3),
            };
        });

        cells.ForEach(cell =>
        {
            cell.edges = edges.FindAll(e => cell.VoronCell.serializedEdgeList.Contains(e.VoronEdge));
            cell.vertices = new List<Vertex>();
            cell.edges.ForEach(e =>
            {
                if (!cell.vertices.Contains(e.vertices[0]))
                {
                    cell.vertices.Add(e.vertices[0]);
                }
                if (!cell.vertices.Contains(e.vertices[1]))
                {
                    cell.vertices.Add(e.vertices[1]);
                }
            });
        });

    }
}