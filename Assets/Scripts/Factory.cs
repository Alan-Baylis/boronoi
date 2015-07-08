using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Assets.Scripts
{
    class CenterComparer : IEqualityComparer<Center>
    {
        public CenterComparer() { }
        public bool Equals(Center x, Center y)
        {
            return GetHashCode(x) == GetHashCode(y);
        }

        public int GetHashCode(Center obj)
        {
            return obj.Point.GetHashCode();
        }
    }

    class CornerComparer : IEqualityComparer<Corner>
    {
        public CornerComparer() { }
        public bool Equals(Corner x, Corner y)
        {
            return x.Point.Equals(y.Point);
        }

        public int GetHashCode(Corner obj)
        {
            return obj.Point.GetHashCode();
        }
    }

    class EdgeComparer : IEqualityComparer<Edge>
    {
        public EdgeComparer() { }
        public bool Equals(Edge x, Edge y)
        {
            return x == y;
        }

        public int GetHashCode(Edge obj)
        {
            return obj.GetHashCode();
        }
    }

    public interface IFactory
    {
        Center CenterFactory();
        Edge EdgeFactory(Corner begin, Corner end, Center Left, Center Right);
        Corner CornerFactory(float ax, float ay, float az);
    }

    public class DataFactory
    {
        private Map _world;

        public DataFactory(Map world)
        {
            _world = world;
        }

        #region Implementation of IFactory

        public Center CenterFactory(Vector3 p)
        {
            if(_world.Centers.ContainsKey(p))
            {
                return _world.Centers[p];
            }
            else
            {
                var nc = new Center(p);
                _world.Centers.Add(nc.Point, nc);
                return nc;
            }
        }

        public Edge EdgeFactory(Corner begin, Corner end, Center Left, Center Right)
        {
            var p = (begin.Point + end.Point)/2;
            if (_world.Edges.ContainsKey(p))
            {
                return _world.Edges[p];
            }
            else
            {
                var nc = new Edge(begin, end, Left, Right);
                _world.Edges.Add(nc.Midpoint, nc);
                return nc;
            }
        }

        public Corner CornerFactory(Vector3 p)
        {
            if (_world.Corners.ContainsKey(p))
            {
                return _world.Corners[p];
            }
            else
            {
                var nc = new Corner(p);
                _world.Corners.Add(nc.Point, nc);
                return nc;
            }
        }

        public void RemoveEdge(Edge e)
        {
            _world.Edges.Remove(e.Midpoint);
        }

        public void RemoveCorner(Corner e)
        {
            _world.Corners.Remove(e.Point);
        }

        #endregion
    }
}
