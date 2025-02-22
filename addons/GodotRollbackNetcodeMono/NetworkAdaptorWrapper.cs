using Godot;
using Godot.Collections;

namespace GodotRollbackNetcode
{
    public partial class NetworkAdaptorWrapper : GDScriptWrapper, INetworkAdaptor
    {
        public NetworkAdaptorWrapper() { }
        public NetworkAdaptorWrapper(GodotObject source) : base(source) { }

        public void AttachNetworkAdaptor(SyncManager syncManager) => Source.Call("attach_network_adaptor", syncManager.Source);
        public void DetachNetworkAdaptor(SyncManager syncManager) => Source.Call("detach_network_adaptor", syncManager.Source);
        public int GetUniqueId() => (int)Source.Call("get_unique_id");
        public bool IsNetworkHost() => (bool)Source.Call("is_network_host");
        public bool IsNetworkMasterForNode(Node node) => (bool)Source.Call("is_network_master_for_node", node);
        public void Poll() => Source.Call("poll");
        public void SendInputTick(int peerId, byte[] msg) => Source.Call("send_input_tick", peerId, msg);
        public void SendPing(int peerId, Dictionary msg) => Source.Call("send_ping", peerId, msg);
        public void SendPingBack(int peerId, Dictionary msg) => Source.Call("send_ping_back", peerId, msg);
        public void SendRemoteStart(int peerId) => Source.Call("send_remote_start", peerId);
        public void SendRemoteStop(int peerId) => Source.Call("send_remote_stop", peerId);
        public void StartNetworkAdaptor(SyncManager syncManager) => Source.Call("start_network_adaptor", syncManager.Source);
        public void StopNetworkAdaptor(SyncManager syncManager) => Source.Call("stop_network_adaptor", syncManager.Source);
    }
}
