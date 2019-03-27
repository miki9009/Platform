using UnityEngine;

public class Torch : MonoBehaviour
{
    public Light light;

    public float minRange = 15;
    public float maxRange = 40;
    public float lerpSpeed = 2;

    float progress = 0;
    float curRange;

    private void Start()
    {
        curRange = Random.Range(minRange, maxRange);
    }

    private void Update()
    {
        if(progress < 1)
        {
            light.range = Mathf.Lerp(light.range, curRange, progress);
            progress += Time.deltaTime * lerpSpeed;
        }
        else
        {
            progress = 0;
            curRange = Random.Range(minRange, maxRange);
        }

    }
}