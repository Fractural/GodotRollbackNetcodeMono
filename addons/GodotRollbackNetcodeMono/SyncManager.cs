using Godot;
using System;
using GDC = Godot.Collections;

namespace GodotRollbackNetcode
{
    public partial class SyncManager : GDScriptWrapper
    {
        #region Singleton
        private static SyncManager instance;
        public static SyncManager Global
        {
            get
            {
                if (instance == null)
                    GD.PrintErr("Expected C# SyncManager singleton to be initialized. Did you forget to call SyncManager.Init(node)?");
                return instance;
            }
        }
        public static void Init(Node node)
        {
            var autoloadedNode = node.GetNodeOrNull("/root/SyncManager");
            if (autoloadedNode != null)
                instance = new SyncManager(autoloadedNode);
        }

        public const string NetworkSyncGroup = "network_sync";
        #endregion

        #region Object Variables
        private NetworkAdaptorWrapper networkAdaptorWrapper;
        public INetworkAdaptor NetworkAdaptor
        {
            get
            {
                var source = (GodotObject)Source.Get("network_adaptor");
                if (source == null) return null;
                if (source is INetworkAdaptor networkAdaptor) return networkAdaptor;
                if (networkAdaptorWrapper == null || networkAdaptorWrapper.Source != source)
                    networkAdaptorWrapper = new NetworkAdaptorWrapper(source);
                return networkAdaptorWrapper;
            }
            set
            {
                if (value is BaseNetworkAdaptor @base)
                    Source.Set("network_adaptor", @base);
                else if (value is NetworkAdaptorWrapper wrapper)
                    Source.Set("network_adaptor", wrapper.Source);
                else if (value is GodotObject obj)
                    Source.Set("network_adaptor", obj);
                else
                    GD.PrintErr($"You can only assign {nameof(BaseNetworkAdaptor)}, {nameof(NetworkAdaptorWrapper)}, or {nameof(Godot)}.{nameof(GodotObject)} to {nameof(SyncManager)}.{nameof(NetworkAdaptor)}");
            }
        }

        private MessageSerializerWrapper messageSerializerWrapper;
        public IMessageSerializer MessageSerializer
        {
            get
            {
                var source = (GodotObject)Source.Get("message_serializer");
                if (source == null) return null;
                if (source is IMessageSerializer messageSerializer) return messageSerializer;
                if (messageSerializerWrapper == null || messageSerializerWrapper.Source != source)
                    messageSerializerWrapper = new MessageSerializerWrapper(source);
                return messageSerializerWrapper;
            }
            set
            {
                if (value is BaseMessageSerializer @base)
                    Source.Set("network_adaptor", @base);
                else if (value is MessageSerializerWrapper wrapper)
                    Source.Set("network_adaptor", wrapper.Source);
                else if (value is GodotObject obj)
                    Source.Set("network_adaptor", obj);
                else
                    GD.PrintErr($"You can only assign {nameof(BaseMessageSerializer)}, {nameof(MessageSerializerWrapper)}, or {nameof(Godot)}.{nameof(GodotObject)} to {nameof(SyncManager)}.{nameof(MessageSerializer)}");
            }
        }

        private HashSerializerWrapper hashSerializerWrapper;
        public IHashSerializer HashSerializer
        {
            get
            {
                var source = (GodotObject)Source.Get("hash_serializer");
                if (source == null) return null;
                if (source is IHashSerializer hashSerializer) return hashSerializer;
                if (hashSerializerWrapper == null || hashSerializerWrapper.Source != source)
                    hashSerializerWrapper = new HashSerializerWrapper(source);
                return hashSerializerWrapper;
            }
            set
            {
                if (value is BaseHashSerializer @base)
                    Source.Set("network_adaptor", @base);
                else if (value is HashSerializerWrapper wrapper)
                    Source.Set("network_adaptor", wrapper.Source);
                else if (value is GodotObject obj)
                    Source.Set("network_adaptor", obj);
                else
                    GD.PrintErr($"You can only assign {nameof(BaseHashSerializer)}, {nameof(HashSerializerWrapper)}, or {nameof(Godot)}.{nameof(GodotObject)} to {nameof(SyncManager)}.{nameof(HashSerializer)}");
            }
        }
        #endregion

        #region Collection Variables
        public GDC.Dictionary Peers
        {
            get => (GDC.Dictionary)Source.Get("peers");
            set => Source.Set("peers", value);
        }

        public Godot.Collections.Array InputBuffer
        {
            get => (Godot.Collections.Array)Source.Get("input_buffer");
            set => Source.Set("input_buffer", value);
        }

