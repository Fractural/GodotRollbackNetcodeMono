class_name WalkingSpriteSpawner
extends Node2D

@export var _walking_sprite_prefab: PackedScene
@export var spawn_amount: int = 100
@export var speed_range: Vector2 = Vector2(10, 20)
@export var seed: int = 1000
@export var padding: int = 100

func _ready() -> void:
	var rng = RandomNumberGenerator.new()
	rng.seed = seed
	var screen_size = DisplayServer.window_get_size()
	for i in range(spawn_amount):
		var pos = Vector2(rng.randi_range(padding, screen_size.x - padding), rng.randi_range(padding, screen_size.y - padding))
		var direction = Vector2(rng.randf_range(-1, 1), rng.randf_range(-1, 1)).normalized()
		var speed = rng.randf_range(speed_range.x, speed_range.y)
		
		var instance = _walking_sprite_prefab.instantiate() as WalkingSprite
		add_child(instance)
		instance.construct(pos, speed, direction)
