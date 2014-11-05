using UnityEngine;
using System.Collections;

public enum FlyDirection { None, Left, Right, Up, Down }

public class FlyDetector : Detector
{
    public FlyDirection FlyDirection { get; private set; }
    public float Angle { get; private set; }
    
    private Vector3 shoulderLeft;
    private Vector3 shoulderRight;
    private Vector3 elbowLeft;
    private Vector3 elbowRight;

    public void Set(Vector3 shoulderLeft, Vector3 shoulderRight, Vector3 elbowLeft, Vector3 elbowRight)
    {
        this.shoulderLeft = shoulderLeft;
        this.shoulderRight = shoulderRight;
        this.elbowLeft = elbowLeft;
        this.elbowRight = elbowRight;
    }

    public override void Start()
    {
        FlyDirection = FlyDirection.None;
        Angle = 0f;

        base.Start();
    }

    void Update()
    {
        if (IsActivated)
        {
            float angleLeft = GetAngle(elbowLeft, shoulderLeft);
            float angleRight = GetAngle(elbowRight, shoulderRight);

            const float minFlyDownAngle = -Mathf.PI / 4f;

            if (angleRight < 0f && angleLeft > 0f)
            {
                FlyDirection = FlyDirection.Right;
                Angle = (angleLeft - angleRight) / 2f;
            }
            else if (angleLeft < 0f && angleRight > 0f)
            {
                FlyDirection = FlyDirection.Left;
                Angle = 2f * Mathf.PI - (angleRight - angleLeft) / 2f;
            }
            else if (angleLeft > 0f && angleRight > 0f)
            {
                FlyDirection = FlyDirection.Up;
                Angle = 0f;
            }
            else if (angleLeft < 0f && angleLeft > minFlyDownAngle &&
                angleRight < 0f && angleRight > minFlyDownAngle)
            {
                FlyDirection = FlyDirection.Down;
                Angle = 0f;
            }
            else
            {
                FlyDirection = FlyDirection.None;
                Angle = 0f;
            }

            //Angle *= 180f / Mathf.PI;
        }
    }

    private float GetAngle(Vector3 elbow, Vector3 shoulder)
    {
        float length = elbow.y - shoulder.y;
        return Mathf.Atan(length / Mathf.Abs(elbow.x - shoulder.x));
    }

    public override void Deactivate()
    {
        FlyDirection = FlyDirection.None;
        base.Deactivate();
    }
}
