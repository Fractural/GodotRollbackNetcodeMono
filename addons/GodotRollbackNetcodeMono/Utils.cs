using Godot;
using System;
using System.Reflection;
using GDDictionary = Godot.Collections.Dictionary;

namespace GodotRollbackNetcode
{
    public static class Utils
    {
        public static T AsWrapper<T>(this Godot.Object source) where T : GDScriptWrapper
        {
            return (T)Activator.CreateInstance(typeof(T), new object[] { source });
        }

        public static T GetNodeAsWrapper<T>(this Node node, NodePath path) where T : GDScriptWrapper
        {
            return node.GetNode(path).AsWrapper<T>();
        }

        public static T Get<T>(this GDDictionary dictionary, object key, T defaultReturn = default)
        {
            if (dictionary.Contains(key))
                return (T)dictionary[key];
            return defaultReturn;
        }

        public static T Get<T>(this GDDictionary dictionary, string key, T defaultReturn = default)
        {
            var keys = key.Split(".");
            for (int i = 0; i < keys.Length; i++)
            {
                if (i == keys.Length - 1)
                {
                    if (dictionary.Contains(key))
                        return (T)dictionary[key];
                    return defaultReturn;
                }
                dictionary = dictionary.Get<GDDictionary>(keys[i]);
                if (dictionary == null)
                    return defaultReturn;
            }
            return defaultReturn;
        }

        public static GDDictionary ToGDDict(this object obj)
        {
            GDDictionary dict = new GDDictionary();
            foreach (var prop in obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                dict[prop.Name] = prop.GetValue(obj, null);
            }
            return dict;
        }

        public static Vector2 Lerp(this Vector2 start, Vector2 end, float weight)
        {
            return new Vector2(Mathf.Lerp(start.x, end.x, weight), Mathf.Lerp(start.y, end.y, weight));
        }
    }
}
