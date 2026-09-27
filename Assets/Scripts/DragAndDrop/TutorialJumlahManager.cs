using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class TutorialJumlahManager : MonoBehaviour
{
    //==================================================
    // STEP DATA
    //==================================================

    [System.Serializable]
    public class TutorialStep
    {
        [Header("Informasi")]
        public string title;

        [TextArea(3,8)]
        public string description;

        [Header("Highlight")]
        public GameObject highlightTarget;

        [Header("Pointer")]

        public bool showPointer;

        public RectTransform pointerStart;

        public RectTransform pointerEnd;
    }


    //==================================================
    // DAFTAR STEP
    //==================================================

    [Header("Tutorial Step")]

    public TutorialStep[] tutorialSteps;


    //==================================================
    // UI
    //==================================================

    [Header("Tutorial UI")]

    public TMP_Text titleText;

    public TMP_Text descriptionText;

    public TMP_Text stepText;


    [Header("Button")]

    [SerializeField]
    private Button nextButton;

    [SerializeField]
    private Button previousButton;

    [Header("Button Text")]

    [SerializeField]
    private TMP_Text nextButtonText;

    [SerializeField]
    private TMP_Text previousButtonText;

    [Header("Pointer")]

    [SerializeField]
    private TutorialHandPointer handPointer;


    //==================================================
    // SCENE
    //==================================================

    [Header("Scene")]

    public string gameplayScene = "Jumlah_Level-1";


    //==================================================
    // CURRENT STEP
    //==================================================

    private int currentStep = 0;


    //==================================================
    // START
    //==================================================

    private void Start()
    {
        ShowStep(0);
    }


    //==================================================
    // MENAMPILKAN STEP
    //==================================================

    private void ShowStep(int stepIndex)
    {
        if(stepIndex < 0)
            return;

        if(stepIndex >= tutorialSteps.Length)
            return;


        currentStep = stepIndex;


        //==============================
        // MATIKAN SEMUA HIGHLIGHT
        //==============================

        foreach(var step in tutorialSteps)
        {
            if(step.highlightTarget != null)
                step.highlightTarget.SetActive(false);
        }


        //==============================
        // AKTIFKAN HIGHLIGHT
        //==============================

        if(tutorialSteps[currentStep].highlightTarget != null)
        {
            tutorialSteps[currentStep]
                .highlightTarget
                .SetActive(true);
        }


        //==============================
        // UPDATE TEXT
        //==============================

        titleText.text =
            tutorialSteps[currentStep].title;

        descriptionText.text =
            tutorialSteps[currentStep].description;

        stepText.text =
            "Step "
            + (currentStep + 1)
            + " / "
            + tutorialSteps.Length;
        
        //==============================
        // HAND POINTER
        //==============================

        if(handPointer != null)
        {
            TutorialStep step = tutorialSteps[currentStep];

            if(step.showPointer &&
            step.pointerStart != null &&
            step.pointerEnd != null)
            {
                handPointer.gameObject.SetActive(true);

                handPointer.SetTarget(
                    step.pointerStart,
                    step.pointerEnd);

                handPointer.Play();
            }
            else
            {
                handPointer.Stop();
                handPointer.gameObject.SetActive(false);
            }
        }


        //==============================
        // BUTTON
        //==============================

        previousButton.interactable =
            currentStep > 0;


        if(currentStep == tutorialSteps.Length - 1)
        {
            nextButtonText.text = "Mulai";
        }
        else
        {
            nextButtonText.text = "Next";
        }
    }


    //==================================================
    // NEXT
    //==================================================

    public void NextStep()
    {
        if(AudioManager.Instance != null)
        {
            AudioManager.Instance
                .PlayButtonClickSound();
        }

        if(currentStep < tutorialSteps.Length - 1)
        {
            ShowStep(currentStep + 1);
        }
        else
        {
            SceneManager.LoadScene(gameplayScene);
        }
    }


    //==================================================
    // PREVIOUS
    //==================================================

    public void PreviousStep()
    {
        if(AudioManager.Instance != null)
        {
            AudioManager.Instance
                .PlayButtonClickSound();
        }

        if(currentStep > 0)
        {
            ShowStep(currentStep - 1);
        }
    }


    //==================================================
    // SKIP
    //==================================================

    public void SkipTutorial()
    {
        if(AudioManager.Instance != null)
        {
            AudioManager.Instance
                .PlayButtonClickSound();
        }

        SceneManager.LoadScene(gameplayScene);
    }
}