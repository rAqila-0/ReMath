using System;

[Serializable]
public class QuestionData
{
    //==================================================
    // GENERAL QUESTION DATA
    //==================================================

    public MathOperation operation;

    public SpellTier spellTier;


    //==================================================
    // OPERANDS
    //==================================================

    public int operandA;

    public int operandB;


    //==================================================
    // ANSWER
    //==================================================

    public int correctAnswer;


    //==================================================
    // DEFENSE DIFFICULTY
    //==================================================

    public DefenseDifficulty defenseDifficulty;


    //==================================================
    // CONSTANT
    //==================================================

    public const int MaxAnswerDigits = 4;


    //==================================================
    // CONSTRUCTOR
    //==================================================

    public QuestionData(
        int operandA,
        int operandB,
        int correctAnswer,
        MathOperation operation,
        SpellTier spellTier,
        DefenseDifficulty defenseDifficulty =
            DefenseDifficulty.Normal
    )
    {
        this.operandA =
            operandA;

        this.operandB =
            operandB;

        this.correctAnswer =
            correctAnswer;

        this.operation =
            operation;

        this.spellTier =
            spellTier;

        this.defenseDifficulty =
            defenseDifficulty;
    }


    //==================================================
    // PROPERTIES
    //==================================================

    public bool IsDefense =>
        spellTier == SpellTier.Defense;


    public int AnswerDigitCount
    {
        get
        {
            int value =
                Math.Abs(correctAnswer);

            if (value == 0)
                return 1;

            return value
                .ToString()
                .Length;
        }
    }
}