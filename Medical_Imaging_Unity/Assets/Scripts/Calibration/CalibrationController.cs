using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Kinect = Windows.Kinect;

public class CalibrationData
{
    public float Total { get; private set; }
    public int NumSamples { get; private set; }
    public float Min { get; private set; }
    public float Max { get; private set; }

    public CalibrationData()
    {
        Reset();
    }

    public float Average
    {
        get { return Total / (float)NumSamples; }
    }

    public void AddData(float newData)
    {
        Total += newData;
        Min = Mathf.Min(newData, Min);
        Max = Mathf.Max(newData, Max);
        NumSamples++;
    }

    public void Reset()
    {
        Total = 0f;
        NumSamples = 0;
        Min = float.PositiveInfinity;
        Max = 0f;
    }

    public override string ToString()
    {
        string format = "0.00";
        return string.Format("Avg: {0}\nMin: {1}\nMax: {2}\n",
            Average.ToString(format), Min.ToString(format), Max.ToString(format));
    }
}

public static class CalibrationThreshold
{
    public static float ShoulderAndHipRatio;
    public static float UpperbodyLength;

    static CalibrationThreshold()
    {
        ShoulderAndHipRatio = 0.30f;
        UpperbodyLength = 0.10f;
    }

    // maybe implement a Match method here - compare with PlayerPrefs
}

public class CalibrationController : MonoBehaviour
{
    public GameObject KinectData;
    public GUIText DisplayText;

    private KinectController kinectData;
    private ulong? trackingId = null;

    private float currentShoulderAndHipRatio;
    private float currentUpperbodyLength;

    private CalibrationData shoulderAndHipRatio = new CalibrationData();
    private CalibrationData upperbodyLength = new CalibrationData();

    private const float calibrationTime = 1f;
    private float calibrationTimer = 0f;

    private const float ftToMeter = 0.3048f;
    private const float minDistance = 4.5f * ftToMeter;
    private const float maxDistance = 6.5f * ftToMeter;
    private float optimalDistance = (minDistance + maxDistance) / 2f;

    void Start()
    {
        currentShoulderAndHipRatio = PlayerPrefs.GetFloat("ShoulderAndHipRatio", 0f);
        currentUpperbodyLength = PlayerPrefs.GetFloat("UpperbodyLength", 0f);
    }

	void Update()
    {
        kinectData = KinectData.GetComponent<KinectController>();

        Kinect.Body closestBody = GetClosestBody();

        if (closestBody == null)
        {
            ResetCalibration(null, "body not found");
            return;
        }

        ulong currentClosestTrackingId = closestBody.TrackingId;

        if (trackingId != currentClosestTrackingId)
        {
            ResetCalibration(currentClosestTrackingId, "body not found");
            return;
        }

        Kinect.Body currentBody = kinectData.Bodies.Find(body => body.TrackingId == trackingId);

        if (calibrationTimer < calibrationTime)
        {
            shoulderAndHipRatio.AddData(GetShoulderAndHipRatio(
                GetBodyPosition(currentBody, Kinect.JointType.ShoulderLeft),
                GetBodyPosition(currentBody, Kinect.JointType.ShoulderRight),
                GetBodyPosition(currentBody, Kinect.JointType.HipLeft),
                GetBodyPosition(currentBody, Kinect.JointType.HipRight)));

            upperbodyLength.AddData(GetUpperbodyLength(
                GetBodyPosition(currentBody, Kinect.JointType.Head),
                GetBodyPosition(currentBody, Kinect.JointType.Neck),
                GetBodyPosition(currentBody, Kinect.JointType.SpineBase)));

            DisplayText.text = string.Format("Time: {2}\n\nSH: {0}\nUL: {1}",
                shoulderAndHipRatio.ToString(),
                upperbodyLength.ToString(),
                calibrationTimer.ToString("0.00"));

            calibrationTimer += Time.deltaTime;
        }
        else
        {
            if (Application.loadedLevelName == "MainMenu" || Application.loadedLevel == 0)
            {
                SetPlayerPrefs(trackingId.Value, shoulderAndHipRatio.Average, upperbodyLength.Average);
                Application.LoadLevel(1); // change this later
            }
            else
            {
                if (Match(shoulderAndHipRatio.Average, upperbodyLength.Average))
                {
                    Application.LoadLevel(Application.loadedLevel);
                }
                else
                {
                    ResetCalibration(null, "not a match");
                }
            }
        }
	}

    private Kinect.Body GetClosestBody()
    {
        var allBodiesInRange = kinectData.Bodies.FindAll(IsInRange);
        return allBodiesInRange == null ? null : ClosestToOptimal(allBodiesInRange);
    }

    private void ResetCalibration(ulong? newTrackingId, string displayText)
    {
        trackingId = newTrackingId;
        calibrationTimer = 0f;
        shoulderAndHipRatio.Reset();
        upperbodyLength.Reset();
        DisplayText.text = displayText;
    }

    private bool Match(float sh, float ub)
    {
        return Mathf.Abs(sh - currentShoulderAndHipRatio) <= CalibrationThreshold.ShoulderAndHipRatio &&
            Mathf.Abs(ub - currentUpperbodyLength) <= CalibrationThreshold.UpperbodyLength;
    }

    private void SetPlayerPrefs(ulong tId, float shRatioAverage, float ubLengthAverage)
    {
        PlayerPrefs.SetString("TrackingId", tId.ToString());
        PlayerPrefs.SetFloat("ShoulderAndHipRatio", shRatioAverage);
        PlayerPrefs.SetFloat("UpperbodyLength", ubLengthAverage);
    }

    private Vector3 GetBodyPosition(Kinect.Body body, Kinect.JointType jointType)
    {
        Kinect.CameraSpacePoint pos = body.Joints[jointType].Position;
        return new Vector3(pos.X, pos.Y, pos.Z);
    }

    private float GetShoulderAndHipRatio(Vector3 shoulderLeft, Vector3 shoulderRight, Vector3 hipLeft, Vector3 hipRight)
    {
        return Vector3.Distance(shoulderLeft, shoulderRight) / Vector3.Distance(hipLeft, hipRight);
    }

    private float GetUpperbodyLength(Vector3 head, Vector3 neck, Vector3 spineBase)
    {
        return Vector3.Distance(head, neck) + Vector3.Distance(neck, spineBase);
    }

    private Kinect.Body ClosestToOptimal(List<Kinect.Body> bodies)
    {
        Kinect.Body minBody = null;
        float min = 100f;

        foreach (Kinect.Body body in bodies)
        {
            float distanceFromOptimal = DistanceFromOptimal(body);
            if (distanceFromOptimal < min)
            {
                minBody = body;
                min = distanceFromOptimal;
            }
        }

        return minBody;
    }
    
    private float DistanceFromOptimal(Kinect.Body body)
    {
        return Mathf.Abs(optimalDistance - SpineBasePosition(body).z);
    }

    private bool IsInRange(Kinect.Body body)
    {
        Vector3 spineBasePosition = SpineBasePosition(body);
        float spineBaseDistance = SpineBasePosition(body).z;
        return spineBaseDistance >= minDistance && spineBaseDistance <= maxDistance &&
            Mathf.Abs(spineBasePosition.x) < 2f; // check this value
    }

    private Vector3 SpineBasePosition(Kinect.Body body)
    {
        var spineBasePosition = body.Joints[Kinect.JointType.SpineBase].Position;
        return new Vector3(spineBasePosition.X, spineBasePosition.Y, spineBasePosition.Z);
    }
}
