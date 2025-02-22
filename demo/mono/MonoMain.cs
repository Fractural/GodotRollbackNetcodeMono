using Godot;
using GodotRollbackNetcode;
using System.Linq;

namespace Game
{
    public partial class MonoMain : Node
    {
        const string LOG_FILE_DIRECTORY = "user://detailed_logs";

        bool loggingEnabled = true;

        [Export] Control mainMenu;
        [Export] Window connectionPanel;
        [Export] LineEdit hostField;
        [Export] LineEdit portField;
        [Export] Label messageLabel;
        [Export] Label syncLostLabel;
        [Export] MonoPlayer serverPlayer;
        [Export] MonoPlayer clientPlayer;
        [Export] Button serverButton;
        [Export] Button clientButton;
        [Export] Button resetButton;
        [Export] Button localButton;
        [Export] Button onlineButton;

        public override void _Ready()
        {
            Multiplayer.PeerConnected += OnNetworkPeerConnected;
            Multiplayer.PeerDisconnected += OnNetworkPeerDisconnected;
            Multiplayer.ServerDisconnected += OnServerDisconnected;

            SyncManager.Global.SyncStarted += OnSyncManagerSyncStarted;
            SyncManager.Global.SyncStopped += OnSyncManagerSyncStopped;
            SyncManager.Global.SyncLost += OnSyncManagerSyncLost;
            SyncManager.Global.SyncRegained += OnSyncManagerSyncRegained;
            SyncManager.Global.SyncError += OnSyncManagerSyncError;

            syncLostLabel.Visible = false;

            serverButton.Pressed += OnServerButtonPressed;
            clientButton.Pressed += OnClientButtonPressed;
            localButton.Pressed += OnLocalButtonPressed;
            onlineButton.Pressed += OnOnlineButtonPressed;
            resetButton.Pressed += OnResetButtonPressed;

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
            var peer = new ENetMultiplayerPeer();
            if (!int.TryParse(portField.Text, out int port))
                return;
            peer.CreateServer(port, 1);
            Multiplayer.MultiplayerPeer = peer;
            messageLabel.Text = "Listening...";
            connectionPanel.Visible = false;
            mainMenu.Visible = false;
        }

        private void OnClientButtonPressed()
        {
            if (!int.TryParse(portField.Text, out int port))
                return;
            var peer = new ENetMultiplayerPeer();
            peer.CreateClient(hostField.Text, port);
            Multiplayer.MultiplayerPeer = peer;
            messageLabel.Text = "Connecting...";
            connectionPanel.Visible = false;
            mainMenu.Visible = false;
        }

        private async void OnNetworkPeerConnected(long peerId)
        {
            messageLabel.Text = "Connected with id: " + peerId;
            SyncManager.Global.AddPeer(peerId);

            serverPlayer.SetMultiplayerAuthority(1);
            if (SyncManager.Global.NetworkAdaptor.IsNetworkHost())
                clientPlayer.SetMultiplayerAuthority((int)peerId);
            else
                clientPlayer.SetMultiplayerAuthority(Multiplayer.MultiplayerPeer.GetUniqueId());

            if (SyncManager.Global.NetworkAdaptor.IsNetworkHost())
            {
                messageLabel.Text = "Starting...";
                await ToSignal(GetTree().CreateTimer(2.0f), "timeout");
                SyncManager.Global.Start();
            }
        }

        private void OnNetworkPeerDisconnected(long peerId)
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
            var peer = Multiplayer.MultiplayerPeer;
            if (peer is ENetMultiplayerPeer enetPeer)
                enetPeer.Close();
            GetTree().ReloadCurrentScene();
        }

        private void OnSyncManagerSyncStarted()
        {
            messageLabel.Text = "Started!";

            if (loggingEnabled && !SyncReplay.Instance.Active)
            {
                if (!DirAccess.DirExistsAbsolute(LOG_FILE_DIRECTORY))
                    DirAccess.MakeDirAbsolute(LOG_FILE_DIRECTORY);

                var datetime = Time.GetDatetimeDictFromSystem(true);
                string logFileName = string.Format("{0}-{1}-{2}_{3}-{4}-{5}_peer-{6}.log", datetime["year"], datetime["month"], datetime["day"], datetime["hour"], datetime["minute"], datetime["second"], SyncManager.Global.NetworkAdaptor.GetUniqueId());

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

            var peer = Multiplayer.MultiplayerPeer;
            if (peer is ENetMultiplayerPeer enetPeer)
                enetPeer.Close();
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
            SyncManager.Global.NetworkAdaptor = new NetworkAdaptorWrapper((GodotObject)dummyNetworkAdaptor.New());
            SyncManager.Global.Start();
            clientPlayer.InputPrefix = "player2_";

            mainMenu.Visible = false;
        }
    }
}