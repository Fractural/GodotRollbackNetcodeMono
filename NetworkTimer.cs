using Fractural;
using Fractural.Commons;
using Godot;
using Godot.Collections;
using System;

namespace GodotRollbackNetcode
{
    [RegisteredType(nameof(NetworkTimer), "res://addons/GodotRollbackNetcodeMono/Assets/NetworkTimer.svg")]
    public class NetworkTimer : Node, INetworkSerializable, INetworkProcess
    {
        public bool Autostart { get; set; }
        public bool OneShot { get; set; }
        public int WaitTicks { get; set; }
        public bool HashState { get; set; }
        public bool IsRunning { get; private set; }
        public bool IsStopped => !IsRunning;
        public int TicksLeft { get; private set; }

        public event Action Timeout;

        public override void _Ready()
        {
            AddToGroup(SyncManager.NetworkSyncGroup);
            SyncManager.Global.Connect("sync_stopped", this, nameof(OnSyncManagerSyncStopped));
        }

        private void OnSyncManagerSyncStopped()
        {
            Stop();
        }

        public void Start(int ticks = -1)
        {
            if (ticks > 0)
                WaitTicks = ticks;
            TicksLeft = WaitTicks;
            IsRunning = true;
        }

        public void Stop()
        {
            IsRunning = false;
            TicksLeft = 0;
        }

        public void _NetworkProcess(Dictionary input)
        {
            if (!IsRunning) return;
            if (TicksLeft <= 0)
            {
                IsRunning = false;
                return;
            }

            TicksLeft--;

            if (TicksLeft == 0)
            {
                if (!OneShot)
                    TicksLeft = WaitTicks;
                Timeout?.Invoke();
            }
        }

        public Dictionary _SaveState()
        {
            var state = new Dictionary()
            {
                [nameof(IsRunning)] = IsRunning,
                [nameof(WaitTicks)] = WaitTicks,
                [nameof(TicksLeft)] = TicksLeft,
            };
            if (HashState)
                return state;
            return state.IgnoreState();
        }

        public void _LoadState(Dictionary state)
        {
            IsRunning = state.GetStateValue<bool>(nameof(IsRunning));
            WaitTicks = state.GetStateValue<int>(nameof(WaitTicks));
            TicksLeft = state.GetStateValue<int>(nameof(TicksLeft));
        }
    }
}
