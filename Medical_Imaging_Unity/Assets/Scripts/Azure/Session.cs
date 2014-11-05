using UnityEngine;
using System.Collections;

public class Session : AzureTable
{
    //DateTime field is taken care by AMS
    public int Result;
    public int Duration;
    public int PatientId;
    public int TherapistId;
    public int LevelId;
}
