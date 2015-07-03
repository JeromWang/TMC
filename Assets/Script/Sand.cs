using UnityEngine;
using System.Collections;

public class Sand : MonoBehaviour {
    public MovieTexture PurpleSandClock;
    public MovieTexture GreenSandClock;
    public MovieTexture SandClockTurning;
    UITexture uiTexture;
    bool myTurnsEnd = false;
	// Use this for initialization
    void Awake()
    {
        uiTexture = transform.GetComponent<UITexture>();
        PurpleSandClock = (MovieTexture)Resources.Load("shalou");
        GreenSandClock = (MovieTexture)Resources.Load("shalougreen");
        SandClockTurning = (MovieTexture)Resources.Load("shalouxuan");
        EnergyManager.Instance.StartTurn += this.StartTurn;
        EnergyManager.Instance.TurnsEnd += this.TurnsEnd;
        uiTexture.mainTexture = null;
    }
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    void OnMouseDown()
    {
        if(!myTurnsEnd)
        {
            PurpleSandClock.Stop();
            uiTexture.mainTexture = GreenSandClock;
            myTurnsEnd = true;
            StartCoroutine(EnergyManager.Instance.End());
        }
    }
    public void StartTurn()
    {
        GreenSandClock.Stop();
        uiTexture.mainTexture = PurpleSandClock;
        PurpleSandClock.Play();
        myTurnsEnd = false;
    }
    public void TurnsEnd()
    {
        GreenSandClock.Play();
    }
}
