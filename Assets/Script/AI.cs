using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum AIType
{
    CardList,//读入整局使用的list
    RoundList,//读入每回合使用的list
    WeakAI,
    //会主动放盾,按着给定的水晶序列放水晶，自己按牌来画线，
    //结界只能是一个pattern的,或者先画第一个再画第二个,而且不停的画线，就算这回合用不出来，也不会留一点给其他卡
    //不能处理变换卡（仅当前面的结界卡都使用完后）,线不能被水晶截成两半，仅考虑当下一步
    StrongAI
}
public enum AIStyle
{
    CrazyDog,//疯狗，快攻，预算血量大于1无脑打对手脸,同归于尽，能打脸一定打脸
    RefreshProtector,//刷新保卫者，在不死的情况下，尽力保卫刷新
    Berserker,//对刚，用自己最强的地方和对手最强的地方对刚，追求一定的破盾打击
    Conservative,//保守，后期，尽力护脸
    YZAM//月之暗面
}

public class AI : MonoBehaviour {

    public static AI Instance;
    public Transform EnemyHand;
    public Transform EReturnList;
    public AIType aiType=AIType.WeakAI;
    AIStyle aiStyle = AIStyle.CrazyDog;

    Hero hero, enemyHero;
    List<Card> accessibleCardList = new List<Card>();
    List<Card> usedAuraList = new List<Card>();
    //List<Line> lineList = new List<Line>();
    List<Point> kengList = new List<Point>();
    List<AttackMagic> freedomList = new List<AttackMagic>();//EnemyFreedomList
    int damagePermit;
    int enemyMiddle ;
    int enemyFreedom ;
    int herosMiddle ;
    int herosFreedom ;//调试pulic

    int L_Shield =  0;
    int M_Shield =  0;
    int R_Shield =  0;
    bool L_Refresh = false;
    bool M_Refresh = false;
    bool R_Refresh = false;

    int EL_Shield = 0;
    int EM_Shield = 0;
    int ER_Shield = 0;
    bool EL_Refresh =  false;
    bool EM_Refresh =  false;
    bool ER_Refresh =  false;
	// Use this for initialization
	void Start () {
        AI.Instance = this;

        hero = GameObject.Find("Hero").GetComponent<Hero>();
        enemyHero = GameObject.Find("EnemyHero").GetComponent<Hero>();
	}
	
    public void ReadKengList(List<Point> list)
    {
        kengList = list;
    }
    public void AIDrawCard()
    {
        if (aiType != AIType.WeakAI)
            return;
        //抽牌+卡牌实体化
        if (EnergyManager.Instance.roundCount == 1)
            for (int i = 0; i < 4; i++)
                DrawCard.Instance.EnemyDraw(i);
        DrawCard.Instance.EnemyDraw(EnergyManager.Instance.roundCount + 3);
    }
    public void SetAIStyle(AIStyle style)
    {
        aiStyle = style;
        switch(aiStyle)
        {
            case AIStyle.Berserker:
                damagePermit = 1;
                break;
            case AIStyle.YZAM:
                damagePermit = 0;
                break;
            case AIStyle.Conservative:
                damagePermit = 1;
                break;
            case AIStyle.RefreshProtector:
                damagePermit = 1;
                break;
            case AIStyle.CrazyDog:
                damagePermit = 2;
                break;
        }
    }
    public void AIOperation()
    {
        //Debug.Log("AIOperation");
        //放水晶
        if (EnergyManager.Instance.roundCount <= 6)
        {
            int round = EnergyManager.Instance.roundCount;
            EnergyManager.Instance.EnemyMagicCircle.KengTrue(kengList[round * 2 - 2]);
            EnergyManager.Instance.EnemyMagicCircle.KengTrue(kengList[round * 2 - 1]);
        }
        AIDefence();
        for (; EnemyDrawLine(); ) ;//把所有能量都拿来画线
        //使用牌后如果没有回手就摧毁
        //使用可以用的结界
        Card card;
        for (int i = EnemyHand.childCount - 1; i >= 0; i--)
        {
            card = EnemyHand.GetChild(i).GetComponent<Card>();
            if (card.IsAccessible(false) == Accessible.OK && card.GetCardType() == CardType.aura && !AuraManager.Instance.InTempList(card))
            {
                DrawCard.Instance.Cast(card.ID, 0);
                AuraManager.Instance.AddTempList(card);
                usedAuraList.Add(card);
                continue;
            }
            if (AuraManager.Instance.InTempList(card))
            {
                DrawCard.Instance.Cast(card.ID, 0);
                usedAuraList.Add(card);
                continue;
            }
        }
        AIUseCard();
        AuraManager.Instance.ClearTempList();
        ReturnHand();
    }
    public void DestroyUsedAura()
    {
        foreach (Card card in usedAuraList)
        {
            card.Destroy();
        }
        usedAuraList.Clear();
    }
    public void EndGame()
    {
        foreach (Transform card in EnemyHand.transform)
        {
            card.GetComponent<Card>().Destroy();
        }
    }
    void UpdateAccessibleCardList()
    {
        accessibleCardList.Clear();
        Accessible ac;
        Card cardScript;
        foreach(Transform card in EnemyHand)
        {
            cardScript = card.GetComponent<Card>();
            ac = cardScript.IsAccessible(false);
            if(ac==Accessible.OK)
            {
                accessibleCardList.Add(cardScript);
            }
        }
    }
    //public void ReadLineList(List<Line> list)
    //{
    //    lineList = list;
    //}
   
