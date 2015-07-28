using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts;

namespace Assets.Helpers
{
    public static class EdgeExtensions
    {
        public static bool IsShore(this Edge edge)
        {
            return (edge.DelaunayStart.Props.Has(ObjectProp.Water) &&
                    edge.DelaunayEnd.Props.Has(ObjectProp.Land)) ||
                   (edge.DelaunayEnd.Props.Has(ObjectProp.Water) &&
                    edge.DelaunayStart.Props.Has(ObjectProp.Land));
        }

        public static Corner OtherCorner(this Edge edge, Corner c)
        {
            return edge.VoronoiStart == c ? edge.VoronoiEnd : edge.VoronoiStart;
        }
    }
}
