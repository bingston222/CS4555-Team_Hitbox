using System.Collections;
using UnityEngine;

public class ShockwaveAttack : MonoBehaviour
{
    public KeyCode key = KeyCode.Mouse0;
    public float radius = 3f;
    public float damage = 10f;
    public float cooldown = 0.4f;
    public Animator animator; //

    bool ready = true;

    void Update()
    {
        if (Input.GetKeyDown(key) && ready)
            StartCoroutine(DoAttack());
    }

    IEnumerator DoAttack()
    {
        ready = false;
        
          if (animator != null)
            animator.SetTrigger("Attack"); //Ensure this matches your Animator parameter name

        // Wait until the hit frame (you can adjust delay to match your animation)
        yield return new WaitForSeconds(0.2f);  

        Collider[] hits = Physics.OverlapSphere(transform.position, radius);
        foreach (var hit in hits)
        {
            Health enemy = hit.GetComponent<Health>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);

                //  Add ultimate charge on hit
                var ult = GetComponent<UltimateCharge>();
                if (ult != null) ult.AddCharge(10f);
            }
        }

        yield return new WaitForSeconds(cooldown);
        ready = true;
    }
}
