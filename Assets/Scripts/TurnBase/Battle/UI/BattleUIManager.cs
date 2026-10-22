using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleUIManager : MonoBehaviour
{
    //==================================================
    // HUD TEXT
    //==================================================

    [Header("HUD Text")]

    [SerializeField]
    private TMP_Text playerNameText;

    [SerializeField]
    private TMP_Text enemyNameText;

    [SerializeField]
    private TMP_Text turnText;

    [SerializeField]
    private TMP_Text levelText;


    //==================================================
    // MAIN PANELS
    //==================================================

    [Header("Main Panels")]

    [SerializeField]
    private GameObject spellPanel;

    [SerializeField]
    private GameObject defensePanel;

    [SerializeField]
    private GameObject questionPanel;


    //==================================================
    // SPELL PREVIEW
    //==================================================

    [Header("Spell Preview")]

    [SerializeField]
    private GameObject spellPreviewPanel;

    [SerializeField]
    private TMP_Text previewSpellNameText;

    [SerializeField]
    private TMP_Text previewDamageText;

    [SerializeField]
    private TMP_Text previewConfirmText;


    //==================================================
    // QUESTION HEADER
    //==================================================

    [Header("Question Header")]

    [SerializeField]
    private TMP_Text questionSpellNameText;

    [SerializeField]
    private TMP_Text questionInfoText;


    //==================================================
    // VERTICAL QUESTION
    //==================================================

    [Header("Vertical Question")]

    [SerializeField]
    private GameObject verticalQuestionArea;

    [SerializeField]
    private TMP_Text operandAText;

    [SerializeField]
    private TMP_Text operatorText;

    [SerializeField]
    private TMP_Text operandBText;

    [SerializeField]
    private TMP_Text answerText;


    //==================================================
    // ENEMY TURN
    //==================================================

    [Header("Enemy Turn UI")]

    [SerializeField]
    private GameObject warningArrow;


    //==================================================
    // FEEDBACK
    //==================================================

    [Header("Feedback")]

    [SerializeField]
    private GameObject mantraSuccess;

    [SerializeField]
    private GameObject mantraFailed;

    [SerializeField]
    private RectTransform mantraSuccessRect;

    [SerializeField]
    private CanvasGroup mantraSuccessCanvasGroup;

    [SerializeField]
    private RectTransform mantraFailedRect;

    [SerializeField]
    private CanvasGroup mantraFailedCanvasGroup;


    [Header("Feedback Animation")]

    [SerializeField]
    private float feedbackPopDuration = 0.18f;

    [SerializeField]
    private float feedbackStartScale = 0.75f;

    private Coroutine feedbackAnimationCoroutine;


    //==================================================
    // RESULT
    //==================================================

    [Header("Battle Result")]

    [SerializeField]
    private GameObject victoryPopup;

    [SerializeField]
    private GameObject defeatPopup;


    //==================================================
    // PLAYER INFORMATION
    //==================================================

    [Header("Player Information")]

    [SerializeField]
    private string playerName = "Oren";

    //==================================================
    // SPELL SELECTION VISUAL
    //==================================================

    [Header("Spell Selection Visual")]

    [SerializeField]
    private GameObject basicSelectedFrame;

    [SerializeField]
    private GameObject mediumSelectedFrame;

    [SerializeField]
    private GameObject advancedSelectedFrame;


    //==================================================
    // INITIALIZE
    //==================================================

    public void Initialize(BattleLevelConfig levelConfig)
    {
        if (levelConfig == null)
        {
            Debug.LogWarning(
                "BattleUIManager: Level Config belum diisi."
            );

            return;
        }

        if (playerNameText != null)
        {
            playerNameText.text = playerName;
        }

        if (enemyNameText != null)
        {
            enemyNameText.text = levelConfig.enemyName;
        }

        if (levelText != null)
        {
            levelText.text =
                "Level " + levelConfig.levelNumber;
        }

        ResetBattleUI();
    }


    //==================================================
    // RESET UI
    //==================================================

    public void ResetBattleUI()
    {
        HideAllBattlePanels();

        HideSpellPreview();

        HideFeedback();

        HideResultPopups();

        SetWarning(false);

        ClearQuestion();

        ShowPlayerTurn();
    }


    //==================================================
    // PLAYER TURN
    //==================================================

    public void ShowPlayerTurn()
    {
        SetTurnText(BattleTurn.Player);

        if (spellPanel != null)
            spellPanel.SetActive(true);

        if (defensePanel != null)
            defensePanel.SetActive(false);

        if (questionPanel != null)
            questionPanel.SetActive(false);

        HideSpellPreview();

        SetWarning(false);

        HideFeedback();

        ClearQuestion();
    }


    //==================================================
    // ENEMY TURN
    //==================================================

    public void ShowEnemyTurn()
    {
        SetTurnText(BattleTurn.Enemy);

        if (spellPanel != null)
            spellPanel.SetActive(false);

        if (defensePanel != null)
            defensePanel.SetActive(true);

        if (questionPanel != null)
            questionPanel.SetActive(false);

        HideSpellPreview();

        HideFeedback();

        ClearQuestion();

        SetWarning(true);
    }


    //==================================================
    // TURN TEXT
    //==================================================

    public void SetTurnText(BattleTurn turn)
    {
        if (turnText == null)
            return;

        switch (turn)
        {
            case BattleTurn.Player:
                turnText.text = "Giliran Pemain";
                break;

            case BattleTurn.Enemy:
                turnText.text = "Giliran Musuh";
                break;
        }
    }


    //==================================================
    // SPELL PREVIEW
    //==================================================

    public void ShowSpellPreview(
        SpellTier spellTier,
        int damage
    )
    {
        if (spellPreviewPanel != null)
        {
            spellPreviewPanel.SetActive(true);
        }

        if (previewSpellNameText != null)
        {
            previewSpellNameText.text =
                GetSpellDisplayName(spellTier);
        }

        if (previewDamageText != null)
        {
            previewDamageText.text =
                "Damage: x" + damage;
        }

        if (previewConfirmText != null)
        {
            previewConfirmText.text =
                "Tap lagi untuk merapal";
        }
    }


    public void HideSpellPreview()
    {
        if (spellPreviewPanel != null)
        {
            spellPreviewPanel.SetActive(false);
        }
    }


    //==================================================
    // QUESTION MODE
    //==================================================

    public void ShowQuestionMode(
        SpellTier spellTier,
        int damage
    )
    {
        if (spellPanel != null)
            spellPanel.SetActive(false);

        if (defensePanel != null)
            defensePanel.SetActive(false);

        HideSpellPreview();

        if (questionPanel != null)
            questionPanel.SetActive(true);

        if (verticalQuestionArea != null)
            verticalQuestionArea.SetActive(true);


        //==============================================
        // SPELL NAME
        //==============================================

        if (questionSpellNameText != null)
        {
            questionSpellNameText.text =
                GetSpellDisplayName(spellTier);
        }


        //==============================================
        // SPELL INFO
        //==============================================

        if (questionInfoText != null)
        {
            if (spellTier == SpellTier.Defense)
            {
                questionInfoText.text =
                    "Blokir serangan musuh!";
            }
            else
            {
                questionInfoText.text =
                    "Damage: x" + damage;
            }
        }

        SetAnswerText("");
    }


    //==================================================
    // SET VERTICAL QUESTION
    //==================================================

    public void SetVerticalQuestion(
        int operandA,
        int operandB,
        MathOperation operation
    )
    {
        if (verticalQuestionArea != null)
        {
            verticalQuestionArea.SetActive(true);
        }

        if (operandAText != null)
        {
            operandAText.text =
                operandA.ToString();
        }

        if (operandBText != null)
        {
            operandBText.text =
                operandB.ToString();
        }

        if (operatorText != null)
        {
            operatorText.text =
                GetOperatorSymbol(operation);
        }

        SetAnswerText("");
    }


    //==================================================
    // ANSWER TEXT
    //==================================================

    public void SetAnswerText(string text)
    {
        if (answerText == null)
            return;


        if (string.IsNullOrEmpty(text))
        {
            answerText.text = "?";
        }
        else
        {
            answerText.text = text;
        }
    }


    //==================================================
    // CLEAR QUESTION
    //==================================================

    public void ClearQuestion()
    {
        if (operandAText != null)
            operandAText.text = "";

        if (operandBText != null)
            operandBText.text = "";

        if (operatorText != null)
            operatorText.text = "";

        if (answerText != null)
            answerText.text = "";
    }

    //==================================================
    // BATTLE ANIMATION MODE
    //==================================================

    public void ShowBattleAnimationMode()
    {
        if (spellPanel != null)
        {
            spellPanel.SetActive(false);
        }

        if (defensePanel != null)
        {
            defensePanel.SetActive(false);
        }

        if (questionPanel != null)
        {
            questionPanel.SetActive(false);
        }

        HideSpellPreview();
    }


    //==================================================
    // WARNING
    //==================================================

    public void SetWarning(bool active)
    {
        if (warningArrow != null)
        {
            warningArrow.SetActive(active);
        }
    }


    //==================================================
    // FEEDBACK
    //==================================================

    public void ShowMantraSuccess()
    {
        HideFeedback();


        if (mantraSuccess != null)
        {
            mantraSuccess.SetActive(true);
        }


        if (feedbackAnimationCoroutine != null)
        {
            StopCoroutine(
                feedbackAnimationCoroutine
            );
        }


        feedbackAnimationCoroutine =
            StartCoroutine(
                AnimateFeedback(
                    mantraSuccessRect,
                    mantraSuccessCanvasGroup
                )
            );
    }


    public void ShowMantraFailed()
    {
        HideFeedback();


        if (mantraFailed != null)
        {
            mantraFailed.SetActive(true);
        }


        if (feedbackAnimationCoroutine != null)
        {
            StopCoroutine(
                feedbackAnimationCoroutine
            );
        }


        feedbackAnimationCoroutine =
            StartCoroutine(
                AnimateFeedback(
                    mantraFailedRect,
                    mantraFailedCanvasGroup
                )
            );
    }

    //==================================================
    // FEEDBACK ANIMATION
    //==================================================

    private IEnumerator AnimateFeedback(
        RectTransform targetRect,
        CanvasGroup targetCanvasGroup
    )
    {
        if (targetRect == null)
            yield break;


        Vector3 finalScale =
            Vector3.one;


        targetRect.localScale =
            finalScale *
            feedbackStartScale;


        if (targetCanvasGroup != null)
        {
            targetCanvasGroup.alpha = 0f;
        }


        float elapsed = 0f;


        while (
            elapsed <
            feedbackPopDuration
        )
        {
            elapsed +=
                Time.deltaTime;


            float progress =
                Mathf.Clamp01(
                    elapsed /
                    feedbackPopDuration
                );


            // Sedikit smooth.
            progress =
                Mathf.SmoothStep(
                    0f,
                    1f,
                    progress
                );


            targetRect.localScale =
                Vector3.Lerp(
                    finalScale *
                    feedbackStartScale,
                    finalScale,
                    progress
                );


            if (targetCanvasGroup != null)
            {
                targetCanvasGroup.alpha =
                    progress;
            }


            yield return null;
        }


        targetRect.localScale =
            finalScale;


        if (targetCanvasGroup != null)
        {
            targetCanvasGroup.alpha = 1f;
        }


        feedbackAnimationCoroutine = null;
    }


    public void HideFeedback()
    {
        if (feedbackAnimationCoroutine != null)
        {
            StopCoroutine(
                feedbackAnimationCoroutine
            );

            feedbackAnimationCoroutine = null;
        }


        if (mantraSuccess != null)
        {
            mantraSuccess.SetActive(false);
        }


        if (mantraFailed != null)
        {
            mantraFailed.SetActive(false);
        }


        if (mantraSuccessRect != null)
        {
            mantraSuccessRect.localScale =
                Vector3.one;
        }


        if (mantraFailedRect != null)
        {
            mantraFailedRect.localScale =
                Vector3.one;
        }


        if (mantraSuccessCanvasGroup != null)
        {
            mantraSuccessCanvasGroup.alpha =
                1f;
        }


        if (mantraFailedCanvasGroup != null)
        {
            mantraFailedCanvasGroup.alpha =
                1f;
        }
    }


    //==================================================
    // VICTORY
    //==================================================

    public void ShowVictory()
    {
        HideAllBattlePanels();

        HideSpellPreview();

        SetWarning(false);

        HideFeedback();

        if (victoryPopup != null)
        {
            victoryPopup.SetActive(true);
        }

        if (defeatPopup != null)
        {
            defeatPopup.SetActive(false);
        }
    }


    //==================================================
    // DEFEAT
    //==================================================

    public void ShowDefeat()
    {
        HideAllBattlePanels();

        HideSpellPreview();

        SetWarning(false);

        HideFeedback();

        if (defeatPopup != null)
        {
            defeatPopup.SetActive(true);
        }

        if (victoryPopup != null)
        {
            victoryPopup.SetActive(false);
        }
    }


    //==================================================
    // HIDE RESULT
    //==================================================

    public void HideResultPopups()
    {
        if (victoryPopup != null)
        {
            victoryPopup.SetActive(false);
        }

        if (defeatPopup != null)
        {
            defeatPopup.SetActive(false);
        }
    }


    //==================================================
    // HIDE MAIN PANELS
    //==================================================

    private void HideAllBattlePanels()
    {
        if (spellPanel != null)
        {
            spellPanel.SetActive(false);
        }

        if (defensePanel != null)
        {
            defensePanel.SetActive(false);
        }

        if (questionPanel != null)
        {
            questionPanel.SetActive(false);
        }
    }


    //==================================================
    // SPELL DISPLAY NAME
    //==================================================

    private string GetSpellDisplayName(
        SpellTier spellTier
    )
    {
        switch (spellTier)
        {
            case SpellTier.Basic:
                return "SIHIR DASAR";

            case SpellTier.Medium:
                return "SIHIR MENENGAH";

            case SpellTier.Advanced:
                return "SIHIR TINGKAT LANJUT";

            case SpellTier.Defense:
                return "SIHIR PERTAHANAN";

            default:
                return "";
        }
    }


    //==================================================
    // OPERATOR SYMBOL
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
    // PREPARE TURN TRANSITION
    //==================================================

    public void PrepareTurnTransition(
        BattleTurn turn
    )
    {
        SetTurnText(turn);


        if (spellPanel != null)
        {
            spellPanel.SetActive(false);
        }


        if (defensePanel != null)
        {
            defensePanel.SetActive(false);
        }


        if (questionPanel != null)
        {
            questionPanel.SetActive(false);
        }


        HideSpellPreview();

        HideFeedback();

        ClearQuestion();
    }


    //==================================================
    // SHOW PLAYER ACTION
    //==================================================

    public void ShowPlayerActionPanel()
    {
        if (spellPanel != null)
        {
            spellPanel.SetActive(true);
        }


        if (defensePanel != null)
        {
            defensePanel.SetActive(false);
        }


        if (questionPanel != null)
        {
            questionPanel.SetActive(false);
        }
    }


    //==================================================
    // SHOW ENEMY ACTION
    //==================================================

    public void ShowEnemyActionPanel()
    {
        if (spellPanel != null)
        {
            spellPanel.SetActive(false);
        }


        if (defensePanel != null)
        {
            defensePanel.SetActive(true);
        }


        if (questionPanel != null)
        {
            questionPanel.SetActive(false);
        }
    }


    //==================================================
    // TEST
    //==================================================

    [ContextMenu("TEST - Player Turn")]
    private void TestPlayerTurn()
    {
        ShowPlayerTurn();
    }


    [ContextMenu("TEST - Enemy Turn")]
    private void TestEnemyTurn()
    {
        ShowEnemyTurn();
    }


    [ContextMenu("TEST - Basic Preview")]
    private void TestBasicPreview()
    {
        ShowSpellPreview(
            SpellTier.Basic,
            1
        );
    }


    [ContextMenu("TEST - Medium Preview")]
    private void TestMediumPreview()
    {
        ShowSpellPreview(
            SpellTier.Medium,
            2
        );
    }


    [ContextMenu("TEST - Advanced Preview")]
    private void TestAdvancedPreview()
    {
        ShowSpellPreview(
            SpellTier.Advanced,
            3
        );
    }


    [ContextMenu("TEST - Addition Question")]
    private void TestAdditionQuestion()
    {
        ShowQuestionMode(
            SpellTier.Basic,
            1
        );

        SetVerticalQuestion(
            425,
            317,
            MathOperation.Addition
        );

        SetAnswerText("742");
    }


    [ContextMenu("TEST - Subtraction Question")]
    private void TestSubtractionQuestion()
    {
        ShowQuestionMode(
            SpellTier.Medium,
            2
        );

        SetVerticalQuestion(
            756,
            324,
            MathOperation.Subtraction
        );

        SetAnswerText("432");
    }


    [ContextMenu("TEST - Defense Question")]
    private void TestDefenseQuestion()
    {
        ShowQuestionMode(
            SpellTier.Defense,
            0
        );

        SetVerticalQuestion(
            37,
            24,
            MathOperation.Addition
        );

        SetAnswerText("");
    }


    [ContextMenu("TEST - Victory")]
    private void TestVictory()
    {
        ShowVictory();
    }


    [ContextMenu("TEST - Defeat")]
    private void TestDefeat()
    {
        ShowDefeat();
    }

    //==================================================
    // SPELL SELECTION VISUAL
    //==================================================

    public void SetSelectedSpellVisual(
        SpellTier spellTier
    )
    {
        ClearSelectedSpellVisual();


        switch (spellTier)
        {
            case SpellTier.Basic:

                if (basicSelectedFrame != null)
                    basicSelectedFrame.SetActive(true);

                break;


            case SpellTier.Medium:

                if (mediumSelectedFrame != null)
                    mediumSelectedFrame.SetActive(true);

                break;


            case SpellTier.Advanced:

                if (advancedSelectedFrame != null)
                    advancedSelectedFrame.SetActive(true);

                break;
        }
    }


    public void ClearSelectedSpellVisual()
    {
        if (basicSelectedFrame != null)
            basicSelectedFrame.SetActive(false);

        if (mediumSelectedFrame != null)
            mediumSelectedFrame.SetActive(false);

        if (advancedSelectedFrame != null)
            advancedSelectedFrame.SetActive(false);
    }
}