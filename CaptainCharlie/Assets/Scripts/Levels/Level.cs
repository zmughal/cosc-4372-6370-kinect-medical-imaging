using UnityEngine;
using System.Collections;

public class Level : MonoBehaviour
{
    public GameObject Player;

    protected PlayerController player { get; private set; }

    private float elapsedTime = 0f;

    public virtual void Start()
    {
        PlayerPrefs.DeleteAll();
    }

    public virtual void FixedUpdate()
    {
        player = Player.GetComponent<PlayerController>();
    }

    public virtual void LateUpdate()
    {
        elapsedTime += Time.deltaTime;
    }

    public void Finish(bool isComplete)
    {
        PlayerPrefs.SetFloat("TotalTimeSpent", elapsedTime);
        PlayerPrefs.SetString("LevelStatus", isComplete ? "Complete" : "Incomplete");
        Application.LoadLevel("Result");
    }
}
