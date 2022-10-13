using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace TotemServices.DNA
{
    public enum TotemDNAType { Undefined, Bool, Int, Map, Color, Range }

    public class TotemDNAFilterItem
    {
        public string description;
        public string id;
        public string type;
        public int gene;
        public int start;
        public int length;

        public List<TotemDNAFilterMapPair> values;
    }

    public class TotemDNAFilterMapPair
    {
        public object value;
        public string key;
    }


    public class TotemDNAFilter
    {
        private List<TotemDNAFilterItem> rules;

        public TotemDNAFilter(string filterJson)
        {
            rules = JsonConvert.DeserializeObject<List<TotemDNAFilterItem>>(filterJson);
        }

        public T FilterDNA<T>(string dna) where T : new()
        {
            T obj = new T();
            Type objType = obj.GetType();

            foreach (var rule in rules)
            {
                var field = objType.GetField(rule.id);

                if (field == null)
                {
                    continue;
                }

                string binVal = dna.Substring(rule.gene * 32 + rule.start, rule.length);

                int intVal = Convert.ToInt32(binVal, 2);

                Enum.TryParse(rule.type, true, out TotemDNAType dnaType);
                switch (dnaType)
                {
                    case TotemDNAType.Bool:

                        if (field.FieldType != typeof(bool))
                        {
                            continue;
                        }

                        field.SetValue(obj, intVal == 1);
                        break;


                    case TotemDNAType.Int:

                        if (field.FieldType != typeof(int))
                        {
                            continue;
                        }

                        field.SetValue(obj, intVal);
                        break;
                        

                    case TotemDNAType.Map:

                        if (field.FieldType != typeof(string))
                        {
                            continue;
                        }
                        
                        foreach (var value in rule.values)
                        {
                            if (Convert.ToInt32(value.value) == intVal)
                            {
                                field.SetValue(obj, value.key);
                                break;
                            }
                        }

                        break;


                    case TotemDNAType.Range:

                        if (field.FieldType != typeof(string))
                        {
                            continue;
                        }

                        foreach (var value in rule.values)
                        {
                            var rangeJArray = value.value as JArray;
                            int[] rangeValues = rangeJArray.ToObject<int[]>();
                            if (intVal >= rangeValues[0] && intVal <= rangeValues[1])
                            {
                                field.SetValue(obj, value.key);
                                break;
                            }
                        }

                        break;

                    case TotemDNAType.Color:

                        if (field.FieldType != typeof(Color))
                        {
                            continue;
                        }

                        byte r = Convert.ToByte(binVal.Substring(0, 8), 2);
                        byte g = Convert.ToByte(binVal.Substring(8, 8), 2);
                        byte b = Convert.ToByte(binVal.Substring(16, 8), 2);
                        Color32 color32 = new Color32(r, g, b, 255);

                        field.SetValue(obj, (Color)color32);

                        break;
                }

            }

            return obj;
        }
    }

}
