﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Assets.Helpers;
using Assets.Scripts;
using Assets.Scripts.Managers;
using Delaunay.Geo;
using LibNoise.Generator;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;
using KDTree;
using Voronoi = Delaunay.Voronoi;

namespace Assets
{
    public static class Globals
    {
        public static int Radius =32;
        public static int Height =2* Radius;
        public static float RowHeight =1.5f* Radius;
        public static float HalfWidth = (float)Math.Sqrt((Radius * Radius) - ((Radius /2) * (Radius /2)));
        public static float Width =2* HalfWidth;
        public static float ExtraHeight = Height - RowHeight;
        public static float Edge = RowHeight - ExtraHeight;
    }

    public class World : MonoBehaviour
    {
        public int Seed = 4;
        public int SiteCount = 300;
        public int Width = 100;
        public int Height = 100;
        public int SmoothingFactor;
        private DataFactory _factory;
        private Map _map;
        public Terrain Terrain;
        public KDTree<Center> _kd;
        private Texture2D _text;

        void Awake()
        {
            //Build();
        }

        [ExecuteInEditMode]
        public void Build()
        {
            _text = Resources.Load<Texture2D>("island");
            _kd = new KDTree<Center>(2);

            var sw = new Stopwatch();
            sw.Start();
            var v = GetVoronoi(Width, Height, Seed, SmoothingFactor);
            _map = CreateDataStructure(v, _kd);
            //_map = CreateHexagonalStructure();

            sw.Stop();
            Debug.Log(string.Format("Generation took [{0}] milisecs", sw.ElapsedMilliseconds));
            Debug.Log(_map.Centers.Count);
            Debug.Log(_map.Corners.Count);
            foreach (var tup in _map.Centers)
            {
                if (InLand(tup.Key, Width, Height, Seed))
                {
                    var center = tup.Value;
                    center.Props.Add(ObjectProp.Land);
                    center.Props.Remove(ObjectProp.Water);
                    foreach (var c in center.Corners.Values)
                    {
                        c.Props.Add(ObjectProp.Land);
                        c.Props.Remove(ObjectProp.Water);
                    }
                    foreach (var c in center.Borders.Values)
                    {
                        c.Props.Add(ObjectProp.Land);
                        c.Props.Remove(ObjectProp.Water);
                    }

                    _kd.AddPoint(new double[] { tup.Key.x, tup.Key.z }, tup.Value);
                }
            }

            var torem = new List<Center>();
            foreach (var center in _map.Centers.Values.Where(x => x.Props.Has(ObjectProp.Water)))
            {
                foreach (var b in center.Borders.Values.Where(x => x.VoronoiStart.Props.Has(ObjectProp.Water) && x.VoronoiEnd.Props.Has(ObjectProp.Water)))
                {
                    _map.Edges.Remove(b.Midpoint);
                }
                foreach (var c in center.Corners.Values.Where(x => x.Props.Has(ObjectProp.Water)))
                {
                    _map.Corners.Remove(c.Point);
                }
                torem.Add(center);
            }
            torem.ForEach(x => _map.Centers.Remove(x.Point));

            foreach (var edge in _map.Edges.Values.Where(x => x.DelaunayStart != null && x.DelaunayEnd != null))
            {
                //edge.IsShore uses site water/land flags
                if (edge.IsShore())
                {
                    _map.Shoreline.Add(edge);
                    edge.Props.Add(ObjectProp.Shore);
                    edge.VoronoiStart.Props.Add(ObjectProp.Shore);
                    edge.VoronoiEnd.Props.Add(ObjectProp.Shore);
                    edge.DelaunayStart.Props.Add(ObjectProp.Shore);
                    edge.DelaunayEnd.Props.Add(ObjectProp.Shore);
                }
            }

            _map.GenerateElevation();
            _map.GenerateRivers(_factory);
            _map.GenerateMoisture();
            _map.GenerateBiome();
            //var go = _map.CreateDiscreteMesh();
            var go = _map.CreateContinuousMesh();
            //var im = go.AddComponent<IslandManager>();
            //im.Init(_map);

            //TerrainStuff();
        }

