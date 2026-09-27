using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{
    // ==================================================
    // REFERENSI TEXT UI
    // ==================================================

    [Header("Referensi Text UI")]

    [Tooltip("Text untuk menampilkan nomor level saat ini.")]
    [SerializeField]
    private TMP_Text levelText;

    [Tooltip("Text untuk menampilkan kalimat atau permintaan dari kucing.")]
    [SerializeField]
    private TMP_Text questionStoryText;

    [Tooltip("Text untuk menampilkan operasi matematika.")]
    [SerializeField]
    private TMP_Text questionOperationText;

    [Tooltip("Text untuk menampilkan total jawaban sementara player.")]
    [SerializeField]
    private TMP_Text currentAnswerText;


    // ==================================================
    // PENGATURAN TEKS
    // ==================================================

    [Header("Pengaturan Teks")]

    [Tooltip("Jika aktif, nama makanan menggunakan Bahasa Indonesia.")]
    [SerializeField]
    private bool useIndonesianFoodName = true;


    // ==================================================
    // START
    // ==================================================

    private void Start()
    {
        DisplayCurrentLevel();
    }


    // ==================================================
    // MENAMPILKAN NOMOR LEVEL
    // ==================================================

    public void DisplayCurrentLevel()
    {
        if (levelText == null)
        {
            Debug.LogWarning(
                "UIManager: Level Text belum dihubungkan di Inspector!"
            );

            return;
        }


        string sceneName =
            SceneManager.GetActiveScene().name;


        int levelNumber =
            GetLevelNumberFromSceneName(sceneName);


        levelText.text =
            "Level " + levelNumber;


        Debug.Log(
            "UIManager: Scene aktif = "
            + sceneName
            + " | Level = "
            + levelNumber
        );
    }


    // ==================================================
    // MENGAMBIL NOMOR LEVEL DARI NAMA SCENE
    // ==================================================

    private int GetLevelNumberFromSceneName(
        string sceneName
    )
    {
        int separatorIndex =
            sceneName.LastIndexOf('-');


        if (separatorIndex >= 0)
        {
            string numberText =
                sceneName.Substring(
                    separatorIndex + 1
                );


            if (
                int.TryParse(
                    numberText,
                    out int levelNumber
                )
            )
            {
                return levelNumber;
            }
        }


        Debug.LogWarning(
            "UIManager: Tidak dapat membaca nomor level "
            + "dari nama scene: "
            + sceneName
        );


        return 0;
    }


    // ==================================================
    // MENAMPILKAN SOAL
    // ==================================================

    public void DisplayQuestion(
        int firstNumber,
        int secondNumber,
        FoodType foodType
    )
    {
        string foodName =
            GetFoodName(foodType);


        // ==============================================
        // TAMPILKAN CERITA SOAL
        // ==============================================

        if (questionStoryText != null)
        {
            questionStoryText.text =
                "Aku punya "
                + firstNumber
                + " "
                + foodName
                + " dan mendapat "
                + secondNumber
                + " lagi. Berapa jumlah semuanya?";
        }


        // ==============================================
        // TAMPILKAN OPERASI MATEMATIKA
        // ==============================================

        if (questionOperationText != null)
        {
            questionOperationText.text =
                firstNumber
                + " + "
                + secondNumber
                + " = ?";
        }


        // ==============================================
        // RESET TAMPILAN JAWABAN
        // ==============================================

        UpdateCurrentAnswer(0);


        Debug.Log(
            "UI soal diperbarui | "
            + firstNumber
            + " + "
            + secondNumber
            + " = ?"
            + " | Tema: "
            + foodType
        );
    }


    // ==================================================
    // UPDATE JAWABAN SEMENTARA
    // ==================================================

    public void UpdateCurrentAnswer(
        int currentAnswer
    )
    {
        if (currentAnswerText != null)
        {
            currentAnswerText.text =
                "Jawaban : "
                + currentAnswer;
        }
    }


    // ==================================================
    // MENGAMBIL NAMA MAKANAN
    // ==================================================

    private string GetFoodName(
        FoodType foodType
    )
    {
        if (useIndonesianFoodName)
        {
            switch (foodType)
            {
                case FoodType.Fish:
                    return "ikan";

                case FoodType.Meat:
                    return "daging";

                default:
                    return "makanan";
            }
        }
        else
        {
            switch (foodType)
            {
                case FoodType.Fish:
                    return "fish";

                case FoodType.Meat:
                    return "meat";

                default:
                    return "food";
            }
        }
    }


    // ==================================================
    // RESET SEMUA TAMPILAN
    // ==================================================

    public void ResetUI()
    {
        if (questionStoryText != null)
        {
            questionStoryText.text = "";
        }

        if (questionOperationText != null)
        {
            questionOperationText.text = "";
        }

        if (currentAnswerText != null)
        {
            currentAnswerText.text =
                "Jawaban : 0";
        }
    }
}