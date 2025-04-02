shader_type canvas_item;
render_mode blend_add;
//render_mode skip_vertex_transform;

uniform sampler2D tex;
uniform sampler2D current_tex;
uniform sampler2D noise;
uniform vec2 speed;
uniform float move_speed;
render_mode skip_vertex_transform;

varying vec2 screen_pos;

void vertex() {
	vec2 global_space = VERTEX;

	VERTEX = (MODEL_MATRIX * vec4(VERTEX, 0.0, 1.0)).xy;

	//VERTEX += vec2(0, sin(TIME * VERTEX.x * move_speed) * 6.0);

	screen_pos = VERTEX.xy / 1920.0;
}

void fragment() {
	vec2 noise_pos = fract(screen_pos.xy + fract(TIME * speed));
	vec4 noise_color = texture(noise, noise_pos);
	vec4 current_color = texture(tex, UV.yx * noise_color.xy);
	vec4 current_texture = texture(current_tex, UV);
	vec4 initial_color = COLOR;


	COLOR = initial_color * current_color * current_texture;
	//COLOR = noise_color;
	//COLOR = vec4(screen_pos.x, screen_pos.y, 0, 1);
}
