tool
extends Node

const LogData = preload("res://addons/godot-rollback-netcode/log_inspector/LogData.gd")

const GAME_ARGUMENTS_SETTING = 'network/rollback/log_inspector/replay_arguments'
const GAME_PORT_SETTING = 'network/rollback/log_inspector/replay_port'
const MAIN_RUN_ARGS_SETTING = 'editor/main_run_args'

var server: TCP_Server
var connection: StreamPeerTCP
var editor_interface = null
var game_pid: int = 0

enum Status {
	NONE,
	LISTENING,
	CONNECTED,
}

signal started_listening ()
signal stopped_listening ()
signal game_connected ()
signal game_disconnected ()

func set_editor_interface(_editor_interface) -> void:
	editor_interface = _editor_interface

func start_listening() -> void:
	if not server:
		var port = 49111
		if ProjectSettings.has_setting(GAME_PORT_SETTING):
			port = ProjectSettings.get_setting(GAME_PORT_SETTING)
		
		server = TCP_Server.new()
		server.listen(port, "127.0.0.1")
		emit_signal("started_listening")

func stop_listening() -> void:
	if server:
		server.stop()
		server = null
		emit_signal("stopped_listening")

func disconnect_from_game(restart_listening: bool = true) -> void:
	if connection:
		connection.disconnect_from_host()
		emit_signal("game_disconnected")
		connection = null
	stop_game()
	if restart_listening:
		start_listening()

func _notification(what: int) -> void:
	if what == NOTIFICATION_PREDELETE:
		disconnect_from_game(false)
		stop_listening()
		stop_game()

func launch_game() -> void:
	stop_game()
	
	var args_string = "replay"
	if ProjectSettings.has_setting(GAME_ARGUMENTS_SETTING):
		args_string = ProjectSettings.get_setting(GAME_ARGUMENTS_SETTING)
	
	if editor_interface:
		var old_main_run_args = ProjectSettings.get_setting(MAIN_RUN_ARGS_SETTING)
		ProjectSettings.set_setting(MAIN_RUN_ARGS_SETTING, old_main_run_args + ' ' + args_string)
		editor_interface.play_main_scene()
		ProjectSettings.set_setting(MAIN_RUN_ARGS_SETTING, old_main_run_args)
	else:
		var args := []
		for arg in args_string.split(" "):
			args.push_back(arg)
		game_pid = OS.execute(OS.get_executable_path(), args, false)

func stop_game() -> void:
	if editor_interface and editor_interface.is_playing_scene():
		editor_interface.stop_playing_scene()
	elif game_pid != 0:
		OS.kill(game_pid)
		game_pid = 0

func is_game_started() -> bool:
	if editor_interface:
		return editor_interface.is_playing_scene()
	return game_pid > 0

func is_connected_to_game() -> bool:
	return connection and connection.is_connected_to_host()

func get_status() -> int:
	if is_connected_to_game():
		return Status.CONNECTED
	elif server and server.is_listening():
		return Status.LISTENING
	return Status.NONE

func send_message(msg: Dictionary) -> void:
	if not is_connected_to_game():
		push_error("Replay server: attempting to send message when not connected to game")
		return
	
	connection.put_var(msg)

func send_match_info(log_data: LogData, my_peer_id: int) -> void:
	if not is_connected_to_game():
		return
	if not log_data or log_data.peer_ids.size() == 0:
		return
	
	var peer_ids := []
	for peer_id in log_data.peer_ids:
		if peer_id != my_peer_id:
			peer_ids.append(peer_id)

	var msg := {
		type = "setup_match",
		my_peer_id = my_peer_id,
		peer_ids = peer_ids,
		match_info = log_data.match_info,
	}
	send_message(msg)

func poll() -> void:
	if connection:
		if connection.get_status() == StreamPeerTCP.STATUS_NONE or connection.get_status() == StreamPeerTCP.STATUS_ERROR:
			disconnect_from_game()
	if server and not connection and server.is_connection_available():
		connection = server.take_connection()
		stop_listening()
		emit_signal("game_connected")

func _process(delta: float) -> void:
	poll()
