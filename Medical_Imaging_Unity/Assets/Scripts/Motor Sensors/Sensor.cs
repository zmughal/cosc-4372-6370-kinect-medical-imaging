using UnityEngine;
using System.Collections;

public class Sensor : MonoBehaviour
{
    public bool IsActivated { get; private set; }
    public float TimeSpent { get; private set; }

    public int totalAdjustedScore = 0;

    public virtual void Start()
    {
        IsActivated = false;
    }

    public virtual void LateUpdate()
    {
        if (IsActivated)
        {
            TimeSpent += Time.deltaTime;
        }
    }

    public virtual void Activate()
    {
        IsActivated = true;
    }

    public virtual void Deactivate()
    {
        IsActivated = false;
        totalAdjustedScore = 0;
    }
    
    public bool IsPassing
    {
        get { return totalAdjustedScore >= 100; }
    }

    public void AddScore(int score)
    {
        totalAdjustedScore += score;
    }
}
