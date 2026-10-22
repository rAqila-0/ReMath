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
    // HEART FEEDBACK
    //==================================================

    [Header("Heart Feedback")]

    [SerializeField]
    private RectTransform heartContainer;

    [SerializeField]
    private float heartPulseScale = 1.08f;

    [SerializeField]
    private float heartPulseDuration = 0.18f;


    private Coroutine heartPulseCoroutine;

    private Vector3 heartOriginalScale;


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

    private void Awake()
    {
        if (heartContainer != null)
        {
            heartOriginalScale =
                heartContainer.localScale;
        }
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

        PlayHeartPulse();

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

        if (heartPulseCoroutine != null)
        {
            StopCoroutine(
                heartPulseCoroutine
            );

            heartPulseCoroutine = null;
        }


        if (heartContainer != null)
        {
            heartContainer.localScale =
                heartOriginalScale;
        }
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

    private void PlayHeartPulse()
    {
        if (heartContainer == null)
            return;


        if (heartPulseCoroutine != null)
        {
            StopCoroutine(
                heartPulseCoroutine
            );
        }


        heartPulseCoroutine =
            StartCoroutine(
                HeartPulseRoutine()
            );
    }


    private IEnumerator HeartPulseRoutine()
    {
        float halfDuration =
            heartPulseDuration * 0.5f;


        //==============================================
        // SCALE UP
        //==============================================

        float elapsed = 0f;


        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;


            float progress =
                Mathf.Clamp01(
                    elapsed /
                    halfDuration
                );


            heartContainer.localScale =
                Vector3.Lerp(
                    heartOriginalScale,
                    heartOriginalScale *
                    heartPulseScale,
                    progress
                );


            yield return null;
        }


        //==============================================
        // SCALE BACK
        //==============================================

        elapsed = 0f;


        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;


            float progress =
                Mathf.Clamp01(
                    elapsed /
                    halfDuration
                );


            heartContainer.localScale =
                Vector3.Lerp(
                    heartOriginalScale *
                    heartPulseScale,
                    heartOriginalScale,
                    progress
                );


            yield return null;
        }


        heartContainer.localScale =
            heartOriginalScale;


        heartPulseCoroutine = null;
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
