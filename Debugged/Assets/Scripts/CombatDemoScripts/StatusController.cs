using UnityEngine;
using System.Collections;

public class StatusController : MonoBehaviour
{
    public bool IsInvulnerable { get; private set; }

    public IEnumerator Invulnerability(float duration)
    {
        IsInvulnerable = true;
        yield return new WaitForSeconds(duration);
        IsInvulnerable = false;
    }
    
}
