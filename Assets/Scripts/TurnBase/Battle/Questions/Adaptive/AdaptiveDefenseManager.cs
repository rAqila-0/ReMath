using System;
using UnityEngine;

/// <summary>
/// Shared adaptive difficulty manager for BOTH attack and defense questions.
/// Kept under the legacy class/file name so existing scene references remain valid.
/// </summary>
public class AdaptiveDefenseManager : MonoBehaviour
{
    private const int MinDigitLevel = 1;
    private const int MaxDigitLevel = 4;
    private const int DefaultStartingDigitLevel = 2;
    private const int WrongAnswersToDecrease = 2;

    [Header("Adaptive Digit Level")]
    [Tooltip("Level awal jika belum ada progress level sebelumnya. Re:Math menggunakan 2 digit sebagai awal.")]
    [SerializeField, Range(MinDigitLevel, MaxDigitLevel)]
    private int initialDigitLevel = DefaultStartingDigitLevel;

    [Header("Debug")]
    [SerializeField]
    private bool showDebugLog = true;

    [Header("Runtime - Read Only During Play Mode")]
    [SerializeField, Range(MinDigitLevel, MaxDigitLevel)]
    private int currentDigitLevel = DefaultStartingDigitLevel;

    [SerializeField] private int correctStreak;
    [SerializeField] private int wrongStreak;
    [SerializeField] private int totalCorrect;
    [SerializeField] private int totalWrong;

    private string activeProgressKey = string.Empty;
    private string activeOperationName = string.Empty;
    private int activeLevelNumber = 1;
    private int activeMaximumDigitLevel = MaxDigitLevel;

    // BattleManager (current project version) also calls RegisterDefenseResult
    // after QuestionManager.CheckAnswer. This flag prevents counting that same
    // defense answer twice after QuestionManager has registered it here.
    private bool skipNextLegacyDefenseRegistration;
    private bool skippedRegistrationExpectedResult;

    public event Action<DefenseDifficulty> DifficultyChanged;

    public int CurrentDigitLevel => Mathf.Clamp(currentDigitLevel, MinDigitLevel, activeMaximumDigitLevel);
    // Compatibility alias. For multiplication/division this is a DDA tier,
    // not a literal count of operand digits.
    public int CurrentOperandDigits => CurrentDigitLevel;
    public int CorrectStreak => correctStreak;
    public int WrongStreak => wrongStreak;
    public int TotalCorrect => totalCorrect;
    public int TotalWrong => totalWrong;
    public int TotalAnswers => totalCorrect + totalWrong;
    public float Accuracy => TotalAnswers <= 0 ? 0f : (float)totalCorrect / TotalAnswers;

    // Compatibility with scripts that still read DefenseDifficulty.
    // 1-2 digit = Easy, 3 digit = Normal, 4 digit = Hard.
    public DefenseDifficulty CurrentDifficulty
    {
        get
        {
            if (CurrentDigitLevel <= 2) return DefenseDifficulty.Easy;
            if (CurrentDigitLevel == 3) return DefenseDifficulty.Normal;
            return DefenseDifficulty.Hard;
        }
    }

    private void Awake()
    {
        initialDigitLevel = Mathf.Clamp(initialDigitLevel, MinDigitLevel, MaxDigitLevel);
        currentDigitLevel = Mathf.Clamp(initialDigitLevel, MinDigitLevel, activeMaximumDigitLevel);
        ResetRuntimeCounters();
    }

    /// <summary>
    /// Call once from QuestionManager.Initialize. Each operation/stage stores
    /// a separate DDA tier (1..4) for each level. A fresh level inherits the
    /// final DDA tier saved by the previous level in the same operation/stage.
    /// </summary>
    public void InitializeForLevel(string operationName, int levelNumber)
    {
        InitializeForLevel(operationName, levelNumber, MaxDigitLevel);
    }

