using UnityEngine;
using System.Collections;
using Windows.Kinect;

public class GestureBuilderDetector : MonoBehaviour
{
	public GameObject Kinect;
	public GameObject objectToManipulate;

	public GUIText leftDebugString;
	public GUIText rightDebugString;
	
	public Vector3 lastRHCoordinate;
	public Vector3 lastLHCoordinate;

	public float xExtensionMin = 0.30f;
	public float xExtensionMax = 0.70f;
	public float maxRotationSpeed = 40.0f;
	public float minRotationSpeed = 0.0f;
	private float rHExtensionFactor;
	private float lHExtensionFactor;

	// Update is called once per frame
	void Update () {
		HandExtensionGestureUpdate ();
	}

	void HandGrabGestureUpdate() {
		KinectController kinect = Kinect.GetComponent<KinectController>();
		bool grabLeft = kinect.PlayerSkeleton.HandLeftState == HandState.Closed;
		bool grabRight = kinect.PlayerSkeleton.HandRightState == HandState.Closed
		
		/* where are all the joints? */
		Vector3 rHPosition = kinect.PlayerSkeleton.GetPosition(JointType.HandRight);
		Vector3 lHPosition = kinect.PlayerSkeleton.GetPosition(JointType.HandLeft);

		/* TODO: take the hand position relative to closed hand position */
	}

	/* this uses the distance from the torso to determine the factor of rotation */
	void HandExtensionGestureUpdate() {
		KinectController kinect = Kinect.GetComponent<KinectController>();

		/* where are all the joints? */
		Vector3 rHPosition = kinect.PlayerSkeleton.GetPosition(JointType.HandRight);
		Vector3 lHPosition = kinect.PlayerSkeleton.GetPosition(JointType.HandLeft);
		Vector3 neckPosition = kinect.PlayerSkeleton.GetPosition(JointType.Neck);

		/* how far are the joints from the body center (use neck x position as an approximation)? */
		float rHOffsetFromBody = rHPosition.x - neckPosition.x;
		float lHOffsetFromBody = lHPosition.x - neckPosition.x;

		/* are the hands on their respective side of the body (e.g., left on the left side)? */
		/*  [Left Hand] [Body Center] [Right Hand]
		 * ---- 0 ---  1 ---  2 -- 3 -- 4 -- 5 -- 6 -- [+x-axis] ---->
		 */ 
		bool rHOnRightSide = rHOffsetFromBody > 0; /* is positive if the right hand is outstretched */
		bool lHOnLeftSide = lHOffsetFromBody < 0; /* is negative if the left hand is outstretched */

		/* calculate the offset absolute value so that it can be used for thresholding */
		rHExtensionFactor = System.Math.Abs(rHOffsetFromBody);
		lHExtensionFactor = System.Math.Abs(lHOffsetFromBody);

		/*DEBUG: */
		rightDebugString.text = rHOffsetFromBody.ToString();
		leftDebugString.text  = lHOffsetFromBody.ToString();

		rightHandMovingRight ();
		leftHandMovingLeft ();
	}

	/* this calculates the direction of a hand movement between frames to determine actions */ 
	void HandMovementGestureUpdate() {
		KinectController kinect = Kinect.GetComponent<KinectController>();
		
		Vector3 rHPosition = kinect.PlayerSkeleton.GetPosition(JointType.HandRight);
		Vector3 lHPosition = kinect.PlayerSkeleton.GetPosition(JointType.HandLeft);
		Vector3 neckPosition = kinect.PlayerSkeleton.GetPosition(JointType.Neck);

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

	/* If the hands are less than a range, then they are in a deadzone, that is, they do no actions */
	bool isInDeadzone( float xExtension ) {
		return xExtension <= xExtensionMin;
	}

	float calculateRotationSpeed( float xExtension ) {
		float clippedExtension = System.Math.Min (xExtension, xExtensionMax) - xExtensionMin;
		float slope = (maxRotationSpeed - minRotationSpeed) / (xExtensionMax - xExtensionMin);
		float intercept = minRotationSpeed;
		return slope * clippedExtension + intercept;
	}

	/* Rotate about the y-axis.
	 * 
	 * Negative values rotate to the right (from an observer on +z-axis).
	 * 
	 * Positive values rotate to the left (from an observer on +z-axis).
	 */
	void rotateYaw( float rotSpeed ) {
		objectToManipulate.transform.Rotate(Vector3.up, Mathf.PI * Time.deltaTime * rotSpeed);
	}

	void rightHandMovingRight() {
		/* rotate right */
		if (! isInDeadzone (rHExtensionFactor)) {
			float rotSpeed = calculateRotationSpeed(rHExtensionFactor);
			rotateYaw( -rotSpeed );
		}
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
		if (! isInDeadzone ( lHExtensionFactor)) {
				float rotSpeed = calculateRotationSpeed( lHExtensionFactor );
				rotateYaw( rotSpeed );
		}
	}

	void leftHandMovingUp() {
		/* TODO */
	}

	void leftHandMovingDown() {
		/* TODO */
	}
}
