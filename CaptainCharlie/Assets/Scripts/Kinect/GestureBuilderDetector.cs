using UnityEngine;
using System.Collections;
using Windows.Kinect;

public class GestureBuilderDetector : MonoBehaviour
{
	public GameObject Kinect;
	public float rotationSpeed = 20.0f;
	public GameObject objectToManipulate;
	
	public Vector3 lastRHCoordinate;
	public Vector3 lastLHCoordinate;

	// Update is called once per frame
	void Update ()
	{
		KinectController kinect = Kinect.GetComponent<KinectController>();
		
		Vector3 rHPosition = kinect.PlayerSkeleton.GetPosition(JointType.HandRight);
		Vector3 lHPosition = kinect.PlayerSkeleton.GetPosition(JointType.HandLeft);


		// Which direction has the joint moved since the last time?
		Vector3 rHDelta = rHPosition - lastRHCoordinate;
		Vector3 lHDelta = lHPosition - lastLHCoordinate;

		// Set the new value of last coordinate to the current position
		lastRHCoordinate = rHPosition;
		lastLHCoordinate = lHPosition;

		// will get a 0/0 error if mouse does not move
		Vector3 rdirection = rHDelta.normalized;
		Vector3 ldirection = lHDelta.normalized;

		float rdot = Vector3.Dot (rdirection, Vector3.up);
		float ldot = Vector3.Dot (ldirection, Vector3.up);

		if (rdot > 0.5) { //can be >= for sideways
			//UP
			Debug.Log ("Moving up 2");
			rightHandMovingUp();
		} else if (rdot < -0.5) { //can be <= for sideways
			//DOWN
			Debug.Log ("Moving down 2");
			rightHandMovingDown();
		} else {
			rdot = Vector3.Dot (rdirection, Vector3.right);
			if (rdot > 0.5) { //can be >= for sideways
				Debug.Log ("Moving right 2");
				rightHandMovingRight();
				//RIGHT
			} else if (rdot < -0.5) { //can be <= for sideways
				Debug.Log ("Moving left 2");
				rightHandMovingLeft();
				//LEFT
			}
		}

		if (ldot > 0.5) { //can be >= for sideways
			//UP
			Debug.Log ("Moving up 2");
			leftHandMovingUp();
		} else if (ldot < -0.5) { //can be <= for sideways
			//DOWN
			Debug.Log ("Moving down 2");
			leftHandMovingDown();
		} else {
			ldot = Vector3.Dot (ldirection, Vector3.right);
			if (ldot > 0.5) { //can be >= for sideways
				Debug.Log ("Moving right 2");
				leftHandMovingRight();
				
				//RIGHT
			} else if (ldot < -0.5) { //can be <= for sideways
				Debug.Log ("Moving left 2");
				leftHandMovingLeft();
				//LEFT
			}
		}


	}

	void rightHandMovingRight() {
		/* rotate right */
		objectToManipulate.transform.Rotate(Vector3.up, -(Mathf.PI * Time.deltaTime * rotationSpeed));
	}

	void rightHandMovingLeft() {
		/* NOP */
	}

	void rightHandMovingUp() {
		/* TODO */
	}

	void rightHandMovingDown() {
		/* TODO */
	}

	void leftHandMovingRight() {
		/* NOP */
	}

	void leftHandMovingLeft() {
		/* rotate left */
		objectToManipulate.transform.Rotate(Vector3.up, Mathf.PI * Time.deltaTime * rotationSpeed);
	}

	void leftHandMovingUp() {
		/* TODO */
	}

	void leftHandMovingDown() {
		/* TODO */
	}
}