    /// <summary>
    /// Initializes the shared DDA tier for the current operation and level.
    /// All operations use tiers 1..4; QuestionManager maps those tiers to
    /// operation-appropriate operand ranges.
    /// </summary>
    public void InitializeForLevel(
        string operationName, int levelNumber, int maximumDigitLevel)
    {
        activeMaximumDigitLevel = Mathf.Clamp(
            maximumDigitLevel, MinDigitLevel, MaxDigitLevel);

        activeOperationName = string.IsNullOrWhiteSpace(operationName)
            ? "UnknownOperation"
            : operationName;
        activeLevelNumber = Mathf.Max(1, levelNumber);
        activeProgressKey = BuildProgressKey(activeOperationName, activeLevelNumber);
        skipNextLegacyDefenseRegistration = false;

        if (PlayerPrefs.HasKey(activeProgressKey))
        {
            LoadActiveProgress();
            currentDigitLevel = Mathf.Clamp(
                currentDigitLevel, MinDigitLevel, activeMaximumDigitLevel);
            SaveActiveProgress();
            Log("Loaded saved DDA progress for " + activeOperationName + " level " + activeLevelNumber + ".");
            return;
        }

        int startDigit = Mathf.Clamp(initialDigitLevel, MinDigitLevel, activeMaximumDigitLevel);
        if (activeLevelNumber > 1)
        {
            string previousKey = BuildProgressKey(activeOperationName, activeLevelNumber - 1);
            if (PlayerPrefs.HasKey(previousKey))
            {
                AdaptiveProgressSave previous = ParseProgress(PlayerPrefs.GetString(previousKey));
                if (previous != null)
                    startDigit = Mathf.Clamp(previous.currentDigitLevel, MinDigitLevel, activeMaximumDigitLevel);
            }
        }

        currentDigitLevel = startDigit;
        ResetRuntimeCounters();
        SaveActiveProgress();
        Log("New DDA level initialized at " + currentDigitLevel + " digit(s): " + activeOperationName + " level " + activeLevelNumber + ".");
    }

    /// <summary>
    /// Legacy BattleManager call during battle initialization. It now reloads
    /// saved progress instead of clearing the player's DDA progress.
    /// </summary>
    public void ResetAdaptation()
    {
        if (!string.IsNullOrEmpty(activeProgressKey) && PlayerPrefs.HasKey(activeProgressKey))
        {
            LoadActiveProgress();
            return;
        }

        currentDigitLevel = Mathf.Clamp(initialDigitLevel, MinDigitLevel, activeMaximumDigitLevel);
        ResetRuntimeCounters();
    }

    /// <summary>
    /// Shared answer registration for every attack and defense spell.
    /// One correct answer increases the DDA tier by one, up to tier 4. Two
    /// consecutive wrong answers decrease it by one, floored at tier 1.
    /// </summary>
    public void RegisterQuestionResult(bool isCorrect, SpellTier spellTier)
    {
        if (spellTier == SpellTier.Defense)
        {
            skipNextLegacyDefenseRegistration = true;
            skippedRegistrationExpectedResult = isCorrect;
        }

        RegisterResultCore(isCorrect, spellTier);
    }

    /// <summary>
    /// Used for attack results by integrations that don't supply a tier.
    /// </summary>
    public void RegisterAttackResult(bool isCorrect)
    {
        RegisterResultCore(isCorrect, SpellTier.Basic);
    }

    /// <summary>
    /// Kept for compatibility with the current BattleManager. The question
    /// result is already registered by QuestionManager, so the next matching
    /// call is ignored once to avoid double-counting defense answers.
    /// </summary>
    public void RegisterDefenseResult(bool isCorrect)
    {
        if (skipNextLegacyDefenseRegistration && skippedRegistrationExpectedResult == isCorrect)
        {
            skipNextLegacyDefenseRegistration = false;
            Log("Defense result already registered by QuestionManager; legacy duplicate ignored.");
            return;
        }

        skipNextLegacyDefenseRegistration = false;
        RegisterResultCore(isCorrect, SpellTier.Defense);
    }

    private void RegisterResultCore(bool isCorrect, SpellTier spellTier)
    {
        int previousDigitLevel = CurrentDigitLevel;

        if (isCorrect)
        {
            totalCorrect++;
            correctStreak++;
            wrongStreak = 0;
            currentDigitLevel = Mathf.Min(activeMaximumDigitLevel, CurrentDigitLevel + 1);
        }
        else
        {
            totalWrong++;
            wrongStreak++;
            correctStreak = 0;

            if (wrongStreak >= WrongAnswersToDecrease)
            {
                wrongStreak = 0;
                currentDigitLevel = Mathf.Max(MinDigitLevel, CurrentDigitLevel - 1);
            }
        }

        currentDigitLevel = Mathf.Clamp(currentDigitLevel, MinDigitLevel, activeMaximumDigitLevel);
        SaveActiveProgress();

        if (previousDigitLevel != currentDigitLevel)
            DifficultyChanged?.Invoke(CurrentDifficulty);

        Log(
            (isCorrect ? "BENAR" : "SALAH") +
            " | spell=" + spellTier +
            " | DDA tier berikutnya=" + CurrentDigitLevel +
            " | salah beruntun=" + wrongStreak);
    }

