using UnityEngine;
using TMPro;

[DisallowMultipleComponent]
public class PCPromptUI : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI text;        // assign the bottom TMP label
    public CanvasGroup group;           // optional (for show/hide). If null, we toggle SetActive

    [Header("Defaults")]
    [TextArea] public string defaultMessage = "";
    public bool startHidden = true;

    void Awake()
    {
        if (!text) text = GetComponentInChildren<TextMeshProUGUI>(true);
        if (!group) group = GetComponent<CanvasGroup>();

        if (startHidden) Hide();
        else Show(defaultMessage);
    }

    public void Show(string msg)
    {
        Set(msg);
        SetVisible(true);
    }

    public void Set(string msg)
    {
        if (text) text.text = msg;
    }

    public void Hide()
    {
        SetVisible(false);
    }

    void SetVisible(bool v)
    {
        if (group)
        {
            group.alpha = v ? 1f : 0f;
            group.blocksRaycasts = v;
            group.interactable = v;
        }
        else
        {
            gameObject.SetActive(v);
        }
    }
}
