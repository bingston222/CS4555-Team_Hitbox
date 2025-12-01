using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public Image fill;
    public void UpdateBar(float currentHP, float maxHP)
    {
        if (fill) fill.fillAmount = Mathf.Clamp01(currentHP / Mathf.Max(0.0001f, maxHP));
    }
}
