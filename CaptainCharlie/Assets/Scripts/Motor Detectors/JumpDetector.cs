using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class JumpDetector : Detector
{
    private List<float> jumpHeights = new List<float>();

    private float timer = 0f;
    private const float time = 1f;

    private Vector3 footLeft;
    private Vector3 footRight;

    public void Set(Vector3 footLeft, Vector3 footRight)
    {
        this.footLeft = footLeft;
        this.footRight = footRight;
    }

    void Update()
    {
        if (IsActivated)
        {
            if (timer < time)
            {
                float newHeight = (footLeft.y + footRight.y) / 2f;
                jumpHeights.Add(newHeight);
                timer += Time.deltaTime;
            }
            else
            {
                int segment = jumpHeights.Count / 5;
                var median = jumpHeights.Skip(segment);
                float minHeight = median.Min();
                float maxHeight = median.Max();
                float diff = Mathf.Abs(maxHeight - minHeight);

                AddScore((int)(diff / 0.2f * 100f));
                timer = 0f;
                jumpHeights = new List<float>();
            }
        }
    }
}
