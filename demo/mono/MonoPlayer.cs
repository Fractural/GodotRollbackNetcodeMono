using Godot;
using Godot.Collections;
using GodotRollbackNetcode;
using System;

namespace Game
{
    public class MonoPlayer : Sprite, IGetLocalInput, INetworkProcess, INetworkSerializable, IPredictRemoteInput, IInterpolateState
    {
        [Export]
        public string inputPrefix = "player1_";

        enum PlayerInputKey
        {
            INPUT_VECTOR
        }

        public Dictionary _SaveState()
        {
            var state = new Dictionary();
            state["position"] = Position;
            return state;
        }

        public void _LoadState(Dictionary state)
        {
            Position = (Vector2)state["position"];
        }

        public void _InterpolateState(Dictionary oldState, Dictionary newState, float weight)
        {
            if (oldState.Get("teleporting", false) || newState.Get("teleporting", false))
                return;
            Position = Utils.Lerp((Vector2)oldState["position"], (Vector2)newState["position"], weight);
        }

        public Dictionary _GetLocalInput()
        {
            var inputVector = Input.GetVector(inputPrefix + "left", inputPrefix + "right", inputPrefix + "up", inputPrefix + "down");
            var input = new Dictionary();
            if (inputVector != Vector2.Zero)
                input[PlayerInputKey.INPUT_VECTOR] = inputVector;

            return input;
        }

        public Dictionary _PredictRemoteInput(Dictionary previousInput, int ticksSinceRealInput)
        {
            var input = previousInput.Duplicate();
            if (ticksSinceRealInput > 5)
                input.Remove("input_vector");
            return input;
        }

        public void _NetworkProcess(Dictionary input)
        {
            var inputVector = input.Get(PlayerInputKey.INPUT_VECTOR, Vector2.Zero);
            Position += inputVector * 8;
        }

    }
}