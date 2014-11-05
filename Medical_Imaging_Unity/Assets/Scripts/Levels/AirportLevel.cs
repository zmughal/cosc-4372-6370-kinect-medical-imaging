using UnityEngine;
using System.Collections;

public class AirportLevel : Level
{
    private Vector3[] destinations = new Vector3[]
    {
        new Vector3(0.46f, 6.6f, -2.87f),
        new Vector3(0.48f, 6.5f, 43.28f),
        new Vector3(-16f, 5.34f, 43.39f),
        new Vector3(-18f, 5.34f, 29.20f)
    };

    private int currentDestination = 0;

    private bool hasBeenOnPlane = false;
    private float flyTime = 10f;

    private Transform[] waypoint;
    private int currentWaypoint = 0;
    private Vector3 velocity;

    void UpdateWaypoint()
    {
        if (currentWaypoint < waypoint.Length)
        {
            Vector3 moveDirection = waypoint[currentWaypoint].position - player.transform.position;
            velocity = rigidbody.velocity;

            if (moveDirection.magnitude < 1)
            {
                currentWaypoint++;
            }
            else
            {
                velocity = moveDirection.normalized * player.Speed;
            }
        }

        rigidbody.velocity = velocity;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        for (int i = 0; i < destinations.Length - 1; i++)
        {
            Gizmos.DrawLine(destinations[i], destinations[i + 1]);
            Gizmos.DrawSphere(destinations[i + 1], 1);
        }
    }

    public override void Start()
    {
        GameObject.Find("Plane").GetComponent<PlaneController>().FlyMode = FlyMode.ThirdPerson;
        base.Start();
    }

    void Update()
    {
        player.Destination = destinations[currentDestination];

        if (player.IsAtDestination)
        {
            currentDestination = Mathf.Clamp(currentDestination + 1, 0, destinations.Length - 1);

            // to make the player slow down before entering the cockpit
            if (currentDestination == 3 && !hasBeenOnPlane)
            {
                player.rigidbody.velocity = Vector3.zero;
                hasBeenOnPlane = true;
            }
        }

        if (flyTime <= 0f)
        {
            Finish(true);
        }
    }

    public override void LateUpdate()
    {
        if (!player.renderer.enabled)
        {
            flyTime -= Time.deltaTime;
        }

        base.LateUpdate();
    }
}
