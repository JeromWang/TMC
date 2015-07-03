using UnityEngine;
using System.Collections;

public enum AIType
{
    CardList,//读入整局使用的list
    RoundList//读入每回合使用的list
}
public class AI : MonoBehaviour {

    public static AI Instance;
    public AIType aiType=AIType.CardList;

	// Use this for initialization
	void Start () {
        AI.Instance = this;
        
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
