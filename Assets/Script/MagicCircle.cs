using UnityEngine;
using System.Collections;

public enum StartType
{
    No,Shrink, Enlarge,Rotate
}
public enum CircleType
{
    Normal,Freedom
}
public class MagicCircle : MonoBehaviour {
    
    public StartType Type ;
    public bool isHeros;
    GameObject core;
    GameObject circle;
    GameObject edge;
    GameObject Value;
    bool changeCircle = true;
    bool changeCore = true;
    bool move = true;
    float time=0;
    iTween itweenCore, itweenCircle, itweenEdge, itweenValue;
    Vector3 normalSize = new Vector3(1f, 1f, 1);
    Vector3 normalValueSize = new Vector3(0.01f, 0.01f, 1);
    Vector3 zeroSize = new Vector3(0, 0, 0);
	// Use this for initialization
    void Awake()
    {
        core = transform.FindChild("core").gameObject;
        edge = transform.FindChild("edge").gameObject;
        circle = transform.FindChild("circle").gameObject;
        Value = transform.FindChild("Value").gameObject;
        //if (Type == 0)
        //{

        //}
        //else if (Type == 1)
        //{
        //    core = transform.FindChild("core").gameObject;
        //    edge = transform.FindChild("edge").gameObject;
        //}
    }
    public void RotateStart()
    {
        edge.transform.localScale = zeroSize;
        iTween.ScaleTo(edge, normalSize, 3);
        Value.transform.localScale = zeroSize;
        RotateStop(core);
        RotateStop(edge);
        RotateStop(circle);
        iTween.RotateBy(circle, new Vector3(0, 1, 0), 3);
        iTween.RotateBy(core, new Vector3(1, 0, 0), 3);
        Invoke("RotateZero", 3);
    }
    public void  ShrinkStart()
    {
        core.transform.localScale = new Vector3(20 , 20, 20);
        circle.transform.localScale = new Vector3(20, 20, 20);
        edge.transform.localScale = new Vector3(20, 20, 20);
        Value.transform.localScale = zeroSize;
    }
    public void  EnlargeStart()
    {
        core.transform.localScale = zeroSize;
        circle.transform.localScale = zeroSize;
        edge.transform.localScale = zeroSize;
        Value.transform.localScale = zeroSize;
        iTween.ScaleTo(edge, normalSize, 2);
    }
    public void Ripe(CircleType type)
    {
        if(type==CircleType.Normal)
        {
            core.transform.renderer.material = (Material)Resources.Load("Material/Purple Core");
            edge.transform.renderer.material = (Material)Resources.Load("Material/Purple Edge");
            circle.transform.renderer.material = (Material)Resources.Load("Material/Purple Edge");
            return;
        }
        if(type==CircleType.Freedom)
        {
            core.transform.renderer.material = (Material)Resources.Load("Material/Purple Freedom Core");
            edge.transform.renderer.material = (Material)Resources.Load("Material/Purple Edge");
            circle.transform.renderer.material = (Material)Resources.Load("Material/Purple Edge");
            return;
        }
    }
    public void TextureChange(CircleType type, bool ripe)
    {
        if(ripe)
        {
            Ripe(type);
        }
        else
        {
            if(type==CircleType.Freedom)
            {
                core.transform.renderer.material = (Material)Resources.Load("Material/Yellow Freedom Core");
                edge.transform.renderer.material = (Material)Resources.Load("Material/Yellow Edge");
                circle.transform.renderer.material = (Material)Resources.Load("Material/Yellow Edge");
                return;
            }
            if (type == CircleType.Normal)
            {
                core.transform.renderer.material = (Material)Resources.Load("Material/Yellow Core");
                edge.transform.renderer.material = (Material)Resources.Load("Material/Yellow Edge");
                circle.transform.renderer.material = (Material)Resources.Load("Material/Yellow Edge");
                return;
            }
        }
    }
    public void Destroy()
    {
        core.SetActive(false);
        edge.SetActive(false);
        circle.SetActive(false);
        Value.SetActive(false);
    }
    public void Shake()
    {
        if(itweenCircle!=null)
        {
            Destroy(itweenCircle);
            //iTween.Stop(circle);
        }
        if (itweenCore != null)
        {
            Destroy(itweenCore);
            //iTween.Stop(core);
        }
        if (itweenEdge != null)
        {
            //iTween.Stop(edge);
            Destroy(itweenEdge);
        }
        if (itweenValue != null)
        {
           // iTween.Stop(Value);
            Destroy(itweenValue);
        }
        iTween.MoveAdd(core, iTween.Hash("z", 0.5f, "space", Space.Self, "time", 0.7f));
        iTween.MoveAdd(circle, iTween.Hash("z", 0.7f, "space", Space.Self, "time", 0.7f));
        iTween.MoveAdd(edge, iTween.Hash("z", 1f, "space", Space.Self, "time", 0.7f));
        if(isHeros)
        {
            iTween.MoveAdd(Value, iTween.Hash("z", -1f, "space", Space.Self, "time", 0.7f));
        }
        else
            iTween.MoveAdd(Value, iTween.Hash("z", 1f, "space", Space.Self, "time", 0.7f));
        Invoke("MoveBack", 0.8f);
        //
       // iTween.MoveFrom(gameObject, transform.forward * -1f, 1);
    }
    public void RotateFast(int num)
    {
        RotateObj(core, num);
        RotateObj(circle, num);
        RotateObj(edge, num);
    }
    void MoveBack()
    {
        iTween.MoveTo(core, transform.position, 2f);
        iTween.MoveTo(circle, transform.position, 2f);
        iTween.MoveTo(edge, transform.position, 2f);
        iTween.MoveTo(Value, transform.position, 2f);
        itweenCircle = circle.GetComponent<iTween>();
        itweenCore = core.GetComponent<iTween>();
        itweenEdge = edge.GetComponent<iTween>();
        itweenValue = Value.GetComponent<iTween>();
        //Invoke("PositionZero", 2f);
    }
    public void PositionZero()
    {
        core.transform.position = transform.position;
        circle.transform.position = transform.position;
        edge.transform.position = transform.position;
        Value.transform.position = transform.position;
    }
    public void ExpendShrink()
    {

        iTween.ScaleTo(edge, new Vector3(1.5f, 1.5f, 1.5f), 1f);
        iTween.ScaleTo(core, new Vector3(1.5f, 1.5f, 1.5f), 1f);
        iTween.ScaleTo(circle, new Vector3(1.5f, 1.5f, 1.5f), 1f);
        Invoke("ReSize", 1f);
    }
    public void ReSize()
    {
        iTween.ScaleTo(edge, normalSize, 1f);
        iTween.ScaleTo(core, normalSize, 1f);
        iTween.ScaleTo(circle, normalSize, 1f);
    }
    public void Expend2()
    {
        iTween.MoveTo(circle, transform.position + new Vector3(0, 2f, 0), 1f);
        iTween.MoveTo(core, transform.position + new Vector3(0, 1f, 0), 1f);
    }
    public void Shrink()
    {
        iTween.MoveTo(circle, transform.position , 1f);
        iTween.MoveTo(core, transform.position , 1f);
    }
	public void Expend()
    {
        Value.SetActive(false);
        iTween.MoveTo(gameObject,transform.position + new Vector3(0, 1.9f, 0), 1.5f);
        iTween.MoveTo(circle, transform.position + new Vector3(0, 2f, 0), 1.5f);
        iTween.MoveTo(core, transform.position + new Vector3(0, 1f, 0), 1.5f);
    }
    public void Lager()
    {
        iTween.ScaleTo(circle,new Vector3(4,4,4),1.5f);
        iTween.ScaleTo(core, new Vector3(4, 4, 4), 1.5f);
        iTween.ScaleTo(edge, new Vector3(4, 4, 4), 1.5f);
    }
    public void Refresh()
    {
        RotateStop(core);
        RotateStop(edge);
        RotateStop(circle);
        iTween.RotateBy(circle, new Vector3(0, 0, 1), 3);
        iTween.RotateBy(edge, new Vector3(0, 1, 0), 3);
        iTween.RotateBy(core, new Vector3(1, 0, 0), 3);
        Invoke("RotateZero", 3);
    }
    void RotateStop(GameObject obj)
    {
        Rotation rotation = obj.GetComponent<Rotation>();
        rotation.isActive=false;
    }
    void RotateStart(GameObject obj)
    {
        Rotation rotation = obj.GetComponent<Rotation>();
        rotation.isActive = true;
    }
    void RotateObj(GameObject obj,int num)
    {
        Rotation rotation = obj.GetComponent<Rotation>();
        rotation.RotationAxis = rotation.OriginRotationAxis * num;
        rotation.isOrigin = false;
    }
    void HalfRotate(GameObject obj)
    {
        Rotation rotation = obj.GetComponent<Rotation>();
        rotation.RotationAxis = rotation.RotationAxis*0.5f;
    }
    void RotateZero()
    {
        circle.transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, circle.transform.eulerAngles.z);
        core.transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, core.transform.eulerAngles.z);
        edge.transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, edge.transform.eulerAngles.z);
        RotateStart(core);
        RotateStart(circle);
        RotateStart(edge);
    }
	// Update is called once per frame
	void Update () {
        time += Time.deltaTime;
        if (Type == StartType.Shrink)
        {
            if (changeCircle & time > 0.5) { iTween.ScaleTo(circle, normalSize, 2); changeCircle = false; }
            if (changeCore & time > 0.65) 
            {
                iTween.ScaleTo(core, normalSize, 4);
                iTween.ScaleTo(Value, normalValueSize, 4);
                changeCore = false;
                Type = StartType.No;
            }
            
            
            //if(time>12)
            //{
            //    //iTween.(edge, 0, 2);
            //    //iTween.FadeTo(core, 0, 2);
            //    //iTween.FadeTo(circle, 0, 2);
            //    //iTween.FadeTo(Value, 0, 2);
            //    //core.renderer.material.color = new Color(0, 0, 0, 0);
            //    Destroy(gameObject);
            //}
        }
        if (Type == StartType.Rotate)
        {
            if (time > 3)
            {
                Value.transform.localScale = normalValueSize;
                Type = StartType.No;
            }
        }
        if (Type == StartType.Enlarge)
        {
            bool changeEdge = true;
            if(changeEdge & time>0 )
            {
                changeEdge = false;
                iTween.ScaleTo(edge, normalSize, 1f);
            }
            if (changeCircle & time > 0.25)
            {
                iTween.ScaleTo(core, normalSize, 1); changeCircle = false; 
            }
            if (changeCore & time > 0.5)
            {
                iTween.ScaleTo(circle, normalSize, 1);
                changeCore = false;
                
            }
            if(time>1.5f)
            {
                iTween.ScaleTo(Value, normalValueSize, 0.2f);
                Type =StartType.No;
            }
        }
        
	}
}
