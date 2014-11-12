using UnityEngine;
using System.Collections;
using Windows.Kinect;

public class GestureBuilderDetector : MonoBehaviour
{
	public GameObject Kinect;


	public Vector3 lastMouseCoordinate;
	public Vector3 lastRHCoordinate;

	// Update is called once per frame
	void Update ()
	{
		KinectController kinect = Kinect.GetComponent<KinectController>();
		
		Vector3 rHPosition = kinect.PlayerSkeleton.GetPosition(JointType.HandRight);


		//Vector3 mouseDelta = Input.mousePosition - lastMouseCoordinate;
		Vector3 rHDelta = rHPosition - lastRHCoordinate;

		//lastMouseCoordinate = Input.mousePosition;
		lastRHCoordinate = rHPosition;

		//will get a 0/0 error if mouse does not move
		Vector3 direction = rHDelta.normalized;

		float dot = Vector3.Dot (direction, Vector3.up);
		if (dot > 0.5) { //can be >= for sideways
			//UP
			Debug.Log ("Moving up 2");
		} else if (dot < -0.5) { //can be <= for sideways
			//DOWN
			Debug.Log ("Moving down 2");
		} else {
			dot = Vector3.Dot (direction, Vector3.right);
			if (dot > 0.5) { //can be >= for sideways
				Debug.Log ("Moving right 2");
				//RIGHT
			} else if (dot < -0.5) { //can be <= for sideways
				Debug.Log ("Moving left 2");
				//LEFT
			}
		}

	}
}
