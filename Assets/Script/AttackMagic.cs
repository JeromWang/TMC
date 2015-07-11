using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum TrajectoryType//enemy 的left和hero的一样，都是左边
{
    Left=-1,Middle=0,Right=1
}
public class AttackMagic : Magic
{
    public GameObject Explode;
    UILabel UIValue = null;
    public MagicCircle magicCircle;
    public int myWaitNum = 0;
    public int eWaitNum = 0;
    public static bool chooseTrajectory = false;
    public int waitToFire = -100;
    public TrajectoryType trajectory = TrajectoryType.Middle;
    public int attackID=-1;
    public int enemyAttackID=-1;
    public int magicValuePlus = 0;
    int oldMagicValue = 0;
    int cohensionMagicValue = 0;

    void Start()
    {
        EnergyManager.Instance.StartTurn += this.StartTurn;
        EnergyManager.Instance.FireGo += this.FireGo;
        oldMagicValue = magicValue;
        
        
        explainLabel = GameObject.Find("ExplainLabel").GetComponent<UILabel>();
        magicCircle = gameObject.AddComponent<MagicCircle>();
        UpdateTexture();
        magicCircle.EnlargeStart();
        magicCircle.Type = StartType.Shrink;
        UpdateMagicValue();
        //AddEffect("Cohension", 2);

        //关卡判断
        if (LevelManager.Instance.level == 1 && LevelManager.Instance.key.Contains("AttackMagic"))
        {
            GuideText.Instance.ReturnText(3);
            LevelManager.Instance.key.Remove("AttackMagic");
            StartCoroutine(Level1());
        }
    }
    //对特殊效果的法阵更新图案
    public void UpdateTexture()
    {
        CircleType circleType=CircleType.Normal;
        bool ripe = false;
        if(Effect.ContainsKey("Freedom") || isHeros && AuraManager.Instance.E02Heros==true
            || !isHeros && AuraManager.Instance.E02Enemy == true)
        {
            circleType = CircleType.Freedom;
        }
        if(waitToFire<=0)
        {
            ripe = true;
        }
        magicCircle.TextureChange(circleType, ripe);
    }
    public void UpdateMagicValue()
    {
        if (isHeros)
        {
            magicValue = oldMagicValue + magicValuePlus + cohensionMagicValue;
            if (oldMagicValue >= 4)
            {
                magicValue += AuraManager.Instance.largeAttackPlus;
            }
            if (oldMagicValue < 4)
            {
                magicValue +=AuraManager.Instance.smallAttackPlus;
            }
            UIValue.text = magicValue.ToString();
        }
        else 
        { 
            magicValue = oldMagicValue + magicValuePlus + cohensionMagicValue;
            if (oldMagicValue >= 4)
            {
                magicValue += AuraManager.Instance.enemyLargeAttackPlus;
            }
            if (oldMagicValue < 4)
            {
                magicValue += AuraManager.Instance.enemySmallAttackPlus;
            }
            UIValue.text = magicValue.ToString();
        }
    }
    IEnumerator Level1()
    {
        yield return new WaitForSeconds(15f);
        if (EnergyManager.Instance.roundCount == 1 && isHeros)
            GuideText.Instance.ReturnText(4);
    }
    public void FindUIValue()
    {
        UILabel ui = transform.FindChild("Value").GetComponent<UILabel>();
        if (ui != null)
        {
            UIValue = ui;
            UIValue.text = magicValue.ToString();
        }
    }
    public Vector3 GetWaitPos()
    {
        Vector3 po;
        float r;
        float delta1, delta2;
        if (isHeros)
        {
            r = 5.17f;
            if (myWaitNum % 2 == 0)
            {
                delta2 = r * Mathf.Cos(myWaitNum * 3.14f / 4 / 3);
                delta1 = Mathf.Pow((r * r - delta2 * delta2), 0.5f);
            }
            else
            {
                delta2 = r * Mathf.Cos((myWaitNum + 1) * 3.14f / 4 / 3);
                delta1 = -Mathf.Pow((r * r - delta2 * delta2), 0.5f);
            }
            po = new Vector3(delta1 + 1000f, 0.1f, delta2 + 850f);
            AttackManager.Instance.WaitPos[myWaitNum] = true;
        }
        else
        {
            r = 10f;
            if (eWaitNum % 2 == 0)
            {
                delta2 = r * Mathf.Cos(eWaitNum * 3.14f / 4 / 5);
                delta1 = Mathf.Pow((r * r - delta2 * delta2), 0.5f);
            }
            else
            {
                delta2 = r * Mathf.Cos((eWaitNum + 1) * 3.14f / 4 / 5);
                delta1 = -Mathf.Pow((r * r - delta2 * delta2), 0.5f);
            }
            po = new Vector3(delta1 + 1000f, 0.1f, -delta2 + 880f);
            AttackManager.Instance.EnermyWaitPos[eWaitNum] = true;
        }

        return po;
    }
    public bool crashed = false;
    public void GetTrajectory()
    {
        //Debug.Log("GetTrajectory");
        TrajectoryPosition.Instance.HideMyPosition();
        TrajectoryPosition.Instance.ShowPosition(0);
        if (Effect.ContainsKey("Freedom") || AuraManager.Instance.E02Heros==true)
        {
            TrajectoryPosition.Instance.ShowPosition(-1);
            TrajectoryPosition.Instance.ShowPosition(1);
        }
        else if (AuraManager.Instance.E26_LeftHeros)
        {
            TrajectoryPosition.Instance.ShowPosition(-1);
        }
        else if (AuraManager.Instance.E26_RightHeros)
        {
            TrajectoryPosition.Instance.ShowPosition(1);
        }
        else
        {
            Guide_OnMouseDown();
        }
        TrajectoryPosition.Instance.SetTrajectory += this.SetTrajectory;
        chooseTrajectory = true;
    }
    bool DefaultMiddle()
    {
        if (Effect.ContainsKey("Freedom") || AuraManager.Instance.E02Heros==true
            ||AuraManager.Instance.E26_LeftHeros ||AuraManager.Instance.E26_RightHeros)
        {
            return false;
        }
        return true;
    }
    void Guide_OnMouseDown()
    {
        if (LevelManager.Instance.level == 1)
        {
            if(waitToFire==1)
            {
                GuideText.Instance.ReturnText("WaitOneRound");
                return;
            }
            if(waitToFire==0)
            {
                GuideText.Instance.ReturnText("WaitRoundEnd");
                return;
            }
        }
        else
        {
            GuideText.Instance.ReturnText("DefaultMiddle");
        }
    }
    public void SetTrajectory()
    {
        //Debug.Log("SetTrajectory");
        trajectory = (TrajectoryType)TrajectoryPosition.Instance.ID;
        if (LevelManager.Instance.IsOnline)
        {
            int tempT=(int)trajectory;
            tempT=-tempT;
            Client.Instance.OnTrajectoryChange(attackID, (TrajectoryType)tempT);
        }
        TrajectoryPosition.Instance.HidePosition();
        TrajectoryPosition.Instance.SetTrajectory -= this.SetTrajectory;
        chooseTrajectory = false;

        if (LevelManager.Instance.level == 1 && LevelManager.Instance.key.Contains("MouseOver"))
        {
            GuideText.Instance.ReturnText(20);
            LevelManager.Instance.key.Remove("MouseOver");
        }
    }
    IEnumerator SetFireBall()
    {
        if (isHeros)
        {
            AttackManager.Instance.WaitNum--;
            AttackManager.Instance.WaitPos[myWaitNum] = false;
        }
        else
        {
            AttackManager.Instance.EnermyWaitNum--;
            AttackManager.Instance.EnermyWaitPos[eWaitNum] = false;
        }
        float time = 1f,waitTime=0.5f;//移到法阵中心的时间，飞出去的总时间
        //控制位置和时间、个数
        if (isHeros)
        {
            if (AttackManager.Instance.SetNum % 2 == 0)
            {
                iTween.MoveTo(gameObject, new Vector3(1000f + AttackManager.Instance.SetNum / 2 * 1.5f, 0.1f, 850f - AttackManager.Instance.SetNum / 2 * 1.5f), time);
            }
            else
            {
                iTween.MoveTo(gameObject, new Vector3(1000f - (AttackManager.Instance.SetNum + 1) / 2 * 1.5f, 0.1f, 850f - (AttackManager.Instance.SetNum + 1) / 2 * 1.5f), time);
            }
            myWaitNum = AttackManager.Instance.SetNum;
            AttackManager.Instance.SetNum++;
        }
        else
        {
            if (AttackManager.Instance.EnermySetNum % 2 == 0)
            {
                iTween.MoveTo(gameObject, new Vector3(1000f + AttackManager.Instance.EnermySetNum / 2 * 1.5f, 0.1f, 880f + AttackManager.Instance.EnermySetNum / 2 * 1.5f), time);
            }
            else
            {
                iTween.MoveTo(gameObject, new Vector3(1000f - (AttackManager.Instance.EnermySetNum + 1) / 2 * 1.5f, 0.1f, 880f + (AttackManager.Instance.EnermySetNum + 1) / 2 * 1.5f), time);
            }
            eWaitNum = AttackManager.Instance.EnermySetNum;
            AttackManager.Instance.EnermySetNum++;
            //对自由弹道的进行随机
            //if (!isHeros && Effect.ContainsKey("Freedom")&& !LevelManager.Instance.IsOnline)
            //{
            //    for (; trajectory == 0; )
            //        trajectory = Random.Range(-1, 2);
            //}
        }
        yield return new WaitForSeconds(time + (isHeros ? waitTime / AttackManager.Instance.MaxFireGoNum * myWaitNum : waitTime / AttackManager.Instance.MaxFireGoNum * eWaitNum));
        magicCircle.Expend();
        Quaternion qua = new Quaternion(-1, 0, 0, 0);
        GameObject Fireball;
        if (Effect.ContainsKey("Cohension"))
        {
            Fireball = (GameObject)Resources.Load("ParticleSystem/FireBall");
            magicCircle.Lager();
        }
        else if (magicValue >= 4)
            Fireball = (GameObject)Resources.Load("ParticleSystem/mask ball 16 big");
        else
            Fireball = (GameObject)Resources.Load("ParticleSystem/mask ball 16");//mask missile 11 、 Fire2
        GameObject Fire = (GameObject)Instantiate(Fireball, transform.position + new Vector3(0, 0.5f, 0f), qua);
        if (Effect.ContainsKey("Cohension"))
        {
            Fire.transform.position += new Vector3(0, 1.65f, 0);
            //Debug.Log("up");
        }
        if (isHeros)
            gameObject.layer = 10;
        else
            gameObject.layer = 11;
        Fire.transform.parent = transform;
        gameObject.GetComponent<BoxCollider>().size = new Vector3(0.8f, 0.8f, 1);
        gameObject.GetComponent<BoxCollider>().center = new Vector3(0, -0.4f, 0f);
        UIValue = null;
        yield return new WaitForSeconds(2f);

        
        magicCircle.Destroy();
        // Debug.Log(myWaitNum.ToString());
        yield return new WaitForSeconds(isHeros ? waitTime / AttackManager.Instance.MaxFireGoNum * myWaitNum : waitTime / AttackManager.Instance.MaxFireGoNum * eWaitNum);
        if (Effect.ContainsKey("Cohension"))
            Explode = (GameObject)Resources.Load("ParticleSystem/explosion");//
        else
            Explode = (GameObject)Resources.Load("ParticleSystem/spark 08");//explosion
        this.gameObject.AddComponent<FLY>();
        FLY fly = gameObject.GetComponent<FLY>();
        fly.trajectory = this.trajectory;
        audio.Play();
    }
    public void StartTurn()
    {
        waitToFire--;
        AttackManager.Instance.SetNum = 0;
        AttackManager.Instance.EnermySetNum = 0;
        if (isHeros)
        {
            for (int i = 0; i < myWaitNum; i++)
            {
                if (AttackManager.Instance.WaitPos[i] == false)
                {
                    AttackManager.Instance.WaitPos[myWaitNum] = false;
                    myWaitNum = i;
                    AttackManager.Instance.WaitPos[myWaitNum] = true;
                    iTween.MoveTo(gameObject, GetWaitPos(), 3f);
                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i < eWaitNum; i++)
            {
                if (AttackManager.Instance.EnermyWaitPos[i] == false)
                {
                    AttackManager.Instance.EnermyWaitPos[eWaitNum] = false;
                    eWaitNum = i;
                    AttackManager.Instance.EnermyWaitPos[eWaitNum] = true;
                    iTween.MoveTo(gameObject, GetWaitPos(), 3f);
                    break;
                }
            }
        }
        if (waitToFire <= 0)
        {
            UpdateTexture();
        }
    }

    
    public void FireGo()
    {
        if (waitToFire <= 0)
        {
            StartCoroutine(SetFireBall());
        }
    }
    public void Crash(Magic opponent)
    {
        //记录原始值
        int MyValue = magicValue;
        int OpponentValue = opponent.magicValue;
        if (opponent is DefenseMagic)//本身是攻击法术与防御法术相撞
        {
            //Debug.Log("attack defense crash");
            (opponent as DefenseMagic).BeHit(MyValue);
            magicValue -= OpponentValue+opponent.GetEffectValue("Weaken");
        }
        else if (opponent is AttackMagic)//本身是攻击法术与攻击法术相撞
        {
            if ((opponent as AttackMagic).crashed == false) //防止两处脚本都计算
            {
                opponent.magicValue -= MyValue;
                magicValue -= OpponentValue;
                crashed = true;
            }
            else
            {
                crashed = false;
            }
            
        }
    }
    public void Crash(Hero opponent)
    {
        // Debug.Log("attack hero crash");
        opponent.damage += magicValue;
        Destroy();
    }
    IEnumerator CohensionAnime()
    {
        GameObject gather;
        gather = (GameObject)Resources.Load("ParticleSystem/gather");
        gather = (GameObject)Instantiate(gather, transform.position + new Vector3(0, 0.5f, 0), transform.rotation);
        gather.SetActive(true);
        iTween.ShakePosition(GameObject.Find("Main Camera"), new Vector3(0.05f, 0.05f, 0.05f), 3f);
        yield return new WaitForSeconds(2f);
        Destroy(gather);
        gather = (GameObject)Resources.Load("ParticleSystem/gather2");
        gather = (GameObject)Instantiate(gather, transform.position + new Vector3(0, 0.5f, 0), transform.rotation);
        gather.SetActive(true);
        refresh = true;
        iTween.ShakePosition(GameObject.Find("Main Camera"), iTween.Hash("amount", new Vector3(0.1f, 0.1f, 0.1f), "time", 1f));
    }
    void Update()
    {
        if (choosed && Input.GetMouseButton(1) && !EnergyManager.Instance.myTurnsEnd && isHeros)
        {
            //Debug.Log("right");
            if (Effect.ContainsKey("Cohension"))
            {
                if (EnergyManager.Instance.MinusEnergy(1))
                {
                    //Debug.Log("minus");
                    if (LevelManager.Instance.IsOnline)
                    {
                        Client.Instance.OnCohesion(attackID);
                    }
                    CohereMagic();
                }
            }

        }
        if (refresh == true)
        {
            Refresh();
            refresh = false;
        }
    }
    public override void Refresh()
    {
        if (magicValue <= 0)
        {
            Destroy();
        }
        if (UIValue != null)
        {
            UIValue.text = magicValue.ToString();
        }
    }
    public void CohereMagic()
    {
        cohensionMagicValue += Effect["Cohension"];
        UpdateMagicValue();
        StartCoroutine(CohensionAnime());
    }
    public override void Destroy()
    {
        if(isHeros)
        {
            AttackManager.Instance.attackMagicList.Remove(this);
        }
        else
        {
            AttackManager.Instance.enemyAttackMagicList.Remove(this);
        }
        
        EnergyManager.Instance.FireGo -= this.FireGo;
        EnergyManager.Instance.StartTurn -= this.StartTurn;
        TrajectoryPosition.Instance.HidePosition();
        base.Destroy();
    }
    new void OnMouseEnter()
    {
        if (chooseTrajectory == false)
        {
            explainLabel.text =
            "【"+ name + "】" + "(" +
            (magicValue==oldMagicValue?magicValue.ToString():(oldMagicValue.ToString()+"+"+(magicValue-oldMagicValue).ToString()))
             + ")\n";
            base.OnMouseEnter();
            if(AuraManager.Instance.E02Heros==true && !Effect.ContainsKey("Freedom")&& isHeros)
            {
                explainLabel.text += "弹道自由\n";
            }
            if (AuraManager.Instance.E02Enemy == true && !Effect.ContainsKey("Freedom") && !isHeros)
            {
                explainLabel.text += "弹道自由\n";
            }
            explainLabel.text += "等待:" + waitToFire.ToString();
            if (isHeros)
                TrajectoryPosition.Instance.ShowMyTrajectory((int)trajectory);
        }
    }
    new void OnMouseExit()
    {
        base.OnMouseExit();
        if (!chooseTrajectory && isHeros)
        {
            TrajectoryPosition.Instance.HidePosition();
            TrajectoryPosition.Instance.HideMyPosition();
        }
        if(chooseTrajectory && DefaultMiddle())
        {
            Guide_OnMouseDown();
        }
    }
    void OnMouseDown()
    {
        if (!isHeros)
            return;
        if (chooseTrajectory == false && !EnergyManager.Instance.myTurnsEnd)
        {
            this.GetTrajectory();
            if (LevelManager.Instance.level == 1 && LevelManager.Instance.key.Contains("ShowTrajectory")&& Effect.ContainsKey("Freedom"))
            {
                GuideText.Instance.ReturnText(19);
                LevelManager.Instance.key.Remove("ShowTrajectory");
            }
        }
        else
        {
            TrajectoryPosition.Instance.HidePosition();
            TrajectoryPosition.Instance.SetTrajectory -= this.SetTrajectory;
            chooseTrajectory = false;
        }
    }

}
