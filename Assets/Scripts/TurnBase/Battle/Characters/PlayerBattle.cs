using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBattle : MonoBehaviour
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
            $"Player terkena {damage} damage. " +
            $"Nyawa tersisa: {currentLives}/{maxLives}"
        );

        if (IsDead)
        {
            Debug.Log("Player kehabisan nyawa.");
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
                    // Kalau belum punya sprite kosong,
                    // hati cukup disembunyikan.
                    heartImages[i].enabled = false;
                }
            }
        }
    }


    //==================================================
    // TEST - OPTIONAL
    //==================================================

    [ContextMenu("TEST - Damage 1")]
    private void TestDamage1()
    {
        TakeDamage(1);
    }


    [ContextMenu("TEST - Damage 2")]
    private void TestDamage2()
    {
        TakeDamage(2);
    }


    [ContextMenu("TEST - Reset Lives")]
    private void TestResetLives()
    {
        ResetLives();
    }
}
