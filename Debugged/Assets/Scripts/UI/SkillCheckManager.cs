using UnityEngine;
using UnityEngine.UI;

public class SkillCheckManager : MonoBehaviour
{
    [Header("UI")]
    public Image progressBar;           
    public RectTransform successZone;

    [Header("Settings")]
    public float baseSpeed = 2f;
    public float speedIncrease = 1f;
    public KeyCode skillKey = KeyCode.Space;

    float t = 0f;
    bool goingRight = true;

    bool active = false;
    int currentCheck = 0;
    int totalChecks = 3;

    float currentSpeed;
    System.Action<bool> callback;

    SuccessZoneData zoneData;

    void Awake()
    {
        zoneData = successZone.GetComponent<SuccessZoneData>();
    }

    public void StartSkillChecks(int checksNeeded, System.Action<bool> resultCallback)
    {
        totalChecks = checksNeeded;
        currentCheck = 0;

        callback = resultCallback;

        currentSpeed = baseSpeed;
        ResetBar();

        active = true;
        gameObject.SetActive(true);
    }

    void ResetBar()
    {
        t = 0f;
        goingRight = true;
    }

    void Update()
    {
        if (!active) return;

        // Move t between 0 and 1
        if (goingRight)
        {
            t += Time.deltaTime * currentSpeed;
            if (t >= 1f) { t = 1f; goingRight = false; }
        }
        else
        {
            t -= Time.deltaTime * currentSpeed;
            if (t <= 0f) { t = 0f; goingRight = true; }
        }

        progressBar.fillAmount = t;

        if (Input.GetKeyDown(skillKey))
        {
            bool success = (t >= zoneData.min && t <= zoneData.max);

            if (!success)
            {
                active = false;
                gameObject.SetActive(false);
                callback(false);
                return;
            }

            // SUCCESS
            currentCheck++;
            currentSpeed += speedIncrease;

            if (currentCheck >= totalChecks)
            {
                active = false;
                gameObject.SetActive(false);
                callback(true);
            }
            else
            {
                ResetBar();
            }
        }
    }
}
