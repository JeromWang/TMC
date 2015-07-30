using UnityEngine;
using System.Collections;

public class GuideText : MonoBehaviour
{
    public static GuideText Instance;
    public bool NextLevel=false;
    public bool ReLevel = false;
    public bool ReturnMenu = false;
    UILabel uiLabel;
    public int page;
    public GameObject backLight;
    public GameObject energyPlusLabel;
    public GameObject attentionLabel;
    GameObject SEWarning;
    GameObject SEGuiding;
    bool startChangeLabel = true;
    const int effectStart = 47;
    const int effectEnd = 53;
    string[] levelText = {
                             "",//0
                             "         魔法教学I",//1
                             "         魔法教学II",//2
                             "         魔法教学III",//3
                             "          随堂小测",//4
                             "         简单的试炼",//5
                             "        “打得不错”",//6
                             //没有7 后面自动减一
                             "   “我要开始认真啦！”",//8
                             "    “回去改卡组吧！”",//9
                             "   “留在这关陪我玩吧”",//10
                             " “你竟然能到这里？！”",//11
                             "      旗鼓相当的对手",//12
                             "     “对此我很抱歉”",//13
                             "      欢迎来到地狱！",//14
                             "         Good Luck!",//15
                             "     能到这关不错啊",//16
                             "   最后一关(后面没了)",//17
                             "      都说了后面没了",//18
                             "         后面真没了",//19
                             "",//
                         };
    string[] PageText = { "",//0
                            //Level1：
                            "向前滚动鼠标中键",//1
                            "法阵每回合可以增加两颗水晶，每两个水晶提供一点能量上限。现在，点击选择手牌，再次点击使用", //2
                            "[b]黄色的法阵[/b]就是卡牌[b]魔法飞弹[/b]所召唤出来的[b]攻击法阵[/b]，[b]红色数字1[/b]表示了魔法的[b]攻击力为1[/b]，[b]它会在下回合末发射[/b]" ,//3
                            "点击右侧沙漏可以结束回合",//4
                            "[b]攻击魔法碰撞时,双方的攻击力互相抵消。比如攻击力3的魔法与攻击力2的魔法碰撞后，会剩下一个攻击力1的魔法[/b]",//5
                            "点击生成的黄色法术",//6
                            "法阵上最多只能有12颗水晶,之后可以调整水晶的位置",//7
                            "只有具有[b]弹道自由[/b]的法术可以选择弹道，[b]它的攻击法阵与普通魔法的花纹不同[/b]。普通法术默认从中路进攻",//8
                            //Level2：
                            "上面有图案的牌，需要将法阵摆成相应图案才能发动，[b]按照手牌图案摆放水晶[/b]",//9
                            "面板下方Round后面的数字表示当前的回合数，后面的★表示还能移动几颗水晶;斜杠前方为当前能量，斜杠后方为能量上限",//10
                            "任意点击[b]三个蓝色指示[/b]中的一个，防御魔法会在对应位置生成",//11
                            "魔法上的绿色数字表示它的防御力",//12
                            "每次连线会消耗一点能量",//13
                            //Level 3
                            "两个阵型的卡牌使用后，魔法阵将[b]从卡牌左侧的形状变成右侧的形状[/b]",//14
                            "",//15
                            "你使用的【三重轰击】具有【准备1】的属性\n     [b]【准备X】[/b]：召唤时间额外增加X回合\n攻击魔上的[b]【等待】[/b]表示其离飞出去还有多久",//16
                            "",//17
                            //Level 1 补充
                            "用左键拖动两颗水晶到法阵上任意的点",//18
                            "点击[b]红色的弹道[/b]选择魔法的攻击路线，默认从中路攻击。",//19
                            "将鼠标移到攻击魔法上可以查看说明，以及弹道。",//20 
                            "攻击法阵[b]变为红色[/b]表示它会在这回合结束时飞出",//21
                            "向后滚动鼠标中键可以回到原视角",//22
                            //Level 2 补充
                            "【动荡阵列】具有【上下对称】的属性。\n[b]【上下对称】[/b]：摆出的图案与牌上图案上下对称均可使用。[b]第二张同名结界必须用未使用的对称图案发动。[/b]",//23
                            "【先知轰击】具有【回手】效果\n[b]【回手】[/b]：使用该牌后，下回合开始时该卡牌会回到手上",//24  
                            "结界卡使用后，其法阵只要不被破坏，就能[b]持续生效[/b]；如果被法阵破坏，该结界卡会在下一回合回到手上",//25
                            //Level3 补充
                            "上面有两个图案的牌，需要将法阵摆成[b]左侧图案[/b]才能发动",//26 
                            //Level 2补充2
                            "选择发动的魔法阵图案",//27
                            "魔法阵上有多余的连线，也可以发动魔法阵",//28
                            //Level1 补充2
                            "有的魔法可以从[b]左侧、右侧[/b]进攻",//29
                            //Level 2补充3
                            "对手使用的【持久壁垒】具有【刷新】效果\n[b]刷新[/b]：回合开始时该防御魔法恢复到原有的强度",//30
                            //Level 6
                            "将鼠标移到对手头像上上可以查看对手结界",//31
                            //Level 2 补充4
                            "[b]右键点击两颗水晶进行连线[/b]，以完成图案",//32
                            "魔法阵上可以有多余的连线,被破坏的结界会在下回合回到手上",//33
                            //
                            "[b]新的防御魔法会覆盖旧的防御魔法[/b]",//34
                            "用鼠标移到任何你感兴趣的物体上，以查看说明",//35
                            "对手使用的【法术偏转】有[b]削弱1[/b]效果：每次受到攻击减少1\n其他效果说明可以从【ESC】->【特效说明】中查看",//36
                            "这关你只有【索林的狙击弩】这一张牌",//37
                            "对着生成的魔法点击右键，会消耗",//
                            "",//
                            "",//
                            "",//41
                            "",//
                            // "",//
                            "",//
                            "",//
                            //"",//
                            //特效的说明
                            "[b][/b]：",//
                            "[b][/b]",//
                            "[b]削弱X[/b]：每次受到攻击减少X",//47
                            "[b]强化X[/b]：注入能量于该魔法中，以提高防御力，每点魔法增加X点防御力，右击该魔法触发",//
                            "[b]聚能X[/b]：将能量聚集于魔法中，以提升攻击力，每点魔法增加X点攻击力，右击该魔法触发",//
                            "[b]刷新[/b]：回合开始时该防御魔法恢复到原有的强度",//
                            "[b]回手[/b]：使用该牌后，下回合开始时该卡牌会回到手上",//
                            "[b]弹道自由[/b]：可以任意选择弹道",//
                            "[b]准备X[/b]：释放后需要额外等待X回合",//
                            "[b]左右对称[/b]：摆出的图案与牌上图案左右对称均可使用。当一个结界已经生效时，第二张同名结界必须用对称的图案发动",//
                            //"[b][/b]：",//
                        };
    string nextString = "\n \n [b]上一页[/b]               [b]下一页[/b]";
    // Use this for initialization
    void Start()
    {
        GuideText.Instance = this;
        SEGuiding = transform.FindChild("SE Guiding").gameObject;
        SEWarning = transform.FindChild("SE Warning").gameObject;
    }
    public void HideLabel()
    {
        //backLight = transform.FindChild("ExplainPanel/BackLight").gameObject;
        //energyPlusLabel = transform.FindChild("ExplainPanel/EnergyLabel/EnergyPlusLabel").gameObject;
        backLight.SetActive(false);
        energyPlusLabel.SetActive(false);
    }
    void FirstPage()
    {
        //教学关
        switch (LevelManager.Instance.level)
        {
            case 1: page = 1; break;
            case 2: page = 9; break;
            case 3: page = 26; break;
            default: page = 0; break;
        }
    }
    public void StartGame()
    {

        FirstPage();
        uiLabel = transform.FindChild("ExplainPanel/ExplainLabel").GetComponent<UILabel>();
        SetFontNormal();
        for (int i = effectStart; i <= effectEnd; i++)
        {
            PageText[i] += nextString;
        }
        Text(page);
    }
    // Update is called once per frame
    void Update()
    {
        //bool clickTwice = true;
	    if(NextLevel)
        {
           // clickTwice = !clickTwice;
            if (Input.GetMouseButtonDown(0))//&& clickTwice
            {
          //      clickTwice = true;
                NextLevel = false;
                LevelManager.Instance.EndGame();
                LevelManager.Instance.level += 1;
                PlayerPrefs.SetInt("Level", LevelManager.Instance.level);
                LevelManager.Instance.StartGame();
                page = 0;
            }
        }
        if(ReLevel)
        {
            if(Input.GetMouseButtonDown(0))
            {
                ReLevel = false;
                LevelManager.Instance.EndGame();
                LevelManager.Instance.StartGame();
                page = 0;
            }
        }
        if(ReturnMenu)
        {
            if (Input.GetMouseButtonDown(0))
            {
                ReturnMenu = false;
                CameraMoving.Instance.Move(-1);
                LevelManager.Instance.EndGame();
                page = 0;
            }
        }
    }
    public void StartTurn()
    {
        if(startChangeLabel==true)
        {
            if (EnergyManager.Instance.roundCount > 1)
                GuideText.Instance.ReturnText(1000);
        }
        else
        {
            startChangeLabel = true;
            page = 1000;
        }
    }
    public void ButtonDown(int ID)
    {
        switch(ID)
        {
            case 1:
                if(page>=effectStart&&page<=effectEnd)
                {
                    page++;
                }
                Text(page);
                break;
            case -1:
                if (page >= effectStart && page <= effectEnd)
                {
                    page--;
                }
                Text(page);
                break;
                case 10:EnergyManager.Instance.esc = false;break;
                case 11: //重新开始
                    LevelManager.Instance.EndGame();
                    LevelManager.Instance.StartGame();
                        break;
                case 12: //退出对战/认输
                if(LevelManager.Instance.IsOnline)
                {
                    Client.Instance.GiveUp();
                }
                else
                {
                    CameraMoving.Instance.Move(-1);
                    LevelManager.Instance.EndGame();
                }
                  break;
                case 13://特效说明
                    page = 48; Text(page); EnergyManager.Instance.esc = false; break;
                case 14://开始游戏 button 
                    LevelManager.Instance.StartGame();
                    break;
                case 15://Deck button 
                    LevelManager.Instance.ChangeDeck();
                    break;
                case 16://Quit button 
                    Application.Quit();
                    break;
                case 17://保存卡组
                    Deck.Instance.SaveButton();
                    break;
            case 18://登陆天梯
                    //Debug.Log("button 18");
                    Client.Instance.StartConnect();
                    break;
            case 19: //寻找对手
                   // Debug.Log("button 19");
                    Client.Instance.FindOpponent();
                break;
            case 20: //天梯返回
                Client.Instance.OffLine();
                break;
            case 21: 
                break;
        }
        
       
    }
    void AtkOutOfRange()
    {
        uiLabel.text = "场上的攻击魔法不能超过12个";
    }
    void Text (string page)
    {
        switch(page)
        {
            case "LevelText":
                int level=LevelManager.Instance.level;
                if(level<levelText.Length-1)
                {
                    uiLabel.text = "[b]          Level  " + level.ToString() + "\n\n";
                    uiLabel.text += levelText[level];
                }
                else
                {
                    uiLabel.text = "[b]          Level  " + level.ToString() + "\n\n";
                }
                //Debug.Log("leveltext");
                return;
            case "WaitRoundEnd":
                uiLabel.text = "[b]这个魔法会在本回合末发射[/b]";
                Invoke("ReturnText", 3f);
                SEGuiding.audio.Play();
                return;
            case "WaitOneRound":
                uiLabel.text = "[b]这个魔法会在下回合末发射[/b]";
                Invoke("ReturnText", 3f);
                SEGuiding.audio.Play();
                return;
            case "DefaultMiddle":
                uiLabel.text = "没有弹道自由的魔法默认从中路攻击";
                SEGuiding.audio.Play();
                return;
            case "AtkOutOfRange":
                uiLabel.text = "场上的攻击魔法不能超过12个";
                Invoke("AtkOutOfRange", 0.3f);
                SEGuiding.audio.Play();
                return;
            case "HaveCrystal":
                uiLabel.text = "你还没向法阵中放水晶";
                SEGuiding.audio.Play();
                return;
            case "NoCard":
                uiLabel.text = "你没有牌了，受到伤害";
                SEGuiding.audio.Play();
                startChangeLabel = false;
                return;
            case "CrystalMoved":
                uiLabel.text = "本回合你已移动过水晶了";
                SEGuiding.audio.Play();
                return;
            case "CannotPlaceInLink":
                uiLabel.text = "不可以在连线上放水晶，重复连线可以取消连线";
                SEGuiding.audio.Play();
                return;
            case "NoJumpLink": 
                uiLabel.text = "不可以跨过水晶连线";
                SEWarning.audio.Play();
                //Invoke("GuideText.Instance.ReturnText(1000)", 4f);
                return;
            case "LinkNeedEnergy":
                uiLabel.text = "连线需要消耗一点能量，你没有能量了";
                SEWarning.audio.Play();
                return;
            case"LinkedCrystalCannotMove":
                uiLabel.text = "连线的水晶不能移动，重复连线可以取消连线";
                SEWarning.audio.Play();
                break;
            case "AuraManager": 
                LabeltextAsAura(AuraManager.Instance.auraText, AuraManager.Instance.GetAuraCount()); 
                this.page = 1000; 
                return;
            case "EnemyAura":
                if (AuraManager.Instance.GetEnemyAuraCount() == 0)
                {
                    return;
                }
                LabeltextAsAura(AuraManager.Instance.enemyAuraText, AuraManager.Instance.GetEnemyAuraCount()); 
                return;
            case "NeedEnergy": uiLabel.text = "能量不足！"; SEGuiding.audio.Play(); break;
            case "NeedPattern": uiLabel.text = "没有合适的魔法阵！"; SEGuiding.audio.Play(); break;
            case "NeedSymmetryPattern": 
                uiLabel.text = "第二张需要另一个对称的魔法阵！"; 
                SEGuiding.audio.Play();
                break;
            //case "NeedEnergy": uiLabel.text = ""; SEGuiding.audio.Play(); break;
        }
    }
    void  SetFontSmall()
    {
        uiLabel.fontSize = 35;
    }
    void SetFontNormal()
    {
        uiLabel.fontSize = 45;
    }
    void LabeltextAsAura(string auraText,int auraNum)
    {
        uiLabel.text = auraText;
        if (auraNum >= 5)
        {
            SetFontSmall();
        }
        else
        {
            SetFontNormal();
        }
        return;
    }
    void Text(int page)
    {
        switch (page)            
        {
            case 100: uiLabel.text = "Both Lose!\n\n\n点击重新开始本关"; return;
            case 101: uiLabel.text = "You Lose!\n\n\n点击重新开始本关"; return;
            case 102:uiLabel.text ="You Win!\n\n\n点击进入下一关";return;
            case 103: uiLabel.text = "Both Lose!!!"; return;
            case 104: uiLabel.text = "You Lose!!!"; return;
            case 105:uiLabel.text ="You Win!!!";return;
            case 106: uiLabel.text = "You Win!\n\n恭喜你获得新卡，在[b]卡组调整[/b]中可以修改卡组"; return;
            case 1000:
            case 1001: 
                LabeltextAsAura(AuraManager.Instance.auraText, AuraManager.Instance.GetAuraCount());
                return;
        }
        //Debug.Log(PageText.Length.ToString());
        //Debug.Log(page.ToString());
        if(page<PageText.Length && uiLabel!=null)
        {
            uiLabel.text = PageText[page];
        }
    }
    public void ReturnText(string p)
    {
        //Debug.Log("ReturnText");
        if (uiLabel == null)
        {
            uiLabel = transform.FindChild("ExplainPanel/ExplainLabel").GetComponent<UILabel>();
            if (uiLabel == null)
                return;
        }
        Text(p);
        backLight.SetActive(true);
        Invoke("CloseBacklight", 2f);
    }
    public void ReturnText(int p)
    {
        page = p;
        Text(page);
        //Debug.Log(p.ToString());
        if(p!=1000)
        {
            if (page < PageText.Length && page > 0)//指导提示音
                SEGuiding.audio.Play();
            backLight.SetActive(true);
            Invoke("CloseBacklight", 2f);
        }
    }
    public void ReturnText()
    {
        Text(page);
    }
    void CloseBacklight()
    {
        backLight.SetActive(false);
    }
    public void EnergyPlusLabelHint()
    {
        energyPlusLabel.SetActive(true);
        Invoke("EnergyPlusLabelHide", 2f);
    }
    void EnergyPlusLabelHide()
    {
        energyPlusLabel.SetActive(false);
    }
    public void AttentionLabelHint()
    {
        //Debug.Log("AttentionLabel");
        attentionLabel.SetActive(true);
        Invoke("AttentionLabelHide", 15f);
    }
    void AttentionLabelHide()
    {
        attentionLabel.SetActive(false);
    }
    public void GuideLevel(int level,int returnText,string key)
    {
        if (LevelManager.Instance.level == level && LevelManager.Instance.key.Contains(key))
        {
            ReturnText(returnText);
            LevelManager.Instance.key.Remove(key);
        }
    }
    public void GuideLevel(int level,int round, int returnText, string key)
    {
        if (LevelManager.Instance.level == level && LevelManager.Instance.key.Contains(key)&&EnergyManager.Instance.roundCount==round)
        {
            ReturnText(returnText);
            LevelManager.Instance.key.Remove(key);
        }
    }
    public  void GuideLevel(int level,int returnText,string key,string requireID,string cardID)
    {
        if (LevelManager.Instance.level == level && LevelManager.Instance.key.Contains(key) && requireID == cardID)
        {
            GuideText.Instance.ReturnText(returnText);
            LevelManager.Instance.key.Remove(key);
        }
    }
}
