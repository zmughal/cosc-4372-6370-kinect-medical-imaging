using UnityEngine;
using System.Collections;
using Windows.Kinect;

public class GestureBuilderDetector : MonoBehaviour
{
	public GameObject Kinect;
	public GameObject objectToManipulate;

	public GUIText leftDebugString;
	public GUIText rightDebugString;

	public GUIText actionStateString;
	
	public Vector3 lastRHCoordinate;
	public Vector3 lastLHCoordinate;

	public float xExtensionMin = 0.30f;
	public float xExtensionMax = 0.70f;
	public float maxRotationSpeed = 40.0f;
	public float minRotationSpeed = 0.0f;
	private float rHExtensionFactor;
	private float lHExtensionFactor;

	private Vector3 rightHandDirection;
	private Vector3 leftHandDirection;

	private Vector3 rightHandAnchor;
	private Vector3 leftHandAnchor;
	private Vector3 objectAnchor;
	private Quaternion objectAngleAnchor;
	private float zoomLevel = 0.25f;

	private enum ActionState { NONE, ZOOM_POS, TRANSLATE_POS, ROTATE_POS,
		                         ZOOM_VEC, TRANSLATE_VEC, ROTATE_VEC };

	ActionState currentHandState;
	ActionState lastHandState;

	float actionSwitchDelay;
	bool actionSwitch = true;
	float actionSwitchDelayMax = 0.1f;

	// Update is called once per frame
	void Update () {
		//HandExtensionGestureUpdate ();
		HandGrabGestureUpdate ();
	}

	void HandGrabGestureUpdate() {
		KinectController kinect = Kinect.GetComponent<KinectController>();
		Body skel = kinect.PlayerSkeleton;
		bool grabLeft = skel.HandLeftState == HandState.Closed
			|| skel.HandLeftState == HandState.Lasso;
		bool grabRight = skel.HandRightState == HandState.Closed
			|| skel.HandRightState == HandState.Lasso;
		bool velocityMode = skel.HandLeftState == HandState.Lasso
				|| skel.HandRightState == HandState.Lasso;

		Vector3 rHPosition = skel.GetPosition(JointType.HandRight);
		Vector3 lHPosition = skel.GetPosition(JointType.HandLeft);
		
		UpdateHandOffsetVectors();

		leftDebugString.text = skel.HandLeftState.ToString();
		rightDebugString.text = skel.HandRightState.ToString();

		if (actionSwitch) {
			actionSwitch = false;
			actionSwitchDelay = actionSwitchDelayMax;
			if (grabLeft && grabRight) {
					/* both hands closed = zoom */
				/* TODO vec */
					currentHandState = ActionState.ZOOM_POS;
			} else if (grabLeft) {
					/* left hand closed only = rotate */
					if( velocityMode ) {
						currentHandState = ActionState.ROTATE_VEC;
					} else {
						currentHandState = ActionState.ROTATE_POS;
					}

			} else if (grabRight) {
					/* right hand closed only = translate */
					if( velocityMode ) {
						currentHandState = ActionState.TRANSLATE_VEC;
					} else {
						currentHandState = ActionState.TRANSLATE_POS;
					}
			} else {
					currentHandState = ActionState.NONE;
			}
		} else {
			/* actionSwitch = false */
			if( actionSwitchDelay > 0 ) {
				actionSwitchDelay -= Time.deltaTime;
			} else {
				actionSwitchDelay = 0;
				actionSwitch = true;
			}
		}


		if( lastHandState  != currentHandState ) {
			/* mark initial position */
			rightHandAnchor = rHPosition;
			leftHandAnchor = lHPosition;
			objectAnchor = objectToManipulate.transform.position;
			objectAngleAnchor = objectToManipulate.transform.rotation;
		}

		if (currentHandState == ActionState.ZOOM_POS) {
				Vector3 interanchor = rightHandAnchor - leftHandAnchor;
				float interanchorDistance = interanchor.magnitude;

				Vector3 currentInterHand = rHPosition - lHPosition;
				float currentInterHandDistance = currentInterHand.magnitude;

				float zoomFactor = currentInterHandDistance / interanchorDistance;

				/* TODO make max red */
				actionStateString.text = "ZOOMING!" + (velocityMode ? "(with VELOCITY!!!)" : "");

				/* TODO Do the zoom */
		} else if (currentHandState == ActionState.TRANSLATE_POS) {
				/* TODO make max blue */ 
				actionStateString.text = "TRANSLATING!" + (velocityMode ? "(with VELOCITY!!!)" : "");
				Vector3 translateDirection = rHPosition - rightHandAnchor;
				translateDirection.z = 0;
				objectToManipulate.transform.position = /*Time.deltaTime */ (objectAnchor + translateDirection / zoomLevel);
		} else if (currentHandState == ActionState.TRANSLATE_VEC) {
				/* TODO make max blue */ 
				actionStateString.text = "TRANSLATING!" + (velocityMode ? "(with VELOCITY!!!)" : "");
				Vector3 translateDirection = rHPosition - rightHandAnchor;
				translateDirection.z = 0;
				objectToManipulate.transform.Translate( /* Time.deltaTime */ translateDirection , Space.World);
		} else if (currentHandState == ActionState.ROTATE_POS) {
				/* TODO make max green */
				Vector3 rotateDirection = lHPosition - leftHandAnchor;

				float x = -rotateDirection.y; /* yes, this is what I mean... */
				float y = rotateDirection.x; /* swapping x and y */

				rotateDirection.x = x;
				rotateDirection.y = y;
				rotateDirection.z = 0;

				rotateDirection *= 40/zoomLevel;

				actionStateString.text = "ROTATING! " + ( rotateDirection.ToString() )  + " "  + (velocityMode ? "(with VELOCITY!!!)" : "");

				objectToManipulate.transform.rotation = Quaternion.Euler( /*Time.deltaTime */ ( objectAngleAnchor.eulerAngles + rotateDirection ) );
				//objectToManipulate.transform.Rotate( rotateDirection );
		} else if (currentHandState == ActionState.ROTATE_VEC) {
			/* TODO make max green */
			Vector3 rotateDirection = lHPosition - leftHandAnchor;
			
			float x = -rotateDirection.y; /* yes, this is what I mean... */
			float y = rotateDirection.x; /* swapping x and y */
			
			rotateDirection.x = x;
			rotateDirection.y = y;
			rotateDirection.z = 0;
			
			rotateDirection *= 1/zoomLevel;
			
			actionStateString.text = "ROTATING! " + ( rotateDirection.ToString() )  + " "  + (velocityMode ? "(with VELOCITY!!!)" : "");
			
			objectToManipulate.transform.Rotate( /*Time.deltaTime */ rotateDirection );
		} else {
				actionStateString.text = "NOTHING!" + (velocityMode ? "(with VELOCITY!!!)" : "");
		}

		lastHandState = currentHandState;
	}