    /// <summary>
    /// Used when retrying the just-failed battle: restore the digit level with
    /// which that attempt started, while preserving the saved per-level progress.
    /// </summary>
    public void SetDigitLevelForReplay(int digitLevel)
    {
        int previousDigitLevel = CurrentDigitLevel;
        currentDigitLevel = Mathf.Clamp(digitLevel, MinDigitLevel, activeMaximumDigitLevel);
        SaveActiveProgress();

        if (previousDigitLevel != currentDigitLevel)
            DifficultyChanged?.Invoke(CurrentDifficulty);
    }

    /// <summary>
    /// Existing BattleManager calls this during Retry. It resets streaks only;
    /// the digit level is restored separately by QuestionManager.
    /// </summary>
    public void RestartAttemptStreaks()
    {
        correctStreak = 0;
        wrongStreak = 0;
        SaveActiveProgress();
        Log("Retry: streaks reset; DDA tier remains " + CurrentDigitLevel + ".");
    }

    private void LoadActiveProgress()
    {
        AdaptiveProgressSave data = ParseProgress(PlayerPrefs.GetString(activeProgressKey, string.Empty));
        if (data == null)
        {
            currentDigitLevel = Mathf.Clamp(initialDigitLevel, MinDigitLevel, activeMaximumDigitLevel);
            ResetRuntimeCounters();
            SaveActiveProgress();
            return;
        }

        currentDigitLevel = Mathf.Clamp(data.currentDigitLevel, MinDigitLevel, activeMaximumDigitLevel);
        correctStreak = Mathf.Max(0, data.correctStreak);
        wrongStreak = Mathf.Clamp(data.wrongStreak, 0, WrongAnswersToDecrease - 1);
        totalCorrect = Mathf.Max(0, data.totalCorrect);
        totalWrong = Mathf.Max(0, data.totalWrong);
    }

    private void SaveActiveProgress()
    {
        if (string.IsNullOrEmpty(activeProgressKey))
            return;

        AdaptiveProgressSave data = new AdaptiveProgressSave
        {
            currentDigitLevel = CurrentDigitLevel,
            correctStreak = correctStreak,
            wrongStreak = wrongStreak,
            totalCorrect = totalCorrect,
            totalWrong = totalWrong
        };

        PlayerPrefs.SetString(activeProgressKey, JsonUtility.ToJson(data));
        PlayerPrefs.Save();
    }

    private static AdaptiveProgressSave ParseProgress(string json)
    {
        if (string.IsNullOrEmpty(json)) return null;
        try { return JsonUtility.FromJson<AdaptiveProgressSave>(json); }
        catch (Exception exception)
        {
            Debug.LogWarning("AdaptiveDefenseManager: progress JSON could not be read: " + exception.Message);
            return null;
        }
    }

    private static string BuildProgressKey(string operationName, int levelNumber)
    {
        return "ReMath_DDA_" + operationName + "_Level_" + levelNumber;
    }

    private void ResetRuntimeCounters()
    {
        correctStreak = 0;
        wrongStreak = 0;
        totalCorrect = 0;
        totalWrong = 0;
    }

    private void Log(string message)
    {
        if (showDebugLog) Debug.Log("Question DDA: " + message);
    }

    [Serializable]
    private class AdaptiveProgressSave
    {
        public int currentDigitLevel = DefaultStartingDigitLevel;
        public int correctStreak;
        public int wrongStreak;
        public int totalCorrect;
        public int totalWrong;
    }

    [ContextMenu("TEST - Correct Answer (Increase One Digit)")]
    private void TestCorrect()
    {
        RegisterQuestionResult(true, SpellTier.Basic);
    }

    [ContextMenu("TEST - Wrong Answer")]
    private void TestWrong()
    {
        RegisterQuestionResult(false, SpellTier.Basic);
    }

    [ContextMenu("TEST - Load Saved Level Progress")]
    private void TestReload()
    {
        ResetAdaptation();
    }
}
