using UnityEngine;
using System.Collections;

public class DefenseMagic : Magic{
    public int defenseID = -1;
    public int enemyDefenseID = -1;
    public int MagicMax;
    UILabel UIValue, UIdamage;
    MagicCircle magicCircle;
//    static int test = 0;
    GameObject WeakenObj=null;
    void Start()
    {
        EnergyManager.Instance.StartTurn += this.StartTurn;
        EnergyManager.Instance.TurnsEnd += this.TurnsEnd;
        UIValue = transform.FindChild("Value").GetComponent<UILabel>();
        UIValue.text = magicValue.ToString();
        magicCircle = gameObject.AddComponent<MagicCircle>();

        //AddEffect("Refresh", 2);
        MagicCircleStart();
        //if (test == 0)//||test==1 ||test==2
        // AddEffect("Weaken", 1);
        //if (test == 1)
        //    AddEffect("Entrench", 3);
        //if (test == 2)
        //    AddEffect("Refresh", 1);
        //test++;
    }
    void MagicCircleStart()
    {
        if (Effect.ContainsKey("Entrench"))
        {
            magicCircle.ShrinkStart();
            magicCircle.Type = StartType.Enlarge;
            return;
        }
        if (Effect.ContainsKey("Refresh"))
        {
            magicCircle.Type = StartType.Rotate;
            magicCircle.RotateStart();
            return;
        }
        {
            magicCircle.Type = StartType.Shrink;
            magicCircle.EnlargeStart();
            return;
        }
    }
    void Update()
    {
        if (choosed && Input.GetMouseButton(1) && !EnergyManager.Instance.myTurnsEnd && isHeros)
        {
            if (Effect.ContainsKey("Entrench"))
            {
                if (EnergyManager.Instance.MinusEnergy(1))
                {
                    if (LevelManager.Instance.IsOnline)
                    {
                        Client.Instance.OnEntrench(defenseID);
                    }
                    magicValue += Effect["Entrench"];
                    refresh = true;
                }
            }

        }
        if(refresh == true)
        {
            Refresh();
            refresh = false;
        }
        if(Effect.ContainsKey("Weaken")&& WeakenObj==null)
        {
            GameObject temp = (GameObject)Resources.Load("ParticleSystem/Weaken");
            WeakenObj = (GameObject)Instantiate(temp, transform.position, transform.rotation);
//            Destroy(temp);
            WeakenObj.transform.parent = transform;
        }
        if(WeakenObj!=null && !Effect.ContainsKey("Weaken"))
        {
            Destroy(WeakenObj);
            WeakenObj = null;
        }
    }
    public void BeHit(int damage)
    {
        if(Effect.ContainsKey("Weaken"))
        {
            damage -= Effect["Weaken"];
            magicCircle.RotateFast(Effect["Weaken"] * 8);
        }
        else
        {
            
        }
        if(damage>0)
        {
            magicValue -= damage;
            magicCircle.isHeros = this.isHeros;
            magicCircle.Shake();
        }
    }
    public override void Refresh()
    {
        
        if (magicValue <= 0)
        {
            UIValue.text = "";
            Destroy();
        }
        UIValue.text = magicValue.ToString();
    }
   

    public override void Destroy()
    {
        ShieldManager.Instance.defenseMagicList.Remove(this);
        EnergyManager.Instance.StartTurn -= this.StartTurn;
        EnergyManager.Instance.TurnsEnd -= this.TurnsEnd;
        base.Destroy();
    }
    public void EntrenchMagic()
    {
        magicValue += Effect["Entrench"];
        refresh = true;
    }
    void StartTurn()
    {
        magicCircle.PositionZero();
        if(Effect.ContainsKey("Refresh"))
        {
            if (magicValue < MagicMax)
            {
                magicValue = MagicMax;
                magicCircle.Refresh();
                refresh = true;
            }
        }
    }
    void TurnsEnd()
    {

    }
    new void OnMouseEnter()
    {
        explainLabel.text =
            "【" + name + "】" + "(" +
            magicValue.ToString()+"/"+MagicMax.ToString() + ")\n";
        base.OnMouseEnter();
    }
  
}