        private void TerrainStuff()
        {
// Get a reference to the terrain data
            TerrainData terrainData = Terrain.terrainData;
            terrainData.splatPrototypes = new[]
            {
                new SplatPrototype
                {
                    tileSize = new Vector2(15, 15),
                    texture = Resources.Load<Texture2D>("water")
                },
                new SplatPrototype
                {
                    tileSize = new Vector2(15, 15),
                    texture = Resources.Load<Texture2D>("beach")
                },
                new SplatPrototype
                {
                    tileSize = new Vector2(15, 15),
                    texture = Resources.Load<Texture2D>("grass")
                }
            };

            // Splatmap data is stored internally as a 3d array of floats, so declare a new empty array ready for your custom splatmap data:
            var splatmapData = new float[2048, 2048, 3];

            for (int y = 0; y < 2048; y++)
            {
                for (int x = 0; x < 2048; x++)
                {
                    // Normalise x/y coordinates to range 0-1 
                    float yy = (float) y/2048;
                    float xx = (float) x/2048;

                    // Sample the height at this location (note GetHeight expects int coordinates corresponding to locations in the heightmap array)
                    //float height = terrainData.GetHeight(Mathf.RoundToInt(yy * terrainData.heightmapHeight), Mathf.RoundToInt(xx * terrainData.heightmapWidth));

                    //// Calculate the normal of the terrain (note this is in normalised coordinates relative to the overall terrain dimensions)
                    //Vector3 normal = terrainData.GetInterpolatedNormal(yy, xx);

                    //// Calculate the steepness of the terrain
                    //float steepness = terrainData.GetSteepness(yy, xx);

                    // Setup an array to record the mix of texture weights at this point
                    var splatWeights = new float[3];

                    var near = _kd.NearestNeighbors(new double[] {xx*10000, yy*10000}, 2);
                    near.MoveNext();
                    if (near.Current != null)
                    {
                        if (near.Current.Biome != null)
                        {
                            if (near.Current.Props.Has(ObjectProp.Land))
                            {
                                if (near.Current.Biome.Name == "Beach")
                                    splatWeights[1] = 1f;
                                else
                                    splatWeights[2] = 1f;
                            }
                            else
                            {
                                splatWeights[0] = 1f;
                            }
                        }
                    }

                    // CHANGE THE RULES BELOW TO SET THE WEIGHTS OF EACH TEXTURE ON WHATEVER RULES YOU WANT

                    //// Texture[1] is stronger at lower altitudes
                    //splatWeights[1] = Mathf.Clamp01((terrainData.heightmapHeight - height));

                    //// Texture[2] stronger on flatter terrain
                    //// Note "steepness" is unbounded, so we "normalise" it by dividing by the extent of heightmap height and scale factor
                    //// Subtract result from 1.0 to give greater weighting to flat surfaces
                    //splatWeights[2] = 1.0f - Mathf.Clamp01(steepness * steepness / (terrainData.heightmapHeight / 5.0f));

                    //// Texture[3] increases with height but only on surfaces facing positive Z axis 
                    //splatWeights[3] = height * Mathf.Clamp01(normal.z);

                    // Sum of all textures weights must add to 1, so calculate normalization factor from sum of weights
                    float z = splatWeights.Sum();

                    // Loop through each terrain texture
                    for (int i = 0; i < 3; i++)
                    {
                        // Normalize so that sum of all texture weights = 1
                        splatWeights[i] /= z;

                        // Assign this point to the splatmap array
                        splatmapData[y, x, i] = splatWeights[i];
                    }
                }
            }

            // Finally assign the new splatmap to the terrainData:
            terrainData.SetAlphamaps(0, 0, splatmapData);
        }
        
        private bool InLand(Vector3 p, int w, int h, int s)
        {
            //return IsLandShape(new Vector3((float) (2*(p.x/w - 0.5)), 0, (float) (2*(p.z/h - 0.5))), s);
            if (p.x < 0 || p.x > w || p.z < 0 || p.z > h)
                return false;

            if (_text.GetPixel((int)((p.x / 10000) * 1024), (int)((p.z / 10000) * 1024)).r > 0)
                return true;
            return false;
        }

