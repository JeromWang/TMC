﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MagicCircleMananger : MonoBehaviour
{
    public static MagicCircleMananger Instance;

	//public int CameraInUse;//当前启用的摄像机

    public delegate void ChangeLineEvent();
    public event ChangeLineEvent ChangeLine;
	
    public bool[,] keng = new bool[9, 5];//法阵里面的点
    public int[,] linekeng = new int[9, 5];//法阵点上面线的条数
    public bool[,] line = new bool[43, 43]; //魔法连线是否存在的bool数组

    public int LineTrueDepth;//递归函数的层数
    public int LineFalseDepth;//递归函数的层数
    public float[] rowKey = new float[5];//标准点阵列坐标
    public float[] lineKey = new float[9];//标准点阵行坐标

    public float StartCoordinate = 1006f;//起始坐标
    public GameObject[] level2Hide=new GameObject[6];
    
    /// <summary>
    /// Awake this instance.
    /// </summary>
    public void Awake()
    {
        MagicCircleMananger.Instance = this;
    }
    // Use this for initialization
    /// <summary>
    /// 对所有数组以及行列坐标进行了初始化
    /// </summary>
    void Start()
    {
        for (int i = 0; i < 9; i++)
            for (int j = 0; j < 5; j++)
                keng[i, j] = false;
        for (int i = 0; i < 9; i++)
            for (int j = 0; j < 5; j++)
                linekeng[i, j] = 0;
        for (int i = 0; i < 43; i++)
        {
            for (int j = 0; j < 43; j++)
                line[i, j] = false;
            line[i, i] = true;
        }
        rowKey[0] = 850 - 2 * 13 * 0.08f;
        rowKey[1] = 850 - 13 * 0.08f;
        rowKey[2] = 850;
        rowKey[3] = 850 + 13 * 0.08f;
        rowKey[4] = 850 + 2 * 13 * 0.08f;
        lineKey[0] = 1000 - 4 * 7.5f * 0.08f;
        lineKey[1] = 1000 - 3 * 7.5f * 0.08f;
        lineKey[2] = 1000 - 2 * 7.5f * 0.08f;
        lineKey[3] = 1000 - 7.5f * 0.08f;
        lineKey[4] = 1000f;
        lineKey[5] = 1000 + 7.5f * 0.08f; ;
        lineKey[6] = 1000 + 2 * 7.5f * 0.08f;
        lineKey[7] = 1000 + 3 * 7.5f * 0.08f;
        lineKey[8] = 1000 + 4 * 7.5f * 0.08f;
        //CameraInUse = 1;
        LineTrueDepth = 0;
        LineFalseDepth = 0;
        if(LevelManager.Instance.level==2)
        {
            linekeng[3, 3] =1;
            linekeng[5, 3] = 1;
            linekeng[2, 2] = 1;
            linekeng[6, 2] = 1;
            linekeng[3, 1] = 1;
            linekeng[5, 1] = 1;

            level2Hide[0]=GameObject.Find("Keng1/1/3.1");
            level2Hide[1] = GameObject.Find("Keng1/1/5.1");
            level2Hide[2] = GameObject.Find("Keng1/2/2.2");
            level2Hide[3] = GameObject.Find("Keng1/2/6.2");
            level2Hide[4] = GameObject.Find("Keng1/3/5.3");
            level2Hide[5] = GameObject.Find("Keng1/3/3.3");
            for(int i=0;i<6;i++)
            {
                level2Hide[i].SetActive(false);
            }
        }
    }
    public void Level2End()
    {
        for (int i = 0; i < 6; i++)
        {
            level2Hide[i].SetActive(true);
        }
    }
    /// <summary>
    /// 将二维的xy转换为一维的
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public int xy2unidimensional(int x, int y)
    {
        return 5 * x + y;
    }
    /// <summary>
    /// 将Line(x,y)置为True
    /// </summary>
    /// <param name='x'>
    /// X.
    /// </param>
    /// <param name='y'>
    /// Y.
    /// </param>
    public void LineTrue(int x, int y)
    {
        LineTrueDepth++;
        if (!GetLine(x, y))
        {
            line[x, y] = true;
            line[y, x] = true;
            LineKengClose(x,y);
        }
        int x1 = x / 5;
        int y1 = x % 5;
        int x2 = y / 5;
        int y2 = y % 5;
        double k1 ;
        if (x1 == x2)
            k1 = 100;
        else
        k1 = (double)(y1 - y2) / (double)(x1 - x2);
        for (int i = 0; i < 43; i++)
            if (line[x, i] && i != x && i != y)
            {
                int x3 = i / 5;
                int y3 = i % 5;
                double k2 ;
                if (x1 == x3)
                    k2 = 100;
                else
                k2 = (double)(y1 - y3) / (double)(x1 - x3);
                if (k1 == k2)
                {
                    if (!line[y, i])
                        LineTrue(y, i);
                    if (!line[i, y])
                        LineTrue(i, y);
                }
            }
        for (int i = 0; i < 43; i++)
            if (line[y, i] && i != x && i != y)
            {
                int x3 = i / 5;
                int y3 = i % 5;
                double k2;
                if(x2==x3)
                    k2=100;
                else
                k2 = (double)(y2 - y3) / (double)(x2 - x3);
                if (k1 == k2 )
                {
                    if (!line[x, i])
                        LineTrue(x, i);
                    if (!line[i, x])
                        LineTrue(i, x);
                }
            }
        if (LineTrueDepth == 1)
            ChangeLine();
        LineTrueDepth--;
    }
    /// <summary>
    /// 将Line(x,y)置为False
    /// </summary>
    /// <param name='x'>
    /// X.
    /// </param>
    /// <param name='y'>
    /// Y.
    /// </param>
    public void LineFalse(int x, int y)
    {
        if (GetLine(x, y))
        {
            line[x, y] = false;
            line[y, x] = false;
            LineKengOpen(x, y);
        }
        int x1 = x / 5;
        int y1 = x % 5;
        int x2 = y / 5;
        int y2 = y % 5;
        double k1 = (double)(Mathf.Abs(y1 - y2)) / (double)(Mathf.Abs(x1 - x2));
        if (x1 == x2)
        {
            if (Mathf.Abs(y1 - y2) == 4)
            {
                LineFalse((5 * x1 + (y1 + y2) / 2), x);
                LineFalse((5 * x1 + (y1 + y2) / 2), y);
            }
        }
        else if (y1 == y2)
        {
            if (Mathf.Abs(x1 - x2) == 4)
            {
                LineFalse((5 * (x1 + (x2 - x1) / 2) + y1), x);
                LineFalse((5 * (x1 + (x2 - x1) / 2) + y1), y);
            }
            else if (Mathf.Abs(x1 - x2) == 6)
            {
                LineFalse((5 * (x1 + (x2 - x1) / 3) + y1), x);
                LineFalse((5 * (x1 + (x2 - x1) / 3) + y1), y);
                LineFalse((5 * (x1 + (x2 - x1) * 2 / 3) + y1), x);
                LineFalse((5 * (x1 + (x2 - x1) * 2 / 3) + y1), y);
            }
            else if (Mathf.Abs(x1 - x2) == 8)
            {
                LineFalse((5 * (x1 + (x2 - x1) / 4) + y1), x);
                LineFalse((5 * (x1 + (x2 - x1) / 4) + y1), y);
                LineFalse((5 * (x1 + (x2 - x1) / 2) + y1), x);
                LineFalse((5 * (x1 + (x2 - x1) / 2) + y1), y);
                LineFalse((5 * (x1 + (x2 - x1) * 3 / 4) + y1), x);
                LineFalse((5 * (x1 + (x2 - x1) * 3 / 4) + y1), y);
            }
        }
        else if (k1 == 1)
        {
            if (Mathf.Abs(x1 - x2) == 2)
            {
                LineFalse((5 * (x1 + (x2 - x1) / 2) + (y1 + (y2 - y1) / 2)), x);
                LineFalse((5 * (x1 + (x2 - x1) / 2) + (y1 + (y2 - y1) / 2)), y);
            }
            else if (Mathf.Abs(x1 - x2) == 3)
            {
                LineFalse((5 * (x1 + (x2 - x1) / 3) + (y1 + (y2 - y1) / 3)), x);
                LineFalse((5 * (x1 + (x2 - x1) / 3) + (y1 + (y2 - y1) / 3)), y);
                LineFalse((5 * (x1 + (x2 - x1) * 2 / 3) + (y1 + (y2 - y1) * 2 / 3)), x);
                LineFalse((5 * (x1 + (x2 - x1) * 2 / 3) + (y1 + (y2 - y1) * 2 / 3)), y);
            }
            else if (Mathf.Abs(x1 - x2) == 4)
            {
                LineFalse((5 * (x1 + (x2 - x1) / 4) + (y1 + (y2 - y1) / 4)), x);
                LineFalse((5 * (x1 + (x2 - x1) / 4) + (y1 + (y2 - y1) / 4)), y);
                LineFalse((5 * (x1 + (x2 - x1) / 2) + (y1 + (y2 - y1) / 2)), x);
                LineFalse((5 * (x1 + (x2 - x1) / 2) + (y1 + (y2 - y1) / 2)), y);
                LineFalse((5 * (x1 + (x2 - x1) * 3 / 4) + (y1 + (y2 - y1) * 3 / 4)), x);
                LineFalse((5 * (x1 + (x2 - x1) * 3 / 4) + (y1 + (y2 - y1) * 3 / 4)), y);
            }
        }
        else if (k1 == (double)1 / (double)3)
        {
            if (Mathf.Abs(y1 - y2) == 2)
            {
                LineFalse((5 * (x1 + (x2 - x1) / 2) + (y1 + (y2 - y1) / 2)), x);
                LineFalse((5 * (x1 + (x2 - x1) / 2) + (y1 + (y2 - y1) / 2)), y);
            }
        }
        LineFalse_Powerup(x, y);
        ChangeLine();
    }
    public void LineFalse_Powerup(int x, int y)
    {
        if (GetLine(x, y))
        {
            line[x, y] = false;
            line[y, x] = false;
            LineKengOpen(x, y);
            //Debug.Log(x+","+y);
        }
        int x1 = x / 5;
        int y1 = x % 5;
        int x2 = y / 5;
        int y2 = y % 5;
        double k1 = (double)(y1 - y2)/ (double)(x1 - x2);
        //Debug.Log("k1:"+k1);
        for (int i = 0; i < 43; i++)
            if (line[x, i] && i != x && i != y)
            {
                int x3 = i / 5;
                int y3 = i % 5;
                double k2 = (double)(y1 - y3)/(double)(x1 - x3);
                //Debug.Log("k2:" + k2);
                if (k1*10 == k2*10)
                {
                    if (line[y, i] && (x2 - x1) * (x3 - x1) < 0)
                    {
                        LineFalse_Powerup(y, i);
                        //////Debug.Log("1-x" + x + "i" + i + "y" + y);
                    }
                    if (line[i, y] && (x2 - x1) * (x3 - x1) < 0)
                        LineFalse_Powerup(i, y);
                }
                if ((k1 == Mathf.Infinity||k1==Mathf.NegativeInfinity)&& (k2 == Mathf.Infinity||k2==Mathf.NegativeInfinity))
                {
                    if (line[y, i] && (y2 - y1) * (y3 - y1) < 0)
                        LineFalse_Powerup(y, i);
                    if (line[i, y] && (y2 - y1) * (y3 - y1) < 0)
                        LineFalse_Powerup(i, y);
                }
            }
        for (int i = 0; i < 43; i++)
            if (line[i, y] && i != x && i != y)
            {
                int x3 = i / 5;
                int y3 = i % 5;
                double k2 = (double)(y2 - y3) / (double)(x2 - x3);
                if (k1*10 == k2*10)
                {
                    if (line[x, i] && (x1 - x2) * (x3 - x2) < 0)
                    {
                        LineFalse_Powerup(x, i);
                        ////Debug.Log("2-x" + x + "i" + i + "y" + y);
                    }
                    if (line[i, x] && (x1 - x2) * (x3 - x2) < 0)
                        LineFalse_Powerup(i, x);
                }
                if ((k1 == Mathf.Infinity || k1 == Mathf.NegativeInfinity) && (k2 == Mathf.Infinity || k2 == Mathf.NegativeInfinity))
                {
                    if (line[x, i] && (y1 - y2) * (y3 - y2) < 0)
                        LineFalse_Powerup(x, i);
                    if (line[i, x] && (y1 - y2) * (y3 - y2) < 0)
                        LineFalse_Powerup(i, x);
                }
            }
    }
    /// <summary>
    /// 将Keng(x,y)置为True
    /// </summary>
    /// <param name='x'>
    /// X.
    /// </param>
    /// <param name='y'>
    /// Y.
    /// </param>
    public void KengTrue(int x, int y)
    {
        keng[x, y] = true;
    }
    /// <summary>
    /// 将Keng(x,y)置为False
    /// </summary>
    /// <param name='x'>
    /// X.
    /// </param>
    /// <param name='y'>
    /// Y.
    /// </param>
    public void KengFalse(int x, int y)
    {
        keng[x, y] = false;
    }
    /// <summary>
    /// Keng(x,y)上线段数增加
    /// </summary>
    /// <param name='x'>
    /// X.
    /// </param>
    /// <param name='y'>
    /// Y.
    /// </param>
    public void LineKengPlus(int x, int y)
    {
        linekeng[x, y]++;
    }
    /// <summary>
    /// Keng(x,y)上线段数减少
    /// </summary>
    /// <param name='x'>
    /// X.
    /// </param>
    /// <param name='y'>
    /// Y.
    /// </param>
    public void LineKengMinus(int x, int y)
    {
        if (linekeng[x, y] > 0)
            linekeng[x, y]--;
        //else
            ////Debug.Log("Minus error");
    }
    /// <summary>
    /// 检测线段xy是否可操作
    /// </summary>
    /// <param name='x'>
    /// X.
    /// </param>
    /// <param name='y'>
    /// Y.
    /// </param>
    public bool IsOperable(int x, int y)
    {
        int x1 = x / 5;
        int y1 = x % 5;
        int x2 = y / 5;
        int y2 = y % 5;
        double k = (double)(Mathf.Abs(y1 - y2)) / (double)(Mathf.Abs(x1 - x2));
        if (x1 == x2)
        {
            if (Mathf.Abs(y1 - y2) == 2)
                return true;
            else
            {
                if (!GetKeng(x1, (y1 + y2) / 2))
                    return true;
                else
                    return false;
            }
        }
        else if (y1 == y2)
        {
            if (Mathf.Abs(x1 - x2) == 2)
                return true;
            else if (Mathf.Abs(x1 - x2) == 4)
            {
                if (!GetKeng((x1 + x2) / 2, y1))
                    return true;
                else
                    return false;
            }
            else if (Mathf.Abs(x1 - x2) == 6)
            {
                if (!GetKeng((x1 + (x2 - x1) / 3), y1) && !GetKeng((x1 + (x2 - x1) * 2 / 3), y1))
                    return true;
                else
                    return false;
            }
            else
            {
                if (!GetKeng((x1 + (x2 - x1) / 4), y1) && !GetKeng((x1 + (x2 - x1) * 2 / 4), y1) && !GetKeng((x1 + (x2 - x1) * 3 / 4), y1))
                    return true;
                else
                    return false;
            }
        }
        else if (Mathf.Abs(x1 - x2) <= 1)
            return true;
        else if (Mathf.Abs(y1 - y2) <= 1)
            return true;
        else if (k != 1 && k != (double)1 / (double)3)
            return true;
        else if (k == 1)
        {
            if (Mathf.Abs(x1 - x2)==1)
                return true;
            else if (Mathf.Abs(x1 - x2) == 2)
            {
                if (!GetKeng((x1 + x2) / 2, (y1 + y2) / 2))
                    return true;
                else
                    return false;
            }
            else if (Mathf.Abs(x1 - x2) == 3)  
            {
                if (!GetKeng(x1 + (x2 - x1) / 3, y1 + (y2 - y1) / 3) && !GetKeng(x1 + (x2 - x1) * 2 / 3, y1 + (y2 - y1) * 2 / 3))
                    return true;
                else
                    return false;
            }
            else
            {
                if (!GetKeng(x1 + (x2 - x1) / 4, y1 + (y2 - y1) / 4) && !GetKeng(x1 + (x2 - x1) * 2 / 4, y1 + (y2 - y1) * 2 / 4) && !GetKeng(x1 + (x2 - x1) * 3 / 4, y1 + (y2 - y1) * 3 / 4))
                    return true;
                else
                    return false;
            }
        }
        else
        {
            if (!GetKeng((x1 + x2) / 2, (y1 + y2) / 2))
                return true;
            else
                return false;
        }
    }
    /// <summary>
    /// 取得Line(x,y)的值
    /// </summary>
    /// <param name='x'>
    /// X.
    /// </param>
    /// <param name='y'>
    /// Y.
    /// </param>
    public bool GetLine(int x, int y)
    {
        return line[x, y];
    }
    /// <summary>
    /// 取得Keng(x,y)的值
    /// </summary>
    /// <param name='x'>
    /// X.
    /// </param>
    /// <param name='y'>
    /// Y.
    /// </param>
    public bool GetKeng(int x, int y)
    {
        return keng[x, y];
    }
    /// <summary>
    /// 取得LineKeng(x,y)的值
    /// </summary>
    /// <param name='x'>
    /// X.
    /// </param>
    /// <param name='y'>
    /// Y.
    /// </param>
    public bool GetLineKeng(int x, int y)
    {
        ////Debug.Log("Keng["+x+","+y+"]" + linekeng[x,y]);
        if (linekeng[x, y] > 0)
            return true;
        else
            return false;
    }
    /// <summary>
    /// 判断是否能连接(x1,y1)与(x2,y2)
    /// </summary>
    /// <returns>
    /// The line switch.
    /// </returns>
    /// <param name='x1'>
    /// If set to <c>true</c> x1.
    /// </param>
    /// <param name='y1'>
    /// If set to <c>true</c> y1.
    /// </param>
    /// <param name='x2'>
    /// If set to <c>true</c> x2.
    /// </param>
    /// <param name='y2'>
    /// If set to <c>true</c> y2.
    /// </param>
    public bool GetLineSwitch(int x1, int y1, int x2, int y2)
    {
        double k = (double)(Mathf.Abs(y1 - y2)) / (double)(Mathf.Abs(x1 - x2));
        if (x1 == x2)
        {
            if (Mathf.Abs(y1 - y2) == 2 )
                return true;
            else if (!GetKeng(x1, (y1 + y2) / 2))
                return true;
            else
                return false;
        }
        else if (y1 == y2)
        {
            if (Mathf.Abs(x1 - x2) == 2)
                return true;
            else if (Mathf.Abs(x1 - x2) == 4)
            {
                if (!GetKeng((x1 + x2) / 2, y1))
                    return true;
                else
                    return false;
            }
            else if (Mathf.Abs(x1 - x2) == 6)
            {
                if (!GetKeng((x1 + (x2 - x1) / 3), y1) && !GetKeng((x1 + (x2 - x1) * 2 / 3), y1))
                    return true;
                else
                    return false;
            }
            else
            {
                if (!GetKeng((x1 + (x2 - x1) / 4), y1) && !GetKeng((x1 + (x2 - x1) * 2 / 4), y1) && !GetKeng((x1 + (x2 - x1) * 3 / 4), y1))
                    return true;
                else
                    return false;
            }
        }
        else if (Mathf.Abs(x1 - x2)<= 1)
            return true;
        else if (Mathf.Abs(y1 - y2) <= 1)
            return true;
        else if (k != 1 && k != (double)1 / (double)3)
            return true;
        else if (k == 1)
        {
            if (Mathf.Abs(x1 - x2) == 1)
                return true;
            else if (Mathf.Abs(x1 - x2) == 2)
            {
                if (!GetKeng((x1 + x2) / 2, (y1 + y2) / 2))
					return true;
                else
                    return false;
            }
            else if (Mathf.Abs(x1 - x2) == 3)
            {
                if (!GetKeng(x1 + (x2 - x1) / 3, y1 + (y2 - y1) / 3) && !GetKeng(x1 + (x2 - x1) * 2 / 3, y1 + (y2 - y1) * 2 / 3))
                    return true;
                else
                    return false;
            }
            else
            {
                if (!GetKeng(x1 + (x2 - x1) / 4, y1 + (y2 - y1) / 4) && !GetKeng(x1 + (x2 - x1) * 2 / 4, y1 + (y2 - y1) * 2 / 4) && !GetKeng(x1 + (x2 - x1) * 3 / 4, y1 + (y2 - y1) * 3 / 4))
                    return true;
                else
                    return false;
            }
        }
        else
        {
            if (!GetKeng((x1 + x2) / 2, (y1 + y2) / 2))
                return true;
            else
                return false;
        }
    }
    /// <summary>
    /// 关闭(l1,l2)上面的坑
    /// </summary>
    /// <param name='l1'>
    /// L1.
    /// </param>
    /// <param name='l2'>
    /// L2.
    /// </param>
    public void LineKengClose(int l1, int l2)
    {
        int x1 = l1 / 5;
        int y1 = l1 % 5;
        int x2 = l2 / 5;
        int y2 = l2 % 5;
        double k = (double)(Mathf.Abs(y1 - y2)) / (double)(Mathf.Abs(x1 - x2));
        if (x1 == x2)
        {
            if (Mathf.Abs(y1 - y2)==2)
                return;
            else
                LineKengPlus(x1, (y1 + y2) / 2);
        }
        else if (y1 == y2)
        {
            if (Mathf.Abs(x1 - x2)==2)
                return;
            else if (Mathf.Abs(x1 - x2) == 4)
                LineKengPlus((x1 + x2) / 2, y1);
            else if (Mathf.Abs(x1 - x2) == 6)
            {
                LineKengPlus((x1 + (x2 - x1) / 3), y1);
                LineKengPlus((x1 + (x2 - x1) * 2 / 3), y1);
            }
            else
            {
                LineKengPlus((x1 + (x2 - x1) / 4), y1);
                LineKengPlus((x1 + (x2 - x1) * 2 / 4), y1);
                LineKengPlus((x1 + (x2 - x1) * 3 / 4), y1);
            }
        }
        else if (Mathf.Abs(x1 - x2) <= 1)
            return;
        else if (Mathf.Abs(y1 - y2) <= 1)
            return;
        else if (k != 1 && k != (double)1 / (double)3)
            return;
        else if (k == 1)
        {
            if (Mathf.Abs(x1 - x2) == 1)
                return;
            else if (Mathf.Abs(x1 - x2) == 2)
                LineKengPlus((x1 + x2) / 2, (y1 + y2) / 2);
            else if (Mathf.Abs(x1 - x2) == 3)
            {
                LineKengPlus(x1 + (x2 - x1) / 3, y1 + (y2 - y1) / 3);
                LineKengPlus(x1 + (x2 - x1) * 2 / 3, y1 + (y2 - y1) * 2 / 3);
            }
            else
            {
                LineKengPlus(x1 + (x2 - x1) / 4, y1 + (y2 - y1) / 4);
                LineKengPlus(x1 + (x2 - x1) * 2 / 4, y1 + (y2 - y1) * 2 / 4);
                LineKengPlus(x1 + (x2 - x1) * 3 / 4, y1 + (y2 - y1) * 3 / 4);
            }
        }
        else
            LineKengPlus((x1 + x2) / 2, (y1 + y2) / 2);
    }
    /// <summary>
    /// 打开(l1,l2)上面的坑
    /// </summary>
    /// <param name='l1'>
    /// L1.
    /// </param>
    /// <param name='l2'>
    /// L2.
    /// </param>
    public void LineKengOpen(int l1, int l2)
    {
        int x1 = l1 / 5;
        int y1 = l1 % 5;
        int x2 = l2 / 5;
        int y2 = l2 % 5;
        double k = (double)(Mathf.Abs(y1 - y2)) / (double)(Mathf.Abs(x1 - x2));
        if (x1 == x2)
        {
            if (Mathf.Abs(y1 - y2)==2)
                return;
            else
            {
                LineKengMinus(x1, (y1 + y2) / 2);
            }
        }
        else if (y1 == y2)
        {
            if (Mathf.Abs(x1 - x2)==2)
                return;
            else if (Mathf.Abs(x1 - x2) == 4)
            {
                LineKengMinus((x1 + x2) / 2, y1);
            }
            else if (Mathf.Abs(x1 - x2) == 6)
            {
                LineKengMinus((x1 + (x2 - x1) / 3), y1);
                LineKengMinus((x1 + (x2 - x1) * 2 / 3), y1);
            }
            else
            {
                LineKengMinus((x1 + (x2 - x1) / 4), y1);
                LineKengMinus((x1 + (x2 - x1) * 2 / 4), y1);
                LineKengMinus((x1 + (x2 - x1) * 3 / 4), y1);
            }
        }
        else if (Mathf.Abs(x1 - x2) <= 1)
            return;
        else if (Mathf.Abs(y1 - y2) <= 1)
            return;
        else if (k != 1 && k != (double)1 / (double)3)
            return;
        else if (k == 1)
        {
            if (Mathf.Abs(x1 - x2) == 1)
                return;
            else if (Mathf.Abs(x1 - x2) == 2)
            {
                LineKengMinus((x1 + x2) / 2, (y1 + y2) / 2);
            }
            else if (Mathf.Abs(x1 - x2) == 3)
            {
                LineKengMinus(x1 + (x2 - x1) / 3, y1 + (y2 - y1) / 3);
                LineKengMinus(x1 + (x2 - x1) * 2 / 3, y1 + (y2 - y1) * 2 / 3);
            }
            else
            {
                LineKengMinus(x1 + (x2 - x1) / 4, y1 + (y2 - y1) / 4);
                LineKengMinus(x1 + (x2 - x1) * 2 / 4, y1 + (y2 - y1) * 2 / 4);
                LineKengMinus(x1 + (x2 - x1) * 3 / 4, y1 + (y2 - y1) * 3 / 4);
            }
        }
        else
        {
            LineKengMinus((x1 + x2) / 2, (y1 + y2) / 2);
        }
    }
    /// <summary>
    /// 根据传入的列坐标返回标准列坐标实现吸附
    /// </summary>
    /// <param name='z'>
    /// Z.
    /// </param>
    public float getz(float z)
    {
        if (z > ((rowKey[4] + rowKey[3]) / 2))
            return rowKey[4];
        else if (z > ((rowKey[3] + rowKey[2]) / 2))
            return rowKey[3];
        else if (z > ((rowKey[2] + rowKey[1]) / 2))
            return rowKey[2];
        else if (z > ((rowKey[1] + rowKey[0]) / 2))
            return rowKey[1];
        else
            return rowKey[0];
    }
    /// <summary>
    /// 根据传入的行坐标返回标准行坐标实现吸附1
    /// </summary>
    /// <param name='x'>
    /// X.
    /// </param>
    public float getx1(float x)
    {
        if (x == StartCoordinate)
        {
            ////Debug.Log("StartCoordinate");
            return -1;
        }
        else if (x > lineKey[7])
            return lineKey[8];
        else if (x > lineKey[5])
            return lineKey[6];
        else if (x > lineKey[3])
            return lineKey[4];
        else if (x > lineKey[1])
            return lineKey[2];
        else
            return lineKey[0];
    }
    /// <summary>
    /// 根据传入的行坐标返回标准行坐标实现吸附2
    /// </summary>
    /// <param name='x'>
    /// X.
    /// </param>
    public float getx2(float x)
    {
        if (x == StartCoordinate)
        {
            ////Debug.Log("StartCoordinate");
            return -1;
        }
        else if (x > lineKey[5])
            return lineKey[6];
        else if (x > lineKey[3])
            return lineKey[4];
        else
            return lineKey[2];
    }
    /// <summary>
    /// 根据传入的行坐标返回标准行坐标实现吸附3
    /// </summary>
    /// <param name='x'>
    /// X.
    /// </param>
    public float getx3(float x)
    {
        if (x == StartCoordinate)
        {
            ////Debug.Log("StartCoordinate");
            return -1;
        }
        else if (x > lineKey[6])
            return lineKey[7];
        else if (x > lineKey[4])
            return lineKey[5];
        else if (x > lineKey[2])
            return lineKey[3];
        else
            return lineKey[1];
    }
    /// <summary>
    /// 根据传入的标准行坐标返回行号
    /// </summary>
    /// <returns>
    /// The kx.
    /// </returns>
    /// <param name='x'>
    /// X.
    /// </param>
    public int getKx(float x)
    {
        if (x == lineKey[8]) return 8;
        else if (x == lineKey[7]) return 7;
        else if (x == lineKey[6]) return 6;
        else if (x == lineKey[5]) return 5;
        else if (x == lineKey[4]) return 4;
        else if (x == lineKey[3]) return 3;
        else if (x == lineKey[2]) return 2;
        else if (x == lineKey[1]) return 1;
        else if (x == lineKey[0]) return 0;
        else return -1;
    }
    /// <summary>
    /// 根据传入的标准列坐标返回列号
    /// </summary>
    /// <returns>
    /// The ky.
    /// </returns>
    /// <param name='y'>
    /// Y.
    /// </param>
    public int getKy(float y)
    {
        if (y == rowKey[4]) return 4;
        else if (y == rowKey[3]) return 3;
        else if (y == rowKey[2]) return 2;
        else if (y == rowKey[1]) return 1;
        else if (y == rowKey[0]) return 0;
        else return -1;

    }
    // Update is called once per frame
    void Update()
    {

    }
}