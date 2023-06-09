using UnityEngine;

public class PulsingEffect : MonoBehaviour
{
    public float minScale = 0.9f;
    public float maxScale = 1.1f;
    public float pulseSpeed = 1.0f;

    private bool isIncreasing = true;

    // Update is called once per frame
    void Update()
    {
        if (isIncreasing)
        {
            transform.localScale += Vector3.one * pulseSpeed * Time.deltaTime;

            if (transform.localScale.x > maxScale)
            {
                isIncreasing = false;
            }
        }
        else
        {
            transform.localScale -= Vector3.one * pulseSpeed * Time.deltaTime;

            if (transform.localScale.x < minScale)
            {
                isIncreasing = true;
            }
        }
    }
}
