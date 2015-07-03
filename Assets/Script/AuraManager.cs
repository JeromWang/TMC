﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AuraManager : MonoBehaviour {
    public static AuraManager Instance;
    public List<Card> auraList;
    public List<Card> enemyAuraList;
    public string auraText;
    public string enemyAuraText;
    public int costMinus = 0;
    public int enemyCostMinus = 0;
    public int healthPlus = 0;
    public int enemyHealthPlus = 0;
    public int smallAttackPlus = 0;
    public int largeAttackPlus = 0;
    public int enemySmallAttackPlus = 0;
    public int enemyLargeAttackPlus = 0;
    public bool E02Heros = false;
    public bool E02Enemy = false;
    public bool E28Heros = false;
    public bool E28Enemy = false;
    List<Card> auraStartTurnList=new List<Card>();
	// Use this for initialization
	void Start () {
        AuraManager.Instance = this;
	}
    public int GetCardPatternUsed(string ID)
    {
        foreach(Card card in auraList)
        {
            if(card.ID==ID)
            {
                return card.PatternUsed;
            }
        }
        return 0;
    }
    public void AddEnemyAura(Card aura)
    {
        enemyAuraList.Add(aura);
        enemyAuraText += AddAuraText(aura);
        EnemyAuraAffect(aura);
    }
    public void AddAura(Card aura)
    {
        //Debug.Log("AddAura");
        auraList.Add(aura);
        AuraAffect(aura);
        auraText+=AddAuraText(aura);
        GuideText.Instance.ReturnText(1001);//"AuraManager"
    }
    string AddAuraText(Card aura)
    {
        switch (aura.ID)
        {
            case "E26":
                return aura.PatternUsed == 1 ? "阴谋施法:左侧弹道自由\n" : "阴谋施法:右侧弹道自由\n";
            default: return (aura.name + ":" + aura.explainText + "\n");
        }
    }
    void EnemyAuraAffect(Card aura)
    {
        switch(aura.ID)
        {
            case "E01":
                auraStartTurnList.Add(aura);
                break;
            case "E02":
                E02Enemy = true;
                foreach (AttackMagic attackMagic in AttackManager.Instance.enemyAttackMagicList)
                {
                    attackMagic.UpdateTexture();
                }
                if (!LevelManager.Instance.IsOnline)
                {
                    AttackManager.Instance.AIAttack();
                }
                break;
            case "E03":
                enemyLargeAttackPlus += 3;
                foreach (AttackMagic a in AttackManager.Instance.enemyAttackMagicList)
                {
                    a.UpdateMagicValue();
                }
                break;
            case "E04":
                enemySmallAttackPlus += 1;
                foreach (AttackMagic a in AttackManager.Instance.enemyAttackMagicList)
                {
                    a.UpdateMagicValue();
                }
                break;
            case "E06":
                EnergyManager.Instance.EnemyCardUsed += this.EnemyE06;
                break;
            case "E09":
                EnergyManager.Instance.EnemyCardUsed += this.EnemyE09;
                break;
            case "E15":
                EnergyManager.Instance.StartTurn += this.EnemyE15;
                break;
            case "E17":
                auraStartTurnList.Add(aura);
                break;
            case "E21":
                enemyHealthPlus += 2;
                break;
            case "E26":
                break;
            case "E28":
                E28Enemy = true;
                break;
        }
    }
    void RemoveEnemyAuraAffect(Card aura)
    {
        switch (aura.ID)
        {
            case "E01":
                auraStartTurnList.Remove(aura);
                break;
            case "E02":
                E02Enemy = false;
                foreach (AttackMagic attackMagic in AttackManager.Instance.enemyAttackMagicList)
                {
                    attackMagic.UpdateTexture();
                }
                break;
            case "E03":
                enemyLargeAttackPlus -= 3;
                foreach (AttackMagic a in AttackManager.Instance.enemyAttackMagicList)
                {
                    a.UpdateMagicValue();
                }
                break;
            case "E04":
                enemySmallAttackPlus -= 1;
                foreach (AttackMagic a in AttackManager.Instance.enemyAttackMagicList)
                {
                    a.UpdateMagicValue();
                }
                break;
            case "E06":
                EnergyManager.Instance.EnemyCardUsed -= this.EnemyE06;
                break;
            case "E09":
                EnergyManager.Instance.EnemyCardUsed -= this.EnemyE09;
                break;
            case "E10":
                //偷懒没写
                break;
            case "E15":
                EnergyManager.Instance.StartTurn -= this.EnemyE15;
                break;
            case "E17":
                auraStartTurnList.Remove(aura);
                break;
            case "E21":
                enemyHealthPlus -= 2;
                break;
            case "E26":
                break;
            case "E28":
                E28Enemy = false;
                break;
        }
    }
    void AuraAffect(Card aura)//新的结界卡生效
    {
        switch(aura.ID)
        {
            case "E01":
                auraStartTurnList.Add(aura);
                break; 
            case "E02":
                E02Heros = true;
                foreach (AttackMagic a in AttackManager.Instance.attackMagicList)
                {
                    a.UpdateTexture();
                }
                break;
            case "E03":
                largeAttackPlus += 3;
                foreach(AttackMagic a in AttackManager.Instance.attackMagicList)
                {
                    a.UpdateMagicValue();
                }
                break;
            case "E04":
                smallAttackPlus += 1;
                foreach(AttackMagic a in AttackManager.Instance.attackMagicList)
                {
                    a.UpdateMagicValue();
                }
                break;
            case "E06":
                EnergyManager.Instance.MyCardUsed += this.E06;
                break;
            case "E09":
                EnergyManager.Instance.MyCardUsed += this.E09;
                break;
            case "E10":
                costMinus += 1;
                foreach (Transform card in DrawCard.Instance.HandZone.transform)
                {
                    Card cardScript = card.GetComponent<Card>();
                    cardScript.UpdateEffectText();
                    cardScript.cardMoving.BacklightShowHide();
                    //AuraAffect(aura, cardScript);
                }
                break;
            case "E15":
                EnergyManager.Instance.StartTurn += this.E15;
                break;
            case "E17":
                auraStartTurnList.Add(aura);
                break;
            case "E21":
                healthPlus += 2;
                break;
            case "E26": 
                string patternUsed=aura.PatternUsed.ToString();
                foreach(AttackMagic attackMagic in AttackManager.Instance.attackMagicList)
                {
                    attackMagic.AddEffect("E26" +patternUsed , 1);
                }
                break;
            case "E28":
                E28Heros = true;
                break;
            //case "E": break;
            //case "E": break;
            //case "E": break;
        }
    }
    void RemoveAuraEffect(Card aura)
    {
        switch (aura.ID)
        {
            case "E01":
                auraStartTurnList.Remove(aura);
                break;
            case "E02":
                E02Heros = false;
                foreach (AttackMagic attackMagic in AttackManager.Instance.attackMagicList)
                {
                    attackMagic.UpdateTexture();
                    if (attackMagic.GetEffectValue("Freedom") == 0 && E02Heros == false)
                    {
                        if (attackMagic.trajectory == -1 && aura.PatternUsed == 1 || attackMagic.trajectory == 1 && aura.PatternUsed == 2)
                        {
                            attackMagic.trajectory = 0;
                        }
                    }
                }
                break;
            case"E03":
                largeAttackPlus -= 3;
                foreach(AttackMagic a in AttackManager.Instance.attackMagicList)
                {
                    a.UpdateMagicValue();
                }
                break;
            case"E04":
                smallAttackPlus -= 1;
                foreach(AttackMagic a in AttackManager.Instance.attackMagicList)
                {
                    a.UpdateMagicValue();
                }
                break;
            case "E06":
                EnergyManager.Instance.MyCardUsed -= this.E06;
                break;
            case "E09":
                EnergyManager.Instance.MyCardUsed -= this.E09;
                break;
            case "E10":
                //foreach (Transform card in DrawCard.Instance.HandZone.transform)
                //{
                //    Card cardScript = card.GetComponent<Card>();
                //    cardScript.RemoveEffect(aura.ID+aura.PatternUsed);
                //    cardScript.UpdateEffectText();
                //}
                costMinus -= 1;
                foreach (Transform card in DrawCard.Instance.HandZone.transform)
                {
                    Card cardScript = card.GetComponent<Card>();
                    cardScript.UpdateEffectText();
                    cardScript.cardMoving.BacklightShowHide();
                }
                break;
            case "E15":
                EnergyManager.Instance.StartTurn -= this.E15;
                break;
            case "E17":
                auraStartTurnList.Remove(aura);
                break;
            case "E21":
                healthPlus -= 2;
                break;
            case "E26": 
                string patternUsed=aura.PatternUsed.ToString();
                foreach(AttackMagic attackMagic in AttackManager.Instance.attackMagicList)
                {
                    attackMagic.RemoveEffect("E26" +patternUsed);
                    if(attackMagic.GetEffectValue("Freedom")==0 && E02Heros==false)
                    {
                        if(attackMagic.trajectory==-1 && aura.PatternUsed==1 ||attackMagic.trajectory==1 && aura.PatternUsed==2)
                        {
                            attackMagic.trajectory = 0;
                        }
                    }
                }
                break;
            case "E28":
                E28Heros = false;
                break;
        }
    }
    public void EndGame()
    {
        foreach (Card card in auraList)
        {
            card.Destroy();
        }
        foreach (Card card in enemyAuraList)
        {
            card.Destroy();
        }
        auraStartTurnList.Clear();
        auraText = "";
        enemyAuraText = "";
        costMinus = 0;
        enemyCostMinus = 0;
        healthPlus = 0;
        enemyHealthPlus = 0;
        smallAttackPlus = 0;
        largeAttackPlus = 0;
        enemySmallAttackPlus = 0;
        enemyLargeAttackPlus = 0;
        E02Heros = false;
        E02Enemy = false;
        E28Heros = false;
        E28Enemy = false;
    }
    public void Restart()
    {
        MagicCircleMananger.Instance.ChangeLine += this.AuraSustain;
        EnergyManager.Instance.StartTurn += AuraStartTurnEffect;
    }
    public void AuraStartTurnEffect()
    {
        //Debug.Log("AuraStartTurnEffect");
        if (auraStartTurnList.Count <= 0)
            return;
        foreach(Card card in auraStartTurnList)
        {
            card.CreateAttackMagic();
        }
    }
    public void AuraSustain()//改变连线后，检查aura是否还生效
    {
      //  Debug.Log("sustain");
        auraText = "";
        for (int i = auraList.Count-1; i >=0; i--)
        {
           // Debug.Log(auraList[i].PatternUsed.ToString());
            bool succeed = false;
            if (auraList[i].PatternUsed == 1)
            {
                succeed = auraList[i].patternTrue(auraList[i].Pattern_1);
              //  Debug.Log(succeed.ToString());
            }
            if (auraList[i].PatternUsed == 2)
            {
                succeed = auraList[i].patternTrue(auraList[i].Pattern_2);
              //  Debug.Log(succeed.ToString());
            }
            if (succeed == false)
            {
                RemoveAuraEffect(auraList[i]);
                EnergyManager.Instance.StartTurn += auraList[i].Return;
                auraList.RemoveAt(i);
            }
            else
            {
                auraText+=AddAuraText(auraList[i]);
            }
        }
      //  Debug.Log(auraText);
        GuideText.Instance.ReturnText(1000);
       // Debug.Log("AuraSustain:"+AuraManager.Instance.auraText);
    }

    public void E15()//每回合治疗2
    {
        LevelManager.Instance.hero.damage -= 2+healthPlus;
    }
    public void EnemyE15()
    {
        LevelManager.Instance.enemyHero.damage -= 2 + enemyHealthPlus;
    }
    //public void E02()//每回合随机使一个魔法获得弹道自由
    //{
    //    if (AttackManager.Instance.attackMagicList.Count == 0)
    //    {
    //        return;
    //    }
    //    Random.seed = System.DateTime.Today.Millisecond;
    //    AttackMagic a = AttackManager.Instance.attackMagicList[Random.Range(0, AttackManager.Instance.attackMagicList.Count)];
    //    a.AddEffect("Freedom", 1);
    //    a.magicCircle.ExpendShrink();
    //}
    public void EnemyE06()
    {
        if(!LevelManager.Instance.IsOnline)//单机情况
        {
            if (AttackManager.Instance.enemyAttackMagicList.Count == 0)
            {
                return;
            }
            Random.seed = System.DateTime.Today.Millisecond;
            AttackMagic a = AttackManager.Instance.enemyAttackMagicList[Random.Range(0, AttackManager.Instance.enemyAttackMagicList.Count)];
            a.magicValuePlus += 1;
            a.UpdateMagicValue();
            a.magicCircle.ExpendShrink();
            return;
        }
    }
    public void E06()
    {
        if(AttackManager.Instance.attackMagicList.Count==0)
        {
            return;
        }
        Random.seed = System.DateTime.Today.Millisecond;
        AttackMagic a=AttackManager.Instance.attackMagicList[Random.Range(0, AttackManager.Instance.attackMagicList.Count)];
        a.magicValuePlus+=1;
        a.UpdateMagicValue();
        a.magicCircle.ExpendShrink();
    }
    public void EnemyE09()
    {
        LevelManager.Instance.enemyHero.damage -= 1 + AuraManager.Instance.enemyHealthPlus;
    }
    public void E09()
    {
        LevelManager.Instance.hero.damage -= 1 + AuraManager.Instance.healthPlus;
    }
}