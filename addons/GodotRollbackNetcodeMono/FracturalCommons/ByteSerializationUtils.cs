using Godot;
using System.Collections.Generic;
using System.Linq;

namespace GodotRollbackNetcode
{
    public interface IBufferSerializable
    {
        void Serialize(StreamPeerBuffer buffer);
        void Deserialize(StreamPeerBuffer buffer);
    }

    public static class ByteSerializationUtils
    {
        private static StreamPeerBuffer buffer = new StreamPeerBuffer();

        #region StreamPeerBuffer
        public static StreamPeerBuffer ToBuffer(this byte[] byteArray)
        {
            var buffer = new StreamPeerBuffer();
            buffer.DataArray = byteArray;
            return buffer;
        }

        public static void PutSerializable(this StreamPeerBuffer buffer, IBufferSerializable serializable)
        {
            serializable.Serialize(buffer);
        }

        public static void PutArray<T>(this StreamPeerBuffer buffer, IEnumerable<T> array) where T : IBufferSerializable, new()
        {
            buffer.Put32(array.Count());
            foreach (var elem in array)
                buffer.PutSerializable(elem);
        }

        public static void PutPrimitiveArray<T>(this StreamPeerBuffer buffer, IEnumerable<T> array) where T : struct
        {
            buffer.Put32(array.Count());
            foreach (var elem in array)
                buffer.PutPrimitive(elem);
        }

        public static void PutPrimitive<T>(this StreamPeerBuffer buffer, T value) where T : struct
        {
            if (typeof(T) == typeof(bool))
                buffer.PutU8((bool)(object)value ? (byte)1 : (byte)0);
            else if (typeof(T) == typeof(short))
                buffer.Put16((short)(object)value);
            else if (typeof(T) == typeof(int))
                buffer.Put32((int)(object)value);
            else if (typeof(T) == typeof(long))
                buffer.Put64((long)(object)value);
            else if (typeof(T) == typeof(ushort))
                buffer.PutU16((ushort)(object)value);
            else if (typeof(T) == typeof(uint))
                buffer.PutU32((uint)(object)value);
            else if (typeof(T) == typeof(ulong))
                buffer.PutU64((ulong)(object)value);
            else
                throw new System.Exception($"Cannot put primitive type <{typeof(T).Name}> from StreamPeerBuffer");
        }

        public static T GetSerializable<T>(this StreamPeerBuffer buffer) where T : IBufferSerializable, new()
        {
            T inst = new T();
            inst.Deserialize(buffer);
            return inst;
        }

        public static T[] GetArray<T>(this StreamPeerBuffer buffer) where T : IBufferSerializable, new()
        {
            T[] array = new T[buffer.Get32()];
            for (int i = 0; i < array.Length; i++)
                array[i] = buffer.GetSerializable<T>();
            return array;
        }

        public static T[] GetPrimitiveArray<T>(this StreamPeerBuffer buffer) where T : struct
        {
            T[] array = new T[buffer.Get32()];
            for (int i = 0; i < array.Length; i++)
                array[i] = buffer.GetPrimitive<T>();
            return array;
        }

        public static T GetPrimitive<T>(this StreamPeerBuffer buffer) where T : struct
        {
            if (typeof(T) == typeof(bool))
                return (T)(object)(buffer.Get8() == 1);
            if (typeof(T) == typeof(short))
                return (T)(object)buffer.Get16();
            if (typeof(T) == typeof(int))
                return (T)(object)buffer.Get32();
            if (typeof(T) == typeof(long))
                return (T)(object)buffer.Get64();
            if (typeof(T) == typeof(ushort))
                return (T)(object)buffer.GetU16();
            if (typeof(T) == typeof(uint))
                return (T)(object)buffer.GetU32();
            if (typeof(T) == typeof(ulong))
                return (T)(object)buffer.GetU64();
            throw new System.Exception($"Cannot get primitive type <{typeof(T).Name}> from StreamPeerBuffer");
        }
        #endregion

        #region byte[] Serialization
        public static byte[] SerializePrimitive<T>(this T value) where T : struct
        {
            buffer.Clear();
            buffer.PutPrimitive<T>(value);
            return buffer.DataArray;
        }

        public static byte[] Serialize(this IBufferSerializable serializable)
        {
            buffer.Clear();
            serializable.Serialize(buffer);
            return buffer.DataArray;
        }

        public static byte[] SerializeArray<T>(this IEnumerable<T> serializableArray) where T : IBufferSerializable, new()
        {
            buffer.Clear();
            buffer.PutArray<T>(serializableArray);
            return buffer.DataArray;
        }

        public static byte[] SerializePrimitiveArray<T>(this IEnumerable<T> serializableArray) where T : struct
        {
            buffer.Clear();
            buffer.PutPrimitiveArray<T>(serializableArray);
            return buffer.DataArray;
        }

        public static T DeserializePrimitive<T>(this byte[] byteArray) where T : struct
        {
            buffer.DataArray = byteArray;
            return buffer.GetPrimitive<T>();
        }

        public static T Deserialize<T>(this byte[] byteArray) where T : IBufferSerializable, new()
        {
            buffer.DataArray = byteArray;
            return buffer.GetSerializable<T>();
        }

        public static T[] DeserializeArray<T>(this byte[] byteArray) where T : IBufferSerializable, new()
        {
            buffer.DataArray = byteArray;
            return buffer.GetArray<T>();
        }

        public static T[] DeserializePrimitiveArray<T>(this byte[] byteArray) where T : struct
        {
            buffer.DataArray = byteArray;
            return buffer.GetPrimitiveArray<T>();
        }
        #endregion
    }
}