        public Godot.Collections.Array StateBuffer
        {
            get => (Godot.Collections.Array)Source.Get("state_buffer");
            set => Source.Set("state_buffer", value);
        }

        public Godot.Collections.Array StateHashes
        {
            get => (Godot.Collections.Array)Source.Get("state_hashes");
            set => Source.Set("state_hashes", value);
        }
        #endregion

        #region Mechanized Variables
        public bool Mechanized
        {
            get => (bool)Source.Get("mechanized");
            set => Source.Set("mechanized", value);
        }

        public GDC.Dictionary MechanizedInputReceived
        {
            get => (GDC.Dictionary)Source.Get("mechanized_input_received");
            set => Source.Set("mechanized_input_received", value);
        }

        public long MechanizedRollbackTicks
        {
            get => (long)Source.Get("mechanized_rollback_ticks");
            set => Source.Set("mechanized_rollback_ticks", value);
        }
        #endregion

        #region General Variables
        public long MaxBufferSize
        {
            get => (long)Source.Get("max_buffer_size");
            set => Source.Set("max_buffer_size", value);
        }

        public long TicksToCalculateAdvantage
        {
            get => (long)Source.Get("ticks_to_calculate_advantage");
            set => Source.Set("ticks_to_calculate_advantage", value);
        }

        public long InputDelay
        {
            get => (long)Source.Get("input_delay");
            set => Source.Set("input_delay", value);
        }

        public long MaxInputFramesPerMessage
        {
            get => (long)Source.Get("max_input_frames_per_message");
            set => Source.Set("max_input_frames_per_message", value);
        }

        public long MaxMessagesAtOnce
        {
            get => (long)Source.Get("max_messages_at_once");
            set => Source.Set("max_messages_at_once", value);
        }

        public long MaxTicksToRegainSync
        {
            get => (long)Source.Get("max_ticks_to_regain_sync");
            set => Source.Set("max_ticks_to_regain_sync", value);
        }

        public long MinLagToRegainSync
        {
            get => (long)Source.Get("min_lag_to_regain_sync");
            set => Source.Set("min_lag_to_regain_sync", value);
        }

        public bool Interpolation
        {
            get => (bool)Source.Get("interpolation");
            set => Source.Set("interpolation", value);
        }

        public long MaxStateMismatchCount
        {
            get => (long)Source.Get("max_state_mismatch_count");
            set => Source.Set("max_state_mismatch_count", value);
        }
        #endregion

        #region Debug Variables
        public long DebugrollbackTicks
        {
            get => (long)Source.Get("debug_rollback_ticks");
            set => Source.Set("debug_rollback_ticks", value);
        }

        public long DebugRandomRollbackTicks
        {
            get => (long)Source.Get("debug_random_rollback_ticks");
            set => Source.Set("debug_random_rollback_ticks", value);
        }

        public long DebugMessageBytes
        {
            get => (long)Source.Get("debug_message_bytes");
            set => Source.Set("debug_message_bytes", value);
        }

        public long DebugSkipNthMessage
        {
            get => (long)Source.Get("debug_skip_nth_message");
            set => Source.Set("debug_skip_nth_message", value);
        }

        public float DebugPhysicsProcessMsecs
        {
            get => (float)Source.Get("debug_physics_process_msecs");
            set => Source.Set("debug_physics_process_msecs", value);
        }

        public float DebugProcessMsecs
        {
            get => (float)Source.Get("debug_process_msecs");
            set => Source.Set("debug_process_msecs", value);
        }

        public bool DebugCheckMessageSerializerRoundtrip
        {
            get => (bool)Source.Get("debug_check_message_serializer_roundtrip");
            set => Source.Set("debug_check_message_serializer_roundtrip", value);
        }

        public bool DebugCheckLocalStateConsistency
        {
            get => (bool)Source.Get("debug_check_local_state_consistency");
            set => Source.Set("debug_check_local_state_consistency", value);
        }
        #endregion

        /// <summary>
        /// The ping frequency in seconds, because we don't want it to be dependent on the network tick.
        /// </summary>
        public float PingFrequency
        {
            get => (float)Source.Get("ping_frequency");
            set => Source.Set("ping_frequency", value);
        }

        #region Readonly Variables
        public long InputTick => (long)Source.Get("input_tick");
        public long CurrentTick => (long)Source.Get("current_tick");
        public long SkipTicks => (long)Source.Get("skip_ticks");
        public long RollbackTicks => (long)Source.Get("rollback_ticks");
        public long RequestedInputCompleteTick => (long)Source.Get("requested_input_complete_tick");
        public bool Started => (bool)Source.Get("started");
        public float TickTime => (float)Source.Get("tick_time");
        #endregion

