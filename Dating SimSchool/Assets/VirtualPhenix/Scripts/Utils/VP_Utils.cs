using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using VirtualPhenix.Dialog;

namespace VirtualPhenix
{
    public class VP_Utils
    {
        public static string AddZerosToCharacterNumber(int number, int maxCharacter = 3)
        {
            return number.ToString().PadLeft(maxCharacter, '0');
        }
        
        // extracts the name of a scene from the path pointing to it
        public static string ExtractSceneNameFromPath(string path)
        {
            int i = path.Length - 1;
            string currentChar = "";

            // browse through characters
            while (i > 0 && currentChar != "/" && currentChar != "\\")
            {
                i--;
                currentChar = path[i].ToString();
            };

            int start = i + 1, length = path.Length - i - 7;
            if (length < 0) { length = 0; Debug.LogWarning("Failed to extract scene name from scene path."); }
            return path.Substring(start, length);
        }

        //---------------------------------------------------------------------
        public static void SetRectValues(RectTransform rt, Vector2 leftRight, Vector2 topBottom)
        {
            SetRectLeft(rt, leftRight.x);
            SetRectRight(rt, leftRight.y);
            SetRectTop(rt, topBottom.x);
            SetRectBottom(rt, topBottom.y);
        }
        public static void SetRectValues(RectTransform rt, float left, float right, float top, float bottom)
        {
            SetRectLeft(rt, left);
            SetRectRight(rt, right);
            SetRectTop(rt, top);
            SetRectBottom(rt, bottom);
        }

        public static float GetRectLeft(RectTransform rt)
        {
            return rt.offsetMin.x;
        }

        public static float GetRectRight(RectTransform rt)
        {
            return rt.offsetMax.x * -1;
        }

        public static float GetRectTop(RectTransform rt)
        {
            return rt.offsetMax.y*-1;
        }

        public static float GetRectBottom(RectTransform rt)
        {
            return rt.offsetMin.y;
        }

        public static Vector2 GetRectLeftRight(RectTransform rt)
        {
            return new Vector2(GetRectLeft(rt), GetRectRight(rt));
        }

        public static Vector2 GetRectTopBottom(RectTransform rt)
        {
            return new Vector2(GetRectTop(rt), GetRectBottom(rt));
        }

        public static void SetRectLeft(RectTransform rt, float left)
        {
            rt.offsetMin = new Vector2(left, rt.offsetMin.y);
        }

        public static void SetRectRight(RectTransform rt, float right)
        {
            rt.offsetMax = new Vector2(-right, rt.offsetMax.y);
        }

        public static void SetRectTop(RectTransform rt, float top)
        {
            rt.offsetMax = new Vector2(rt.offsetMax.x, -top);
        }

        public static void SetRectBottom(RectTransform rt, float bottom)
        {
            rt.offsetMin = new Vector2(rt.offsetMin.x, bottom);
        }

        public static string CreateID()
        {
            StringBuilder builder = new StringBuilder();
            Enumerable
               .Range(65, 26)
                .Select(e => ((char)e).ToString())
                .Concat(Enumerable.Range(97, 26).Select(e => ((char)e).ToString()))
                .Concat(Enumerable.Range(0, 10).Select(e => e.ToString()))
                .OrderBy(e => Guid.NewGuid())
                .Take(11)
                .ToList().ForEach(e => builder.Append(e));
            string id = builder.ToString();
            //Debug.Log("Creating ID: " + id);
            return id;
        }

        public static Color ObjectToColor(object value)
        {
            if (value is Color) return (Color)value;
            if (value is Color32) return ObjectToColor((Color32)value);
            if (value is Vector3) return ObjectToColor((Vector3)value);
            if (value is Vector4) return ObjectToColor((Vector4)value);
            return ObjectToColor(ObjectToInt(value));
        }

        public static int ColorToInt(Color32 color)
        {
            return (color.a << 24) +
                   (color.r << 16) +
                   (color.g << 8) +
                   color.b;
        }

        public static int ObjectToInt(object value)
        {
            if (value == null)
            {
                return 0;
            }
            else if (value is Color)
            {
                return ColorToInt((Color)value);
            }
            else if (value is Color32)
            {
                return ColorToInt((Color32)value);
            }
            else if (value is System.IConvertible)
            {
                try
                {
                    return System.Convert.ToInt32(value);
                }
                catch
                {
                    return 0;
                }
            }
            else
            {
                return ObjectToInt(value.ToString());
            }
        }

