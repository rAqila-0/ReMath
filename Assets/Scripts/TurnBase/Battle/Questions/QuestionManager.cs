using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionManager : MonoBehaviour
{
    //==================================================
    // REFERENCES
    //==================================================

    [Header("References")]

    [SerializeField]
    private BattleLevelConfig levelConfig;

    [SerializeField]
    private BattleUIManager battleUI;


    //==================================================
    // QUESTION SESSION
    //==================================================

    private QuestionSession questionSession =
        new QuestionSession();


    //==================================================
    // CURRENT QUESTION
    //==================================================

    private int currentOperandA;
    private int currentOperandB;
    private int currentCorrectAnswer;

    private MathOperation currentOperation;
    private SpellTier currentSpellTier;

    private bool hasCurrentQuestion = false;


    //==================================================
    // PROPERTIES
    //==================================================

    public int CurrentOperandA =>
        currentOperandA;

    public int CurrentOperandB =>
        currentOperandB;

    public int CurrentCorrectAnswer =>
        currentCorrectAnswer;

    public MathOperation CurrentOperation =>
        currentOperation;

    public SpellTier CurrentSpellTier =>
        currentSpellTier;

    public bool HasCurrentQuestion =>
        hasCurrentQuestion;


    //==================================================
    // INITIALIZE
    //==================================================

    public void Initialize(
        BattleLevelConfig config
    )
    {
        if (config == null)
        {
            Debug.LogError(
                "QuestionManager: " +
                "BattleLevelConfig tidak boleh null."
            );

            return;
        }


        levelConfig = config;


        // Scene / level baru
        // = session soal baru.
        StartNewSession();
    }


    //==================================================
    // NEW SESSION
    //==================================================

    public void StartNewSession()
    {
        questionSession.StartNewSession();

        ClearCurrentQuestion();


        Debug.Log(
            "Question Session baru dibuat."
        );
    }


    //==================================================
    // RETRY SESSION
    //==================================================

    public void RestartSessionAttempt()
    {
        // History soal TIDAK dihapus.
        // Hanya index dikembalikan ke awal.
        questionSession.RestartAttempt();

        ClearCurrentQuestion();


        Debug.Log(
            "Question Session diulang. " +
            "Soal lama akan digunakan kembali."
        );
    }


    //==================================================
    // GENERATE QUESTION
    //==================================================

    public bool GenerateQuestion(
        SpellTier spellTier
    )
    {
        if (levelConfig == null)
        {
            Debug.LogError(
                "QuestionManager: " +
                "Level Config belum diisi."
            );

            return false;
        }


        //==================================================
        // CEK APAKAH ADA SOAL LAMA
        //==================================================

        if (
            questionSession.TryGetRecordedQuestion(
                spellTier,
                out BattleQuestion recordedQuestion
            )
        )
        {
            ApplyQuestion(
                recordedQuestion
            );


            Debug.Log(
                $"Mengulang soal [{spellTier}] : " +
                $"{currentOperandA} " +
                $"{GetOperatorSymbol(currentOperation)} " +
                $"{currentOperandB}"
            );


            return true;
        }


        //==================================================
        // TIDAK ADA SOAL LAMA
        // → BUAT SOAL BARU
        //==================================================

        QuestionRange range =
            GetQuestionRange(
                spellTier
            );


        if (range == null)
        {
            Debug.LogError(
                $"QuestionManager: Range untuk " +
                $"{spellTier} tidak ditemukan."
            );

            return false;
        }


        BattleQuestion newQuestion =
            GenerateNewQuestion(
                spellTier,
                range
            );


        if (newQuestion == null)
        {
            Debug.LogError(
                "QuestionManager: " +
                "Gagal membuat soal."
            );

            return false;
        }


        // Simpan ke session.
        questionSession.RecordNewQuestion(
            newQuestion
        );


        ApplyQuestion(
            newQuestion
        );


        Debug.Log(
            $"Soal BARU [{spellTier}] : " +
            $"{currentOperandA} " +
            $"{GetOperatorSymbol(currentOperation)} " +
            $"{currentOperandB} = " +
            $"{currentCorrectAnswer}"
        );


        return true;
    }


    //==================================================
    // GENERATE NEW QUESTION
    //==================================================

    private BattleQuestion GenerateNewQuestion(
        SpellTier spellTier,
        QuestionRange range
    )
    {
        MathOperation operation =
            levelConfig.operation;


        switch (operation)
        {
            case MathOperation.Addition:

                return GenerateAddition(
                    spellTier,
                    range
                );


            case MathOperation.Subtraction:

                return GenerateSubtraction(
                    spellTier,
                    range
                );


            case MathOperation.Multiplication:

                return GenerateMultiplication(
                    spellTier,
                    range
                );


            case MathOperation.Division:

                return GenerateDivision(
                    spellTier,
                    range
                );


            default:

                return null;
        }
    }


    //==================================================
    // ADDITION
    //==================================================

    private BattleQuestion GenerateAddition(
        SpellTier spellTier,
        QuestionRange range
    )
    {
        int a =
            GetRandomValue(
                range.minA,
                range.maxA
            );

        int b =
            GetRandomValue(
                range.minB,
                range.maxB
            );


        return new BattleQuestion(
            a,
            b,
            a + b,
            MathOperation.Addition,
            spellTier
        );
    }


    //==================================================
    // SUBTRACTION
    //==================================================

    private BattleQuestion GenerateSubtraction(
        SpellTier spellTier,
        QuestionRange range
    )
    {
        int a =
            GetRandomValue(
                range.minA,
                range.maxA
            );

        int b =
            GetRandomValue(
                range.minB,
                range.maxB
            );


        // Cegah hasil negatif.
        if (a < b)
        {
            int temporary = a;

            a = b;
            b = temporary;
        }


        return new BattleQuestion(
            a,
            b,
            a - b,
            MathOperation.Subtraction,
            spellTier
        );
    }


    //==================================================
    // MULTIPLICATION
    //==================================================

    private BattleQuestion GenerateMultiplication(
        SpellTier spellTier,
        QuestionRange range
    )
    {
        int a =
            GetRandomValue(
                range.minA,
                range.maxA
            );

        int b =
            GetRandomValue(
                range.minB,
                range.maxB
            );


        return new BattleQuestion(
            a,
            b,
            a * b,
            MathOperation.Multiplication,
            spellTier
        );
    }


    //==================================================
    // DIVISION
    //==================================================

    private BattleQuestion GenerateDivision(
        SpellTier spellTier,
        QuestionRange range
    )
    {
        // Range A = hasil bagi
        // Range B = pembagi.

        int quotient =
            GetRandomValue(
                range.minA,
                range.maxA
            );


        int divisor =
            GetRandomValue(
                Mathf.Max(1, range.minB),
                Mathf.Max(1, range.maxB)
            );


        int dividend =
            quotient *
            divisor;


        return new BattleQuestion(
            dividend,
            divisor,
            quotient,
            MathOperation.Division,
            spellTier
        );
    }


    //==================================================
    // APPLY QUESTION
    //==================================================

    private void ApplyQuestion(
        BattleQuestion question
    )
    {
        if (question == null)
            return;


        currentOperandA =
            question.operandA;

        currentOperandB =
            question.operandB;

        currentCorrectAnswer =
            question.correctAnswer;

        currentOperation =
            question.operation;

        currentSpellTier =
            question.spellTier;


        hasCurrentQuestion = true;


        // Tampilkan ke UI.
        if (battleUI != null)
        {
            battleUI.SetVerticalQuestion(
                currentOperandA,
                currentOperandB,
                currentOperation
            );
        }
    }


    //==================================================
    // CHECK ANSWER
    //==================================================

    public bool CheckAnswer(
        int playerAnswer
    )
    {
        if (!hasCurrentQuestion)
        {
            Debug.LogWarning(
                "QuestionManager: " +
                "Belum ada soal aktif."
            );

            return false;
        }


        bool isCorrect =
            playerAnswer ==
            currentCorrectAnswer;


        Debug.Log(
            isCorrect
                ? "Jawaban BENAR."
                : $"Jawaban SALAH. " +
                  $"Jawaban benar: " +
                  $"{currentCorrectAnswer}"
        );


        return isCorrect;
    }


    //==================================================
    // GET RANGE
    //==================================================

    private QuestionRange GetQuestionRange(
        SpellTier spellTier
    )
    {
        switch (spellTier)
        {
            case SpellTier.Basic:

                return levelConfig.basicRange;


            case SpellTier.Medium:

                return levelConfig.mediumRange;


            case SpellTier.Advanced:

                return levelConfig.advancedRange;


            case SpellTier.Defense:

                return levelConfig.defenseRange;


            default:

                return null;
        }
    }


    //==================================================
    // RANDOM
    //==================================================

    private int GetRandomValue(
        int minimum,
        int maximum
    )
    {
        if (minimum > maximum)
        {
            int temporary =
                minimum;

            minimum =
                maximum;

            maximum =
                temporary;
        }


        return Random.Range(
            minimum,
            maximum + 1
        );
    }


    //==================================================
    // CLEAR CURRENT QUESTION
    //==================================================

    public void ClearCurrentQuestion()
    {
        currentOperandA = 0;
        currentOperandB = 0;
        currentCorrectAnswer = 0;

        hasCurrentQuestion = false;
    }


    //==================================================
    // OPERATOR
    //==================================================

    private string GetOperatorSymbol(
        MathOperation operation
    )
    {
        switch (operation)
        {
            case MathOperation.Addition:
                return "+";

            case MathOperation.Subtraction:
                return "-";

            case MathOperation.Multiplication:
                return "×";

            case MathOperation.Division:
                return "÷";

            default:
                return "";
        }
    }


    //==================================================
    // DEBUG SESSION
    //==================================================

    [ContextMenu("DEBUG - Session Info")]
    private void DebugSessionInfo()
    {
        Debug.Log(
            "QUESTION SESSION\n" +
            $"Basic: " +
            $"{questionSession.GetRecordedCount(SpellTier.Basic)}\n" +
            $"Medium: " +
            $"{questionSession.GetRecordedCount(SpellTier.Medium)}\n" +
            $"Advanced: " +
            $"{questionSession.GetRecordedCount(SpellTier.Advanced)}\n" +
            $"Defense: " +
            $"{questionSession.GetRecordedCount(SpellTier.Defense)}"
        );
    }
}