    void CastAtk(Card card)
    {
        if (card == null)
            return;
        EnergyManager.Instance.EMinusEnergy(card.cost);
        DrawCard.Instance.Cast(card.ID, 0);
    }
    void UpdateEShieldInfo(Card card,TrajectoryType tra)
    {
        switch (tra)
        {
            case TrajectoryType.Left:
                EL_Shield = card.magicValue;
                EL_Refresh = card.HasEffect("Refresh");
                break;
            case TrajectoryType.Middle:
                EM_Shield = card.magicValue;
                EM_Refresh = card.HasEffect("Refresh");
                break;
            case TrajectoryType.Right:
                ER_Shield = card.magicValue;
                ER_Refresh = card.HasEffect("Refresh");
                break;
        }
    }
    void CastShield_Destroy(Card card,TrajectoryType tra)
    {
       // Debug.Log("CastShield");
        if (card == null)
            return;
        int trajectory=0;
        switch(tra)
        {
            case TrajectoryType.Left:
                trajectory = -1;
                break;
            case TrajectoryType.Middle:
                trajectory = 0;
                break;
            case TrajectoryType.Right:
                trajectory = 1;
                break;
        }
        EnergyManager.Instance.EMinusEnergy(card.cost);
        DrawCard.Instance.Cast(card.ID, trajectory);
        UpdateEShieldInfo(card, tra);
        DestroyUsedCard(card);
    }
    void DestroyUsedCard(Card card)
    {
        if (card == null)
            return;
        if(card.HasEffect("Return")==false)
        {
            card.Destroy();
            return;
        }
        //Return
        card.transform.parent = EReturnList;
    }
    void ReturnHand()
    {
        List<Card> cardList=GetCardList(EReturnList);
        foreach(Card card in cardList)
        {
            card.transform.parent = EnemyHand;
        }
    }
    int Damage(int damagePermit,int atk)
    {
        return atk - damagePermit;
    }
    List<Card> GetDefList()
    {
        List<Card> DefList = new List<Card>();
        foreach (Transform c in EnemyHand)
        {
            if (c.GetComponent<Card>().GetCardType() == CardType.defence)
                DefList.Add(c.GetComponent<Card>());
        }
        return DefList;
    }
    void AIDefence()
    {
        
        bool hasE02 = false;
        foreach (Transform c in EnemyHand)
        {
            if (c.GetComponent<Card>().ID=="E02")
            {
                hasE02 = true;
                break;
            }
        }
        GetAIInformation();
        //(damagePermit + attackMiddle + EM_Shield, herosMiddle+herosFreedom)? converative?
        int Mdamage = hasE02 ? Damage(damagePermit + EM_Shield, herosMiddle + herosFreedom) : Damage(damagePermit + enemyMiddle + EM_Shield, herosMiddle + herosFreedom);//以后要改
        int Rdamage = Damage(damagePermit + ER_Shield, herosFreedom);
        int Ldamage = Damage(damagePermit + EL_Shield, herosFreedom);
        //Debug.Log("M:" + Mdamage.ToString());
        //Debug.Log("L:" + Ldamage.ToString());
        //Debug.Log("R:" + Rdamage.ToString());
        if (Mdamage <= 0 && Rdamage <= 0 && Ldamage <= 0)
            return;
        //比较三个方向的damage找到最大的，在该方向放盾，repeat
        TrajectoryType tra;
        Dictionary<TrajectoryType,int> traDict=new Dictionary<TrajectoryType,int>();
        traDict.Add(TrajectoryType.Middle,Mdamage);
        int random= Random.Range(0, 2);
        if (random == 0)//交换顺序
        {
            traDict.Add(TrajectoryType.Left, Ldamage);
            traDict.Add(TrajectoryType.Right, Rdamage);
        }
        else
        {
            traDict.Add(TrajectoryType.Right, Rdamage);
            traDict.Add(TrajectoryType.Left, Ldamage);
        }
        for (int i = 0; i < 3;i++ )
        {
            tra=WorseTrajectory(traDict);
            traDict.Remove(tra);
            //Debug.Log(tra.ToString());
            if(tra==TrajectoryType.Null)
                break;
            Trajectory2Def(damagePermit,tra,hasE02);
        }
    }
    TrajectoryType WorseTrajectory(Dictionary<TrajectoryType,int> traDict)
    {
        KeyValuePair<TrajectoryType, int> worse = new KeyValuePair<TrajectoryType, int>(TrajectoryType.Null, 0);
        foreach(KeyValuePair<TrajectoryType,int> t in traDict )
        {
            if(t.Value>worse.Value)
            {
                worse = t;
            }
        }
        return worse.Key;
    }
    bool RefreshFreedomEnough(TrajectoryType tra,bool E02)
    {
        switch(tra)
        {
            case TrajectoryType.Middle:
                if (EM_Refresh == false)
                    return false;
                if(E02)
                    return Damage(enemyMiddle+EM_Shield+enemyFreedom,herosMiddle+herosFreedom)<=0;//要改
                else
                    return Damage(enemyMiddle + EM_Shield + enemyFreedom, herosMiddle + herosFreedom) <= 0;
            case TrajectoryType.Left:
                if (EL_Refresh == false)
                    return false;
                if (E02)
                    return Damage(EL_Shield + enemyFreedom, herosFreedom) <= 0;//要改
                else
                    return Damage(enemyMiddle + EL_Shield + enemyFreedom, herosFreedom) <= 0;
            case TrajectoryType.Right:
                if (ER_Refresh == false)
                    return false;
                if (E02)
                    return Damage(ER_Shield + enemyFreedom, herosFreedom) <= 0;//要改
                else
                    return Damage(enemyMiddle + ER_Shield + enemyFreedom, herosFreedom) <= 0;
        }
        return true;
    }
    void Trajectory2Def(int damagePermit,TrajectoryType tra,bool E02)
    {

        if (RefreshFreedomEnough(tra, E02))
            return;
        switch(tra)
        {
            case TrajectoryType.Middle:
                if(E02==true)
                {
                    UseCard2Def( EM_Shield, damagePermit, herosMiddle + herosFreedom, TrajectoryType.Middle);
                    return;
                }
                UseCard2Def(EM_Shield, damagePermit + enemyMiddle, herosMiddle + herosFreedom, TrajectoryType.Middle);
                return;
            case TrajectoryType.Left:
                UseCard2Def( EL_Shield, damagePermit, herosFreedom, TrajectoryType.Left);
                return;
            case TrajectoryType.Right:
                UseCard2Def( ER_Shield, damagePermit, herosFreedom, TrajectoryType.Right);
                return;
        }
    }
    void UseCard2Def(int defenceNow,int atkPermit,int atk,TrajectoryType tra)
    {
        Card card = FindDefenceCard(defenceNow,atkPermit, atk);
        CastShield_Destroy(card, tra);
    }
    Card FindDefenceCard(int defenceNow,int atkPermit,int atk)//选择费用最少的
    {
        List<Card> cannotlist=new List<Card>();
        List<Card> canlist = new List<Card>();
        bool canDefence = false;
        List<Card> DefList = GetDefList();
        foreach(Card card in DefList)
        {
            if(card.IsAccessible(false)==Accessible.OK)
            {
                if(defenceNow<card.magicValue)
                {
                    cannotlist.Add(card);
                    if (card.magicValue + atkPermit >= atk)
                    {
                        canDefence = true;
                        canlist.Add(card);
                    }
                }
            }
        }
        if(canDefence)
        {
            //Debug.Log("CanDefence");
            return FindMinCostCard(canlist);
        }
        //Debug.Log("Can't CanDefence");
        return FindValueBiggest(cannotlist);
    }
    Card FindValueBiggest(List<Card> list)
    {
        if (list.Count == 0)
            return null;
        Card c = list[list.Count - 1];
        for (int i = list.Count - 1; i >= 0; i--)
        {
            if (list[i].magicValue > c.magicValue)
            {
                c = list[i];
                Debug.Log(c.name);
            }
        }
        return c;
    }
    Card FindMinCostCard(List<Card> list)
    {
        if (list.Count == 0)
            return null;
        Card c=list[list.Count-1];
        for (int i = list.Count - 1;i>=0 ; i--)
        {
            if(list[i].cost<c.cost)
            {
                c = list[i];
            }
        }
        return c;
    }
    List<Card> GetCardList(Transform t)
    {
        List<Card> cardList = new List<Card>();
        foreach (Transform c in t)
        {
            cardList.Add(c.GetComponent<Card>());
        }
        return cardList;
    }
    void AIUseCard()//【尽量使用能量】，先使用攻击牌，按顺序丢，没有合适的牌使用时可以使用英雄技能
    {
        List<Card> cardList = GetCardList(EnemyHand);
        foreach(Card card in cardList)
        {
            if(card.IsAccessible(false)==Accessible.OK && card.GetCardType()==CardType.attack)
            {
                CastAtk(card);
                DestroyUsedCard(card);
            }
        }
        foreach(Card card in cardList)
        {
            if(card.ID=="S00")
            {
                RandShield(card);
                break;
            }
        }
    }
    void RandShield(Card card)
    {
        if (card.IsAccessible(false) != Accessible.OK)
        {
            return;
        }
        if(EM_Shield < card.magicValue && !EM_Refresh)
        {
            CastShield_Destroy(card, TrajectoryType.Middle);
            return;
        }
        int i = Random.Range(0, 2);
        if(i==0)
        {
            if (ER_Shield < card.magicValue && !ER_Refresh)
            {
                CastShield_Destroy(card, TrajectoryType.Right);
                return;
            }
            if (EL_Shield < card.magicValue && !EL_Refresh)
            {
                CastShield_Destroy(card, TrajectoryType.Left);
                return;
            }
        }
        else
        {
            if (EL_Shield < card.magicValue && !EL_Refresh)
            {
                CastShield_Destroy(card, TrajectoryType.Left);
                return;
            }
            if (ER_Shield < card.magicValue && !ER_Refresh)
            {
                CastShield_Destroy(card, TrajectoryType.Right);
                return;
            }
        }
        
    }
    
