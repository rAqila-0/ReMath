using UnityEngine;

public class QuestionDisplayController : MonoBehaviour
{
    //==================================================
    // ROOT
    //==================================================

    [Header("Root")]

    [SerializeField]
    private GameObject questionDisplayRoot;


    //==================================================
    // QUESTION PANELS
    //==================================================

    [Header("Question Panels")]

    [SerializeField]
    private GameObject addSubQuestionPanel;

    [SerializeField]
    private GameObject multiplicationQuestionPanel;

    [SerializeField]
    private GameObject divisionQuestionPanel;

    //==================================================
    // ANSWER INPUT
    //==================================================

    [Header("Answer Input")]

    [SerializeField]
    private AnswerSlotInputManager
        answerSlotInputManager;


    [Header("Answer Slot Groups")]

    [SerializeField]
    private AnswerSlotGroup
        addSubAnswerSlotGroup;

    [SerializeField]
    private AnswerSlotGroup
        multiplicationAnswerSlotGroup;

    [SerializeField]
    private AnswerSlotGroup
        divisionAnswerSlotGroup;

    //==================================================
    // RENDERERS
    //==================================================

    [Header("Question Renderers")]

    [SerializeField]
    private VerticalAddSubRenderer
        verticalAddSubRenderer;

    [SerializeField]
    private MultiplicationRenderer
        multiplicationRenderer;

    [SerializeField]
    private DivisionPorogapitRenderer
        divisionPorogapitRenderer;


    //==================================================
    // SHOW
    //==================================================

    public void ShowForOperation(
        MathOperation operation
    )
    {
        HideAll();


        if (questionDisplayRoot != null)
        {
            questionDisplayRoot.SetActive(true);
        }


        switch (operation)
        {
            //==================================================
            // ADDITION / SUBTRACTION
            //==================================================

            case MathOperation.Addition:

            case MathOperation.Subtraction:

                if (addSubQuestionPanel != null)
                {
                    addSubQuestionPanel.SetActive(true);
                }


                if (
                    answerSlotInputManager != null &&
                    addSubAnswerSlotGroup != null
                )
                {
                    answerSlotInputManager.SetActiveGroup(
                        addSubAnswerSlotGroup,
                        AnswerInputMode.ColumnRightToLeft
                    );
                }

                break;


            //==================================================
            // MULTIPLICATION
            //==================================================

            case MathOperation.Multiplication:

            if (
                multiplicationQuestionPanel !=
                null
            )
            {
                multiplicationQuestionPanel
                    .SetActive(true);
            }


            if (
                answerSlotInputManager != null &&
                multiplicationAnswerSlotGroup != null
            )
            {
                answerSlotInputManager
                    .SetActiveGroup(
                        multiplicationAnswerSlotGroup,
                        AnswerInputMode.NormalNumber
                    );
            }

            break;


            //==================================================
            // DIVISION
            //==================================================

            case MathOperation.Division:

            if (
                divisionQuestionPanel !=
                null
            )
            {
                divisionQuestionPanel
                    .SetActive(true);
            }


            if (
                answerSlotInputManager != null &&
                divisionAnswerSlotGroup != null
            )
            {
                answerSlotInputManager
                    .SetActiveGroup(
                        divisionAnswerSlotGroup,
                        AnswerInputMode.NormalNumber
                    );
            }

            break;
        }
    }


    //==================================================
    // HIDE ALL
    //==================================================

    public void HideAll()
    {
        if (addSubQuestionPanel != null)
        {
            addSubQuestionPanel.SetActive(false);
        }


        if (multiplicationQuestionPanel != null)
        {
            multiplicationQuestionPanel.SetActive(false);
        }


        if (divisionQuestionPanel != null)
        {
            divisionQuestionPanel.SetActive(false);
        }


        if (questionDisplayRoot != null)
        {
            questionDisplayRoot.SetActive(false);
        }
    }

    //==================================================
    // DISPLAY QUESTION
    //==================================================

    public bool DisplayQuestion(
        QuestionData question
    )
    {
        if (question == null)
        {
            Debug.LogError(
                "QuestionDisplayController: QuestionData null."
            );

            return false;
        }


        ShowForOperation(
            question.operation
        );


        switch (question.operation)
        {
            case MathOperation.Addition:
            case MathOperation.Subtraction:

                if (verticalAddSubRenderer == null)
                {
                    Debug.LogError(
                        "QuestionDisplayController: " +
                        "VerticalAddSubRenderer belum diisi."
                    );

                    return false;
                }

                return verticalAddSubRenderer
                    .Render(question);


            case MathOperation.Multiplication:

                if (multiplicationRenderer == null)
                {
                    Debug.LogError(
                        "QuestionDisplayController: " +
                        "MultiplicationRenderer belum diisi."
                    );

                    return false;
                }

                return multiplicationRenderer
                    .Render(question);


            case MathOperation.Division:

                if (divisionPorogapitRenderer == null)
                {
                    Debug.LogError(
                        "QuestionDisplayController: " +
                        "DivisionPorogapitRenderer belum diisi."
                    );

                    return false;
                }

                return divisionPorogapitRenderer
                    .Render(question);


            default:

                Debug.LogError(
                    "QuestionDisplayController: " +
                    "MathOperation tidak dikenal."
                );

                return false;
        }
    }




    //==================================================
    // TEST
    //==================================================

    [ContextMenu("TEST - Addition")]
    private void TestAddition()
    {
        ShowForOperation(
            MathOperation.Addition
        );
    }


    [ContextMenu("TEST - Subtraction")]
    private void TestSubtraction()
    {
        ShowForOperation(
            MathOperation.Subtraction
        );
    }


    [ContextMenu("TEST - Multiplication")]
    private void TestMultiplication()
    {
        ShowForOperation(
            MathOperation.Multiplication
        );
    }


    [ContextMenu("TEST - Division")]
    private void TestDivision()
    {
        ShowForOperation(
            MathOperation.Division
        );
    }


    [ContextMenu("TEST - Hide")]
    private void TestHide()
    {
        HideAll();
    }

    [ContextMenu(
        "TEST - Render Addition 5674 + 1238"
    )]
    private void TestRenderAdditionQuestion()
    {
        QuestionData question =
            new QuestionData(
                5674,
                1238,
                6912,
                MathOperation.Addition,
                SpellTier.Advanced
            );


        DisplayQuestion(
            question
        );
    }

    [ContextMenu(
    "TEST - Render Multiplication 7 x 7"
    )]
    private void TestRenderMultiplication7x7()
    {
        QuestionData question =
            new QuestionData(
                7,
                7,
                49,
                MathOperation.Multiplication,
                SpellTier.Advanced
            );


        DisplayQuestion(
            question
        );
    }

    [ContextMenu(
    "TEST - Render Division 84 / 7"
    )]
    private void TestRenderDivision84By7()
    {
        QuestionData question =
            new QuestionData(
                84,
                7,
                12,
                MathOperation.Division,
                SpellTier.Medium
            );


        DisplayQuestion(
            question
        );
    }
}