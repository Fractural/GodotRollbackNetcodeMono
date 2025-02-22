using Godot;
using Godot.Collections;
using GodotRollbackNetcode;
using System;
using GDC = Godot.Collections;

namespace Game
{
    public class WalkingSprite : Sprite, INetworkProcess, INetworkSerializable, IInterpolateState
    {
        [Export]
        private Vector2 startPosition;
        [Export]
        private Vector2 position;
        [Export]
        private float speed;
        [Export]
        private Vector2 direction;

        public void Construct(Vector2 _startPosition, float _speed, Vector2 _direction)
        {
            speed = _speed;
            direction = _direction;
            startPosition = _startPosition;
            position = startPosition;
            Position = startPosition;
        }

        public void _interpolate_state(GDC.Dictionary oldState, GDC.Dictionary newState, float weight)
        {
            Position = oldState.GetStateValue<Vector2>("position").LinearInterpolate(newState.GetStateValue<Vector2>("position"), weight);
        }

        public void _load_state(GDC.Dictionary state)
        {
            position = state.GetStateValue<Vector2>("position");
        }

        public void _network_process(GDC.Dictionary input)
        {
            position += speed * direction;
            var screenSize = OS.GetScreenSize();
            if (position.x < 0 || position.x > screenSize.x || position.y < 0 || position.y > screenSize.y)
            {
                position = startPosition;
            }
        }

        public GDC.Dictionary _save_state()
        {
            return new GDC.Dictionary()
            {
                ["position"] = position
            };
        }
    }
}