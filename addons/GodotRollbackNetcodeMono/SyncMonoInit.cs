using Godot;

namespace GodotRollbackNetcode
{
    public partial class SyncMonoInit : Node
    {
        public override void _Ready()
        {
            SyncManager.Init(this);
            SyncReplay.Init(this);
        }
    }
}
