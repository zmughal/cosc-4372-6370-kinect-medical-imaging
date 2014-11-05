using UnityEngine;
using System.Collections;
using Windows.Kinect;

public class RightHandController : MonoBehaviour
{
    public GameObject Kinect;
	public float speedMultiplier = 3.0f;
	public float rotationSpeed = 8.0f;
	public GameObject rotateLeftHotSpot;
	public GameObject rotateRightHotSpot;
	public GameObject objectToManipulate;
	private bool isColliding = false;


    void Update()
    {
        KinectController kinect = Kinect.GetComponent<KinectController>();

        Vector3 newPostion = kinect.PlayerSkeleton.GetPosition(JointType.HandRight);
        //Vector3 diff = newPostion - transform.position;
        //Debug.Log(diff);
        //transform.position += diff.normalized;
        //transform.TransformPoint(diff.normalized);
		newPostion.z = 0;
		newPostion.x *= speedMultiplier;
		newPostion.y *= speedMultiplier;
		transform.position = newPostion;

    }

	void OnCollisionStay(Collision collision)
	{    
		Debug.Log (collision.gameObject.tag);

		if(collision.gameObject.tag == "rotateLeft")        
		{
			objectToManipulate.transform.Rotate(Vector3.up, Mathf.PI * Time.deltaTime * rotationSpeed);
			Debug.Log("Left collision");
		}        
		else if(collision.gameObject.tag == "rotateRight")
		{
			objectToManipulate.transform.Rotate(Vector3.up, -(Mathf.PI * Time.deltaTime * rotationSpeed));
			Debug.Log("Right collision");
		}
	}
}
