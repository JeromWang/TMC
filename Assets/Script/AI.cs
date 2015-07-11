using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum AIType
{
    CardList,//读入整局使用的list
    RoundList,//读入每回合使用的list
    WeakAI//会主动放盾
}
public enum AIStyle
{
    CrazyDog,//疯狗，快攻，预算血量大于1无脑打对手脸,同归于尽，能打脸一定打脸
    RefreshProtector,//刷新保卫者，在不死的情况下，尽力保卫刷新
    Berserker,//对刚，用自己最强的地方和对手最强的地方对刚
    Conservative,//保守，后期，尽力护脸
    YZAM//月之暗面
}

public class AI : MonoBehaviour {

    public static AI Instance;
    public Transform EnemyHand;
    public AIType aiType=AIType.WeakAI;
    public AIStyle aiStyle = AIStyle.CrazyDog;

    Hero hero, enemyHero;
    List<Card> accessibleCardList = new List<Card>();
    List<Line> lineList = new List<Line>();
    List<AttackMagic> freedomList = new List<AttackMagic>();//EnemyFreedomList
    int attackMiddle ;
    int attackFreedom ;
    int herosMiddle ;
    int herosFreedom ;

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
	
	// Update is called once per frame
	void Update () {
	
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
    public void AddLineList(Line line)
    {
        lineList.Add(line);
    }
    void EnemyDrawLine()
    {
        if (lineList.Count <= 0)
            return;
        if (EnergyManager.Instance.EEnergyAccessible(1) == false)
            return;
        if(MagicCircleMananger.Instance.EDrawLine(lineList[0]))
        {
            lineList.RemoveAt(0);
            EnergyManager.Instance.EMinusEnergy(1);
        }
    }
    public void ReStart()
    {
        EnergyManager.Instance.StartTurn += this.GetHerosInformation;
    }
    public void GetHerosInformation()//回合开始时获得玩家的状况
    {
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
        attackMiddle = AttackManager.Instance.CaculateAttackMiddle();
        attackFreedom = AttackManager.Instance.CaculateAttackFreedom();
        freedomList = AttackManager.Instance.GetFreedomList();

        EL_Shield = GetShieldValue(ShieldManager.Instance.EL_DefenseMagic);
        EM_Shield = GetShieldValue(ShieldManager.Instance.EM_DefenseMagic);
        ER_Shield = GetShieldValue(ShieldManager.Instance.ER_DefenseMagic);

        EL_Refresh = HasRefresh(ShieldManager.Instance.EL_DefenseMagic);
        EM_Refresh = HasRefresh(ShieldManager.Instance.EM_DefenseMagic);
        ER_Refresh = HasRefresh(ShieldManager.Instance.ER_DefenseMagic);

        if (AuraManager.Instance.E02Enemy == true)
        {
            attackFreedom += attackMiddle;
            attackMiddle = 0;
            freedomList.Clear();
            freedomList = AttackManager.Instance.GetFireList();
        }
    }
    //从头开始一个一个拉出来，如果超过了就继续,同时加强该路的防御
    void DefenseTheWay(int defencePower, int atkPower, TrajectoryType way)
    {
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
    public void AIFreedomChooseWay()
    {
        GetAIInformation();
        //Debug.Log("Attack");
        //如果可以斩杀，就尽量斩杀
        //不要主动去打盾
        //否则有刷新盾的优先打爆
        //没有刷新盾或不能打爆，优先造成血量伤害
        //都不能打爆的也绝不打刷新盾

        if (attackFreedom == 0)
        {
            //Debug.Log("0");
            return;
        }

        #region 如果一侧防御和血量和攻击的和<= 斩杀
        if (hero.health + M_Shield + herosMiddle + herosFreedom
            <= (attackMiddle + attackFreedom))
        {
            //Debug.Log("1");
            AttackManager.Instance.SetFreedomTrajectory(0, freedomList);
            return;
        }
        if ((hero.health + L_Shield + herosFreedom)
           <= attackFreedom)
        {
            //Debug.Log("2");
            AttackManager.Instance.SetFreedomTrajectory(TrajectoryType.Left, freedomList);
            return;
        }
        if ((hero.health + R_Shield + herosFreedom)
           <= attackFreedom)
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
            <= (attackMiddle + attackFreedom))
            {
                //Debug.Log("CrazyDog 1");
                AttackManager.Instance.SetFreedomTrajectory(0, freedomList);
                return;
            }
            if ((hero.health + L_Shield)
               <= attackFreedom)
            {
                //Debug.Log("CrazyDog 2");
                AttackManager.Instance.SetFreedomTrajectory(TrajectoryType.Left, freedomList);
                return;
            }
            if ((hero.health + R_Shield)
               <= attackFreedom)
            {
                //Debug.Log("CrazyDog 3");
                AttackManager.Instance.SetFreedomTrajectory(TrajectoryType.Right, freedomList);
                return;
            }
        }
        #endregion

        //能活，用自己的自由来防御
        

        //用自己的自由去保护刷新
        //用刚好的还是超额的
        //从头开始一个一个拉出来，如果超过了就继续
        if (AI.Instance.aiStyle == AIStyle.RefreshProtector)
        {
            //先护脸
            int damagePermit = 3;
            if (herosMiddle + herosFreedom > EM_Shield + attackMiddle + damagePermit)
            {
                DefenseTheWay(EM_Shield + attackMiddle + damagePermit, herosMiddle + herosFreedom, TrajectoryType.Middle);
            }
            if (herosFreedom > ER_Shield + damagePermit)
            {
                DefenseTheWay(ER_Shield + damagePermit, herosFreedom, TrajectoryType.Right);
            }
            if (herosFreedom > EL_Shield + damagePermit)
            {
                DefenseTheWay(EL_Shield + damagePermit, herosFreedom, TrajectoryType.Left);
            }

            if (EM_Refresh == true && (herosMiddle + herosFreedom > EM_Shield + attackMiddle))
            {
                DefenseTheWay(EM_Shield + attackMiddle,herosMiddle + herosFreedom,TrajectoryType.Middle);
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
        //在盾会被打爆的情况下护一下脸 ！AIStyle.CrazyDog
        //不同情况下选择护脸，[不护脸伤害太高]，[百分比]，[所剩生命值太低],[下回合防不住]
        if(aiStyle!=AIStyle.CrazyDog)
        {
            int damagePermit=2;
            if(herosMiddle+herosFreedom>EM_Shield+attackMiddle+damagePermit)
            {
                DefenseTheWay(EM_Shield + attackMiddle+damagePermit, herosMiddle + herosFreedom, TrajectoryType.Middle);
            }
            if(herosFreedom > ER_Shield+damagePermit)
            {
                DefenseTheWay(ER_Shield + damagePermit, herosFreedom, TrajectoryType.Right);
            }
            if (herosFreedom > EL_Shield + damagePermit)
            {
                DefenseTheWay(EL_Shield + damagePermit, herosFreedom, TrajectoryType.Left);
            }
        }
        if(aiStyle==AIStyle.Conservative)
        {
            int damagePermit = 1;
            if (herosMiddle + herosFreedom > EM_Shield + attackMiddle + damagePermit)
            {
                DefenseTheWay(EM_Shield + attackMiddle + damagePermit, herosMiddle + herosFreedom, TrajectoryType.Middle);
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

        //Berserker,用所有的自由来
        if(aiStyle==AIStyle.Berserker)
        {
            if (enemyHero.health + EM_Shield + attackMiddle
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
        //……


        //不要主动去打盾
        if (ShieldManager.Instance.M_DefenseMagic == null && ShieldManager.Instance.L_DefenseMagic != null && ShieldManager.Instance.R_DefenseMagic != null)
        {
            //Debug.Log("14");
            AttackManager.Instance.SetFreedomTrajectory(TrajectoryType.Middle, freedomList);
            return;
        }
        if (ShieldManager.Instance.M_DefenseMagic != null && ShieldManager.Instance.L_DefenseMagic == null && ShieldManager.Instance.R_DefenseMagic != null)
        {
            //Debug.Log("15");
            AttackManager.Instance.SetFreedomTrajectory(TrajectoryType.Left, freedomList);
            return;
        }
        if (ShieldManager.Instance.M_DefenseMagic != null && ShieldManager.Instance.L_DefenseMagic != null && ShieldManager.Instance.R_DefenseMagic == null)
        {
            //Debug.Log("16");
            AttackManager.Instance.SetFreedomTrajectory(TrajectoryType.Right, freedomList);
            return;
        }
        if (ShieldManager.Instance.M_DefenseMagic == null && ShieldManager.Instance.L_DefenseMagic == null && ShieldManager.Instance.R_DefenseMagic != null)
        {
            //Debug.Log("17");
            AttackManager.Instance.SetFreedomTrajectory(TrajectoryType.Left, freedomList);
            return;
        }
        if (ShieldManager.Instance.M_DefenseMagic == null && ShieldManager.Instance.L_DefenseMagic != null && ShieldManager.Instance.R_DefenseMagic == null)
        {
            //Debug.Log("18");
            AttackManager.Instance.SetFreedomTrajectory(TrajectoryType.Right, freedomList);
            return;
        }
        //优先破刷新
        if (ShieldManager.Instance.M_DefenseMagic != null && ShieldManager.Instance.M_DefenseMagic.HasEffect("Refresh") && (ShieldManager.Instance.M_DefenseMagic.MagicMax <= (attackFreedom + attackMiddle)))
        {
            //Debug.Log("4");
            AttackManager.Instance.SetFreedomTrajectory(TrajectoryType.Middle, freedomList);
            return;
        }
        if (ShieldManager.Instance.L_DefenseMagic != null && ShieldManager.Instance.L_DefenseMagic.HasEffect("Refresh") && (ShieldManager.Instance.L_DefenseMagic.MagicMax <= attackFreedom))
        {
            //Debug.Log("5");
            AttackManager.Instance.SetFreedomTrajectory(TrajectoryType.Left, freedomList);
            return;
        }
        if (ShieldManager.Instance.R_DefenseMagic != null && ShieldManager.Instance.R_DefenseMagic.HasEffect("Refresh") && (ShieldManager.Instance.R_DefenseMagic.MagicMax <= attackFreedom))
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
                    if (a.trajectory == TrajectoryType.Right && ShieldManager.Instance.R_DefenseMagic != null)
                    {
                        if (ShieldManager.Instance.L_DefenseMagic == null || ShieldManager.Instance.L_DefenseMagic != null && !ShieldManager.Instance.L_DefenseMagic.HasEffect("Refresh"))
                            a.trajectory = TrajectoryType.Left;
                    }
                    else if (a.trajectory == TrajectoryType.Left && ShieldManager.Instance.L_DefenseMagic != null)
                    {
                        if (ShieldManager.Instance.R_DefenseMagic == null || ShieldManager.Instance.R_DefenseMagic != null && !ShieldManager.Instance.R_DefenseMagic.HasEffect("Refresh"))
                            a.trajectory = TrajectoryType.Right;
                    }
                }
            }
        }

    }
}
