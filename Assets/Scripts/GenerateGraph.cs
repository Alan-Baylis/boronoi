using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Delaunay.Geo;
using VoronoiNS;

public class GenerateGraph : MonoBehaviour 
{
    public int numSitesToGenerate = 10;
    private List<Vector2> m_points;
    private List<LineSegment> m_edges = null;
    private float m_mapWidth = 20;
    private float m_mapHeight = 10;
    private Delaunay.Voronoi v;

	void Start () 
    {
		Generate ();
	}

	void Generate()
	{
		List<uint> colors = new List<uint>();
		m_points = new List<Vector2>();
		
		for (int i = 0; i < numSitesToGenerate; i++)
		{
			colors.Add(0);
			m_points.Add(new Vector2(Random.Range(0, m_mapWidth), Random.Range(0, m_mapHeight)));
		}
		v = new Delaunay.Voronoi(m_points, colors, new Rect(0, 0, m_mapWidth, m_mapHeight));
		m_edges = v.VoronoiDiagram();
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
        if (v == null)
            return;

        Gizmos.color = Color.red;
        if (m_points != null)
        {
            for (int i = 0; i < m_points.Count; i++)
            {
                Gizmos.DrawSphere(m_points[i], 0.02f);
            }
        }

        if (m_edges != null)
        {
            Gizmos.color = Color.gray;
            for (int i = 0; i < m_edges.Count; i++)
            {
                Vector2 left = (Vector2)m_edges[i].p0;
                Vector2 right = (Vector2)m_edges[i].p1;
                Gizmos.DrawLine((Vector3)left, (Vector3)right);
            }
        }
    }
}
