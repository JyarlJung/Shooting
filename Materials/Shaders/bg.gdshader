shader_type spatial;
render_mode unshaded,depth_draw_never;
uniform sampler2D view:source_color;
void fragment() {
	ALBEDO = texture(view, UV).rgb;
}
