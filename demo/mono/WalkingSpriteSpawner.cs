using Godot;

namespace Game
{
    public class WalkingSpriteSpawner : Node
    {
        [Export]
        private PackedScene _walkingSpritePrefab;
        [Export]
        private int _spawnAmount = 100;
        [Export]
        private Vector2 _speedRange = new Vector2(10, 20f);
        [Export]
        private int _seed = 1000;
        [Export]
        private int _padding = 100;

        public override void _Ready()
        {
            var rng = new RandomNumberGenerator();
            rng.Seed = (ulong)_seed;
            var screenSize = OS.GetScreenSize();
            for (int i = 0; i < _spawnAmount; i++)
            {
                var pos = new Vector2(rng.RandiRange(_padding, (int)screenSize.x - _padding), rng.RandiRange(_padding, (int)screenSize.y - _padding));
                var direction = new Vector2(rng.RandfRange(-1, 1), rng.RandfRange(-1, 1)).Normalized();
                var speed = rng.RandfRange(_speedRange.x, _speedRange.y);

                var instance = _walkingSpritePrefab.Instance<WalkingSprite>();
                AddChild(instance);
                instance.Construct(pos, speed, direction);
            }
        }
    }
}