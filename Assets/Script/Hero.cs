using UnityEngine;
using System.Collections;

public class Hero : MonoBehaviour {
	
    UILabel UIhealth, UIdamage;
    public bool isHero;
    public int health = 10;
    public int damage = 0;//与其他脚本交互用
    float time;
	// Use this for initialization
	void Awake () {
        UIhealth = transform.FindChild("Health").GetComponent<UILabel>();
        UIdamage = transform.FindChild("Damage").GetComponent<UILabel>();
        UIhealth.text = health.ToString();
	}
    public void Restart()
    {
        health = 10;
        UIhealth.text = health.ToString();
    }
    void Update()
    {

        if (damage != 0)
        {
            StartCoroutine(Damage(damage));
            damage = 0;
        }
    }
    void OnMouseEnter()
    {
        if (!isHero && EnergyManager.Instance.roundCount>0 )
        {
            GuideText.Instance.ReturnText("EnemyAura");
            time = 0;
        }
    }
    void OnMouseOver()
    {
        if (!isHero && EnergyManager.Instance.roundCount > 0)
        {
            time += Time.deltaTime;
            if(time>0.5f && EnergyManager.Instance.turnsEnd==false)
            {
                AuraManager.Instance.ShowEnemyAuraCard();
            }
        }
    }
    
    void OnMouseExit()
    {
        if (!isHero && EnergyManager.Instance.roundCount > 0)
        {
            GuideText.Instance.ReturnText();
            AuraManager.Instance.HideEnemyAuraCard();
        }
    }
    IEnumerator Damage(int da)
    {
        //Debug.Log(da.ToString());
        if (da > 0)
        {
            UIdamage.text = "-" + da.ToString();
        }
        else if(da<0)
        {
            UIdamage.text = "+" +Mathf.Abs(da).ToString();
        }
        health -= da;
        UIhealth.text = "";
        yield return new WaitForSeconds(1f);
        UIhealth.text = health.ToString();
        UIdamage.text = "";
    }
}
