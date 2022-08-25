namespace GodotRollbackNetcode
{
    public class NetworkRandomNumberGenerator : GDScriptWrapper
    {
        public NetworkRandomNumberGenerator() { }
        public NetworkRandomNumberGenerator(Godot.Object source) : base(source) { }

        public int Seed
        {
            get => (int)Source.Call("get_seed");
            set => Source.Call("set_seed", value);
        }

        public void Randomize() => Source.Call("randomize");
        public int Randi() => (int)Source.Call("randi");
        public int RandiRange(int from, int to) => (int)Source.Call("randi_range", from, to);
    }
}
