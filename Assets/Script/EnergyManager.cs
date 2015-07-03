using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Operation 
{
    Cast,ChangeTrajectory,Cohere,Entrench
}

public class EnergyManager : MonoBehaviour
{
    public static EnergyManager Instance;
    public GameObject StartMenu;
    public int totalEnergy;//总水晶能量
    public bool unmatchedEnergy;//单独水晶能量
    public int accessibleEnergy;//可用能量
    public int roundCount;//回合数
    public int accessibleCrystal;//可用水晶数
    public int cameraFlag;//相机指示
    public bool myTurnsEnd = false;
    public bool enemyTurnsEnd = false;
    public int cardUsedTurn = 0;//本回合用过的非结界卡数
    bool testMode = false;
    GameObject handZone;
    GameObject shaLou;
 //   GameObject ESC;
    UILabel EnergyLabel;
    UILabel RoundLabel;
    Hero hero, enemyHero;
    public List<string> enemyOperationID;
    public List<Operation> operation;
    public List<int> enemyTrajectoryID;
    public int enemyOperationIndex;//为了确定当前是对方用的第几张牌

    public bool esc = false;
    string[] crystalString = { "      ", " ★  ", " ★★" };

    public delegate void ChangeEnergyEvent();
    public event ChangeEnergyEvent ChangeEnergy;
    public delegate void MyCardUsedEvent();
    public event MyCardUsedEvent MyCardUsed;
    public delegate void EnemyCardUsedEvent();
    public event EnemyCardUsedEvent EnemyCardUsed;
    public void MyCardUse()
    {
        MyCardUsed();
    }
    public void EnemyCardUse()
    {
        EnemyCardUsed();
    }

    //public delegate int EndGameEvent();
    public delegate void StartGameEvent();
    public delegate void StartTurnEvent();
    public delegate void StartTurnAuraEvent();
    public delegate void MyTurnEndEvent();
    //    public delegate void OpponentTurnEndEvent();
    public delegate void TurnsEndEvent();
    public delegate void FireGoEvent();

    //public event EndGameEvent EndGame;
    public event StartGameEvent StartGame;
    public event StartTurnEvent StartTurn;
    public event MyTurnEndEvent MyTurnEnd;
    //// public event OpponentTurnEndEvent OpponentTurnEnd();
    public event TurnsEndEvent TurnsEnd;
    public event FireGoEvent FireGo;

    public float TurnsEndTime = 0;
    //public float FireGoTime = 0;
    bool enemyCardShowing = false;

    bool win = false;//胜负指示位
    bool hidePatternChose = false;//教学关显示选择图案卡牌
    void Awake()
    {
       // Debug.Log("Energy create");
        EnergyManager.Instance = this;
        hero = GameObject.Find("Hero").GetComponent<Hero>();
        enemyHero = GameObject.Find("EnemyHero").GetComponent<Hero>();

    }
    // Use this for initialization
    void Start()
    {
        totalEnergy = 0;
        accessibleEnergy = 0;
        unmatchedEnergy = false;
        roundCount = 0;
        accessibleCrystal = 2;
        enemyOperationID=new List<string>();
        enemyTrajectoryID = new List<int>();
        operation = new List<Operation>();
        StartMenu = GameObject.Find("StartPanel");
        handZone = transform.FindChild("HandZone").gameObject;
        shaLou = transform.FindChild("ShaLou").gameObject;
        EnergyLabel = GameObject.Find("EnergyLabel").GetComponent<UILabel>();
        RoundLabel = GameObject.Find("RoundLabel").GetComponent<UILabel>();
      //  ESC = LevelManager.Instance.ESC;
        CameraMoving.Instance.CameraMovingEvent();
        GuideText.Instance.HideLabel();
        Invoke("StartParticle", 3f);
        Invoke("GameStart", 5.5f);
    }
    void StartParticle()
    {
        if(StartMenu!=null)
            StartMenu.SetActive(false);
        GameObject startParticle = (GameObject)Resources.Load("ParticleSystem/virtical 15");
        GameObject i = (GameObject)Instantiate(startParticle, new Vector3(1000f, 0.1f, 850f), new Quaternion(1, 0, 0, 0));
        i.transform.localEulerAngles = new Vector3(0, 0, 0);
        Destroy(i, 5f);
        i = (GameObject)Instantiate(startParticle, new Vector3(1000f, 0.1f, 881f), new Quaternion(1, 0, 0, 0));
        i.transform.localEulerAngles = new Vector3(0, 0, 0);
        Destroy(i, 5f);
    }
    void GameStart()
    {
        if (LevelManager.Instance.level > 3 || LevelManager.Instance.IsOnline|| LevelManager.Instance.level==0)
            StartGame();
        else
            CameraMoving.Instance.StartGame();
        GuideText.Instance.StartGame();

        roundCount++;
        StartTurn();
    }


