using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts;

namespace Assets.Helpers
{
    public static class CornerExtensions
    {
        public static bool IsShore(this Corner corner)
        {
            return corner.Touches.Any(x => x.Props.Has(ObjectProp.Land)) &&
                    corner.Touches.Any(x => x.Props.Has(ObjectProp.Water));
        }
    }
}
