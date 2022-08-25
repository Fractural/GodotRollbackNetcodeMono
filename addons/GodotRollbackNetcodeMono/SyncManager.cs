using Godot;
using System;
using GDDictionary = Godot.Collections.Dictionary;

namespace GodotRollbackNetcode
{
    public class SyncManager : GDScriptWrapper
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
        #endregion

        #region Object Variables
        private NetworkAdaptorWrapper networkAdaptorWrapper;
        public INetworkAdaptor NetworkAdaptor
        {
            get
            {
                var source = (Godot.Object)Source.Get("network_adaptor");
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
                else if (value is Godot.Object obj)
                    Source.Set("network_adaptor", obj);
                else
                    GD.PrintErr($"You can only assign {nameof(BaseNetworkAdaptor)}, {nameof(NetworkAdaptorWrapper)}, or {nameof(Godot)}.{nameof(Godot.Object)} to {nameof(SyncManager)}.{nameof(NetworkAdaptor)}");
            }
        }

        private MessageSerializerWrapper messageSerializerWrapper;
        public IMessageSerializer MessageSerializer
        {
            get
            {
                var source = (Godot.Object)Source.Get("message_serializer");
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
                else if (value is Godot.Object obj)
                    Source.Set("network_adaptor", obj);
                else
                    GD.PrintErr($"You can only assign {nameof(BaseMessageSerializer)}, {nameof(MessageSerializerWrapper)}, or {nameof(Godot)}.{nameof(Godot.Object)} to {nameof(SyncManager)}.{nameof(MessageSerializer)}");
            }
        }

        private HashSerializerWrapper hashSerializerWrapper;
        public IHashSerializer HashSerializer
        {
            get
            {
                var source = (Godot.Object)Source.Get("hash_serializer");
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
                else if (value is Godot.Object obj)
                    Source.Set("network_adaptor", obj);
                else
                    GD.PrintErr($"You can only assign {nameof(BaseHashSerializer)}, {nameof(HashSerializerWrapper)}, or {nameof(Godot)}.{nameof(Godot.Object)} to {nameof(SyncManager)}.{nameof(HashSerializer)}");
            }
        }
        #endregion

