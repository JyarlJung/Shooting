shader_type spatial;
render_mode unshaded,blend_add,depth_draw_never,depth_test_disabled;

uniform sampler2D view:source_color;
uniform sampler2D noise:source_color;

void fragment() {
	vec2 noiseuv=UV;
	noiseuv.y+=TIME*0.2f;
	vec2 uv = UV + (texture(noise, noiseuv).rg-vec2(0.5f))*0.002f;
	vec3 tex =texture(view, uv).rgb;
	tex*=0.95f;
	tex=max(tex-vec3(0.01f,0.01f,0.01f),vec3(0.0f));
	ALBEDO = tex;
}
