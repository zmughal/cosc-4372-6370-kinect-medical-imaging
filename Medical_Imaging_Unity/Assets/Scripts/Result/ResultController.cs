using UnityEngine;
using System.Collections;

public class ResultController : MonoBehaviour
{
    public GUIText Result;

    void Start()
    {
        if (PlayerPrefs.GetString("LevelStatus") == "Complete")
        {
            float totalTimeSpent = PlayerPrefs.GetFloat("TotalTimeSpent", 0f);
            float handWaveDuration = PlayerPrefs.GetFloat("HandWaveDetectorDuration", 0f);
            float jumpDuration = PlayerPrefs.GetFloat("JumpDetectorDuration", 0f);

            Result.text = string.Format("Game Play Result\n\nHand Wave: {0} seconds\nJump: {1} seconds\n\nTotal: {2} seconds",
                handWaveDuration.ToString("0.00"), jumpDuration.ToString("0.00"), totalTimeSpent.ToString("0"));
        }
        else
        {
            Result.text = "Player did not complete the level.";
        }

        Result.text += string.Format("\n\nLevel Name: {0}", Application.loadedLevelName);

        PlayerPrefs.DeleteAll();
    }
}
