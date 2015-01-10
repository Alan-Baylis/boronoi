using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Map
{
    public List<Cell> cells = new List<Cell>();
    public List<Edge> edges = new List<Edge>();
    public List<Corner> corners = new List<Corner>();

    public Map(BoronoiDiagram boron)
    {
        foreach (var boronEdge in boron.edges)
        {
            var edge = new Edge(boronEdge);

            if (!cells.Exists(c => c.BoronCell == boronEdge.cell1))
            {
                var cell = new Cell(boronEdge.cell1);
                cells.Add(cell);
                edge.cells.Add(cell);
            }

            if (!cells.Exists(c => c.BoronCell == boronEdge.cell2))
            {
                var cell = new Cell(boronEdge.cell2);
                cells.Add(cell);
                edge.cells.Add(cell);
            }

            if (!corners.Exists(c => c.BoronVertex == boronEdge.edgeVertexA))
            {
                var corner = new Corner(boronEdge.edgeVertexA);
                corners.Add(corner);
                edge.corners.Add(corner);
            }

            if (!corners.Exists(c => c.BoronVertex == boronEdge.edgeVertexB))
            {
                var corner = new Corner(boronEdge.edgeVertexB);
                corners.Add(corner);
                edge.corners.Add(corner);
            }
        }


    }
}