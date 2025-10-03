using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SliceTearUI : MonoBehaviour
{
    [Header("Settings")]
    public RectTransform targetCanvas;      // drag your top-level Canvas (RectTransform)
    public Color sliceColor = new Color(1f, 1f, 1f, 0.25f);
    public float minHeight = 12f;
    public float maxHeight = 80f;
    public float minDuration = 0.05f;       // how long a slice is visible
    public float maxDuration = 0.14f;
    public Vector2 waitRange = new Vector2(0.6f, 1.8f); // time between slices
    public float maxOffset = 45f;           // how far slice shifts sideways

    RectTransform rt;

    void Awake() { rt = GetComponent<RectTransform>(); }

    void OnEnable() { StartCoroutine(Loop()); }

    IEnumerator Loop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(waitRange.x, waitRange.y));
            yield return StartCoroutine(SpawnSlice());
        }
    }

    IEnumerator SpawnSlice()
    {
        // Make a masked strip
        var maskGO = new GameObject("slice_mask", typeof(RectTransform), typeof(Image), typeof(Mask));
        var maskRT = maskGO.GetComponent<RectTransform>();
        maskRT.SetParent(rt, false);
        maskRT.anchorMin = new Vector2(0, 0);
        maskRT.anchorMax = new Vector2(1, 0);
        float h = Random.Range(minHeight, maxHeight);
        float y = Random.Range(0f, rt.rect.height - h);
        maskRT.sizeDelta = new Vector2(0, h);
        maskRT.anchoredPosition = new Vector2(0, y);
        var maskImg = maskGO.GetComponent<Image>();
        maskImg.color = Color.white;
        maskGO.GetComponent<Mask>().showMaskGraphic = false;

        // Inside it, a full-screen image that we offset horizontally
        var sliceGO = new GameObject("slice_fill", typeof(RectTransform), typeof(Image));
        var sliceRT = sliceGO.GetComponent<RectTransform>();
        sliceRT.SetParent(maskRT, false);
        sliceRT.anchorMin = new Vector2(0, 0);
        sliceRT.anchorMax = new Vector2(1, 1);
        sliceRT.offsetMin = Vector2.zero;
        sliceRT.offsetMax = Vector2.zero;
        var sliceImg = sliceGO.GetComponent<Image>();
        sliceImg.color = sliceColor;

        float dur = Random.Range(minDuration, maxDuration);
        float t = 0f;
        float offset = Random.Range(-maxOffset, maxOffset);

        // quick sideways shove with slight alpha flicker
        while (t < dur)
        {
            t += Time.unscaledDeltaTime;
            float k = t / dur;
            sliceRT.anchoredPosition = new Vector2(Mathf.Lerp(offset, 0, k), 0);
            sliceImg.color = new Color(sliceColor.r, sliceColor.g, sliceColor.b, sliceColor.a * (0.6f + 0.4f * Random.value));
            yield return null;
        }

        Destroy(maskGO);
    }
}
