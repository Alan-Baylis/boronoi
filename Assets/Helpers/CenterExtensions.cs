using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts;
using UnityEngine;

namespace Assets.Helpers
{
    public static class CenterExtensions
    {
        public static void PushEdge(this Center c, Edge e, DataFactory factory)
        {
            var v1 = e.VoronoiStart;
            var v2 = e.VoronoiEnd;
            Vector3 v11 = Vector3.zero, v22 = Vector3.zero;

            Edge e11 = null;
            foreach (var oe in v1.Protrudes.Values)
            {
                var other = oe.OtherCorner(v1);
                if (other != v2 && other.Touches.ContainsKey(c.Point))
                {
                    if (oe.Props.Has(ObjectProp.River))
                    {
                        v11 = v1.Point + (c.Point - v1.Point).normalized * 20;
                    }
                    else
                    {
                        v11 = v1.Point + (other.Point - v1.Point).normalized * 20;
                        e11 = oe;
                    }
                }
            }

            Edge e22 = null;
            foreach (var oe in v2.Protrudes.Values)
            {
                var other = oe.OtherCorner(v2);
                if (other != v1 && other.Touches.ContainsKey(c.Point))
                {
                    if (oe.Props.Has(ObjectProp.River))
                    {
                        v22 = v2.Point + (c.Point - v2.Point).normalized * 20;
                    }
                    else
                    {
                        v22 = v2.Point + (other.Point - v2.Point).normalized * 20;
                        e22 = oe;
                    }
                }
            }
            
            c.Corners.Remove(v1.Point);
            c.Corners.Remove(v2.Point);
            
            var nc11 = factory.CornerFactory(v11);
            nc11.Normal = Vector3.up;
            if(!nc11.Touches.ContainsKey(c.Point))
                nc11.Touches.Add(c.Point,c);
            if(!c.Corners.ContainsKey(nc11.Point))
                c.Corners.Add(nc11.Point, nc11);
            
            var nc22 = factory.CornerFactory(v22);
            nc22.Normal = Vector3.up;
            if (!nc22.Touches.ContainsKey(c.Point))
                nc22.Touches.Add(c.Point, c);
            if (!c.Corners.ContainsKey(nc22.Point))
                c.Corners.Add(nc22.Point, nc22);

            c.Borders.Remove(e.Midpoint);
            var ee = factory.EdgeFactory(nc11, nc22, c, c);
            if(!nc11.Protrudes.ContainsKey(ee.Midpoint))
                nc11.Protrudes.Add(ee.Midpoint, ee);
            if (!nc22.Protrudes.ContainsKey(ee.Midpoint))
                nc22.Protrudes.Add(ee.Midpoint, ee);
            if(!c.Borders.ContainsKey(ee.Midpoint))
                c.Borders.Add(ee.Midpoint, ee); 

            if (e11 != null && nc11.Point != Vector3.zero)
            {
                Edge ne = null;
                if (e11.VoronoiStart == v1)
                {
                    ne = factory.EdgeFactory(nc11, e11.VoronoiEnd, e11.DelaunayStart, e11.DelaunayEnd);
                    e11.VoronoiEnd.Protrudes.Remove(e11.Midpoint);
                    if (!e11.VoronoiEnd.Protrudes.ContainsKey(ne.Midpoint))
                        e11.VoronoiEnd.Protrudes.Add(ne.Midpoint, ne);
                }
                else
                {
                    ne = factory.EdgeFactory(e11.VoronoiStart, nc11, e11.DelaunayStart, e11.DelaunayEnd);
                    e11.VoronoiStart.Protrudes.Remove(e11.Midpoint);
                    if(!e11.VoronoiStart.Protrudes.ContainsKey(ne.Midpoint))
                        e11.VoronoiStart.Protrudes.Add(ne.Midpoint, ne);
                }
                if(!nc11.Protrudes.ContainsKey(ne.Midpoint))
                    nc11.Protrudes.Add(ne.Midpoint, ne);
                if(!c.Borders.ContainsKey(ne.Midpoint))
                    c.Borders.Add(ne.Midpoint, ne);
                c.Borders.Remove(e11.Midpoint);
            }

            if (e22 != null && nc22.Point != Vector3.zero)
            {
                Edge ne = null;
                if (e22.VoronoiStart == v2)
                {
                    ne = factory.EdgeFactory(nc22, e22.VoronoiEnd, e22.DelaunayStart, e22.DelaunayEnd);
                    e22.VoronoiEnd.Protrudes.Remove(e22.Midpoint);
                    if (!e22.VoronoiEnd.Protrudes.ContainsKey(ne.Midpoint))
                        e22.VoronoiEnd.Protrudes.Add(ne.Midpoint, ne);
                }
                else
                {
                    ne = factory.EdgeFactory(e22.VoronoiStart, nc22, e22.DelaunayStart, e22.DelaunayEnd);
                    e22.VoronoiStart.Protrudes.Remove(e22.Midpoint);
                    if (!e22.VoronoiStart.Protrudes.ContainsKey(ne.Midpoint))
                        e22.VoronoiStart.Protrudes.Add(ne.Midpoint, ne);
                }
                if(!nc22.Protrudes.ContainsKey(ne.Midpoint))
                    nc22.Protrudes.Add(ne.Midpoint, ne);
                if(!c.Borders.ContainsKey(ne.Midpoint))
                    c.Borders.Add(ne.Midpoint, ne);
                c.Borders.Remove(e22.Midpoint);
            }
        }

        public static void OrderCorners(this Center center)
        {
            var list = center.Corners.Values.ToList();

            center.Corners.Clear();
            foreach (var s in list.OrderByDescending(x => Math.Atan2(x.Point.x - center.Point.x, x.Point.z - center.Point.z)))
            {
                center.Corners.Add(s.Point, s);
            }

            //var currentCorner = center.Corners.Values.First();
            //var ordered = new List<Corner>();
            //Edge ed;

            //ordered.Add(currentCorner);
            //do
            //{
            //    ed = currentCorner.Protrudes.FirstOrDefault(
            //        x => center.Borders.ContainsKey(x.Key) && 
            //            !(ordered.Contains(x.Value.Corners[0]) && 
            //            (ordered.Contains(x.Value.Corners[1])))).Value;

            //    if (ed != null)
            //    {
            //        var newdot = ed.Corners.FirstOrDefault(x => !ordered.Contains(x));
            //        ordered.Add(newdot);
            //        currentCorner = newdot;
            //    }
            //} while (ed != null);

            //center.Corners.Clear();

            //foreach (var corner in ordered)
            //{
            //    center.Corners.Add(corner.Point, corner);
            //}
        }

        public static Color GetBiomeColor(this Center center)
        {
            if (center.Props.Has(ObjectProp.Land))
            {
                //brnkhy yields moisture map
                var moisture = center.Corners.Values.Average(x => x.Moisture);
                return new Color(0f / 255f, 1f / 255f, moisture);

                if (center.Props.Has(ObjectProp.Shore))
                {
                    return new Color(239f / 255f, 221f / 255f, 111f / 255f);
                }

                return new Color(0f / 255f, 155f / 255f, 0f / 255f); 
            }
            else if (center.Props.Has(ObjectProp.Water))
            {
                if (center.Props.Has(ObjectProp.Shore))
                {
                    return new Color(0 / 50, 66f / 255f, 166f / 255f);
                }

                return new Color(0f / 255f, 0 / 255f, 0f / 255f);
            }

            return new Color(0f / 255f, 155f / 255f, 0f / 255f);
        }
    }
}
