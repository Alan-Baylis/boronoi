using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using KDTree;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Managers
{
    [ExecuteInEditMode]
    public class IslandManager : MonoBehaviour
    {
        public Transform CenterInfoText;
        private KDTree<Center> _kd;
        private Map _map;
        private List<GameObject> _balls;
        private Center _highlightedCenter;

        private Center HighlightedCenter
        {
            get { return _highlightedCenter; }
            set
            {
                _highlightedCenter = value;
                CenterInfoText.GetComponent<Text>().text = _highlightedCenter.ToString();
            }
        }

        public void Init(Map map)
        {
            _map = map;
            CenterInfoText = GameObject.Find("CenterInfo").transform;
            _balls = new List<GameObject>();
            for (int i = 0; i < 10; i++)
            {
                var s = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                s.transform.parent = transform;
                s.transform.localScale = new Vector3(40,40,40);
                s.GetComponent<MeshRenderer>().sharedMaterial.color = Color.red;
               _balls.Add(s); 
            }

            _kd = new KDTree<Center>(2);
            foreach (var center in map.Centers)
            {
                _kd.AddPoint(new double[]{center.Key.x, center.Key.z}, center.Value);
            }
        }

        [ExecuteInEditMode]
        void OnRenderObject()
        {
            //foreach (var edge in _map.Edges.Values)
            //{
            //    if (edge.Props.Has(ObjectProp.River))
            //        Debug.DrawLine(edge.VoronoiStart.Point, edge.VoronoiEnd.Point, Color.blue, 10000);

            //}

            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100000))
            {
                //Debug.DrawLine(hit.point, hit.point + Vector3.up * 1000, Color.red, 1);
                var query = _kd.NearestNeighbors(new double[] {hit.point.x, hit.point.z}, 1);
                query.MoveNext();
                var center = query.Current;

                if (center != HighlightedCenter)
                {
                    HighlightedCenter = center;
                    foreach (var ball in _balls)
                    {
                        ball.transform.position = new Vector3(0, -1000, 0);
                    }

                    for (int i = 0; i < HighlightedCenter.Corners.Count; i++)
                    {
                        var corner = HighlightedCenter.Corners.Values.ElementAt(i);
                        _balls[i].transform.position = corner.Point;
                    }
                }
            }
            
        }

        public KDTree<Center> GetKdTree()
        {
            return _kd;
        }
    }
}
