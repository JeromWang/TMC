using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public enum Accessible//卡牌是否可以使用
{
    OK,NeedEnergy,NeedPattern,NeedSymmetryPattern
}
public enum CardType
{
    error,attack,defence,aura,heal
}
public class Card : MonoBehaviour
{
    public bool isHeros;
    public string ID;
    public string name;
    public string effectText;
    public string typeText;
    public string explainText;
    public int[][] Pattern_1 = new int[0][];//要求的线 x1,y1,x2,y2  = new int[12, 4];
    public int[][] Pattern_2 = new int[0][];//另一种选择
    public int PatternNumber = 0;//Pattern的种类
    public int PatternSize = 0;//Pattern的线数量
    public int PatternUsed = 0;//使用的Pattern的种类
    List<int[]> Pattern_consumption_1 = new List<int[]>();//消耗的线
    List<int[]> Pattern_consumption_2 = new List<int[]>();//消耗的线
    List<int[]> Pattern_reconstitution_1 = new List<int[]>();//新生成的线
    List<int[]> Pattern_reconstitution_2 = new List<int[]>();//新生成的线
    public int cost = 0;//消耗水晶
    public int costOld = 0;//原本消耗的能量


    public int number;//生成法球个数
    int tempNumber;//目前还需生成的防御魔法个数
    public int magicValue;//数值
    Dictionary<string, int> Effect = new Dictionary<string, int>();//效果字典 string：名字，int：数值

    //public GameObject shield;
    //public GameObject AttackCircle;
    //public GameObject Heal;
    Vector3 HealPos = new Vector3(1000, 0.2f, 850);
    Vector3 EnermyHealPos = new Vector3(1000f, 0.2f, 880f);
    Vector3 ShieldPos;//new Vector3(1000, 1.91f, 850);
    Quaternion qua = new Quaternion(-1, 0, 0, 0);
    Quaternion quaAtkCircle = new Quaternion(0.7f, 0, 0, 0.7f);

    UILabel NameLabel;
    UILabel TypeLabel;
    static string[] TypeString = { "", "攻击", "防御", "治疗","结界" };
    UILabel EffectLabel;

    public CardMoving cardMoving;
    bool used = false;//已使用 Return效果用

