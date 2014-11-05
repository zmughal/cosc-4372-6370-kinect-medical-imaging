using UnityEngine;
using System.Collections;

public enum Hand { None, Left, Right }

public class HandWaveDetector : Detector
{
    private Hand handRaised = Hand.None;
    private float handWaveTimer = 0f;
    private const float TotalHandWaveTime = 2f;
    private float rawScore = 0f;
    private float previousAngle = 0f;

    private Vector3 handLeft;
    private Vector3 handRight;
    private Vector3 elbowLeft;
    private Vector3 elbowRight;

    public void Set(Vector3 handLeft, Vector3 handRight, Vector3 elbowLeft, Vector3 elbowRight)
    {
        this.handLeft = handLeft;
        this.handRight = handRight;
        this.elbowLeft = elbowLeft;
        this.elbowRight = elbowRight;
    }

    void Update()
    {
        if (IsActivated)
        {
            CheckForHandWave(handLeft, handRight, elbowLeft, elbowRight);
        }
    }

    public override void Deactivate()
    {
        handRaised = Hand.None;
        handWaveTimer = 0f;
        rawScore = 0f;
        previousAngle = 0f;

        base.Deactivate();
    }

    private void CheckForHandWave(Vector3 hl, Vector3 hr, Vector3 el, Vector3 er)
    {
        if (handRaised == Hand.None)
        {
            //InstructPlayer(HandWaveInstruction.WaveHand, true);
            DetectHandRaised(hl, hr, el, er);
        }
        else
        {
            //InstructPlayer(HandWaveInstruction.WaveHand, false);
            WaveHand(hl, hr, el, er, Time.deltaTime);
        }
    }

    private void DetectHandRaised(Vector3 hl, Vector3 hr, Vector3 el, Vector3 er)
    {
        float rightAngle = GetAngle(hr, er);
        float leftAngle = GetAngle(hl, el);

        if (rightAngle > 0f)
        {
            handRaised = Hand.Right;
            previousAngle = rightAngle;
        }
        else if (leftAngle > 0f)
        {
            handRaised = Hand.Left;
            previousAngle = leftAngle;
        }
    }

    private void WaveHand(Vector3 hl, Vector3 hr, Vector3 el, Vector3 er, float elapsedTime)
    {
        float angle = handRaised == Hand.Right
            ? GetAngle(hr, er)
            : GetAngle(hl, el);

        if (angle > 0f && handWaveTimer < TotalHandWaveTime)
        {
            rawScore += GetScoreBetweenTwoAngles(angle, previousAngle, elapsedTime);
            previousAngle = angle;
            handWaveTimer += elapsedTime;
        }
        else
        {
            AddScore(GetAdjustedScore(rawScore));
            ResetScore();
        }
    }

    private void ResetScore()
    {
        rawScore = 0f;
        handWaveTimer = 0f;
    }

    private int GetAdjustedScore(float rawScore)
    {
        float[] passingScores = new float[] { 0.7f, 0.5f, 0.4f };
        int[] adjustedScores = new int[] { 100, 50, 25 };

        for (int i = 0; i < 3; i++)
        {
            if (rawScore >= passingScores[i])
            {
                return adjustedScores[i];
            }
        }

        return 0;
    }

    private float GetAngle(Vector3 hand, Vector3 elbow)
    {
        float armVerticalLength = hand.y - elbow.y;
        return armVerticalLength < 0f ? 0f : Mathf.Atan(armVerticalLength / Mathf.Abs(hand.x - elbow.x));
    }

    private float GetScoreBetweenTwoAngles(float angle, float previousAngle, float elapsedTime)
    {
        return GetPointsFromAngle(angle) * Mathf.Abs(angle - previousAngle) * elapsedTime;
    }

    private float GetPointsFromAngle(float angle)
    {
        return Mathf.Pow(angle, 2f) / 2.4674f * 35f; // (pi/2)^2 = 2.4674
    }
}
