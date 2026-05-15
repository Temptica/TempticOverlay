extends RigidBody3D

class_name Egg

@export var image: Texture2D:
	set = _update_image,
	get = _get_albedo

@onready var egg: MeshInstance3D = %Model

var title: String
var material: StandardMaterial3D


func _ready() -> void:
	material = egg.mesh.surface_get_material(0).duplicate()
	egg.set_surface_override_material(0, material)
	
	
func _update_image(val: Texture2D) -> void:
	image = val
	if not is_node_ready(): await ready
	material.albedo_texture = val


func _get_albedo() -> Texture2D:
	if not material: return null
	return material.albedo_texture
	
	
func _to_collectable() -> Dictionary:
	return {
		"title": title
	}
	
	
