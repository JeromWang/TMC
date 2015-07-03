using UnityEngine;
using System.Collections;

public class Rotation : MonoBehaviour
{

    public Vector3 OriginRotationAxis;
    public Vector3 RotationAxis;
    public bool isActive;
    public bool isOrigin;
    float time = 0;

    void Start()
    {
        isActive = true;
//        RotationAxis = OriginRotationAxis;
        OriginRotationAxis = RotationAxis;
        isOrigin = true;
    }

    void Update()
    {
        if (isActive)
        {
            this.transform.Rotate(RotationAxis * Time.deltaTime);
        }
        if (!isOrigin)
        {
            time = time + Time.deltaTime;
            //if(time>2)
            //{
            //    RotationAxis = 1/2*OriginRotationAxis;
            //}
            if(time>4)
            {
                time = 0;
                isOrigin = true;
                RotationAxis = OriginRotationAxis;
            }
        }
    }

}
