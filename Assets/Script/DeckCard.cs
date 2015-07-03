using UnityEngine;
using System.Collections;

public class DeckCard : MonoBehaviour {
    bool mouseIn = false;
    public static bool magnify = false;
    public bool inPool;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	    if(mouseIn)
        {
            
            if (magnify
                && (Input.GetMouseButton(0) || Input.GetAxis("Mouse ScrollWheel") < 0 || Input.GetAxis("Mouse ScrollWheel") > 0))
            {
              //  Debug.Log("no"); 
                transform.localScale = new Vector3(1f,1f,1f);
                transform.localPosition = transform.localPosition + new Vector3(0.1f, 0, 0);
                Deck.Instance.Tile(Deck.Instance.CardZone);
                Deck.Instance.Tile(Deck.Instance.PoolZone);
                
                magnify = false;
            }
            if(!magnify &&Input.GetMouseButton(1))
            {
//                Debug.Log("right");
                transform.position = GameObject.Find("DeckShowCard").transform.position;
                transform.localScale = new Vector3(2f, 2f, 2f);
                transform.localPosition = transform.localPosition + new Vector3(-0.1f, 0, 0);
                magnify = true;
            }
            
            if (Input.GetAxis("Mouse ScrollWheel") < 0 )
            {
                iTween.MoveAdd(transform.parent.gameObject, new Vector3(0, 0, 0.8f), 0.4f);
            }
            if (Input.GetAxis("Mouse ScrollWheel") > 0 )
            {
                iTween.MoveAdd(transform.parent.gameObject, new Vector3(0, 0, -0.8f), 0.4f);
            }
        }
	}
    void OnMouseDown()
    {
        if (!magnify)//&& Input.GetMouseButton(0)
        {
           // Debug.Log("change");
            Deck.Instance.ChangeList(transform);
        }
    }
    void OnMouseOver()
    {
        //Debug.Log("in");
        mouseIn = true;
    }
    void OnMouseExit()
    {
//        Debug.Log("out");
        mouseIn = false;
    }
}
