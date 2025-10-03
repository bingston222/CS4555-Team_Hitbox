using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class ProceduralNoiseTexture : MonoBehaviour
{
    [Range(64, 1024)] public int size = 256;
    [Range(0f, 1f)] public float contrast = 1f; // 1 = white noise, <1 = softer
    public bool perlin = false;                 // set true if you want Perlin

    RawImage img;

    void Awake() {
        img = GetComponent<RawImage>();
        img.texture = MakeNoise(size, perlin, contrast);
        img.uvRect = new Rect(0, 0, 1, 1);
    }

    Texture2D MakeNoise(int s, bool usePerlin, float k) {
        var tex = new Texture2D(s, s, TextureFormat.R8, false);
        tex.wrapMode = TextureWrapMode.Repeat;
        for (int y = 0; y < s; y++) {
            for (int x = 0; x < s; x++) {
                float v = usePerlin
                    ? Mathf.PerlinNoise(x * 0.07f, y * 0.07f)
                    : Random.value;
                v = Mathf.Lerp(0.5f, v, k); // adjust contrast
                tex.SetPixel(x, y, new Color(v, v, v, 1));
            }
        }
        tex.Apply();
        return tex;
    }
}
