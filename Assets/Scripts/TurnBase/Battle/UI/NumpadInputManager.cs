using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumpadInputManager : MonoBehaviour
{
    //==================================================
    // REFERENCES
    //==================================================

    [Header("References")]

    [SerializeField]
    private BattleUIManager battleUI;


    //==================================================
    // INPUT SETTINGS
    //==================================================

    [Header("Input Settings")]

    [SerializeField]
    [Min(1)]
    private int maxDigits = 5;


    //==================================================
    // CURRENT INPUT
    //==================================================

    private string currentInput = "";


    //==================================================
    // PROPERTIES
    //==================================================

    public string CurrentInput =>
        currentInput;

    public bool HasInput =>
        !string.IsNullOrEmpty(currentInput);


    //==================================================
    // INPUT NUMBER
    //==================================================

    public void InputNumber(int number)
    {
        if (number < 0 || number > 9)
        {
            Debug.LogWarning(
                "NumpadInputManager: " +
                "Input angka harus 0 sampai 9."
            );

            return;
        }


        //==============================================
        // BATAS DIGIT
        //==============================================

        if (currentInput.Length >= maxDigits)
        {
            return;
        }


        //==============================================
        // CEGAH NOL BERULANG DI DEPAN
        //==============================================

        if (currentInput == "0")
        {
            if (number == 0)
            {
                return;
            }

            currentInput =
                number.ToString();
        }
        else
        {
            currentInput +=
                number.ToString();
        }


        UpdateAnswerDisplay();
    }


    //==================================================
    // CLEAR
    //==================================================

    public void ClearInput()
    {
        currentInput = "";

        UpdateAnswerDisplay();
    }


    //==================================================
    // BACKSPACE
    //==================================================

    public void Backspace()
    {
        if (string.IsNullOrEmpty(currentInput))
            return;

        currentInput =
            currentInput.Substring(
                0,
                currentInput.Length - 1
            );

        UpdateAnswerDisplay();
    }


    //==================================================
    // RESET INPUT
    //==================================================

    public void ResetInput()
    {
        currentInput = "";

        UpdateAnswerDisplay();
    }


    //==================================================
    // GET ANSWER AS INT
    //==================================================

    public bool TryGetAnswer(out int answer)
    {
        if (string.IsNullOrEmpty(currentInput))
        {
            answer = 0;

            return false;
        }

        return int.TryParse(
            currentInput,
            out answer
        );
    }


    //==================================================
    // UPDATE ANSWER UI
    //==================================================

    private void UpdateAnswerDisplay()
    {
        if (battleUI != null)
        {
            battleUI.SetAnswerText(
                currentInput
            );
        }
    }


    //==================================================
    // TEST
    //==================================================

    [ContextMenu("TEST - Input 123")]
    private void TestInput123()
    {
        ResetInput();

        InputNumber(1);
        InputNumber(2);
        InputNumber(3);
    }


    [ContextMenu("TEST - Clear")]
    private void TestClear()
    {
        ClearInput();
    }


    [ContextMenu("TEST - Backspace")]
    private void TestBackspace()
    {
        Backspace();
    }
}
