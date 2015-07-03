using UnityEngine;
using System.Collections;

public class H_Shield : MonoBehaviour {
    Rotation rotation;
    public ShieldManager shieldPosition;
    public int Id;
	// Use this for initialization
	void Start () {
        rotation= transform.GetComponent("Rotation") as Rotation;
        rotation.isActive=false;
        //Debug.Log(rotation.isActive.ToString());
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    void OnMouseDown()
    {
        ShieldManager.Instance.NewShield(transform.position, transform.rotation,Id);
    }
    void OnMouseOver()
    {
        rotation.isActive = true;
    }
    void OnMouseExit()
    {
        rotation.isActive = false;
    }
}
