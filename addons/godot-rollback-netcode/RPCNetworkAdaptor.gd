extends "res://addons/godot-rollback-netcode/NetworkAdaptor.gd"

func send_ping(peer_id: int, msg: Dictionary) -> void:
	_remote_ping.rpc_id(peer_id, msg)

@rpc("any_peer", "unreliable")
func _remote_ping(msg: Dictionary) -> void:
	var peer_id = multiplayer.get_remote_sender_id()
	received_ping.emit(peer_id, msg)

func send_ping_back(peer_id: int, msg: Dictionary) -> void:
	_remote_ping_back.rpc_id(peer_id, msg)

@rpc("any_peer", "unreliable")
func _remote_ping_back(msg: Dictionary) -> void:
	var peer_id = multiplayer.get_remote_sender_id()
	received_ping_back.emit(peer_id, msg)

func send_remote_start(peer_id: int) -> void:
	_remote_start.rpc_id(peer_id)

@rpc("any_peer")
func _remote_start() -> void:
	received_remote_start.emit()

func send_remote_stop(peer_id: int) -> void:
	_remote_stop.rpc_id(peer_id)

@rpc("any_peer")
func _remote_stop() -> void:
	received_remote_stop.emit()

func send_input_tick(peer_id: int, msg: PackedByteArray) -> void:
	_rit.rpc_id(peer_id, msg)

func is_network_host() -> bool:
	return multiplayer.is_server()

func is_network_master_for_node(node: Node) -> bool:
	return node.is_multiplayer_authority()

func get_unique_id() -> int:
	return multiplayer.get_unique_id()

# _rit is short for _receive_input_tick. The method name ends up in each message
# so, we're trying to keep it short.
@rpc("any_peer", "unreliable")
func _rit(msg: PackedByteArray) -> void:
	received_input_tick.emit(multiplayer.get_remote_sender_id(), msg)
