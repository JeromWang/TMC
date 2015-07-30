using UnityEngine;
using System.Collections;

public class PatternChose : MonoBehaviour
{
    public Card card;
    public GameObject otherPattern;
    public int pattern;

    UILabel NameLabel;
    UILabel TypeLabel;
    static string[] TypeString = { "", "攻击", "防御", "治疗", "结界" };
    UILabel EffectLabel;
    string ID;
    public GameObject backLight;
    public bool canUse=true;
    public void CardCopy(Card card)
    {
        this.card = card;
        if (NameLabel == null)
            Debug.Log("null");
        ID = card.ID;
        NameLabel.text = card.name;
        //TypeLabel.text = card.typeText;
        EffectLabel.text = card.effectText;

        if(pattern==1)
        {
            (transform.FindChild("Texture").GetComponent<UITexture>()).mainTexture = (Texture)Resources.Load("CardTexture/" + ID);
        }
        else if(pattern==2)
        {
            (transform.FindChild("Texture").GetComponent<UITexture>()).mainTexture = (Texture)Resources.Load("CardTexture/" + ID+"_2");
        }
    }
    void OnMouseDown()
    {
        if (canUse)
        {
            card.PatternUsed = pattern; 
            backLight.SetActive(true);
            gameObject.SetActive(false);
            otherPattern.SetActive(false);
        }
    }
	// Use this for initialization
	void Awake () {
        NameLabel = transform.Find("Name").GetComponent<UILabel>();
        TypeLabel = transform.Find("Type").GetComponent<UILabel>();
        EffectLabel = transform.Find("Effect").GetComponent<UILabel>();
        backLight = transform.Find("BackLight").gameObject;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