    void Awake()
    {
        NameLabel = transform.Find("Name").GetComponent<UILabel>();
        TypeLabel = transform.Find("Type").GetComponent<UILabel>();
        EffectLabel = transform.Find("Effect").GetComponent<UILabel>();
    }
    // Use this for initialization
    void Start()
    {
        cardMoving = transform.GetComponent<CardMoving>();
        XmlInitial();
        UpdateText();
        
        if(cardMoving!=null)
            cardMoving.BacklightShowHide();
    }
    void UpdateText()
    {
        NameLabel.text = name;
        TypeLabel.text = typeText;
        UpdateEffectText();
    }
    int UpdateCost()
    {
        if (CaculateCost() < 0)
            return 0;
        return CaculateCost();
    }
    int CaculateCost()
    {
        if (AuraManager.Instance.costMinus > 0 && isHeros)
        {
            return costOld - AuraManager.Instance.costMinus;
        }
        if (AuraManager.Instance.enemyCostMinus > 0 && !isHeros)
        {
            return costOld - AuraManager.Instance.enemyCostMinus;
        }
         return  costOld;
    }
    public void UpdateEffectText() //更新effect text 效果说明
    {
        //所有的特性+数值
        effectText = "";
        string s;
        foreach (KeyValuePair<string, int> item in Effect)
        {
            s = TextChange.Instance.Chinese(item.Key, item.Value);
            if(s!="")
            {
                effectText += s + "\n";
            }
        }
        cost = UpdateCost();

        if (ID[0] == 'S' || ID[0] == 'C' && cost > 0)
        {
            effectText += (cost == costOld ? "能量 " + cost.ToString() : ("[ff0000]" + "费用 " + cost.ToString() + "[-]")) + "\n";
        }
        if(ID[0]=='E')
        {
            effectText += explainText;
        }
        else
        {
            effectText += typeText + ":" + magicValue.ToString() + (number > 1 ? "*" + number.ToString() : "");
        }
        effectText = "[000000]" + effectText + "[-]";
        EffectLabel.text = effectText;
        //effectText += "PatternNumber " + PatternNumber.ToString();

    }
    void XmlInitial()
    {
        foreach (XmlNode node in XmlReader.Instance.root.ChildNodes)
        {
            if (ID[0] == 'S' && node.Name == "Spell")
            {
                #region Spell
                foreach (XmlNode subNode in node.ChildNodes)
                {
                    XmlElement xe = (XmlElement)subNode;
                    if (xe.GetAttribute("ID").ToString() == ID)
                    {
                        name = xe["CardName"].GetAttribute("value");
                        cost = int.Parse(xe["Cost"].GetAttribute("value"));
                        costOld = cost;
                        number = int.Parse(xe["Quantity"].GetAttribute("value"));
                        magicValue = int.Parse(xe["Value"].GetAttribute("value"));
                        typeText=TypeString[int.Parse(xe["CardType"].GetAttribute("value"))];
                        foreach (XmlElement feature in xe["Feature"].ChildNodes)
                        {
                            Effect.Add(feature.Name, int.Parse(feature.GetAttribute("value")));
                        }
                        break;
                    }
                }
                #endregion
            }
            else if (ID[0] == 'E' && node.Name == "Aura")
            {
                #region 结界
                foreach (XmlNode subNode in node.ChildNodes)
                {
                    XmlElement xe = (XmlElement)subNode;
                    if (xe.GetAttribute("ID").ToString() == ID)
                    {
                        name = xe["CardName"].GetAttribute("value");
                        typeText = TypeString[int.Parse(xe["CardType"].GetAttribute("value"))];
                        explainText = xe["Explain"].GetAttribute("value");
                        foreach (XmlElement feature in xe["Feature"].ChildNodes)
                        {
                            Effect.Add(feature.Name, int.Parse(feature.GetAttribute("value")));
                        }
                        foreach (XmlElement subsubNode in subNode.ChildNodes)
                        {
                            if (subsubNode.Name == "Value")
                            {
                                magicValue = int.Parse(subsubNode.GetAttribute("value"));
                            }
                        }
                        //把线加入Pattern数组中
                        PatternNumber = int.Parse(xe["PatternNumber"].GetAttribute("value"));
                        PatternSize = int.Parse(xe["Require"].GetAttribute("value"));
                        int i = 0;
                        {
                            Pattern_1 = new int[PatternSize][];
                            foreach (XmlElement pattern in xe["Pattern_1"])
                            {
                                Pattern_1[i] = GetLine(pattern.GetAttribute("value"));
                                i++;
                            }
                        }

                        if (PatternNumber > 1)
                        {
                            i = 0;
                            Pattern_2 = new int[PatternSize][];
                            foreach (XmlElement pattern in xe["Pattern_2"])
                            {
                                Pattern_2[i] = GetLine(pattern.GetAttribute("value"));
                                i++;
                            }
                        }
                    }
                }

                (transform.FindChild("Texture").GetComponent<UITexture>()).mainTexture = (Texture)Resources.Load("CardTexture/" + ID);
                #endregion
            }
            else if (ID[0] == 'C' && node.Name == "Circle")
            {
                #region Circle
                foreach (XmlNode subNode in node.ChildNodes)
                {
                    XmlElement xe = (XmlElement)subNode;
                    if (xe.GetAttribute("ID").ToString() == ID)
                    {
                        name = xe["CardName"].GetAttribute("value");
                        number = int.Parse(xe["Quantity"].GetAttribute("value"));
                        magicValue = int.Parse(xe["Value"].GetAttribute("value"));
                        if (xe["Cost"]!=null)
                        {
                            cost = int.Parse(xe["Cost"].GetAttribute("value"));
                            costOld = cost;
                           // Debug.Log(cost.ToString());
                        }
                            
                        switch (int.Parse(xe["CardType"].GetAttribute("value")))
                        {
                            case 1: typeText = "攻击"; break;
                            case 2: typeText = "防御"; break;
                            case 3: typeText = "治疗"; break;
                            case 4: typeText = "结界"; break;
                        }
                        foreach (XmlElement feature in xe["Feature"].ChildNodes)
                        {
                            Effect.Add(feature.Name, int.Parse(feature.GetAttribute("value")));
                        }

                        //把线加入Pattern数组中
                        PatternNumber = int.Parse(xe["PatternNumber"].GetAttribute("value"));
                        PatternSize = int.Parse(xe["Require"].GetAttribute("value"));
                        int i = 0;
                        {
                            Pattern_1 = new int[PatternSize][];
                            foreach (XmlElement pattern in xe["Pattern_1"])
                            {
                                Pattern_1[i] = GetLine(pattern.GetAttribute("value"));
                                i++;
                            }
                            foreach (XmlElement pattern in xe["Pattern_consumption_1"])
                            {
                                Pattern_consumption_1.Add(GetLine(pattern.GetAttribute("value")));
                            }
                            if (xe["Pattern_reconstitution_1"] != null)
                            {
                                foreach (XmlElement pattern in xe["Pattern_reconstitution_1"])
                                {
                                    Pattern_reconstitution_1.Add(GetLine(pattern.GetAttribute("value")));
                                }
                            }
                        }

                        if (PatternNumber > 1)
                        {
                            i = 0;
                            Pattern_2 = new int[PatternSize][];
                            foreach (XmlElement pattern in xe["Pattern_2"])
                            {
                                Pattern_2[i] = GetLine(pattern.GetAttribute("value"));
                                i++;
                            }
                            foreach (XmlElement pattern in xe["Pattern_consumption_2"])
                            {
                                Pattern_consumption_2.Add(GetLine(pattern.GetAttribute("value")));
                            }
                            if (xe["Pattern_reconstitution_2"] != null)
                            {
                                foreach (XmlElement pattern in xe["Pattern_reconstitution_2"])
                                {
                                    Pattern_reconstitution_2.Add(GetLine(pattern.GetAttribute("value")));
                                }
                            }
                        }

                    }
                }
                #endregion
                (transform.FindChild("Texture").GetComponent<UITexture>()).mainTexture = (Texture)Resources.Load("CardTexture/"+ID);
            }
        }

    }
    public CardType GetCardType()
    {
        if (typeText == "攻击")
            return CardType.attack;
        if (typeText == "防御")
            return CardType.defence;
        if (typeText == "结界")
            return CardType.aura;
        if (typeText == "治疗")
            return CardType.heal;
        return CardType.error;
    }
    public void Return()
    {
        if (used)
        {
            cardMoving = gameObject.AddComponent<CardMoving>();
            gameObject.SetActive(true);
            transform.parent = GameObject.Find("HandZone").transform;
            DrawCard.Instance.clearUpHand(1);
            UpdateEffectText();
            used = false;
            if(Effect.ContainsKey("ReturnOnce"))
            {
                RemoveEffect("ReturnOnce");
            }
            EnergyManager.Instance.StartTurn -= this.Return;
            // cardMoving
        }
    }
    int[] GetLine(string pattern)
    {
        int[] line = new int[4];
        string[] strline = pattern.Split(',');
        for (int i = 0; i < strline.Length; i++)
        {
            line[i] = int.Parse(strline[i]);
        }
        return line;

    }
    Accessible AuraAccessible()
    {
        int type = isHeros?
            AuraManager.Instance.GetCardPatternUsed(ID):AuraManager.Instance.EGetCardPatternUsed(ID);//0:没有使用的样式
        if (PatternNumber > 1)
        {
            if (patternTrue(Pattern_2) && type != 2)
            {
                PatternUsed = 2;
                return Accessible.OK;
            }
            if (patternTrue(Pattern_1) && type != 1)
            {
                PatternUsed = 1;
                return Accessible.OK;
            }
            if (type == 0)
                return Accessible.NeedPattern;

            return Accessible.NeedSymmetryPattern;
        }
        if (patternTrue(Pattern_1) && type != 1)
        {
            PatternUsed = 1;
            //Debug.Log(" AuraAccessible 2");
            return Accessible.OK;
        }
        return Accessible.NeedPattern;
    }
    Accessible ConvertAccessible(bool[,] line)
    {
        if (PatternNumber > 1)
        {
            if (patternTrue(Pattern_2))
            {
                PatternUsed = 2;
                return Accessible.OK;
            }

        }
        if (patternTrue(Pattern_1))
        {
            PatternUsed = 1;
            return Accessible.OK;
        }
        return Accessible.NeedPattern;
    }
    public Accessible IsAccessible(bool isHeros)//是否可以施放  
    {
        if (EnergyManager.Instance.accessibleEnergy < cost && isHeros)
            return Accessible.NeedEnergy;
        if (EnergyManager.Instance.EEnergyAccessible(cost)==false && !isHeros)
            return Accessible.NeedEnergy;
         if (ID[0] == 'E')//结界
         {
             return AuraAccessible();
         }
        //变换卡
         {
             if (isHeros)
                 return ConvertAccessible(EnergyManager.Instance.HeroMagicCircle.line);
             else
                 return ConvertAccessible(EnergyManager.Instance.EnemyMagicCircle.line);
         }
         
    }
   
