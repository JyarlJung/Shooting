using Godot;
using System;
[Tool]
public partial class BlurTex : MeshInstance3D
{
	private ShaderMaterial mat;
	[Export]
	public SubViewport viewport;
	private ViewportTexture tex;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Visible=true;
		mat=(ShaderMaterial)MaterialOverride;
		tex=viewport.GetTexture();
		mat.SetShaderParameter("view",tex);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
