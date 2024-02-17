using Godot;
using System;
using System.Collections.Generic;

public partial class Global:Node
{
    //Property
    public static Camera3D Camera;
    public static NodeContainer Container;
    public static SoundManager Sound;
    public static Ui UiNode;

    //Member
	private static float _frameDelta;
    private static int _time=0;
    private static float _secTime=0;
    private static Vector4 _box=new Vector4(-2.1f,-1.54f,0.7f,1.54f);

    //MemberGetter
    public static float FrameDelta { get => _frameDelta;}
    public static int Time { get => _time;}
    public static float SecTime { get => _secTime;}
    public static Vector4 Box { get => _box;}

    public override void _Ready()
    {
        _frameDelta=1f/Engine.MaxFps;
		Camera = GetViewport().GetCamera3D();
                    ChangeSize(new Vector2I(960,720));
    }
    public override void _Process(double delta)
    {
        _time++;
        _secTime+=_frameDelta;
        RenderingServer.GlobalShaderParameterSet("time",_secTime);
        // foreach(ShaderMaterial shader in shaders)
        // {
        //     shader.SetShaderParameter("time",Time);
        // }
        if(Input.IsKeyPressed(Key.Key1))
        {
            ChangeSize(new Vector2I(480,360));
        }
        else if(Input.IsKeyPressed(Key.Key2))
        {
            ChangeSize(new Vector2I(960,720));
        }
        else if(Input.IsKeyPressed(Key.Key3))
        {
            ChangeSize(new Vector2I(1440,1080));
        }
        else if(Input.IsKeyPressed(Key.Key4))
        {
            ChangeSize(new Vector2I(1920,1440));
        }
    }
    public void ChangeSize(Vector2I size)
    {
        GetWindow().Position=GetWindow().Position+(GetWindow().Size-size)/2;
        GetWindow().Size=size;
    }
    public static float GetNegPos(float f)
    {
        return (f>=0f) ? 1f:-1f;
    }
    public static Vector3 GetArrow(Vector3 pos, Vector3 target)
    {
        return Vector3.Back * Mathf.Atan2(target.Y-pos.Y,target.X-pos.X)*(57.3f);
    }
    public static Vector3 MoveVector(float z)
    {
        
        return new Vector3(Mathf.Cos(Mathf.DegToRad(z)),Mathf.Sin(Mathf.DegToRad(z)),0f);
    }
    // public static void MaterialSyncTime(ShaderMaterial shader)
    // {
    //     foreach(ShaderMaterial mat in shaders)
    //     {
    //         if(mat.Equals(shader))return;
    //     }
    //     shaders.Add(shader);
    // }
}
