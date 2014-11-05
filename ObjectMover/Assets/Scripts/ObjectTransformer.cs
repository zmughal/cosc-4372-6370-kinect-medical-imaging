using UnityEngine;
using System.Collections;

public class ObjectTransformer : MonoBehaviour {

	public float positionSpeed = 0.1f;
	public float rotationSpeed = 10f;
	private Vector3 originalPosition;
	private Vector3 originalRotation;

	void Update () 
	{
		originalPosition = gameObject.transform.position;
		originalRotation = gameObject.transform.eulerAngles;


		if (Input.GetKeyDown(KeyCode.UpArrow))
			increasePositionY();
		if (Input.GetKeyDown(KeyCode.DownArrow))
			decreasePositionY();
		if (Input.GetKeyDown(KeyCode.RightArrow))
			increasePositionX();
		if (Input.GetKeyDown(KeyCode.LeftArrow))
			decreasePositionX();
		if (Input.GetKeyDown(KeyCode.Comma))
			increasePositionZ();
		if (Input.GetKeyDown(KeyCode.Period))
			decreasePositionZ();

		if (Input.GetKeyDown(KeyCode.W))
			increaseRotationY();
		if (Input.GetKeyDown(KeyCode.S))
			decreaseRotationY();
		if (Input.GetKeyDown(KeyCode.D))
			increaseRotationX();
		if (Input.GetKeyDown(KeyCode.A))
			decreaseRotationX();
		if (Input.GetKeyDown(KeyCode.X))
			increaseRotationZ();
		if (Input.GetKeyDown(KeyCode.Z))
			decreaseRotationZ();
	}

	void increasePositionY ()
	{
		gameObject.transform.position = new Vector3(originalPosition.x, originalPosition.y+positionSpeed, originalPosition.z);
	}

	void decreasePositionY ()
	{
		gameObject.transform.position = new Vector3(originalPosition.x, originalPosition.y-positionSpeed, originalPosition.z);
	}

	void increasePositionX ()
	{
		gameObject.transform.position = new Vector3(originalPosition.x+positionSpeed, originalPosition.y, originalPosition.z);
	}

	void decreasePositionX ()
	{
		gameObject.transform.position = new Vector3(originalPosition.x-positionSpeed, originalPosition.y, originalPosition.z);
	}

	void increasePositionZ ()
	{
		gameObject.transform.position = new Vector3(originalPosition.x, originalPosition.y, originalPosition.z+positionSpeed);
	}
	
	void decreasePositionZ ()
	{
		gameObject.transform.position = new Vector3(originalPosition.x, originalPosition.y, originalPosition.z-positionSpeed);
	}





	void increaseRotationY ()
	{
		gameObject.transform.eulerAngles = new Vector3(originalRotation.x, originalRotation.y+rotationSpeed, originalRotation.z);
	}
	
	void decreaseRotationY ()
	{
		gameObject.transform.eulerAngles = new Vector3(originalRotation.x, originalRotation.y-rotationSpeed, originalRotation.z);
	}
	
	void increaseRotationX ()
	{
		gameObject.transform.eulerAngles = new Vector3(originalRotation.x+rotationSpeed, originalRotation.y, originalRotation.z);
	}
	
	void decreaseRotationX ()
	{
		gameObject.transform.eulerAngles = new Vector3(originalRotation.x-rotationSpeed, originalRotation.y, originalRotation.z);
	}
	
	void increaseRotationZ ()
	{
		gameObject.transform.eulerAngles = new Vector3(originalRotation.x, originalRotation.y, originalRotation.z+rotationSpeed);
	}
	
	void decreaseRotationZ ()
	{
		gameObject.transform.eulerAngles = new Vector3(originalRotation.x, originalRotation.y, originalRotation.z-rotationSpeed);
	}
}
