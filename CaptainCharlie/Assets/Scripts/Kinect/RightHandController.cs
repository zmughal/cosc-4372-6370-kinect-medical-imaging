using UnityEngine;
using System.Collections;
using Windows.Kinect;

public class RightHandController : MonoBehaviour
{
    public GameObject Kinect;
	public float speedMultiplier = 3.0f;
	public float rotationSpeed = 8.0f;
	public GameObject rotateRightHotSpot;
	public GameObject objectToManipulate;
	private bool isColliding = false;


    void Update()
    {
        KinectController kinect = Kinect.GetComponent<KinectController>();

        Vector3 newPostion = kinect.PlayerSkeleton.GetPosition(JointType.HandRight);
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
		else if(collision.gameObject.tag == "translateLeft")        
		{
			objectToManipulate.transform.Translate(0.1f,0,0);
			Debug.Log("Left collision");
		}        
		else if(collision.gameObject.tag == "translateRight")
		{
			objectToManipulate.transform.Translate(-0.1f,0,0);
			Debug.Log("Right collision");
		}
	}
}
