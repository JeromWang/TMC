using UnityEngine;
using System.Collections;

public class TextChange : MonoBehaviour {
    public static TextChange Instance;
	// Use this for initialization
	void Start () {
        TextChange.Instance = this;
	}
	public string Chinese(string s,int v)
    {
        switch (s)
        {
            case "Cohension": return "聚能:"+v.ToString();
            case "Entrench": return "强化:"+v.ToString();
            case "Preparation": return "准备:"+v.ToString();
            case "Quantity": return "数量";
            case "Refresh": return "刷新";
            case "Return": return "回手";
            case "Freedom": return "弹道自由";
            case "Weaken": return "削弱:"+v.ToString();
            case "LeftRight": return "左右对称";
            case "TopDown": return "上下对称";
            case "E261": return "左侧自由";
            case "E262": return "右侧自由";
            case "E101":case "E102":return "";
            default: return s;//return s;
        }
    }
	// Update is called once per frame
	void Update () {
	
	}
}
