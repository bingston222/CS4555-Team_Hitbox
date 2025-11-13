using UnityEngine;
public class LightFlash : MonoBehaviour {
    public Light targetLight;
    public Color flashColor = new Color(0.7f, 1f, 1f, 1f);
    public float peakIntensity = 6000f;   // use HDR intensity if URP
    public float riseTime = 0.05f;
    public float decayTime = 0.35f;

    float t;
    bool playing;

    void Awake() {
        if (!targetLight) targetLight = GetComponent<Light>();
        targetLight.enabled = false;
    }

    public void PlayFlash() {
        playing = true; t = 0f;
        targetLight.enabled = true;
        targetLight.color = flashColor;
    }

    void Update() {
        if (!playing) return;
        t += Time.deltaTime;
        float val;
        if (t < riseTime) {
            val = Mathf.Lerp(0f, peakIntensity, t / riseTime);
        } else if (t < riseTime + decayTime) {
            float dt = (t - riseTime) / decayTime;
            val = Mathf.Lerp(peakIntensity, 0f, dt);
        } else {
            targetLight.enabled = false;
            playing = false;
            return;
        }
        targetLight.intensity = val;
    }
}
