extends "res://addons/godot-rollback-netcode/NetworkAdaptor.gd"

func send_ping(peer_id: int, msg: Dictionary) -> void:
	rpc_unreliable_id(peer_id, "_remote_ping", msg)

remote func _remote_ping(msg: Dictionary) -> void:
	var peer_id = get_tree().get_rpc_sender_id()
	emit_signal("received_ping", peer_id, msg)

func send_ping_back(peer_id: int, msg: Dictionary) -> void:
	rpc_unreliable_id(peer_id, "_remote_ping_back", msg)

remote func _remote_ping_back(msg: Dictionary) -> void:
	var peer_id = get_tree().get_rpc_sender_id()
	emit_signal("received_ping_back", peer_id, msg)

func send_remote_start(peer_id: int) -> void:
	rpc_id(peer_id, "_remote_start")

remote func _remote_start() -> void:
	emit_signal("received_remote_start")

func send_remote_stop(peer_id: int) -> void:
	rpc_id(peer_id, "_remote_stop")

remote func _remote_stop() -> void:
	emit_signal("received_remote_stop")

func send_input_tick(peer_id: int, msg: PoolByteArray) -> void:
	rpc_unreliable_id(peer_id, '_rit', msg)

func is_network_host() -> bool:
	return get_tree().is_network_server()

func is_network_master_for_node(node: Node) -> bool:
	return node.is_network_master()

func get_network_unique_id() -> int:
	return get_tree().get_network_unique_id()

# _rit is short for _receive_input_tick. The method name ends up in each message
# so, we're trying to keep it short.
remote func _rit(msg: PoolByteArray) -> void:
	emit_signal("received_input_tick", get_tree().get_rpc_sender_id(), msg)

