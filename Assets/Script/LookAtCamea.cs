using UnityEngine;
using System.Collections;

public class LookAtCamea : MonoBehaviour {
	public GameObject camera;
	// Use this for initialization
	void Start () {
	transform.LookAt(camera.transform.position);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