	void UpdateHandOffsetVectors () {
		KinectController kinect = Kinect.GetComponent<KinectController>();
		
		Body skel = kinect.PlayerSkeleton;
		Vector3 rHPosition = skel.GetPosition(JointType.HandRight);
		Vector3 lHPosition = skel.GetPosition(JointType.HandLeft);

		// Which direction has the joint moved since the last time?
		Vector3 rHDelta = rHPosition - lastRHCoordinate;
		Vector3 lHDelta = lHPosition - lastLHCoordinate;
		
		// Set the new value of last coordinate to the current position
		lastRHCoordinate = rHPosition;
		lastLHCoordinate = lHPosition;

		// will get a 0/0 error if mouse does not move
		rightHandDirection = rHDelta.normalized;
		leftHandDirection = lHDelta.normalized;
	}

	/* this uses the distance from the torso to determine the factor of rotation */
	void HandExtensionGestureUpdate() {
		KinectController kinect = Kinect.GetComponent<KinectController>();

		/* where are all the joints? */
		Body skel = kinect.PlayerSkeleton;
		Vector3 rHPosition = skel.GetPosition(JointType.HandRight);
		Vector3 lHPosition = skel.GetPosition(JointType.HandLeft);
		Vector3 neckPosition = skel.GetPosition(JointType.Neck);

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

	void DispatchDirection () {
		float rdot = Vector3.Dot (rightHandDirection, Vector3.up);
		float ldot = Vector3.Dot (leftHandDirection, Vector3.up);
		
		if (rdot > 0.5) { //can be >= for sideways
			//UP
			Debug.Log ("Moving up 2");
			rightHandMovingUp();
		} else if (rdot < -0.5) { //can be <= for sideways
			//DOWN
			Debug.Log ("Moving down 2");
			rightHandMovingDown();
		} else {
			rdot = Vector3.Dot (rightHandDirection, Vector3.right);
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
			ldot = Vector3.Dot (leftHandDirection, Vector3.right);
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

	/* this calculates the direction of a hand movement between frames to determine actions */ 
	void HandMovementGestureUpdate() {
		UpdateHandOffsetVectors();
		DispatchDirection();
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
		if( currentHandState == ActionState.TRANSLATE_POS ) {
			/* TODO */
		}
	}

	void rightHandMovingLeft() {
		/* TODO */
	}

	void rightHandMovingUp() {
		/* TODO */
	}

	void rightHandMovingDown() {
		/* TODO */
	}

	void leftHandMovingRight() {
		/* TODO */
	}

	void leftHandMovingLeft() {
		/* TODO */
	}

	void leftHandMovingUp() {
		/* TODO */
	}

	void leftHandMovingDown() {
		/* TODO */
	}
}
