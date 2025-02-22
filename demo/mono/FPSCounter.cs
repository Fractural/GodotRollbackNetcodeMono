using Godot;

namespace Game
{
    public partial class FPSCounter : Label
    {
        public override void _Process(double delta)
        {
            Text = Engine.GetFramesPerSecond() + " FPS";
        }
    }
}