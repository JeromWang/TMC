using UnityEngine;
using System.Collections;

public class HeroMagicCircle : MagicCircleMananger {

    GameObject[] level2Hide = new GameObject[6];//第二关需要被隐藏的点

    public delegate void ChangeLineEvent();
    public event ChangeLineEvent ChangeLine;
    protected override void LineChange()
    {
        ChangeLine();
    }
    void Start()
    {
        base.Start();
        if (LevelManager.Instance.level == 2)
        {
            Level2Start();
        }
    }
    void Level2Start()
    {
        linekeng[3, 3] = 1;
        linekeng[5, 3] = 1;
        linekeng[2, 2] = 1;
        linekeng[6, 2] = 1;
        linekeng[3, 1] = 1;
        linekeng[5, 1] = 1;

        level2Hide[0] = GameObject.Find("Keng1/1/3.1");
        level2Hide[1] = GameObject.Find("Keng1/1/5.1");
        level2Hide[2] = GameObject.Find("Keng1/2/2.2");
        level2Hide[3] = GameObject.Find("Keng1/2/6.2");
        level2Hide[4] = GameObject.Find("Keng1/3/5.3");
        level2Hide[5] = GameObject.Find("Keng1/3/3.3");
        for (int i = 0; i < 6; i++)
        {
            level2Hide[i].SetActive(false);
        }
    }
    void Level2End()
    {
        for (int i = 0; i < 6; i++)
        {
            level2Hide[i].SetActive(true);
        }
    }
    public void Destroy()
    {
        if (LevelManager.Instance.level == 2)
        {
            Level2End();
        }
        base.Destroy();
    }
}
