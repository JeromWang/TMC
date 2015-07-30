using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Point
{
    int x, y;
    int uni;//一维表示法
    public Point(int x1,int y1)
    {
        x = x1;
        y = y1;
        uni = 5 * x + y;
    }
    public Point(int uni1)
    {
        uni = uni1;
        x = uni / 5;
        y = uni % 5;
    }
    public int GetUni()
    {
        return uni;
    }
    public int GetX()
    {
        return x;
    }
    public int GetY()
    {
        return y;
    }
    public static bool operator ==(Point p1,Point p2)
    {
        if((p1 as object)!=null && (p2 as object)!=null)
        {
            return p1.GetUni() == p2.GetUni();
        }
        if ((p1 as object) == null && (p2 as object) == null)
            return true;
        return false;
    }
    public static bool operator !=(Point p1, Point p2)
    {
        return !(p1 == p2);
    }
}
public class Line
{
    Point point1, point2;

    public Point Point1
    {
        get { return point1; }
        set { point1 = value; }
    }

    public Point Point2
    {
        get { return point2; }
        set { point2 = value; }
    }
    public Line(Point p1,Point p2)
    {
        point1 = p1;
        point2 = p2;
    }
    public Line (int x1,int y1,int x2,int y2)
    {
        point1 = new Point(x1, y1);
        point2 = new Point(x2, y2);
    }
    public static bool operator ==(Line l1, Line l2)
    {
        if ((l1 as object) != null && (l2 as object) != null)
        {
            return l1.point1.GetUni() == l2.point1.GetUni() && l1.point2.GetUni() == l2.point2.GetUni()
                || l1.point1.GetUni() == l2.point2.GetUni() && l1.point2.GetUni() == l2.point1.GetUni();
        }
        if ((l1 as object) == null && (l2 as object) == null)
            return true;
        return false;
    }
    public static bool operator !=(Line l1, Line l2)
    {
        return !(l1 == l2);
    }
    
}
public class MagicCircleMananger : MonoBehaviour
{

	//public int CameraInUse;//当前启用的摄像机

	
    public bool[,] keng = new bool[9, 5];//法阵里面的点
    public int[,] linekeng = new int[9, 5];//法阵点上面线的条数
    public bool[,] line = new bool[43, 43]; //魔法连线是否存在的bool数组

    int LineTrueDepth;//递归函数的层数
    int LineFalseDepth;//递归函数的层数
    public float[] rowKey = new float[5];//标准点阵列坐标
    public float[] lineKey = new float[9];//标准点阵行坐标

    public float StartCoordinate = 1006f;//起始坐标

    const float startRowKey = 850f;//中心位置
    const float startLineKey = 1000f;//中心位置
    const float scale = 0.08f;//转换比例
    const float rowGap = 13f;
    const float lineGap = 7.5f;
    
    void Initialize()
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
    }
    void IniKeyPos()
    {
        rowKey[0] = startRowKey - 2 * rowGap * scale;
        rowKey[1] = startRowKey - rowGap * scale;
        rowKey[2] = startRowKey;
        rowKey[3] = startRowKey + rowGap * scale;
        rowKey[4] = startRowKey + 2 * rowGap * scale;
        lineKey[0] = startLineKey - 4 * lineGap * scale;
        lineKey[1] = startLineKey - 3 * lineGap * scale;
        lineKey[2] = startLineKey - 2 * lineGap * scale;
        lineKey[3] = startLineKey - lineGap * scale;
        lineKey[4] = startLineKey;
        lineKey[5] = startLineKey + lineGap * scale; ;
        lineKey[6] = startLineKey + 2 * lineGap * scale;
        lineKey[7] = startLineKey + 3 * lineGap * scale;
        lineKey[8] = startLineKey + 4 * lineGap * scale;
    }
    
    public virtual void Destroy()
    {
        Destroy(this);
    }
    protected void Start()
    {
        Initialize();
        IniKeyPos();
        LineTrueDepth = 0;
        LineFalseDepth = 0;
        
    }
    //在RayTest里还有一个副本，那个专门删3d线的
    public void DeleteLine(int x, int y)
    {
        LineFalse(x, y);
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
    /// <summary>
    /// 将Line(x,y)置为True
    /// </summary>
    public void LineTrue(int x, int y)
    {
        LineTrueDepth++;
        if (!GetLine(new Point(x), new Point(y)))
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
            LineChange();
        LineTrueDepth--;
    }
    virtual protected void LineChange()
    {

    }
    /// <summary>
    /// 将Line(x,y)置为False
    /// </summary>
    public void LineFalse(int x, int y)
    {
        if (GetLine(new Point(x),new Point(y)))
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
        LineChange();
    }
    public void LineFalse_Powerup(int x, int y)
    {
        if (GetLine(new Point(x), new Point(y)))
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
    public void KengTrue(int x, int y)
    {
        keng[x, y] = true;
    }
    public void KengTrue(Point p)
    {
        keng[p.GetX(), p.GetY()] = true;
    }
    /// <summary>
    /// 将Keng(x,y)置为False
    /// </summary>
    public void KengFalse(int x, int y)
    {
        keng[x, y] = false;
    }
    /// Keng(x,y)上线段数增加
    public void LineKengPlus(int x, int y)
    {
        linekeng[x, y]++;
    }
    /// Keng(x,y)上线段数减少
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
    public bool IsOperable(Line line)
    {
        int x1 = line.Point1.GetX();
        int y1 = line.Point1.GetY();
        int x2 = line.Point2.GetX();
        int y2 = line.Point2.GetY();
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
    public bool GetLine(Point p1,Point p2)
    {
        return line[p1.GetUni(), p2.GetUni()];
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
    public int getKy(float y)
    {
        if (y == rowKey[4]) return 4;
        else if (y == rowKey[3]) return 3;
        else if (y == rowKey[2]) return 2;
        else if (y == rowKey[1]) return 1;
        else if (y == rowKey[0]) return 0;
        else return -1;

    }
}
