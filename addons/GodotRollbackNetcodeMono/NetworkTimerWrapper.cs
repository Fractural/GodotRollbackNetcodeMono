using Godot;
using System;

namespace GodotRollbackNetcode
{
    public partial class NetworkTimerWrapper : GDScriptWrapper
    {
        public bool Autostart { get => Source.Get<bool>("auto_start"); set => Source.Set("auto_start", value); }
        public bool OneShot { get => Source.Get<bool>("one_shot"); set => Source.Set("one_shot", value); }
        public int WaitTicks { get => Source.Get<int>("wait_ticks"); set => Source.Set("wait_ticks", value); }
        public bool HashState { get => Source.Get<bool>("hash_state"); set => Source.Set("hash_state", value); }
        public bool IsRunning => Source.Get<bool>("_running");
        public bool IsStopped => !IsRunning;
        public int TicksLeft => Source.Get<int>("ticks_left");

        public event Action Timeout;

        public void Start(int ticks = -1) => Source.Call("start", ticks);

        public void Stop() => Source.Call("stop");

        private void OnTimeout() => Timeout?.Invoke();

        protected override void ForwardSignalsToEvents()
        {
            Source.Connect("timeout", new Callable(this, nameof(OnTimeout)));
        }
    }
}
