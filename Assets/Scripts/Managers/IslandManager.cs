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

            //var go = new GameObject("trees");
            //go.isStatic = true;
            //go.transform.parent = transform;
            //var tlist = new List<GameObject>();
            //foreach (var center in map.Centers.Values.Where(x => x.Biome != null && x.Biome.Name.Contains("Forest")))
            //{
            //    var cent = center.Point;
            //    for (int i = 0; i < center.Corners.Count - 1; i++)
            //    {
            //        var f = center.Corners.ElementAt(i + 1).Point;
            //        var s = center.Corners.ElementAt(i).Point;

            //        for (int j = 0; j < 5; j++)
            //        {
            //            var a = Random.Range(0f, 1f);
            //            var b = Random.Range(0f, 1f);
            //            double c = 0;
            //            float px, py, pz;
            //            if (a + b > 1)
            //            {
            //                a = 1 - a;
            //                b = 1 - b;
            //            }
            //            c = 1 - a - b;

            //            px = (float)((a * cent.x) + (b * f.x) + (c * s.x));
            //            py = (float)((a * cent.y) + (b * f.y) + (c * s.y));
            //            pz = (float)((a * cent.z) + (b * f.z) + (c * s.z));

            //            var tree = (GameObject)Instantiate(Resources.Load("tree_1"));
            //            tree.isStatic = true;
            //            tree.transform.position = new Vector3(px, py, pz);
            //            tree.transform.localScale = new Vector3(5, 5, 5);
            //            tree.transform.parent = go.transform;
            //            tlist.Add(tree);
            //        }
            //    }
            //}
            //StaticBatchingUtility.Combine(go);
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
