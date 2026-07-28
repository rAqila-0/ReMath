using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIButtonSound : MonoBehaviour
{
    private Button button;


    // ==================================================
    // AWAKE
    // ==================================================

    private void Awake()
    {
        button = GetComponent<Button>();
    }


    // ==================================================
    // ON ENABLE
    // ==================================================

    private void OnEnable()
    {
        if (button != null)
        {
            button.onClick.AddListener(
                PlayClickSound
            );
        }
    }


    // ==================================================
    // ON DISABLE
    // ==================================================

    private void OnDisable()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(
                PlayClickSound
            );
        }
    }


    // ==================================================
    // MAINKAN SUARA CLICK
    // ==================================================

    private void PlayClickSound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance
                .PlayButtonClickSound();
        }
    }
}
