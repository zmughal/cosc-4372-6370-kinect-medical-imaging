using UnityEngine;
using System.Collections;

public class FlyingLevel : Level
{
    public override void Start()
    {
        GameObject.Find("Plane").GetComponent<PlaneController>().FlyMode = FlyMode.ThirdPerson;
        base.Start();
    }
}
