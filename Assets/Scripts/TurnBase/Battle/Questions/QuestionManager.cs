using UnityEngine;

public class QuestionManager : MonoBehaviour
{
    //==================================================
    // REFERENCES
    //==================================================

    [Header("References")]

    [SerializeField]
    private QuestionDisplayController
        questionDisplayController;

    [SerializeField]
    private AdaptiveDefenseManager
        adaptiveDefenseManager;


    //==================================================
    // RUNTIME
    //==================================================

    private BattleLevelConfig levelConfig;


    private readonly QuestionSession
        questionSession =
            new QuestionSession();


    private QuestionData
        currentQuestion;


    //==================================================
    // PROPERTIES
    //==================================================

    public QuestionData CurrentQuestion =>
        currentQuestion;


    public bool HasActiveQuestion =>
        currentQuestion != null;


    //==================================================
    // INITIALIZE
    //==================================================

    public void Initialize(
        BattleLevelConfig config
    )
    {
        levelConfig =
            config;


        StartNewSession();


        ClearCurrentQuestion();
    }


    //==================================================
    // NEW SESSION
    //==================================================

    public void StartNewSession()
    {
        questionSession
            .StartNewSession();
    }


    //==================================================
    // RETRY SAME SESSION
    //==================================================

    public void RestartSessionAttempt()
    {
        questionSession
            .RestartAttempt();


        ClearCurrentQuestion();
    }


    //==================================================
    // GENERATE QUESTION
    //==================================================

    public QuestionData GenerateQuestion(
        SpellTier spellTier
    )
    {
        if (levelConfig == null)
        {
            Debug.LogError(
                "QuestionManager: " +
                "BattleLevelConfig belum diinisialisasi."
            );

            return null;
        }


        //==============================================
        // REUSE QUESTION HISTORY
        //==============================================

        if (
            questionSession
                .TryGetRecordedQuestion(
                    spellTier,
                    out QuestionData
                        recordedQuestion
                )
        )
        {
            if (
                ApplyQuestion(
                    recordedQuestion
                )
            )
            {
                Debug.Log(
                    "QuestionManager: " +
                    "Menggunakan soal dari session history."
                );


                return recordedQuestion;
            }


            return null;
        }


        //==============================================
        // CREATE NEW QUESTION
        //==============================================

        QuestionData newQuestion =
            CreateQuestion(
                spellTier
            );


        if (newQuestion == null)
        {
            Debug.LogError(
                "QuestionManager: " +
                "Gagal membuat soal."
            );

            return null;
        }


        //==============================================
        // DISPLAY
        //==============================================

        if (
            !ApplyQuestion(
                newQuestion
            )
        )
        {
            return null;
        }


        //==============================================
        // RECORD HISTORY
        //==============================================

        questionSession
            .RecordNewQuestion(
                newQuestion
            );


        Debug.Log(
            "QuestionManager: " +
            FormatDebugQuestion(
                newQuestion
            )
        );


        return newQuestion;
    }


    //==================================================
    // CREATE QUESTION
    //==================================================

    private QuestionData CreateQuestion(
        SpellTier spellTier
    )
    {
        DefenseDifficulty
            defenseDifficulty =
                DefenseDifficulty.Normal;


        if (
            spellTier ==
            SpellTier.Defense
        )
        {
            if (
                adaptiveDefenseManager !=
                null
            )
            {
                defenseDifficulty =
                    adaptiveDefenseManager
                        .CurrentDifficulty;
            }
        }


        switch (
            levelConfig.operation
        )
        {
            case MathOperation.Addition:

                return CreateAdditionQuestion(
                    spellTier,
                    defenseDifficulty
                );


            case MathOperation.Subtraction:

                return CreateSubtractionQuestion(
                    spellTier,
                    defenseDifficulty
                );


            case MathOperation.Multiplication:

                return CreateMultiplicationQuestion(
                    spellTier,
                    defenseDifficulty
                );


            case MathOperation.Division:

                return CreateDivisionQuestion(
                    spellTier,
                    defenseDifficulty
                );


            default:

                return null;
        }
    }


    //==================================================
    // ADDITION
    //==================================================

    private QuestionData CreateAdditionQuestion(
        SpellTier spellTier,
        DefenseDifficulty difficulty
    )
    {
        GetAddSubRange(
            spellTier,
            difficulty,
            true,
            out int minA,
            out int maxA,
            out int minB,
            out int maxB
        );


        int operandA =
            Random.Range(
                minA,
                maxA + 1
            );


        int operandB =
            Random.Range(
                minB,
                maxB + 1
            );


        int answer =
            operandA +
            operandB;


        //==================================================
        // JANGAN IZINKAN HASIL 5 DIGIT
        //==================================================

        int safety = 0;


        while (
            answer > 9999 &&
            safety < 100
        )
        {
            operandA =
                Random.Range(
                    minA,
                    maxA + 1
                );


            operandB =
                Random.Range(
                    minB,
                    maxB + 1
                );


            answer =
                operandA +
                operandB;


            safety++;
        }


        //==================================================
        // FALLBACK
        //==================================================

        if (answer > 9999)
        {
            Debug.LogWarning(
                "QuestionManager: " +
                "Range Addition menghasilkan 5 digit. " +
                "Menggunakan fallback aman."
            );


            operandA = 4000;

            operandB = 3000;

            answer = 7000;
        }


        return new QuestionData(
            operandA,
            operandB,
            answer,
            MathOperation.Addition,
            spellTier,
            difficulty
        );
    }

