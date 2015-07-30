using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum LevelType
{
    TuitionI, 
    TuitionII, 
    TuitionIII, 
    EasyI, 
    EasyII,
    PrepareRefresh,
    DD_JD_1, DD_JD_2,
    DD_Small,
    SC_MR_1, SC_MR_2, SC_MR_3,
    GY_DD_Small,
    ZS_DD_1, ZS_DD_2,
    YZAM_1,
    YZAM_2,
    Heal
}
public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    //public static bool Guiding = false;
    public int level=1;
    public List<string> key;
    public int debugLevel;

    //其他脚本涉及开关的物体
    public GameObject ESC;
    public GameObject RoundLabel;
    public GameObject ExplainLabel, EndTurn;
    public GameObject shalou;
    public GameObject DeckPanel;
    public UILabel levelLabel;
    public GameObject pattern1, pattern2;

    public bool IsOnline;
    public Hero hero, enemyHero;
    Deck deck;
    string[] newCardList ={
                             "",//0
                             "",//1
                             "",//2
                             "",//3
                             "E10C04",//4
                             "E02C08",//5
                             "E03C23",//6
                             "E26E04S71",//7
                             "E09C24S72",//8
                             "E15C27E06",//9
                             "E21C26E17",//10
                             "E24E28C01",//11
                             "C03C17C29C13",//12
                             "",//13
                             "",//14
                             "",//15
                             ""//16
                         };
	// Use this for initialization
    void Start()
    {
        if(!PlayerPrefs.HasKey("FirstTime"+XmlReader.Instance.version))
        {
            PlayerPrefs.SetString("啦啦啦", "C02C05C07E01E04E10E10S73S70S69S10S08S08S07S06S06S04S02S02S01");
            PlayerPrefs.SetInt("FirstTime" + XmlReader.Instance.version, 0);
            PlayerPrefs.SetInt("Level", 1);
            //Debug.Log("FirstTime");
        }
        LevelManager.Instance = this;
#if UNITY_EDITOR 
        level = debugLevel;
#else
        level = PlayerPrefs.GetInt("Level");
#endif
        if(level==0)
        {
            level = 1;
        }
        hero = GameObject.Find("Hero").GetComponent<Hero>();
        enemyHero = GameObject.Find("EnemyHero").GetComponent<Hero>();
        //ESC = GameObject.Find("EscPanel");
        levelLabel = GameObject.Find("StartPanel/StartButton/level").GetComponent<UILabel>();
        UpdateLevelLabel();
        pattern1 = transform.FindChild("Pattern1/EnemyCardShow").gameObject;
        pattern2 = transform.FindChild("Pattern2/EnemyCardShow").gameObject;

        pattern1.SetActive(false);
        pattern2.SetActive(false);
        ESC.SetActive(false);
        ExplainLabel.SetActive(false);
        EndTurn.SetActive(false);
        shalou.SetActive(false);
        DeckPanel.SetActive(false);
        IsOnline = false;
    }
    void UpdateLevelLabel()
    {
        levelLabel.text = "Level " + level.ToString();
    }
	
	
	// Update is called once per frame
	void Update () {
	}
    public void ChangeDeck()
    {
        DeckPanel.SetActive(true);
        CameraMoving.Instance.Move(3);
        deck = DeckPanel.AddComponent<Deck>();
    }
    void SetNewCard(int level)
    {
        if(level<newCardList.Length)
        DrawCard.Instance.GetNewCardList(newCardList[level]);
    }
    void SetLevel(LevelType levelType)
    {
        DrawCard.Instance.GetCardList("啦啦啦");
        List<Point> keng = new List<Point>();
        switch(levelType)
        {
            case LevelType.TuitionI:
                DrawCard.Instance.GetCardListByList("S08S08S08S08S08S08S08S08S08S08S08S08S08S10S10S12S08S03S02S01");
                AI.Instance.aiType = AIType.RoundList;
                DrawCard.Instance.EnemyRoundList.Add("S01");//1
                DrawCard.Instance.EnemyRoundList.Add("S04");//2
                DrawCard.Instance.EnemyRoundList.Add("E10");//3
                DrawCard.Instance.EnemyRoundList.Add("S01");//4
                DrawCard.Instance.EnemyRoundList.Add("S01");//5
                DrawCard.Instance.EnemyRoundList.Add("S01");//6
                DrawCard.Instance.EnemyRoundList.Add("S01");//7
                DrawCard.Instance.EnemyRoundList.Add("S67");//8
                DrawCard.Instance.EnemyRoundList.Add("S01");//9
                DrawCard.Instance.EnemyRoundList.Add("S01");//10
                DrawCard.Instance.EnemyRoundList.Add("S01");//11
                DrawCard.Instance.EnemyRoundList.Add("S67");//12
                DrawCard.Instance.EnemyRoundList.Add("S01");//13
                DrawCard.Instance.EnemyRoundList.Add("S01");//14
                DrawCard.Instance.EnemyRoundList.Add("S67");//15
                DrawCard.Instance.EnemyRoundList.Add("S01");//16
                key.Add("MoveCrystal");
                key.Add("LeftRightAttack");
                key.Add("AddEnergy");
                key.Add("AttackMagic");
                key.Add("Freedom");
                key.Add("TurnRed");
                key.Add("ShowTrajectory");
                key.Add("Crash");
                key.Add("FullEnergy");
                key.Add("OnlyFreedom");
                key.Add("MouseOver");
                key.Add("MouseDown");
                key.Add("Interesting");
                break;
            case LevelType.TuitionII:
                DrawCard.Instance.GetCardListByList("S08S08S08S08S08S08S08S08S08S02S13S07S13E02E10S14E10E01S06E01");
                AI.Instance.aiType = AIType.RoundList;
                DrawCard.Instance.EnemyRoundList.Add("S67");//1
                DrawCard.Instance.EnemyRoundList.Add("S67");//2
                DrawCard.Instance.EnemyRoundList.Add("S02");//3
                DrawCard.Instance.EnemyRoundList.Add("S69");//4
                DrawCard.Instance.EnemyRoundList.Add("S08");//5
                DrawCard.Instance.EnemyRoundList.Add("S08");//6
                DrawCard.Instance.EnemyRoundList.Add("S01");//7
                DrawCard.Instance.EnemyRoundList.Add("S74");//8
                DrawCard.Instance.EnemyRoundList.Add("S08");//9
                DrawCard.Instance.EnemyRoundList.Add("S08");//10
                DrawCard.Instance.EnemyRoundList.Add("S74");//11
                DrawCard.Instance.EnemyRoundList.Add("S67");//12
                DrawCard.Instance.EnemyRoundList.Add("S01");//13
                DrawCard.Instance.EnemyRoundList.Add("S01");//14
                DrawCard.Instance.EnemyRoundList.Add("S08");//15
                //key.Add("TopDownAura");
                key.Add("Link");
                key.Add("CostEnergy");
                key.Add("ExplainLabel");
                key.Add("CardBack");
                key.Add("DestroyAura");
                key.Add("PatternChoose");
                key.Add("Refresh");
                key.Add("RedundentLink");
                break;
            case LevelType.TuitionIII:
                DrawCard.Instance.GetCardListByList("S08S08S08S08S08S08S08S08S08S68C02S07S75C07S10S08C02S67S67C07");
                AI.Instance.aiType = AIType.RoundList;
                DrawCard.Instance.EnemyRoundList.Add("S01");//1
                DrawCard.Instance.EnemyRoundList.Add("S02");//2
                DrawCard.Instance.EnemyRoundList.Add("S08");//3
                DrawCard.Instance.EnemyRoundList.Add("S69");//4
                DrawCard.Instance.EnemyRoundList.Add("C03");//5
                DrawCard.Instance.EnemyRoundList.Add("S08");//6
                DrawCard.Instance.EnemyRoundList.Add("S12");//7
                DrawCard.Instance.EnemyRoundList.Add("S08");//8
                DrawCard.Instance.EnemyRoundList.Add("S08");//9
                DrawCard.Instance.EnemyRoundList.Add("S74");//10
                DrawCard.Instance.EnemyRoundList.Add("S01");//11
                DrawCard.Instance.EnemyRoundList.Add("S69");//12
                DrawCard.Instance.EnemyRoundList.Add("S08");//13
                key.Add("Defense");
                key.Add("DefensePosition");
                key.Add("Pattern");
                key.Add("Pattern2");
                key.Add("WaitToFire");
                key.Add("Prepare");
                key.Add("RedundentLink");
                break;
            case LevelType.EasyI:
                AI.Instance.aiType = AIType.CardList;
                DrawCard.Instance.GetEnemyCardListByList("S08S08S08S08S08S08S08C23S69S01S74S08S08S12S08C03S69S08S02S01");
                break;
            case LevelType.EasyII:
                AI.Instance.aiType = AIType.CardList;
                DrawCard.Instance.GetEnemyCardListByList("S08S08E05S08S08C03S08S08C23S01S74S08C23S12S08C03S69S08S02S01");
                break;
            case LevelType.PrepareRefresh:
                //天启+刷新
                AI.Instance.SetAIStyle(AIStyle.RefreshProtector);
                AI.Instance.aiType = AIType.WeakAI;
                DrawCard.Instance.GetEnemyCardListByList("S00S01S15S73S15S08S73S16S74S16S74S08S04S12S13S13S69S12S11S11");
                //和坑没有关系
                keng.Add(new Point(4, 4));
                keng.Add(new Point(4, 2));
                keng.Add(new Point(1, 1));
                keng.Add(new Point(7, 1));
                keng.Add(new Point(4, 0));
                keng.Add(new Point(1, 3));
                keng.Add(new Point(7, 3));
                keng.Add(new Point(6, 4));
                keng.Add(new Point(2, 4));
                keng.Add(new Point(0, 2));
                keng.Add(new Point(8, 2));
                keng.Add(new Point(2, 0));
                AI.Instance.ReadKengList(keng);
                break;

            case LevelType.DD_JD_1:
                //动荡阵列+激荡阵列[1]
                AI.Instance.SetAIStyle(AIStyle.Conservative);
                AI.Instance.aiType = AIType.WeakAI;
                DrawCard.Instance.GetEnemyCardListByList("S00E01E10E01E02E10S69S69S07S10S70S10S15S13S03S14S16S13S16S11");
                keng.Add(new Point(4, 4));
                keng.Add(new Point(4, 2));
                keng.Add(new Point(1, 1));
                keng.Add(new Point(7, 1));
                keng.Add(new Point(4, 0));
                keng.Add(new Point(1, 3));
                keng.Add(new Point(7, 3));
                keng.Add(new Point(6, 4));
                keng.Add(new Point(2, 4));
                keng.Add(new Point(0, 2));
                keng.Add(new Point(8, 2));
                keng.Add(new Point(2, 0));
                AI.Instance.ReadKengList(keng);
                break;
            case LevelType.DD_JD_2:
                //动荡阵列+激荡阵列+元素导流[2]
                AI.Instance.SetAIStyle(AIStyle.Conservative);
                AI.Instance.aiType = AIType.WeakAI;
                DrawCard.Instance.GetEnemyCardListByList("S00E10E01E01E10E02S69E06S69S10S13S74S10S15S03S14S16S13S16S11");
                keng.Add(new Point(1, 1));
                keng.Add(new Point(4, 4));
                keng.Add(new Point(4, 2));
                keng.Add(new Point(7, 1));
                keng.Add(new Point(4, 0));
                keng.Add(new Point(1, 3));
                keng.Add(new Point(7, 3));
                keng.Add(new Point(6, 4));
                keng.Add(new Point(2, 4));
                keng.Add(new Point(0, 2));
                keng.Add(new Point(8, 2));
                keng.Add(new Point(2, 0));
                AI.Instance.ReadKengList(keng);
                break;
            case LevelType.DD_Small:
                //小法术+动荡阵列
                AI.Instance.SetAIStyle(AIStyle.CrazyDog);
                AI.Instance.aiType = AIType.WeakAI;
                DrawCard.Instance.GetEnemyCardListByList("S00E04S68E04E01S08S04S69S08S07S10S13S74S12S12S10S15S04S13S16");
                keng.Add(new Point(3,3));
                keng.Add(new Point(2,2));
                keng.Add(new Point(4, 2));
                keng.Add(new Point(5,3));
                keng.Add(new Point(6,2));
                keng.Add(new Point(3,1));
                keng.Add(new Point(5,1));
                keng.Add(new Point(1,1));
                keng.Add(new Point(7,1));
                keng.Add(new Point(4,4));
                keng.Add(new Point(4,0));
                keng.Add(new Point(1,3));
                AI.Instance.ReadKengList(keng);
                break;
            case LevelType.SC_MR_1:
                //三重轰击+末日施法者[1]
                AI.Instance.aiType = AIType.RoundList;
                DrawCard.Instance.EnemyRoundList.Add("S67");//1
                DrawCard.Instance.EnemyRoundList.Add("");//2
                DrawCard.Instance.EnemyRoundList.Add("C07E01");//3
                DrawCard.Instance.EnemyRoundList.Add("S02");//4
                DrawCard.Instance.EnemyRoundList.Add("S08");//5
                DrawCard.Instance.EnemyRoundList.Add("C02");//6
                DrawCard.Instance.EnemyRoundList.Add("S68S68");//7
                DrawCard.Instance.EnemyRoundList.Add("C02");//8
                DrawCard.Instance.EnemyRoundList.Add("E03S69S69");//9
                DrawCard.Instance.EnemyRoundList.Add("S69");//10
                DrawCard.Instance.EnemyRoundList.Add("S08S13");//11
                DrawCard.Instance.EnemyRoundList.Add("E01S14");//12
                DrawCard.Instance.EnemyRoundList.Add("S09S00");//13
                DrawCard.Instance.EnemyRoundList.Add("S14");//14
                DrawCard.Instance.EnemyRoundList.Add("S14");//15
                DrawCard.Instance.EnemyRoundList.Add("S14");//16
                DrawCard.Instance.EnemyRoundList.Add("S00S13");//17
                DrawCard.Instance.EnemyRoundList.Add("S14");//18
                break;
            case LevelType.SC_MR_2:
                //三重轰击+末日施法者[2]
                AI.Instance.aiType = AIType.RoundList;
                DrawCard.Instance.EnemyRoundList.Add("S67");//1
                DrawCard.Instance.EnemyRoundList.Add("");//2
                DrawCard.Instance.EnemyRoundList.Add("C07E01");//3
                DrawCard.Instance.EnemyRoundList.Add("S02");//4
                DrawCard.Instance.EnemyRoundList.Add("S08");//5
                DrawCard.Instance.EnemyRoundList.Add("C02");//6
                DrawCard.Instance.EnemyRoundList.Add("S68S68");//7
                DrawCard.Instance.EnemyRoundList.Add("C02");//8
                DrawCard.Instance.EnemyRoundList.Add("E03S69S69");//9
                DrawCard.Instance.EnemyRoundList.Add("E02");//10
                DrawCard.Instance.EnemyRoundList.Add("S08S13");//11
                DrawCard.Instance.EnemyRoundList.Add("E01S14");//12
                DrawCard.Instance.EnemyRoundList.Add("S09S00");//13
                DrawCard.Instance.EnemyRoundList.Add("S14");//14
                DrawCard.Instance.EnemyRoundList.Add("S14");//15
                DrawCard.Instance.EnemyRoundList.Add("S14");//16
                DrawCard.Instance.EnemyRoundList.Add("S00S13");//17
                DrawCard.Instance.EnemyRoundList.Add("S14");//18
                break;
            case LevelType.SC_MR_3:
                //三重轰击+末日施法者[3]
                AI.Instance.aiType = AIType.RoundList;
                DrawCard.Instance.EnemyRoundList.Add("S67");//1
                DrawCard.Instance.EnemyRoundList.Add("");//2
                DrawCard.Instance.EnemyRoundList.Add("C07E01");//3
                DrawCard.Instance.EnemyRoundList.Add("S15");//4
                DrawCard.Instance.EnemyRoundList.Add("S08");//5
                DrawCard.Instance.EnemyRoundList.Add("C02C07");//6
                DrawCard.Instance.EnemyRoundList.Add("S68S68");//7
                DrawCard.Instance.EnemyRoundList.Add("E03C02");//8
                DrawCard.Instance.EnemyRoundList.Add("S69S69");//9
                DrawCard.Instance.EnemyRoundList.Add("E02");//10
                DrawCard.Instance.EnemyRoundList.Add("S08S13");//11
                DrawCard.Instance.EnemyRoundList.Add("E01S14");//12
                DrawCard.Instance.EnemyRoundList.Add("S16S00");//13
                DrawCard.Instance.EnemyRoundList.Add("S14");//14
                DrawCard.Instance.EnemyRoundList.Add("S14");//15
                DrawCard.Instance.EnemyRoundList.Add("S14");//16
                DrawCard.Instance.EnemyRoundList.Add("S00S13");//17
                DrawCard.Instance.EnemyRoundList.Add("S14");//18
                break;
            case LevelType.GY_DD_Small:
                //小法术+光翼+动荡阵列
                AI.Instance.aiType = AIType.RoundList;
                DrawCard.Instance.EnemyRoundList.Add("S67");//1
                DrawCard.Instance.EnemyRoundList.Add("");//2
                DrawCard.Instance.EnemyRoundList.Add("E04");//3
                DrawCard.Instance.EnemyRoundList.Add("S03");//4
                DrawCard.Instance.EnemyRoundList.Add("E04S08");//5
                DrawCard.Instance.EnemyRoundList.Add("S08S07");//6
                DrawCard.Instance.EnemyRoundList.Add("E09S68");//7
                DrawCard.Instance.EnemyRoundList.Add("S11");//8
                DrawCard.Instance.EnemyRoundList.Add("E01S69S69");//9
                DrawCard.Instance.EnemyRoundList.Add("S10S00");//10
                DrawCard.Instance.EnemyRoundList.Add("S10S00");//11
                DrawCard.Instance.EnemyRoundList.Add("S13S13");//12
                DrawCard.Instance.EnemyRoundList.Add("S13S00");//13
                DrawCard.Instance.EnemyRoundList.Add("S13S13");//14
                break;
            case LevelType.ZS_DD_1:
                //咒术风暴+动荡阵列[1]
                AI.Instance.aiType = AIType.CardList;
                DrawCard.Instance.GetEnemyCardListByList("C04S71S74S08C02C04S08S10C05S70S15C03C23C05S16C03S16S08S04S01");
                break;
            case LevelType.ZS_DD_2:
                //咒术风暴+动荡阵列[2]
                AI.Instance.aiType = AIType.CardList;
                DrawCard.Instance.GetEnemyCardListByList("C04S71S74S08C02C04S08S10C05S70S15C03E04C05S16C03S16S08S04S01");
                break;
            
            case LevelType.YZAM_1:
                //月之暗面[1]
                AI.Instance.aiType = AIType.RoundList;
                DrawCard.Instance.EnemyRoundList.Add("");//1
                DrawCard.Instance.EnemyRoundList.Add("");//2
                DrawCard.Instance.EnemyRoundList.Add("S67");//3
                DrawCard.Instance.EnemyRoundList.Add("S73");//4
                DrawCard.Instance.EnemyRoundList.Add("S67E28");//5
                DrawCard.Instance.EnemyRoundList.Add("E03S68");//6
                DrawCard.Instance.EnemyRoundList.Add("S75");//7
                DrawCard.Instance.EnemyRoundList.Add("S76");//8
                DrawCard.Instance.EnemyRoundList.Add("S75");//9
                DrawCard.Instance.EnemyRoundList.Add("S76");//10
                DrawCard.Instance.EnemyRoundList.Add("S72");//11
                DrawCard.Instance.EnemyRoundList.Add("S75S00");//12
                DrawCard.Instance.EnemyRoundList.Add("S00S70");//13
                DrawCard.Instance.EnemyRoundList.Add("S00S70");//14
                DrawCard.Instance.EnemyRoundList.Add("S71");//15
                DrawCard.Instance.EnemyRoundList.Add("S72");//16
                DrawCard.Instance.EnemyRoundList.Add("S68S68S00");//17
                DrawCard.Instance.EnemyRoundList.Add("S72");//18
                DrawCard.Instance.EnemyRoundList.Add("S00");//19
                // keng.Add(new Point(4,4));
                //keng.Add(new Point(4,2));
                //keng.Add(new Point(3,3));
                //keng.Add(new Point(5,3));
                //keng.Add(new Point(4,0));
                //keng.Add(new Point(1,1));
                //keng.Add(new Point(0,2));
                //keng.Add(new Point(7,1));
                //keng.Add(new Point(8,2));
                //keng.Add(new Point(2, 4));
                //keng.Add(new Point(6, 4));
                //keng.Add(new Point(7, 3));
                break;
            case LevelType.YZAM_2:
                //月之暗面[2]
                AI.Instance.SetAIStyle(AIStyle.YZAM);
                AI.Instance.aiType = AIType.WeakAI;
                keng.Add(new Point(4,4));
                keng.Add(new Point(4,2));
                keng.Add(new Point(1, 1));
                keng.Add(new Point(1, 3));
                keng.Add(new Point(7, 1));
                keng.Add(new Point(7, 3));
                keng.Add(new Point(4,0));
                keng.Add(new Point(3,3));
                keng.Add(new Point(5,3));
                keng.Add(new Point(0,2));
                keng.Add(new Point(8,2));
                keng.Add(new Point(2, 4));
                AI.Instance.ReadKengList(keng);
                DrawCard.Instance.GetEnemyCardListByList("S00E02S15E28S73S74S69S75S76S75S76S71S70S70S69S68S13S67S16S08");
                AI.Instance.ReadKengList(keng);
                break;
           
            case LevelType.Heal:
                AI.Instance.aiType = AIType.RoundList;
                DrawCard.Instance.EnemyRoundList.Add("");//1
                DrawCard.Instance.EnemyRoundList.Add("");//2
                DrawCard.Instance.EnemyRoundList.Add("E09C23");//3
                DrawCard.Instance.EnemyRoundList.Add("S00");//4
                DrawCard.Instance.EnemyRoundList.Add("E15S00");//5
                DrawCard.Instance.EnemyRoundList.Add("E21S08");//6
                DrawCard.Instance.EnemyRoundList.Add("E04S07");//7
                DrawCard.Instance.EnemyRoundList.Add("S10E01");//8
                DrawCard.Instance.EnemyRoundList.Add("S10S00");//9
                DrawCard.Instance.EnemyRoundList.Add("S15S15S04");//10
                DrawCard.Instance.EnemyRoundList.Add("S08S13");//11
                DrawCard.Instance.EnemyRoundList.Add("E01S14");//12
                DrawCard.Instance.EnemyRoundList.Add("S16S00");//13
                DrawCard.Instance.EnemyRoundList.Add("S13S13");//14
                DrawCard.Instance.EnemyRoundList.Add("S12S00");//15
                DrawCard.Instance.EnemyRoundList.Add("S12S03");//16
                DrawCard.Instance.EnemyRoundList.Add("S00S13");//17

                break;

            
        }
    }
    void SetLevel(int level)
    {
        key.Clear();
        SetNewCard(level);
        GuideText.Instance.ReturnText("LevelText");
        AI.Instance.SetAIStyle(AIStyle.CrazyDog);
        switch(level)
        {
            case 1:
                SetLevel(LevelType.TuitionI);
                break;
            case 2:
                SetLevel(LevelType.TuitionII);
                break;
            case 3:
                SetLevel(LevelType.TuitionIII);
                break;
            case 4:
                SetLevel(LevelType.EasyI);
                break;
            case 5:
                SetLevel(LevelType.EasyII);
                break;

            case 6:
                SetLevel(LevelType.PrepareRefresh);
                break;

            case 7:
                SetLevel(LevelType.SC_MR_1);
                break;

            case 8:
                SetLevel(LevelType.GY_DD_Small);
                break;

            case 9:
                SetLevel(LevelType.DD_JD_1);
                break;

            case 10:
                SetLevel(LevelType.DD_Small);
                break;

            case 11:
                SetLevel(LevelType.SC_MR_2);
                break;

            case 12:
                SetLevel(LevelType.ZS_DD_1);
                break;

            case 13:
                SetLevel(LevelType.DD_JD_2);
                break;

            case 14:
                SetLevel(LevelType.SC_MR_3);
                break;

            case 15:
                SetLevel(LevelType.ZS_DD_2);
                break;
            case 16:
                SetLevel(LevelType.YZAM_1);
                break;
            case 17:
                SetLevel(LevelType.YZAM_2);
                break;
            case 18:
                SetLevel(LevelType.Heal);
                break;
            default:
                Debug.Log("default");
                SetLevel(Random.Range(8, 18));
                break;
        }
    }

    public void RoundLabelShow()
    {
        RoundLabel.SetActive(true);
        //RoundLabel.transform.FindChild("Label").GetComponent<UILabel>().text =
        //    "新回合";//"Round "+ EnergyManager.Instance.roundCount.ToString();
        Invoke("RoundLabelHide", 3f);
    }
    void RoundLabelHide()
    {
        RoundLabel.SetActive(false);
    }
    public void StartGame()
    {
//        Debug.Log("StartGame");
        hero.Restart();
        enemyHero.Restart();
        ExplainLabel.SetActive(true);
        shalou.SetActive(true);
        gameObject.AddComponent<EnergyManager>();
        gameObject.AddComponent<DrawCard>();

        if (!LevelManager.Instance.IsOnline)
        {
            if (level == 0)
                level = 1;
            SetLevel(level);
        }
        else
        {
            DrawCard.Instance.GetCardList("啦啦啦");
        }
        GameObject.Find("ShaLou").AddComponent<SandClock>();
        CameraMoving.Instance.Move(0);
    }
    public void EndGame()
    {
        //        Debug.Log("EndGame");
        CameraMoving.Instance.EndGame();
        RayTest.Instance.EndGame();
        gameObject.GetComponent<EnergyManager>().Destroy();
        DrawCard.Instance.EndGame();
        Destroy(gameObject.GetComponent<DrawCard>());
        Destroy(transform.FindChild("ShaLou").GetComponent<SandClock>());
        //        Destroy(GameObject.Find("EndTurnTexture").GetComponent<Sand>());
        ESC.SetActive(false);
        ExplainLabel.SetActive(false);
        EndTurn.SetActive(false);
        shalou.SetActive(false);
        ShieldManager.Instance.EndGame();
        AttackManager.Instance.EndGame();
        AuraManager.Instance.EndGame();
        foreach (Transform crystal in GameObject.Find("CrystalUsed").transform)
        {
            crystal.GetComponent<Move>().Destroy();
        }
        if(IsOnline)
        {
            //level = 1;
            Client.Instance.OffLine();
        }
        UpdateLevelLabel();
    }

}
