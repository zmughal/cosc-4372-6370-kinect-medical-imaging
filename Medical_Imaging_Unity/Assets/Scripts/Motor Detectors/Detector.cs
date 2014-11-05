using UnityEngine;
using System.Collections;

public class Detector : MonoBehaviour
{
    public bool IsActivated { get; private set; }

    private float elapsedTime = 0f;
    private int totalAdjustedScore = 0; // make it protected

    public virtual void Start()
    {
        IsActivated = false;
    }

    public virtual void LateUpdate()
    {
        if (IsActivated)
        {
            elapsedTime += Time.deltaTime;
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

    public virtual void OnDestroy()
    {
        PlayerPrefs.SetFloat(this.GetType() + "Duration", elapsedTime);
    }
}
