using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using WindowsKinect = Windows.Kinect;

public enum FlyMode { None, FirstPerson, ThirdPerson }

public class PlaneController : MonoBehaviour
{
    public GameObject Player;
    public GameObject Kinect;
    public GameObject Fly;

    public FlyMode FlyMode = FlyMode.None;

    private const float takeOffTime = 5f;
    private const float speed = 10f;
    private float takeOffTimer = 0f;
	
	void Update()
    {
        PlayerController player = Player.GetComponent<PlayerController>();
        KinectController kinect = Kinect.GetComponent<KinectController>();
        FlyDetector fly = Fly.GetComponent<FlyDetector>();

        if (kinect.PlayerSkeleton == null)
        {
            return;
        }

        if (fly.IsActivated)
        {
            float elapsedTime = Time.deltaTime;

            if (takeOffTimer < takeOffTime)
            {
                transform.position += Vector3.up * speed * elapsedTime;
                takeOffTimer += elapsedTime;
            }
            else
            {
                WindowsKinect.Body playerSkeleton = kinect.PlayerSkeleton;

                fly.Set(
                    playerSkeleton.GetPosition(WindowsKinect.JointType.ShoulderLeft),
                    playerSkeleton.GetPosition(WindowsKinect.JointType.ShoulderRight),
                    playerSkeleton.GetPosition(WindowsKinect.JointType.ElbowLeft),
                    playerSkeleton.GetPosition(WindowsKinect.JointType.ElbowRight));

                //Debug.Log(string.Format("{0} | {1}", transform.rotation.z, fly.Angle));
                if (Mathf.Abs(transform.rotation.z - fly.Angle) > 0.02f)
                {
                    FlyDirection rotation = fly.FlyDirection;
                    
                    float degree = 0f;

                    if (rotation == FlyDirection.Right)
                    {
                        degree = fly.Angle > transform.rotation.z ? 1f : -1f;
                    }
                    else if (rotation == FlyDirection.Left)
                    {
                        degree = fly.Angle < transform.rotation.z ? -1f : 1f;
                    }

                    transform.Rotate(new Vector3(0f, 0f, 1f), 2f * degree * Mathf.PI / 2f * elapsedTime);
                }

                //transform.position += new Vector3(0f, 0f, -1f) * speed * elapsedTime;
            }
        }
        else if (!player.renderer.enabled)
        {
            fly.Activate();
        }
	}
}
