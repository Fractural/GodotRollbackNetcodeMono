using Fractural;
using Fractural.Commons;
using Fractural.Utils;
using Godot;
using Godot.Collections;

namespace GodotRollbackNetcode
{
    [RegisteredType(nameof(NetworkRandomNumberGenerator), "res://addons/GodotRollbackNetcodeMono/Assets/NetworkRNG.svg")]
    public class NetworkRandomNumberGenerator : Node, INetworkSerializable
    {
        private RandomNumberGenerator generator;

        private ulong seed;
        public ulong Seed
        {
            get => seed;
            set
            {
                seed = value;
                generator.Seed = value;
            }
        }

        public override void _Ready()
        {
            generator = new RandomNumberGenerator();
        }

        public void Randomize() => generator.Randomize();
        public uint Randi() => generator.Randi();
        public int RandiRange(int from, int to) => generator.RandiRange(from, to);

        public Dictionary _SaveState()
        {
            return new Dictionary()
            {
                ["state"] = generator.State.Serialize()
            };
        }

        public void _LoadState(Dictionary state)
        {
            generator.State = state.Get<byte[]>("state").DeserializePrimitive<ulong>();
        }
    }
}
