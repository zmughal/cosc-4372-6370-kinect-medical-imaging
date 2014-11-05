using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using WindowsKinect = Windows.Kinect;

public class KinectController : MonoBehaviour
{
    public GameObject BodySourceManager;
    public WindowsKinect.Body PlayerSkeleton { get; private set; }

    private ulong? trackingId = null;

    void FixedUpdate()
    {
        BodySourceManager bodyManager = BodySourceManager.GetComponent<BodySourceManager>();

        WindowsKinect.Body[] allBodiesInTheScreen = bodyManager == null
            ? new WindowsKinect.Body[0]
            : GetAllSkeletonsInTheScreen(bodyManager);

        if (allBodiesInTheScreen.Length < 1)
        {
            PlayerSkeleton = null;
            return;
        }

        bool noTrackingId = trackingId == null;
        WindowsKinect.Body firstBodyEnteringTheScreen = allBodiesInTheScreen[0];
        PlayerSkeleton = noTrackingId ? firstBodyEnteringTheScreen : GetCurrentSkeleton(trackingId.Value, allBodiesInTheScreen);
        trackingId = noTrackingId
            ? (ulong?)firstBodyEnteringTheScreen.TrackingId
            : (PlayerSkeleton == null ? (ulong?)null : (ulong?)PlayerSkeleton.TrackingId);
    }

    private WindowsKinect.Body GetCurrentSkeleton(ulong currentTrackingId, WindowsKinect.Body[] allBodiesInTheScreen)
    {
        return System.Array.Find(allBodiesInTheScreen, body => body.TrackingId == currentTrackingId);
    }

    private WindowsKinect.Body[] GetAllSkeletonsInTheScreen(BodySourceManager bodyManager)
    {
        WindowsKinect.Body[] bodyManagerData = bodyManager.GetData();
        WindowsKinect.Body[] trackedBodies = bodyManagerData == null
            ? new WindowsKinect.Body[0]
            : System.Array.FindAll(bodyManagerData, body => body.IsTracked && IsInRange(body));
        return trackedBodies.Length == 0 ? new WindowsKinect.Body[0] : trackedBodies;
    }

    private bool IsInRange(WindowsKinect.Body skeleton)
    {
        const float ftToMeter = 0.3048f;
        const float minDistance = 4.5f * ftToMeter;
        const float maxDistance = 6.5f * ftToMeter;

        Vector3 spineBasePosition = skeleton.GetPosition(WindowsKinect.JointType.SpineBase);
        return spineBasePosition.z >= minDistance &&
            spineBasePosition.z <= maxDistance &&
            Mathf.Abs(spineBasePosition.x) < 1.5f;
    }
}
