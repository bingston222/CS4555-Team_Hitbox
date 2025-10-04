using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class ScrollRawImage : MonoBehaviour
{
    public Vector2 speed = new Vector2(0.0f, 0.35f);
    RawImage img;
    Vector2 uv;

    void Awake(){ img = GetComponent<RawImage>(); }
    void Update(){
        uv += speed * Time.unscaledDeltaTime;   // ignore timescale
        img.uvRect = new Rect(uv.x, uv.y, 1f, 1f);
    }
}
