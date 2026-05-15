extends Sprite3D
#
#var receiver: PipeWireStream
#
#func _ready():
#    receiver = PipeWireStream.new()
#    receiver.create_receiver("some-sender-name")
#
#func _process(_delta):
#    var img = receiver.receive_image()
#    if img:
#        texture = ImageTexture.create_from_image(img)