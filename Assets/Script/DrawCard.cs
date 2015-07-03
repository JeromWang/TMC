using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//卡牌的移动 和 卡组的初值
public class DrawCard : MonoBehaviour
{
    public static DrawCard Instance;
    // public Transform enemyHand;
    public GameObject cardObject;
    public Transform mycarddeap;
    public Transform HandZone;

    // public Transform mHnadTarget;
    public Transform EnemyHand;
    public Transform ECardShow;
    public Transform EnemyList;
    public Transform EnemyAuraList;
    //public Transform NewCardShow;
    public Transform NewCardList;
    static int DeckMax = 20;//卡组上限
    int childcount;
    int EnemyListNum = 0;
    List<string> CardList = new List<string>();
    List<string> EnemyCardList = new List<string>();
    public List<string> EnemyRoundList=new List<string>();

    List<string> newCardList = new List<string>();//本关奖励的新卡
    int residue=0;
    int enemyResidue=0;
    Hero hero,enemyHero;
    void Awake()
    {

       // Debug.Log("drawCard Awake");
        DrawCard.Instance = this;
        EnergyManager.Instance.StartTurn += this.StartTurn;
        EnergyManager.Instance.StartGame += this.StartGame;
        HandZone = GameObject.Find("HandZone").transform;
        mycarddeap = GameObject.Find("Deck_Our").transform;
        EnemyHand = transform.FindChild("EnemyHand");
        ECardShow = transform.FindChild("EnemyCardShow");
        EnemyList = transform.FindChild("EnemyList");
        EnemyAuraList = transform.FindChild("EnemyAuraList");
        NewCardList = transform.FindChild("NewCardList");
        hero = GameObject.Find("Hero").GetComponent<Hero>();
        enemyHero = GameObject.Find("EnemyHero").GetComponent<Hero>();
        cardObject =(GameObject)Resources.Load("card new 1");
    }
    public void GetCardList(string deckName)
    {
        CardList.Clear();
        string s = PlayerPrefs.GetString(deckName);
        for (int i = 0; i < 20; i++)
        {
            CardList.Add(s.Substring(i * 3, 3));
        }
        residue = CardList.Count;
    }
    public void GetEnemyRoundList(int round)
    {
        //Debug.Log("GetEnemyRoundList");
        string s="";
        if (EnemyRoundList.Count < round)
        {
            s = EnemyRoundList[EnemyRoundList.Count - 1];
        }
        else
        {
            s = EnemyRoundList[round - 1];
        }
        //Debug.Log(s);
        for(int i=0;i<s.Length;i=i+3)
        {
            EnergyManager.Instance.enemyOperationID.Add(s.Substring(i , 3));
            EnergyManager.Instance.operation.Add(Operation.Cast);

            EnergyManager.Instance.enemyTrajectoryID.Add(0);
        }
    }
    public void GetEnemyCardList(string deckName)
    {
        EnemyCardList.Clear();
        string s = PlayerPrefs.GetString(deckName);
        for (int i = 0; i < 20; i++)
        {
            EnemyCardList.Add(s.Substring(i * 3, 3));
           // Debug.Log(s.Substring(i * 3, 3));
        }
        enemyResidue = EnemyCardList.Count;
      //  Debug.Log(enemyResidue.ToString());
    }
    public void GetNewCardList(string s)
    {
        //Debug.Log(s);
        newCardList.Clear();
        for (int i = 0; i < s.Length/3; i++)
        {
            newCardList.Add(s.Substring(i * 3, 3));
        }
    }
    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void clearUpHand(int Type=0)
    {
        CameraMoving.Instance.canMove = false;
        int x = 1;
        //间隔值
        float length = 0.2f;
        float height = 0.15f;
        float yheight = 0.55f;
        float angel = 90f;
        float interval = length / (HandZone.childCount + 1);
        float HeightInterval =
            height / (HandZone.childCount + 1);
        float yInterval = yheight / (HandZone.childCount + 1);
        float rotate;
        if (HandZone.childCount != 0)
            rotate = angel / (HandZone.childCount);
        else
        {
            //Debug.Log("child==0 error");
            return;
        }
        float start = length / 2;
       // float startAngel = angel / 2;
        GameObject ro = new GameObject();//用于存储旋转的值

        foreach (Transform t in HandZone.transform)
        {
            //进行排列
            Vector3 p = new Vector3(start - x * interval, -x * yInterval, x * (HeightInterval));
            if (x != 1)
            {
                p.z = p.z + 0.015f;
            }
            p = HandZone.TransformPoint(p);
            if (Type == 1)
                iTween.MoveTo(t.gameObject, p, 0.5f);
            else
                t.position = p;
            ro.transform.localEulerAngles = new Vector3(305, 0, 1);
            ro.transform.Rotate(Vector3.up, 0 - rotate * (x - 1));
            t.transform.localEulerAngles = ro.transform.localEulerAngles;
            t.Rotate(Vector3.forward, -2f);
            t.GetComponent<CardMoving>().index = x;
            x++;
        }
        Destroy(ro);
        if (Type == 1)
            Invoke("canMove", 0.5f);//要等上面itween.moveto完才可以移动
        else
            canMove();
    }
    void canMove()
    {
        CameraMoving.Instance.canMove = true;
    }
    public IEnumerator EnemyShowCard()
    {
        if (!LevelManager.Instance.IsOnline &&  (enemyResidue > 0 ||AI.Instance.aiType==AIType.RoundList)|| LevelManager.Instance.IsOnline  )
        {
            GameObject c = (GameObject)Instantiate(cardObject, EnemyHand.position, EnemyHand.rotation);
            getID(c.transform, false);
            EnemyListNum++;
            iTween.MoveTo(c.gameObject, ECardShow.position, 0.5f);
            iTween.RotateTo(c.gameObject, new Vector3(-50f, 0f, 0f), 1f);
            iTween.MoveTo(c.gameObject, iTween.Hash("position", EnemyList.position + (EnemyListNum - 1) * new Vector3(0.18f, 0, 0), "delay", 2));
            c.transform.parent = EnemyList;
            Card cardScript = c.GetComponent<Card>();
            yield return new WaitForSeconds(1f);
            StartCoroutine(cardScript.Use());
            //Debug.Log("escEnd");
        }
    }
    public IEnumerator ShowNewCard()
    {
        EnemyListNum=0;
        for(int i=0;i<newCardList.Count;i++)
        {
            ShowNewCard(newCardList[i]);
            yield return new WaitForSeconds(2f);
        }
    }
    void ShowNewCard(string ID)
    {
        GameObject c = (GameObject)Instantiate(cardObject, EnemyHand.position, EnemyHand.rotation);
        Card cardScript = c.gameObject.AddComponent<Card>();
        c.gameObject.AddComponent<CardMoving>().BacklightShowHide(true);
        cardScript.ID = ID;
        iTween.MoveTo(c.gameObject,NewCardList.position + (NewCardList.childCount - 1) * new Vector3(0.18f, 0, 0), 0.5f);
        iTween.RotateTo(c.gameObject, new Vector3(-50f, 0f, 0f), 1f);
        c.transform.parent = NewCardList;
    }
    public void draw(bool heroPower = false)
    {
        if (residue > 0)
        {
            GameObject c = (GameObject)Instantiate(cardObject, mycarddeap.position, transform.rotation);
            getID(c.transform,true,heroPower);
            c.transform.parent = HandZone;
            childcount = HandZone.childCount;
            c.transform.Rotate(-90f, 180f, 180f);
            clearUpHand(1);
        }
        else
        {
            hero.damage = 2;
            GuideText.Instance.ReturnText("NoCard");
            EnergyManager.Instance.WinLose();
            clearUpHand(1);
        }
    }
    void getID(Transform c, bool isHeros,bool heroPower=false)
    {
        Card cardScript = c.gameObject.AddComponent<Card>();
        c.gameObject.AddComponent<CardMoving>();
        cardScript.isHeros = isHeros;
        if (heroPower)
        {
            cardScript.ID = "S00";
            return;
        }
            
        int index;
        if (isHeros)
        {
            if (LevelManager.Instance.level<=3 && !LevelManager.Instance.IsOnline || LevelManager.Instance.level==0)//教学关&&不是在线对战
            {
                index = residue - 1;
            }
            else
            {
                index = Random.Range(0, residue);
            }
             
            cardScript.ID = CardList[index];//传递卡牌ID
            CardList.RemoveAt(index);
            residue--;
            return;
        }
        else
        {
            if (!LevelManager.Instance.IsOnline)//本地关卡
            {
                //Debug.Log(AI.Instance.aiType.ToString());
                if(AI.Instance.aiType==AIType.CardList)
                {
                    index = enemyResidue - 1;
                    cardScript.ID = EnemyCardList[index];//传递卡牌ID
                    EnemyCardList.RemoveAt(index);
                    enemyResidue--;
                    return ;
                }
                if(AI.Instance.aiType==AIType.RoundList)
                {
                    //Debug.Log("AIType.RoundList");
                    cardScript.ID = EnergyManager.Instance.enemyOperationID[EnergyManager.Instance.enemyOperationIndex];
                    //Debug.Log("对面使用了" + cardScript.ID);
                    return;
                }
                
            }
            else//联机
            {
                cardScript.ID = EnergyManager.Instance.enemyOperationID[EnergyManager.Instance.enemyOperationIndex];
                //Debug.Log("对面使用了"+cardScript.ID);
            }
        }
    }
    public void StartTurn()
    {
        draw();
        EnemyListNum = 0;
    }
    public void StartGame()
    {
        if (LevelManager.Instance.level > 3 || LevelManager.Instance.IsOnline)
            draw(true);//英雄技能卡
        for (int i = 0; i < 3; i++)
            draw();
    }
    public void EndGame()
    {
        foreach(Transform card in HandZone.transform)
        {
            card.GetComponent<Card>().Destroy();
        }
        foreach(Transform card in EnemyList.transform)
        {
            card.GetComponent<Card>().Destroy();
        }
        foreach (Transform card in NewCardList.transform)
        {
            card.GetComponent<Card>().Destroy();
        }
    }
}
