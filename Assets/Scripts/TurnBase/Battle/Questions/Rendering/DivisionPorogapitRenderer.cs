using TMPro;
using UnityEngine;

public class DivisionPorogapitRenderer : MonoBehaviour
{
    //==================================================
    // REFERENCES
    //==================================================

    [Header("Porogapit Display")]

    [SerializeField]
    private TMP_Text divisorText;

    [SerializeField]
    private TMP_Text dividendText;

    [SerializeField]
    private TMP_Text bracketText;


    //==================================================
    // OPTIONAL WORK AREA
    //==================================================

    [Header("Optional Work Area")]

    [SerializeField]
    private GameObject divisionWorkArea;


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
        // VALIDATE
        //==============================================

        if (question == null)
        {
            Debug.LogError(
                "DivisionPorogapitRenderer: " +
                "QuestionData null."
            );

            return false;
        }


        if (
            question.operation !=
            MathOperation.Division
        )
        {
            Debug.LogError(
                "DivisionPorogapitRenderer hanya " +
                "untuk operasi Division."
            );

            return false;
        }


        if (question.operandB == 0)
        {
            Debug.LogError(
                "DivisionPorogapitRenderer: " +
                "Pembagi tidak boleh 0."
            );

            return false;
        }


        if (
            question.operandA < 0 ||
            question.operandB < 1
        )
        {
            Debug.LogError(
                "DivisionPorogapitRenderer: " +
                "Operand pembagian tidak valid."
            );

            return false;
        }


        //==============================================
        // VALIDATE EXACT DIVISION
        //==============================================

        if (
            question.operandA %
            question.operandB != 0
        )
        {
            Debug.LogError(
                "DivisionPorogapitRenderer: " +
                "Soal pembagian harus habis dibagi."
            );

            return false;
        }


        int expectedAnswer =
            question.operandA /
            question.operandB;


        if (
            expectedAnswer !=
            question.correctAnswer
        )
        {
            Debug.LogWarning(
                "DivisionPorogapitRenderer: " +
                "CorrectAnswer tidak sesuai " +
                "hasil pembagian."
            );
        }


        if (
            expectedAnswer >
            9999
        )
        {
            Debug.LogError(
                "DivisionPorogapitRenderer: " +
                "Jawaban melebihi 4 digit."
            );

            return false;
        }


        //==============================================
        // SAVE
        //==============================================

        currentQuestion =
            question;


        //==============================================
        // DISPLAY
        //==============================================

        if (dividendText != null)
        {
            dividendText.text =
                question.operandA
                    .ToString();
        }


        if (divisorText != null)
        {
            divisorText.text =
                question.operandB
                    .ToString();
        }


        if (bracketText != null)
        {
            bracketText.text =
                ")";
        }


        // Jangan tampilkan langkah penyelesaian
        // ketika pemain masih menjawab.
        if (divisionWorkArea != null)
        {
            divisionWorkArea
                .SetActive(false);
        }


        return true;
    }


    //==================================================
    // CLEAR
    //==================================================

    public void ClearDisplay()
    {
        if (dividendText != null)
        {
            dividendText.text = "";
        }


        if (divisorText != null)
        {
            divisorText.text = "";
        }


        if (bracketText != null)
        {
            bracketText.text = "";
        }


        if (divisionWorkArea != null)
        {
            divisionWorkArea
                .SetActive(false);
        }


        currentQuestion =
            null;
    }
}