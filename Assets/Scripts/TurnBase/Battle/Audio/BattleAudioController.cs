using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleAudioController : MonoBehaviour
{
    //==================================================
    // MANTRA FEEDBACK
    //==================================================

    public void PlayMantraSuccess()
    {
        if (AudioManager.Instance == null)
            return;

        AudioManager.Instance
            .PlayMantraSuccessSound();
    }


    public void PlayMantraFailed()
    {
        if (AudioManager.Instance == null)
            return;

        AudioManager.Instance
            .PlayMantraFailedSound();
    }


    //==================================================
    // PLAYER CAST
    //==================================================

    public void PlaySpellCast(
        SpellTier spellTier
    )
    {
        if (AudioManager.Instance == null)
            return;


        switch (spellTier)
        {
            case SpellTier.Basic:

                AudioManager.Instance
                    .PlayBasicCastSound();

                break;


            case SpellTier.Medium:

                AudioManager.Instance
                    .PlayMediumCastSound();

                break;


            case SpellTier.Advanced:

                AudioManager.Instance
                    .PlayAdvancedCastSound();

                break;
        }
    }


    //==================================================
    // MAGIC IMPACT
    //==================================================

    public void PlayMagicImpact()
    {
        if (AudioManager.Instance == null)
            return;

        AudioManager.Instance
            .PlayMagicImpactSound();
    }


    //==================================================
    // ENEMY ATTACK
    //==================================================

    public void PlayEnemyAttack()
    {
        if (AudioManager.Instance == null)
            return;

        AudioManager.Instance
            .PlayEnemyAttackSound();
    }


    //==================================================
    // SHIELD
    //==================================================

    public void PlayShieldBlock()
    {
        if (AudioManager.Instance == null)
            return;

        AudioManager.Instance
            .PlayShieldBlockSound();
    }


    //==================================================
    // PLAYER HIT
    //==================================================

    public void PlayPlayerHit()
    {
        if (AudioManager.Instance == null)
            return;

        AudioManager.Instance
            .PlayPlayerHitSound();
    }


    //==================================================
    // ENEMY HIT
    //==================================================

    public void PlayEnemyHit()
    {
        if (AudioManager.Instance == null)
            return;

        AudioManager.Instance
            .PlayEnemyHitSound();
    }


    //==================================================
    // RESULT
    //==================================================

    public void PlayVictory()
    {
        if (AudioManager.Instance == null)
            return;

        AudioManager.Instance
            .PlayVictorySound();
    }


    public void PlayDefeat()
    {
        if (AudioManager.Instance == null)
            return;

        AudioManager.Instance
            .PlayDefeatSound();
    }
}