    bool EnemyDrawLine()
    {
        //if (lineList.Count <= 0)
        //    return false;
        if (EnergyManager.Instance.EEnergyAccessible(1) == false)
            return false;
        Card card;
        foreach (Transform c in EnemyHand.transform)
        {
            card = c.GetComponent<Card>();
            //如果它在aura的templist上就查看下一个card
            if (AuraManager.Instance.InTempList(card))
                continue;
            if(card.GetCardType()==CardType.aura && card.IsAccessible(false)!=Accessible.OK)
            {
               // Debug.Log(card.ID);
                bool success = false;
                if(card.IsAccessible(false)==Accessible.NeedPattern)
                    success=card.EDrawLine(1);
                if (card.IsAccessible(false) == Accessible.NeedSymmetryPattern)
                    success=card.EDrawLine(2);
                if (success == false)
                    continue;
                EnergyManager.Instance.EMinusEnergy(1);
                if(card.IsAccessible(false)==Accessible.OK)
                {
                    AuraManager.Instance.AddTempList(card);
                }
                return true;
            }
        }
        
        return false;
    }
    public void ReStart()
    {
        
    }
    public void GetHerosInformation()//回合开始时获得玩家的状况
    {
        //Debug.Log("GetHerosInformation");
        herosMiddle = AttackManager.Instance.HerosMiddle();
        herosFreedom = AttackManager.Instance.HerosFreedom();
        L_Shield = GetShieldValue(ShieldManager.Instance.L_DefenseMagic);
        M_Shield = GetShieldValue(ShieldManager.Instance.M_DefenseMagic);
        R_Shield = GetShieldValue(ShieldManager.Instance.R_DefenseMagic);
        L_Refresh = HasRefresh(ShieldManager.Instance.L_DefenseMagic);
        M_Refresh = HasRefresh(ShieldManager.Instance.M_DefenseMagic);
        R_Refresh = HasRefresh(ShieldManager.Instance.R_DefenseMagic);
        if (AuraManager.Instance.E02Heros == true)
        {
            herosFreedom += herosMiddle;
            herosMiddle = 0;
        }
    }
    int GetShieldValue(DefenseMagic magic)
    {
        return magic != null ? magic.magicValue : 0;
    }
    bool HasRefresh(DefenseMagic magic)
    {
        return magic != null ? magic.HasEffect("Refresh") : false;
    }
    void GetAIInformation()
    {
        enemyMiddle = AttackManager.Instance.CaculateAttackMiddle();
        enemyFreedom = AttackManager.Instance.CaculateAttackFreedom();
        freedomList = AttackManager.Instance.GetFreedomList();

        EL_Shield = GetShieldValue(ShieldManager.Instance.EL_DefenseMagic);
        EM_Shield = GetShieldValue(ShieldManager.Instance.EM_DefenseMagic);
        ER_Shield = GetShieldValue(ShieldManager.Instance.ER_DefenseMagic);

        EL_Refresh = HasRefresh(ShieldManager.Instance.EL_DefenseMagic);
        EM_Refresh = HasRefresh(ShieldManager.Instance.EM_DefenseMagic);
        ER_Refresh = HasRefresh(ShieldManager.Instance.ER_DefenseMagic);

        if (AuraManager.Instance.E02Enemy == true)
        {
            enemyFreedom += enemyMiddle;
            enemyMiddle = 0;
            freedomList.Clear();
            freedomList = AttackManager.Instance.GetFireList();
        }
    }
    //从头开始一个一个拉出来，如果超过了就继续,同时加强该路的防御
    void DefenseTheWay(int defencePower, int atkPower, TrajectoryType way)
    {
        Debug.Log("DTW:" + way.ToString()+"def："+defencePower.ToString()+"atk:"+atkPower.ToString() +"  NUM:"+freedomList.Count.ToString());
        int def = 0;
        for (int i = freedomList.Count - 1; i >= 0; i--)
        {
            def += freedomList[i].magicValue;
            AttackManager.Instance.SetFreedomTrajectory(way, freedomList[i]);
            freedomList.RemoveAt(i);
            if (def + defencePower > atkPower)
            {
                break;
            }
        }
        switch(way)
        {
            case TrajectoryType.Left:
                EL_Shield += def;
                break;
            case  TrajectoryType.Middle:
                EM_Shield += def;
                break;
            case TrajectoryType.Right:
                ER_Shield += def;
                break;
        }
    }
    int CaculateValueSum(List<AttackMagic> List)
    {
        int sum = 0;
        foreach(AttackMagic a in List)
        {
            sum += a.magicValue;
        }
        return sum;
    }
    public void AIFreedomChooseWay()
    {
        GetAIInformation();
        //Debug.Log("Attack");
        //如果可以斩杀，就尽量斩杀
        //不要主动去打盾
        //否则有刷新盾的优先打爆
        //没有刷新盾或不能打爆，优先造成血量伤害
        //都不能打爆的也绝不打刷新盾

        if (enemyFreedom == 0)
        {
            //Debug.Log("0");
            return;
        }
        //能活，用AI自己的自由来防御
        {
            if (herosMiddle + herosFreedom > EM_Shield + enemyMiddle + enemyHero.health)
            {
                DefenseTheWay(EM_Shield + enemyMiddle + damagePermit, herosMiddle + herosFreedom, TrajectoryType.Middle);
            }
            if (herosFreedom > ER_Shield + enemyHero.health)
            {
                DefenseTheWay(ER_Shield + damagePermit, herosFreedom, TrajectoryType.Right);
            }
            if (herosFreedom > EL_Shield + enemyHero.health)
            {
                DefenseTheWay(EL_Shield + damagePermit, herosFreedom, TrajectoryType.Left);
            }
        }
        #region 如果玩家一侧防御和血量和攻击的和<= 斩杀
        if (hero.health + M_Shield + herosMiddle + herosFreedom
            <= (enemyMiddle + enemyFreedom))
        {
            //Debug.Log("1");
            AttackManager.Instance.SetFreedomTrajectory(0, freedomList);
            return;
        }
        if ((hero.health + L_Shield + herosFreedom)
           <= enemyFreedom)
        {
            //Debug.Log("2");
            AttackManager.Instance.SetFreedomTrajectory(TrajectoryType.Left, freedomList);
            return;
        }
        if ((hero.health + R_Shield + herosFreedom)
           <= enemyFreedom)
        {
            //Debug.Log("3");
            AttackManager.Instance.SetFreedomTrajectory(TrajectoryType.Right, freedomList);
            return;
        }
        #endregion

        #region 只要玩家的自由不进行防御，就必死，投机  AIStyle.CrazyDog
        if (AI.Instance.aiStyle == AIStyle.CrazyDog)
        {
            if (hero.health + M_Shield + herosMiddle
            <= (enemyMiddle + enemyFreedom))
            {
                //Debug.Log("CrazyDog 1");
                AttackManager.Instance.SetFreedomTrajectory(0, freedomList);
                return;
            }
            if ((hero.health + L_Shield)
               <= enemyFreedom)
            {
                //Debug.Log("CrazyDog 2");
                AttackManager.Instance.SetFreedomTrajectory(TrajectoryType.Left, freedomList);
                return;
            }
            if ((hero.health + R_Shield)
               <= enemyFreedom)
            {
                //Debug.Log("CrazyDog 3");
                AttackManager.Instance.SetFreedomTrajectory(TrajectoryType.Right, freedomList);
                return;
            }
        }
        #endregion

        


        //在盾会被打爆的情况下护一下脸 ！AIStyle.CrazyDog
        //不同情况下选择护脸，[不护脸伤害太高]，[百分比]，[所剩生命值太低],[下回合防不住]
        {
            if (herosMiddle + herosFreedom > EM_Shield + enemyMiddle + damagePermit)
            {
                DefenseTheWay(EM_Shield + enemyMiddle + damagePermit, herosMiddle + herosFreedom, TrajectoryType.Middle);
            }
            if (herosFreedom > ER_Shield + damagePermit)
            {
                DefenseTheWay(ER_Shield + damagePermit, herosFreedom, TrajectoryType.Right);
            }
            if (herosFreedom > EL_Shield + damagePermit)
            {
                DefenseTheWay(EL_Shield + damagePermit, herosFreedom, TrajectoryType.Left);
            }
        }
        //用自己的自由去保护刷新
        //用刚好的还是超额的
        //从头开始一个一个拉出来，如果超过了就继续
        if (AI.Instance.aiStyle == AIStyle.RefreshProtector)
        {
            //Debug.Log("protector:" + EM_Refresh.ToString() + (herosMiddle + herosFreedom > EM_Shield + enemyMiddle).ToString());
            if (EM_Refresh == true && (herosMiddle + herosFreedom > EM_Shield + enemyMiddle))
            {
                DefenseTheWay(EM_Shield + enemyMiddle,herosMiddle + herosFreedom,TrajectoryType.Middle);
            }
            if (ER_Refresh == true && (herosFreedom > ER_Shield))
            {
                DefenseTheWay(ER_Shield, herosFreedom, TrajectoryType.Right);
            }
            if (EL_Refresh == true && (herosFreedom > EL_Shield))
            {
                DefenseTheWay(EL_Shield, herosFreedom, TrajectoryType.Left);
            }
        }
        
        //Berserker,用所有的自由来
        if(aiStyle==AIStyle.Berserker)
        {
            if (enemyHero.health + EM_Shield + enemyMiddle
            <= (herosMiddle + herosFreedom))
            {
                //Debug.Log("Berserker 1");
                AttackManager.Instance.SetFreedomTrajectory(0, freedomList);
                return;
            }
            if ((enemyHero.health + EL_Shield)
               <= herosFreedom)
            {
                //Debug.Log("Berserker 2");
                AttackManager.Instance.SetFreedomTrajectory(TrajectoryType.Left, freedomList);
                return;
            }
            if ((enemyHero.health + ER_Shield)
               <= herosFreedom)
            {
                //Debug.Log("Berserker 3");
                AttackManager.Instance.SetFreedomTrajectory(TrajectoryType.Right, freedomList);
                return;
            }
        }
        
        //优先打爆盾/都打到都很低？
        //计算破盾后的伤害[大于某值]【百分比】【剩余血量】
        enemyFreedom = CaculateValueSum(freedomList);
        if(enemyFreedom-L_Shield>=4)
        {
            AttackManager.Instance.SetFreedomTrajectory(TrajectoryType.Left, freedomList);
            return;
        }
        if (enemyFreedom - R_Shield >= 4)
        {
            AttackManager.Instance.SetFreedomTrajectory(TrajectoryType.Right, freedomList);
            return;
        }

        //不要主动去打盾
        if (M_Shield==0 && L_Shield>0 && R_Shield>0)
        {
            //Debug.Log("14");
            AttackManager.Instance.SetFreedomTrajectory(TrajectoryType.Middle, freedomList);
            return;
        }
        if (M_Shield>0 && L_Shield==0 && R_Shield>0)
        {
            //Debug.Log("15");
            AttackManager.Instance.SetFreedomTrajectory(TrajectoryType.Left, freedomList);
            return;
        }
        if (M_Shield>0 && L_Shield>0 && R_Shield==0)
        {
            //Debug.Log("16");
            AttackManager.Instance.SetFreedomTrajectory(TrajectoryType.Right, freedomList);
            return;
        }
        if (M_Shield == 0 && L_Shield == 0 && R_Shield > 0)
        {
            //Debug.Log("17");
            AttackManager.Instance.SetFreedomTrajectory(TrajectoryType.Left, freedomList);
            return;
        }
        if (M_Shield == 0 && L_Shield > 0 && R_Shield == 0)
        {
            //Debug.Log("18");
            AttackManager.Instance.SetFreedomTrajectory(TrajectoryType.Right, freedomList);
            return;
        }
        //优先破刷新
        if (M_Shield>0 && M_Refresh && (M_Shield <= (enemyFreedom + enemyMiddle)))
        {
            //Debug.Log("4");
            AttackManager.Instance.SetFreedomTrajectory(TrajectoryType.Middle, freedomList);
            return;
        }
        if (L_Shield>0 && L_Refresh && (L_Shield <= enemyFreedom))
        {
            //Debug.Log("5");
            AttackManager.Instance.SetFreedomTrajectory(TrajectoryType.Left, freedomList);
            return;
        }
        if (R_Shield>0 && R_Refresh && (R_Shield <= enemyFreedom))
        {
            //Debug.Log("6");
            AttackManager.Instance.SetFreedomTrajectory(TrajectoryType.Right, freedomList);
            return;
        }
        else
        {
            //Debug.Log("7");
            foreach (AttackMagic a in freedomList)
            {
                for (; a.trajectory == TrajectoryType.Middle; )
                {
                    a.trajectory = (TrajectoryType)Random.Range(-1, 2);
                    if (a.trajectory == TrajectoryType.Right && R_Shield>0)
                    {
                        if ( !L_Refresh)
                            a.trajectory = TrajectoryType.Left;
                    }
                    else if (a.trajectory == TrajectoryType.Left && L_Shield>0)
                    {
                        if (!R_Refresh)
                            a.trajectory = TrajectoryType.Right;
                    }
                }
            }
        }

    }
}
