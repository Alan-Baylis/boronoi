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
        public static void OrderCorners(this Center center)
        {
            var currentCorner = center.Corners.First();
            var ordered = new List<Corner>();
            Edge ed;

            ordered.Add(currentCorner);
            do
            {
                ed = currentCorner.Protrudes.FirstOrDefault(x => center.Borders.Contains(x) && !(ordered.Contains(x.Corners[0]) && (ordered.Contains(x.Corners[1]))));

                if (ed != null)
                {
                    var newdot = ed.Corners.FirstOrDefault(x => !ordered.Contains(x));
                    ordered.Add(newdot);
                    currentCorner = newdot;
                }
            } while (ed != null);

            center.Corners.Clear();

            foreach (var corner in ordered)
            {
                center.Corners.Add(corner);
            }
        }

        public static Color GetBiomeColor(this Center center)
        {
            if (center.States.Has(StateFlags.Land))
            {
                if (center.States.Has(StateFlags.Shore))
                    return new Color(239f / 255f, 221f / 255f, 111f / 255f);

                return new Color(0f / 255f, 155f / 255f, 0f / 255f); 
            }
            else if (center.States.Has(StateFlags.Water))
            {
                if (center.States.Has(StateFlags.Shore))
                    return new Color(0 / 50, 66f / 255f, 166f / 255f);

                return new Color(0f / 255f, 0 / 255f, 0f / 255f);
            }

            return new Color(0f / 255f, 155f / 255f, 0f / 255f);
        }
    }
}
