using Godot;
using GodotRollbackNetcode;
using GDC = Godot.Collections;

namespace Game
{
    public partial class MonoWalkingSprite : Sprite2D, INetworkProcess, INetworkSerializable, IInterpolateState
    {
        [Export]
        private Vector2 startPosition;
        [Export]
        private float speed;
        [Export]
        private Vector2 direction;
        [Export]
        private bool teleporting;

        public void Construct(Vector2 _startPosition, float _speed, Vector2 _direction)
        {
            speed = _speed;
            direction = _direction;
            startPosition = _startPosition;
            Position = startPosition;
        }

        public void _interpolate_state(GDC.Dictionary oldState, GDC.Dictionary newState, float weight)
        {
            if (oldState.GetStateValue<bool>("teleporting") || newState.GetStateValue<bool>("teleporting"))
                return;

            Position = oldState.GetStateValue<Vector2>("position").Lerp(newState.GetStateValue<Vector2>("position"), weight);
        }

        public void _load_state(GDC.Dictionary state)
        {
            Position = state.GetStateValue<Vector2>("position");
        }

        public void _network_process(GDC.Dictionary input)
        {
            Position += speed * direction;
            var screenSize = DisplayServer.WindowGetSize();
            teleporting = false;
            if (Position.X < 0 || Position.X > screenSize.X || Position.Y < 0 || Position.Y > screenSize.Y)
            {
                Position = startPosition;
                teleporting = true;
            }
        }

        public GDC.Dictionary _save_state()
        {
            return new GDC.Dictionary()
            {
                ["position"] = Position,
                ["teleporting"] = teleporting
            };
        }
    }
}