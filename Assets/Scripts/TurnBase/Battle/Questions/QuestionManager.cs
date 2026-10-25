using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class QuestionManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private QuestionDisplayController questionDisplayController;
    [SerializeField] private AdaptiveDefenseManager adaptiveDefenseManager;

    private BattleLevelConfig levelConfig;
    private QuestionData currentQuestion;
    private LevelQuestionProgress progress;
    private string questionProgressKey = string.Empty;

    // The current question remains pending after a first wrong answer. After
    // correct / second consecutive wrong answer, it is replaced by a question
    // generated at the updated shared digit level.
    private readonly List<QuestionData> questionsShownThisAttempt = new List<QuestionData>();
    private readonly List<QuestionData> retryQuestionSequence = new List<QuestionData>();
    private int retryQuestionIndex;
    private bool replayingRetrySequence;
    private int attemptStartDigitLevel = 2;

    public QuestionData CurrentQuestion => currentQuestion;
    public bool HasActiveQuestion => currentQuestion != null;
    public int CurrentDigitLevel => GetCurrentDigitLevel();

    [Serializable]
    private class LevelQuestionProgress
    {
        public int schemaVersion = 1;
        public int savedDigitLevel;
        public int savedWrongStreak;
        public QuestionData pendingQuestion;
        public List<QuestionData> questionHistory = new List<QuestionData>();
    }

    //==================================================
    // INITIALIZE / LOAD PER-LEVEL PROGRESS
    //==================================================

    public void Initialize(BattleLevelConfig config)
    {
        levelConfig = config;
        currentQuestion = null;
        questionsShownThisAttempt.Clear();
        retryQuestionSequence.Clear();
        retryQuestionIndex = 0;
        replayingRetrySequence = false;

        if (levelConfig == null)
        {
            Debug.LogError("QuestionManager: BattleLevelConfig belum diinisialisasi.");
            return;
        }

        if (adaptiveDefenseManager != null)
        {
            adaptiveDefenseManager.InitializeForLevel(
                levelConfig.operation.ToString(),
                levelConfig.levelNumber,
                GetMaximumDdaLevelForOperation(levelConfig.operation));
        }
        else
        {
            Debug.LogError(
                "QuestionManager: AdaptiveDefenseManager belum diisi. " +
                "DDA akan menggunakan fallback lokal, tetapi pastikan reference " +
                "ini menunjuk ke komponen yang sama dengan BattleManager.");
        }

        questionProgressKey = BuildQuestionProgressKey(
            levelConfig.operation,
            levelConfig.levelNumber);

        LoadQuestionProgress();

        // Semua operasi memakai satu DDA level bersama (1..4).
        // Bentuk operand tetap dibatasi sesuai operasi: tambah/kurang 1-4 digit,
        // perkalian 1..10 x 1..10, pembagian 10..99 : 1..9.
        progress.savedDigitLevel = Mathf.Clamp(progress.savedDigitLevel, 1, 4);

        // Buang pending question lama yang tidak sesuai aturan operasi terbaru.
        if (progress.pendingQuestion != null &&
            !IsQuestionWithinConfiguredOperandLimit(progress.pendingQuestion))
        {
            Debug.LogWarning(
                "QuestionManager: pending question lama tidak sesuai batas operasi terbaru; " +
                "membuat soal baru sesuai aturan operasi saat ini.");
            progress.pendingQuestion = null;
        }

        // Question progress is the source of truth if an older save has the
        // question stored but the DDA key is missing/out of sync.
        if (adaptiveDefenseManager != null && progress.savedDigitLevel >= 1)
        {
            adaptiveDefenseManager.SetDigitLevelForReplay(progress.savedDigitLevel);
        }

        if (progress.pendingQuestion == null)
        {
            CreatePendingQuestion(SpellTier.Basic, GetCurrentDigitLevel());
        }

        attemptStartDigitLevel = GetCurrentDigitLevel();
        SyncAndSaveProgress();

        Debug.Log(
            "QuestionManager: progress level dimuat | operation=" + levelConfig.operation +
            " | level=" + levelConfig.levelNumber +
            " | digit awal=" + GetCurrentDigitLevel() +
            " | pending=" + FormatDebugQuestion(progress.pendingQuestion));
    }

    /// <summary>
    /// Explicitly starts a fresh question sequence in the current level.
    /// Does not clear other levels' saved progress.
    /// </summary>
    public void StartNewSession()
    {
        currentQuestion = null;
        questionsShownThisAttempt.Clear();
        retryQuestionSequence.Clear();
        retryQuestionIndex = 0;
        replayingRetrySequence = false;

        progress = new LevelQuestionProgress
        {
            savedDigitLevel = GetCurrentDigitLevel(),
            savedWrongStreak = GetCurrentWrongStreak()
        };

        if (levelConfig != null)
        {
            CreatePendingQuestion(SpellTier.Basic, GetCurrentDigitLevel());
            SyncAndSaveProgress();
        }
    }

    private void LoadQuestionProgress()
    {
        progress = null;

        if (PlayerPrefs.HasKey(questionProgressKey))
        {
            string json = PlayerPrefs.GetString(questionProgressKey, string.Empty);
            try
            {
                progress = JsonUtility.FromJson<LevelQuestionProgress>(json);
            }
            catch (Exception exception)
            {
                Debug.LogWarning("QuestionManager: progress soal tidak dapat dibaca: " + exception.Message);
            }
        }

        if (progress == null)
            progress = new LevelQuestionProgress();

        if (progress.questionHistory == null)
            progress.questionHistory = new List<QuestionData>();

        for (int i = 0; i < progress.questionHistory.Count; i++)
        {
            progress.questionHistory[i] = NormalizeQuestion(progress.questionHistory[i]);
        }

        progress.pendingQuestion = NormalizeQuestion(progress.pendingQuestion);

        if (progress.savedDigitLevel < 1 || progress.savedDigitLevel > 4)
        {
            progress.savedDigitLevel = adaptiveDefenseManager != null
                ? adaptiveDefenseManager.CurrentDigitLevel
                : 2;
        }

        progress.savedDigitLevel = Mathf.Clamp(
            progress.savedDigitLevel, 1, GetMaximumDigitLevel());
    }

    //==================================================
    // GENERATE / DISPLAY QUESTION
    //==================================================

    public QuestionData GenerateQuestion(SpellTier spellTier)
    {
        if (levelConfig == null)
        {
            Debug.LogError("QuestionManager: BattleLevelConfig belum diinisialisasi.");
            return null;
        }

        QuestionData sourceQuestion;

        // Retry within the same battle replays the exact displayed question
        // sequence from that attempt, including duplicate questions after a
        // first wrong answer.
        if (replayingRetrySequence && retryQuestionIndex < retryQuestionSequence.Count)
        {
            sourceQuestion = NormalizeQuestion(retryQuestionSequence[retryQuestionIndex]);
            retryQuestionIndex++;

            if (sourceQuestion != null)
            {
                if (adaptiveDefenseManager != null)
                {
                    adaptiveDefenseManager.SetDigitLevelForReplay(
                        sourceQuestion.adaptiveLevel);
                }
                else
                {
                    progress.savedDigitLevel = sourceQuestion.adaptiveLevel;
                }

                progress.pendingQuestion = CloneQuestion(sourceQuestion, sourceQuestion.spellTier);
                SyncAndSaveProgress();
            }
        }
        else
        {
            replayingRetrySequence = false;

            if (progress.pendingQuestion == null)
            {
                CreatePendingQuestion(spellTier, GetCurrentDigitLevel());
            }

            sourceQuestion = progress.pendingQuestion;
        }

        if (sourceQuestion == null)
        {
            Debug.LogError("QuestionManager: Tidak ada soal pending yang dapat ditampilkan.");
            return null;
        }

        QuestionData questionForThisSpell = CloneQuestion(sourceQuestion, spellTier);

        if (!ApplyQuestion(questionForThisSpell))
            return null;

        questionsShownThisAttempt.Add(CloneQuestion(questionForThisSpell, spellTier));
        SyncAndSaveProgress();

        Debug.Log("QuestionManager: " + FormatDebugQuestion(questionForThisSpell));
        return questionForThisSpell;
    }

    private bool ApplyQuestion(QuestionData question)
    {
        if (question == null)
            return false;

        if (questionDisplayController == null)
        {
            Debug.LogError("QuestionManager: QuestionDisplayController belum diisi.");
            return false;
        }

        if (!questionDisplayController.DisplayQuestion(question))
            return false;

        currentQuestion = question;
        return true;
    }

    //==================================================
    // ANSWER CHECK + SHARED DDA REGISTRATION
    //==================================================

    public bool CheckAnswer(int playerAnswer)
    {
        if (currentQuestion == null)
        {
            Debug.LogWarning("QuestionManager: Tidak ada soal aktif.");
            return false;
        }

        QuestionData answeredQuestion = currentQuestion;
        bool isCorrect = playerAnswer == answeredQuestion.correctAnswer;

        if (adaptiveDefenseManager != null)
        {
            // Register for every question, regardless of selected attack spell
            // or defense spell. RegisterQuestionResult also guards against the
            // legacy duplicate RegisterDefenseResult call in BattleManager.
            adaptiveDefenseManager.RegisterQuestionResult(
                isCorrect,
                answeredQuestion.spellTier);
        }
        else
        {
            ApplyFallbackDda(isCorrect);
        }

        if (isCorrect)
        {
            // Every correct answer advances the next question by one digit,
            // with four digits as the maximum. At the maximum, a new four-digit
            // question is prepared.
            CreatePendingQuestion(
                answeredQuestion.spellTier,
                GetCurrentDigitLevel());
        }
        else if (GetCurrentWrongStreak() > 0)
        {
            // First wrong answer: repeat the exact same numbers next time,
            // even if the following battle action uses a different spell.
            progress.pendingQuestion = CloneQuestion(
                answeredQuestion,
                answeredQuestion.spellTier);
        }
        else
        {
            // Second consecutive wrong answer: DDA has already lowered the
            // digit level by one (minimum one digit), so prepare a new question.
            CreatePendingQuestion(
                answeredQuestion.spellTier,
                GetCurrentDigitLevel());
        }

        SyncAndSaveProgress();
        return isCorrect;
    }

    /// <summary>
    /// Commits the level using the tier and question that were actually answered
    /// to defeat the enemy. CheckAnswer may already have generated a future
    /// pending question at the next tier; that unused question must not become
    /// this level's saved completion state or the next level's inherited tier.
    /// </summary>
    public void CommitLevelCompletion()
    {
        if (levelConfig == null || progress == null || currentQuestion == null)
        {
            Debug.LogWarning(
                "QuestionManager: Tidak dapat menyimpan penyelesaian level; " +
                "konfigurasi, progress, atau soal terakhir tidak tersedia.");
            return;
        }

        QuestionData completedQuestion = NormalizeQuestion(currentQuestion);
        if (completedQuestion == null)
            return;

        int completedTier = Mathf.Clamp(completedQuestion.adaptiveLevel, 1, 4);

        // Remove the generated-but-never-played pending question from history,
        // if it is still the last history entry.
        if (progress.pendingQuestion != null &&
            progress.questionHistory != null &&
            progress.questionHistory.Count > 0)
        {
            int lastIndex = progress.questionHistory.Count - 1;
            if (AreSameQuestion(progress.questionHistory[lastIndex], progress.pendingQuestion) &&
                !AreSameQuestion(progress.pendingQuestion, completedQuestion))
            {
                progress.questionHistory.RemoveAt(lastIndex);
            }
        }

        // The completed level retains the last question that was really played.
        progress.pendingQuestion = CloneQuestion(
            completedQuestion,
            completedQuestion.spellTier,
            completedTier);
        progress.savedDigitLevel = completedTier;
        progress.savedWrongStreak = 0;
        attemptStartDigitLevel = completedTier;

        if (adaptiveDefenseManager != null)
            adaptiveDefenseManager.SetDigitLevelForReplay(completedTier);

        SyncAndSaveProgress();

        Debug.Log(
            "QuestionManager: level selesai; soal terakhir dan tier " +
            completedTier + " disimpan: " + FormatDebugQuestion(progress.pendingQuestion));
    }

    /// <summary>
    /// Retry from a defeat result starts a new attempt at the DDA tier reached
    /// when the battle ended. Unlike pause Retry, it does not replay the old
    /// attempt's entire question sequence.
    /// </summary>
    public void RestartAfterDefeat()
    {
        if (levelConfig == null || progress == null)
        {
            Debug.LogError("QuestionManager: Tidak dapat Retry setelah kalah; progress level belum tersedia.");
            return;
        }

        int defeatTier = Mathf.Clamp(GetCurrentDigitLevel(), 1, 4);

        currentQuestion = null;
        questionsShownThisAttempt.Clear();
        retryQuestionSequence.Clear();
        retryQuestionIndex = 0;
        replayingRetrySequence = false;
        attemptStartDigitLevel = defeatTier;

        // Reuse the pending question when it matches the defeat tier; otherwise
        // prepare a fresh question at that same tier. This avoids restoring the
        // tier from the beginning of the previous attempt.
        if (progress.pendingQuestion == null ||
            progress.pendingQuestion.adaptiveLevel != defeatTier ||
            !IsQuestionWithinConfiguredOperandLimit(progress.pendingQuestion))
        {
            CreatePendingQuestion(SpellTier.Basic, defeatTier);
        }

        if (questionDisplayController != null)
            questionDisplayController.HideAll();

        SyncAndSaveProgress();

        Debug.Log(
            "QuestionManager: Retry setelah kalah; percobaan baru dimulai pada DDA tier " +
            defeatTier + " dengan soal " + FormatDebugQuestion(progress.pendingQuestion));
    }

    private static bool AreSameQuestion(QuestionData a, QuestionData b)
    {
        if (a == null || b == null)
            return false;

        return a.operation == b.operation &&
               a.operandA == b.operandA &&
               a.operandB == b.operandB &&
               a.correctAnswer == b.correctAnswer &&
               a.adaptiveLevel == b.adaptiveLevel;
    }

    private void ApplyFallbackDda(bool isCorrect)
    {
        if (isCorrect)
        {
            progress.savedDigitLevel = Mathf.Min(GetMaximumDigitLevel(), GetCurrentDigitLevel() + 1);
            progress.savedWrongStreak = 0;
        }
        else
        {
            progress.savedWrongStreak++;
            if (progress.savedWrongStreak >= 2)
            {
                progress.savedWrongStreak = 0;
                progress.savedDigitLevel = Mathf.Max(1, GetCurrentDigitLevel() - 1);
            }
        }
    }

    //==================================================
    // QUESTION FACTORY: SHARED DDA LEVEL WITH OPERATION-SPECIFIC DIGIT CAP
    //==================================================

    private void CreatePendingQuestion(SpellTier spellTier, int digitLevel)
    {
        QuestionData question = CreateQuestion(spellTier, digitLevel);
        if (question == null)
        {
            Debug.LogError("QuestionManager: Gagal membuat soal untuk digit level " + digitLevel + ".");
            return;
        }

        progress.pendingQuestion = question;
        if (progress.questionHistory == null)
            progress.questionHistory = new List<QuestionData>();

        progress.questionHistory.Add(CloneQuestion(question, question.spellTier));
        SyncAndSaveProgress();
    }

    private QuestionData CreateQuestion(SpellTier spellTier, int adaptiveLevel)
    {
        adaptiveLevel = Mathf.Clamp(adaptiveLevel, 1, 4);
        DefenseDifficulty legacyDifficulty = GetLegacyDifficulty(adaptiveLevel);

        switch (levelConfig.operation)
        {
            case MathOperation.Addition:
            {
                GetNumberBounds(adaptiveLevel, out int min, out int max);
                return CreateAdditionQuestion(
                    spellTier, adaptiveLevel, legacyDifficulty, min, max);
            }

            case MathOperation.Subtraction:
            {
                GetNumberBounds(adaptiveLevel, out int min, out int max);
                return CreateSubtractionQuestion(
                    spellTier, adaptiveLevel, legacyDifficulty, min, max);
            }

            case MathOperation.Multiplication:
                return CreateMultiplicationQuestion(
                    spellTier, adaptiveLevel, legacyDifficulty);

            case MathOperation.Division:
                return CreateDivisionQuestion(
                    spellTier, adaptiveLevel, legacyDifficulty);

            default:
                Debug.LogError("QuestionManager: Operasi tidak didukung: " + levelConfig.operation);
                return null;
        }
    }

    private QuestionData CreateAdditionQuestion(
        SpellTier spellTier, int adaptiveLevel, DefenseDifficulty difficulty, int min, int max)
    {
        int operandA = min;
        int operandB = min;
        int answer = operandA + operandB;

        // UI tambah mendukung jawaban sampai 4 digit. Kedua operand mengikuti
        // jumlah digit DDA untuk operasi penjumlahan.
        for (int attempt = 0; attempt < 500; attempt++)
        {
            operandA = Random.Range(min, max + 1);
            operandB = Random.Range(min, max + 1);
            answer = operandA + operandB;
            if (answer <= 9999)
                break;
        }

        if (answer > 9999)
        {
            operandA = min;
            operandB = min;
            answer = operandA + operandB;
        }

        return new QuestionData(
            operandA, operandB, answer, MathOperation.Addition,
            spellTier, difficulty, GetMaxOperandDigitCount(operandA, operandB), adaptiveLevel);
    }

    private QuestionData CreateSubtractionQuestion(
        SpellTier spellTier, int adaptiveLevel, DefenseDifficulty difficulty, int min, int max)
    {
        int operandA = Random.Range(min, max + 1);
        int operandB = Random.Range(min, max + 1);

        if (operandA < operandB)
        {
            int temp = operandA;
            operandA = operandB;
            operandB = temp;
        }

        return new QuestionData(
            operandA, operandB, operandA - operandB, MathOperation.Subtraction,
            spellTier, difficulty, GetMaxOperandDigitCount(operandA, operandB), adaptiveLevel);
    }

    private QuestionData CreateMultiplicationQuestion(
        SpellTier spellTier, int adaptiveLevel, DefenseDifficulty difficulty)
    {
        // Kelas 3 SD: kedua faktor selalu dalam rentang 1..10.
        // DDA tetap berkembang 1..4, tetapi tingkatnya mengatur kelompok fakta
        // perkalian yang dipilih, bukan memperbesar operand menjadi 2-4 digit.
        int operandA;
        int operandB;

        int[] easyFactors = { 1, 2, 5, 10 };
        int[] coreFactors = { 3, 4, 6, 7, 8, 9 };
        int[] hardestFactors = { 6, 7, 8, 9 };

        switch (adaptiveLevel)
        {
            case 1:
                // Minimal satu faktor dari tabel yang relatif mudah.
                if (Random.value < 0.5f)
                {
                    operandA = PickRandom(easyFactors);
                    operandB = Random.Range(1, 11);
                }
                else
                {
                    operandA = Random.Range(1, 11);
                    operandB = PickRandom(easyFactors);
                }
                break;

            case 2:
                // Tingkat awal: seluruh rentang 1x1 sampai 10x10 tetap mungkin,
                // tetapi minimal satu faktor berasal dari tabel inti 3,4,6,7,8,9.
                operandA = Random.Range(1, 11);
                operandB = Random.Range(1, 11);
                if (!Contains(coreFactors, operandA) && !Contains(coreFactors, operandB))
                {
                    if (Random.value < 0.5f)
                        operandA = PickRandom(coreFactors);
                    else
                        operandB = PickRandom(coreFactors);
                }
                break;

            case 3:
                // Kedua faktor berasal dari tabel inti.
                operandA = PickRandom(coreFactors);
                operandB = PickRandom(coreFactors);
                break;

            default:
                // Tingkat tertinggi menekankan fakta perkalian yang lebih menantang.
                operandA = PickRandom(hardestFactors);
                operandB = PickRandom(hardestFactors);
                break;
        }

        int answer = operandA * operandB;
        int actualOperandDigits = GetMaxOperandDigitCount(operandA, operandB);

        return new QuestionData(
            operandA, operandB, answer, MathOperation.Multiplication,
            spellTier, difficulty, actualOperandDigits, adaptiveLevel);
    }

    private QuestionData CreateDivisionQuestion(
        SpellTier spellTier, int adaptiveLevel, DefenseDifficulty difficulty)
    {
        // Pembagian selalu 2 digit : 1 digit, hasil harus bilangan bulat.
        // Tingkat DDA memengaruhi kelompok pembagi dan rentang hasil bagi.
        int[] divisors;
        int maxQuotientByTier;

        switch (adaptiveLevel)
        {
            case 1:
                divisors = new[] { 2, 5 };
                maxQuotientByTier = 12;
                break;
            case 2:
                divisors = new[] { 2, 3, 4, 5, 6 };
                maxQuotientByTier = 20;
                break;
            case 3:
                divisors = new[] { 3, 4, 5, 6, 7, 8 };
                maxQuotientByTier = 30;
                break;
            default:
                divisors = new[] { 6, 7, 8, 9 };
                maxQuotientByTier = 40;
                break;
        }

        for (int attempt = 0; attempt < 100; attempt++)
        {
            int divisor = PickRandom(divisors);
            int minQuotient = Mathf.Max(2, Mathf.CeilToInt(10f / divisor));
            int maxQuotient = Mathf.Min(maxQuotientByTier, 99 / divisor);

            if (minQuotient > maxQuotient)
                continue;

            int quotient = Random.Range(minQuotient, maxQuotient + 1);
            int dividend = divisor * quotient;

            if (dividend < 10 || dividend > 99)
                continue;

            return new QuestionData(
                dividend, divisor, quotient, MathOperation.Division,
                spellTier, difficulty, 2, adaptiveLevel);
        }

        // Safety fallback: valid exact division for every build/configuration.
        int fallbackDivisor = 2;
        int fallbackDividend = 10;
        int fallbackQuotient = fallbackDividend / fallbackDivisor;
        return new QuestionData(
            fallbackDividend, fallbackDivisor, fallbackQuotient, MathOperation.Division,
            spellTier, difficulty, 2, adaptiveLevel);
    }

    private static int PickRandom(int[] values)
    {
        return values[Random.Range(0, values.Length)];
    }

    private static bool Contains(int[] values, int value)
    {
        for (int i = 0; i < values.Length; i++)
        {
            if (values[i] == value)
                return true;
        }

        return false;
    }

    private static int GetMaxOperandDigitCount(int operandA, int operandB)
    {
        return Mathf.Max(CountDigits(operandA), CountDigits(operandB));
    }

    private static void GetNumberBounds(int digitLevel, out int min, out int max)
    {
        digitLevel = Mathf.Clamp(digitLevel, 1, 4);
        min = digitLevel == 1 ? 1 : (int)Math.Pow(10, digitLevel - 1);
        max = (int)Math.Pow(10, digitLevel) - 1;
    }

    private static DefenseDifficulty GetLegacyDifficulty(int digitLevel)
    {
        if (digitLevel <= 2) return DefenseDifficulty.Easy;
        if (digitLevel == 3) return DefenseDifficulty.Normal;
        return DefenseDifficulty.Hard;
    }

    private int GetMaximumDigitLevel()
    {
        // DDA memiliki empat tingkat untuk seluruh operasi. Untuk kali/bagi,
        // tingkat DDA memetakan ke rentang fakta soal yang sesuai kelas 3 SD.
        return 4;
    }

    private static int GetMaximumDdaLevelForOperation(MathOperation operation)
    {
        // Signature dipertahankan agar kompatibel dengan pemanggilan lama.
        // Semua operasi kini memakai DDA level 1..4.
        return 4;
    }

    private bool IsQuestionWithinConfiguredOperandLimit(QuestionData question)
    {
        if (question == null || levelConfig == null)
            return false;

        if (question.operation != levelConfig.operation)
            return false;

        if (question.adaptiveLevel < 1 || question.adaptiveLevel > 4)
            return false;

        switch (question.operation)
        {
            case MathOperation.Addition:
            case MathOperation.Subtraction:
                // Operand tambah/kurang harus mengikuti tier digit yang tersimpan.
                return CountDigits(question.operandA) == question.adaptiveLevel &&
                       CountDigits(question.operandB) == question.adaptiveLevel &&
                       question.operandA >= 1 && question.operandB >= 1 &&
                       (question.operation != MathOperation.Addition ||
                        question.operandA + question.operandB == question.correctAnswer) &&
                       (question.operation != MathOperation.Subtraction ||
                        question.operandA - question.operandB == question.correctAnswer);

            case MathOperation.Multiplication:
                return question.operandA >= 1 && question.operandA <= 10 &&
                       question.operandB >= 1 && question.operandB <= 10 &&
                       question.operandA * question.operandB == question.correctAnswer;

            case MathOperation.Division:
                return question.operandA >= 10 && question.operandA <= 99 &&
                       question.operandB >= 1 && question.operandB <= 9 &&
                       question.operandA % question.operandB == 0 &&
                       question.operandA / question.operandB == question.correctAnswer;

            default:
                return false;
        }
    }

    private int GetCurrentDigitLevel()
    {
        int maximumDigitLevel = GetMaximumDigitLevel();

        if (adaptiveDefenseManager != null)
            return Mathf.Clamp(adaptiveDefenseManager.CurrentDigitLevel, 1, maximumDigitLevel);

        if (progress != null && progress.savedDigitLevel >= 1)
            return Mathf.Clamp(progress.savedDigitLevel, 1, maximumDigitLevel);

        return Mathf.Clamp(2, 1, maximumDigitLevel);
    }

    private int GetCurrentWrongStreak()
    {
        if (adaptiveDefenseManager != null)
            return Mathf.Max(0, adaptiveDefenseManager.WrongStreak);

        return progress != null ? Mathf.Max(0, progress.savedWrongStreak) : 0;
    }

    private void SyncAndSaveProgress()
    {
        if (progress == null)
            return;

        progress.savedDigitLevel = GetCurrentDigitLevel();
        progress.savedWrongStreak = GetCurrentWrongStreak();

        if (!string.IsNullOrEmpty(questionProgressKey))
        {
            PlayerPrefs.SetString(questionProgressKey, JsonUtility.ToJson(progress));
            PlayerPrefs.Save();
        }
    }

    private void SaveProgress()
    {
        SyncAndSaveProgress();
    }

    private static string BuildQuestionProgressKey(MathOperation operation, int levelNumber)
    {
        return "ReMath_QuestionProgress_" + operation + "_Level_" + Mathf.Max(1, levelNumber);
    }

    private static QuestionData NormalizeQuestion(QuestionData question)
    {
        if (question == null)
            return null;

        // Older saved QuestionData does not have adaptiveLevel. In that case,
        // use the legacy operandDigitCount as the best available DDA tier.
        int adaptiveLevel = question.adaptiveLevel;
        if (adaptiveLevel < 1 || adaptiveLevel > 4)
        {
            adaptiveLevel = question.operandDigitCount >= 1 && question.operandDigitCount <= 4
                ? question.operandDigitCount
                : Mathf.Max(CountDigits(question.operandA), CountDigits(question.operandB));
        }

        return CloneQuestion(question, question.spellTier, adaptiveLevel);
    }

    private static QuestionData CloneQuestion(
        QuestionData question, SpellTier spellTier, int adaptiveLevelOverride = 0)
    {
        if (question == null)
            return null;

        int adaptiveLevel = adaptiveLevelOverride >= 1 && adaptiveLevelOverride <= 4
            ? adaptiveLevelOverride
            : question.adaptiveLevel;

        if (adaptiveLevel < 1 || adaptiveLevel > 4)
        {
            adaptiveLevel = question.operandDigitCount >= 1 && question.operandDigitCount <= 4
                ? question.operandDigitCount
                : Mathf.Max(CountDigits(question.operandA), CountDigits(question.operandB));
        }

        int actualOperandDigits = GetMaxOperandDigitCount(
            question.operandA, question.operandB);

        return new QuestionData(
            question.operandA,
            question.operandB,
            question.correctAnswer,
            question.operation,
            spellTier,
            question.defenseDifficulty,
            actualOperandDigits,
            adaptiveLevel);
    }

    private static int CountDigits(int value)
    {
        long safeValue = Math.Abs((long)value);
        return safeValue == 0 ? 1 : safeValue.ToString().Length;
    }

    //==================================================
    // RETRY SAME QUESTION SEQUENCE
    //==================================================

    public void RestartSessionAttempt()
    {
        retryQuestionSequence.Clear();
        for (int i = 0; i < questionsShownThisAttempt.Count; i++)
        {
            QuestionData copy = CloneQuestion(
                questionsShownThisAttempt[i],
                questionsShownThisAttempt[i].spellTier);
            if (copy != null)
                retryQuestionSequence.Add(copy);
        }

        if (retryQuestionSequence.Count == 0 && progress != null && progress.pendingQuestion != null)
        {
            retryQuestionSequence.Add(CloneQuestion(
                progress.pendingQuestion,
                progress.pendingQuestion.spellTier));
        }

        retryQuestionIndex = 0;
        replayingRetrySequence = retryQuestionSequence.Count > 0;
        questionsShownThisAttempt.Clear();
        currentQuestion = null;

        if (adaptiveDefenseManager != null)
            adaptiveDefenseManager.SetDigitLevelForReplay(attemptStartDigitLevel);
        else if (progress != null)
            progress.savedDigitLevel = attemptStartDigitLevel;

        if (replayingRetrySequence && progress != null)
        {
            progress.pendingQuestion = CloneQuestion(
                retryQuestionSequence[0],
                retryQuestionSequence[0].spellTier);
        }

        if (questionDisplayController != null)
            questionDisplayController.HideAll();

        SyncAndSaveProgress();
    }

    //==================================================
    // CLEAR CURRENT QUESTION
    //==================================================

    public void ClearCurrentQuestion()
    {
        currentQuestion = null;

        if (questionDisplayController != null)
            questionDisplayController.HideAll();
    }

    //==================================================
    // DEBUG HELPERS
    //==================================================

    private static string FormatDebugQuestion(QuestionData question)
    {
        if (question == null)
            return "<no question>";

        string operationSymbol;
        switch (question.operation)
        {
            case MathOperation.Addition: operationSymbol = "+"; break;
            case MathOperation.Subtraction: operationSymbol = "-"; break;
            case MathOperation.Multiplication: operationSymbol = "×"; break;
            case MathOperation.Division: operationSymbol = "÷"; break;
            default: operationSymbol = "?"; break;
        }

        return question.operandA + " " + operationSymbol + " " + question.operandB +
               " = ? | digit operand=" + question.operandDigitCount +
               " | DDA level=" + question.adaptiveLevel +
               " | spell=" + question.spellTier;
    }
}
