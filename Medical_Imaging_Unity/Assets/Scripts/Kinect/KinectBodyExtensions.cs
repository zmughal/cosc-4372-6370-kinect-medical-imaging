using UnityEngine;
using System.Collections;
using WindowsKinect = Windows.Kinect;

public static class KinectBodyExtensions
{
    public static Vector3 GetPosition(this WindowsKinect.Body body, WindowsKinect.JointType jointType)
    {
        var position = body.Joints[jointType].Position;
        return new Vector3(position.X, position.Y, position.Z);
    }
}
