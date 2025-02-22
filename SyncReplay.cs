using Godot;

namespace GodotRollbackNetcode
{
    public partial class SyncReplay : GDScriptWrapper
    {
        #region Singleton
        private static SyncReplay instance;
        public static SyncReplay Instance
        {
            get
            {
                if (instance == null)
                    GD.PrintErr("Expected C# SyncReplay singleton to be initialized. Did you forget to call SyncReplay.Init(node)?");
                return instance;
            }
        }
        public static void Init(Node node)
        {
            var autoloadedNode = node.GetNodeOrNull("/root/SyncReplay");
            if (autoloadedNode != null)
                instance = new SyncReplay(autoloadedNode);
        }
        #endregion

        public SyncReplay() { }

        public SyncReplay(GodotObject source) : base(source) { }

        #region Properties
        public bool Active
        {
            get => (bool)Source.Get("active");
            set => Source.Set("active", value);
        }

        public StreamPeerTcp Connection
        {
            get => (StreamPeerTcp)Source.Get("connection");
            set => Source.Set("connection", value);
        }

        public string MatchScenePath
        {
            get => (string)Source.Get("match_scene_path");
            set => Source.Set("match_scene_path", value);
        }

        public string MatchSceneMethod
        {
            get => (string)Source.Get("match_scene_method");
            set => Source.Set("match_scene_method", value);
        }
        #endregion
    }
}
