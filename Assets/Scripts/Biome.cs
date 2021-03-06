﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class Biome
    {
        public string Name { get; set; }
        public Color Color { get; set; }

        public Biome(string name, Color color)
        {
            Name = name;
            Color = color;
        }

        public Biome(string name, string color, byte alpha = 0)
        {
            Name = name;
            Color = HexToColor(color, alpha);
        }

        public static Color HexToColor(string hex, byte alpha = 0)
        {
            hex = hex.Replace("0x", "");//in case the string is formatted 0xFFFFFF
            hex = hex.Replace("#", "");//in case the string is formatted #FFFFFF
            byte a = alpha;//assume fully visible unless specified in hex
            byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            //Only use alpha if the string has enough characters
            if (hex.Length == 8)
            {
                a = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            }
            return new Color(r/255, g/255, b/255, a);
        }
    }

    public static class BiomeTypes
    {
        public static Biome Ocean = new Biome("Ocean", "ffffff");
        public static Biome ShallowWater = new Biome("ShallowWater", "ffffff");
        public static Biome Ice = new Biome("Ice", "ffffff");
        public static Biome Lake = new Biome("Lake", "ffffff");
        
        public static Biome Snow = new Biome("Snow", "ffffff");
        public static Biome Tundra = new Biome("Tundra", "ffffff"); //"c4ccbb"
        public static Biome Bare = new Biome("Beach", "ffffff");
        
        public static Biome Marsh = new Biome("Marsh", "ffffff");
        public static Biome Cliff = new Biome("Cliff", "ffffff");
        public static Biome Taiga = new Biome("Taiga", "ffffff"); //"ccd4bb"
        public static Biome Shrubland = new Biome("Shrubland", "ffffff");
        public static Biome TemperateDesert = new Biome("TemperateDesert", "ffffff");
        public static Biome TemperateRainForest = new Biome("TemperateRainForest", "ffffff"); //"a4c4a8"
        public static Biome TemperateDeciduousForest = new Biome("TemperateDeciduousForest", "ffffff"); //"b4c9a9"
        
        public static Biome SubtropicalDesert = new Biome("SubtropicalDesert", "ffffff");
        
        

        public static Biome Beach = new Biome("Beach", "000000", 0);
        public static Biome Grassland = new Biome("Grassland", "ff0000", 0);
        public static Biome TropicalSeasonalForest = new Biome("TropicalSeasonalForest", "ffff00", 0); //"558b70"
        public static Biome TropicalRainForest = new Biome("TropicalRainForest", "ffffff", 0); //"9cbba9"
        public static Biome Scorched = new Biome("Beach", "ffffff", 1);
        

        //public static Biome Ocean = new Biome("Ocean", "363661");
        //public static Biome ShallowWater = new Biome("ShallowWater", "364671");
        //public static Biome Ice = new Biome("Ice", "364671");
        //public static Biome Lake = new Biome("Lake", "364671");
        //public static Biome Beach = new Biome("Beach", "ac9f8b");
        //public static Biome Snow = new Biome("Snow", "FFFFFF");
        //public static Biome Tundra = new Biome("Tundra", "c4ccbb"); //"c4ccbb"
        //public static Biome Bare = new Biome("Beach", "bbbbbb");
        //public static Biome Scorched = new Biome("Beach", "999999");
        //public static Biome Marsh = new Biome("Marsh", "c4ccbb");
        //public static Biome Cliff = new Biome("Cliff", "8B4513");
        //public static Biome Taiga = new Biome("Taiga", "ccd4bb"); //"ccd4bb"
        //public static Biome Shrubland = new Biome("Shrubland", "99a68b");
        //public static Biome TemperateDesert = new Biome("TemperateDesert", "e4e8ca");
        //public static Biome TemperateRainForest = new Biome("TemperateRainForest", "a4c4a8"); //"a4c4a8"
        //public static Biome TemperateDeciduousForest = new Biome("TemperateDeciduousForest", "b4c9a9"); //"b4c9a9"
        //public static Biome Grassland = new Biome("Grassland", "99b470");
        //public static Biome SubtropicalDesert = new Biome("SubtropicalDesert", "e9ddc7");
        //public static Biome TropicalRainForest = new Biome("TropicalRainForest", "9cbba9"); //"9cbba9"
        //public static Biome TropicalSeasonalForest = new Biome("TropicalSeasonalForest", "ff0000"); //"558b70"

        public static Biome BiomeSelector(MapObjectState props, int mapHeight, float elevation, float moisture)
        {
            if (props.Has(ObjectProp.Water) && elevation < -0.1d)
            {
                return BiomeTypes.Ocean;
            }
            else if (props.Has(ObjectProp.Water) && elevation > -0.1d)
            {
                return BiomeTypes.ShallowWater;
            }
            else if (props.Has(ObjectProp.Water))
            {
                //if (elevation < 0.1 * mapHeight / 6)
                //{
                //    return BiomeTypes.Marsh;
                //}
                if (elevation > 0.8 * mapHeight / 12)
                {
                    return BiomeTypes.Ice;
                }
                else
                {
                    return BiomeTypes.Lake;
                }
            }
            else if (props.Has(ObjectProp.Shore))
            {
                return BiomeTypes.Beach;
            }
            else if (elevation > 0.45 * mapHeight)
            {
                if (moisture > 0.8)
                {
                    return BiomeTypes.Snow;
                }
                else if (moisture > 0.33)
                {
                    return BiomeTypes.Tundra;
                }
                else if (moisture > 0.16)
                {
                    return BiomeTypes.Bare;
                }
                else
                {
                    return BiomeTypes.Scorched;
                }
            }
            else if (elevation > 0.7 * mapHeight)
            {
                if (moisture > 0.66)
                {
                    return BiomeTypes.Taiga;
                }
                else if (moisture > 0.33)
                {
                    return BiomeTypes.Shrubland;
                }
                else
                {
                    return BiomeTypes.TemperateDesert;
                }
            }
            else if (elevation > 0.5 * mapHeight)
            {
                if (moisture > 0.83)
                {
                    return BiomeTypes.TemperateRainForest;
                }
                else if (moisture > 0.50)
                {
                    return BiomeTypes.TemperateDeciduousForest;
                }
                else if (moisture > 0.16)
                {
                    return BiomeTypes.Grassland;
                }
                else
                {
                    return BiomeTypes.TemperateDesert;
                }
            }
            else
            {
                if (moisture > 0.66)
                {
                    return BiomeTypes.TropicalRainForest;
                }
                else if (moisture > 0.33)
                {
                    return BiomeTypes.TropicalSeasonalForest;
                }
                else if (moisture > 0.16)
                {
                    return BiomeTypes.Grassland;
                }
                else
                {
                    return BiomeTypes.SubtropicalDesert;
                }
            }
        }
    }
}
