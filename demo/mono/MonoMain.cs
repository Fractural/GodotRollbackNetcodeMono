using Godot;
using GodotRollbackNetcode;
using System.Linq;

namespace Game
{
    public class MonoMain : Node2D
    {
        const string LOG_FILE_DIRECTORY = "user://detailed_logs";

        bool loggingEnabled = true;

        Control mainMenu;
        WindowDialog connectionPanel;
        LineEdit hostField;
        LineEdit portField;
        Label messageLabel;
        Label syncLostLabel;
        MonoPlayer serverPlayer;
        MonoPlayer clientPlayer;
        Button resetButton;

        public override void _Ready()
        {
            mainMenu = GetNode<Control>("CanvasLayer/MainMenu");
            connectionPanel = GetNode<WindowDialog>("CanvasLayer/ConnectionPanel");
            hostField = GetNode<LineEdit>("CanvasLayer/ConnectionPanel/GridContainer/HostField");
            portField = GetNode<LineEdit>("CanvasLayer/ConnectionPanel/GridContainer/PortField");
            messageLabel = GetNode<Label>("CanvasLayer/MessageLabel");
            syncLostLabel = GetNode<Label>("CanvasLayer/SyncLostLabel");
            resetButton = GetNode<Button>("CanvasLayer/ResetButton");
            serverPlayer = GetNode<MonoPlayer>("ServerPlayer");
            clientPlayer = GetNode<MonoPlayer>("ClientPlayer");

            GetTree().Connect("network_peer_connected", this, nameof(OnNetworkPeerConnected));
            GetTree().Connect("network_peer_disconnected", this, nameof(OnNetworkPeerDisconnected));
            GetTree().Connect("server_disconnected", this, nameof(OnServerDisconnected));

            SyncManager.Global.SyncStarted += OnSyncManagerSyncStarted;
            SyncManager.Global.SyncStopped += OnSyncManagerSyncStopped;
            SyncManager.Global.SyncLost += OnSyncManagerSyncLost;
            SyncManager.Global.SyncRegained += OnSyncManagerSyncRegained;
            SyncManager.Global.SyncError += OnSyncManagerSyncError;

            syncLostLabel.Visible = false;

            var cmdlineArgs = OS.GetCmdlineArgs().OfType<string>();
            if (cmdlineArgs.Contains("server"))
                OnServerButtonPressed();
            else if (cmdlineArgs.Contains("client"))
                OnClientButtonPressed();
        }

        public override void _Notification(int what)
        {
            if (what == NotificationPredelete)
            {
                SyncManager.Global.SyncStarted -= OnSyncManagerSyncStarted;
                SyncManager.Global.SyncStopped -= OnSyncManagerSyncStopped;
                SyncManager.Global.SyncLost -= OnSyncManagerSyncLost;
                SyncManager.Global.SyncRegained -= OnSyncManagerSyncRegained;
                SyncManager.Global.SyncError -= OnSyncManagerSyncError;
            }
        }

        private void OnServerButtonPressed()
        {
            var peer = new NetworkedMultiplayerENet();
            if (!int.TryParse(portField.Text, out int port))
                return;
            peer.CreateServer(port, 1);
            GetTree().NetworkPeer = peer;
            messageLabel.Text = "Listening...";
            connectionPanel.Visible = false;
            mainMenu.Visible = false;
        }

        private void OnClientButtonPressed()
        {
            if (!int.TryParse(portField.Text, out int port))
                return;
            var peer = new NetworkedMultiplayerENet();
            peer.CreateClient(hostField.Text, port);
            GetTree().NetworkPeer = peer;
            messageLabel.Text = "Connecting...";
            connectionPanel.Visible = false;
            mainMenu.Visible = false;
        }

        private async void OnNetworkPeerConnected(int peerId)
        {
            messageLabel.Text = "Connected with id: " + peerId;
            SyncManager.Global.AddPeer(peerId);

            serverPlayer.SetNetworkMaster(1);
            if (SyncManager.Global.NetworkAdaptor.IsNetworkHost())
                clientPlayer.SetNetworkMaster(peerId);
            else
                clientPlayer.SetNetworkMaster(GetTree().GetNetworkUniqueId());

            if (SyncManager.Global.NetworkAdaptor.IsNetworkHost())
            {
                messageLabel.Text = "Starting...";
                await ToSignal(GetTree().CreateTimer(2.0f), "timeout");
                SyncManager.Global.Start();
            }
        }

        private void OnNetworkPeerDisconnected(int peerId)
        {
            messageLabel.Text = "Disconnected with id: " + peerId;
            SyncManager.Global.RemovePeer(peerId);
        }

        private void OnServerDisconnected()
        {
            OnNetworkPeerDisconnected(1);
        }

        private void OnResetButtonPressed()
        {
            SyncManager.Global.Stop();
            SyncManager.Global.ClearPeers();
            var peer = GetTree().NetworkPeer;
            if (peer is NetworkedMultiplayerENet enetPeer)
                enetPeer.CloseConnection();
            GetTree().ReloadCurrentScene();
        }

        private void OnSyncManagerSyncStarted()
        {
            messageLabel.Text = "Started!";

            if (loggingEnabled && !SyncReplay.Instance.Active)
            {
                var dir = new Directory();
                if (!dir.DirExists(LOG_FILE_DIRECTORY))
                    dir.MakeDir(LOG_FILE_DIRECTORY);

                var datetime = OS.GetDatetime(true);
                string logFileName = string.Format("{0}-{1}-{2}_{3}-{4}-{5}_peer-{6}.log", datetime["year"], datetime["month"], datetime["day"], datetime["hour"], datetime["minute"], datetime["second"], SyncManager.Global.NetworkAdaptor.GetNetworkUniqueId());

                SyncManager.Global.StartLogging(LOG_FILE_DIRECTORY + "/" + logFileName);
            }
        }

        private void OnSyncManagerSyncStopped()
        {
            if (loggingEnabled)
                SyncManager.Global.StopLogging();
        }

        private void OnSyncManagerSyncLost()
        {
            syncLostLabel.Visible = true;
        }

        private void OnSyncManagerSyncRegained()
        {
            syncLostLabel.Visible = false;
        }

        private void OnSyncManagerSyncError(string msg)
        {
            messageLabel.Text = "Fatal sync error: " + msg;
            syncLostLabel.Visible = false;

            var peer = GetTree().NetworkPeer;
            if (peer is NetworkedMultiplayerENet enetPeer)
                enetPeer.CloseConnection();
            SyncManager.Global.ClearPeers();
        }

        public void SetupMatchForReplay(int myPeerId, Godot.Collections.Array peerIds, Godot.Collections.Dictionary matchInfo)
        {
            connectionPanel.Visible = false;
            mainMenu.Visible = false;
            resetButton.Visible = false;
        }

        private void OnOnlineButtonPressed()
        {
            SyncManager.Global.ResetNetworkAdaptor();
            connectionPanel.PopupCentered();
        }

        private void OnLocalButtonPressed()
        {
            var dummyNetworkAdaptor = GD.Load<GDScript>("res://addons/godot-rollback-netcode/DummyNetworkAdaptor.gd");
            SyncManager.Global.NetworkAdaptor = new NetworkAdaptorWrapper((Godot.Object)dummyNetworkAdaptor.New());
            SyncManager.Global.Start();
            clientPlayer.InputPrefix = "player2_";

            mainMenu.Visible = false;
        }
    }
}