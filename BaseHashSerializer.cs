using GDC = Godot.Collections;
using Godot;
using System;
using Fractural.Utils;

namespace GodotRollbackNetcode
{
    public interface IHashSerializer
    {
        object Serialize(object value);

        object Unserialize(object value);
    }

    public class BaseHashSerializer : Godot.Reference, IHashSerializer
    {
        private object serialize(object value) => Serialize(value);

        public virtual object Serialize(object value)
        {
            if (value is GDC.Dictionary dict)
                return SerializeDictionary(dict);
            else if (value is GDC.Array array)
                return SerializeArray(array);
            else if (value is Resource resource)
                return SerializeResource(resource);
            else if (value is Godot.Object obj)
                return SerializeObject(obj);

            return SerializeOther(value);
        }

        protected virtual GDC.Dictionary SerializeDictionary(GDC.Dictionary value)
        {
            var serialized = new GDC.Dictionary();
            foreach (var key in value.Keys)
                serialized[key] = Serialize(value[key]);
            return serialized;
        }

        protected virtual GDC.Array SerializeArray(GDC.Array array)
        {
            var serialized = new GDC.Array();
            foreach (var item in array)
                serialized.Add(Serialize(item));
            return serialized;
        }

        protected virtual GDC.Dictionary SerializeResource(Resource value)
        {
            return new GDC.Dictionary()
            {
                ["_"] = nameof(Resource),
                ["path"] = value.ResourcePath
            };
        }

        protected virtual GDC.Dictionary SerializeObject(Godot.Object value)
        {
            return new GDC.Dictionary()
            {
                ["_"] = nameof(Godot.Object),
                ["str"] = value.ToString(),
            };
        }

        protected virtual object SerializeOther(object value)
        {
            if (value is Vector2 vector2)
                return new GDC.Dictionary()
                {
                    ["_"] = nameof(Vector2),
                    ["x"] = vector2.x,
                    ["y"] = vector2.y
                };
            else if (value is Vector3 vector3)
                return new GDC.Dictionary()
                {
                    ["_"] = nameof(Vector3),
                    ["x"] = vector3.x,
                    ["y"] = vector3.y,
                    ["z"] = vector3.z
                };
            else if (value is Transform2D transform2D)
                return new GDC.Dictionary()
                {
                    ["_"] = nameof(Transform2D),
                    ["x"] = new GDC.Dictionary()
                    {
                        ["x"] = transform2D.x.x,
                        ["y"] = transform2D.x.y
                    },
                    ["y"] = new GDC.Dictionary()
                    {
                        ["x"] = transform2D.y.x,
                        ["y"] = transform2D.y.y
                    },
                    ["origin"] = new GDC.Dictionary()
                    {
                        ["x"] = transform2D.origin.x,
                        ["y"] = transform2D.origin.y
                    }
                };
            else if (value is Transform transform)
                return new GDC.Dictionary()
                {
                    ["_"] = nameof(Transform),
                    ["x"] = new GDC.Dictionary()
                    {
                        ["x"] = transform.basis.x.x,
                        ["y"] = transform.basis.x.y,
                        ["z"] = transform.basis.x.z,
                    },
                    ["y"] = new GDC.Dictionary()
                    {
                        ["x"] = transform.basis.y.x,
                        ["y"] = transform.basis.y.y,
                        ["z"] = transform.basis.y.z
                    },
                    ["z"] = new GDC.Dictionary()
                    {
                        ["x"] = transform.basis.z.x,
                        ["y"] = transform.basis.z.y,
                        ["z"] = transform.basis.z.z
                    },
                    ["origin"] = new GDC.Dictionary()
                    {
                        ["x"] = transform.origin.x,
                        ["y"] = transform.origin.y,
                        ["z"] = transform.origin.z,
                    }
                };
            return value;
        }

        private object unserialize(object value) => Unserialize(value);

        readonly string[] SerializedOthers = new[] { nameof(Vector2), nameof(Vector3), nameof(Transform2D), nameof(Transform) };

        public virtual object Unserialize(object value)
        {
            if (value is GDC.Dictionary dictionary)
            {
                if (!dictionary.Contains("_"))
                    return UnserializeDictionary(dictionary);
                var type = dictionary.Get<string>("_");
                if (type == nameof(Resource))
                    return UnserializeResource(dictionary);
                else if (Array.IndexOf(SerializedOthers, type) > -1)
                    return UnserializeOther(dictionary);
            }
            else if (value is GDC.Array array)
            {
                return UnserializeArray(array);
            }
            return value;
        }

        protected virtual GDC.Dictionary UnserializeDictionary(GDC.Dictionary value)
        {
            var unserialized = new GDC.Dictionary();
            foreach (var key in value)
                unserialized[key] = Unserialize(value[key]);
            return unserialized;
        }

        protected virtual GDC.Array UnserializeArray(GDC.Array value)
        {
            var unserialized = new GDC.Array();
            foreach (var item in value)
                unserialized.Add(Unserialize(item));
            return unserialized;
        }

        protected virtual Resource UnserializeResource(GDC.Dictionary value)
        {
            return GD.Load<Resource>((string)value["path"]);
        }

        protected virtual string UnserializeObject(GDC.Dictionary value)
        {
            if (value["_"] is Godot.Object)
                return (string)value["string"];
            return null;
        }

        protected virtual object UnserializeOther(GDC.Dictionary value)
        {
            switch ((string)value["_"])
            {
                case nameof(Vector2):
                    return new Vector2(value.Get<float>("x"), value.Get<float>("y"));
                case nameof(Vector3):
                    return new Vector3(value.Get<float>("x"), value.Get<float>("y"), value.Get<float>("z"));
                case nameof(Transform2D):
                    return new Transform2D(
                        new Vector2(value.Get<float>("x.x"), value.Get<float>("x.y")),
                        new Vector2(value.Get<float>("y.x"), value.Get<float>("y.y")),
                        new Vector2(value.Get<float>("origin.x"), value.Get<float>("origin.y"))
                    );
                case nameof(Transform):
                    return new Transform(
                        new Vector3(value.Get<float>("x.x"), value.Get<float>("x.y"), value.Get<float>("x.z")),
                        new Vector3(value.Get<float>("y.x"), value.Get<float>("y.y"), value.Get<float>("y.z")),
                        new Vector3(value.Get<float>("z.x"), value.Get<float>("z.y"), value.Get<float>("z.z")),
                        new Vector3(value.Get<float>("origin.x"), value.Get<float>("origin.y"), value.Get<float>("origin.z"))
                    );
            }
            return null;
        }
    }
}
