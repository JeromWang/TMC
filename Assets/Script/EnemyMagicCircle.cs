using UnityEngine;
using System.Collections;

public class EnemyMagicCircle : MagicCircleMananger
{

    bool CanDraw(Line line)
    {
        if (GetLine(line.point1, line.point2))//已经有线了
            return false;
        if (keng[line.point1.GetX(), line.point1.GetY()] == false)
            return false;
        if (keng[line.point2.GetX(), line.point2.GetY()] == false)
            return false;
        return true;
    }
    public bool EDrawLine(Line line)
    {
        if (CanDraw(line) == false)
            return false;
        LineTrue(line.point1.GetUni(), line.point2.GetUni());
        return true;
    }
    public void Destroy()
    {
        base.Destroy();
    }
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
