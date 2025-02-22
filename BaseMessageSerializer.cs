using Godot;
using GDDictionary = Godot.Collections.Dictionary;
using System;
using System.Linq;

namespace GodotRollbackNetcode
{
    public interface IMessageSerializer
    {
        byte[] SerializeInput(GDDictionary input);
        GDDictionary UnserializeInput(byte[] serialized);
        byte[] SerializeMessage(GDDictionary msg);
        GDDictionary UnserializeMessage(byte[] serialized);
    }

    public partial class BaseMessageSerializer : Godot.RefCounted, IMessageSerializer
    {
        const int DEFAULT_MESSAGE_BUFFER_SIZE = 1280;
        enum InputMessageKey
        {
            NEXT_INPUT_TICK_REQUESTED = 0,
            INPUT = 1,
            NEXT_HASH_TICK_REQUESTED = 2,
            STATE_HASHES = 3,
        }

        private byte[] serialize_input(GDDictionary input) => SerializeInput(input);

        public virtual byte[] SerializeInput(GDDictionary input)
        {
            return GD.VarToBytes(input);
        }

        private GDDictionary unserialize_input(byte[] serialized) => UnserializeInput(serialized);

        public virtual GDDictionary UnserializeInput(byte[] serialized)
        {
            return (GDDictionary)GD.BytesToVar(serialized);
        }

        private byte[] serialize_message(GDDictionary msg) => SerializeMessage(msg);

        public virtual byte[] SerializeMessage(GDDictionary msg)
        {
            var buffer = new StreamPeerBuffer();
            buffer.Resize(DEFAULT_MESSAGE_BUFFER_SIZE);

            buffer.Put32((int)msg[(int)InputMessageKey.NEXT_INPUT_TICK_REQUESTED]);

            GDDictionary inputTicks = (GDDictionary)msg[(int)InputMessageKey.INPUT];
            buffer.PutU8((byte)inputTicks.Count);
            if (inputTicks.Count > 0)
            {
                var inputKeys = inputTicks.Keys.OfType<int>().OrderBy((key) => key);
                buffer.Put32(inputKeys.First());
                foreach (var inputKey in inputKeys)
                {
                    var input = (byte[])inputTicks[inputKey];
                    buffer.PutU16(Convert.ToUInt16(input.Length));
                    buffer.PutData(input);
                }
            }

            buffer.Put32((int)msg[(int)InputMessageKey.NEXT_HASH_TICK_REQUESTED]);

            GDDictionary stateHashes = (GDDictionary)msg[(int)InputMessageKey.STATE_HASHES];
            buffer.PutU8(Convert.ToByte(stateHashes.Count));
            if (stateHashes.Count > 0)
            {
                var stateHashKeys = stateHashes.Keys.OfType<int>().OrderBy((key) => key);
                buffer.Put32(stateHashKeys.First());
                foreach (var stateHashKey in stateHashKeys)
                    // HACK: Currently Godot marshalls all GDScript ints into C# ints, even though they
                    //       have widly different ranges. GDScript ints are 64-bit, while C# ints are
                    //       32-bit.
                    //       https://github.com/godotengine/godot/issues/57141
                    buffer.Put32((int)stateHashes[stateHashKey]);
            }

            buffer.Resize(buffer.GetPosition());
            return buffer.DataArray;
        }

        private GDDictionary unserialize_message(byte[] serialized) => UnserializeMessage(serialized);

        public virtual GDDictionary UnserializeMessage(byte[] serialized)
        {
            var buffer = new StreamPeerBuffer();
            buffer.PutData(serialized);
            buffer.Seek(0);

            var msg = new GDDictionary();
            msg[(int)InputMessageKey.INPUT] = new GDDictionary();
            msg[(int)InputMessageKey.STATE_HASHES] = new GDDictionary();

            msg[(int)InputMessageKey.NEXT_INPUT_TICK_REQUESTED] = buffer.GetU32();

            var inputTickCount = buffer.GetU8();
            if (inputTickCount > 0)
            {
                var inputTick = buffer.GetU32();
                for (int i = 0; i < inputTickCount; i++)
                {
                    var inputSize = buffer.GetU16();
                    ((GDDictionary)msg[(int)InputMessageKey.INPUT])[inputTick] = buffer.GetData(inputSize)[1];
                    inputTick += 1;
                }
            }

            msg[(int)InputMessageKey.NEXT_HASH_TICK_REQUESTED] = buffer.GetU32();

            var hashTickCount = buffer.GetU8();
            if (hashTickCount > 0)
            {
                var hashTick = buffer.GetU32();
                for (int i = 0; i < hashTickCount; i++)
                {
                    ((GDDictionary)msg[(int)InputMessageKey.STATE_HASHES])[hashTick] = buffer.GetU32();
                    hashTick += 1;
                }
            }

            return msg;
        }
    }
}
