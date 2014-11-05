using UnityEngine;
using System.Collections;

public class PlaneTakeOffController : MonoBehaviour
{
    public GameObject Plane;

    private Vector3 direction = Vector3.zero;

    private float speed = 20f;

    private float takeOffTimer = 7f;
    private const float takeOffTime = 7f;

    void Update()
    {
        if (takeOffTimer < takeOffTime)
        {
            Plane.transform.position += direction;
            takeOffTimer += Time.deltaTime;
        }
    }

    void OnCollisionEnter(Collision otherObject)
    {
        if (otherObject.gameObject.name == "Player")
        {
            direction = new Vector3(0f, .25f, 0f);
            takeOffTimer = 0f;
            gameObject.SetActive(false);
        }
    }
}
