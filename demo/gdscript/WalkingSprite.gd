class_name WalkingSprite
extends Sprite2D

@export var start_position: Vector2
@export var speed: float
@export var direction: Vector2
@export var teleporting: bool


func construct(_start_position: Vector2, _speed: float, _direction: Vector2):
	speed = _speed
	direction = _direction
	start_position = _start_position
	position = start_position


func _interpolate_state(old_state: Dictionary, new_state: Dictionary, weight: float):
	if old_state["teleporting"] or new_state["teleporting"]:
		return
	position = old_state["position"].lerp(new_state["position"], weight)


func _load_state(state: Dictionary):
	position = state["position"]


func _network_process(input: Dictionary):
	position += speed * direction
	var screen_size = DisplayServer.window_get_size()
	teleporting = false
	if (position.x < 0 or position.x > screen_size.x or position.y < 0 or position.y > screen_size.y):
		position = start_position
		teleporting = true


func _save_state() -> Dictionary:
	return {
		"position": position,
		"teleporting": teleporting
	}
