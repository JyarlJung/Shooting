shader_type spatial;
render_mode depth_draw_never,depth_test_disabled,blend_add;

uniform sampler2D color:source_color;
uniform sampler2D tex:source_color;

void fragment() {
	vec2 uv = UV*2.0f+vec2(-TIME*0.25f,0.0f);
	ALBEDO = texture(tex, uv).rgb * texture(color, vec2(TIME*0.2f,0.0f)).rgb;
	ALPHA*=texture(tex, uv).a;
}
