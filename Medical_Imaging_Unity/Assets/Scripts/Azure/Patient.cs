using UnityEngine;
using System.Collections;

public class Patient : AzureTable
{
    public string FirstName;
    public string LastName;
    public int Gender;
    public int YearOfBirth; //or DOB
    public int SchoolId;
}
