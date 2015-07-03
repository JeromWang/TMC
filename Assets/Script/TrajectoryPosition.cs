using UnityEngine;
using System.Collections;

public class TrajectoryPosition : MonoBehaviour {
    public static TrajectoryPosition Instance;
    public GameObject L_Trajectory_pos;
    public GameObject M_Trajectory_pos;
    public GameObject R_Trajectory_pos;
    public GameObject MyL_Trajectory_pos;
    public GameObject MyM_Trajectory_pos;
    public GameObject MyR_Trajectory_pos;
    public int ID;
    public delegate void SetTrajectoryEvent();
    public event SetTrajectoryEvent SetTrajectory;
	// Use this for initialization
	void Start () {
        TrajectoryPosition.Instance = this;
        HidePosition();
        HideMyPosition();
	}
    public void TrajectoryFinish()
    {
        SetTrajectory();
    }
    public void ShowPosition()
    {
        L_Trajectory_pos.SetActive(true);
        M_Trajectory_pos.SetActive(true);
        R_Trajectory_pos.SetActive(true);
    }
    public void ShowPosition(int num)
    {
        switch(num)
        {
            case -1: L_Trajectory_pos.SetActive(true); break;
            case 0: M_Trajectory_pos.SetActive(true); break;
            case 1: R_Trajectory_pos.SetActive(true); break;
        }
    }
    public void ShowMyTrajectory(int num)
    {
        switch (num)
        {
            case -1: MyL_Trajectory_pos.SetActive(true); break;
            case 0: MyM_Trajectory_pos.SetActive(true); break;
            case 1: MyR_Trajectory_pos.SetActive(true); break;
        }
    }
    public void HideMyPosition()
    {
        MyL_Trajectory_pos.SetActive(false);
        MyM_Trajectory_pos.SetActive(false);
        MyR_Trajectory_pos.SetActive(false);
    }
    public void HidePosition()
    {
        L_Trajectory_pos.SetActive(false);
        M_Trajectory_pos.SetActive(false);
        R_Trajectory_pos.SetActive(false);
    }
    public void ShowTrajectory(int t)
    {
        if(t==0)
        {
            M_Trajectory_pos.SetActive(true);
        }
        else if(t==-1)
        {
            L_Trajectory_pos.SetActive(true);
        }
        else
        {
            R_Trajectory_pos.SetActive(true);
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
