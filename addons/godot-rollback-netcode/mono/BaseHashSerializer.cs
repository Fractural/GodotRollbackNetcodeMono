using GDDictionary = Godot.Collections.Dictionary;

namespace GodotRollbackNetcode
{
    public interface IHashSerializer
    {
        GDDictionary Serialize(GDDictionary value);

        GDDictionary Unserialize(GDDictionary value);
    }

    public abstract class BaseHashSerializer : Godot.Reference, IHashSerializer
    {
        private GDDictionary serialize(GDDictionary value) => Serialize(value);

        public abstract GDDictionary Serialize(GDDictionary value);

        private GDDictionary unserialize(GDDictionary value) => Unserialize(value);

        public abstract GDDictionary Unserialize(GDDictionary value);
    }
}