    public void CreateDefenceMagic()
    {
        if(tempNumber>0)
        {
            ShieldManager.Instance.SetPostion += this.SetPostion;
            ShieldManager.Instance.ShowPosition();
            tempNumber--;
        }
    }
    bool AtkNumOutRange()
    {
        if (AttackManager.Instance.attackMagicList.Count >= 12 && isHeros)
        {
            GuideText.Instance.ReturnText("AtkOutOfRange");
            return true;
        }
        if (AttackManager.Instance.enemyAttackMagicList.Count >= 12 && !isHeros)
        {
            return true;
        }
        return false;
    }
    public void CreateAttackMagic()
    {
        if(AtkNumOutRange())
        {
            return;
        }
        GameObject AttackCircle = (GameObject)Resources.Load("AttackCircle");
        GameObject AtkCircle = (GameObject)Instantiate(AttackCircle, transform.position, quaAtkCircle);
        AttackMagic attackMagic = AtkCircle.GetComponent<AttackMagic>();
        //对AttackMagic的属性进行更改
        if (isHeros)
        {
            AttackManager.Instance.attackMagicList.Add(attackMagic);
            AttackManager.Instance.WaitNum++;
            //AtkCircle.layer = 10;//己方的魔法
            attackMagic.myWaitNum = AttackManager.Instance.WaitNum;
            AttackManager.Instance.AttackID++;
            attackMagic.attackID = AttackManager.Instance.AttackID;
        }
        else
        {
            if (AttackManager.Instance.enemyAttackMagicList.Count >= 12)
            {
                return;
            }
            AttackManager.Instance.enemyAttackMagicList.Add(attackMagic);
            AttackManager.Instance.EnermyWaitNum++;
            //AtkCircle.layer = 11;//对方的魔法
            attackMagic.eWaitNum = AttackManager.Instance.EnermyWaitNum;
            AttackManager.Instance.EnemyAttackID++;
            attackMagic.enemyAttackID = AttackManager.Instance.EnemyAttackID;
        }
        InitialAtk(attackMagic);
        AtkCircle.transform.position = attackMagic.GetWaitPos();
        
    }
    void InitialAtk(AttackMagic attackMagic)
    {
        attackMagic.isHeros = isHeros;
        attackMagic.FindUIValue();
        attackMagic.name = this.name;
        attackMagic.magicValue = this.magicValue;
        AddEffect(attackMagic);
        attackMagic.refresh = true;
    }
    void CardUseEffect(string address)
    {
        GameObject Aura = (GameObject)Resources.Load(address);
        Object obj;
        if(isHeros)
        {
            obj = Instantiate(Aura, new Vector3(1000f, 0.1f, 850f), qua);
        }
        else
        {
            obj = Instantiate(Aura, new Vector3(1000f, 0.1f, 881f), qua);
        }
        Destroy(obj, 5f);
    }
    void AuraE24Return()
    {
        if (ID[0] != 'E')
        {
            EnergyManager.Instance.cardUsedTurn++;
            if (AuraManager.Instance.E24Heros && EnergyManager.Instance.cardUsedTurn == 1)
            {
                AddEffect("ReturnOnce", 1);
            }
        }
    }
    public IEnumerator Use()
    {
        //Debug.Log("Card:"+ID);
        #region 结界动画
        if (ID[0] == 'C')
        {
            CardUseEffect("ParticleSystem/Aura/gune 11");
        }
        if (ID[0] == 'E' && PatternNumber>1 && !isHeros)
        {
            CardUseEffect("ParticleSystem/Aura 1");
        }
        if (ID[0] == 'E' && PatternNumber ==1)
        {
            CardUseEffect("ParticleSystem/Aura 2");
        }
        #endregion

        if (isHeros)
        {
            #region isHeros
            EnergyManager.Instance.MyCardUse();//我的出牌事件
            AuraE24Return();

            EnergyManager.Instance.MinusEnergy(cost);//消耗能量
            if (ID[0] == 'C')
            {
                //抉择！！！
                //线消耗//线生成
                if (patternTrue(Pattern_1))
                {
                    patternDel(Pattern_consumption_1);
                    patternCreate(Pattern_reconstitution_1);
                }
                else if (patternTrue(Pattern_2))
                {
                    patternDel(Pattern_consumption_2);
                    patternCreate(Pattern_reconstitution_2);
                }
            }
            
            if (ID[0] == 'E' && PatternNumber > 1)//带抉择的结界
            {
                if (LevelManager.Instance.level!=2)
                {
                    #region 非第二教学关
                    if (patternTrue(Pattern_1) && !patternTrue(Pattern_2) || AuraManager.Instance.GetCardPatternUsed(ID) == 2)
                    {
                        PatternUsed = 1;
                        CardUseEffect("ParticleSystem/Aura 1");
                        CreateAura();
                    }
                    else if (patternTrue(Pattern_2) && !patternTrue(Pattern_1) || AuraManager.Instance.GetCardPatternUsed(ID) == 1)
                    {
                        PatternUsed = 2;
                        CardUseEffect("ParticleSystem/Aura 1");
                        CreateAura();
                    }
                    else
                    {
                        //抉择！
                        PatternUsed = 0;
                        LevelManager.Instance.pattern1.SetActive(true);
                        LevelManager.Instance.pattern2.SetActive(true);
                        LevelManager.Instance.pattern1.GetComponent<PatternChose>().CardCopy(this);
                        LevelManager.Instance.pattern2.GetComponent<PatternChose>().CardCopy(this);
                        transform.position = new Vector3(0, 0, 0);
                        for (; ; )
                        {
                            if(PatternUsed == 0)
                            yield return new WaitForSeconds(0.1f);
                            else
                            {
                                CreateAura();
                                CardUseEffect("ParticleSystem/Aura 1");
                                break;
                            }
                            
                        }
                    }
                    if (LevelManager.Instance.IsOnline)
                        Client.Instance.OnCastAura(ID);
                #endregion
                }
                else// 教学关 第二关
                {
                    #region 教学关 第二关
                    
                    LevelManager.Instance.pattern1.SetActive(true);
                    LevelManager.Instance.pattern2.SetActive(true);
                    PatternChose p1, p2;
                    p1=LevelManager.Instance.pattern1.GetComponent<PatternChose>();
                    p2 = LevelManager.Instance.pattern2.GetComponent<PatternChose>();
                    transform.position = new Vector3(0, 0, 0);
                    p1.CardCopy(this);
                    p2.CardCopy(this);
                    p1.canUse = true;
                    p2.canUse = true;
                    p1.backLight.SetActive(true);
                    p2.backLight.SetActive(true);
                    if (!patternTrue(Pattern_1) || AuraManager.Instance.GetCardPatternUsed(ID) == 1)
                    {
                        p1.backLight.SetActive(false);
                        p1.canUse = false;
                    }
                    if (!patternTrue(Pattern_2) || AuraManager.Instance.GetCardPatternUsed(ID) == 2)
                    {
                        p2.backLight.SetActive(false);
                        p2.canUse = false;
                    }
                    if (LevelManager.Instance.key.Contains("PatternChoose") && ID == "E01")
                    {
                        GuideText.Instance.ReturnText(27);
                        LevelManager.Instance.key.Add("TopDownAura");
                        LevelManager.Instance.key.Remove("PatternChoose");
                    }
                    PatternUsed = 0;
                    for (; ; )
                    {
                        if (PatternUsed == 0)
                            yield return new WaitForSeconds(0.1f);
                        else
                        {
                            //Debug.Log(PatternUsed.ToString());
                            CardUseEffect("ParticleSystem/Aura 1");
                            CreateAura();
                            if (LevelManager.Instance.key.Contains("DestroyAura") && ID == "E01")
                            {
                                //Debug.Log("destroy");
                                GuideText.Instance.ReturnText(25);
                                LevelManager.Instance.key.Remove("DestroyAura");
                            }
                            break;
                        }

                    }
                    #endregion
                }
            }
            if(ID[0] == 'E' && PatternNumber == 1)
            {
                CreateAura();
            }
            #endregion
        }
        else
        {
            EnergyManager.Instance.EnemyCardUse();//对手的出牌事件
            if(AI.Instance.aiType==AIType.WeakAI)
            {
                EnergyManager.Instance.EMinusEnergy(cost);
                //Debug.Log("weeak");
            }
            if(typeText=="结界")
            {
                CreateAura();
            }
        }
        
        if (typeText == "攻击" || isHeros && AuraManager.Instance.E28Heros && typeText == "防御"
            || !isHeros && AuraManager.Instance.E28Enemy && typeText == "防御")
        {
            #region 攻击
            if (isHeros)
            {
                if (LevelManager.Instance.IsOnline)
                    Client.Instance.OnCastAttack(ID);
            }
            for (int i = 1; i <= number; i++)//生成火球实体
            {
                CreateAttackMagic();
            }
            if(isHeros)
            {
                CameraMoving.Instance.canMove = true;
            }
            #endregion
        }
        else if (typeText == "防御")
        {
            #region 防御
            if (isHeros)
            {
                tempNumber = number;
                CreateDefenceMagic();
            }
            else
            {
                if (LevelManager.Instance.IsOnline)
                {
                    ShieldManager.Instance.EnermyShield(EnemyShield(EnergyManager.Instance.enemyTrajectoryID[EnergyManager.Instance.enemyOperationIndex]), EnergyManager.Instance.enemyTrajectoryID[EnergyManager.Instance.enemyOperationIndex]);
                }
                else
                {
                    CreateEnemyDefence();
                }
            }
            #endregion
        }
        else if(typeText=="治疗")
        {
            #region 治疗
            GameObject heal = (GameObject)Resources.Load("ParticleSystem/heal");
            if(isHeros)
            {
                heal= (GameObject)Instantiate(heal, HealPos, qua);
                heal.transform.eulerAngles = new Vector3(-90f, 0, 0);
                Hero hero = GameObject.Find("Hero").GetComponent<Hero>();
                hero.damage -= magicValue + AuraManager.Instance.healthPlus;
                CameraMoving.Instance.canMove = true;
            }
            else
            {
                heal = (GameObject)Instantiate(heal, EnermyHealPos, qua);
                heal.transform.eulerAngles = new Vector3(-90f, 0, 0);
                Hero hero = GameObject.Find("EnemyHero").GetComponent<Hero>();
                hero.damage -= magicValue + AuraManager.Instance.enemyHealthPlus;
            }
            Destroy(heal, 5f);
            #endregion
        }
        //else if(typeText=="结界") // 在上面
        //{
        //    #region 结界
            
        //    #endregion
        //}


        #region 教学判断
        GuideText.Instance.GuideLevel(1, 6, "Freedom", "S08", ID);
        GuideText.Instance.GuideLevel(3, 11, "DefensePosition", "S67", ID);
        GuideText.Instance.GuideLevel(3, 11, "DefensePosition", "S68", ID);
        GuideText.Instance.GuideLevel(3, 16, "Prepare", "C02", ID);
        if(ID=="C23" &&( LevelManager.Instance.level==4|| LevelManager.Instance.level==5))
        {
            LevelManager.Instance.key.Add("Weaken");
        }
        #endregion
        if ((Effect.ContainsKey("Return") || Effect.ContainsKey("ReturnOnce")) && isHeros)
        {
            #region 回手
            EnergyManager.Instance.StartTurn += this.Return;
            used = true;
            gameObject.SetActive(false);
            cardMoving.BeDestroy();
            #endregion
        }
        else if (isHeros)
        {
            if(gameObject.activeSelf)
                gameObject.SetActive(false);
            if (ID[0] != 'E')//非结界
                Destroy();//摧毁卡牌
        }
        
    }
    void CreateEnemyDefence()
    {
        if (ShieldManager.Instance.EM_Shield == null)
        {
            ShieldManager.Instance.EnermyShield(EnemyShield(0), 0);
        }
        else if (ShieldManager.Instance.EL_Shield == null && ShieldManager.Instance.ER_Shield == null)
        {
            int r = Random.Range(-1, 2);
            for (; r == 0; )
            {
                r = Random.Range(-1, 2);
            }
            ShieldManager.Instance.EnermyShield(EnemyShield(r), r);
        }
        else if (ShieldManager.Instance.EL_Shield == null)
        {
            ShieldManager.Instance.EnermyShield(EnemyShield(-1), -1);
        }
        else if (ShieldManager.Instance.ER_Shield == null)
        {
            ShieldManager.Instance.EnermyShield(EnemyShield(1), 1);
        }
        else
        {
            ShieldManager.Instance.EnermyShield(EnemyShield(0), 0);
        }
    }
    void CreateAura()
    {
        if (isHeros)
        {
            AuraManager.Instance.AddAura(this);
            cardMoving.BeDestroy();
            CameraMoving.Instance.canMove = true;
            used = true;
        }
        else
        {
            AuraManager.Instance.AddEnemyAura(this);
        }
    }

