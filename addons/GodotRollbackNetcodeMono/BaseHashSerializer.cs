using Godot;
using System;
using GDC = Godot.Collections;

namespace GodotRollbackNetcode
{
    public interface IHashSerializer
    {
        Variant Serialize(Variant value);

        Variant Unserialize(Variant value);
    }

    public partial class BaseHashSerializer : Godot.RefCounted, IHashSerializer
    {
        private object serialize(Variant value) => Serialize(value);

        public virtual Variant Serialize(Variant value)
        {
            if (value.Obj is GDC.Dictionary dict)
                return SerializeDictionary(dict);
            else if (value.Obj is GDC.Array array)
                return SerializeArray(array);
            else if (value.Obj is Resource resource)
                return SerializeResource(resource);
            else if (value.Obj is GodotObject obj)
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

        protected virtual GDC.Dictionary SerializeObject(GodotObject value)
        {
            return new GDC.Dictionary()
            {
                ["_"] = nameof(GodotObject),
                ["str"] = value.ToString(),
            };
        }

        protected virtual Variant SerializeOther(Variant value)
        {
            if (value.Obj is Vector2 vector2)
                return new GDC.Dictionary()
                {
                    ["_"] = nameof(Vector2),
                    ["x"] = vector2.X,
                    ["y"] = vector2.Y
                };
            else if (value.Obj is Vector3 vector3)
                return new GDC.Dictionary()
                {
                    ["_"] = nameof(Vector3),
                    ["x"] = vector3.X,
                    ["y"] = vector3.Y,
                    ["z"] = vector3.Z
                };
            else if (value.Obj is Transform2D transform2D)
                return new GDC.Dictionary()
                {
                    ["_"] = nameof(Transform2D),
                    ["x"] = new GDC.Dictionary()
                    {
                        ["x"] = transform2D.X.X,
                        ["y"] = transform2D.X.Y
                    },
                    ["y"] = new GDC.Dictionary()
                    {
                        ["x"] = transform2D.Y.X,
                        ["y"] = transform2D.Y.Y
                    },
                    ["origin"] = new GDC.Dictionary()
                    {
                        ["x"] = transform2D.Origin.X,
                        ["y"] = transform2D.Origin.Y
                    }
                };
            else if (value.Obj is Transform3D transform)
                return new GDC.Dictionary()
                {
                    ["_"] = nameof(Transform3D),
                    ["x"] = new GDC.Dictionary()
                    {
                        ["x"] = transform.Basis.X.X,
                        ["y"] = transform.Basis.X.Y,
                        ["z"] = transform.Basis.X.Z,
                    },
                    ["y"] = new GDC.Dictionary()
                    {
                        ["x"] = transform.Basis.Y.X,
                        ["y"] = transform.Basis.Y.Y,
                        ["z"] = transform.Basis.Y.Z
                    },
                    ["z"] = new GDC.Dictionary()
                    {
                        ["x"] = transform.Basis.Z.X,
                        ["y"] = transform.Basis.Z.Y,
                        ["z"] = transform.Basis.Z.Z
                    },
                    ["origin"] = new GDC.Dictionary()
                    {
                        ["x"] = transform.Origin.X,
                        ["y"] = transform.Origin.Y,
                        ["z"] = transform.Origin.Z,
                    }
                };
            return value;
        }

        private Variant unserialize(Variant value) => Unserialize(value);

        readonly string[] SerializedOthers = new[] { nameof(Vector2), nameof(Vector3), nameof(Transform2D), nameof(Transform3D) };

        public virtual Variant Unserialize(Variant value)
        {
            if (value.Obj is GDC.Dictionary dictionary)
            {
                if (!dictionary.Keys.Contains("_"))
                    return UnserializeDictionary(dictionary);
                var type = dictionary.Get<string>("_");
                if (type == nameof(Resource))
                    return UnserializeResource(dictionary);
                else if (Array.IndexOf(SerializedOthers, type) > -1)
                    return UnserializeOther(dictionary);
            }
            else if (value.Obj is GDC.Array array)
            {
                return UnserializeArray(array);
            }
            return value;
        }

        protected virtual GDC.Dictionary UnserializeDictionary(GDC.Dictionary value)
        {
            var unserialized = new GDC.Dictionary();
            foreach (var key in value.Keys)
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
            if (value["_"] is GodotObject)
                return (string)value["string"];
            return null;
        }

        protected virtual Variant UnserializeOther(GDC.Dictionary value)
        {
            switch ((string)value["_"])
            {
                case nameof(Vector2):
                    return new Vector2(value.Get<float>("x"), value.Get<float>("y"));
                case nameof(Vector3):
                    return new Vector3(value.Get<float>("x"), value.Get<float>("y"), value.Get<float>("z"));
                case nameof(Transform2D):
                    return new Transform2D(
                        new Vector2(value.Get<float>("x.X"), value.Get<float>("x.Y")),
                        new Vector2(value.Get<float>("y.X"), value.Get<float>("y.Y")),
                        new Vector2(value.Get<float>("origin.X"), value.Get<float>("origin.Y"))
                    );
                case nameof(Transform3D):
                    return new Transform3D(
                        new Vector3(value.Get<float>("x.X"), value.Get<float>("x.Y"), value.Get<float>("x.Z")),
                        new Vector3(value.Get<float>("y.X"), value.Get<float>("y.Y"), value.Get<float>("y.Z")),
                        new Vector3(value.Get<float>("z.X"), value.Get<float>("z.Y"), value.Get<float>("z.Z")),
                        new Vector3(value.Get<float>("origin.X"), value.Get<float>("origin.Y"), value.Get<float>("origin.Z"))
                    );
            }
            return default;
        }
    }
}