        public static string GetStringBetweenStrings(string strSource, string strStart, string strEnd)
        {
            int Start, End;
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                Start = GetWordIndexInString(strSource, strStart);
                End = strSource.IndexOf(strEnd, Start);
                return strSource.Substring(Start, End - Start);
            }
            else
            {
                UnityEngine.Debug.Log("words not contained in source string");
                return "";
            }
        }

        public static int GetWordIndexInString(string strSource, string word)
        {
            int ret = -1;
            if (strSource.Contains(word))
            {
                ret = strSource.IndexOf(word, 0) + word.Length;
            }
            return ret;
        }

        public static float GetRandomDeviation(float value, float deviation)
        {
            return value + deviation * (UnityEngine.Random.value * 2f - 1f);
        }

        public static string SplitCamelCase(string str)
        {
            return Regex.Replace(Regex.Replace(str, "(\\P{Ll})(\\P{Ll}\\p{Ll})", "$1 $2"), "(\\p{Ll})(\\P{Ll})", "$1 $2");
        }

        public static float AreaOfTriangle(Vector2 pt1, Vector2 pt2, Vector2 pt3)
        {
            float len = Vector2.Distance(pt1, pt2);
            float len2 = Vector2.Distance(pt2, pt3);
            float len3 = Vector2.Distance(pt3, pt1);
            return VP_Utils.AreaOfTriangle(len, len2, len3);
        }

        public static float AreaOfTriangle(float len1, float len2, float len3)
        {
            float num = (len1 + len2 + len3) / 2f;
            return Mathf.Sqrt(Mathf.Max(num * (num - len1) * (num - len2) * (num - len3), 0f));
        }

        //-------------------------------------------------------------------------
        public static string GetStringBetweenChars(string fullStr, char element, char element2)
        {
            int str1 = fullStr.IndexOf(element);
            int str2 = fullStr.IndexOf(element2);

            return "";
        }

        public static int GetIndexFromString(string fullStr, char element)
        {
            return fullStr.IndexOf(element);
        }

        public static class DialogUtils
        {
            public static List<VP_DialogTextSymbol> CreateSymbolListFromText(string text)
            {
                var symbolList = new List<VP_DialogTextSymbol>();
                int parsedCharacters = 0;
                while (parsedCharacters < text.Length)
                {
                    VP_DialogTextSymbol symbol = null;

                    // Check for tags
                    var remainingText = text.Substring(parsedCharacters, text.Length - parsedCharacters);
                    if (VP_DialogRichTextTag.StringStartsWithTag(remainingText))
                    {
                        var tag = VP_DialogRichTextTag.ParseNextWithMiddleText(remainingText);
                        symbol = new VP_DialogTextSymbol(tag);
                    }
                    else
                    {
                        symbol = new VP_DialogTextSymbol(remainingText.Substring(0, 1));
                    }

                    parsedCharacters += symbol.Length;
                    symbolList.Add(symbol);
                }

                return symbolList;
            }

            public static void SetVariableToDatabase<T>(string _name, T value, VP_VariableDataBase database)
            {
                if (typeof(T).Equals(typeof(string)))
                {
                    string _val = (string)System.Convert.ChangeType(value, typeof(string));
                    database.AddStringVariable(_name, _val);
                }
                else if (typeof(T).Equals(typeof(int)))
                {
                    int _val = (int)System.Convert.ChangeType(value, typeof(int));
                    database.AddIntVariable(_name, _val);
                }
                else if (typeof(T).Equals(typeof(float)))
                {
                    float _val = (float)System.Convert.ChangeType(value, typeof(float));
                    database.AddFloatVariable(_name, _val);
                }
                else if (typeof(T).Equals(typeof(double)))
                {
                    double _val = (double)System.Convert.ChangeType(value, typeof(double));
                    database.AddDoubleVariable(_name, _val);
                }
                else if (typeof(T).Equals(typeof(bool)))
                {
                    bool _val = (bool)System.Convert.ChangeType(value, typeof(bool));
                    database.AddBoolVariable(_name, _val);
                }
                else if (typeof(T).Equals(typeof(UnityEngine.GameObject)))
                {
                    UnityEngine.GameObject _val = (UnityEngine.GameObject)System.Convert.ChangeType(value, typeof(UnityEngine.GameObject));
                    database.AddGameObjectVariable(_name, _val);
                }
                else if (typeof(T).Equals(typeof(UnityEngine.Object)))
                {
                    UnityEngine.Object _val = (UnityEngine.Object)System.Convert.ChangeType(value, typeof(UnityEngine.Object));
                    database.AddUnityObjectVariable(_name, _val);
                }
                else
                {
                    Debug.LogError("Unknown or not accepted variable type in variable with name " + _name);
                }
            }

