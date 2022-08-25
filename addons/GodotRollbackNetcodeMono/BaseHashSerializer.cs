using GDDictionary = Godot.Collections.Dictionary;
using GDArray = Godot.Collections.Array;

namespace GodotRollbackNetcode
{
    public interface IHashSerializer
    {
        GDDictionary Serialize(object value);

        GDDictionary Unserialize(object value);
    }

    public abstract class BaseHashSerializer : Godot.Reference, IHashSerializer
    {
        private GDDictionary serialize(object value) => Serialize(value);

        public virtual GDDictionary Serialize(object value)
        {
            if (value is GDDictionary)
                return
        }

        protected virtual GDDictionary SerializeDictionary(GDDictionary value)
        {
            var serialized = new GDDictionary();
            foreach (var key in value.Keys)
                serialized[key] = serialize(value[key]);
            return serialized;
        }

        protected virtual GDArray SerializeArray(GDArray array)
        {
            var serialized = new GDArray();
            foreach (var item in array)
                serialized.Add(serialize(item));
            return serialized;
        }

        private GDDictionary unserialize(object value) => Unserialize(value);

        public virtual GDDictionary Unserialize(object value)
        {

        }
    }
}