    void LineComsumption(List<int[]> Pattern_comsumption)
    {
        int[] temp;
        for (; Pattern_comsumption.Count != 0; )
        {
            temp = Pattern_comsumption[0];
            Pattern_comsumption.RemoveAt(0);
        }
    }

    public GameObject SetPostion()
    {
        ShieldPos = ShieldManager.Instance.pos;
        ShieldPos.y = 1.96f;
        qua.x = 0.0f;
        qua.z = 0.0f;
        qua.w = 1.0f;
        if (Mathf.Abs(ShieldPos.x - 1000) < 1)
        {
            qua.y = 0.0f;
            if (LevelManager.Instance.IsOnline)
            {
                //Debug.Log("Sheild "+ID+",0");
                Client.Instance.OnSheildUse(ID, 0);
            }
        }
        else if (ShieldPos.x > 1000)
        {
            qua.y = 0.2f;
            if (LevelManager.Instance.IsOnline)
            {
                //Debug.Log("Sheild " + ID + ",1");
                Client.Instance.OnSheildUse(ID, 1);
            }
        }
        else
        {
            qua.y = -0.2f;
            if (LevelManager.Instance.IsOnline)
            {
                //Debug.Log("Sheild " + ID + ",-1");
                Client.Instance.OnSheildUse(ID, -1);
            }
        }
        GameObject shield = (GameObject)Resources.Load("MagicShield2");
        GameObject Shield = (GameObject)Instantiate(shield, ShieldPos, qua);
        //对DefenseMagic的属性进行更改
        if(isHeros)
        {
            Shield.layer = 10;
            Shield.transform.localEulerAngles += new Vector3(0,180f,0);
            Shield.transform.FindChild("Value").localEulerAngles += new Vector3(0, 180f, 0);
        }
        DefenseMagic defenseMagic = Shield.GetComponent<DefenseMagic>();
        InitialDefence(defenseMagic);
        

        ShieldManager.Instance.HidePosition();
        ShieldManager.Instance.SetPostion -= this.SetPostion;
        CameraMoving.Instance.canMove = true;
        CreateDefenceMagic();


        //关卡说明
        if (LevelManager.Instance.level == 3 && LevelManager.Instance.key.Contains("Defense") && (ID == "S67" || ID == "S68"))
        {
            GuideText.Instance.ReturnText(12);
            LevelManager.Instance.key.Remove("Defense");
        }
        return Shield;
    }
    void InitialDefence(DefenseMagic defenseMagic)
    {
        defenseMagic.name = this.name;
        defenseMagic.magicValue = this.magicValue;
        defenseMagic.MagicMax = this.magicValue;
        defenseMagic.refresh = true;
        defenseMagic.isHeros = isHeros;
        AddEffect(defenseMagic);
        ShieldManager.Instance.defenseMagicList.Add(defenseMagic);
        ShieldManager.Instance.DefenseID++;
        defenseMagic.defenseID = ShieldManager.Instance.DefenseID;
    }
    public GameObject EnemyShield(int pos)
    {
        switch(pos)
        {
            case -1: ShieldPos = new Vector3(1005f, 1.96f, 874f); qua.x = 0; qua.y = 0.2f; break;
            case 0: ShieldPos = new Vector3(1000f, 1.96f, 872f); qua.x = 0; break;
            case 1: ShieldPos = new Vector3(995f, 1.96f, 874f); qua.x = 0; qua.y = -0.2f; break;
        }
        GameObject shield = (GameObject)Resources.Load("MagicShield2");
        GameObject Shield = (GameObject)Instantiate(shield, ShieldPos, qua);
        Shield.layer = 11;
        switch (pos)
        {
            case -1: Shield.transform.localEulerAngles = new Vector3(0, -25f, 0); break;
            case 0: Shield.transform.localEulerAngles=new Vector3(0,0f,0);  break;
            case 1: Shield.transform.localEulerAngles=new Vector3(0,25f,0);  break;
        }
        DefenseMagic defenseMagic = Shield.GetComponent<DefenseMagic>();
        defenseMagic.name = this.name;
        defenseMagic.magicValue = this.magicValue;
        defenseMagic.MagicMax = this.magicValue;
        defenseMagic.refresh = true;
        defenseMagic.isHeros = isHeros;
        ShieldManager.Instance.defenseMagicList.Add(defenseMagic);
        ShieldManager.Instance.EnemyDefenseID++;
        defenseMagic.enemyDefenseID = ShieldManager.Instance.EnemyDefenseID;
        AddEffect(defenseMagic);
        return Shield;
    }
    void AddEffect(Magic magic)
    {
        foreach (KeyValuePair<string, int> item in Effect)
        {
            magic.AddEffect(item.Key, item.Value);
        }
        if (Effect.ContainsKey("Preparation"))
        {
            (magic as AttackMagic).waitToFire += Effect["Preparation"];
        }
    }
    public void Destroy()
    {
        if(cardMoving!=null)
            cardMoving.BeDestroy();

        Destroy(this.gameObject);
    }

