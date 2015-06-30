using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Delaunay.Geo;
using VoronoiNS;

public class GenerateGraph : MonoBehaviour 
{
    public int numSitesToGenerate = 10;
    private List<Vector2> points;
    private List<LineSegment> edges = null;
    private const float MapWidth = 20;
    private const float MapHeight = 10;
    private Delaunay.Voronoi voron;

	void Start() 
    {
		Generate();
	}

	void Generate()
	{
		List<uint> colors = new List<uint>();
		points = new List<Vector2>();
		
		for (int i = 0; i < numSitesToGenerate; i++)
		{
			colors.Add(0);
			points.Add(new Vector2(Random.Range(0, MapWidth), Random.Range(0, MapHeight)));
		}

		voron = new Delaunay.Voronoi(points, colors, new Rect(0, 0, MapWidth, MapHeight));
		edges = voron.VoronoiDiagram();
	    
        var voronDiag = VoronoiDiagram.CreateDiagramFromVoronoiOutput(voron, false);
	    var map = new Map(voronDiag);

	}

	void Update()
	{
		if(Input.GetKeyDown(KeyCode.A))
		{
			Generate ();
		}
	}

    void OnDrawGizmos()
    {
        if (voron == null)
        {
            return;
        }

        Gizmos.color = Color.red;
        if (points != null)
        {
            for (int i = 0; i < points.Count; i++)
            {
                Gizmos.DrawSphere(points[i], 0.02f);
            }
        }

        if (edges != null)
        {
            Gizmos.color = Color.gray;
            for (int i = 0; i < edges.Count; i++)
            {
                Vector2 left = (Vector2)edges[i].p0;
                Vector2 right = (Vector2)edges[i].p1;
                Gizmos.DrawLine(left, right);
            }
        }
    }
}