    //==================================================
    // SUBTRACTION
    //==================================================

    private QuestionData CreateSubtractionQuestion(
        SpellTier spellTier,
        DefenseDifficulty difficulty
    )
    {
        GetAddSubRange(
            spellTier,
            difficulty,
            false,
            out int minA,
            out int maxA,
            out int minB,
            out int maxB
        );


        int operandA =
            Random.Range(
                minA,
                maxA + 1
            );


        int operandB =
            Random.Range(
                minB,
                maxB + 1
            );


        //==================================================
        // TIDAK BOLEH NEGATIF
        //==================================================

        if (operandA < operandB)
        {
            int temp =
                operandA;

            operandA =
                operandB;

            operandB =
                temp;
        }


        int answer =
            operandA -
            operandB;


        return new QuestionData(
            operandA,
            operandB,
            answer,
            MathOperation.Subtraction,
            spellTier,
            difficulty
        );
    }

    //==================================================
    // ADD / SUB RANGE
    //==================================================

    private void GetAddSubRange(
        SpellTier spellTier,
        DefenseDifficulty difficulty,
        bool isAddition,
        out int minA,
        out int maxA,
        out int minB,
        out int maxB
    )
    {
        //==================================================
        // DEFENSE = ADAPTIVE
        //==================================================

        if (
            spellTier ==
            SpellTier.Defense
        )
        {
            switch (difficulty)
            {
                //==========================================
                // EASY = 2 DIGIT
                //==========================================

                case DefenseDifficulty.Easy:

                    minA = 10;
                    maxA = 99;

                    minB = 10;
                    maxB = 99;

                    return;


                //==========================================
                // NORMAL = 3 DIGIT
                //==========================================

                case DefenseDifficulty.Normal:

                    minA = 100;
                    maxA = 999;

                    minB = 100;
                    maxB = 999;

                    return;


                //==========================================
                // HARD = 4 DIGIT
                //==========================================

                case DefenseDifficulty.Hard:

                    minA = 1000;

                    maxA =
                        isAddition
                            ? 4999
                            : 9999;


                    minB = 1000;

                    maxB =
                        isAddition
                            ? 4999
                            : 9999;

                    return;
            }
        }


        //==================================================
        // ATTACK = BACA BATTLE LEVEL CONFIG
        //==================================================

        QuestionRange range =
            GetAttackQuestionRange(
                spellTier
            );


        if (range != null)
        {
            minA =
                range.minA;

            maxA =
                range.maxA;

            minB =
                range.minB;

            maxB =
                range.maxB;


            return;
        }


        //==================================================
        // FALLBACK
        //==================================================

        Debug.LogWarning(
            "QuestionManager: " +
            "Attack Question Range tidak ditemukan. " +
            "Menggunakan range Basic."
        );


        minA = 10;
        maxA = 99;

        minB = 10;
        maxB = 99;
    }

    //==================================================
    // GET ATTACK QUESTION RANGE
    //==================================================

    private QuestionRange GetAttackQuestionRange(
        SpellTier spellTier
    )
    {
        if (levelConfig == null)
        {
            return null;
        }


        switch (spellTier)
        {
            case SpellTier.Basic:

                return levelConfig
                    .basicRange;


            case SpellTier.Medium:

                return levelConfig
                    .mediumRange;


            case SpellTier.Advanced:

                return levelConfig
                    .advancedRange;


            default:

                return null;
        }
    }

    //==================================================
    // MULTIPLICATION
    //==================================================

    private QuestionData CreateMultiplicationQuestion(
        SpellTier spellTier,
        DefenseDifficulty difficulty
    )
    {
        int[] allowedTables =
            GetAllowedTables(
                spellTier,
                difficulty
            );


        int table =
            allowedTables[
                Random.Range(
                    0,
                    allowedTables.Length
                )
            ];


        //==================================================
        // MULTIPLIER
        //==================================================

        int maxMultiplier = 10;


        // Attack mengikuti progression level.
        if (
            spellTier != SpellTier.Defense &&
            levelConfig != null
        )
        {
            maxMultiplier =
                Mathf.Clamp(
                    levelConfig.maxFactMultiplier,
                    1,
                    10
                );
        }


        // Defense TIDAK mengikuti level.
        // Defense mengikuti Easy / Normal / Hard.
        int multiplier =
            Random.Range(
                1,
                maxMultiplier + 1
            );


        int operandA =
            table;


        int operandB =
            multiplier;


        int answer =
            operandA *
            operandB;


        return new QuestionData(
            operandA,
            operandB,
            answer,
            MathOperation.Multiplication,
            spellTier,
            difficulty
        );
    }

