using Fractural.Commons;
using Fractural.Utils;
using Godot;
using Godot.Collections;

namespace GodotRollbackNetcode
{
    [RegisteredType(nameof(NetworkAnimationPlayer), "res://addons/GodotRollbackNetcodeMono/Assets/NetworkAnimationPlayer.svg", nameof(AnimationPlayer))]
    public class NetworkAnimationPlayer : AnimationPlayer, INetworkProcess, INetworkSerializable
    {
        [Export]
        public bool AutoReset { get; set; } = true;

        public override void _Ready()
        {
            MethodCallMode = AnimationMethodCallMode.Immediate;
            PlaybackProcessMode = AnimationProcessMode.Manual;
            AddToGroup(SyncManager.NetworkSyncGroup);
        }

        public void _network_process(Dictionary input)
        {
            if (IsPlaying())
                Advance(SyncManager.Global.TickTime);
        }

        public Dictionary _save_state()
        {
            if (IsPlaying() && (!AutoReset || CurrentAnimation != "RESET"))
                return new Dictionary()
                {
                    [nameof(IsPlaying)] = true,
                    [nameof(CurrentAnimation)] = CurrentAnimation,
                    [nameof(CurrentAnimationPosition)] = CurrentAnimationPosition,
                    [nameof(PlaybackSpeed)] = PlaybackSpeed,
                };
            else
                return new Dictionary()
                {
                    [nameof(IsPlaying)] = false,
                    [nameof(CurrentAnimation)] = "",
                    [nameof(CurrentAnimationPosition)] = 0f,
                    [nameof(PlaybackSpeed)] = 1,
                };
        }

        public void _load_state(Dictionary state)
        {
            if (state.Get<bool>(nameof(IsPlaying)))
            {
                var stateAnim = state.Get<string>(nameof(CurrentAnimation));
                if (!IsPlaying() || CurrentAnimation != stateAnim)
                    Play(stateAnim);
                Seek(state.Get<int>(nameof(CurrentAnimationPosition)));
            }
            else
            {
                if (AutoReset && HasAnimation("RESET"))
                    Play("RESET");
                else
                    Stop();
            }
        }
    }
}
