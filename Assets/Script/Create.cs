using UnityEngine;
using System.Collections;

public class Create : MonoBehaviour {
	public GameObject obj;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnMouseDown (){
	 Instantiate(obj,transform.position,transform.rotation);
	}
}
