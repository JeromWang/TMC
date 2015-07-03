using UnityEngine;
using System.Collections;
using Vectrosity;
public class Move : MonoBehaviour
{
    //public static bool[,] keng=new bool [9,5];
    static bool plusEnergy = false;
    Vector3 lastVector;
    Vector3 originalVector;
    GameObject halo;
    bool isIn = false;
    GameObject Fog;
    public void Destroy()
    {
        Destroy(gameObject);
    }
    public static void Restart()
    {
        plusEnergy = false;
    }
    // Use this for initialization
    void Start()
    {
        lastVector = transform.position;
        originalVector = transform.position;
        Transform t = transform.FindChild("Halo");
        if (t != null)
        {
            halo = t.gameObject;
            halo.SetActive(false);
        }
        Fog = transform.FindChild("fogExpend").gameObject;
        Fog.SetActive(false);
    }
    void OnMouseEnter()
    {
       // Debug.Log("enter");
        isIn = true;
        //if (RayTest.Instance.draw && transform.parent.name != "Crystal")
        //{
        //    //RayTest.Instance.crystal = this.gameObject;
        //    //RayTest.Instance.crytal2 = true;
        //    RayTest.Instance.Link(this.gameObject);
        //    //RayTest.Instance.crystal = null;
        //} 
        halo.SetActive(true);
        //Invoke("HaloTrue", 0.2f);
    }
    void HaloTrue()
    {
        halo.SetActive(true);
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1) && isIn)
        {
            if (transform.parent.name!="Crystal")
            {
               // Debug.Log(gameObject.name);
               // Debug.Log("right");
                //RayTest.Instance.crystal = this.gameObject;
                //RayTest.Instance.linkStop = !RayTest.Instance.linkStop;
                //RayTest.Instance.crytal2 = RayTest.Instance.linkStop;
                RayTest.Instance.Link(this.gameObject);
                //RayTest.Instance.crystal = null;
                Fog.SetActive(true);
                Invoke("FogFalse", 1.5f);

            }
        }
    }

    void FogFalse()
    {
        Fog.SetActive(false);
    }

    void OnMouseExit()
    {
//        Debug.Log("exit");
        halo.SetActive(false);
        isIn = false;
        //if (RayTest.Instance.linkStop == false && RayTest.Instance.crytal2 == true && transform.parent.name != "Crystal")
        //{
        //    //Debug.Log("false");
        //    RayTest.Instance.crystal = this.gameObject;
        //    RayTest.Instance.Link(this.gameObject);
        //    RayTest.Instance.crystal = null;
        //}
    }

    IEnumerator OnMouseDown()
    {//移动
        if (EnergyManager.Instance.accessibleCrystal <= 0)
        {
            GuideText.Instance.ReturnText("CrystalMoved");
        }
        if (EnergyManager.Instance.accessibleCrystal > 0 && CameraMoving.Instance.status==2)// (GameManager.Instance.CameraInUse == 2)
        {
            if (transform.position == originalVector)
            {
                Object newCrystal = Instantiate(transform, transform.position, transform.rotation);
                Transform newCrystal2 = (Transform)newCrystal;
                newCrystal2.parent = transform.parent;
                transform.parent = GameObject.Find("CrystalUsed").transform;
            }
            

            Vector3 screenSpace = Camera.main.WorldToScreenPoint(transform.position);
            Vector3 offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenSpace.z));
            while (Input.GetMouseButton(0))
            {
                //Vector3 curScreenSpace =new Vector3((float)((Input.mousePosition.x-x)/2.6+x), (float)((Input.mousePosition.y-y)/2.6+y), screenSpace.z);
                Vector3 curScreenSpace = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenSpace.z);
                transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
                Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenSpace) + offset;
                transform.position = curPosition;
                yield return new WaitForSeconds(0.001f);//这个很重要，循环执行
            }
            adsorption();
            Fog.SetActive(true);
            Invoke("FogFalse", 1.5f);
        }
    }

    void adsorption()
    {//吸附效果
        float x = 0, z = 0; int kx = -1, ky = -1, kx0 = -1, ky0 = -1, l = 0;
        bool s = false;
        //在法阵范围之外
        if (transform.position.x < 996.5f || transform.position.x > 1003.5f || transform.position.z < 847 || transform.position.z > 853)
        {
            transform.position = lastVector;
            transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            if (lastVector == originalVector)
            {
                Destroy(gameObject, 0.3f);
            }
            //Debug.Log("Out of range.");
            return;
        }

        z = MagicCircleMananger.Instance.getz(transform.position.z);
        if (z == MagicCircleMananger.Instance.rowKey[2])
            x = MagicCircleMananger.Instance.getx1(transform.position.x);
        else if (z == MagicCircleMananger.Instance.rowKey[4] || z == MagicCircleMananger.Instance.rowKey[0])
            x = MagicCircleMananger.Instance.getx2(transform.position.x);
        else
            x = MagicCircleMananger.Instance.getx3(transform.position.x);

        kx = MagicCircleMananger.Instance.getKx(x);
        ky = MagicCircleMananger.Instance.getKy(z);
        kx0 = MagicCircleMananger.Instance.getKx(lastVector.x);
        ky0 = MagicCircleMananger.Instance.getKy(lastVector.z);
        //Debug.Log("x"+kx);
        //Debug.Log("y"+ky);
        s = false;
        if (kx0 != -1)
            l = 5 * kx0 + ky0;
        //是否连着线
        for (int n = 0; n < 43; n++)
            if (n != l)
                if (MagicCircleMananger.Instance.GetLine(l, n))
                {
                    s = true;
                    GuideText.Instance.ReturnText("LinkedCrystalCannotMove");
                    break;
                }
        //
        if (MagicCircleMananger.Instance.GetLineKeng(kx, ky))
        {
            if (lastVector == originalVector)
            {
                transform.position = originalVector;
                Destroy(gameObject, 0.3f);
            }
            else
            {
                transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                transform.position = lastVector;
                GuideText.Instance.ReturnText("CannotPlaceInLink");
            }
            return;
        }
        else if (s)
        {
            if (lastVector == originalVector)
            {
                transform.position = originalVector;
                Destroy(gameObject, 0.3f);
            }
            else
            {
                transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                transform.position = lastVector;
            }
            return;
        }
        transform.position = new Vector3(x, 0.1f, z);
        transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        if (kx0 != -1)
        {
            MagicCircleMananger.Instance.KengFalse(kx0, ky0);
            MagicCircleMananger.Instance.LineKengMinus(kx0, ky0);
        }
        MagicCircleMananger.Instance.KengTrue(kx, ky);
        MagicCircleMananger.Instance.LineKengPlus(kx, ky);
        EnergyManager.Instance.accessibleCrystal--;
        if(lastVector==originalVector)
        {
            if (plusEnergy)
            {
                EnergyManager.Instance.PlusEnergy(1);
            }
            plusEnergy = !plusEnergy;
            if (EnergyManager.Instance.unmatchedEnergy)
            {
                EnergyManager.Instance.unmatchedEnergy = false;
                EnergyManager.Instance.totalEnergy++;
                EnergyManager.Instance.CrystalSee();
            }
            else
                EnergyManager.Instance.unmatchedEnergy = true;
        }

        lastVector = transform.position;
    }
}
