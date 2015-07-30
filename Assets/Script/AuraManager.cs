using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AuraManager : MonoBehaviour {
    public static AuraManager Instance;
    List<Card> auraList=new List<Card>();
    List<Card> enemyAuraList=new List<Card> ();
    List<Card> Etemplist = new List<Card>();
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
    public bool E24Heros = false;
    public bool E26_LeftHeros = false;
    public bool E26_RightHeros = false;
    List<Card> auraStartTurnList=new List<Card>();

    public int GetEnemyAuraCount()
    {
        return enemyAuraList.Count;
    }
    public int GetAuraCount()
    {
        return auraList.Count;
    }
    public int EGetCardPatternUsed(string ID)
    {
        foreach (Card card in enemyAuraList)
        {
            if (card.ID == ID)
            {
                return card.PatternUsed;
            }
        }
        foreach (Card card in Etemplist)
        {
            if (card.ID == ID)
            {
                return card.PatternUsed;
            }
        }
        return 0;
    }
    public void AddTempList(Card card)
    {
        Etemplist.Add(card);
    }
    public bool InTempList(Card card)
    {
        return Etemplist.Contains(card);
    }
    public void ClearTempList()
    {
        Etemplist.Clear();
    }
    public int GetCardPatternUsed(string ID)
    {
        Card card= GetCardByID(ID, auraList,0);
        if(card!=null)
            return card.PatternUsed;
        return 0;
    }
    public void AddEnemyAura(Card aura)
    {
        enemyAuraList.Add(aura);
        enemyAuraText += AddAuraText(aura);
        EnemyAuraAffect(aura);
        if (AI.Instance.aiType == AIType.WeakAI)
        {
            AIAuraAffect(aura);
        }
    }
    public void AddAura(Card aura)
    {
        //Debug.Log("AddAura");
        auraList.Add(aura);
        CardBackLightHide();
        AuraAffect(aura);
        auraText += AddAuraText(aura);
        GuideText.Instance.ReturnText(1001);//"AuraManager"
    }
    public void Restart()
    {
        EnergyManager.Instance.HeroMagicCircle.ChangeLine += this.AuraSustain;
    }
    public void ShowEnemyAuraCard()
    {
        DrawCard.Instance.ShowAuraCard(enemyAuraList);
    }
    public void HideEnemyAuraCard()
    {
        foreach(Card c in enemyAuraList)
        {
            c.transform.parent = null;
            c.gameObject.SetActive(false);
        }
    }
    public void AuraStartTurnEffect()
    {
        //Debug.Log("AuraStartTurnEffect");
        if (auraStartTurnList.Count <= 0)
            return;
        foreach (Card card in auraStartTurnList)
        {
            card.CreateAttackMagic();
        }
    }
    public void AuraSustain()//改变连线后，检查aura是否还生效
    {
        //  Debug.Log("sustain");
        auraText = "";
        for (int i = auraList.Count - 1; i >= 0; i--)
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
                if (LevelManager.Instance.IsOnline)
                {
                    Client.Instance.OnDestroyAura(auraList[i].ID, auraList[i].PatternUsed);
                }
                RemoveAuraEffect(auraList[i]);
                EnergyManager.Instance.StartTurn += auraList[i].Return;
                auraList.RemoveAt(i);
            }
            else
            {
                auraText += AddAuraText(auraList[i]);
            }
        }
        //  Debug.Log(auraText);
        GuideText.Instance.ReturnText(1000);
        // Debug.Log("AuraSustain:"+AuraManager.Instance.auraText);
    }
    public void DestroyEnemyAura(string ID, int PatternUsed)
    {
        Card card = GetCardByID(ID, enemyAuraList,PatternUsed);
        RemoveEnemyAuraAffect(card);
        enemyAuraList.Remove(card);
    }
    Card GetCardByID(string ID, List<Card> list,int PatternUsed)
    {
        foreach(Card card in list)
        {
            if(card.ID==ID)
            {
                if(card.PatternUsed==PatternUsed)
                    return card;
                if (PatternUsed == 0)
                    return card;
            }
        }
        return null;
    }
    void ENumberInitial()
    {
        E02Heros = false;
        E02Enemy = false;
        E24Heros = false;
        E28Heros = false;
        E28Enemy = false;
        E26_LeftHeros = false;
        E26_RightHeros = false;
    }
	void Start () {
        AuraManager.Instance = this;
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
    void AIAuraAffect(Card aura)
    {
        switch(aura.ID)
        {
            case"E10":
                 foreach (Transform card in AI.Instance.EnemyHand.transform)
                {
                    Card cardScript = card.GetComponent<Card>();
                    cardScript.UpdateEffectText();
                    //AuraAffect(aura, cardScript);
                }
                break;
        }
    }
    void RemoveAIAuraAffect(Card aura)
    {
        switch (aura.ID)
        {
            case "E10":
                foreach (Transform card in AI.Instance.EnemyHand.transform)
                {
                    Card cardScript = card.GetComponent<Card>();
                    cardScript.UpdateEffectText();
                    //AuraAffect(aura, cardScript);
                }
                break;
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
            case "E10":
                enemyCostMinus += 1;
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
            case "E24"://第二张回手
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
                enemyCostMinus -= 1;
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
            case "E24":
                break;
            case "E26":
                break;
            case "E28":
                E28Enemy = false;
                break;
        }
    }
    void CardBackLightHide()//手牌中相同ID的牌是否可用
    {
        foreach (Transform card in DrawCard.Instance.HandZone.transform)
        {
            Card cardScript = card.GetComponent<Card>();
            cardScript.cardMoving.BacklightShowHide();
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
            case "E24":
                E24Heros = true;
                break;
            case "E26": 
                if (aura.PatternUsed == 1)
                {
                    E26_LeftHeros = true;
                }
                if (aura.PatternUsed==2)
                {
                    E26_RightHeros = true;
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
                        if (attackMagic.trajectory == TrajectoryType.Left && aura.PatternUsed == 1 || attackMagic.trajectory == TrajectoryType.Right && aura.PatternUsed == 2)
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
            case "E24":
                E24Heros = false;
                break;
            case "E26": 
                if (aura.PatternUsed == 1)
                {
                    E26_LeftHeros = false;
                }
                if (aura.PatternUsed == 2)
                {
                    E26_RightHeros = false;
                }
                foreach(AttackMagic attackMagic in AttackManager.Instance.attackMagicList)
                {
                    if(attackMagic.GetEffectValue("Freedom")==0 && E02Heros==false)
                    {
                        if (attackMagic.trajectory == TrajectoryType.Left && aura.PatternUsed == 1 || attackMagic.trajectory == TrajectoryType.Right && aura.PatternUsed == 2)
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
        auraList.Clear();
        enemyAuraList.Clear();
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
        ENumberInitial();

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
            AttackMagic a = AttackManager.Instance.GetRandomAtk(false);
            if (a == null)
                return;
            a.magicValuePlus += 1;
            a.UpdateMagicValue();
            a.magicCircle.ExpendShrink();
            return;
        }
    }
    public void EnemyE06(int ID)
    {
        if(LevelManager.Instance.IsOnline)
        {
            AttackMagic a = AttackManager.Instance.FindEnemyByID(ID);
            if (a == null)
                return;
            a.magicValuePlus += 1;
            a.UpdateMagicValue();
            a.magicCircle.ExpendShrink();
            return;
        }
    }
    public void E06()
    {
        AttackMagic a=AttackManager.Instance.GetRandomAtk(true);
        if (a == null)
            return;
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
