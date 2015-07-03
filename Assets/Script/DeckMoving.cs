using UnityEngine;
using System.Collections;

public class DeckMoving : MonoBehaviour {
    bool mouseIn = false;
    Vector3 originPosition;
	// Use this for initialization
	void Start () {
        originPosition = transform.position;
	}
	void OnMouseOver()
    {
        mouseIn = true;
    }
    void OnMouseExit()
    {
        mouseIn = false;
    }
	// Update is called once per frame
	void Update () {
        if (mouseIn  )
        {
            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                iTween.MoveAdd(gameObject, new Vector3(0, 0, 0.8f), 0.4f);
            }
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                iTween.MoveAdd(gameObject, new Vector3(0, 0, -0.8f), 0.4f);
            }
        }
	}
}
