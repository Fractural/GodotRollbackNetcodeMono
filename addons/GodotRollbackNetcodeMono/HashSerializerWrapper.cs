using GDDictionary = Godot.Collections.Dictionary;

namespace GodotRollbackNetcode
{
    public class HashSerializerWrapper : GDScriptWrapper, IHashSerializer
    {
        public HashSerializerWrapper() { }
        public HashSerializerWrapper(Godot.Object source) : base(source) { }

        public object Serialize(object value) => (GDDictionary)Source.Call("serialize", value);

        public object Unserialize(object value) => (GDDictionary)Source.Call("unserialize", value);
    }
}
