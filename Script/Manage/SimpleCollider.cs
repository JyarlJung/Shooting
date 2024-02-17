using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
[GlobalClass]
public partial class SimpleCollider : Node3D
{
    private static int MAX_LAYER=32;

    //Member
    [Export]
    private Array<Vector3> _points=new Array<Vector3>();
    [Export(PropertyHint.Layers3DPhysics)]
    private int _layer=1, _collision=1;
    [Export]
    private Node3D _rootNode;

    private Vector4 _rect=Vector4.Zero;
    private float _maxRadius=0.1f;
	private static List<List<SimpleCollider>> colliders= new List<List<SimpleCollider>>();
    private List<Segment> _segments=new List<Segment>();

    //Property
    [Export]
    public float Width=0;
    [Export]
    public bool UseRect=false;

    //MemberGetter
    public Node3D RootNode { get => _rootNode;}
    public List<Segment> Segments { get => _segments;}

    public override void _Ready()
    {
        SetCollisionLayer(this);
        foreach(Vector3 pos in _points)
        {
            AddSegment(pos,true);
        }
    }
    public override void _ExitTree()
    {
        RemoveCollisionLayer(this);
    }

    public override void _Process(double delta)
    {
        _rect=Vector4.Zero;
        _maxRadius=Width;
        foreach(Segment pos in _segments)
        {
            if(!pos.Enable)continue;
            RadiusFunc(pos.Pos);
        }
    }

