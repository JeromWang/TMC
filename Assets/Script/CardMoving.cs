using UnityEngine;
using System.Collections;

public class CardMoving : MonoBehaviour
{
	public Vector3 turnDegrees = new Vector3(30f, 30f, 30f);
    public float smooth = 10;
    public int index = 0;

	GameObject MainCamera;
	GameObject backlight;

    Vector3 screenSpace;
    Vector3 Destination;
    Vector3 Delta;
    bool choosed = false;
	public bool magnify= false;
    public bool isHeros ;
	Vector3 originalPosition;
    Card cardScript;
    
    DrawCard drawCard;
    UILabel explainLabel;
    Transform Parent;

    Quaternion rotationTemp;
    bool showBacklight = false;
    bool changeText = true;//MouseExit改变说明面板文字
    void Awake()
    {
        //Debug.Log("CardMovinng Awake"); 
        MainCamera = GameObject.Find("Main Camera");
        drawCard = MainCamera.GetComponent("DrawCard") as DrawCard;
        cardScript = transform.GetComponent("Card") as Card;

        Transform t = transform.FindChild("BackLight");
        if (t != null)
        {
            backlight = t.gameObject;
            backlight.SetActive(false);
        }
    }
    void Start()
    {
        // Debug.Log("CardMovinng Start");
        explainLabel = GameObject.Find("ExplainLabel").GetComponent<UILabel>();

        EnergyManager.Instance.HeroMagicCircle.ChangeLine += this.BacklightShowHide;
        EnergyManager.Instance.ChangeEnergy += this.BacklightShowHide;
        Parent = transform.parent; 
        BacklightShowHide();
        isHeros = cardScript.isHeros;
        //Debug.Log("cardMoving Start end");
    }

    void Update()
    {
        if (choosed && isHeros)//选中后移动
        {
            screenSpace = Camera.main.WorldToScreenPoint(transform.position);
            Destination = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenSpace.z));
            Destination = Vector3.Lerp(transform.position, Destination, Time.deltaTime * smooth);

            Delta = Destination - transform.position;
            transform.position = Destination;
            transform.localRotation = Quaternion.Euler(-Delta.y * turnDegrees.x, Delta.x * turnDegrees.y, -Delta.x * turnDegrees.z);
           // Debug.Log(Quaternion.Euler(-Delta.y * turnDegrees.x, Delta.x * turnDegrees.y, -Delta.x * turnDegrees.z));
			
		 transform.localScale = new Vector3(1f, 1f, 1f);
        }
        if (choosed && Input.GetMouseButton(1))//右键取消，回到原位
        {
            transform.parent = Parent;
            transform.Rotate(Vector3.forward, -8f);
            choosed = !choosed;
            drawCard.clearUpHand(1);
        }
    }
    void OnMouseDown()
    {
        if (isHeros && !EnergyManager.Instance.myTurnsEnd)
        {
            choosed = !choosed;
            if (choosed)//单击选中后，让卡牌跟着移动
            {
                ReSize();
                //Debug.Log("choosed");
                transform.parent = null;
                drawCard.clearUpHand();
            }
            else//第二次单击
            {
                //使用卡牌
                Accessible ac=cardScript.IsAccessible(isHeros);
                if (ac==Accessible.OK)//可以使用？
                {
                    //choosed = !choosed;
                    drawCard.clearUpHand();
                    CameraMoving.Instance.Move(1);
                    CameraMoving.Instance.canMove = false;
                    CameraMoving.Instance.cardMoving = false;
                    StartCoroutine(cardScript.Use());
                    GuideText.Instance.ReturnText();
                    //voke("clearUpHand", 3f);
                }
                else
                {
                    //choosed = !choosed;
                    transform.Rotate(Vector3.forward, -8f);
                    transform.parent = Parent;
                    drawCard.clearUpHand();
                    GuideText.Instance.ReturnText(ac.ToString());
                    changeText = false;
                    if(ac==Accessible.NeedSymmetryPattern)
                    {
                        LevelManager.Instance.pattern2.SetActive(true);
                        PatternChose p2 = LevelManager.Instance.pattern2.GetComponent<PatternChose>();
                        p2.CardCopy(cardScript);
                        p2.canUse = false;
                        Invoke("PatternChoseHide", 2f);
                    }
                }
            }
        }
    }
    void PatternChoseHide()
    {
        LevelManager.Instance.pattern2.SetActive(false);
    }
    void Magnify()//查看效果 放大，向前向上移动
    {
        rotationTemp = transform.rotation;
        if (CameraMoving.Instance.status == 1)
        {
            transform.rotation = new Quaternion(-0.5f, 0, 0, 0.9f);
        }
        else if (CameraMoving.Instance.status == 2)
        {
            transform.rotation = new Quaternion(0, 0, 0, 1f);
        }
        transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
        transform.position += transform.up * 0.2f;
        transform.position += (transform.forward * (0.08f + 0.01f * index) + transform.right * (-0.08f));
    }
    void ReSize()
    {
        CameraMoving.Instance.cardMoving = false;
        //Debug.Log("exit");
        transform.localScale = new Vector3(1f, 1f, 1f);
        transform.position -= transform.up * 0.2f;
        transform.position -= (transform.forward * (0.08f + 0.01f * index) + transform.right * (-0.08f));
        magnify = false;

        transform.rotation = rotationTemp;
        //drawCard.clearUpHand();
    }
    void OnMouseOver()
    {
        CameraMoving.Instance.cardMoving = true;
        if (!choosed && isHeros)
        {//Debug.Log("over");
            if (!magnify)//查看效果 放大，向前向上移动
            {
                Magnify();
                magnify = true;
            }            
        }
        //说明面板介绍
        if(changeText)
        explainLabel.text = "【"+cardScript.name+"】" + "\n" + cardScript.effectText + "\n" ;
    }
    void OnMouseExit()
    {
        //Debug.Log("Exit");
		if(!choosed && isHeros)//查看效果结束
		{
            ReSize();
		}
        if(changeText)
        {
            GuideText.Instance.ReturnText();
        }
        else
        {
            changeText = false;
        }
       
	}
    
    public void BacklightShowHide()
    {
        //Debug.Log("BacklightShowHide"+" "+transform.name);
        if (cardScript.IsAccessible(isHeros)==Accessible.OK && isHeros || showBacklight)
        {
            backlight.SetActive(true); //Debug.Log("BacklightShowHide True");
            Guide_BacklightShowHide();
        }
        else
        {
            backlight.SetActive(false);//Debug.Log("BacklightShowHide False");
        }
    }
    public void BacklightShowHide(bool b)
    {
        if(b)
        {
            backlight.SetActive(true);
            showBacklight = true;
        }
        else
        {
            backlight.SetActive(false);
            showBacklight = false;
        }
    }
    public void BeDestroy()
    {
        EnergyManager.Instance.HeroMagicCircle.ChangeLine -= this.BacklightShowHide;
        EnergyManager.Instance.ChangeEnergy -= this.BacklightShowHide;
        CameraMoving.Instance.cardMoving = false;
        Destroy(this);
    }
    void Guide_BacklightShowHide()
    {
        if (LevelManager.Instance.level == 3 && LevelManager.Instance.key.Contains("Pattern") && cardScript.ID == "C07")
        {
            GuideText.Instance.ReturnText(14);
            LevelManager.Instance.key.Remove("Pattern");
        }
        if (LevelManager.Instance.level == 3 && LevelManager.Instance.key.Contains("Pattern2") && cardScript.ID == "C02")
        {
            GuideText.Instance.ReturnText(14);
            LevelManager.Instance.key.Remove("Pattern2");
        }
    }
}
