using UnityEngine;

public class SuccessZoneData : MonoBehaviour
{
    public float min;
    public float max;

    void Update()
    {
        RectTransform parent = transform.parent as RectTransform;
        RectTransform rt = GetComponent<RectTransform>();

        float parentWidth = parent.rect.width;

        // Convert center position into normalized 0-1 space
        float centerNormalized = (rt.anchoredPosition.x / parentWidth) + 0.5f;

        // Convert half-width into normalized 0-1
        float halfNormalized = (rt.rect.width / 2f) / parentWidth;

        min = centerNormalized - halfNormalized;
        max = centerNormalized + halfNormalized;
    }
}
