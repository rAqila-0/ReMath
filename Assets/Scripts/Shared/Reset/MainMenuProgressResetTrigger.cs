using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Opens a tester-only password panel when the Main Menu title button is
/// clicked/tapped twice within the configured interval.
/// </summary>
public class MainMenuProgressResetTrigger : MonoBehaviour
{
    [Header("Title Double Tap")]
    [Tooltip("Button yang dipasang pada gambar/judul game di Main Menu.")]
    [SerializeField]
    private Button titleButton;

    [Tooltip("Batas waktu antar dua tap/klik untuk membuka panel password.")]
    [SerializeField, Min(0.1f)]
    private float doubleTapInterval = 0.6f;

    [Header("Password Panel")]
    [SerializeField]
    private GameObject passwordPanel;

    [SerializeField]
    private TMP_InputField passwordInputField;

    [Tooltip("Opsional: TMP_Text untuk menampilkan pesan salah/berhasil.")]
    [SerializeField]
    private TMP_Text feedbackText;

    [Tooltip("Tombol verifikasi. Listener dipasang otomatis oleh script.")]
    [SerializeField]
    private Button submitButton;

    [Tooltip("Tombol tutup/batal. Listener dipasang otomatis oleh script.")]
    [SerializeField]
    private Button closeButton;

    [Header("Reset Verification")]
    [Tooltip("Masukkan kode tester di Inspector. Kode ini bukan sistem autentikasi yang aman untuk data sensitif.")]
    [SerializeField]
    private string resetPassword = "TESTER-RESET-2026";

    [SerializeField]
    private ReMathProgressResetter progressResetter;

    private float lastTapTime = -100f;
    private int tapCount;
    private bool resetCompleted;

    private void Awake()
    {
        if (progressResetter == null)
            progressResetter = GetComponent<ReMathProgressResetter>();

        if (passwordPanel != null)
            passwordPanel.SetActive(false);

        if (titleButton != null)
            titleButton.onClick.AddListener(HandleTitleTap);
        else
            Debug.LogError("MainMenuProgressResetTrigger: Title Button belum diisi di Inspector.", this);

        if (submitButton != null)
            submitButton.onClick.AddListener(SubmitPassword);

        if (closeButton != null)
            closeButton.onClick.AddListener(ClosePasswordPanel);

        if (passwordPanel == null)
            Debug.LogError("MainMenuProgressResetTrigger: Password Panel belum diisi di Inspector.", this);

        if (passwordInputField == null)
            Debug.LogError("MainMenuProgressResetTrigger: TMP_InputField belum diisi di Inspector.", this);

        if (progressResetter == null)
            Debug.LogError("MainMenuProgressResetTrigger: ReMathProgressResetter belum diisi dan tidak ditemukan pada GameObject yang sama.", this);
    }

    private void OnDestroy()
    {
        if (titleButton != null)
            titleButton.onClick.RemoveListener(HandleTitleTap);

        if (submitButton != null)
            submitButton.onClick.RemoveListener(SubmitPassword);

        if (closeButton != null)
            closeButton.onClick.RemoveListener(ClosePasswordPanel);
    }

    /// <summary>
    /// Called automatically by the title button. Two clicks/taps within the
    /// interval open the password panel.
    /// </summary>
    public void HandleTitleTap()
    {
        if (passwordPanel != null && passwordPanel.activeSelf)
            return;

        float currentTime = Time.unscaledTime;

        if (currentTime - lastTapTime <= doubleTapInterval)
            tapCount++;
        else
            tapCount = 1;

        lastTapTime = currentTime;

        if (tapCount >= 2)
        {
            tapCount = 0;
            lastTapTime = -100f;
            OpenPasswordPanel();
        }
    }

    public void OpenPasswordPanel()
    {
        if (passwordPanel == null)
        {
            Debug.LogError("MainMenuProgressResetTrigger: Password Panel belum diisi.", this);
            return;
        }

        resetCompleted = false;
        passwordPanel.SetActive(true);

        if (feedbackText != null)
            feedbackText.text = string.Empty;

        if (passwordInputField != null)
        {
            passwordInputField.interactable = true;
            passwordInputField.text = string.Empty;
            passwordInputField.Select();
            passwordInputField.ActivateInputField();
        }

        if (submitButton != null)
            submitButton.interactable = true;
    }

    public void SubmitPassword()
    {
        if (resetCompleted)
            return;

        if (string.IsNullOrEmpty(resetPassword))
        {
            SetFeedback("Kode reset belum diatur di Inspector.");
            Debug.LogError("MainMenuProgressResetTrigger: Reset Password kosong. Atur password di Inspector.", this);
            return;
        }

        if (passwordInputField == null)
        {
            SetFeedback("Input kode belum dikonfigurasi.");
            Debug.LogError("MainMenuProgressResetTrigger: TMP_InputField belum diisi.", this);
            return;
        }

        if (!string.Equals(passwordInputField.text, resetPassword, StringComparison.Ordinal))
        {
            SetFeedback("Kode salah. Silakan coba lagi.");
            passwordInputField.text = string.Empty;
            passwordInputField.ActivateInputField();
            return;
        }

        if (progressResetter == null)
        {
            SetFeedback("Sistem reset belum dikonfigurasi.");
            Debug.LogError("MainMenuProgressResetTrigger: ReMathProgressResetter belum diisi.", this);
            return;
        }

        bool success = progressResetter.ResetAllProgress();
        if (!success)
        {
            SetFeedback("Reset gagal. Periksa Console Unity.");
            return;
        }

        resetCompleted = true;
        SetFeedback("Verifikasi berhasil. Semua progres level sudah direset.");
        passwordInputField.text = string.Empty;
        passwordInputField.interactable = false;

        if (submitButton != null)
            submitButton.interactable = false;
    }

    public void ClosePasswordPanel()
    {
        if (passwordPanel != null)
            passwordPanel.SetActive(false);

        if (passwordInputField != null)
        {
            passwordInputField.text = string.Empty;
            passwordInputField.interactable = true;
        }

        if (submitButton != null)
            submitButton.interactable = true;

        if (feedbackText != null)
            feedbackText.text = string.Empty;

        resetCompleted = false;
        tapCount = 0;
        lastTapTime = -100f;
    }

    private void SetFeedback(string message)
    {
        if (feedbackText != null)
            feedbackText.text = message;

        Debug.Log("MainMenuProgressResetTrigger: " + message, this);
    }
}
