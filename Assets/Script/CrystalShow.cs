using UnityEngine;
using System.Collections;

public class CrystalShow : MonoBehaviour {
    public GameObject crystal;
    public static CrystalShow Instance;
	// Use this for initialization
	void Start () {
        CrystalShow.Instance = this;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    public void ShowHide(bool b)
    {
        crystal.SetActive(b);
    }
}
