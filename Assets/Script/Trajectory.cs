using UnityEngine;
using System.Collections;

public class Trajectory : MonoBehaviour
{
    public int ID;
    void OnMouseDown()
    {
        TrajectoryPosition.Instance.ID = this.ID;
        TrajectoryPosition.Instance.TrajectoryFinish();
    }
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