    #region Effect相关
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
    #endregion
    #region Pattern 相关
    public bool EGetLine2Draw(int Pattern2Draw)
    {
        if (Pattern2Draw == 1)
            return EGetLine2Draw(Pattern_1);
        if (Pattern2Draw == 2)
            return EGetLine2Draw(Pattern_2);
        return false;
    }
    bool EGetLine2Draw(int [][] Pattern)
    {
        Line line;
        for (int i = 0; i < Pattern.GetLength(0); i++)
        {
            line=new Line(Pattern[i][0], Pattern[i][1],Pattern[i][2], Pattern[i][3]);
            if(EnergyManager.Instance.EnemyMagicCircle.EDrawLine(line)==true)
            {
                return true;
            }
        }
        return false;
    }
    public bool patternTrue(int[][] Pattern)
    {
        Point point1, point2;
        for (int i = 0; i < Pattern.GetLength(0); i++)
        {
            point1 = new Point(Pattern[i][0], Pattern[i][1]);
            point2 = new Point(Pattern[i][2], Pattern[i][3]);
            //Debug.Log(Pattern[i][0].ToString() + "+" + Pattern[i][1].ToString() + "+" + Pattern[i][2].ToString() + "+" + Pattern[i][3].ToString());
            if (isHeros && !EnergyManager.Instance.HeroMagicCircle.GetLine(point1, point2))
                return false;
            if (!isHeros && !EnergyManager.Instance.EnemyMagicCircle.GetLine(point1, point2))
                return false;
        }
        return true;
    }
    void patternDel(List<int[]> Pattern)
    {
        Point point1, point2;
        for (int i = 0; i < Pattern.Count; i++)
        {
            point1 = new Point(Pattern[i][0], Pattern[i][1]);
            point2 = new Point(Pattern[i][2], Pattern[i][3]);
            //Debug.Log("pattenDel" + Pattern[i][0].ToString() + "+" + Pattern[i][1].ToString() + "+" + Pattern[i][2].ToString() + "+" + Pattern[i][3].ToString());
            if (isHeros)
                RayTest.Instance.DeleteLine(point1, point2);
            else
                EnergyManager.Instance.EnemyMagicCircle.DeleteLine(point1.GetUni(), point2.GetUni());
        }
    }
    void patternCreate(List<int[]> Pattern)
    {
        Point point1, point2;
        for (int i = 0; i < Pattern.Count; i++)
        {
            point1 = new Point(Pattern[i][0], Pattern[i][1]);
            point2 = new Point(Pattern[i][2], Pattern[i][3]);
            if ( isHeros &&!EnergyManager.Instance.HeroMagicCircle.GetLine(point1, point2))
            {
                //Debug.Log("pattenCreate"+Pattern[i][0].ToString() + "+" + Pattern[i][1].ToString() + "+" + Pattern[i][2].ToString() + "+" + Pattern[i][3].ToString());
                RayTest.Instance.AddLine(point1.GetUni(), point2.GetUni());

            }
            if(!isHeros && !EnergyManager.Instance.EnemyMagicCircle.GetLine(point1,point2))
            {
                EnergyManager.Instance.EnemyMagicCircle.LineTrue(point1.GetUni(), point2.GetUni());
            }
        }
    }
    #endregion
}
