using UnityEngine;
using System.Collections;
public class FLY : MonoBehaviour
{
    AttackMagic attackMagic;
    //GameObject MainCamera;
    public GameObject Explode;
    public Vector3 RandPosition;
    public TrajectoryType trajectory;
    public float rotate = 0f;
    Vector3 nextDestination;
    float a = (880.0f - 850.0f) / 2;//椭圆轨迹长轴
    float b = 9.3f;//椭圆轨迹短轴
    float y=1.6f;//椭圆轨迹高度
    float v0 = 2.0f;//法术飞行初速度
    float time = 2.0f;//飞行总时间
    float k = 0.8f;//两侧起点位置参数
    float km = 0.6f;//中间起点位置参数
    float oz = 865.0f;//原点位置修正值z
    float ox = 1000.0f;//原点位置修正值x
    float z_standard_l = 853.0f;//左起点z
    float z_standard_r = 853.0f;//右起点z
    float z_standard_m = 856.0f;//中起点z
    float x_standard_l = 994.2f;//左起点x
    float x_standard_r = 1005.8f;//右起点x
    float x_standard_m = 1000.0f;//中起点x
    float time_to_prepare = 0.75f;//就位时间
    float t = 0;//飞行时间
    bool isHeros;
//    float x = 0.5f;
    bool trigger = false;
    iTween itween;
    bool isItween = true;//itween是否存在
    bool startFly = true;//刚开始飞行？
    // Use this for initialization
    void Start()
    {
       // MainCamera = GameObject.Find("Main Camera");
        attackMagic = this.GetComponent("AttackMagic") as AttackMagic;
        gameObject.transform.Rotate(0, rotate, 0);
        isHeros=attackMagic.isHeros;
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;
        if (t > time_to_prepare)//*4/5
        {
            if(isItween)
            {
                Destroy(itween);
                isItween = false;
            }
            if (isHeros)
            {
                if (trajectory == TrajectoryType.Right) //if (transform.position.x > x_standard_m)
                {
                    nextDestination.y = y;
                    nextDestination.z = a * ((1 + k) * ((t - time_to_prepare) * (t - time_to_prepare)) / (time * time) - k) + oz + v0 * (t - time_to_prepare);
                    nextDestination.x = b * Mathf.Sqrt(1 - Mathf.Pow((nextDestination.z-oz)/a, 2)) + ox;
                }
                else if (trajectory == TrajectoryType.Left)//if (transform.position.x < x_standard_m)
                {
                    nextDestination.y = y;
                    nextDestination.z = a * ((1 + k) * ((t - time_to_prepare) * (t - time_to_prepare)) / (time * time) - k) + oz + v0 * (t - time_to_prepare);
                    nextDestination.x = -b * Mathf.Sqrt(1 - Mathf.Pow((nextDestination.z-oz)/a, 2)) + ox;
                }
                else if (transform.position.z < 880.0f)
                {
                    nextDestination.y = y;
                    nextDestination.z = a * ((1 + km) * ((t - time_to_prepare) * (t - time_to_prepare)) / (time * time) - km) + oz + v0 * (t - time_to_prepare);
                    nextDestination.x = x_standard_m;
                }
            }
            else if (!isHeros)
            {
                if (trajectory == TrajectoryType.Right)//if (transform.position.x > x_standard_m)
                {
                    nextDestination.y = y;
                    nextDestination.z = -a * ((1 + k) * ((t - time_to_prepare) * (t - time_to_prepare)) / (time * time) - k) + oz - v0 * (t - time_to_prepare);
                    nextDestination.x = b * Mathf.Sqrt(1 - Mathf.Pow((nextDestination.z-oz)/a, 2)) + ox;
                }
                else if (trajectory == TrajectoryType.Left)//if (transform.position.x < x_standard_m)
                {
                    nextDestination.y = y;
                    nextDestination.z = -a * ((1 + k) * ((t - time_to_prepare) * (t - time_to_prepare)) / (time * time) - k) + oz - v0 * (t - time_to_prepare);
                    nextDestination.x = -b * Mathf.Sqrt(1 - Mathf.Pow((nextDestination.z-oz)/a, 2)) + ox;
                }
                else if (transform.position.z > 850.0f)
                {
                    nextDestination.y = y;
                    nextDestination.z = -a * ((1 + km) * ((t - time_to_prepare) * (t - time_to_prepare)) / (time * time) - km) + oz - v0 * (t - time_to_prepare);
                    nextDestination.x = x_standard_m;
                }
            }
//            Debug.Log(nextDestination.x);
            transform.position = nextDestination;
        }
        else if(startFly)
        {
            Vector3 neolocation;
            if (isHeros)
            {
                if (trajectory == TrajectoryType.Right) //if (transform.position.x > x_standard_m)
                {
                    neolocation.x = x_standard_r;
                    neolocation.y = y;
                    neolocation.z = z_standard_r;
                }
                else if (trajectory == TrajectoryType.Left)//if (transform.position.x < x_standard_m)
                {
                    neolocation.x = x_standard_l;
                    neolocation.y = y;
                    neolocation.z = z_standard_l;
                }
                else 
                {
                    neolocation.x = x_standard_m;
                    neolocation.y = y;
                    neolocation.z = z_standard_m;
                }
            }
            else
            {
                if (trajectory == TrajectoryType.Right)//if (transform.position.x > x_standard_m)
                {
                    neolocation.x = x_standard_r;
                    neolocation.y = y;
                    neolocation.z = z_standard_r+2*(oz-z_standard_r);
                }
                else if (trajectory == TrajectoryType.Left)//if (transform.position.x < x_standard_m)
                {
                    neolocation.x = x_standard_l;
                    neolocation.y = y;
                    neolocation.z = z_standard_l+2*(oz-z_standard_l);
                }
                else 
                {
                    neolocation.x = x_standard_m;
                    neolocation.y = y;
                    neolocation.z = z_standard_m+2*(oz-z_standard_m);
                }
            }
            //位置偏移
            if ( trajectory==0 &&attackMagic.isHeros ? attackMagic.myWaitNum != 0 : attackMagic.eWaitNum != 0)
            {
                RandPosition = new Vector3((isHeros ? (attackMagic.myWaitNum % 2 == 0 ? 1 : -1) : (attackMagic.eWaitNum % 2 == 0 ? 1 : -1)) * Random.Range(0.2f, 0.5f), Random.Range(-0.2f, 0.2f), 0);
                neolocation += RandPosition;
                x_standard_m += RandPosition.x;
                y += RandPosition.y;
               // gameObject.GetComponent<BoxCollider>().center -= RandPosition;
            }
          iTween.MoveTo(gameObject, iTween.Hash("position", neolocation, "time", time_to_prepare, "easeType", iTween.EaseType.linear));//
          itween = gameObject.GetComponent<iTween>();
          startFly = false;
        }
        
    }
    void OnTriggerEnter(Collider otherObject)
    {
        
        if (!trigger)
        {

            Magic opponent = otherObject.transform.GetComponent<Magic>();
            if (opponent != null)//碰撞的是魔法
            {
                attackMagic.Crash(opponent);

                //更新数值
                if (opponent != null) { opponent.refresh = true; }
                if (attackMagic != null) { attackMagic.refresh = true; }
            }
            else//碰撞的是英雄
            {

                Hero enermyHero = otherObject.transform.GetComponent<Hero>();
                if(enermyHero!=null)
                attackMagic.Crash(enermyHero);
            }
           
            //trigger = true;
            //attackMagic.Crash(opponent);
            Explode = attackMagic.Explode;
            Object exp =Instantiate(Explode, transform.position, Quaternion.identity);
            CameraMoving.Instance.ShakeCamera(new Vector3(0.1f, 0.1f, 0.1f), 0.5f);
            Destroy(exp, 3f);
        }
    }
}