    void Update()
    {
        //if (Time.frameCount % 50 == 0)
        //{
        //    System.GC.Collect();
        //}  
        if (Time.frameCount % 10 == 0)
        {
            EnergyLabel.text = accessibleEnergy.ToString() + "/" + totalEnergy.ToString();
            RoundLabel.text = "Round " + roundCount.ToString();
            if (accessibleCrystal < 3)
                RoundLabel.text += crystalString[accessibleCrystal];
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            esc = true;
        }
        if (esc)
        {
            if(LevelManager.Instance.IsOnline)
            {
                LevelManager.Instance.ESC.transform.FindChild("QuitButton/label").GetComponent<UILabel>().text = "认输";
            }
            else
            {
                LevelManager.Instance.ESC.transform.FindChild("QuitButton/label").GetComponent<UILabel>().text = "退出对战";
            }
            LevelManager.Instance.ESC.SetActive(true);
        }
        else
        {
            LevelManager.Instance.ESC.SetActive(false);
        }
        if(Input.GetMouseButtonDown(0)&&hidePatternChose)//教学关 PatternChose
        {
            hidePatternChose = false;
            LevelManager.Instance.pattern1.SetActive(false);
            LevelManager.Instance.pattern2.SetActive(false);
        }
    }
    public void PlusEnergy(int num)
    {
        accessibleEnergy += num;
        ChangeEnergy();

        //关卡判断
        if (LevelManager.Instance.level == 1)
        {
            GuideText.Instance.EnergyPlusLabelHint();
        }
        if (LevelManager.Instance.level == 1 && LevelManager.Instance.key.Contains("AddEnergy"))
        {
            GuideText.Instance.ReturnText(2);
            LevelManager.Instance.key.Remove("AddEnergy");
        }
        if (LevelManager.Instance.level == 1 && roundCount==2 && LevelManager.Instance.key.Contains("MouseDown"))
        {
            GuideText.Instance.ReturnText(22);
            LevelManager.Instance.key.Remove("MouseDown");
        }
    }
    public bool MinusEnergy(int num)
    {
        //关卡判断
        if (LevelManager.Instance.level == 2 && LevelManager.Instance.key.Contains("CostEnergy"))
        {
            GuideText.Instance.ReturnText(13);
            LevelManager.Instance.key.Remove("CostEnergy");
        }
        if (num <= accessibleEnergy)
        {
            accessibleEnergy -= num;
            ChangeEnergy();
            return true;
        }
        else
        {
            //Debug.Log("MinusEnergy error, num > accessibleEnergy");
            return false;
        }

    }