        private bool IsLandShape(Vector3 v, int seed)
        {
            const double islandFactor = 1;
            Random.seed = seed;
            int bumps = Random.Range(1, 6);
            double startAngle = Random.Range(0f,1f) * 2 * Math.PI;
            double dipAngle = Random.Range(0f, 1f) * 2 * Math.PI;
            double dipWidth = Random.Range(2f, 7f) / 10;

            double angle = Math.Atan2(v.z, v.x);
            double length = 0.5 * (Math.Max(Math.Abs(v.x), Math.Abs(v.z)) + v.magnitude);

            double r1 = 0.5 + 0.40 * Math.Sin(startAngle + bumps * angle + Math.Cos((bumps + 3) * angle));
            double r2 = 0.7 - 0.20 * Math.Sin(startAngle + bumps * angle - Math.Sin((bumps + 2) * angle));
            if (Math.Abs(angle - dipAngle) < dipWidth
                || Math.Abs(angle - dipAngle + 2 * Math.PI) < dipWidth
                || Math.Abs(angle - dipAngle - 2 * Math.PI) < dipWidth)
            {
                r1 = r2 = 0.2;
            }
            return (length < r1 || (length > r1 * islandFactor && length < r2));
        }

        private Vector3 ToPixel(Vector2 hc)
        {
            var x = (hc.x * Globals.Width) + (((int)hc.y & 1) * Globals.Width / 2);
            return new Vector3(x, 0, (float)(hc.y * 1.5 * Globals.Radius));
        }

        private Map CreateHexagonalStructure()
        {
            var map = new Map();
            _factory = new DataFactory(map);

            for (int x = 0; x < Width/Globals.Width; x++)
            {
                for (int y = 0; y < Height/Globals.RowHeight; y++)
                {
                    var hexcord = new Vector2(x, y);
                    var pos = ToPixel(hexcord);

                    var v0 = pos + new Vector3(0, 0, -Globals.Radius);
                    var v1 = pos + new Vector3(Globals.HalfWidth, 0, -Globals.Radius/2);
                    var v2 = pos + new Vector3(Globals.HalfWidth, 0, Globals.Radius/2);
                    var v3 = pos + new Vector3(0, 0, Globals.Radius);
                    var v4 = pos + new Vector3(-Globals.HalfWidth, 0, Globals.Radius/2);
                    var v5 = pos + new Vector3(-Globals.HalfWidth, 0, -Globals.Radius/2);

                    var h = _factory.CenterFactory(pos);
                    var c0 = _factory.CornerFactory(v0);
                    var c1 = _factory.CornerFactory(v1);
                    var c2 = _factory.CornerFactory(v2);
                    var c3 = _factory.CornerFactory(v3);
                    var c4 = _factory.CornerFactory(v4);
                    var c5 = _factory.CornerFactory(v5);

                    h.Corners[v0] = c0;
                    h.Corners[v1] = c1;
                    h.Corners[v2] = c2;
                    h.Corners[v3] = c3;
                    h.Corners[v4] = c4;
                    h.Corners[v5] = c5;

                    _factory.EdgeFactory(c0, c1, null, h);
                    _factory.EdgeFactory(c1, c2, null, h);
                    _factory.EdgeFactory(c2, c3, null, h);
                    _factory.EdgeFactory(c3, c4, null, h);
                    _factory.EdgeFactory(c4, c5, null, h);
                    _factory.EdgeFactory(c5, c0, null, h);
                }
            }

            return map;
        }

