using UnityEngine;

[RequireComponent(typeof(Animator))]
public class SimpleAnimatorSetup : MonoBehaviour
{
    public AnimationClip idleClip;
    public AnimationClip attackClip;
    public AnimationClip deathClip;

    public bool testAttack = false;
    public bool testDeath = false;

    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();

        if (anim.runtimeAnimatorController == null)
        {
            Debug.LogWarning("No Animator Controller found");
            Debug.Log("Create Animator Controller in Project window");
            Debug.Log("Right-click -> Create -> Animator Controller");
            Debug.Log("Name it Enemy_AC_Simple");
            Debug.Log("Assign it to this enemy");
        }
    }

    void Update()
    {
        if (testAttack)
        {
            testAttack = false;
            if (anim != null)
            {
                anim.SetBool("IsAttacking", true);
                Invoke("ResetAttack", 0.5f);
            }
        }

        if (testDeath)
        {
            testDeath = false;
            if (anim != null)
            {
                anim.SetBool("IsDead", true);
            }
        }
    }

    void ResetAttack()
    {
        if (anim != null)
        {
            anim.SetBool("IsAttacking", false);
        }
    }
}