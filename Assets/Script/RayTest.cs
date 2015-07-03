using UnityEngine;
using System.Collections;
using Vectrosity;
public class RayTest : MonoBehaviour
{
    // public GameObject crystal;
    public static RayTest Instance;
    public Material lineMaterial;
    public Material lineMaterial2;

    Vector3[] linePoints;
    private int index;
    int l1, l2, l3;
    VectorLine[,] line = new VectorLine[43, 43], line2 = new VectorLine[43, 43];
    VectorLine temp1, temp2;
    Vector3 screenSpace;
    public bool draw = false;
    //public bool linkStop = true;
    //public bool crytal2 = true;
    public void EndGame()
    {
        for (int i = 0; i < 43; i++)
        {
            for (int j = 0; j < 43; j++)
            {
                DestroyVectorLine(i, j);
            }
        }
    }
    Vector3 GetPointPosition(int x)
    {
        int y1 = x % 5;
        int x1 = (x - y1) / 5;
        return new Vector3((float)(997.6 + x1 * 0.6), 0.1f, (float)(847.9 + y1 * 1.05));
    }
    public void AddLine(int x, int y)
    {
        //Debug.Log(x.ToString() + " " + y.ToString());
        //Debug.Log(linePoints[0].ToString());
        //Debug.Log(linePoints[1].ToString());
        Vector3[] linePoints = new Vector3[2];
        linePoints[0] = GetPointPosition(x);
        linePoints[1] = GetPointPosition(y);
        line[x, y] = new VectorLine("3DLine", linePoints, Color.green, lineMaterial, 8.0f);
        line2[x, y] = new VectorLine("3DLine", linePoints, Color.green, lineMaterial2, 15.0f);
        line[x, y].Draw3DAuto();
        line2[x, y].Draw3DAuto();
        //Debug.Log(line[x, y].ToString());
        MagicCircleMananger.Instance.LineTrue(x, y);
    }
    void DestroyVectorLine(int x, int y)
    {
        VectorLine.Destroy(ref line[x, y]);
        VectorLine.Destroy(ref line2[x, y]);
        VectorLine.Destroy(ref line[y, x]);
        VectorLine.Destroy(ref line2[y, x]);
    }
    public void DeleteLine(int x, int y)
    {
        DestroyVectorLine(x, y);
        MagicCircleMananger.Instance.LineFalse(x, y);
        int x1 = x / 5;
        int y1 = x % 5;
        int x2 = y / 5;
        int y2 = y % 5;
        double k = (double)(Mathf.Abs(y1 - y2)) / (double)(Mathf.Abs(x1 - x2));
        if (x1 == x2)
        {
            if (Mathf.Abs(y1 - y2) == 4)
            {
                DeleteLine((5 * x1 + (y1 + y2) / 2), x);
                DeleteLine((5 * x1 + (y1 + y2) / 2), y);
            }
        }
        else if (y1 == y2)
        {
            if (Mathf.Abs(x1 - x2) == 4)
            {
                DeleteLine((5 * (x1 + (x2 - x1) / 2) + y1), x);
                DeleteLine((5 * (x1 + (x2 - x1) / 2) + y1), y);
            }
            else if (Mathf.Abs(x1 - x2) == 6)
            {
                DeleteLine((5 * (x1 + (x2 - x1) / 3) + y1), x);
                DeleteLine((5 * (x1 + (x2 - x1) / 3) + y1), y);
                DeleteLine((5 * (x1 + (x2 - x1) * 2 / 3) + y1), x);
                DeleteLine((5 * (x1 + (x2 - x1) * 2 / 3) + y1), y);
            }
            else if (Mathf.Abs(x1 - x2) == 8)
            {
                DeleteLine((5 * (x1 + (x2 - x1) / 4) + y1), x);
                DeleteLine((5 * (x1 + (x2 - x1) / 4) + y1), y);
                DeleteLine((5 * (x1 + (x2 - x1) / 2) + y1), x);
                DeleteLine((5 * (x1 + (x2 - x1) / 2) + y1), y);
                DeleteLine((5 * (x1 + (x2 - x1) * 3 / 4) + y1), x);
                DeleteLine((5 * (x1 + (x2 - x1) * 3 / 4) + y1), y);
            }
        }
        else if (k == 1)
        {
            if (Mathf.Abs(x1 - x2) == 2)
            {
                DeleteLine((5 * (x1 + (x2 - x1) / 2) + (y1 + (y2 - y1) / 2)), x);
                DeleteLine((5 * (x1 + (x2 - x1) / 2) + (y1 + (y2 - y1) / 2)), y);
            }
            else if (Mathf.Abs(x1 - x2) == 3)
            {
                DeleteLine((5 * (x1 + (x2 - x1) / 3) + (y1 + (y2 - y1) / 3)), x);
                DeleteLine((5 * (x1 + (x2 - x1) / 3) + (y1 + (y2 - y1) / 3)), y);
                DeleteLine((5 * (x1 + (x2 - x1) * 2 / 3) + (y1 + (y2 - y1) * 2 / 3)), x);
                DeleteLine((5 * (x1 + (x2 - x1) * 2 / 3) + (y1 + (y2 - y1) * 2 / 3)), y);
            }
            else if (Mathf.Abs(x1 - x2) == 4)
            {
                DeleteLine((5 * (x1 + (x2 - x1) / 4) + (y1 + (y2 - y1) / 4)), x);
                DeleteLine((5 * (x1 + (x2 - x1) / 4) + (y1 + (y2 - y1) / 4)), y);
                DeleteLine((5 * (x1 + (x2 - x1) / 2) + (y1 + (y2 - y1) / 2)), x);
                DeleteLine((5 * (x1 + (x2 - x1) / 2) + (y1 + (y2 - y1) / 2)), y);
                DeleteLine((5 * (x1 + (x2 - x1) * 3 / 4) + (y1 + (y2 - y1) * 3 / 4)), x);
                DeleteLine((5 * (x1 + (x2 - x1) * 3 / 4) + (y1 + (y2 - y1) * 3 / 4)), y);
            }
        }
        else if (k == (double)1 / (double)3)
        {
            if (Mathf.Abs(y1 - y2) == 2)
            {
                DeleteLine((5 * (x1 + (x2 - x1) / 2) + (y1 + (y2 - y1) / 2)), x);
                DeleteLine((5 * (x1 + (x2 - x1) / 2) + (y1 + (y2 - y1) / 2)), y);
            }
        }
    }
    void Start()
    {
        l1 = l2 = l3 = 0;
        RayTest.Instance = this;
        draw = false;
        //linkStop = true;
        //crytal2 = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(2))//删除水晶
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);//从摄像机发出到点击坐标的射线
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo))
            {
                Debug.DrawLine(ray.origin, hitInfo.point);//划出射线，只有在scene视图中才能看到
                GameObject gameObj = hitInfo.collider.gameObject;
                //Debug.Log("click object tag is "+gameObj.tag );
                if (gameObj.tag == "Crystal")//当射线碰撞目标为Crystal类型的物品
                {
                    float x = 0, z = 0;
                    int kx = -1, ky = -1;
                    z = MagicCircleMananger.Instance.getz(gameObj.transform.position.z);
                    if (z == MagicCircleMananger.Instance.rowKey[2])
                        x = MagicCircleMananger.Instance.getx1(gameObj.transform.position.x);
                    else if (z == MagicCircleMananger.Instance.rowKey[0] || z == MagicCircleMananger.Instance.rowKey[4])
                        x = MagicCircleMananger.Instance.getx2(gameObj.transform.position.x);
                    else
                        x = MagicCircleMananger.Instance.getx3(gameObj.transform.position.x);
                    kx = MagicCircleMananger.Instance.getKx(x);
                    ky = MagicCircleMananger.Instance.getKy(z);
                    if (kx != -1)
                    {
                        MagicCircleMananger.Instance.KengFalse(kx, ky);
                        MagicCircleMananger.Instance.LineKengMinus(kx, ky);
                        Destroy(gameObj);
                        draw = false;
                        if (EnergyManager.Instance.unmatchedEnergy)
                            EnergyManager.Instance.unmatchedEnergy = false;
                        else
                        {
                            EnergyManager.Instance.unmatchedEnergy = true;
                            EnergyManager.Instance.totalEnergy--;
                            EnergyManager.Instance.CrystalSee();
                        }
                        l3 = 5 * kx + ky;
                        for (int n = 0; n < 43; n++)
                            if (n != l3)
                                if (MagicCircleMananger.Instance.GetLine(l3, n)&&MagicCircleMananger.Instance.IsOperable(l3,n))
                                    DeleteLine(l3, n);
                    }
                }
            }
        }
        Link();
        //crystal = null;
    }
    void Link()
    {
        if (draw)
        {
            linePoints[1] = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenSpace.z));
            temp1.Draw3D();
            temp2.Draw3D();
        }
    }
    public void Link(GameObject crystal)
    {
        if (crystal != null)//水晶连线   && linkStop == false
        {
            if (!draw)//如果还没开始画线
            {
                //if (EnergyManager.Instance.accessibleEnergy > 0)
                {
                    #region
                    float x = 0, z = 0; int kx = -1, ky = -1;
                    linePoints = new Vector3[2];
                    linePoints[0] = crystal.transform.position;
                    z = MagicCircleMananger.Instance.getz(linePoints[0].z);
                    if (z == MagicCircleMananger.Instance.rowKey[2])
                        x = MagicCircleMananger.Instance.getx1(linePoints[0].x);
                    else if (z == MagicCircleMananger.Instance.rowKey[0] || z == MagicCircleMananger.Instance.rowKey[4])
                        x = MagicCircleMananger.Instance.getx2(linePoints[0].x);
                    else
                        x = MagicCircleMananger.Instance.getx3(linePoints[0].x);
                    kx = MagicCircleMananger.Instance.getKx(x);
                    ky = MagicCircleMananger.Instance.getKy(z);
                    if (kx != -1)
                    {
                        l1 = 5 * kx + ky;
                        draw = true;
                        //EnergyManager.Instance.MinusEnergy(1);
                    }
                    screenSpace = Camera.main.WorldToScreenPoint(crystal.transform.position);
                    linePoints[1] = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenSpace.z));
                    temp1 = new VectorLine("3DLine", linePoints, Color.green, lineMaterial, 8.0f);
                    temp2 = new VectorLine("3DLine", linePoints, Color.green, lineMaterial2, 15.0f);
                    temp1.Draw3DAuto();
                    temp2.Draw3DAuto();
                    #endregion
                }
                //else
                //{
                //    //linkStop = true;
                //}
            }
            else
            {
                #region
                float x = 0, z = 0; int kx = -1, ky = -1;
                linePoints[1] = crystal.transform.position;
                z = MagicCircleMananger.Instance.getz(linePoints[1].z);
                if (z == MagicCircleMananger.Instance.rowKey[2])
                    x = MagicCircleMananger.Instance.getx1(linePoints[1].x);
                else if (z == MagicCircleMananger.Instance.rowKey[0] || z == MagicCircleMananger.Instance.rowKey[4])
                    x = MagicCircleMananger.Instance.getx2(linePoints[1].x);
                else
                    x = MagicCircleMananger.Instance.getx3(linePoints[1].x);
                kx = MagicCircleMananger.Instance.getKx(x);
                ky = MagicCircleMananger.Instance.getKy(z);
                l2 = 5 * kx + ky;
                if (l2 == l1)//连自己=>取消连线
                {
                    VectorLine.Destroy(ref temp1);
                    VectorLine.Destroy(ref temp2);
                    //EnergyManager.Instance.MinusEnergy(-1);
                }
                else if (MagicCircleMananger.Instance.IsOperable(l1, l2))
                {
                    if (kx != -1 && !MagicCircleMananger.Instance.GetLine(l1, l2) && MagicCircleMananger.Instance.GetLineSwitch(l1 / 5, l1 % 5, l2 / 5, l2 % 5))//画线
                    {
                        //AddLine(l1, l2);
                        if (EnergyManager.Instance.accessibleEnergy>0)
                        {
                            temp1.Draw3D();
                            temp2.Draw3D();
                            line[l1, l2] = temp1; line2[l1, l2] = temp2;
                            MagicCircleMananger.Instance.LineTrue(l1, l2);
                            EnergyManager.Instance.MinusEnergy(1);
                        }
                        else
                        {
                            VectorLine.Destroy(ref temp1);
                            VectorLine.Destroy(ref temp2);
                            GuideText.Instance.ReturnText("LinkNeedEnergy");
                        }
                    }
                    else if (MagicCircleMananger.Instance.GetLine(l1, l2))//删线
                    {

                        // Debug.Log("delete");
                        DeleteLine(l1, l2);
                        VectorLine.Destroy(ref temp1);
                        VectorLine.Destroy(ref temp2);
                        //EnergyManager.Instance.MinusEnergy(-1);
                        //linkStop = true;
                    }
                }
                else
                {
                    GuideText.Instance.ReturnText("NoJumpLink");
                    return;
                }
                draw = false;
                #endregion
            }
        }
    }
}

