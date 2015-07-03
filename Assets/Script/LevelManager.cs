using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    //public static bool Guiding = false;
    public int level;
    public List<string> key;

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
	// Use this for initialization
    void Start()
    {
        if(!PlayerPrefs.HasKey("FirstTime"+XmlReader.Instance.version))
        {
            PlayerPrefs.SetString("啦啦啦", "C02C05C07E01E04E10E10S73S70S69S10S08S08S07S06S06S04S02S02S01");
            //表演关 PlayerPrefs.SetString("啦啦啦", "S08S16S70S08S13S10C11C03S09S71S70S16C03S08S72C11C02S71C07S01");
            PlayerPrefs.SetInt("FirstTime" + XmlReader.Instance.version, 0);
            PlayerPrefs.SetInt("Level", 1);
            Debug.Log("FirstTime");
        }
        LevelManager.Instance = this;
#if UNITY_EDITOR 
        level = 6;
#else
        level = PlayerPrefs.GetInt("Level");
#endif
        hero = GameObject.Find("Hero").GetComponent<Hero>();
        enemyHero = GameObject.Find("EnemyHero").GetComponent<Hero>();
        //ESC = GameObject.Find("EscPanel");
        levelLabel = GameObject.Find("StartPanel/StartButton/level").GetComponent<UILabel>();
        levelLabel.text = "Level " + level.ToString();
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
	
	
	// Update is called once per frame
	void Update () {
	}
    public void ChangeDeck()
    {
        DeckPanel.SetActive(true);
        CameraMoving.Instance.Move(3);
        deck = DeckPanel.AddComponent<Deck>();
    }
    void SetLevel(int level)
    {
        key.Clear();
        GuideText.Instance.ReturnText("LevelText");
        switch(level)
        {
            //case 0:
            //    AI.Instance.aiType = AIType.CardList;
            //    PlayerPrefs.SetString("Level", "S08S16S70S08S13S10C11C03S09S73S70S16C03S08S74C11C02S73C07S01");
            // // PlayerPrefs.SetString("EnemyLevel0", "S01S03S08C03C08S69C23C04S71C05S07S72S08S08S10S15C03S13S16S70");
            //    PlayerPrefs.SetString("EnemyLevel" , "C23C04S71C05S07S72S08S08S10S70S15C03C08C04S16C03S69S08S03S01");
            //    DrawCard.Instance.GetCardList("Level" );
            //    DrawCard.Instance.GetEnemyCardList("EnemyLevel" );
            //    break;
            case 1:
                AI.Instance.aiType = AIType.CardList;
                PlayerPrefs.SetString("Level" , "S08S08S08S08S08S08S08S08S08S08S08S08S08S10S10S12S08S03S02S01");
                PlayerPrefs.SetString("EnemyLevel" , "S01S01S01S01S01S01S67S01S01S67S01S01S01S67S01S01S01S01S04S01");
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
                DrawCard.Instance.GetCardList("Level" );
                DrawCard.Instance.GetEnemyCardList("EnemyLevel" );
                break;
            case 2:
                AI.Instance.aiType = AIType.CardList;
                PlayerPrefs.SetString("Level" , "S08S08S08S08S08S08S08S08S08S02S13S07S13E02E10S14E10E01S06E01");//E01 E01 E03 E10 E10 E06 
                PlayerPrefs.SetString("EnemyLevel" , "S08S08S08S08S08S08S08S08S69S01S74S08S08S74S08S08S69S02S67S67");
                //PlayerPrefs.SetString("EnemyLevel2", "S01S01S01S01S01S01S01S01S01S01S01S01S01S71S01C03S69S08S02C02");
               // PlayerPrefs.SetString("Level2", "S08S08S08S08S08S08S08S08S08S08S08S08S08S10S10S08C02S67S67S70");
                //key.Add("TopDownAura");
                key.Add("CostEnergy");
                key.Add("ExplainLabel");
                key.Add("CardBack");
                key.Add("DestroyAura");
                key.Add("PatternChoose");
                key.Add("Refresh");
                DrawCard.Instance.GetCardList("Level" );
                DrawCard.Instance.GetEnemyCardList("EnemyLevel" );
                break;
            case 3:
                AI.Instance.aiType = AIType.CardList;
                PlayerPrefs.SetString("Level", "S08S08S08S08S08S08S08S08S08S68C02S07S71C07S10S08C02S67S67C07");
                PlayerPrefs.SetString("EnemyLevel", "S08S08S08S08S08S08S08S08S69S01S74S08S08S12S08C03S69S08S02S01");
                key.Add("Defense");
                key.Add("DefensePosition");
                key.Add("Pattern");
                key.Add("WaitToFire");
                key.Add("Prepare");
                DrawCard.Instance.GetCardList("Level");
                DrawCard.Instance.GetEnemyCardList("EnemyLevel");
                break;
            case 4:
                AI.Instance.aiType = AIType.CardList;
                PlayerPrefs.SetString("EnemyLevel", "S08S08S08S08S08S08S08C23S69S01S74S08S08S12S08C03S69S08S02S01");
                PlayerPrefs.SetString("NewCard" , "E10C04");
                DrawCard.Instance.GetNewCardList("NewCard" );
                DrawCard.Instance.GetCardList("啦啦啦");
                break;
            case 5:
                AI.Instance.aiType = AIType.CardList;
                PlayerPrefs.SetString("EnemyLevel" , "S08S08E05S08S08C03S08S08C23S01S74S08C23S12S08C03S69S08S02S01");
                PlayerPrefs.SetString("NewCard" , "E02C08");
                DrawCard.Instance.GetNewCardList("NewCard" );
                DrawCard.Instance.GetCardList("啦啦啦");
                DrawCard.Instance.GetEnemyCardList("EnemyLevel" );
                break;
            case 6:
                PlayerPrefs.SetString("NewCard" , "E03C23");
                DrawCard.Instance.GetNewCardList("NewCard" );
                DrawCard.Instance.GetCardList("啦啦啦");
                DrawCard.Instance.GetEnemyCardList("EnemyLevel" );
                AI.Instance.aiType = AIType.RoundList;
                DrawCard.Instance.EnemyRoundList.Add("");//1
                DrawCard.Instance.EnemyRoundList.Add("E01");//2
                DrawCard.Instance.EnemyRoundList.Add("E10");//3
                DrawCard.Instance.EnemyRoundList.Add("E10S00");//4
                DrawCard.Instance.EnemyRoundList.Add("S13S13S00");//5
                DrawCard.Instance.EnemyRoundList.Add("E02S00");//6
                DrawCard.Instance.EnemyRoundList.Add("E01S16S14");//7
                DrawCard.Instance.EnemyRoundList.Add("S16S14");//8
                DrawCard.Instance.EnemyRoundList.Add("S00S13S14");//9
                DrawCard.Instance.EnemyRoundList.Add("S00S13S14");//10
                DrawCard.Instance.EnemyRoundList.Add("S00S15S14");//11
                DrawCard.Instance.EnemyRoundList.Add("S00S13S14");//12
                DrawCard.Instance.EnemyRoundList.Add("S00S15S13S14");//13
                DrawCard.Instance.EnemyRoundList.Add("S00S13S14");//14
                DrawCard.Instance.EnemyRoundList.Add("S00S13S14");//15
                DrawCard.Instance.EnemyRoundList.Add("S00S13S14");//16
                DrawCard.Instance.EnemyRoundList.Add("S00S13S14");//17
                DrawCard.Instance.EnemyRoundList.Add("S00S13S14");//18
                break;
            case 7:
                PlayerPrefs.SetString("NewCard" , "E26E04S71");
                DrawCard.Instance.GetNewCardList("NewCard" );
                DrawCard.Instance.GetCardList("啦啦啦");
                DrawCard.Instance.GetEnemyCardList("EnemyLevel" ); 
                AI.Instance.aiType = AIType.RoundList;
                DrawCard.Instance.EnemyRoundList.Add("");//1
                DrawCard.Instance.EnemyRoundList.Add("E01");//2
                DrawCard.Instance.EnemyRoundList.Add("E10");//3
                DrawCard.Instance.EnemyRoundList.Add("E10S00");//4
                DrawCard.Instance.EnemyRoundList.Add("S13S13S00");//5
                DrawCard.Instance.EnemyRoundList.Add("E02S00");//6
                DrawCard.Instance.EnemyRoundList.Add("E01S16S14");//7
                DrawCard.Instance.EnemyRoundList.Add("E06S16S14");//8
                DrawCard.Instance.EnemyRoundList.Add("S00S13S14");//9
                DrawCard.Instance.EnemyRoundList.Add("S00S13S14");//10
                DrawCard.Instance.EnemyRoundList.Add("S00S13S14");//11
                DrawCard.Instance.EnemyRoundList.Add("S00S13S14");//12
                DrawCard.Instance.EnemyRoundList.Add("S00S13S14");//13
                DrawCard.Instance.EnemyRoundList.Add("S00S13S14");//14
                DrawCard.Instance.EnemyRoundList.Add("S00S13S14");//15
                DrawCard.Instance.EnemyRoundList.Add("S00S13S14");//16
                DrawCard.Instance.EnemyRoundList.Add("S00S13S14");//17
                DrawCard.Instance.EnemyRoundList.Add("S00S13S14");//18
                break;
            case 8:

                PlayerPrefs.SetString("NewCard", "E09C24S72");
                DrawCard.Instance.GetNewCardList("NewCard");
                DrawCard.Instance.GetCardList("啦啦啦");
                DrawCard.Instance.GetEnemyCardList("EnemyLevel");
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
            case 9:
                AI.Instance.aiType = AIType.CardList;
                PlayerPrefs.SetString("EnemyLevel", "C04S71S74S08C02C04S08S10C05S70S15C03C23C05S16C03S16S08S04S01");
                PlayerPrefs.SetString("NewCard", "E15C27E06");
                DrawCard.Instance.GetNewCardList("NewCard");
                DrawCard.Instance.GetCardList("啦啦啦");
                DrawCard.Instance.GetEnemyCardList("EnemyLevel");
                break;
            case 10:
                PlayerPrefs.SetString("NewCard", "E21C26E17");
                DrawCard.Instance.GetNewCardList("NewCard");
                DrawCard.Instance.GetCardList("啦啦啦");
                DrawCard.Instance.GetEnemyCardList("EnemyLevel");
                  AI.Instance.aiType = AIType.RoundList;
                DrawCard.Instance.EnemyRoundList.Add("S67");//1
                DrawCard.Instance.EnemyRoundList.Add("S67");//2
                DrawCard.Instance.EnemyRoundList.Add("S01");//3
                DrawCard.Instance.EnemyRoundList.Add("S71");//4
                DrawCard.Instance.EnemyRoundList.Add("E28");//5
                DrawCard.Instance.EnemyRoundList.Add("S75");//6
                DrawCard.Instance.EnemyRoundList.Add("E02S76");//7
                DrawCard.Instance.EnemyRoundList.Add("S76");//8
                DrawCard.Instance.EnemyRoundList.Add("S70");//9
                DrawCard.Instance.EnemyRoundList.Add("E03S69S69");//10
                DrawCard.Instance.EnemyRoundList.Add("E17");//11
                DrawCard.Instance.EnemyRoundList.Add("S75S00");//12
                DrawCard.Instance.EnemyRoundList.Add("S00S70");//13
                DrawCard.Instance.EnemyRoundList.Add("S00S70");//14
                DrawCard.Instance.EnemyRoundList.Add("S71");//15
                DrawCard.Instance.EnemyRoundList.Add("S72");//16
                DrawCard.Instance.EnemyRoundList.Add("S68S68S00");//17
                DrawCard.Instance.EnemyRoundList.Add("S00");//18
                break;
            case 11:
                PlayerPrefs.SetString("NewCard", "E24E28C01");
                DrawCard.Instance.GetNewCardList("NewCard");
                DrawCard.Instance.GetCardList("啦啦啦");
                DrawCard.Instance.GetEnemyCardList("EnemyLevel");
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

                break;
            case 12:
                PlayerPrefs.SetString("NewCard", "C03C17C29C13");
                DrawCard.Instance.GetNewCardList("NewCard");
                DrawCard.Instance.GetCardList("啦啦啦");
                DrawCard.Instance.GetEnemyCardList("EnemyLevel");
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
            case 13:
                AI.Instance.aiType = AIType.CardList;
                PlayerPrefs.SetString("EnemyLevel", "C04S71S74S08C02C04S08S10C05S70S15C03C23C05S16C03S16S08S04S01");
                DrawCard.Instance.GetCardList("啦啦啦");
                DrawCard.Instance.GetEnemyCardList("EnemyLevel");
                break;
            default:
                SetLevel(Random.Range(7, 14));
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
       // ESC.SetActive(true);
        ExplainLabel.SetActive(true);
        shalou.SetActive(true);
       // EndTurn.SetActive(true);
        gameObject.AddComponent<MagicCircleMananger>();
        gameObject.AddComponent<EnergyManager>();
        gameObject.AddComponent<DrawCard>();

        if (!LevelManager.Instance.IsOnline)
            SetLevel(level);
        else
        {
            DrawCard.Instance.GetCardList("啦啦啦");
        }
        
        //GameObject.Find("EndTurnTexture").AddComponent<Sand>();
        GameObject.Find("ShaLou").AddComponent<SandClock>();
        AttackManager.Instance.Restart();
        AuraManager.Instance.Restart();
        Move.Restart();
        CameraMoving.Instance.Move(0);
    }
    public void EndGame()
    {
        //        Debug.Log("EndGame");
        CameraMoving.Instance.EndGame();
        RayTest.Instance.EndGame();
        if(level==2)
        {
            gameObject.GetComponent<MagicCircleMananger>().Level2End();
        }
        Destroy(gameObject.GetComponent<MagicCircleMananger>());
        Destroy(gameObject.GetComponent<EnergyManager>());
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
        //levelLabel.text = "Level " + level.ToString();
    }

}
