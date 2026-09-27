using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBattle : MonoBehaviour
{
    //==================================================
    // LIFE
    //==================================================

    [Header("Life Settings")]

    [SerializeField]
    private int maxLives = 5;

    private int currentLives;


    //==================================================
    // HEART UI
    //==================================================

    [Header("Heart UI")]

    [SerializeField]
    private Image[] heartImages;

    [SerializeField]
    private Sprite fullHeartSprite;

    [SerializeField]
    private Sprite emptyHeartSprite;


    //==================================================
    // PROPERTIES
    //==================================================

    public int CurrentLives => currentLives;

    public int MaxLives => maxLives;

    public bool IsDead => currentLives <= 0;


    //==================================================
    // START
    //==================================================

    private void Start()
    {
        ResetLives();
    }


    //==================================================
    // INITIALIZE
    //==================================================

    public void Initialize(int lives)
    {
        maxLives = Mathf.Max(1, lives);

        ResetLives();
    }


    //==================================================
    // TAKE DAMAGE
    //==================================================

    public void TakeDamage(int damage)
    {
        if (damage <= 0)
            return;

        if (IsDead)
            return;

        currentLives -= damage;

        currentLives = Mathf.Clamp(
            currentLives,
            0,
            maxLives
        );

        UpdateHeartUI();

        Debug.Log(
            $"Enemy terkena {damage} damage. " +
            $"Nyawa tersisa: {currentLives}/{maxLives}"
        );

        if (IsDead)
        {
            Debug.Log("Enemy kehabisan nyawa.");
        }
    }


    //==================================================
    // RESET LIFE
    //==================================================

    public void ResetLives()
    {
        currentLives = maxLives;

        UpdateHeartUI();
    }


    //==================================================
    // UPDATE HEART UI
    //==================================================

    private void UpdateHeartUI()
    {
        if (heartImages == null)
            return;

        for (int i = 0; i < heartImages.Length; i++)
        {
            if (heartImages[i] == null)
                continue;

            // Heart masih aktif
            if (i < currentLives)
            {
                heartImages[i].enabled = true;

                if (fullHeartSprite != null)
                {
                    heartImages[i].sprite =
                        fullHeartSprite;
                }
            }

            // Heart sudah hilang
            else
            {
                if (emptyHeartSprite != null)
                {
                    heartImages[i].enabled = true;

                    heartImages[i].sprite =
                        emptyHeartSprite;
                }
                else
                {
                    heartImages[i].enabled = false;
                }
            }
        }
    }


    //==================================================
    // TEST
    //==================================================

    [ContextMenu("TEST - Basic Spell Damage")]
    private void TestBasicDamage()
    {
        TakeDamage(1);
    }


    [ContextMenu("TEST - Medium Spell Damage")]
    private void TestMediumDamage()
    {
        TakeDamage(2);
    }


    [ContextMenu("TEST - Advanced Spell Damage")]
    private void TestAdvancedDamage()
    {
        TakeDamage(3);
    }


    [ContextMenu("TEST - Reset Lives")]
    private void TestResetLives()
    {
        ResetLives();
    }
}