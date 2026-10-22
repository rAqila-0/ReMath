using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIButtonSound : MonoBehaviour
{
    private Button button;


    //==================================================
    // AWAKE
    //==================================================

    private void Awake()
    {
        button = GetComponent<Button>();
    }


    //==================================================
    // ENABLE
    //==================================================

    private void OnEnable()
    {
        if (button != null)
        {
            button.onClick.AddListener(
                PlayClickSound
            );
        }
    }


    //==================================================
    // DISABLE
    //==================================================

    private void OnDisable()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(
                PlayClickSound
            );
        }
    }


    //==================================================
    // CLICK SOUND
    //==================================================

    private void PlayClickSound()
    {
        if (AudioManager.Instance == null)
        {
            Debug.LogWarning(
                "UIButtonSound: " +
                "AudioManager.Instance tidak ditemukan."
            );

            return;
        }


        AudioManager.Instance
            .PlayButtonClickSound();
    }
}