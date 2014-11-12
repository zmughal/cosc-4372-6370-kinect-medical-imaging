using UnityEngine;
using System.Collections;
using Windows.Kinect;

public class HandBall : MonoBehaviour
{
	public GameObject Kinect;
	public JointType whichHand = JointType.HandRight;
	public float speedMultiplier = 3.0f;
		
	void Update()
	{
		KinectController kinect = Kinect.GetComponent<KinectController>();
		
		Vector3 newPostion = kinect.PlayerSkeleton.GetPosition(whichHand);
		newPostion.z = 0;
		newPostion.x *= speedMultiplier;
		newPostion.y *= speedMultiplier;
		transform.position = newPostion;
	}
}