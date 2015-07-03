using UnityEngine;
using System.Collections;

public class SandClock : MonoBehaviour
{
    Animation animation1, animation2;
    GameObject shalou, shalouGreen;
    bool isCrystal = false;
    bool isAudio = false;
    float time = 0;
    // Use this for initialization
    void Start()
    {
        shalou = transform.FindChild("shalou").gameObject;
        shalouGreen = transform.FindChild("shalouGreen").gameObject;
        animation1 = shalou.GetComponent<Animation>();
        animation2 = shalouGreen.GetComponent<Animation>();
        EnergyManager.Instance.StartTurn += this.StartTurn;
        EnergyManager.Instance.TurnsEnd += this.TurnsEnd;
        gameObject.SetActive(false);
    }

    void OnMouseDown()
    {
        if (!(EnergyManager.Instance.roundCount <= 6 && EnergyManager.Instance.totalEnergy < EnergyManager.Instance.roundCount && EnergyManager.Instance.accessibleCrystal!=0)  || isCrystal)
        {
            if (!EnergyManager.Instance.myTurnsEnd)
            {
                
                shalou.SetActive(false);
                shalouGreen.SetActive(true);
                animation2.Play(); shalouGreen.audio.Play();
                EnergyManager.Instance.myTurnsEnd = true;
                time = 0;
                if (LevelManager.Instance.IsOnline)
                {
                    Client.Instance.OnEndRound();
                    if (EnergyManager.Instance.enemyTurnsEnd)
                    {
                        StartCoroutine(EnergyManager.Instance.End());
                    }
                }
                else
                    StartCoroutine(EnergyManager.Instance.End());
            }
        }
        else
        {
            GuideText.Instance.ReturnText("HaveCrystal");
            isCrystal = true;
        }

    }
    public void StartTurn()
    {
        
        gameObject.SetActive(true);
        shalou.SetActive(true);
        shalouGreen.SetActive(false);
        shalou.audio.Play();
        animation1.Play();
        EnergyManager.Instance.myTurnsEnd = false;
        time = 0;
        isAudio = false;
        isCrystal = false;
        GuideText.Instance.StartTurn();
    }
    public void TurnsEnd()
    {
        //GuideText.Instance.ReturnText(0);
    }
    void Update()
    {
//        if(EnergyManager.Instance!=null)
//        {
//            if (EnergyManager.Instance.myTurnsEnd == false)
//            {
//                time += Time.deltaTime;
//               // Debug.Log(time.ToString());
//            }
                
//        }
        
//        if(time>30f && !isAudio)
//        {
//            SEPlayer.Instance.Play();
//            isAudio = true;
////            Debug.Log("play");
//        }
    }
  
}
