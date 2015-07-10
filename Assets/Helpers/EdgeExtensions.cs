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
            return (edge.DelaunayStart.States.Has(StateFlags.Water) &&
                    edge.DelaunayEnd.States.Has(StateFlags.Land)) ||
                   (edge.DelaunayEnd.States.Has(StateFlags.Water) &&
                    edge.DelaunayStart.States.Has(StateFlags.Land));
        }
    }
}
