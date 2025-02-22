shader_type canvas_item;

uniform float inner_radius = 0.1;
uniform float outer_radius = 1;
uniform float vignette_strength = 1.0;
uniform float dither_strength = 0.03;
uniform vec4 vignette_color: source_color;
uniform vec2 scale = vec2(1.0);


void fragment() {
	float dist = distance(UV * scale, vec2(0.5) * scale);
	
	float vignette = smoothstep(inner_radius, outer_radius, dist) * vignette_strength;
	float dither = fract(sin(dot(UV, vec2(12.9898, 78.233))) * 43758.5453123) * dither_strength;
	
	COLOR = vec4(vignette_color.rgb, (vignette + dither) * vignette_color.a);
	
}