    //==================================================
    // DIVISION
    //==================================================

    private QuestionData CreateDivisionQuestion(
        SpellTier spellTier,
        DefenseDifficulty difficulty
    )
    {
        //==================================================
        // PILIH PEMBAGI
        //==================================================

        int[] allowedTables =
            GetAllowedTables(
                spellTier,
                difficulty
            );


        int divisor =
            allowedTables[
                Random.Range(
                    0,
                    allowedTables.Length
                )
            ];


        //==================================================
        // QUOTIENT / HASIL BAGI
        //==================================================

        int maxQuotient = 10;


        // Attack mengikuti progression level.
        if (
            spellTier != SpellTier.Defense &&
            levelConfig != null
        )
        {
            maxQuotient =
                Mathf.Clamp(
                    levelConfig.maxFactMultiplier,
                    1,
                    10
                );
        }


        // Defense tetap 1-10 karena
        // kesulitannya dikontrol DDA.
        int quotient =
            Random.Range(
                1,
                maxQuotient + 1
            );


        //==================================================
        // DIVIDEND
        //
        // divisor × quotient
        //
        // Dengan cara ini pembagian
        // selalu habis tanpa sisa.
        //==================================================

        int dividend =
            divisor *
            quotient;


        //==================================================
        // QUESTION DATA
        //
        // operandA = dividend
        // operandB = divisor
        // answer   = quotient
        //==================================================

        return new QuestionData(
            dividend,
            divisor,
            quotient,
            MathOperation.Division,
            spellTier,
            difficulty
        );
    }

    //==================================================
    // TABLE DIFFICULTY
    //==================================================

    private int[] GetAllowedTables(
        SpellTier spellTier,
        DefenseDifficulty difficulty
    )
    {
        //==============================================
        // DEFENSE
        //==============================================

        if (
            spellTier ==
            SpellTier.Defense
        )
        {
            switch (difficulty)
            {
                case DefenseDifficulty.Easy:

                    return new int[]
                    {
                        1,
                        2,
                        5,
                        10
                    };


                case DefenseDifficulty.Normal:

                    return new int[]
                    {
                        3,
                        4,
                        6
                    };


                case DefenseDifficulty.Hard:

                    return new int[]
                    {
                        7,
                        8,
                        9
                    };
            }
        }


        //==============================================
        // ATTACK
        //==============================================

        switch (spellTier)
        {
            case SpellTier.Basic:

                return new int[]
                {
                    1,
                    2,
                    5,
                    10
                };


            case SpellTier.Medium:

                return new int[]
                {
                    3,
                    4,
                    6
                };


            case SpellTier.Advanced:

                return new int[]
                {
                    7,
                    8,
                    9
                };


            default:

                return new int[]
                {
                    1,
                    2,
                    5,
                    10
                };
        }
    }


    //==================================================
    // APPLY QUESTION
    //==================================================

    private bool ApplyQuestion(
        QuestionData question
    )
    {
        if (question == null)
            return false;


        if (
            questionDisplayController ==
            null
        )
        {
            Debug.LogError(
                "QuestionManager: " +
                "QuestionDisplayController belum diisi."
            );

            return false;
        }


        bool displayed =
            questionDisplayController
                .DisplayQuestion(
                    question
                );


        if (!displayed)
        {
            return false;
        }


        currentQuestion =
            question;


        return true;
    }


    //==================================================
    // CHECK ANSWER
    //==================================================

    public bool CheckAnswer(
        int playerAnswer
    )
    {
        if (currentQuestion == null)
        {
            Debug.LogWarning(
                "QuestionManager: " +
                "Tidak ada soal aktif."
            );

            return false;
        }


        return
            playerAnswer ==
            currentQuestion.correctAnswer;
    }


    //==================================================
    // CLEAR QUESTION
    //==================================================

    public void ClearCurrentQuestion()
    {
        currentQuestion =
            null;


        if (
            questionDisplayController !=
            null
        )
        {
            questionDisplayController
                .HideAll();
        }
    }


    //==================================================
    // DEBUG
    //==================================================

    private string FormatDebugQuestion(
        QuestionData question
    )
    {
        string operationSymbol =
            "?";


        switch (question.operation)
        {
            case MathOperation.Addition:

                operationSymbol = "+";

                break;


            case MathOperation.Subtraction:

                operationSymbol = "-";

                break;


            case MathOperation.Multiplication:

                operationSymbol = "×";

                break;


            case MathOperation.Division:

                operationSymbol = "÷";

                break;
        }


        return
            question.operandA +
            " " +
            operationSymbol +
            " " +
            question.operandB +
            " = " +
            question.correctAnswer +
            " | Spell = " +
            question.spellTier +
            " | Defense Difficulty = " +
            question.defenseDifficulty;
    }
}