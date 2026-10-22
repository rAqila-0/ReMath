using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TurnPresentationController : MonoBehaviour
{
    //==================================================
    // TURN BANNER
    //==================================================

    [Header("Turn Banner")]

    [SerializeField]
    private GameObject turnBanner;

    [SerializeField]
    private CanvasGroup turnBannerCanvasGroup;

    [SerializeField]
    private TMP_Text turnBannerText;

    [SerializeField]
    private RectTransform turnBannerRect;


    //==================================================
    // WARNING
    //==================================================

    [Header("Warning Arrow")]

    [SerializeField]
    private GameObject warningArrow;

    [SerializeField]
    private RectTransform warningArrowRect;

    [SerializeField]
    private CanvasGroup warningArrowCanvasGroup;


    //==================================================
    // TURN TIMING
    //==================================================

    [Header("Turn Banner Timing")]

    [SerializeField]
    private float bannerFadeDuration = 0.2f;

    [SerializeField]
    private float bannerHoldDuration = 0.65f;


    //==================================================
    // WARNING SETTINGS
    //==================================================

    [Header("Warning Animation")]

    [SerializeField]
    private float warningPulseSpeed = 4f;

    [SerializeField]
    private float warningMinScale = 0.9f;

    [SerializeField]
    private float warningMaxScale = 1.1f;

    [SerializeField]
    private float warningMinAlpha = 0.55f;

    [SerializeField]
    private float warningMaxAlpha = 1f;


    //==================================================
    // RUNTIME
    //==================================================

    private Coroutine warningCoroutine;

    private Vector3 bannerOriginalScale;
    private Vector3 warningOriginalScale;


    //==================================================
    // AWAKE
    //==================================================

    private void Awake()
    {
        if (turnBannerRect != null)
        {
            bannerOriginalScale =
                turnBannerRect.localScale;
        }

        if (warningArrowRect != null)
        {
            warningOriginalScale =
                warningArrowRect.localScale;
        }


        HideTurnBanner();

        StopWarning();
    }


    //==================================================
    // TURN INTRO
    //==================================================

    public IEnumerator PlayTurnIntro(
        BattleTurn turn
    )
    {
        if (turnBanner == null)
            yield break;


        //==============================================
        // TEXT
        //==============================================

        if (turnBannerText != null)
        {
            switch (turn)
            {
                case BattleTurn.Player:

                    turnBannerText.text =
                        "GILIRAN PEMAIN";

                    break;


                case BattleTurn.Enemy:

                    turnBannerText.text =
                        "GILIRAN MUSUH";

                    break;
            }
        }


        //==============================================
        // SHOW
        //==============================================

        turnBanner.SetActive(true);


        if (turnBannerCanvasGroup != null)
        {
            turnBannerCanvasGroup.alpha = 0f;
        }


        if (turnBannerRect != null)
        {
            turnBannerRect.localScale =
                bannerOriginalScale * 0.85f;
        }


        //==============================================
        // FADE IN
        //==============================================

        float elapsed = 0f;


        while (elapsed < bannerFadeDuration)
        {
            elapsed += Time.deltaTime;


            float progress =
                Mathf.Clamp01(
                    elapsed /
                    bannerFadeDuration
                );


            if (turnBannerCanvasGroup != null)
            {
                turnBannerCanvasGroup.alpha =
                    progress;
            }


            if (turnBannerRect != null)
            {
                turnBannerRect.localScale =
                    Vector3.Lerp(
                        bannerOriginalScale * 0.85f,
                        bannerOriginalScale,
                        progress
                    );
            }


            yield return null;
        }


        //==============================================
        // HOLD
        //==============================================

        yield return new WaitForSeconds(
            bannerHoldDuration
        );


        //==============================================
        // FADE OUT
        //==============================================

        elapsed = 0f;


        while (elapsed < bannerFadeDuration)
        {
            elapsed += Time.deltaTime;


            float progress =
                Mathf.Clamp01(
                    elapsed /
                    bannerFadeDuration
                );


            if (turnBannerCanvasGroup != null)
            {
                turnBannerCanvasGroup.alpha =
                    1f - progress;
            }


            yield return null;
        }


        HideTurnBanner();
    }


    //==================================================
    // HIDE TURN BANNER
    //==================================================

    private void HideTurnBanner()
    {
        if (turnBannerCanvasGroup != null)
        {
            turnBannerCanvasGroup.alpha = 0f;
        }


        if (turnBannerRect != null)
        {
            turnBannerRect.localScale =
                bannerOriginalScale;
        }


        if (turnBanner != null)
        {
            turnBanner.SetActive(false);
        }
    }


    //==================================================
    // START WARNING
    //==================================================

    public void StartWarning()
    {
        if (warningArrow == null)
            return;


        StopWarning();


        warningArrow.SetActive(true);


        if (warningArrowCanvasGroup != null)
        {
            warningArrowCanvasGroup.alpha = 1f;
        }


        warningCoroutine =
            StartCoroutine(
                WarningPulseRoutine()
            );
    }


    //==================================================
    // STOP WARNING
    //==================================================

    public void StopWarning()
    {
        if (warningCoroutine != null)
        {
            StopCoroutine(
                warningCoroutine
            );

            warningCoroutine = null;
        }


        if (warningArrowRect != null)
        {
            warningArrowRect.localScale =
                warningOriginalScale;
        }


        if (warningArrowCanvasGroup != null)
        {
            warningArrowCanvasGroup.alpha = 1f;
        }


        if (warningArrow != null)
        {
            warningArrow.SetActive(false);
        }
    }


    //==================================================
    // WARNING LOOP
    //==================================================

    private IEnumerator WarningPulseRoutine()
    {
        while (true)
        {
            float wave =
                (
                    Mathf.Sin(
                        Time.time *
                        warningPulseSpeed
                    ) +
                    1f
                ) * 0.5f;


            //==========================================
            // SCALE
            //==========================================

            if (warningArrowRect != null)
            {
                float scale =
                    Mathf.Lerp(
                        warningMinScale,
                        warningMaxScale,
                        wave
                    );


                warningArrowRect.localScale =
                    warningOriginalScale *
                    scale;
            }


            //==========================================
            // ALPHA
            //==========================================

            if (warningArrowCanvasGroup != null)
            {
                warningArrowCanvasGroup.alpha =
                    Mathf.Lerp(
                        warningMinAlpha,
                        warningMaxAlpha,
                        wave
                    );
            }


            yield return null;
        }
    }


    //==================================================
    // RESET
    //==================================================

    public void ResetPresentation()
    {
        HideTurnBanner();

        StopWarning();
    }
}
