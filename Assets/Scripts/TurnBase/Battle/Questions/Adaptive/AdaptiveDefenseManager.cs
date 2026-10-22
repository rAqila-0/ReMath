using System;
using UnityEngine;

public class AdaptiveDefenseManager : MonoBehaviour
{
    //==================================================
    // SETTINGS
    //==================================================

    [Header("Initial Difficulty")]

    [SerializeField]
    private DefenseDifficulty initialDifficulty =
        DefenseDifficulty.Normal;


    [Header("Adaptive Threshold")]

    [Min(1)]
    [SerializeField]
    private int correctStreakToIncrease = 3;

    [Min(1)]
    [SerializeField]
    private int wrongStreakToDecrease = 2;


    [Header("Debug")]

    [SerializeField]
    private bool showDebugLog = true;


    //==================================================
    // RUNTIME DATA
    //==================================================

    [Header("Runtime - Read Only During Play Mode")]

    [SerializeField]
    private DefenseDifficulty currentDifficulty =
        DefenseDifficulty.Normal;

    [SerializeField]
    private int correctStreak;

    [SerializeField]
    private int wrongStreak;

    [SerializeField]
    private int totalCorrect;

    [SerializeField]
    private int totalWrong;


    //==================================================
    // EVENT
    //==================================================

    public event Action<DefenseDifficulty>
        DifficultyChanged;


    //==================================================
    // PROPERTIES
    //==================================================

    public DefenseDifficulty CurrentDifficulty =>
        currentDifficulty;


    public int CorrectStreak =>
        correctStreak;


    public int WrongStreak =>
        wrongStreak;


    public int TotalCorrect =>
        totalCorrect;


    public int TotalWrong =>
        totalWrong;


    public int TotalAnswers =>
        totalCorrect +
        totalWrong;


    public float Accuracy
    {
        get
        {
            if (TotalAnswers <= 0)
                return 0f;


            return
                (float)totalCorrect /
                TotalAnswers;
        }
    }


    //==================================================
    // AWAKE
    //==================================================

    private void Awake()
    {
        // Safety jika nilai Inspector aneh.
        correctStreakToIncrease =
            Mathf.Max(
                1,
                correctStreakToIncrease
            );


        wrongStreakToDecrease =
            Mathf.Max(
                1,
                wrongStreakToDecrease
            );


        ResetAdaptation();
    }


    //==================================================
    // REGISTER RESULT
    //==================================================

    public void RegisterDefenseResult(
        bool isCorrect
    )
    {
        if (isCorrect)
        {
            RegisterCorrect();
        }
        else
        {
            RegisterWrong();
        }
    }


    //==================================================
    // CORRECT
    //==================================================

    private void RegisterCorrect()
    {
        totalCorrect++;


        correctStreak++;

        // Salah beruntun terputus.
        wrongStreak = 0;


        if (showDebugLog)
        {
            Debug.Log(
                "Defense DDA: BENAR | " +
                "Correct Streak = " +
                correctStreak +
                " | Difficulty = " +
                currentDifficulty
            );
        }


        if (
            correctStreak >=
            correctStreakToIncrease
        )
        {
            correctStreak = 0;

            IncreaseDifficulty();
        }
    }


    //==================================================
    // WRONG
    //==================================================

    private void RegisterWrong()
    {
        totalWrong++;


        wrongStreak++;

        // Benar beruntun terputus.
        correctStreak = 0;


        if (showDebugLog)
        {
            Debug.Log(
                "Defense DDA: SALAH | " +
                "Wrong Streak = " +
                wrongStreak +
                " | Difficulty = " +
                currentDifficulty
            );
        }


        if (
            wrongStreak >=
            wrongStreakToDecrease
        )
        {
            wrongStreak = 0;

            DecreaseDifficulty();
        }
    }


    //==================================================
    // INCREASE DIFFICULTY
    //==================================================

    private void IncreaseDifficulty()
    {
        DefenseDifficulty previousDifficulty =
            currentDifficulty;


        switch (currentDifficulty)
        {
            case DefenseDifficulty.Easy:

                currentDifficulty =
                    DefenseDifficulty.Normal;

                break;


            case DefenseDifficulty.Normal:

                currentDifficulty =
                    DefenseDifficulty.Hard;

                break;


            case DefenseDifficulty.Hard:

                // Sudah maksimal.
                currentDifficulty =
                    DefenseDifficulty.Hard;

                break;
        }


        NotifyDifficultyChange(
            previousDifficulty
        );
    }


    //==================================================
    // DECREASE DIFFICULTY
    //==================================================

    private void DecreaseDifficulty()
    {
        DefenseDifficulty previousDifficulty =
            currentDifficulty;


        switch (currentDifficulty)
        {
            case DefenseDifficulty.Hard:

                currentDifficulty =
                    DefenseDifficulty.Normal;

                break;


            case DefenseDifficulty.Normal:

                currentDifficulty =
                    DefenseDifficulty.Easy;

                break;


            case DefenseDifficulty.Easy:

                // Sudah minimum.
                currentDifficulty =
                    DefenseDifficulty.Easy;

                break;
        }


        NotifyDifficultyChange(
            previousDifficulty
        );
    }


    //==================================================
    // NOTIFY
    //==================================================

    private void NotifyDifficultyChange(
        DefenseDifficulty previousDifficulty
    )
    {
        // Tidak berubah, misalnya sudah Hard.
        if (
            previousDifficulty ==
            currentDifficulty
        )
        {
            if (showDebugLog)
            {
                Debug.Log(
                    "Defense DDA: Difficulty tetap " +
                    currentDifficulty
                );
            }


            return;
        }


        if (showDebugLog)
        {
            Debug.Log(
                "Defense DDA: " +
                previousDifficulty +
                " → " +
                currentDifficulty
            );
        }


        DifficultyChanged?.Invoke(
            currentDifficulty
        );
    }


    //==================================================
    // RESET
    //==================================================

    public void ResetAdaptation()
    {
        currentDifficulty =
            initialDifficulty;


        correctStreak = 0;

        wrongStreak = 0;

        totalCorrect = 0;

        totalWrong = 0;


        if (showDebugLog)
        {
            Debug.Log(
                "Defense DDA Reset → " +
                currentDifficulty
            );
        }
    }

    //==================================================
    // RESTART ATTEMPT
    //==================================================

    public void RestartAttemptStreaks()
    {
        correctStreak = 0;

        wrongStreak = 0;


        if (showDebugLog)
        {
            Debug.Log(
                "Defense DDA Retry | " +
                "Difficulty tetap = " +
                currentDifficulty
            );
        }
    }


    //==================================================
    // DEBUG TEST
    //==================================================

    [ContextMenu("TEST - Defense Correct")]
    private void TestCorrect()
    {
        RegisterDefenseResult(true);
    }


    [ContextMenu("TEST - Defense Wrong")]
    private void TestWrong()
    {
        RegisterDefenseResult(false);
    }


    [ContextMenu("TEST - Reset Adaptive")]
    private void TestReset()
    {
        ResetAdaptation();
    }
}