	public List<Collision> HitTestAll(int mask = -1)
	{
        if(!IsVisibleInTree()) return null;
		List<Collision> res=new List<Collision>();
        if(mask==-1)
        {
            int i=0;
            mask=_collision;
            while(mask>0)
            {
                if(mask%2==1)
                {
                    foreach(SimpleCollider obj in colliders[i])
                    {
                        if(this.Equals(obj) || !obj.IsVisibleInTree())continue;
                        int ind=this.HitTest(obj);
                        if(ind!=-1)res.Add(new Collision(obj,ind));
                    }
                }
                i++;
                mask = mask >> 1;
            }
        }
        else
        {
            foreach(SimpleCollider obj in colliders[mask])
            {
                if(this.Equals(obj) || !obj.IsVisibleInTree())continue;
                int ind=this.HitTest(obj);
                if(ind!=-1)res.Add(new Collision(obj,ind));
            }
        }
		return res;
	}
    public Collision HitTestFirst(int mask = -1)
	{
        if(!IsVisibleInTree()) return null;
        if(mask==-1)
        {
            int i=0;
            mask=_collision;
            while(mask>0)
            {
                if(mask%2==1)
                {
                    foreach(SimpleCollider obj in colliders[i])
                    {
                        if(this.Equals(obj) || !obj.IsVisibleInTree())continue;
                        int ind=this.HitTest(obj);
                        if(ind!=-1)return new Collision(obj,ind);
                    }
                }
                i++;
                mask = mask >> 1;
            }
        }
        else
        {
            foreach(SimpleCollider obj in colliders[mask])
            {
                if(this.Equals(obj) || !obj.IsVisibleInTree())continue;
                int ind=this.HitTest(obj);
                if(ind!=-1)return new Collision(obj,ind);
            }
        }
		return null;
	}
	public int HitTest(SimpleCollider obj)
	{
        Vector3 objpos=obj.GlobalPosition;
        if(obj.UseRect)
        {
            if(!UseRect)
            {
                float dx=Math.Max(objpos.X+(obj._rect.X)-GlobalPosition.X,GlobalPosition.X-(objpos.X+obj._rect.Z));
                float dy=Math.Max(objpos.Y+(obj._rect.Y)-GlobalPosition.Y,GlobalPosition.Y-(objpos.Y+obj._rect.W));
                if(dx>=0 && dy>=0)
                {
                    if(Mathf.Sqrt(dx*dx + dy*dy)>_maxRadius) return -1;
                }

            }
            else 
            {
                if(objpos.X+obj._rect.X>GlobalPosition.X+_rect.Z && objpos.X>GlobalPosition.X)return -1;
                if(objpos.X+obj._rect.Z<GlobalPosition.X+_rect.X && objpos.X<=GlobalPosition.X)return -1;
                if(objpos.Y+obj._rect.Y>GlobalPosition.Y+_rect.W && objpos.Y>GlobalPosition.Y)return -1;
                if(objpos.Y+obj._rect.W<GlobalPosition.Y+_rect.Y && objpos.Y<=GlobalPosition.Y)return -1;
            }
        }
        else
        {
            if(UseRect)
            {
                float dx=Math.Max(GlobalPosition.X+(_rect.X)-objpos.X,objpos.X-(GlobalPosition.X+_rect.Z));
                float dy=Math.Max(GlobalPosition.Y+(_rect.Y)-objpos.Y,objpos.Y-(GlobalPosition.Y+_rect.W));
                //GD.Print(dx,"dy",dy);
                if(dx>=0 || dy>=0)
                {
                    if(Mathf.Sqrt(dx*dx + dy*dy)>obj._maxRadius) return -1;
                }
            }
            else 
            {
                if(GlobalPosition.DistanceTo(obj.GlobalPosition)>_maxRadius+obj._maxRadius)return -1;
            }
        }
        if(_segments.Count==1)
        {
            if(obj._segments.Count==1)
            {
                return PointToPoint(0,obj,0) ? 0 : -1;
            }
            for (int i=0; i<obj._segments.Count;i++)
            {
                if(PointToSegment(0,obj,i))return 0;
            }
            return -1;
        }
        else if(obj._segments.Count==1)
        {
            for(int i=0; i<_segments.Count;i++)
            {
                if(obj.PointToSegment(0,this,i))return i;
            }
            return -1;
        }
        for(int i=0; i<_segments.Count;i++)
        {
            if(!_segments[i].Enable)continue;
            for (int j=0; j<obj._segments.Count-1;j++)
            {
                if(SegmentToSegment(i,obj,j))return i;
            }
        }
		return -1;
	}
    private bool PointToPoint(int index, SimpleCollider obj, int objIndex)
    {
        if(!_segments[index].Enable || !obj._segments[objIndex].Enable) return false;
        Vector3 src=Trans(this,index);
        Vector3 dest=Trans(obj,objIndex);

        return src.DistanceTo(dest) <= Width+obj.Width;
    }
    private bool PointToSegment(int index, SimpleCollider obj, int objIndex)
    {
        if(!_segments[index].Enable || !obj._segments[objIndex].Enable) return false;
        if(obj._segments.Count-1==objIndex)
        {
            return PointToPoint(index, obj, objIndex);
        }
        Vector3 src=Trans(this,index);
        Vector3 dest=Trans(obj,objIndex);
        Vector3 destEnd=Trans(obj,objIndex+1);
        Vector3 ab = destEnd - dest;
        Vector3 av = src - dest;
        
        if(av.Dot(ab)<=0f)
        {
            return av.Length() <= Width+obj.Width;
        }

        Vector3 bv = src - destEnd;

        if(bv.Dot(ab)>=0f)
        {
            return bv.Length() <= Width+obj.Width;
        }
        return ab.Cross(av).Length()/ab.Length() <= Width+obj.Width;
    }
    private bool SegmentToSegment(int index, SimpleCollider obj, int objIndex)
    {
        float eta=0.0001f;

        if(!_segments[index].Enable || !obj._segments[objIndex].Enable) return false;
        if(_segments.Count-1==index)
        {
            return PointToSegment(index,obj,objIndex);
        }
        if(obj._segments.Count-1==objIndex)
        {
            return obj.PointToSegment(objIndex,this,index);
        }
        Vector2 a0= new Vector2(Trans(this,index).X,Trans(this,index).Y);
        Vector2 a1= new Vector2(Trans(this,index+1).X,Trans(this,index+1).Y);
        Vector2 b0= new Vector2(Trans(obj,objIndex).X,Trans(obj,objIndex).Y);
        Vector2 b1= new Vector2(Trans(obj,objIndex+1).X,Trans(obj,objIndex+1).Y);
        Vector2 r = b0 - a0;
        Vector2 u = a1 - a0;
        Vector2 v = b1 - b0;

        float ru = r.Dot(u);
        float rv = r.Dot(v);
        float uu = u.Dot(u);
        float uv = u.Dot(v);
        float vv = v.Dot(v);

        float det = uu*vv - uv*uv;
        float s, t;

        if (det < eta*uu*vv)
        {
            s = Mathf.Clamp(ru/uu, 0, 1);
            t = 0;
        }
        else
        {
            s = Mathf.Clamp((ru*vv - rv*uv)/det, 0, 1);
            t = Mathf.Clamp((ru*uv - rv*uu)/det, 0, 1);
        }

        float S = Mathf.Clamp((t*uv + ru)/uu, 0, 1);
        float T = Mathf.Clamp((s*uv - rv)/vv, 0, 1);

        Vector2 A = a0 + S*u;
        Vector2 B = b0 + T*v;
        return (B - A).Length() <= Width+obj.Width;
    }
    public bool InLayer(int mask)
    {
        return (_layer & (1<<mask))!=0;
    }
	private static void SetCollisionLayer(SimpleCollider node)
	{
        if(colliders.Count<32)Init();
        int ind=0;
        int layer=node._layer;
        while(layer>0)
        {
            if(layer%2==1)
            {
                colliders[ind].Add(node);
            }
            ind++;
            layer = layer >> 1;
        }
	}
	private static void RemoveCollisionLayer(SimpleCollider node)
	{
		if(colliders.Count<32)Init();
        int ind=0;
        int layer=node._layer;
        while(layer>0)
        {
            if(layer%2==1)
            {
                colliders[ind].Remove(node);
            }
            ind++;
            layer = layer >> 1;
        }
	}
    private static void Init()
    {
        for(int i=0; i<MAX_LAYER;i++)
        {
            colliders.Add(new List<SimpleCollider>());
        }
    }
    private static Vector3 Trans(SimpleCollider obj, int ind)
    {
        return obj._segments[ind].Pos.Rotated(Vector3.Back,obj.GlobalRotation.Z) + obj.GlobalPosition;
    }
    public void AddSegment(Vector3 pos, bool enable = true)
    {
        this._segments.Add(new Segment(pos,enable));
        RadiusFunc(pos);
    }
    public Vector3 GetPos(int index)
    {
        return _segments[index].Pos;
    }
    public bool GetEnable(int index)
    {
        return _segments[index].Enable;
    }
    public void SetSegment(int index,Vector3 pos)
    {
        _segments[index].Pos=pos;
    }
    public void SetSegment(int index,Vector3 pos, bool set)
    {
        _segments[index].Pos=pos;
        _segments[index].Enable=set;
    }
    public void SetSegment(int index,bool set = true)
    {
        _segments[index].Enable=set;
    }
    public void RadiusFunc(Vector3 pos)
    {
        if(UseRect)
        {
            if(pos.X+Width>_rect.Z)_rect.Z=pos.X+Width;
            else if(pos.X-Width<_rect.X)_rect.X=pos.X-Width;
            if(pos.Y+Width>_rect.W)_rect.W=pos.Y+Width;
            else if(pos.Y-Width<_rect.Y)_rect.Y=pos.Y-Width;
        }
        else if(pos.Length()+Width>_maxRadius)
        {
            _maxRadius=pos.Length()+Width;
        }
    }
}
