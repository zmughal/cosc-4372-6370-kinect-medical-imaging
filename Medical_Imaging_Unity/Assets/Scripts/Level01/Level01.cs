using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Kinect = Windows.Kinect;

public class Level01 : MonoBehaviour
{
    public GameObject Player;

    private PlayerController player;
    private Vector3[] destinations = new Vector3[]
    {
        new Vector3(0.48f, 6.5f, 43.28f),
        new Vector3(-16f, 5.34f, 43.39f),
        new Vector3(-18f, 5.34f, 29.20f)
    };

    private int currentDestination = 0;

    private bool hasBeenOnPlane = false;

    private float totalTimeSpent = 0f;

    private float flyTime = 10f;
	
	void Update()
    {
        player = Player.GetComponent<PlayerController>();
        player.SetDestination(destinations[currentDestination]);

        if (player.IsAtDestination)
        {
            currentDestination = Mathf.Clamp(currentDestination + 1, 0, destinations.Length - 1);

            if (currentDestination == 2 && !hasBeenOnPlane)
            {
                player.rigidbody.velocity = Vector3.zero;
                hasBeenOnPlane = true;
            }
        }

        if (flyTime <= 0f)
        {
            PlayerPrefs.SetFloat("TotalTimeSpent", totalTimeSpent);
            PlayerPrefs.SetString("LevelStatus", "Complete");
            Application.LoadLevel(1);
        }

        float elapsedTime = Time.deltaTime;

        if (player.IsReadyToFly)
        {
            flyTime -= elapsedTime;
        }

        totalTimeSpent += elapsedTime;
	}
}
