using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public GameObject Player;
    public GameObject Plane;

    void Update()
    {
        PlayerController player = Player.GetComponent<PlayerController>();

        if (player.renderer.enabled)
        {
            SetCamera(Player.transform.position, player.Destination);
        }
        else
        {
            PlaneController plane = Plane.GetComponent<PlaneController>();
            Vector3 planePosition = plane.transform.position;

            if (plane.FlyMode == FlyMode.FirstPerson)
            {
                Vector3 cockpitPosition = planePosition + new Vector3(0f, 20f, -107.4f);
                SetCamera(cockpitPosition, cockpitPosition + new Vector3(0f, 0f, -1f));
            }
            else if (plane.FlyMode == FlyMode.ThirdPerson)
            {
                Vector3 thirdPersonPosition = planePosition + new Vector3(0f, 60f, 220f);
                SetCamera(thirdPersonPosition, thirdPersonPosition + new Vector3(0f, -0.2f, -1f));
            }
        }
    }

    private void SetCamera(Vector3 position, Vector3 lookAtPosition)
    {
        transform.position = position;
        transform.LookAt(lookAtPosition);
    }
}
