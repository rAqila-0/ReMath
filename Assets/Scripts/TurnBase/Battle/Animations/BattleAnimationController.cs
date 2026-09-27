using System;
using System.Collections;
using UnityEngine;


public class BattleAnimationController : MonoBehaviour
{
    //==================================================
    // CHARACTER ANIMATORS
    //==================================================

    [Header("Character Animators")]

    [SerializeField]
    private Animator playerAnimator;

    [SerializeField]
    private Animator enemyAnimator;


    //==================================================
    // PROJECTILE POINTS
    //==================================================

    [Header("Projectile Points")]

    [SerializeField]
    private Transform playerCastPoint;

    [SerializeField]
    private Transform enemyHitPoint;


    //==================================================
    // PROJECTILES
    //==================================================

    [Header("Spell Projectiles")]

    [SerializeField]
    private GameObject basicProjectile;

    [SerializeField]
    private GameObject mediumProjectile;

    [SerializeField]
    private GameObject advancedProjectile;


    //==================================================
    // DEFENSE
    //==================================================

    [Header("Defense Effect")]

    [SerializeField]
    private GameObject shieldEffect;


    //==================================================
    // TIMING
    //==================================================

    [Header("Player Attack Timing")]

    [SerializeField]
    private float castWindUpDuration = 0.4f;

    [SerializeField]
    private float projectileTravelDuration = 0.45f;

    [SerializeField]
    private float enemyHitDuration = 0.4f;


    [Header("Enemy Attack Timing")]

    [SerializeField]
    private float enemyAttackImpactDelay = 0.55f;

    [SerializeField]
    private float playerHitDuration = 0.4f;


    [Header("Defense Timing")]

    [SerializeField]
    private float shieldAppearDelay = 0.15f;

    [SerializeField]
    private float shieldHoldDuration = 0.5f;


    [Header("Result Timing")]

    [SerializeField]
    private float resultAnimationDuration = 0.8f;


    //==================================================
    // AWAKE
    //==================================================

    private void Awake()
    {
        ResetVisualState();
    }


    //==================================================
    // PLAYER ATTACK
    //==================================================

    public IEnumerator PlayPlayerAttackUntilImpact(
        SpellTier spellTier
    )
    {
        Debug.Log(
            "ANIMATION: Player attack " +
            spellTier
        );
        //==============================================
        // CAST ANIMATION
        //==============================================

        switch (spellTier)
        {
            case SpellTier.Basic:

                SetTriggerIfExists(
                    playerAnimator,
                    "CastBasic"
                );

                break;


            case SpellTier.Medium:

                SetTriggerIfExists(
                    playerAnimator,
                    "CastMedium"
                );

                break;


            case SpellTier.Advanced:

                SetTriggerIfExists(
                    playerAnimator,
                    "CastAdvanced"
                );

                break;
        }


        yield return new WaitForSeconds(
            castWindUpDuration
        );


        //==============================================
        // PROJECTILE
        //==============================================

        GameObject projectile =
            GetProjectile(spellTier);

        if (projectile == null)
        {
            Debug.LogError(
                "PROJECTILE NULL untuk spell: " +
                spellTier
            );
        }
        else
        {
            Debug.Log(
                "Projectile ditemukan: " +
                projectile.name
            );
        }


        if (
            projectile == null ||
            playerCastPoint == null ||
            enemyHitPoint == null
        )
        {
            // Tetap memberi waktu animasi
            // walaupun projectile belum dipasang.
            yield return new WaitForSeconds(
                projectileTravelDuration
            );

            yield break;
        }


        DisableAllProjectiles();


        projectile.SetActive(true);

        Debug.Log(
            "Projectile ACTIVE"
        );


        Vector3 startPosition =
            playerCastPoint.position;

        Vector3 targetPosition =
            enemyHitPoint.position;


        projectile.transform.position =
            startPosition;


        float elapsedTime = 0f;


        while (
            elapsedTime <
            projectileTravelDuration
        )
        {
            elapsedTime +=
                Time.deltaTime;


            float progress =
                elapsedTime /
                projectileTravelDuration;


            progress =
                Mathf.Clamp01(progress);


            projectile.transform.position =
                Vector3.Lerp(
                    startPosition,
                    targetPosition,
                    progress
                );


            yield return null;
        }


        projectile.transform.position =
            targetPosition;


        projectile.SetActive(false);
    }


    //==================================================
    // ENEMY HIT
    //==================================================

