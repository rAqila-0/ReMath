using TMPro;
using UnityEngine;

public class VerticalAddSubRenderer : MonoBehaviour
{
    //==================================================
    // CONSTANT
    //==================================================

    private const int ColumnCount = 4;


    //==================================================
    // HINT / CARRY / BORROW
    //==================================================

    [Header("Carry / Borrow Hint - Left to Right")]

    [SerializeField]
    private TMP_Text[] hintTexts =
        new TMP_Text[ColumnCount];


    //==================================================
    // OPERAND A
    //==================================================

    [Header("Operand A Digits - Left to Right")]

    [SerializeField]
    private TMP_Text[] operandADigitTexts =
        new TMP_Text[ColumnCount];


    //==================================================
    // OPERAND B
    //==================================================

    [Header("Operand B")]

    [SerializeField]
    private TMP_Text operatorText;

    [SerializeField]
    private TMP_Text[] operandBDigitTexts =
        new TMP_Text[ColumnCount];


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
    // RENDER QUESTION
    //==================================================

    public bool Render(
        QuestionData question
    )
    {
        if (question == null)
        {
            Debug.LogError(
                "VerticalAddSubRenderer: " +
                "QuestionData null."
            );

            return false;
        }


        if (
            question.operation !=
                MathOperation.Addition &&
            question.operation !=
                MathOperation.Subtraction
        )
        {
            Debug.LogError(
                "VerticalAddSubRenderer hanya " +
                "untuk Addition / Subtraction."
            );

            return false;
        }


        // Maksimal 4 digit operand.
        if (
            question.operandA > 9999 ||
            question.operandB > 9999 ||
            question.operandA < 0 ||
            question.operandB < 0
        )
        {
            Debug.LogError(
                "VerticalAddSubRenderer: " +
                "Operand harus 0 sampai 9999."
            );

            return false;
        }


        // Kita tidak memakai pengurangan negatif.
        if (
            question.operation ==
                MathOperation.Subtraction &&
            question.operandA <
                question.operandB
        )
        {
            Debug.LogError(
                "VerticalAddSubRenderer: " +
                "Pengurangan tidak boleh menghasilkan " +
                "bilangan negatif."
            );

            return false;
        }


        // Advanced Addition tidak boleh
        // menghasilkan 5 digit.
        if (
            question.operation ==
                MathOperation.Addition &&
            question.operandA +
                question.operandB >
                9999
        )
        {
            Debug.LogError(
                "VerticalAddSubRenderer: " +
                "Hasil Penjumlahan melebihi 4 digit."
            );

            return false;
        }


        currentQuestion =
            question;


        ClearDisplay();


        //==============================================
        // OPERANDS
        //==============================================

        SetNumberRow(
            operandADigitTexts,
            question.operandA
        );


        SetNumberRow(
            operandBDigitTexts,
            question.operandB
        );


        //==============================================
        // OPERATOR
        //==============================================

        if (operatorText != null)
        {
            operatorText.text =
                question.operation ==
                    MathOperation.Addition
                    ? "+"
                    : "-";
        }


        //==============================================
        // CARRY / BORROW
        //==============================================

        if (
            question.operation ==
            MathOperation.Addition
        )
        {
            RenderAdditionCarry(
                question.operandA,
                question.operandB
            );
        }
        else
        {
            RenderSubtractionBorrow(
                question.operandA,
                question.operandB
            );
        }


        return true;
    }


    //==================================================
    // SET NUMBER ROW
    //==================================================

    private void SetNumberRow(
        TMP_Text[] rowTexts,
        int number
    )
    {
        if (
            rowTexts == null ||
            rowTexts.Length < ColumnCount
        )
        {
            Debug.LogError(
                "VerticalAddSubRenderer: " +
                "Digit Text array harus memiliki 4 element."
            );

            return;
        }


        string numberString =
            number.ToString();


        int firstDigitIndex =
            ColumnCount -
            numberString.Length;


        for (
            int i = 0;
            i < ColumnCount;
            i++
        )
        {
            if (rowTexts[i] == null)
                continue;


            if (i < firstDigitIndex)
            {
                rowTexts[i].text = "";
            }
            else
            {
                int stringIndex =
                    i -
                    firstDigitIndex;


                rowTexts[i].text =
                    numberString[
                        stringIndex
                    ].ToString();
            }
        }
    }


    //==================================================
    // ADDITION CARRY
    //==================================================

