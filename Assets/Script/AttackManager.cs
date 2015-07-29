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

    const int FireThisTurn = 0;
    const int FireNextTurn = 1;
	// Use this for initialization
	void Start () {
        AttackManager.Instance = this;
        EnemyAttackID = 0;
        AttackID = 0;
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

    }
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
            if (a.waitToFire == FireThisTurn)
                return false;
        }
        foreach (AttackMagic a in enemyAttackMagicList)
        {
            if (a.waitToFire == FireThisTurn)
                return false;
        }
        return true;
    }
    public AttackMagic RandomAtk(bool isHeros)
    {
        Random.seed = System.DateTime.Today.Millisecond;
        if(isHeros)
        {
            if (attackMagicList.Count == 0)
                return null;
            return attackMagicList[Random.Range(0, attackMagicList.Count)];
        }
        if (isHeros==false)
        {
            if (enemyAttackMagicList.Count == 0)
                return null;
            return enemyAttackMagicList[Random.Range(0, enemyAttackMagicList.Count)];
        }
        return null;
    }
    public AttackMagic FindEnemyByID(int ID)
    {
        foreach(AttackMagic a in enemyAttackMagicList)
        {
            if (a.enemyAttackID == ID)
                return a;
        }
        return null;
    }
    public int CaculateAttackMiddle()
    {
        int attackMiddle = 0;
        foreach (AttackMagic a in enemyAttackMagicList)
        {
            if (a.waitToFire == FireThisTurn)
            {
                if (!a.HasEffect("Freedom"))
                {
                    attackMiddle += a.magicValue;
                }
            }
        }
        return attackMiddle;
    }
    public int CaculateAttackFreedom()
    {
        int attackFreedom = 0;
        foreach (AttackMagic a in enemyAttackMagicList)
        {
            if (a.waitToFire == FireThisTurn)
            {
                if (a.HasEffect("Freedom"))
                {
                    attackFreedom += a.magicValue;
                }
            }
        }
        return attackFreedom;
    }
    public int HerosMiddle()
    {
        int attackMiddle = 0;
        foreach (AttackMagic a in attackMagicList)
        {
            if (a.waitToFire == FireThisTurn)
            {
                if (!a.HasEffect("Freedom"))
                {
                    attackMiddle += a.magicValue;
                }
            }
        }
        return attackMiddle;
    }
    public int HerosFreedom()
    {
        int attackFreedom = 0;
        foreach (AttackMagic a in attackMagicList)
        {
            if (a.waitToFire == FireThisTurn)
            {
                if (a.HasEffect("Freedom"))
                {
                    attackFreedom += a.magicValue;
                }
            }
        }
        return attackFreedom;
    }
    public List<AttackMagic> GetFireList()
    {
        List<AttackMagic> atkList = new List<AttackMagic>();
        foreach (AttackMagic a in enemyAttackMagicList)
        {
            if (a.waitToFire == FireThisTurn)
            {
                atkList.Add(a);
            }
        }
        return atkList;
    }
    public List<AttackMagic> GetFreedomList()
    {
        List<AttackMagic> atkList = new List<AttackMagic>();
        foreach (AttackMagic a in enemyAttackMagicList)
        {
            if (a.waitToFire == FireThisTurn)
            {
                if (a.HasEffect("Freedom"))
                {
                    atkList.Add(a);
                }
            }
        }
        return atkList;
    }
    
    public void SetFreedomTrajectory(TrajectoryType t,AttackMagic atk)
    {
        atk.trajectory = t;
    }
    public void SetFreedomTrajectory(TrajectoryType t, List<AttackMagic> enemyAttackMagicList)
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
            if (a.waitToFire == FireThisTurn && a.HasEffect("Freedom"))
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
