using GDDictionary = Godot.Collections.Dictionary;
using GDArray = Godot.Collections.Array;

namespace GodotRollbackNetcode
{
    public interface INetworkSync { }
    public interface IGetLocalInput : INetworkSync
    {
        GDDictionary _GetLocalInput();
    }

    public interface INetworkProcess : INetworkSync
    {
        void _NetworkProcess(GDDictionary input);
    }

    public interface INetworkPreProcess : INetworkSync
    {
        void _NetworkPreProcess(GDDictionary input);
    }

    public interface INetworkPostProcess : INetworkSync
    {
        void _NetworkPostProcess(GDDictionary input);
    }

    public interface IInterpolateState : INetworkSync
    {
        void _InterpolateState(GDDictionary oldState, GDDictionary newState, float weight);
    }

    public interface IPredictRemoteInput : INetworkSync
    {
        GDDictionary _PredictRemoteInput(GDDictionary previousInput, int ticksSinceRealInput);
    }

    public interface INetworkSerializable : INetworkSync
    {
        GDDictionary _SaveState();
        void _LoadState(GDDictionary state);
    }

    public interface INetworkSpawnPreProcess
    {
        void _NetworkSpawnPreProcess(GDDictionary data);
    }

    public interface INetworkSpawn
    {
        void _NetworkSpawn(GDDictionary data);
    }

    public interface INetworkDespawn
    {
        void _NetworkDespawn();
    }
}
