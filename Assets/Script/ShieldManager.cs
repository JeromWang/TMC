using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShieldManager : MonoBehaviour {
    public static ShieldManager Instance;
    public int EnemyDefenseID;
    public int DefenseID;
    public GameObject L_Shield_pos;
    public GameObject M_Shield_pos;
    public GameObject R_Shield_pos;
    public GameObject L_Shield;
    public GameObject M_Shield;
    public GameObject R_Shield;
    public GameObject EL_Shield;
    public GameObject EM_Shield;
    public GameObject ER_Shield;
    public DefenseMagic M_DefenseMagic;
    public DefenseMagic L_DefenseMagic;
    public DefenseMagic R_DefenseMagic;
    public DefenseMagic EM_DefenseMagic;
    public DefenseMagic EL_DefenseMagic;
    public DefenseMagic ER_DefenseMagic;
    public Vector3 pos=new Vector3(0,0,0);
    public Quaternion rot;
    public List<DefenseMagic> defenseMagicList=new List<DefenseMagic>();
    public delegate GameObject SetPostionEvent( );
    public event SetPostionEvent SetPostion;
    DefenseMagic defenseMagic;
	void Start () {
        EnemyDefenseID = 0;
        DefenseID = 0;
        L_Shield = null;
        M_Shield = null;
        R_Shield = null;
        EL_Shield = null;
        EM_Shield = null;
        ER_Shield = null;
        ShieldManager.Instance = this;
        HidePosition();
	}
    public void EnermyShield(GameObject Shield ,int ID)
    {
        switch (ID)
        {
            case -1: 
                 NewShield(Shield, ref EL_Shield);
                 EL_DefenseMagic = Shield.GetComponent<DefenseMagic>();
                 break;
            case 0: 
                 NewShield(Shield, ref EM_Shield);
                 EM_DefenseMagic = Shield.GetComponent<DefenseMagic>();
                 break;
            case 1: 
                 NewShield(Shield, ref ER_Shield);
                 ER_DefenseMagic = Shield.GetComponent<DefenseMagic>();
                 break;
        }
    }
    void NewShield(GameObject Shield,ref GameObject OldShield )
    {
        if (OldShield != null)
        {
            defenseMagic = OldShield.GetComponent<DefenseMagic>();
            defenseMagic.Destroy();
        }
        OldShield = Shield;
    }
    public void ShowPosition()
    {
        //Debug.Log("show position");
        L_Shield_pos.SetActive(true);
        M_Shield_pos.SetActive(true);
        R_Shield_pos.SetActive(true);
    }
    public void HidePosition()
    {
        L_Shield_pos.SetActive(false);
        M_Shield_pos.SetActive(false);
        R_Shield_pos.SetActive(false);
    }
    public void EndGame()
    {
        HidePosition();
        EnemyDefenseID = 0;
        DefenseID = 0;
        defenseMagicList.Clear();
        NewShield(null, ref ER_Shield);
        NewShield(null, ref EM_Shield);
        NewShield(null, ref EL_Shield);
        NewShield(null, ref R_Shield);
        NewShield(null, ref M_Shield);
        NewShield(null, ref L_Shield);
    }
    public void NewShield(Vector3 position,Quaternion rotation,int ID)
    {
        pos = position;
        rot = rotation;
        GameObject Shield=SetPostion();
        
        switch (ID)
        {
            case -1:
                Guide_ReplaceExplain(ref L_Shield);
                NewShield(Shield, ref L_Shield);
                L_DefenseMagic = Shield.GetComponent<DefenseMagic>();
                break;
            case 0:
                Guide_ReplaceExplain(ref M_Shield);
                NewShield(Shield, ref M_Shield);
                M_DefenseMagic = Shield.GetComponent<DefenseMagic>();
                break;
            case 1:
                Guide_ReplaceExplain(ref R_Shield);
                NewShield(Shield, ref R_Shield); 
                R_DefenseMagic = Shield.GetComponent<DefenseMagic>();
                break;
        }
    }
    void Guide_ReplaceExplain(ref GameObject OldShield)
    {
        if ((LevelManager.Instance.level == 2 || LevelManager.Instance.level == 3 || LevelManager.Instance.level == 4)
            && OldShield!=null )
        {
            GuideText.Instance.ReturnText(34);
        }
    }
	// Update is called once per frame
	void Update () {
	
	}
}
