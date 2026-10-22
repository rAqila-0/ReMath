using TMPro;
using UnityEngine;

public class MultiplicationRenderer : MonoBehaviour
{
    //==================================================
    // REFERENCES
    //==================================================

    [Header("Multiplication Display")]

    [SerializeField]
    private TMP_Text operandAText;

    [SerializeField]
    private TMP_Text operatorText;

    [SerializeField]
    private TMP_Text operandBText;


    //==================================================
    // CURRENT QUESTION
    //==================================================

    private QuestionData currentQuestion;


    //==================================================
    // PROPERTY
    //==================================================

    public QuestionData CurrentQuestion =>
        currentQuestion;


    //==================================================
    // RENDER
    //==================================================

    public bool Render(
        QuestionData question
    )
    {
        //==============================================
        // VALIDATE QUESTION
        //==============================================

        if (question == null)
        {
            Debug.LogError(
                "MultiplicationRenderer: " +
                "QuestionData null."
            );

            return false;
        }


        if (
            question.operation !=
            MathOperation.Multiplication
        )
        {
            Debug.LogError(
                "MultiplicationRenderer hanya " +
                "untuk operasi Multiplication."
            );

            return false;
        }


        //==============================================
        // VALIDATE RANGE 1 - 10
        //==============================================

        if (
            question.operandA < 1 ||
            question.operandA > 10 ||
            question.operandB < 1 ||
            question.operandB > 10
        )
        {
            Debug.LogError(
                "MultiplicationRenderer: " +
                "Operand perkalian harus " +
                "antara 1 sampai 10."
            );

            return false;
        }


        //==============================================
        // VALIDATE ANSWER
        //==============================================

        int expectedAnswer =
            question.operandA *
            question.operandB;


        if (
            question.correctAnswer !=
            expectedAnswer
        )
        {
            Debug.LogWarning(
                "MultiplicationRenderer: " +
                "CorrectAnswer QuestionData " +
                "tidak sesuai hasil perkalian."
            );
        }


        currentQuestion =
            question;


        //==============================================
        // DISPLAY
        //==============================================

        if (operandAText != null)
        {
            operandAText.text =
                question.operandA.ToString();
        }


        if (operatorText != null)
        {
            operatorText.text =
                "×";
        }


        if (operandBText != null)
        {
            operandBText.text =
                question.operandB.ToString();
        }


        return true;
    }


    //==================================================
    // CLEAR
    //==================================================

    public void ClearDisplay()
    {
        if (operandAText != null)
        {
            operandAText.text = "";
        }


        if (operatorText != null)
        {
            operatorText.text = "";
        }


        if (operandBText != null)
        {
            operandBText.text = "";
        }


        currentQuestion =
            null;
    }
}