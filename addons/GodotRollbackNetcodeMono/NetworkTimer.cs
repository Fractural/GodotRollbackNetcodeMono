using Godot;
using Godot.Collections;

namespace GodotRollbackNetcode
{
    [GlobalClass]
    [Icon("res://addons/GodotRollbackNetcodeMono/Assets/NetworkTimer.svg")]
    public partial class NetworkTimer : Node, INetworkSerializable, INetworkProcess
    {
        [Export]
        public bool Autostart { get; set; }
        [Export]
        public bool OneShot { get; set; }
        [Export]
        public int WaitTicks { get; set; }
        [Export]
        public bool HashState { get; set; }
        public bool IsRunning { get; private set; }
        public bool IsStopped => !IsRunning;
        public int TicksLeft { get; private set; }

        [Signal]
        public delegate void TimeoutEventHandler();

        public override void _Ready()
        {
            AddToGroup(SyncManager.NetworkSyncGroup);
            SyncManager.Global.Connect("sync_stopped", new Callable(this, nameof(OnSyncManagerSyncStopped)));
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

        public void _network_process(Dictionary input)
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
                EmitSignal(nameof(TimeoutEventHandler));
            }
        }

        public Dictionary _save_state()
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

        public void _load_state(Dictionary state)
        {
            IsRunning = state.GetStateValue<bool>(nameof(IsRunning));
            WaitTicks = state.GetStateValue<int>(nameof(WaitTicks));
            TicksLeft = state.GetStateValue<int>(nameof(TicksLeft));
        }
    }
}
