using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    public EnemyHealth enemy;
    public Slider slider;
    public Transform target;             // what to follow (usually enemy transform)
    public Vector3 offset = new Vector3(0, 2.0f, 0);

    Camera cam;

    void Awake()
    {
        cam = Camera.main;
        if (slider == null) slider = GetComponentInChildren<Slider>();
        if (enemy == null) enemy = GetComponentInParent<EnemyHealth>();
        if (target == null && enemy != null) target = enemy.transform;
    }

    void Start()
    {
        if (enemy != null && slider != null)
        {
            slider.minValue = 0;
            slider.maxValue = enemy.maxHealth;
            slider.value   = enemy.currentHealth;
        }
    }

    void LateUpdate()
    {
        if (enemy == null || slider == null)
        {
            Destroy(gameObject);
            return;
        }

        // follow target
        if (target != null) transform.position = target.position + offset;

        // face camera (billboard)
        if (cam != null)
            transform.rotation = Quaternion.LookRotation(transform.position - cam.transform.position);

        // update value
        slider.value = enemy.currentHealth;
    }
}