        #region Collection Variables
        public GDDictionary Peers
        {
            get => (GDDictionary)Source.Get("peers");
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

        public GDDictionary MechanizedInputReceived
        {
            get => (GDDictionary)Source.Get("mechanized_input_received");
            set => Source.Set("mechanized_input_received", value);
        }

        public int MechanizedRollbackTicks
        {
            get => (int)Source.Get("mechanized_rollback_ticks");
            set => Source.Set("mechanized_rollback_ticks", value);
        }
        #endregion

        #region General Variables
        public int MaxBufferSize
        {
            get => (int)Source.Get("max_buffer_size");
            set => Source.Set("max_buffer_size", value);
        }

        public int TicksToCalculateAdvantage
        {
            get => (int)Source.Get("ticks_to_calculate_advantage");
            set => Source.Set("ticks_to_calculate_advantage", value);
        }

        public int InputDelay
        {
            get => (int)Source.Get("input_delay");
            set => Source.Set("input_delay", value);
        }

        public int MaxInputFramesPerMessage
        {
            get => (int)Source.Get("max_input_frames_per_message");
            set => Source.Set("max_input_frames_per_message", value);
        }

        public int MaxMessagesAtOnce
        {
            get => (int)Source.Get("max_messages_at_once");
            set => Source.Set("max_messages_at_once", value);
        }

        public int MaxTicksToRegainSync
        {
            get => (int)Source.Get("max_ticks_to_regain_sync");
            set => Source.Set("max_ticks_to_regain_sync", value);
        }

        public int MinLagToRegainSync
        {
            get => (int)Source.Get("min_lag_to_regain_sync");
            set => Source.Set("min_lag_to_regain_sync", value);
        }

        public bool Interpolation
        {
            get => (bool)Source.Get("interpolation");
            set => Source.Set("interpolation", value);
        }

        public int MaxStateMismatchCount
        {
            get => (int)Source.Get("max_state_mismatch_count");
            set => Source.Set("max_state_mismatch_count", value);
        }
        #endregion

        #region Debug Variables
        public int DebugrollbackTicks
        {
            get => (int)Source.Get("debug_rollback_ticks");
            set => Source.Set("debug_rollback_ticks", value);
        }

        public int DebugRandomRollbackTicks
        {
            get => (int)Source.Get("debug_random_rollback_ticks");
            set => Source.Set("debug_random_rollback_ticks", value);
        }

        public int DebugMessageBytes
        {
            get => (int)Source.Get("debug_message_bytes");
            set => Source.Set("debug_message_bytes", value);
        }

        public int DebugSkipNthMessage
        {
            get => (int)Source.Get("debug_skip_nth_message");
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
        public int InputTick => (int)Source.Get("input_tick");
        public int CurrentTick => (int)Source.Get("current_tick");
        public int SkipTicks => (int)Source.Get("skip_ticks");
        public int RollbackTicks => (int)Source.Get("rollback_ticks");
        public int RequestedInputCompleteTick => (int)Source.Get("requested_input_complete_tick");
        public bool Started => (bool)Source.Get("started");
        public float TickTime => (float)Source.Get("tick_time");
        #endregion

        public SyncManager() { }

        public SyncManager(Godot.Object source) : base(source) { }

        #region Methods
        public void ResetNetworkAdaptor() => Source.Call("reset_network_adaptor");

        public void AddPeer(int peerId) => Source.Call("add_peer", peerId);

        public bool HasPeer(int peerId) => (bool)Source.Call("has_peer", peerId);

        public void RemovePeer(int peerid) => Source.Call("remove_peer", peerid);

        public void ClearPeers() => Source.Call("clear_peers");

        public void StartLogging(string logFilePath) => StartLogging(logFilePath, new GDDictionary());

        public void StartLogging(string logFilePath, GDDictionary matchInfo) => Source.Call("start_logging", logFilePath, matchInfo);

        public void StopLogging() => Source.Call("stop_logging");

        public void Start() => Source.Call("start");

        public void Stop() => Source.Call("stop");

        public void ResetMechanizedData() => Source.Call("reset_mechanized_data");


        public void ExecuteMechanizedTick() => Source.Call("execute_mechanized_tick");

        public void ExecuteMechanizedInterpolationFrame(float delta) => Source.Call("execute_mechanized_interpolation_frame", delta);

        public void ExecuteMechanizedInterframe() => Source.Call("execute_mechanized_interframe");

        public GDDictionary SortDictionaryKeys(GDDictionary dictionary) => (GDDictionary)Source.Call("sort_dictionary_keys");

        public Node Spawn(string name, Node parent, PackedScene scene, GDDictionary data, bool rename = true, string signalName = "") => (Node)Source.Call("spawn", name, parent, scene, data, rename, signalName);

        public void Despawn(Node node) => Source.Call("despawn", node);

        public bool IsInRollback => (bool)Source.Call("is_in_rollback");
        public bool IsRespawning => (bool)Source.Call("is_respawning");

        public void SetDefaultSoundBus(string bus) => Source.Call("set_default_sound_bus", bus);

        public void PlaySound(string identifier, AudioStream sound, GDDictionary info) => Source.Call("play_sound", identifier, sound, info);

        public void PlaySound(string identifier, AudioStream sound, Vector2? position = null, float? volumeDb = null, float? pitchScale = null, string bus = null)
        {
            var info = new GDDictionary();
            if (position != null) info["position"] = position;
            if (volumeDb != null) info["volume_db"] = volumeDb;
            if (pitchScale != null) info["pitch_scale"] = pitchScale;
            if (bus != null) info["bus"] = bus;
            Source.Call("play_sound", identifier, sound, info);
        }

        public bool EnsureCurrentTickInputComplete() => (bool)Source.Call("ensure_current_tick_input_complete");

        public string OrderedDict2Str(GDDictionary dict) => (string)Source.Call("ordered_dict2str");
        #endregion

        #region Signal Events
        public event Action SyncStarted;
        public event Action SyncStopped;
        public event Action SyncLost;
        public event Action SyncRegained;

        public delegate void SyncErrorDelegate(string message);
        public event SyncErrorDelegate SyncError;

        public delegate void SkipTicksFlaggedDelegate(int count);
        public event SkipTicksFlaggedDelegate SkipTicksFlagged;

        public delegate void RollbackFlaggedDelegate(int tick);
        public event RollbackFlaggedDelegate RollbackFlagged;

        public delegate void PredictionMissedDelegate(int tick, int peerId, GDDictionary localInput, GDDictionary remoteInput);
        public event PredictionMissedDelegate PredictionMissed;

        public delegate void RemoteStateMismatchDelegate(int tick, int peerId, int localHash, int remoteHash);
        public event RemoteStateMismatchDelegate RemoteStateMismatch;

        public delegate void PeerAddedDelegate(int peerId);
        public event PeerAddedDelegate PeerAdded;

        public delegate void PeerRemovedDelegate(int peerId);
        public event PeerRemovedDelegate PeerRemoved;

        public delegate void PeerPingedBackDelegate(Godot.Object peer);
        public event PeerPingedBackDelegate PeerPingedBack;

        public delegate void StatedLoadedDelegate(int rollbackTicks);
        public event StatedLoadedDelegate StateLoaded;

        public delegate void TickFinishedDelegate(bool isRollback);
        public event TickFinishedDelegate TickFinished;

        public delegate void TickRetiredDelegate(int tick);
        public event TickRetiredDelegate TickRetired;

        public delegate void TickInputCompleteDelegate(int tick);
        public event TickInputCompleteDelegate TickInputComplete;

        public delegate void SceneSpawnedDelegate(string name, Node spawnedNode, PackedScene scene, GDDictionary data);
        public event SceneSpawnedDelegate SceneSpawned;

        public delegate void SceneDespawnedDelegate(string name, Node node);
        public event SceneDespawnedDelegate SceneDespawned;

        public event Action InterpolationFrame;
        #endregion

        #region Signal Forwarding
        protected override void ForwardSignalsToEvents()
        {
            Source.Connect("sync_started", this, nameof(OnSyncStarted));
            Source.Connect("sync_stopped", this, nameof(OnSyncStopped));
            Source.Connect("sync_lost", this, nameof(OnSyncLost));
            Source.Connect("sync_regained", this, nameof(OnSyncRegained));
            Source.Connect("sync_error", this, nameof(OnSyncError));

            Source.Connect("skip_ticks_flagged", this, nameof(OnSkipTicksFlagged));
            Source.Connect("rollback_flagged", this, nameof(OnRollbackFlagged));
            Source.Connect("prediction_missed", this, nameof(OnPredictionMissed));
            Source.Connect("remote_state_mismatch", this, nameof(OnRemoteStateMismatch));

            Source.Connect("peer_added", this, nameof(OnPeerAdded));
            Source.Connect("peer_removed", this, nameof(OnPeerRemoved));
            Source.Connect("peer_pinged_back", this, nameof(OnPeerPingedBack));

            Source.Connect("state_loaded", this, nameof(OnStateLoaded));
            Source.Connect("tick_finished", this, nameof(OnTickFinished));
            Source.Connect("tick_retired", this, nameof(OnTickRetired));
            Source.Connect("tick_input_complete", this, nameof(OnTickInputComplete));
            Source.Connect("scene_spawned", this, nameof(OnSceneSpawned));
            Source.Connect("scene_despawned", this, nameof(OnSceneDespawned));
            Source.Connect("interpolation_frame", this, nameof(OnInterpolationFrame));
        }

        private void OnInterpolationFrame()
        {
            InterpolationFrame?.Invoke();
        }

        private void OnSceneDespawned(string name, Node node)
        {
            SceneDespawned?.Invoke(name, node);
        }

        private void OnSceneSpawned(string name, Node spawnedNode, PackedScene scene, GDDictionary data)
        {
            SceneSpawned?.Invoke(name, spawnedNode, scene, data);
        }

        private void OnTickInputComplete(int tick)
        {
            TickInputComplete?.Invoke(tick);
        }

        private void OnTickRetired(int tick)
        {
            TickRetired?.Invoke(tick);
        }

        private void OnTickFinished(bool isRollback)
        {
            TickFinished?.Invoke(isRollback);
        }

        private void OnStateLoaded(int rollbackTicks)
        {
            StateLoaded?.Invoke(rollbackTicks);
        }

        private void OnPeerPingedBack(Godot.Object peer)
        {
            PeerPingedBack?.Invoke(peer);
        }

        private void OnPeerRemoved(int peerId)
        {
            PeerRemoved?.Invoke(peerId);
        }

        private void OnPeerAdded(int peerId)
        {
            PeerAdded?.Invoke(peerId);
        }

        private void OnRemoteStateMismatch(int tick, int peerId, int localHash, int remoteHash)
        {
            RemoteStateMismatch?.Invoke(tick, peerId, localHash, remoteHash);
        }

        private void OnPredictionMissed(int tick, int peerId, GDDictionary localInput, GDDictionary remoteInput)
        {
            PredictionMissed?.Invoke(tick, peerId, localInput, remoteInput);
        }

        private void OnRollbackFlagged(int tick)
        {
            RollbackFlagged?.Invoke(tick);
        }

        private void OnSkipTicksFlagged(int count)
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