    void OnGUI()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            testMode = true;
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            testMode = false;
        }
        if (testMode)
        {
            if (GUILayout.Button("抽牌"))
            {
                DrawCard.Instance.draw();
            }
            //GUI.Label(new Rect(20, 210, 120, 20), "AccessibleCrystal:");
            //GUI.Label(new Rect(150, 210, 50, 20), accessibleCrystal.ToString());

            if (GUILayout.Button("Moving"))
            {
                //CameraMoving.Instance.MoveTo();
            }
            if (GUILayout.Button("MAX"))
            {
                totalEnergy = 20;
                accessibleEnergy = 20;
                ChangeEnergy();
            }

            if (GUILayout.Button("Max Crystal"))
            {
                accessibleCrystal = 20;
            }
            if (GUILayout.Button("front"))
            {
                CameraMoving.Instance.MoveAdd(new Vector3(0, 5f, 10f));
            }
            if (GUILayout.Button("back"))
            {
                CameraMoving.Instance.MoveAdd(new Vector3(0, -5f, -10f));
            }
            if (GUILayout.Button("Win"))
            {
                win = true;
                WinLose();
            }
            //if (GUILayout.Button("Show1"))
            //{
            //    explainPanel.SetActive(false);
            //    handZone.SetActive(false);
            //    shaLou.SetActive(false);
            //    CameraMoving.Instance.MoveWithSth(AttackManager.Instance.attackMagicList[0].transform,new Vector3(20f,1f,4f));
            //}
            //if (GUILayout.Button("Show2"))
            //{

            //}
            //if (GUILayout.Button("Return"))
            //{
            //    explainPanel.SetActive(true);
            //    handZone.SetActive(true);
            //    shaLou.SetActive(true);
            //    CameraMoving.Instance.Move(1);
            //    CameraMoving.Instance.isMoveWithSth = false;
            //}

        }
    }
    IEnumerator OfflineEnd()
    {
        if (AI.Instance.aiType == AIType.CardList)
        {
            StartCoroutine(DrawCard.Instance.EnemyShowCard());
            yield return new WaitForSeconds(2f);
            enemyCardShowing = false;
        }
        if (AI.Instance.aiType == AIType.RoundList)
        {
            DrawCard.Instance.GetEnemyRoundList(roundCount);
            StartCoroutine(Online_End());
        }
    }
    public IEnumerator End()
    {
        TurnsEndTime = 0f;
        myTurnsEnd = true;
        MyTurnEnd();
        TurnsEnd();
        yield return new WaitForSeconds(1f);
        enemyCardShowing = true;
        if(LevelManager.Instance.IsOnline)//联机
        {
            StartCoroutine(Online_End());//开了一个线程但并不能阻止本函数继续运行
        }
        else//单机
        {
            StartCoroutine(OfflineEnd());
        }
        for (; enemyCardShowing; )
        {
            yield return new WaitForSeconds(0.1f);
        }
        TurnsEndTime += 3;
        yield return new WaitForSeconds(TurnsEndTime);

        for (int i = DrawCard.Instance.EnemyList.childCount - 1; i >= 0; i--)
        {
            if(DrawCard.Instance.EnemyList.GetChild(i).GetComponent<Card>().typeText=="结界")
            {
                DrawCard.Instance.EnemyList.GetChild(i).parent = DrawCard.Instance.EnemyAuraList;
                continue;
            }
            CardMoving cardScript = DrawCard.Instance.EnemyList.GetChild(i).GetComponent<CardMoving>();
            cardScript.BeDestroy();
            Destroy(DrawCard.Instance.EnemyList.GetChild(i).gameObject);
        }
        if(AttackManager.Instance.SetFireTime()!=0)
            FireGo();
        StartCoroutine(startTurn());
    }
    IEnumerator Online_End()
    {
        //Debug.Log("count " + enemyOperationID.Count);
        for (enemyOperationIndex = 0; enemyOperationIndex < enemyOperationID.Count; enemyOperationIndex++)
        {
            //Debug.Log("第" + enemyOperationIndex + "个操作");
            if (operation[enemyOperationIndex] == Operation.Cast)
            {
               // Debug.Log("isuse" + operation[enemyOperationIndex]);
                StartCoroutine(DrawCard.Instance.EnemyShowCard());
                yield return new WaitForSeconds(2f);
            }
            else if (operation[enemyOperationIndex] == Operation.ChangeTrajectory)
            {
                foreach (AttackMagic a in AttackManager.Instance.enemyAttackMagicList)
                {
                    if (a.enemyAttackID.ToString() == enemyOperationID[enemyOperationIndex])
                    {
                        a.trajectory = enemyTrajectoryID[enemyOperationIndex];
                    }
                }
            }
            else if (operation[enemyOperationIndex] == Operation.Cohere)
            {
                foreach (AttackMagic a in AttackManager.Instance.enemyAttackMagicList)
                {
                    if (a.enemyAttackID.ToString() == enemyOperationID[enemyOperationIndex])
                    {
                        a.CohereMagic();
                    }
                }
            }
            else if (operation[enemyOperationIndex] == Operation.Entrench)
            {
                foreach (DefenseMagic a in ShieldManager.Instance.defenseMagicList)
                {
                    if (a.enemyDefenseID.ToString() == enemyOperationID[enemyOperationIndex])
                    {
                        a.EntrenchMagic();
                    }
                }
            }
        }
        enemyCardShowing = false;
    }
    void changeEnergy()
    {
        ChangeEnergy();
    }
    public bool WinLose()
    {
        if (hero.health <= 0 && enemyHero.health <= 0)//Both Lose
        {
            if(LevelManager.Instance.IsOnline)
            {
                Client.Instance.OnDraw();
                GuideText.Instance.ReturnText(103);
                GuideText.Instance.ReturnMenu = true;
                return true;
            }
            GuideText.Instance.ReturnText(100);
            GuideText.Instance.ReLevel = true;
            return true;
        }
        if (hero.health <= 0 && enemyHero.health > 0)//Lose
        {
            if (LevelManager.Instance.IsOnline)
            {
                Client.Instance.OnWin();
                GuideText.Instance.ReturnText(104);
                GuideText.Instance.ReturnMenu = true;
                return true;
            }
            GuideText.Instance.ReturnText(101);
            GuideText.Instance.ReLevel = true;
            return true;
        }
        if ((hero.health > 0 && enemyHero.health <= 0)||win)//Win
        {
            if (LevelManager.Instance.IsOnline)
            {
                Client.Instance.OnLose();
                GuideText.Instance.ReturnText(105);
                GuideText.Instance.ReturnMenu = true;
                return true;
            }

            if(LevelManager.Instance.level<=12 && LevelManager.Instance.level>=4)
            {
                GuideText.Instance.ReturnText(106);
                StartCoroutine(DrawCard.Instance.ShowNewCard());
            }
            else
            {
                GuideText.Instance.ReturnText(102);
            }
            GuideText.Instance.NextLevel = true;
            return true;
        }
        return false;
    }
    
    
    IEnumerator startTurn()
    {
        for (; !AttackManager.Instance.AllFireDestroy(); )
        {
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(1.2f);
        operation.Clear();
        enemyOperationID.Clear();
        enemyOperationIndex = 0;
        enemyTrajectoryID.Clear();
        myTurnsEnd = false;
        enemyTurnsEnd = false;
        cardUsedTurn = 0;
        if (!WinLose())//失败直接退出
        {
            roundCount++;
            StartTurn();
            accessibleCrystal = 2;
            accessibleEnergy = totalEnergy;
            ChangeEnergy();
            myTurnsEnd = false;

            //教学关卡判断
            Guide_startTurn();
            yield return new WaitForSeconds(0.5f);
            changeEnergy();
        }
        LevelManager.Instance.RoundLabelShow();
    }
    public bool CrystalSee()
    {
        if (totalEnergy >= 6)
        {
            CrystalShow.Instance.ShowHide(false);
            //关卡判断
            if (LevelManager.Instance.level == 1 && LevelManager.Instance.key.Contains("FullEnergy"))
            {
                GuideText.Instance.ReturnText(7);
                LevelManager.Instance.key.Remove("FullEnergy");
            }
            return false;
        }
        else
        {
            CrystalShow.Instance.ShowHide(true);
            return true;
        }
    }
    #region 教学关相关
    void Guide_startTurn()
    {
        if (LevelManager.Instance.level == 1 && LevelManager.Instance.key.Contains("Crash") && roundCount == 3)
        {
            GuideText.Instance.ReturnText(5);
            LevelManager.Instance.key.Remove("Crash");
        }
        if (LevelManager.Instance.level == 1 && roundCount == 2 && LevelManager.Instance.key.Contains("TurnRed"))
        {
            GuideText.Instance.ReturnText(21);
            LevelManager.Instance.key.Remove("TurnRed");
        }
        if (LevelManager.Instance.level == 1 && roundCount == 4 && LevelManager.Instance.key.Contains("LeftRightAttack"))
        {
            GuideText.Instance.ReturnText(29);
            LevelManager.Instance.key.Remove("LeftRightAttack");
        }
        if (LevelManager.Instance.level == 1 && LevelManager.Instance.key.Contains("OnlyFreedom") && roundCount == 5)
        {
            GuideText.Instance.ReturnText(8);
            LevelManager.Instance.key.Remove("OnlyFreedom");
        }
        if (LevelManager.Instance.level == 2 && LevelManager.Instance.key.Contains("ExplainLabel") && roundCount == 2)
        {
            GuideText.Instance.ReturnText(10);
            LevelManager.Instance.key.Remove("ExplainLabel");
        }
        if (LevelManager.Instance.level == 2 && LevelManager.Instance.key.Contains("TopDownAura") && roundCount >= 3)
        {
            GuideText.Instance.ReturnText(23);
            LevelManager.Instance.key.Remove("TopDownAura");
            LevelManager.Instance.pattern1.SetActive(true);
            LevelManager.Instance.pattern2.SetActive(true);
            hidePatternChose = true;
        }
        if (LevelManager.Instance.level == 2 && LevelManager.Instance.key.Contains("CardBack") && roundCount == 5)
        {
            GuideText.Instance.ReturnText(24);
            LevelManager.Instance.key.Remove("CardBack");
        }
        if (LevelManager.Instance.level == 2 && LevelManager.Instance.key.Contains("Refresh") && roundCount == 8)
        {
            GuideText.Instance.ReturnText(30);
            LevelManager.Instance.key.Remove("Refresh");
        }
        if (LevelManager.Instance.level == 3 && roundCount == 1)
        {
            GuideText.Instance.ReturnText(26);
        }
        if (LevelManager.Instance.level == 6 && (roundCount == 4 || roundCount ==5 || roundCount == 6))
        {
            GuideText.Instance.ReturnText(31);
        }
        //if (LevelManager.Instance.level == 3 && LevelManager.Instance.key.Contains("WaitToFire") && roundCount == 4)
        //{
        //    GuideText.Instance.ReturnText(15);
        //    LevelManager.Instance.key.Remove("WaitToFire");
        //}
    }
    #endregion
}
