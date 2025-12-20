shader_type spatial;
render_mode unshaded, cull_disabled; // Use unshaded for simple sprite look, adjust cull_disabled as needed

// Uniform to control the pixelation "size" on the texture
// A lower value (e.g., 64.0) means fewer, larger "pixels"
// A higher value (e.g., 256.0) means more, smaller "pixels"
uniform float pixel_divisions = 128.0;

// You need a uniform for your SpoutTexture
// The name might vary based on your SpoutTexture setup,
// but typically for a spatial shader texture it might be called 'albedo_texture'
uniform sampler2D albedo_texture : source_color;

void fragment() {
    // 1. Quantize the UV coordinates
    // We multiply the UV by the division count, floor it to get an integer step,
    // and then divide back to get the quantized UV.
    // The +0.5 ensures we sample from the center of the "pixel" step.
    vec2 quantized_uv = (floor(UV * pixel_divisions) + 0.5) / pixel_divisions;

    // 2. Sample the texture using the quantized UV
    // textureLod is often recommended over texture() to help prevent unwanted filtering
    // at the edges by using the highest LOD (0.0).
    vec4 tex_color = textureLod(albedo_texture, quantized_uv, 0.0);

    // 3. Set the ALBEDO color
    ALBEDO = tex_color.rgb;

    // 4. Handle transparency (important for sprites)
    ALPHA = tex_color.a;
}