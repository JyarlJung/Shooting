shader_type spatial;
render_mode unshaded,depth_draw_never,depth_test_disabled,blend_sub;

uniform vec4 albedo : source_color = vec4(1.0f);
uniform sampler2D rim : source_color;
uniform float rim_length : hint_range(0.1f, 16.0f) = 8.0f;
uniform sampler2D albedo_texture : source_color;
uniform vec4 unicolor : source_color=vec4(1.0f);

instance uniform vec4 color : source_color=vec4(1.0f);

void fragment() {
	vec4 mixcolor=vec4(1.0f)-unicolor * color;
	mixcolor.a=(unicolor * color).a;
	float NdotV = dot(NORMAL, -normalize(CAMERA_DIRECTION_WORLD));
	float rim_light = clamp(NdotV,0.0f,1.0f);
	rim_light=pow(rim_light,rim_length);
	rim_light = clamp(rim_light,0.01f,0.99f);
	vec4 rimColor = texture(rim,vec2(rim_light,0.0f));
	
	vec4 baseTexture = vec4(1.0f)-(albedo * texture(albedo_texture,UV));
	baseTexture.a=(albedo * texture(albedo_texture,UV)).a;
	vec4 finalColor= (1.0f-rimColor.r) * baseTexture + rimColor.r * mixcolor;
	
	ALBEDO = finalColor.rgb;
	ALPHA *= finalColor.a;
}