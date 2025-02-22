using Godot;

namespace GodotRollbackNetcode
{
    public partial class HashSerializerWrapper : GDScriptWrapper, IHashSerializer
    {
        public HashSerializerWrapper() { }
        public HashSerializerWrapper(GodotObject source) : base(source) { }

        public Variant Serialize(Variant value) => Source.Call("serialize", value).AsGodotDictionary();

        public Variant Unserialize(Variant value) => Source.Call("unserialize", value).AsGodotDictionary();
    }
}
