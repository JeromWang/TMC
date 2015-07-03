using UnityEngine;
using System.Collections;

public class Hit : MonoBehaviour {
	public int choose = 0;
	GameObject fire;
	public GameObject FireBall;
	public GameObject Explode;
	public static bool F= false ;
	FLY fly;
	
	public GameObject MagicShield;
	GameObject magicshield;
	public static bool MS = false;
	public static Object ms;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);//从摄像机发出到点击坐标的射线
            RaycastHit hitInfo;
            if(Physics.Raycast(ray,out hitInfo))
            {
                Debug.DrawLine(ray.origin,hitInfo.point);//划出射线，只有在scene视图中才能看到
                GameObject gameObj = hitInfo.collider.gameObject;
                //Debug.Log("click object tag is "+gameObj.tag );
                if(gameObj.tag == "Fire")//当射线碰撞目标为Crystal类型的物品 ，连线
                {
					choose =1;
					fire = gameObj;
				}
				else if (1== choose && gameObj.tag == "Enermy" )
				{
					choose = 2;
					fire.AddComponent("FLY");
					fly = fire.GetComponent("FLY") as FLY;
					float angle;
					Vector3 a= new Vector3 (fire.transform.position.x ,0, fire.transform.position.z);
					Vector3 b= new Vector3 (gameObj.transform.position.x, 0,gameObj.transform.position.z);
					Vector3 a2bDirection = b - a;//获得一条从A向量尾指向B向量尾的向量。
					if(gameObj.transform.position.x - fire.transform.position.x>=0)
						 angle =180f + Vector3.Angle(a2bDirection,fire.transform.forward);//获得A物体的Z方向和a2bDirection的夹角
					else 
						 angle =180f - Vector3.Angle(a2bDirection,fire.transform.forward);
					fly.rotate=angle;
					fly.Explode=Explode;
					F = false;
				}
				else
					Debug.Log(gameObj.tag);
			}
		}
	}

	void OnGUI(){
		if(GUILayout.Button("Fire Ball",GUILayout.Height(30),GUILayout.Width(100)))
		{
			if(F == false)
			{
				GameObject FB=(GameObject)Instantiate(FireBall,new Vector3(1000f,1f,850f),Quaternion.identity);
				FB.transform.Rotate(0,180f,180f);
				F= true;
			}
		}
		if(GUILayout.Button("MagicShield",GUILayout.Height(30),GUILayout.Width(100)))
			if(MS== false)
			{
				ms=Instantiate(MagicShield,new Vector3(1000f,2f,858f),Quaternion.identity);
				MS=true;
			}
			else
			{
				Destroy(ms);
				MS=false;
			}
	}
}
