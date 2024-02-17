shader_type spatial;
render_mode unshaded,blend_add,depth_draw_never,depth_test_disabled;
uniform sampler2D rim : source_color;
uniform float rim_length : hint_range(0.1f, 16.0f) = 8.0f;
uniform sampler2D albedo : source_color;
uniform sampler2D albedo_texture : source_color;
uniform vec4 color : source_color=vec4(1.0f);

global uniform float time;
instance uniform float setTime=0;

void fragment() {
	float currentTime = clamp(float(time-setTime)*4.0f,0.03f,0.97f);
	vec4 baseTexture = texture(albedo,vec2(currentTime,0.0f)) * texture(albedo_texture, UV);

	float NdotV = dot(NORMAL, -normalize(CAMERA_DIRECTION_WORLD));
	float rim_light = clamp(NdotV,0.0f,1.0f);
	rim_light=pow(rim_light,rim_length);
	rim_light = clamp(rim_light,0.01f,0.99f);
	vec4 rimColor = texture(rim,vec2(rim_light,0.0f));
	
	vec4 finalColor= (1.0f-rimColor.r) * baseTexture + rimColor.r * color;
	
	ALBEDO = finalColor.rgb;
	ALPHA *= finalColor.a;
}