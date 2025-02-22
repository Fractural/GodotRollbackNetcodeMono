using Fractural;
using Fractural.Utils;
using Godot;

namespace GodotRollbackNetcode
{
    public class NetworkAnimationPlayerWrapper : GDScriptWrapper
    {
        public bool AutoReset { get => Source.Get<bool>("auto_reset"); set => Source.Set("auto_reset", value); }
        public new AnimationPlayer Source => base.Source as AnimationPlayer;
    }
}
