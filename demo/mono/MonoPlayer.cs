using Godot;
using Godot.Collections;
using GodotRollbackNetcode;

namespace Game
{
    public partial class MonoPlayer : Sprite2D, IGetLocalInput, INetworkProcess, INetworkSerializable, IPredictRemoteInput, IInterpolateState
    {
        [Export]
        public string InputPrefix { get; set; } = "player1_";

        enum PlayerInputKey : int
        {
            MovementVector,
        }

        public Dictionary _save_state()
        {
            return new Dictionary()
            {
                [nameof(Position)] = Position
            };
        }

        public void _load_state(Dictionary state)
        {
            Position = state.Get<Vector2>(nameof(Position));
        }

        public void _interpolate_state(Dictionary oldState, Dictionary newState, float weight)
        {
            Position = oldState.Get<Vector2>(nameof(Position)).Lerp(
                newState.Get<Vector2>(nameof(Position)),
                weight);
        }

        public Dictionary _get_local_input()
        {
            var inputVector = Vector2.Zero;
            if (Input.IsActionPressed(InputPrefix + "left"))
                inputVector.X -= 1;
            if (Input.IsActionPressed(InputPrefix + "right"))
                inputVector.X += 1;
            if (Input.IsActionPressed(InputPrefix + "up"))
                inputVector.Y -= 1;
            if (Input.IsActionPressed(InputPrefix + "down"))
                inputVector.Y += 1;
            inputVector = inputVector.Normalized();
            //var inputVector = Input.GetVector(InputPrefix + "left", InputPrefix + "right", InputPrefix + "up", InputPrefix + "down");
            var input = new Dictionary();
            if (inputVector != Vector2.Zero)
                input[(int)PlayerInputKey.MovementVector] = inputVector;
            return input;
        }

        public Dictionary _predict_remote_input(Dictionary previousInput, int ticksSinceRealInput)
        {
            var input = previousInput.Duplicate();
            if (ticksSinceRealInput > 5)
                input.Remove((int)PlayerInputKey.MovementVector);
            return input;
        }

        public void _network_process(Dictionary input)
        {
            var inputVector = input.Get((int)PlayerInputKey.MovementVector, Vector2.Zero);
            Position += inputVector * 8;
        }

    }
}