using GDDictionary = Godot.Collections.Dictionary;

namespace GodotRollbackNetcode
{
    public class HashSerializerWrapper : GDScriptWrapper, IHashSerializer
    {
        public HashSerializerWrapper() { }
        public HashSerializerWrapper(Godot.Object source) : base(source) { }

        public GDDictionary Serialize(GDDictionary value) => (GDDictionary)Source.Call("serialize", value);

        public GDDictionary Unserialize(GDDictionary value) => (GDDictionary)Source.Call("unserialize", value);
    }
}