    private void RenderAdditionCarry(
        int operandA,
        int operandB
    )
    {
        int[] digitsA =
            GetDigits(
                operandA
            );

        int[] digitsB =
            GetDigits(
                operandB
            );


        int carry = 0;


        // Hitung dari kanan → kiri.
        for (
            int i = ColumnCount - 1;
            i >= 0;
            i--
        )
        {
            int columnResult =
                digitsA[i] +
                digitsB[i] +
                carry;


            int nextCarry =
                columnResult >= 10
                    ? 1
                    : 0;


            // Carry hasil kolom ini
            // diletakkan di kolom sebelah kirinya.
            if (
                nextCarry > 0 &&
                i > 0
            )
            {
                SetHint(
                    i - 1,
                    nextCarry.ToString()
                );
            }


            carry =
                nextCarry;
        }


        // Jika sampai sini masih ada carry,
        // berarti hasil sebenarnya 5 digit.
        if (carry > 0)
        {
            Debug.LogWarning(
                "VerticalAddSubRenderer: " +
                "Ada carry keluar dari kolom ribuan. " +
                "Generator soal harus mencegah hasil 5 digit."
            );
        }
    }


    //==================================================
    // SUBTRACTION BORROW
    //==================================================

    private void RenderSubtractionBorrow(
        int operandA,
        int operandB
    )
    {
        int[] topDigits =
            GetDigits(
                operandA
            );

        int[] bottomDigits =
            GetDigits(
                operandB
            );


        // workingDigits akan berubah
        // ketika terjadi peminjaman.
        int[] workingDigits =
            (int[])topDigits.Clone();


        bool[] modified =
            new bool[ColumnCount];


        //==============================================
        // DARI SATUAN → RIBUAN
        //==============================================

        for (
            int i = ColumnCount - 1;
            i >= 0;
            i--
        )
        {
            // Tidak perlu pinjam.
            if (
                workingDigits[i] >=
                bottomDigits[i]
            )
            {
                continue;
            }


            //==========================================
            // CARI DIGIT DI KIRI YANG BISA DIPINJAM
            //==========================================

            int donorIndex =
                i - 1;


            while (
                donorIndex >= 0 &&
                workingDigits[donorIndex] == 0
            )
            {
                donorIndex--;
            }


            if (donorIndex < 0)
            {
                Debug.LogWarning(
                    "VerticalAddSubRenderer: " +
                    "Tidak menemukan digit untuk dipinjam."
                );

                return;
            }


            //==========================================
            // KURANGI DONOR
            //==========================================

            workingDigits[donorIndex]--;

            modified[donorIndex] =
                true;


            //==========================================
            // LEWATI NOL DI ANTARA DONOR DAN TARGET
            //
            // contoh:
            // 1000 - 1
            //
            // menjadi:
            // 0 9 9 10
            //==========================================

            for (
                int column =
                    donorIndex + 1;

                column < i;

                column++
            )
            {
                workingDigits[column] += 9;

                modified[column] =
                    true;
            }


            //==========================================
            // TARGET MENERIMA 10
            //==========================================

            workingDigits[i] += 10;

            modified[i] =
                true;
        }


        //==============================================
        // TAMPILKAN DIGIT YANG BERUBAH
        //==============================================

        for (
            int i = 0;
            i < ColumnCount;
            i++
        )
        {
            if (!modified[i])
                continue;


            SetHint(
                i,
                workingDigits[i].ToString()
            );
        }
    }


    //==================================================
    // GET DIGITS
    //==================================================

    private int[] GetDigits(
        int value
    )
    {
        int[] digits =
            new int[ColumnCount];


        int remainingValue =
            Mathf.Abs(value);


        //==============================================
        // SATUAN → RIBUAN
        //==============================================

        for (
            int i = ColumnCount - 1;
            i >= 0;
            i--
        )
        {
            digits[i] =
                remainingValue % 10;

            remainingValue /= 10;
        }


        return digits;
    }


    //==================================================
    // SET HINT
    //==================================================

    private void SetHint(
        int index,
        string value
    )
    {
        if (
            hintTexts == null ||
            index < 0 ||
            index >= hintTexts.Length
        )
        {
            return;
        }


        if (hintTexts[index] == null)
            return;


        hintTexts[index].text =
            value;
    }


    //==================================================
    // CLEAR DISPLAY
    //==================================================

    public void ClearDisplay()
    {
        ClearTextArray(
            hintTexts
        );

        ClearTextArray(
            operandADigitTexts
        );

        ClearTextArray(
            operandBDigitTexts
        );


        if (operatorText != null)
        {
            operatorText.text = "";
        }


        currentQuestion =
            null;
    }


    //==================================================
    // CLEAR TEXT ARRAY
    //==================================================

    private void ClearTextArray(
        TMP_Text[] texts
    )
    {
        if (texts == null)
            return;


        foreach (
            TMP_Text text
            in texts
        )
        {
            if (text != null)
            {
                text.text = "";
            }
        }
    }
}