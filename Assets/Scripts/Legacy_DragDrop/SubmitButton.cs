using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubmitButton : MonoBehaviour
{
    [Header("Pengaturan Tombol Submit")]
    [Tooltip("Tombol Paw yang digunakan untuk mengonfirmasi jawaban.")]
    [SerializeField] private Button submitButton;


    private void Awake()
    {
        // Jika belum dihubungkan melalui Inspector,
        // coba ambil komponen Button dari GameObject ini.
        if (submitButton == null)
        {
            submitButton = GetComponent<Button>();
        }

        // Validasi apakah Button ditemukan.
        if (submitButton == null)
        {
            Debug.LogError(
                "SubmitButton pada " + gameObject.name +
                " tidak menemukan komponen Button!"
            );

            return;
        }

        // Tambahkan fungsi SubmitAnswer ke event OnClick.
        submitButton.onClick.AddListener(SubmitAnswer);
    }


    // ==================================================
    // KETIKA TOMBOL PAW DITEKAN
    // ==================================================

    public void SubmitAnswer()
    {
        // Pastikan GameManager tersedia.
        if (GameManager.Instance == null)
        {
            Debug.LogError(
                "GameManager.Instance tidak ditemukan!"
            );

            return;
        }

        Debug.Log("Tombol Paw ditekan. Mengecek jawaban...");

        // Meminta GameManager mengecek jawaban pemain.
        GameManager.Instance.CheckAnswer();
    }


    // ==================================================
    // MEMBERSIHKAN LISTENER
    // ==================================================

    private void OnDestroy()
    {
        if (submitButton != null)
        {
            submitButton.onClick.RemoveListener(SubmitAnswer);
        }
    }
}