            public static FieldData GetVariableValueFromDatabase<T>(string _variableName, T _type, VP_VariableDataBase database)
            {
                return database?.GetVariableValue(_variableName, _type);
            }           
            
            public static FieldData GetVariableValueStrTypeFromDatabase (string _variableName, string _type, VP_VariableDataBase database)
            {
                return database?.GetVariableValueStrType(_variableName, _type);
            }

            public static string GetVariableValueStringFromStrTypeFromDatabase(string _variableName, string _type, VP_VariableDataBase database)
            {
                return GetVariableFieldDataTextValue(database?.GetVariableValueStrType(_variableName, _type));
            }

            public static string GetVariableValueStringFromDatabase<T>(string _variableName, T _type, VP_VariableDataBase database)
            {
                return GetVariableFieldDataTextValue(database?.GetVariableValue(_variableName, _type));
            }

            public static string GetVariableFieldDataTextValue(FieldData _data)
            {
                if (_data == null)
                    return "";

                if (_data is Field<string>)
                {
                    return (_data as Field<string>).Value;
                }
                else if (_data is Field<bool>)
                {
                    return (_data as Field<bool>).Value.ToString();
                }
                else if (_data is Field<int>)
                {
                    return (_data as Field<int>).Value.ToString();
                }
                else if (_data is Field<float>)
                {
                    return (_data as Field<float>).Value.ToString();
                }
                else if (_data is Field<double>)
                {
                    return (_data as Field<double>).Value.ToString();
                }
                else if (_data is Field<GameObject>)
                {
                    return (_data as Field<GameObject>).Value.name;
                }
                else if (_data is Field<UnityEngine.Object>)
                {
                    return (_data as Field<UnityEngine.Object>).Value.name;
                }
                else
                {
                    return "";
                }
            }

            public static string RemoveAllTags(string textWithTags)
            {
                string textWithoutTags = textWithTags;
                textWithoutTags = RemoveUnityTags(textWithoutTags);
                textWithoutTags = RemoveCustomTags(textWithoutTags);

                return textWithoutTags;
            }

            public static string RemoveCustomTags(string textWithTags)
            {
                return RemoveTags(textWithTags, VP_DialogSetup.Tags.CUSTOM_TAGS);
            }

            public static string RemoveUnityTags(string textWithTags)
            {
                return RemoveTags(textWithTags, VP_DialogSetup.Tags.UNITY_TAGS);
            }

            private static string RemoveTags(string textWithTags, params string[] tags)
            {
                string textWithoutTags = textWithTags;
                foreach (var tag in tags)
                {
                    textWithoutTags = VP_DialogRichTextTag.RemoveTagsFromString(textWithoutTags, tag);
                }

                return textWithoutTags;
            }
        }

#if UNITY_EDITOR
        public static class ScriptableObjectUtility
        {
            /// <summary>
            //	This makes it easy to create, name and place unique new ScriptableObject asset files.
            /// </summary>
            public static void CreateAsset<T>() where T : ScriptableObject
            {
                T asset = ScriptableObject.CreateInstance<T>();

                string path = UnityEditor.AssetDatabase.GetAssetPath(UnityEditor.Selection.activeObject);
                if (path == "")
                {
                    path = "Assets";
                }
                else if (Path.GetExtension(path) != "")
                {
                    path = path.Replace(Path.GetFileName(UnityEditor.AssetDatabase.GetAssetPath(UnityEditor.Selection.activeObject)), "");
                }

                string assetPathAndName = UnityEditor.AssetDatabase.GenerateUniqueAssetPath(path + "/New " + typeof(T).ToString() + ".asset");

                UnityEditor.AssetDatabase.CreateAsset(asset, assetPathAndName);

                UnityEditor.AssetDatabase.SaveAssets();
                UnityEditor.AssetDatabase.Refresh();
                UnityEditor.EditorUtility.FocusProjectWindow();
                UnityEditor.Selection.activeObject = asset;
            }

        }


#endif
    }
}
