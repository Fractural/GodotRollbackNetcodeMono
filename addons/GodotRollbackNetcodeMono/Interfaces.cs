using GDDictionary = Godot.Collections.Dictionary;

namespace GodotRollbackNetcode
{
    public interface INetworkSync { }
    public interface IGetLocalInput : INetworkSync
    {
        GDDictionary _get_local_input();
    }

    public interface INetworkProcess : INetworkSync
    {
        void _network_process(GDDictionary input);
    }

    public interface INetworkPreProcess : INetworkSync
    {
        void _network_preprocess(GDDictionary input);
    }

    public interface INetworkPostProcess : INetworkSync
    {
        void _network_postprocess(GDDictionary input);
    }

    public interface IInterpolateState : INetworkSync
    {
        void _interpolate_state(GDDictionary oldState, GDDictionary newState, float weight);
    }

    public interface IPredictRemoteInput : INetworkSync
    {
        GDDictionary _predict_remote_input(GDDictionary previousInput, int ticksSinceRealInput);
    }

    public interface INetworkSerializable : INetworkSync
    {
        GDDictionary _save_state();
        void _load_state(GDDictionary state);
    }

    public interface INetworkSpawnPreProcess
    {
        void _network_spawn_preprocess(GDDictionary data);
    }

    public interface INetworkSpawn
    {
        void _network_spawn(GDDictionary data);
    }

    public interface INetworkDespawn
    {
        void _network_despawn();
    }
}