        public SyncManager() { }

        public SyncManager(GodotObject source) : base(source) { }

        #region Methods
        public void ResetNetworkAdaptor() => Source.Call("reset_network_adaptor");

        public void AddPeer(long peerId) => Source.Call("add_peer", peerId);

        public bool HasPeer(long peerId) => (bool)Source.Call("has_peer", peerId);

        public void RemovePeer(long peerid) => Source.Call("remove_peer", peerid);

        public void ClearPeers() => Source.Call("clear_peers");

        public void StartLogging(string logFilePath) => StartLogging(logFilePath, new GDC.Dictionary());

        public void StartLogging(string logFilePath, GDC.Dictionary matchInfo) => Source.Call("start_logging", logFilePath, matchInfo);

        public void StopLogging() => Source.Call("stop_logging");

        public void Start() => Source.Call("start");

        public void Stop() => Source.Call("stop");

        public void ResetMechanizedData() => Source.Call("reset_mechanized_data");


        public void ExecuteMechanizedTick() => Source.Call("execute_mechanized_tick");

        public void ExecuteMechanizedInterpolationFrame(float delta) => Source.Call("execute_mechanized_interpolation_frame", delta);

        public void ExecuteMechanizedInterframe() => Source.Call("execute_mechanized_interframe");

        public GDC.Dictionary SortDictionaryKeys(GDC.Dictionary dictionary) => (GDC.Dictionary)Source.Call("sort_dictionary_keys");

        public Node Spawn(string name, Node parent, PackedScene scene, GDC.Dictionary data, bool rename = true, string signalName = "") => (Node)Source.Call("spawn", name, parent, scene, data, rename, signalName);

        public void Despawn(Node node) => Source.Call("despawn", node);

        public bool IsInRollback => (bool)Source.Call("is_in_rollback");
        public bool IsRespawning => (bool)Source.Call("is_respawning");

        public void SetDefaultSoundBus(string bus) => Source.Call("set_default_sound_bus", bus);

        public void PlaySound(string identifier, AudioStream sound, GDC.Dictionary info) => Source.Call("play_sound", identifier, sound, info);

        public void PlaySound(string identifier, AudioStream sound, Vector2? position = null, float? volumeDb = null, float? pitchScale = null, string bus = null)
        {
            var info = new GDC.Dictionary();
            if (position != null) info["position"] = position.Value;
            if (volumeDb != null) info["volume_db"] = volumeDb.Value;
            if (pitchScale != null) info["pitch_scale"] = pitchScale.Value;
            if (bus != null) info["bus"] = bus;
            Source.Call("play_sound", identifier, sound, info);
        }

        public bool EnsureCurrentTickInputComplete() => (bool)Source.Call("ensure_current_tick_input_complete");

        public string OrderedDict2Str(GDC.Dictionary dict) => (string)Source.Call("ordered_dict2str");
        #endregion

        #region Signal Events
        public event Action SyncStarted;
        public event Action SyncStopped;
        public event Action SyncLost;
        public event Action SyncRegained;

        public delegate void SyncErrorDelegate(string message);
        public event SyncErrorDelegate SyncError;

        public delegate void SkipTicksFlaggedDelegate(long count);
        public event SkipTicksFlaggedDelegate SkipTicksFlagged;

        public delegate void RollbackFlaggedDelegate(long tick);
        public event RollbackFlaggedDelegate RollbackFlagged;

        public delegate void PredictionMissedDelegate(long tick, long peerId, GDC.Dictionary localInput, GDC.Dictionary remoteInput);
        public event PredictionMissedDelegate PredictionMissed;

        public delegate void RemoteStateMismatchDelegate(long tick, long peerId, long localHash, long remoteHash);
        public event RemoteStateMismatchDelegate RemoteStateMismatch;

        public delegate void PeerAddedDelegate(long peerId);
        public event PeerAddedDelegate PeerAdded;

        public delegate void PeerRemovedDelegate(long peerId);
        public event PeerRemovedDelegate PeerRemoved;

        public delegate void PeerPingedBackDelegate(GodotObject peer);
        public event PeerPingedBackDelegate PeerPingedBack;

        public delegate void StatedLoadedDelegate(long rollbackTicks);
        public event StatedLoadedDelegate StateLoaded;

        public delegate void TickFinishedDelegate(bool isRollback);
        public event TickFinishedDelegate TickFinished;

        public delegate void TickRetiredDelegate(long tick);
        public event TickRetiredDelegate TickRetired;

        public delegate void TickInputCompleteDelegate(long tick);
        public event TickInputCompleteDelegate TickInputComplete;

