using System;
using UnityEngine;


namespace TotemUtils
{
    public static class ColorConverter
    {

        static int HexToDec(string value)
        {
            int val = Convert.ToInt32(value, 16);
            return val;
        }

        static string DecToHex(int value)
        {
            return value.ToString("X2");
        }


        static string IntToHex(float value)
        {
            return DecToHex(Mathf.RoundToInt(value * 255f));
        }
        
        static float HexToInt(string hexcode)
        {
            return HexToDec(hexcode) / 255f;
        }

        public static Color GetColorByHexCode(string hexcode)
        {
            float r, g, b;

            r = HexToInt(hexcode.Substring(1,2));
            g = HexToInt(hexcode.Substring(3,2));
            b = HexToInt(hexcode.Substring(5,2));

            return new Color(r, g, b);
        }

        public static string GetHexCodeByColor(Color color)
        {
            string r = IntToHex(color.r);
            string g = IntToHex(color.g);
            string b = IntToHex(color.b);

            return $"{r}{g}{b}";
        }
        
    }
}