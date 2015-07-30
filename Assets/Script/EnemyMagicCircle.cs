using UnityEngine;
using System.Collections;

public class EnemyMagicCircle : MagicCircleMananger
{

    bool CanDraw(Line line)
    {
        if (GetLine(line.Point1, line.Point2))//已经有线了
            return false;
        if (keng[line.Point1.GetX(), line.Point1.GetY()] == false)
            return false;
        if (keng[line.Point2.GetX(), line.Point2.GetY()] == false)
            return false;
        return true;
    }
    public bool EDrawLine(Line line)
    {
        if (CanDraw(line) == false)
            return false;
        LineTrue(line.Point1.GetUni(), line.Point2.GetUni());
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
