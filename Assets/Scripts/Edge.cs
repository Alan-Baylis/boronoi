using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Edge : MapObject
{
    public List<Cell> cells = new List<Cell>();
    public List<Corner> corners = new List<Corner>();

    public VoronoiNS.VoronoiCellEdge BoronEdge
    {
        get
        {
            return base.BoronObject as VoronoiNS.VoronoiCellEdge;
        }
    }


    public Edge(MonoBehaviour boronObject) : base(boronObject) { }

}
