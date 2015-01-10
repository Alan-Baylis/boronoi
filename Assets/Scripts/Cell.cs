using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cell : MapObject
{
    public List<Edge> edges = new List<Edge>();
    public List<Corner> corners = new List<Corner>();

    public VoronoiNS.VoronoiCell BoronCell
    {
        get
        {
            return base.BoronObject as VoronoiNS.VoronoiCell;
        }
    }


    public Cell(MonoBehaviour boronObject) : base(boronObject) { }

}
