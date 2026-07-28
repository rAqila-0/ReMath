using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // ==================================================
    // SINGLETON
    // ==================================================

    public static AudioManager Instance { get; private set; }


    // ==================================================
    // AUDIO SOURCE
    // ==================================================

    [Header("Audio Source")]

    [Tooltip("AudioSource yang digunakan untuk memainkan sound effect.")]
    [SerializeField]
    private AudioSource sfxSource;


    // ==================================================
    // AUDIO CLIPS - ITEM
    // ==================================================

    [Header("Sound Effect - Item")]

    [Tooltip("Suara ketika player mulai mengambil item.")]
    [SerializeField]
    private AudioClip dragSound;

    [Tooltip("Suara ketika item berhasil masuk ke kotak jawaban.")]
    [SerializeField]
    private AudioClip dropSound;


    // ==================================================
    // AUDIO CLIPS - JAWABAN
    // ==================================================

    [Header("Sound Effect - Jawaban")]

    [Tooltip("Suara ketika jawaban benar.")]
    [SerializeField]
    private AudioClip correctSound;

    [Tooltip("Suara ketika jawaban salah.")]
    [SerializeField]
    private AudioClip wrongSound;


    // ==================================================
    // AUDIO CLIPS - TOMBOL
    // ==================================================

    [Header("Sound Effect - Tombol")]

    [Tooltip("Suara ketika tombol Undo ditekan.")]
    [SerializeField]
    private AudioClip undoSound;

    [Tooltip("Suara ketika tombol Reset ditekan.")]
    [SerializeField]
    private AudioClip resetSound;

    [Tooltip("Suara umum ketika tombol UI ditekan.")]
    [SerializeField]
    private AudioClip buttonClickSound;


    // ==================================================
    // AWAKE
    // ==================================================

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);


        if (sfxSource == null)
        {
            sfxSource = GetComponent<AudioSource>();
        }


        if (sfxSource == null)
        {
            Debug.LogError(
                "AudioManager: AudioSource tidak ditemukan!"
            );
        }
    }


    // ==================================================
    // METHOD UTAMA MEMAINKAN SFX
    // ==================================================

    private void PlaySFX(AudioClip clip)
    {
        if (sfxSource == null)
        {
            Debug.LogWarning(
                "AudioManager: SFX Source belum tersedia."
            );

            return;
        }


        if (clip == null)
        {
            Debug.LogWarning(
                "AudioManager: AudioClip belum dihubungkan."
            );

            return;
        }


        sfxSource.PlayOneShot(clip);
    }


    // ==================================================
    // SUARA DRAG
    // ==================================================

    public void PlayDragSound()
    {
        PlaySFX(dragSound);
    }


    // ==================================================
    // SUARA DROP
    // ==================================================

    public void PlayDropSound()
    {
        PlaySFX(dropSound);
    }


    // ==================================================
    // SUARA JAWABAN BENAR
    // ==================================================

    public void PlayCorrectSound()
    {
        PlaySFX(correctSound);
    }


    // ==================================================
    // SUARA JAWABAN SALAH
    // ==================================================

    public void PlayWrongSound()
    {
        PlaySFX(wrongSound);
    }


    // ==================================================
    // SUARA UNDO
    // ==================================================

    public void PlayUndoSound()
    {
        PlaySFX(undoSound);
    }


    // ==================================================
    // SUARA RESET
    // ==================================================

    public void PlayResetSound()
    {
        PlaySFX(resetSound);
    }


    // ==================================================
    // SUARA TOMBOL
    // ==================================================

    public void PlayButtonClickSound()
    {
        PlaySFX(buttonClickSound);
    }
}
