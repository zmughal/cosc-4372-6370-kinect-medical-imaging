using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Bitrave.Azure;

public static class Azure
{
    private static AzureMobileServices ams = new AzureMobileServices(
        "https://autismdb.azure-mobile.net/", "tmMCIIpNNNWHKLokRURYfqOHpYnPuT48");

    public static List<Patient> GetPatients(int schoolId)
    {
        return new List<Patient>(); //return all patients given the schoolId
    }

    public static List<Therapist> GetTherapists(int schoolId)
    {
        return new List<Therapist>();
    }

    public static void SubmitGameResult(Session session, List<Quest> quests)
    {
        //insert session - and get sessionId
        //insert quests
    }
}
