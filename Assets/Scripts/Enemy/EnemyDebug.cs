using UnityEngine;

public class EnemyDebug : MonoBehaviour
{
    private Animator anim;

    void Start()
    {
        Debug.Log("Enemy " + name + " setup:");
        Debug.Log("Tag: " + tag);

        HealthEnemy health = GetComponent<HealthEnemy>();
        if (health == null)
        {
            Debug.LogError("HealthEnemy component not found");
        }
        else
        {
            Debug.Log("HealthEnemy component: OK");
        }

        anim = GetComponent<Animator>();
        if (anim == null)
        {
            Debug.LogError("Animator component not found");
        }
        else
        {
            Debug.Log("Animator component: OK");

            if (anim.runtimeAnimatorController == null)
            {
                Debug.LogError("No Animator Controller assigned");
            }
            else
            {
                Debug.Log("Animator Controller: " + anim.runtimeAnimatorController.name);
            }

            Debug.Log("Animator parameters:");
            if (anim.parameterCount == 0)
            {
                Debug.LogWarning("No parameters found");
            }
            else
            {
                foreach (AnimatorControllerParameter param in anim.parameters)
                {
                    Debug.Log("  " + param.name + " (" + param.type + ")");
                }
            }
        }

        Collider2D collider = GetComponent<Collider2D>();
        if (collider == null)
        {
            Debug.LogError("Collider2D not found");
        }
        else
        {
            Debug.Log("Collider2D: OK, isTrigger: " + collider.isTrigger);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            TestAnimator();
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            TestAttackAnimation();
        }

        if (Input.GetKeyDown(KeyCode.F3))
        {
            TestDeathAnimation();
        }
    }

    void TestAnimator()
    {
        Debug.Log("Current animator state hash: " + anim.GetCurrentAnimatorStateInfo(0).shortNameHash);
        Debug.Log("IsAttacking: " + anim.GetBool("IsAttacking"));
        Debug.Log("IsDead: " + anim.GetBool("IsDead"));
    }

    void TestAttackAnimation()
    {
        Debug.Log("Testing attack animation");
        anim.SetBool("IsAttacking", true);

        Invoke("ResetAttack", 1f);
    }

    void ResetAttack()
    {
        anim.SetBool("IsAttacking", false);
        Debug.Log("Attack animation reset");
    }

    void TestDeathAnimation()
    {
        Debug.Log("Testing death animation");
        anim.SetBool("IsDead", true);

        HealthEnemy health = GetComponent<HealthEnemy>();
        if (health != null)
        {
            health.TakeDamage(1000f);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Enemy collision with: " + collision.gameObject.name);
    }
}