using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Deck : MonoBehaviour {
    public static Deck Instance;
    public GameObject card;

    //用于显示位置和未保存时list的储存
    public Transform CardZone;
    public Transform PoolZone;


    //用于初始化和保存
    List<string> PoolList = new List<string>();
    List<string> CardList = new List<string>();
    //List<string> DeckList = new List<string>();
    UILabel numberLabel;
    void GetPoolList()
    {
        //S;1-22,5\11\12不行
        //C:02,03,04,05,07,08,10,11,23
        for (int i = 0; i < 2; i++)
        {
            for (int j = 1; j < 17; j++)
            {
                PoolList.Add("S" + (j >= 10 ? "" : "0") + j.ToString());
            }
            for (int j = 67; j < 77; j++)
            {
                PoolList.Add("S" + (j >= 10 ? "" : "0") + j.ToString());
            }
            PoolList.Remove("S05");
            PoolList.Remove("S71");
            PoolList.Remove("S72");
            if(LevelManager.Instance.level>7)
            {
                PoolList.Add("S71");
            }
            if (LevelManager.Instance.level > 8)
            {
                PoolList.Add("S72");
            }

            PoolList.Add("C02");


            PoolList.Add("C05");
            PoolList.Add("C07");
            PoolList.Add("C11");

            if (LevelManager.Instance.level > 4)
            {
                PoolList.Add("C04");
                PoolList.Add("E10");
            }
            if (LevelManager.Instance.level > 5)
            {
                PoolList.Add("C08");
                PoolList.Add("E01");
            }
            if (LevelManager.Instance.level > 6)
            {
                PoolList.Add("E03");
                PoolList.Add("C23");
            }
            if (LevelManager.Instance.level > 7)
            {
                PoolList.Add("E04");
                PoolList.Add("E26");
            }
            if (LevelManager.Instance.level > 8)
            {
                PoolList.Add("E09");
                PoolList.Add("C24");
                PoolList.Add("S71");
            }
            if (LevelManager.Instance.level > 9)
            {
                PoolList.Add("C27");
                PoolList.Add("S72");
            }
            if (LevelManager.Instance.level > 10)
            {
                PoolList.Add("C26");
            }
            if (LevelManager.Instance.level > 11)
            {
                PoolList.Add("C01");
            }
            if (LevelManager.Instance.level > 12)
            {
                PoolList.Add("C03");
                PoolList.Add("C13");
                PoolList.Add("C17");
                PoolList.Add("C29");
            }
        }

        if (LevelManager.Instance.level > 5)
        {
            PoolList.Add("E02");
        }
        if (LevelManager.Instance.level > 9)
        {
            PoolList.Add("E15");
        }
        if (LevelManager.Instance.level > 10)
        {
            PoolList.Add("E21");
            PoolList.Add("E06");
        }
        if (LevelManager.Instance.level > 11)
        {
            PoolList.Add("E24");
            PoolList.Add("E28");
            PoolList.Add("E17");
        }

    }
	// Use this for initialization
	void Start () {
        Deck.Instance = this;
        card = (GameObject)Resources.Load("card new 1");
        CardZone = GameObject.Find("Deck").transform;
        PoolZone = GameObject.Find("Pool").transform;
        numberLabel = transform.FindChild("numberLabel").GetComponent<UILabel>();

        GetPoolList();
        ReadDeck("啦啦啦");
       // Debug.Log(PlayerPrefs.GetString("啦啦啦"));
        CardList.Sort();
        PoolList.Sort();
        foreach(string s in CardList)
        {
            PoolList.Remove(s);
        }
        StartCoroutine(ShowCardList());
        StartCoroutine(ShowCardPool());
	}
    void UpdateNumberLabel()
    {
        numberLabel.text = CardZone.transform.childCount.ToString() + "/20";
        if (CanSave())
            numberLabel.color = Color.black;
        else
        {
            numberLabel.text = "牌组数量需要20:" + numberLabel.text;
            numberLabel.color = Color.red;
        }
    }
    bool CanSave()
    {
        if (CardZone.transform.childCount == 20)
            return true;
        return false;
    }
    public void SaveButton()
    {
        if(CanSave())
        {
            SaveDeck("啦啦啦");
            CameraMoving.Instance.Move(-1);
            Destroy(this);
            foreach (Transform t in PoolZone)
            {
                t.GetComponent<Card>().Destroy();
            }
            foreach (Transform t in CardZone)
            {
                t.GetComponent<Card>().Destroy();
            }
            gameObject.SetActive(false);
        }
        else
        {
            UpdateNumberLabel();
        }
        
    }
    void ReadDeck(string deckName)
    {
        CardList.Clear();
        string s = PlayerPrefs.GetString(deckName);
        for(int i=0;i<20;i++)
        {
            CardList.Add(s.Substring(i * 3, 3));
        }        
    }
    void SaveDeck(string deckName)
    {
        CardList.Clear();
        foreach(Transform temp in CardZone.transform)
        {
            CardList.Add(temp.GetComponent<Card>().ID);
        }
        CardList.Sort();
        string s = "";
        foreach(string cardName in CardList)
        {
            s += cardName;
        }
        PlayerPrefs.SetString(deckName, s);
    }
    void CreateCard(string id, bool inPool, Transform parent, int num)
    {
        GameObject c = (GameObject)Instantiate(card, parent.position +num * (new Vector3(0f, 0f, -0.2f)), transform.rotation);
        c.transform.localEulerAngles = new Vector3(-90, 90, 0f);
        Card cardScript = c.gameObject.AddComponent<Card>();
        DeckCard deckCard = c.gameObject.AddComponent<DeckCard>();
        deckCard.inPool = inPool;
        cardScript.ID = id;//传递卡牌ID
        c.transform.parent = parent;
    }
    IEnumerator ShowCardList()
    {
        for(int i=0;i<CardList.Count;i++)
        {
            CreateCard(CardList[i], false, CardZone, i);
            yield return new WaitForSeconds(0.1f);
        }
        Tile(CardZone);
        UpdateNumberLabel();
        
    }
    IEnumerator ShowCardPool()
    {
        for (int i = 0; i < PoolList.Count; i++)
        {
            CreateCard(PoolList[i], true , PoolZone, i);
            yield return new WaitForSeconds(0.1f);
        }
        Tile(PoolZone);
    }
    void Tile(Transform t)
    {
        //Debug.Log("Tile");
        int x = 0;
        foreach (Transform temp in t.transform)
        {
            temp.position = x * (new Vector3(0f, 0f, -0.2f)) + t.position;
            x++;
        }
    }
    public void ChangeList(Transform t)
    {
        DeckCard deckCard = t.GetComponent<DeckCard>();
        Card cardScript = t.GetComponent<Card>();
       // Debug.Log(cardScript.ID);
        if(deckCard.inPool)
        {
            //foreach (string s in PoolList)
            //{
            //    if (cardScript.ID == s)
            //    {
            //        //Debug.Log("pool");
            //        PoolList.Remove(s);
            //            CardList.Add(s);
            //        t.parent = CardZone;
            //        break;
            //    }
            //}
            t.parent = CardZone;
        }
        else
        {
            //foreach (string s in CardList)
            //{
            //    if (cardScript.ID == s)
            //    {
            //       // Debug.Log("card");
            //        CardList.Remove(s);
            //        if(iniPool)//如果初始化完了
            //            PoolList.Add(s);
            //        t.parent = PoolZone;
            //        break;
            //    }
            //}
            t.parent = PoolZone;
        }

        deckCard.inPool = !deckCard.inPool; 
        Tile();
        UpdateNumberLabel();
    }
    public void Tile()
    {
        Tile(PoolZone);
        Tile(CardZone);
    }
}
