using System;

[Serializable]
public class QuestionData
{
    public MathOperation operation;
    public SpellTier spellTier;

    // Actual number of digits used by the operands. This is not the DDA tier.
    public int operandDigitCount;

    // Shared DDA tier (1..4) for every attack spell and defense spell.
    // For addition/subtraction it normally matches operand digit count.
    // For multiplication/division it maps to operation-specific fact difficulty.
    public int adaptiveLevel;

    public int operandA;
    public int operandB;
    public int correctAnswer;

    // Legacy field kept for compatibility with existing display/UI scripts.
    public DefenseDifficulty defenseDifficulty;

    // Four answer slots are enough: 10 x 10 = 100, and 99 / 2 = 49 max.
    public const int MaxAnswerDigits = 4;

    public QuestionData(
        int operandA,
        int operandB,
        int correctAnswer,
        MathOperation operation,
        SpellTier spellTier,
        DefenseDifficulty defenseDifficulty = DefenseDifficulty.Normal,
        int operandDigitCount = 0,
        int adaptiveLevel = 0)
    {
        this.operandA = operandA;
        this.operandB = operandB;
        this.correctAnswer = correctAnswer;
        this.operation = operation;
        this.spellTier = spellTier;
        this.defenseDifficulty = defenseDifficulty;

        int actualDigitCount = Math.Max(
            GetDigitCount(operandA), GetDigitCount(operandB));

        this.operandDigitCount = actualDigitCount;

        this.adaptiveLevel = adaptiveLevel >= 1 && adaptiveLevel <= 4
            ? adaptiveLevel
            : (operandDigitCount >= 1 && operandDigitCount <= 4
                ? operandDigitCount
                : actualDigitCount);
    }

    public bool IsDefense => spellTier == SpellTier.Defense;
    public int AnswerDigitCount => GetDigitCount(correctAnswer);

    private static int GetDigitCount(int value)
    {
        long safeValue = Math.Abs((long)value);
        return safeValue == 0 ? 1 : safeValue.ToString().Length;
    }
}
