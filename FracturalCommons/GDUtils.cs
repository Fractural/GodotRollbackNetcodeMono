using Godot;
using System;
using System.Collections.Generic;
using GDC = Godot.Collections;

/// <summary>
/// Utilities
/// </summary>
namespace GodotRollbackNetcode
{
    /// <summary>
    /// Utilities for Godot Nodes.
    /// </summary>
    public static class GDUtils
    {
        // Bridges the custom GDScipt typing system
        // into the C# world.

        /// <summary>
        /// Checks the type of a GDScript class.
        /// Only use this when you want compatability 
        /// with GDScript classes.
        /// </summary>
        /// <param name="obj">Object being checked</param>
        /// <returns>Type of "obj" as a string</returns>
        public static string GetTypeName(object obj)
        {
            if (obj is GodotObject gdObj && gdObj.GetScript().Obj is Godot.GDScript && gdObj.HasMethod("get_types"))
            {
                // GDScript custom type name
                return (gdObj.Call("get_types").Obj as string[])[0];
            }
            return obj.GetType().Name;
        }

        /// <summary>
        /// Check the type of a GDScript class.
        /// Only use this when you want compatability
        /// with GDScript classes.
        /// </summary>
        /// <param name="obj">Object being checked</param>
        /// <param name="type">Type that we want to check</param>
        /// <returns>True if "obj" is "type"</returns>
        public static bool IsType(object obj, string type)
        {
            if (obj is GodotObject gdObj && gdObj.GetScript().Obj is Godot.GDScript && gdObj.HasMethod("get_types"))
            {
                // GDScript custom type checking
                return Array.Exists((gdObj.Call("get_types").Obj as string[]), typeString => typeString == type);
            }
            return obj.GetType().Name == "type";
        }

        /// <summary>
        /// Attempts to free a Godot Object.
        /// </summary>
        /// <returns>True if the object could be freed</returns>
        public static bool TryFree(GodotObject obj)
        {
            if (obj != null && !(obj is RefCounted))
            {
                obj.Free();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Attempts to free a collection of Godot Objects.
        /// </summary>
        /// <param name="collection">Collection to be freed</param>
        /// <returns>True if all elements in "collection" could be freed</returns>
        public static bool TryFree(this IEnumerable<GodotObject> collection)
        {
            foreach (GodotObject obj in collection)
            {
                if (!TryFree(obj))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Gets a property from a Godot Object that is
        /// casted to a certain type.
        /// </summary>
        /// <param name="obj">Object being checked</param>
        /// <param name="property">Name of the property</param>
        /// <typeparam name="T">Type to cast the property's value too</typeparam>
        /// <returns>The value of the property casted to "T"</returns>
        public static T Get<T>(this GodotObject obj, string property)
        {
            return (T)obj.Get(property).Obj;
        }

        /// <summary>
        /// Checks if a Godot Object has a property.
        /// </summary>
        /// <param name="obj">Object being used</param>
        /// <param name="property">Name of the property</param>
        /// <returns>True if the object has the property</returns>
        public static bool Has(this GodotObject obj, string property)
        {
            return obj.Get(property).Obj != null;
        }

        /// <summary>
        /// Calls a method on a Godot Object and returns the 
        /// result as a certain type.
        /// </summary>
        /// <param name="obj">Object being used</param>
        /// <param name="method">Name of method to call</param>
        /// <param name="args">Arguments to pass into the method</param>
        /// <typeparam name="T">Type to cast the result to</typeparam>
        /// <returns>The result of the method call, casted to "T"</returns>
        public static T Call<T>(this GodotObject obj, string method, params Variant[] args)
        {
            return (T)obj.Call(method, args).Obj;
        }

        public static T AsWrapper<T>(this GodotObject source) where T : GDScriptWrapper
        {
            return (T)Activator.CreateInstance(typeof(T), new object[] { source });
        }

        public static T GetNodeAsWrapper<T>(this Node node, NodePath path) where T : GDScriptWrapper
        {
            return node.GetNode(path).AsWrapper<T>();
        }

        public static Vector2 Lerp(this Vector2 start, Vector2 end, float weight)
        {
            return new Vector2(
                Mathf.Lerp(start.X, end.X, weight),
                Mathf.Lerp(start.Y, end.Y, weight)
            );
        }

        public static Vector3 Lerp(this Vector3 start, Vector3 end, float weight)
        {
            return new Vector3(
                Mathf.Lerp(start.X, end.X, weight),
                Mathf.Lerp(start.Y, end.Y, weight),
                Mathf.Lerp(start.Z, end.Z, weight)
            );
        }

        public static Theme GetThemeFromAncestor(this Node node, bool scanNonControlParents = false)
        {
            if (node == null) return null;
            if (node is Control control)
            {
                if (control.Theme != null)
                    return control.Theme;
            }
            else if (!scanNonControlParents)
                // The node wasn't a control
                // If we aren't including non control parents in the search, then our search ends here.
                return null;
            return GetThemeFromAncestor(node.GetParent(), scanNonControlParents);
        }

        public static T GetStylebox<T>(this Theme theme, string name, string themeType) where T : StyleBox
        {
            return theme.GetStylebox(name, themeType) as T;
        }

        public static T GetFont<T>(this Theme theme, string name, string themeType) where T : Font
        {
            return theme.GetFont(name, themeType) as T;
        }

        public static T GetStylebox<T>(this Control node, string name, string themeType) where T : StyleBox
        {
            return node.GetThemeStylebox(name, themeType) as T;
        }

        public static T GetFont<T>(this Control node, string name, string themeType) where T : Font
        {
            return node.GetThemeFont(name, themeType) as T;
        }

        public static string ToGDJSON(this Variant obj) => Json.Stringify(obj);

        public static object FromGDJSON(this string json)
        {
            var result = Json.ParseString(json).Obj;
            if (result != null)
                return null;
            return result;
        }

        /// <summary>
        /// Used to fill in the "binds" variable of GodotObject.Connect()
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static GDC.Array GDParams(params Variant[] array)
        {
            var gdArray = new GDC.Array();
            foreach (var elem in array)
                gdArray.Add(elem);
            return gdArray;
        }

        public static object[] Params(params object[] array) => array;

        public static bool TryConnect(this GodotObject obj, string signal, Callable callable, uint flags = 0)
        {
            if (obj.IsConnected(signal, callable))
                return false;
            obj.Connect(signal, callable, flags);
            return true;
        }

        public static bool TryDisconnect(this GodotObject obj, string signal, Callable callable)
        {
            if (!obj.IsConnected(signal, callable))
                return false;
            obj.Disconnect(signal, callable);
            return true;
        }

        /// <summary>
        /// Returns the object's metadata for a give name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <param name="defaultReturn"></param>
        /// <returns></returns>
        public static T GetMeta<T>(this GodotObject obj, string name, T defaultReturn = default)
        {
            return (T)obj.GetMeta(name, defaultReturn);
        }

        /// <summary>
        /// Returns a random element from a list
        /// </summary>
        /// <param name="rng"></param>
        /// <param name="list"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T RandElem<T>(this RandomNumberGenerator rng, IList<T> list)
        {
            return list[(int)rng.Randi() % list.Count];
        }
    }
}