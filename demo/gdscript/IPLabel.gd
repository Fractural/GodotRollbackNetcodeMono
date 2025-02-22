extends Label


func _ready() -> void:
	var addresses = []
	for addr in IP.get_local_addresses():
		if addr.contains("."):
			addresses.append(addr)
	text = ",\n".join(addresses)
