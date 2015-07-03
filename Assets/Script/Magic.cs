using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Magic : MonoBehaviour
{
    protected UILabel explainLabel;

    int ID;
    public bool refresh = false;//是否刷新
    public int magicValue;//{get;set;}
    public string name;
    protected Dictionary<string, int> Effect = new Dictionary<string, int>();//效果字典 string：名字，int：数值
    protected bool choosed = false;
    public bool isHeros;

    void Awake()
    {
        explainLabel = GameObject.Find("ExplainLabel").GetComponent<UILabel>();
       // Debug.Log(explainLabel.text);
    }
    void Update()
    {
        
    }
    protected void OnMouseEnter()
    {
        Explain();
        choosed = true;
    }
    protected void OnMouseExit()
    {
        GuideText.Instance.ReturnText();
        choosed = false;
    }
    protected void Explain()
    {
        foreach (KeyValuePair<string, int> item in Effect)
        {
            explainLabel.text += TextChange.Instance.Chinese(item.Key,item.Value) + "\n";
        }
    }
    virtual public void Destroy()
    {
        Destroy(gameObject);
       // Destroy(this);
    }
    public Magic()
    {

    }
    virtual public void Refresh() { }
    public void PlusEffectValue(string EffectName, int Value)//对效果的数值进行+操作，不存在这个效果则添加
    {
        if (Effect.ContainsKey(EffectName))
        {
            AddEffectValue(EffectName, Value);
        }
        else
        {
            AddEffect(EffectName, Value);
        }
    }
    public bool HasEffect(string EffectName)
    {
        if (Effect.ContainsKey(EffectName))
        {
            return true;
        }
        return false;
    }
    public bool AddEffect(string EffectName, int Value)//添加效果，若已有返回false
    {
        if (Effect.ContainsKey(EffectName))
        {
            return false;
        }
        Effect.Add(EffectName, Value);
        return true;
    }
    public int GetEffectValue(string EffectName)//得到该效果的数值,没有返回0
    {
        if (Effect.ContainsKey(EffectName))
        {
            return Effect[EffectName];
        }
        return 0;
    }
    public bool AddEffectValue(string EffectName, int number)//对效果的数值进行+操作，不存在这个效果返回false
    {
        if (Effect.ContainsKey(EffectName))
        {
            Effect[EffectName] += number;//是否要判定效果值<0的情况？
            return true;
        }
        return false;
    }
    public bool RemoveEffect(string EffectName)//移除效果，不存在该效果返回False
    {
        if (Effect.ContainsKey(EffectName))
        {
            Effect.Remove(EffectName);
            return true;
        }
        return false;
    }

}
