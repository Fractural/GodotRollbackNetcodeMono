using Fractural;
using Godot.Collections;

namespace GodotRollbackNetcode
{
    public class MessageSerializerWrapper : GDScriptWrapper, IMessageSerializer
    {
        public MessageSerializerWrapper() { }
        public MessageSerializerWrapper(Godot.Object source) : base(source) { }

        public byte[] SerializeInput(Dictionary input) => (byte[])Source.Call("serialize_input", input);
        public byte[] SerializeMessage(Dictionary msg) => (byte[])Source.Call("serialize_message", msg);
        public Dictionary UnserializeInput(byte[] serialized) => (Dictionary)Source.Call("unserialize_input", serialized);
        public Dictionary UnserializeMessage(byte[] serialized) => (Dictionary)Source.Call("unserialize_message", serialized);
    }
}
