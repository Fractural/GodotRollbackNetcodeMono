using Fractural.Utils;
using Godot;
using Godot.Collections;
using GodotRollbackNetcode;
using System;

namespace Game
{
    public class MonoPlayer : Sprite, IGetLocalInput, INetworkProcess, INetworkSerializable, IPredictRemoteInput, IInterpolateState
    {
        [Export]
        public string InputPrefix { get; set; } = "player1_";

        enum PlayerInputKey
        {
            MovementVector,
        }

        public Dictionary _SaveState()
        {
            return new Dictionary()
            {
                [nameof(Position)] = Position
            };
        }

        public void _LoadState(Dictionary state)
        {
            Position = state.Get<Vector2>(nameof(Position));
        }

        public void _InterpolateState(Dictionary oldState, Dictionary newState, float weight)
        {
            Position = oldState.Get<Vector2>(nameof(Position)).Lerp(
                newState.Get<Vector2>(nameof(Position)),
                weight);
        }

        public Dictionary _GetLocalInput()
        {
            var inputVector = Input.GetVector(InputPrefix + "left", InputPrefix + "right", InputPrefix + "up", InputPrefix + "down");
            var input = new Dictionary();
            if (inputVector != Vector2.Zero)
                input[PlayerInputKey.MovementVector] = inputVector;
            return input;
        }

        public Dictionary _PredictRemoteInput(Dictionary previousInput, int ticksSinceRealInput)
        {
            var input = previousInput.Duplicate();
            if (ticksSinceRealInput > 5)
                input.Remove(PlayerInputKey.MovementVector);
            return input;
        }

        public void _NetworkProcess(Dictionary input)
        {
            var inputVector = input.Get(PlayerInputKey.MovementVector, Vector2.Zero);
            Position += inputVector * 8;
        }

    }
}