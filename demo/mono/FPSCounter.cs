using Godot;

namespace Game
{
    public class FPSCounter : Label
    {
        public override void _Process(float delta)
        {
            Text = Engine.GetFramesPerSecond() + " FPS";
        }
    }
}