    public IEnumerator PlayEnemyHit()
    {
        SetTriggerIfExists(
            enemyAnimator,
            "Hit"
        );


        yield return new WaitForSeconds(
            enemyHitDuration
        );
    }


    //==================================================
    // PLAYER SPELL FAIL
    //==================================================

    public IEnumerator PlayPlayerSpellFail()
    {
        SetTriggerIfExists(
            playerAnimator,
            "Fail"
        );


        yield return new WaitForSeconds(
            0.3f
        );
    }


    //==================================================
    // ENEMY ATTACK UNTIL IMPACT
    //==================================================

    public IEnumerator PlayEnemyAttackUntilImpact()
    {
        SetTriggerIfExists(
            enemyAnimator,
            "Attack"
        );


        yield return new WaitForSeconds(
            enemyAttackImpactDelay
        );
    }


    //==================================================
    // PLAYER HIT
    //==================================================

    public IEnumerator PlayPlayerHit()
    {
        SetTriggerIfExists(
            playerAnimator,
            "Hit"
        );


        yield return new WaitForSeconds(
            playerHitDuration
        );
    }


    //==================================================
    // DEFENSE SUCCESS
    //==================================================

    public IEnumerator PlayDefenseSuccess(
    Action onImpact = null
    )
    {
        //==============================================
        // PLAYER DEFENSE
        //==============================================

        SetTriggerIfExists(
            playerAnimator,
            "Defense"
        );


        yield return new WaitForSeconds(
            shieldAppearDelay
        );


        //==============================================
        // SHIELD ON
        //==============================================

        if (shieldEffect != null)
        {
            shieldEffect.SetActive(true);
        }


        //==============================================
        // ENEMY ATTACK
        //==============================================

        SetTriggerIfExists(
            enemyAnimator,
            "Attack"
        );


        yield return new WaitForSeconds(
            enemyAttackImpactDelay
        );


        //==============================================
        // IMPACT KE SHIELD
        //==============================================

        onImpact?.Invoke();


        yield return new WaitForSeconds(
            shieldHoldDuration
        );


        //==============================================
        // SHIELD OFF
        //==============================================

        if (shieldEffect != null)
        {
            shieldEffect.SetActive(false);
        }
    }

    //==================================================
    // PLAYER VICTORY
    //==================================================

    public IEnumerator PlayPlayerVictory()
    {
        SetTriggerIfExists(
            playerAnimator,
            "Victory"
        );


        yield return new WaitForSeconds(
            resultAnimationDuration
        );
    }


    //==================================================
    // ENEMY DEFEAT
    //==================================================

    public IEnumerator PlayEnemyDefeat()
    {
        SetTriggerIfExists(
            enemyAnimator,
            "Defeat"
        );


        yield return new WaitForSeconds(
            resultAnimationDuration
        );
    }


    //==================================================
    // PLAYER DEFEAT
    //==================================================

    public IEnumerator PlayPlayerDefeat()
    {
        SetTriggerIfExists(
            playerAnimator,
            "Defeat"
        );


        yield return new WaitForSeconds(
            resultAnimationDuration
        );
    }


    //==================================================
    // GET PROJECTILE
    //==================================================

    private GameObject GetProjectile(
        SpellTier spellTier
    )
    {
        switch (spellTier)
        {
            case SpellTier.Basic:

                return basicProjectile;


            case SpellTier.Medium:

                return mediumProjectile;


            case SpellTier.Advanced:

                return advancedProjectile;


            default:

                return null;
        }
    }


    //==================================================
    // DISABLE PROJECTILES
    //==================================================

    private void DisableAllProjectiles()
    {
        if (basicProjectile != null)
            basicProjectile.SetActive(false);

        if (mediumProjectile != null)
            mediumProjectile.SetActive(false);

        if (advancedProjectile != null)
            advancedProjectile.SetActive(false);
    }


    //==================================================
    // RESET VISUAL
    //==================================================

    public void ResetVisualState()
    {
        DisableAllProjectiles();


        if (shieldEffect != null)
        {
            shieldEffect.SetActive(false);
        }
    }


    //==================================================
    // SAFE ANIMATOR TRIGGER
    //==================================================

    private void SetTriggerIfExists(
        Animator animator,
        string triggerName
    )
    {
        if (animator == null)
            return;


        foreach (
            AnimatorControllerParameter parameter
            in animator.parameters
        )
        {
            if (
                parameter.type ==
                AnimatorControllerParameterType.Trigger &&
                parameter.name ==
                triggerName
            )
            {
                animator.SetTrigger(
                    triggerName
                );

                return;
            }
        }
    }
}