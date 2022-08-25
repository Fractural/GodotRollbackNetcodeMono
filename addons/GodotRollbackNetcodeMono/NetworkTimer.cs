using System;

namespace GodotRollbackNetcode
{
    public class NetworkTimer : GDScriptWrapper
    {
        public NetworkTimer() : base() { }
        public NetworkTimer(Godot.Object source) : base(source)
        {
            source.Connect("timeout", this, nameof(OnTimeout));
        }

        public bool Autostart
        {
            get => (bool)Source.Get("autostart");
            set => Source.Set("autostart", value);
        }

        public bool OneShot
        {
            get => (bool)Source.Get("one_shot");
            set => Source.Set("one_shot", value);
        }

        public bool WaitTicks
        {
            get => (bool)Source.Get("wait_ticks");
            set => Source.Set("wait_ticks", value);
        }

        public bool HashState
        {
            get => (bool)Source.Get("hash_state");
            set => Source.Set("hash_state", value);
        }

        public event Action Timeout;

        private void OnTimeout()
        {
            Timeout?.Invoke();
        }

        public void Start(int ticks = -1)
        {
            Source.Call("start", ticks);
        }

        public void Stop()
        {
            Source.Call("stop");
        }

        public bool IsStopped => (bool)Source.Call("is_stopped");
    }
}
