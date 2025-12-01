using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    public Slider slider;
    public Transform target; // Enemy this bar follows
    public Vector3 offset = new Vector3(0, 2.2f, 0);

    Camera cam;

    void Start()
    {
        cam = Camera.main;
        if (slider == null)
            slider = GetComponentInChildren<Slider>();
    }

    void LateUpdate()
    {
        if (target == null) { Destroy(gameObject); return; }

        // Follow enemy + face camera
        transform.position = target.position + offset;
        transform.LookAt(transform.position + cam.transform.rotation * Vector3.forward,
                         cam.transform.rotation * Vector3.up);
    }

    public void UpdateHealth(float current, float max)
    {
        if (slider != null)
            slider.value = current / max;
    }
}