        public delegate void SceneSpawnedDelegate(string name, Node spawnedNode, PackedScene scene, GDC.Dictionary data);
        public event SceneSpawnedDelegate SceneSpawned;

        public delegate void SceneDespawnedDelegate(string name, Node node);
        public event SceneDespawnedDelegate SceneDespawned;

        public event Action InterpolationFrame;
        #endregion

        #region Signal Forwarding
        protected override void ForwardSignalsToEvents()
        {
            Source.Connect("sync_started", new Callable(this, nameof(OnSyncStarted)));
            Source.Connect("sync_stopped", new Callable(this, nameof(OnSyncStopped)));
            Source.Connect("sync_lost", new Callable(this, nameof(OnSyncLost)));
            Source.Connect("sync_regained", new Callable(this, nameof(OnSyncRegained)));
            Source.Connect("sync_error", new Callable(this, nameof(OnSyncError)));

            Source.Connect("skip_ticks_flagged", new Callable(this, nameof(OnSkipTicksFlagged)));
            Source.Connect("rollback_flagged", new Callable(this, nameof(OnRollbackFlagged)));
            Source.Connect("prediction_missed", new Callable(this, nameof(OnPredictionMissed)));
            Source.Connect("remote_state_mismatch", new Callable(this, nameof(OnRemoteStateMismatch)));

            Source.Connect("peer_added", new Callable(this, nameof(OnPeerAdded)));
            Source.Connect("peer_removed", new Callable(this, nameof(OnPeerRemoved)));
            Source.Connect("peer_pinged_back", new Callable(this, nameof(OnPeerPingedBack)));

            Source.Connect("state_loaded", new Callable(this, nameof(OnStateLoaded)));
            Source.Connect("tick_finished", new Callable(this, nameof(OnTickFinished)));
            Source.Connect("tick_retired", new Callable(this, nameof(OnTickRetired)));
            Source.Connect("tick_input_complete", new Callable(this, nameof(OnTickInputComplete)));
            Source.Connect("scene_spawned", new Callable(this, nameof(OnSceneSpawned)));
            Source.Connect("scene_despawned", new Callable(this, nameof(OnSceneDespawned)));
            Source.Connect("interpolation_frame", new Callable(this, nameof(OnInterpolationFrame)));
        }

        private void OnInterpolationFrame()
        {
            InterpolationFrame?.Invoke();
        }

        private void OnSceneDespawned(string name, Node node)
        {
            SceneDespawned?.Invoke(name, node);
        }

        private void OnSceneSpawned(string name, Node spawnedNode, PackedScene scene, GDC.Dictionary data)
        {
            SceneSpawned?.Invoke(name, spawnedNode, scene, data);
        }

        private void OnTickInputComplete(long tick)
        {
            TickInputComplete?.Invoke(tick);
        }

        private void OnTickRetired(long tick)
        {
            TickRetired?.Invoke(tick);
        }

        private void OnTickFinished(bool isRollback)
        {
            TickFinished?.Invoke(isRollback);
        }

        private void OnStateLoaded(long rollbackTicks)
        {
            StateLoaded?.Invoke(rollbackTicks);
        }

        private void OnPeerPingedBack(GodotObject peer)
        {
            PeerPingedBack?.Invoke(peer);
        }

        private void OnPeerRemoved(long peerId)
        {
            PeerRemoved?.Invoke(peerId);
        }

        private void OnPeerAdded(long peerId)
        {
            PeerAdded?.Invoke(peerId);
        }

        private void OnRemoteStateMismatch(long tick, long peerId, long localHash, long remoteHash)
        {
            RemoteStateMismatch?.Invoke(tick, peerId, localHash, remoteHash);
        }

        private void OnPredictionMissed(long tick, long peerId, GDC.Dictionary localInput, GDC.Dictionary remoteInput)
        {
            PredictionMissed?.Invoke(tick, peerId, localInput, remoteInput);
        }

        private void OnRollbackFlagged(long tick)
        {
            RollbackFlagged?.Invoke(tick);
        }

        private void OnSkipTicksFlagged(long count)
        {
            SkipTicksFlagged?.Invoke(count);
        }

        private void OnSyncError(string msg)
        {
            SyncError?.Invoke(msg);
        }

        private void OnSyncRegained()
        {
            SyncRegained?.Invoke();
        }

        private void OnSyncStopped()
        {
            SyncStopped?.Invoke();
        }

        private void OnSyncLost()
        {
            SyncLost?.Invoke();
        }

        private void OnSyncStarted()
        {
            SyncStarted?.Invoke();
        }
        #endregion
    }
}