        private Map CreateDataStructure(Voronoi v, KDTree<Center> kd)
        {
            var map = new Map();
            _factory = new DataFactory(map);
            foreach (var voronEdge in v.Edges())
            {
                if (voronEdge.leftSite == null
                    || voronEdge.rightSite == null
                    || voronEdge.leftVertex == null
                    || voronEdge.rightVertex == null)
                {
                    continue;
                }

                var centerLeft = _factory.CenterFactory(voronEdge.leftSite.Coord.ToVector3xz());
                var centerRight = _factory.CenterFactory(voronEdge.rightSite.Coord.ToVector3xz());
                var cornerLeft = _factory.CornerFactory(voronEdge.leftVertex.Coord.ToVector3xz());
                var cornerRight = _factory.CornerFactory(voronEdge.rightVertex.Coord.ToVector3xz());
                var ed = _factory.EdgeFactory(cornerLeft, cornerRight, centerLeft, centerRight);
            }

            foreach (var edge in map.Edges.Values)
            {
                edge.VoronoiStart.Protrudes[edge.Midpoint]= edge;
                edge.VoronoiEnd.Protrudes[edge.Midpoint]= edge;
                edge.DelaunayStart.Borders[edge.Midpoint]= edge;
                edge.DelaunayEnd.Borders[edge.Midpoint]= edge;
            }

            foreach (var corner in map.Corners.Values)
            {
                foreach (var edge in corner.Protrudes.Values)
                {
                    if (edge.VoronoiStart != corner)
                    {
                        corner.Adjacents[edge.VoronoiStart.Point]= edge.VoronoiStart;
                    }
                    if (edge.VoronoiEnd != corner)
                    {
                        corner.Adjacents[edge.VoronoiEnd.Point]= edge.VoronoiEnd;
                    }

                    // Adding one side of every protruder will make it all
                    if (!corner.Touches.ContainsKey(edge.DelaunayStart.Point))
                        corner.Touches[edge.DelaunayStart.Point]= edge.DelaunayStart;
                    if (!corner.Touches.ContainsKey(edge.DelaunayEnd.Point))
                        corner.Touches[edge.DelaunayEnd.Point]= edge.DelaunayEnd;
                }
            }

            foreach (var center in map.Centers.Values)
            {
                foreach (var edge in center.Borders.Values)
                {
                    if (edge.DelaunayStart != center)
                    {
                        center.Neighbours[edge.DelaunayStart.Point]= edge.DelaunayStart;
                    }
                    if (edge.DelaunayStart != center && !center.Neighbours.ContainsKey(edge.DelaunayStart.Point))
                    {
                        center.Neighbours[edge.DelaunayStart.Point]= edge.DelaunayStart;
                    }

                    // Adding one side of every border will make it all
                    if (!center.Corners.ContainsKey(edge.VoronoiStart.Point))
                        center.Corners[edge.VoronoiStart.Point]= edge.VoronoiStart;
                    if (!center.Corners.ContainsKey(edge.VoronoiEnd.Point))
                        center.Corners[edge.VoronoiEnd.Point]= edge.VoronoiEnd;
                }
            }

            return map;
        }

        private Voronoi GetVoronoi(int width, int height, int seed, int smoothingFactor)
        {
            var sw = new Stopwatch();
            sw.Start();

            var points = new List<Vector2>();
            Random.seed = seed;

            for (int i = 0; i < SiteCount; i++)
            {
                points.Add(new Vector2(
                    Random.Range(0f, width),
                    Random.Range(0f, height))
                    );
            }
            var v = new Voronoi(points, null, new Rect(0, 0, width, height));
            
            for (int i = 0; i < smoothingFactor; i++)
            {
                points = new List<Vector2>();
                foreach (var site in v.Sites())
                {
                    //brnkhy - voices are telling me that, this should use circumference, not average
                    var sum = Vector2.zero;
                    var count = 0;
                    foreach (var r in site.edges)
                    {
                        if (r.leftVertex != null)
                            sum += r.leftVertex.Coord;
                        if (r.rightVertex != null)
                            sum += r.rightVertex.Coord;
                        count += 2;
                    }
                    points.Add(sum/count);
                }

                v = new Voronoi(points, null, new Rect(0, 0, width, height));
            }
            sw.Stop();
            Debug.Log(string.Format("Voronoi generation took [{0}] milisecs with {1} smoothing iterations", sw.ElapsedMilliseconds, smoothingFactor));
            return v;
        }

        [ExecuteInEditMode]
        void Update()
        {
            //foreach (var center in _map.Centers.Values.Where(x => (x.States & StateFlags.Land) != 0))
            //{
            //    foreach (var edge in center.Borders)
            //    {
            //        Debug.DrawLine(edge.Corners[0].Point, edge.Corners[1].Point);
            //    }
            //}

            if (_map != null)
            {
                foreach (var edge in _map.Edges.Values)
                {
                    if (edge.Props.Has(ObjectProp.River))
                        Debug.DrawLine(edge.VoronoiStart.Point, edge.VoronoiEnd.Point, Color.blue);

                }
            }
        }
    }
}
