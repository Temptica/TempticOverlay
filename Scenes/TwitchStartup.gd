extends Node

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	TwitchService.instance.setup()
	pass # Replace with function body.
