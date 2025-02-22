using Godot;
using GDC = Godot.Collections;

namespace GodotRollbackNetcode
{
    public partial class MessageSerializerWrapper : GDScriptWrapper, IMessageSerializer
    {
        public MessageSerializerWrapper() { }
        public MessageSerializerWrapper(GodotObject source) : base(source) { }

        public byte[] SerializeInput(GDC.Dictionary input) => (byte[])Source.Call("serialize_input", input);
        public byte[] SerializeMessage(GDC.Dictionary msg) => (byte[])Source.Call("serialize_message", msg);
        public GDC.Dictionary UnserializeInput(byte[] serialized) => Source.Call("unserialize_input", serialized).AsGodotDictionary();
        public GDC.Dictionary UnserializeMessage(byte[] serialized) => Source.Call("unserialize_message", serialized).AsGodotDictionary();
    }
}
