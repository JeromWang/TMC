using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AttackManager: MonoBehaviour
{
    public  static AttackManager Instance;
    public int EnemyAttackID;
    public int AttackID;
    public int HerosFireGONum;
    public int EnemyFireGONum;
    public int MaxFireGoNum;
    public  int WaitNum = -1;
    public  int SetNum = 0;
    public bool[] WaitPos ;//{ false, false, false, false, false, false, false, false, false, false, false, false };
    public  int EnermyWaitNum = -1;
    public  int EnermySetNum = 0;
    public bool[] EnermyWaitPos ; //{ false, false, false, false, false, false, false, false, false, false, false, false };
    public List<AttackMagic> attackMagicList =new List<AttackMagic>();
    public List<AttackMagic> enemyAttackMagicList = new List<AttackMagic>();
    Hero hero;
	// Use this for initialization
	void Start () {
        AttackManager.Instance = this;
        EnemyAttackID = 0;
        AttackID = 0;
        hero = GameObject.Find("Hero").GetComponent<Hero>();
	}
    public void Restart()
    {
        WaitPos = new bool[12];
        EnermyWaitPos = new bool[12];
        for (int i = 0; i < 12; i++)
        {
           // //Debug.Log(i.ToString());
            EnermyWaitPos[i] = false;
            WaitPos[i] = false;
        }
        SetNum = 0;
        WaitNum = -1;
        EnermySetNum = 0;
        EnermyWaitNum = -1;

        EnergyManager.Instance.StartTurn += this.AIAttack;
    }
    //public void ChangePos()
    //{
    //    for (int i = 0; i < attackMagicList.Count - 1; i++)
    //    {

    //    }
    //}
	public void EndGame()
    {
        for(int i=attackMagicList.Count-1;i>=0;i--)
        {
            attackMagicList[i].Destroy();
        }
        for (int i = enemyAttackMagicList.Count - 1; i >= 0; i--)
        {
            enemyAttackMagicList[i].Destroy();
        }
        AttackID = 0;
        EnemyAttackID = 0;
        attackMagicList.Clear();
        enemyAttackMagicList.Clear();
    }
    
    public bool AllFireDestroy()
    {
        foreach(AttackMagic a in attackMagicList)
        {
            if (a.waitToFire == 0)
                return false;
        }
        foreach (AttackMagic a in enemyAttackMagicList)
        {
            if (a.waitToFire == 0)
                return false;
        }
        return true;
    }
    public void AIAttack()
    {
        //Debug.Log("Attack");
        //如果可以斩杀，就尽量斩杀
        //不要主动去打盾
        //否则有刷新盾的优先打爆
        //没有刷新盾或不能打爆，优先造成血量伤害
        //都不能打爆的也绝不打刷新盾
        int attackMiddle = 0,attackFreedom=0;
        int attackHeros = 0;
        foreach (AttackMagic a in enemyAttackMagicList)
        {
            if (a.waitToFire <= 1 )
            {
                if( a.HasEffect("Freedom"))
                {
                    attackFreedom += a.magicValue;
                }
                else
                {
                    attackMiddle += a.magicValue;
                }
            }
        }
        foreach (AttackMagic a in attackMagicList)
        {
            if (a.waitToFire <= 1)
            {
                attackHeros += a.magicValue;
            }
        }
        if(AuraManager.Instance.E02Enemy==true)
        {
            attackFreedom += attackMiddle;
            attackMiddle = 0;
        }
        if (attackFreedom == 0)
        {
            //Debug.Log("0");
            return;
        }
            
        //如果中心防御和血量的和<= 斩杀
        if (hero.health + (ShieldManager.Instance.M_DefenseMagic!=null?ShieldManager.Instance.M_DefenseMagic.magicValue:0)+attackHeros
            <=(attackMiddle+attackFreedom))
        {
            //Debug.Log("1");
            SetFreedomTrajectory(0);
        }
        else if((hero.health+ (ShieldManager.Instance.L_DefenseMagic!=null?ShieldManager.Instance.L_DefenseMagic.magicValue:0))
            <=attackFreedom)
        {
            //Debug.Log("2");
            SetFreedomTrajectory(-1);
        }
        else if ((hero.health + (ShieldManager.Instance.R_DefenseMagic != null ? ShieldManager.Instance.R_DefenseMagic.magicValue : 0))
            <= attackFreedom)
        {
            //Debug.Log("3");
            SetFreedomTrajectory(1);
        }
        //不要主动去打盾
        else if (ShieldManager.Instance.M_DefenseMagic == null && ShieldManager.Instance.L_DefenseMagic != null && ShieldManager.Instance.R_DefenseMagic != null)
        {
            //Debug.Log("14");
            SetFreedomTrajectory(0);
        }
        else if (ShieldManager.Instance.M_DefenseMagic != null && ShieldManager.Instance.L_DefenseMagic == null && ShieldManager.Instance.R_DefenseMagic != null)
        {
            //Debug.Log("15");
            SetFreedomTrajectory(-1);
        }
        else if (ShieldManager.Instance.M_DefenseMagic != null && ShieldManager.Instance.L_DefenseMagic != null && ShieldManager.Instance.R_DefenseMagic == null)
        {
            //Debug.Log("16");
            SetFreedomTrajectory(1);
        }
        else if (ShieldManager.Instance.M_DefenseMagic == null && ShieldManager.Instance.L_DefenseMagic == null && ShieldManager.Instance.R_DefenseMagic != null)
        {
            //Debug.Log("17");
            SetFreedomTrajectory(-1);
        }
        else if (ShieldManager.Instance.M_DefenseMagic == null && ShieldManager.Instance.L_DefenseMagic != null && ShieldManager.Instance.R_DefenseMagic == null)
        {
            //Debug.Log("18");
            SetFreedomTrajectory(1);
        }
        //优先破刷新
        else if (ShieldManager.Instance.M_DefenseMagic != null && ShieldManager.Instance.M_DefenseMagic.HasEffect("Refresh") && (ShieldManager.Instance.M_DefenseMagic.MagicMax <= (attackFreedom + attackMiddle)))
        {
            //Debug.Log("4");
            SetFreedomTrajectory(0);
        }
        else if (ShieldManager.Instance.L_DefenseMagic != null && ShieldManager.Instance.L_DefenseMagic.HasEffect("Refresh") && (ShieldManager.Instance.L_DefenseMagic.MagicMax <= attackFreedom))
        {
            //Debug.Log("5");
            SetFreedomTrajectory(-1);
        }
        else if (ShieldManager.Instance.R_DefenseMagic != null && ShieldManager.Instance.R_DefenseMagic.HasEffect("Refresh") && (ShieldManager.Instance.R_DefenseMagic.MagicMax <= attackFreedom))
        {
            //Debug.Log("6");
            SetFreedomTrajectory(1);
        }
        else
        {
            //Debug.Log("7");
            foreach (AttackMagic a in enemyAttackMagicList)
            {
                if (a.waitToFire <= 1 && a.HasEffect("Freedom") || AuraManager.Instance.E02Enemy==true)
                {
                    for (; a.trajectory == 0; )
                    {
                        a.trajectory = Random.Range(-1, 2);
                        if (a.trajectory == 1 && ShieldManager.Instance.R_DefenseMagic != null)
                        {
                            if (ShieldManager.Instance.L_DefenseMagic == null || ShieldManager.Instance.L_DefenseMagic != null && !ShieldManager.Instance.L_DefenseMagic.HasEffect("Refresh"))
                                 a.trajectory = -1;
                        }
                        else if(a.trajectory == -1 && ShieldManager.Instance.L_DefenseMagic != null)
                        {
                            if (ShieldManager.Instance.R_DefenseMagic == null || ShieldManager.Instance.R_DefenseMagic != null && !ShieldManager.Instance.R_DefenseMagic.HasEffect("Refresh"))
                            a.trajectory = 1;
                        }
                    }
                }
            }
        }

    }
    void SetFreedomTrajectory(int t)
    {
        if(AuraManager.Instance.E02Enemy==true)
        {
            foreach (AttackMagic a in enemyAttackMagicList)
            {
                a.trajectory = t;
            }
        }
        foreach (AttackMagic a in enemyAttackMagicList)
        {
            if (a.waitToFire <= 1 && a.HasEffect("Freedom"))
            {
                a.trajectory = t;
            }
        }
    }
    public int SetFireTime()
    {
        HerosFireGONum = 0;
        EnemyFireGONum = 0;
        MaxFireGoNum = 0;
        foreach(AttackMagic a in attackMagicList)
        {
            if (a.waitToFire == 0)
            {
                HerosFireGONum ++;
            }
        }
        foreach (AttackMagic a in enemyAttackMagicList)
        {
            if (a.waitToFire == 0)
            {
                EnemyFireGONum++;
            }
        }
        MaxFireGoNum = Mathf.Max(HerosFireGONum+1, EnemyFireGONum+1);
        return MaxFireGoNum;
    }
}
