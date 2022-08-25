using Godot;
using System;

namespace GodotRollbackNetcode
{
    public class NetworkAnimationPlayer : GDScriptWrapper
    {
        public NetworkAnimationPlayer() : base() { }
        public NetworkAnimationPlayer(Godot.Object source) : base(source) { }

        #region Properties
        public bool AutoReset
        {
            get => (bool)Source.Get("auto_reset");
            set => Source.Set("auto_reset", value);
        }

        public string AssignedAnimation
        {
            get => (string)Source.Get("assigned_animation");
            set => Source.Set("assigned_animation", value);
        }

        public string AutoPlay
        {
            get => (string)Source.Get("autoplay");
            set => Source.Set("autoplay", value);
        }

        public string CurrentAnimation
        {
            get => (string)Source.Get("current_animation");
            set => Source.Set("current_animation", value);
        }

        public float CurrentAnimationLength
        {
            get => (float)Source.Get("current_animation_length");
            set => Source.Set("current_animation_length", value);
        }

        public float CurrentAnimationPosition
        {
            get => (float)Source.Get("current_animation_position");
            set => Source.Set("current_animation_position", value);
        }

        public AnimationPlayer.AnimationMethodCallMode MethodCallMode
        {
            get => (AnimationPlayer.AnimationMethodCallMode)Source.Get("method_call_mode");
            set => Source.Set("method_call_mode", value);
        }

        public bool PlaybackActive
        {
            get => (bool)Source.Get("playback_active");
            set => Source.Set("playback_active", value);
        }

        public float PlackbackDefaultBlendTime
        {
            get => (float)Source.Get("playback_default_blend_time");
            set => Source.Set("playback_default_blend_time", value);
        }

        public AnimationPlayer.AnimationProcessMode PlaybackProcessMode
        {
            get => (AnimationPlayer.AnimationProcessMode)Source.Get("playback_process_mode");
            set => Source.Set("playback_process_mode", value);
        }

        public float PlaybackSpeed
        {
            get => (float)Source.Get("playback_speed");
            set => Source.Set("playback_speed", value);
        }

        public bool ResetOnSave
        {
            get => (bool)Source.Get("reset_on_save");
            set => Source.Set("reset_on_save", value);
        }

        public NodePath RootNode
        {
            get => (NodePath)Source.Get("root_node");
            set => Source.Set("root_node", value);
        }
        #endregion

        #region Methods
        public Error AddAnimation(string name, Animation animation) => (Error)Source.Call("add_animation", name, animation);

        public void Advance(float delta) => Source.Call("advance", delta);

        public string AnimationGetNext(string animFrom) => (string)Source.Call("animation_get_next", animFrom);

        public void AnimationSetNext(string animFrom, string animTo) => Source.Call("animation_set_next", animFrom, animTo);

        public void ClearCaches() => Source.Call("clear_caches");

        public void ClearQueue() => Source.Call("clear_queue");

        public string FindAnimation(Animation animation) => (string)Source.Call("find_animation", animation);

        public Animation GetAnimation(string name) => (Animation)Source.Call("get_animation", name);

        public string[] GetAnimationList() => (string[])Source.Call("get_animation_list");

        public float GetBlendTime(string animFrom, string animTo) => (float)Source.Call("get_blend_time", animFrom, animTo);

        public float GetPlayingSpeed() => (float)Source.Call("get_playing_speed");

        public string[] GetQueue() => (string[])Source.Call("get_queue");

        public bool HasAnimation(string name) => (bool)Source.Call("has_animation", name);

        public bool IsPlaying => (bool)Source.Call("is_playing");

        public void Play(string name = "", float customBlend = -1, float customSpeed = 1, bool fromEnd = false) => Source.Call("play", name, customBlend, customSpeed, fromEnd);

        public void PlayBackwards(string name = "", float customBlend = -1) => Source.Call("play_backwards", name, customBlend);

        public void Queue(string name) => Source.Call("queue", name);

        public void RemoveAnimation(string name) => Source.Call("remove_animation", name);

        public void RenameAnimation(string name, string newName) => Source.Call("rename_animation", name, newName);

        public void Seek(float seconds, bool update = false) => Source.Call("seek", seconds, update);

        public void SetBlendTime(string animFrom, string animTo, float sec) => Source.Call("set_blend_time", animFrom, animTo, sec);

        public void Stop(bool reset = true) => Source.Call("stop", reset);
        #endregion

        #region Signals
        public delegate void AnimationChangedDelegate(string oldName, string newName);
        public event AnimationChangedDelegate AnimationChanged;

        public delegate void AnimationFinishedDelegate(string animName);
        public event AnimationFinishedDelegate AnimationFinished;

        public delegate void AnimationStartedDelegate(string animName);
        public event AnimationStartedDelegate AnimationStarted;

        public event Action CachesCleared;

        protected override void ForwardSignalsToEvents()
        {
            Source.Connect("animation_changed", this, nameof(OnAnimationChanged));
            Source.Connect("animation_finished", this, nameof(OnAnimationFinished));
            Source.Connect("animation_started", this, nameof(OnAnimationStarted));
            Source.Connect("caches_cleared", this, nameof(OnCachesCleared));
        }

        private void OnCachesCleared()
        {
            CachesCleared?.Invoke();
        }

        private void OnAnimationStarted(string animName)
        {
            AnimationStarted?.Invoke(animName);
        }

        private void OnAnimationFinished(string animName)
        {
            AnimationFinished?.Invoke(animName);
        }

        private void OnAnimationChanged(string oldName, string newName)
        {
            AnimationChanged?.Invoke(oldName, newName);
        }
        #endregion
    }
}
