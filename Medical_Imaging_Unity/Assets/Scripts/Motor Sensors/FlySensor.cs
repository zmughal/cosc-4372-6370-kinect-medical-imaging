using UnityEngine;
using System.Collections;

public enum FlyDirection
{
    None, Left, Right
}

public class FlySensor : Sensor
{
    public FlyDirection Rotation { get; private set; }
    
    private Vector3 shoulderLeft;
    private Vector3 shoulderRight;
    private Vector3 elbowLeft;
    private Vector3 elbowRight;

    public void Set(Vector3 sl, Vector3 sr, Vector3 el, Vector3 er)
    {
        shoulderLeft = sl;
        shoulderRight = sr;
        elbowLeft = el;
        elbowRight = er;
    }

    public override void Start()
    {
        Rotation = FlyDirection.None;
        base.Start();
    }

    void Update()
    {
        if (IsActivated)
        {
            float angleLeft = GetAngle(elbowLeft, shoulderLeft);
            float angleRight = GetAngle(elbowRight, shoulderRight);

            if (angleRight < 0f && angleLeft > 0f)
            {
                Rotation = FlyDirection.Right;
            }
            else if (angleLeft < 0f && angleRight > 0f)
            {
                Rotation = FlyDirection.Left;
            }
            else
            {
                Rotation = FlyDirection.None;
            }
        }
    }

    //private float AverageAngle(float angleLeft, float angleRight)
    //{
    //    return (angleLeft - angleRight) / 2f;
    //}

    private float GetAngle(Vector3 elbow, Vector3 shoulder)
    {
        float length = elbow.y - shoulder.y;
        return Mathf.Atan(length / Mathf.Abs(elbow.x - shoulder.x));
    }

    public override void Deactivate()
    {
        Rotation = FlyDirection.None;
        base.Deactivate();
    }
}
