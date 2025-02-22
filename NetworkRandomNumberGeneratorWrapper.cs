using Godot;

namespace GodotRollbackNetcode
{
    public partial class NetworkRandomNumberGeneratorWrapper : GDScriptWrapper
    {
        public RandomNumberGenerator Generator => Source.Get<RandomNumberGenerator>("generator");
        public ulong Seed { get => Generator.Seed; set => Generator.Seed = value; }

        public void Randomize() => Generator.Randomize();
        public uint Randi() => Generator.Randi();
        public int RandiRange(int from, int to) => Generator.RandiRange(from, to);
    }
}
