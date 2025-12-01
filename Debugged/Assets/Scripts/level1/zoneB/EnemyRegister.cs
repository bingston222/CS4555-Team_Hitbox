using UnityEngine;

public class EnemyRegister : MonoBehaviour
{
    bool registered;

    void OnEnable()
    {
        if (!registered)
        {
            AlarmManager.Instance?.RegisterEnemy(this);
            registered = true;
        }
    }

    void OnDisable()
    {
        if (registered)
        {
            AlarmManager.Instance?.DeregisterEnemy(this);
            registered = false;
        }
    }

    void OnDestroy() => OnDisable(); // safety
}
