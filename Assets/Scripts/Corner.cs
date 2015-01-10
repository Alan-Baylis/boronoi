using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Corner : MapObject
{
    public List<Cell> cells = new List<Cell>();
    public List<Edge> edges = new List<Edge>();

    public VoronoiNS.VoronoiCellVertex BoronVertex
    {
        get
        {
            return base.BoronObject as VoronoiNS.VoronoiCellVertex;
        }
    }


    public Corner(MonoBehaviour boronObject) : base(boronObject) { }
}
