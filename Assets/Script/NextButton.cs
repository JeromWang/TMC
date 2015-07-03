using UnityEngine;
using System.Collections;

public class NextButton : MonoBehaviour {
    public int ID;
    UILabel label;
	// Use this for initialization
	void Start () {
        label = transform.FindChild("label").GetComponent<UILabel>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    void OnMouseDown()
    {
        GuideText.Instance.ButtonDown(ID);
        label.color = Color.black;
    }
    void OnMouseEnter()
    {
        audio.Play();
        label.color = Color.blue;
    }
    void OnMouseExit()
    {
        label.color = Color.black;
    }
}
