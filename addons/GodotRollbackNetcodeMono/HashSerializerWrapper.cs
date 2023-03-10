using Fractural;
using GDC = Godot.Collections;

namespace GodotRollbackNetcode
{
    public class HashSerializerWrapper : GDScriptWrapper, IHashSerializer
    {
        public HashSerializerWrapper() { }
        public HashSerializerWrapper(Godot.Object source) : base(source) { }

        public object Serialize(object value) => (GDC.Dictionary)Source.Call("serialize", value);

        public object Unserialize(object value) => (GDC.Dictionary)Source.Call("unserialize", value);
    }
}
