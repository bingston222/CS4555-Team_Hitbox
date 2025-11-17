using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public Image fill;

    public void UpdateBar(float currentHP, float maxHP)
    {
        fill.fillAmount = currentHP / maxHP;
    }
}
