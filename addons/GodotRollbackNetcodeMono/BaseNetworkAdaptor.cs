using Godot;
using GDDictionary = Godot.Collections.Dictionary;

namespace GodotRollbackNetcode
{
    public interface INetworkAdaptor
    {
        void AttachNetworkAdaptor(SyncManager syncManager);
        void DetachNetworkAdaptor(SyncManager syncManager);
        void StopNetworkAdaptor(SyncManager syncManager);
        void Poll();
        void SendPing(int peerId, GDDictionary msg);
        void SendPingBack(int peerId, GDDictionary msg);
        void SendRemoteStart(int peerId);
        void SendRemoteStop(int peerId);
        void SendInputTick(int peerId, byte[] msg);
        bool IsNetworkHost();
        bool IsNetworkMasterForNode(Node node);
        int GetUniqueId();
    }

    public abstract partial class BaseNetworkAdaptor : Godot.RefCounted, INetworkAdaptor
    {
        private void attach_network_adaptor(GodotObject sync_manager) => AttachNetworkAdaptor(sync_manager.AsWrapper<SyncManager>());

        public virtual void AttachNetworkAdaptor(SyncManager syncManager) { }

        private void detach_network_adaptor(GodotObject sync_manager) => DetachNetworkAdaptor(sync_manager.AsWrapper<SyncManager>());

        public virtual void DetachNetworkAdaptor(SyncManager syncManager) { }

        private void stop_network_adaptor(GodotObject sync_manager) => StopNetworkAdaptor(sync_manager.AsWrapper<SyncManager>());

        public virtual void StopNetworkAdaptor(SyncManager syncManager) { }

        private void poll() => Poll();

        public virtual void Poll() { }

        private void send_ping(int peer_id, GDDictionary msg) => SendPing(peer_id, msg);

        public abstract void SendPing(int peerId, GDDictionary msg);

        private void send_ping_back(int peer_id, GDDictionary msg) => SendPingBack(peer_id, msg);

        public abstract void SendPingBack(int peerId, GDDictionary msg);

        private void send_remote_start(int peer_id) => SendRemoteStart(peer_id);

        public abstract void SendRemoteStart(int peerId);

        private void send_remote_stop(int peer_id) => SendRemoteStop(peer_id);

        public abstract void SendRemoteStop(int peerId);

        private void send_input_tick(int peer_id, byte[] msg) => SendInputTick(peer_id, msg);

        public abstract void SendInputTick(int peerId, byte[] msg);

        private bool is_network_host() => IsNetworkHost();

        public abstract bool IsNetworkHost();

        private bool is_network_master_for_node(Node node) => IsNetworkMasterForNode(node);

        public abstract bool IsNetworkMasterForNode(Node node);

        private int get_network_unique_id() => GetUniqueId();

        public abstract int GetUniqueId();
    }
}
