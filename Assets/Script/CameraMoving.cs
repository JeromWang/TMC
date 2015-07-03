using UnityEngine;
using System.Collections;

public class CameraMoving : MonoBehaviour
{
    public GameObject crystal;
    public Vector3 position0;
    public Vector3 position1;
    public Vector3 position2;
    public Vector3 position3;
    public Vector3 p3_4;
    public Vector3 rotation0;
    public Vector3 rotation1;
    public Vector3 rotation2;

    public static CameraMoving Instance;
    public GameObject StartMenu;
    public GameObject Light;
    public int status = -1;//用于记录相机位置
    public bool cardMoving = false;
    public bool canMove;
    public bool startGame;
    float now;
    float count = 6f;
    public iTween itween;
    Vector3 vec;
    Transform sth;
    public bool isMoveWithSth = false;
    // Use this for initialization
    void Start()
    {
        //Debug.Log(transform.rotation.ToString());
        CameraMoving.Instance = this;
        crystal.SetActive(false);
    }
    public void CameraMovingEvent()
    {
        MagicCircleMananger.Instance.ChangeLine += this.ChangeLine;
        EnergyManager.Instance.ChangeEnergy += this.ChangeEnergy;
        EnergyManager.Instance.StartGame += this.StartGame;
        EnergyManager.Instance.StartTurn += this.StartTurn;
        EnergyManager.Instance.MyTurnEnd += this.MyTurnEnd;
        EnergyManager.Instance.TurnsEnd += this.TurnsEnd;
        EnergyManager.Instance.FireGo += this.FireGo;
        EnergyManager.Instance.MyCardUsed += this.MyCardUsed;
        EnergyManager.Instance.EnemyCardUsed += this.Null;
    }
    //防止事件调用为空
    public void Null()
    {

    }
    public void MyCardUsed() { }
    public void StartGame()
    {
        startGame = true;
    }
    public void FireGo()
    {

    }
    public void ChangeLine()
    { }
    public void ChangeEnergy()
    { }
    public void StartTurn()
    {
        //canMove = true;
    }
    public void MyTurnEnd() { }
    public void TurnsEnd()
    {
        Move(1);
        canMove = false;
    }


    public void EndGame()
    {
        startGame = false;
    }
    public void MoveAdd(Vector3 v)
    {
        iTween.MoveAdd(gameObject, v, 0.4f);
    }
    public void MoveTo(Vector3 p, Vector3 r,float time)
    {
        iTween.MoveTo(gameObject, p, time);
        iTween.RotateTo(gameObject, r, time);
        //Invoke("clearUpHand", 1.5f);
        //Destroy(this);
    }
    public void ShakeCamera(Vector3 v,float t)
    {
        itween= gameObject.GetComponent<iTween>();
        if(itween!=null)
        {
            //Debug.Log(itween.type + itween.method);
            if (itween.method == "position" && itween.type == "shake")
            {
               // itween.StopCoroutine("position");
                //iTween.Stop(gameObject);//gameObject, "shake"
                //Debug.Log("12");
                //Debug.Log(itween.type + itween.method);
                //iTween.ShakePosition(gameObject, v, t);
            }
        }
        iTween.ShakePosition(gameObject, v, t);
        itween = gameObject.GetComponent<iTween>();
  //      if (itween != null)
//            Debug.Log("shake");
    }
    public void MoveWithSth(Transform sth,Vector3 vec)
    {
        this.sth = sth;
        this.vec = vec;
        isMoveWithSth = true;
    }
    public void Move(int Type)
    {
        if(Type!=3)
        {
            //GameManager.Instance.CameraInUse = Type;
            status = Type;
        }
        
        switch(Type)
        {
            case -1://主菜单视角 
                Light.SetActive(false);
                StartMenu.SetActive(true);
                CrystalShow.Instance.ShowHide(false);
                MoveTo(position0, rotation0, 5);
                break;
            case 0://开始游戏，进入对战视角
                Light.SetActive(true);
                Light.light.intensity = 0.75f;
                CrystalShow.Instance.ShowHide(EnergyManager.Instance.CrystalSee() && false);
                MoveTo(position1, rotation1, 5);status =1;
                break;
            case 1://对战视角
                Light.light.intensity = 0.75f;
                CrystalShow.Instance.ShowHide(EnergyManager.Instance.CrystalSee() && false);
                MoveTo(position1, rotation1, 1f);
                break;
            case 2://连线视角 
                Light.light.intensity = 0.35f;
                CrystalShow.Instance.ShowHide(EnergyManager.Instance.CrystalSee() && true);
                MoveTo(position2, rotation2, 1f);
                break;
            case 3: //组卡视角
                Light.SetActive(true);
                Light.light.intensity = 0.6f;
                CrystalShow.Instance.ShowHide(false);
                MoveTo(position3, p3_4, 3f);
                break;
        }               
    }
    // Update is called once per frame
    void Update()
    {
        if (canMove && !cardMoving && startGame)//如果卡牌被放大、选中、clearUpHand就不能移动camera
        {
            if ((Input.GetKeyUp(KeyCode.Alpha1) || Input.GetAxis("Mouse ScrollWheel") < 0))
            {
                Move(1);
            }
            else if ((Input.GetKeyUp(KeyCode.Alpha2) || Input.GetAxis("Mouse ScrollWheel") > 0))
            {
                Move(2);          
                if(LevelManager.Instance.level==1 && LevelManager.Instance.key.Contains("MoveCrystal"))
                {
                    GuideText.Instance.ReturnText(18);
                    LevelManager.Instance.key.Remove("MoveCrystal");
                }
            }
        }
        if(isMoveWithSth)
        {
            if(sth!=null)
            {
                if (transform.position.z < 860)
                    transform.position = sth.position + vec;
                transform.LookAt(sth);
            }
        }
    }
}
