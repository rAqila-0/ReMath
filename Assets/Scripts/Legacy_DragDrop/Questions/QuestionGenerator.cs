using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionGenerator : MonoBehaviour
{
    // ==================================================
    // DATA SATU SOAL
    // ==================================================

    [System.Serializable]
    public class QuestionData
    {
        [Header("Operasi Penjumlahan")]

        [Tooltip("Angka pertama pada soal penjumlahan.")]
        public int firstNumber;

        [Tooltip("Angka kedua pada soal penjumlahan.")]
        public int secondNumber;


        [Header("Tema Makanan")]

        [Tooltip(
            "Jenis makanan yang digunakan pada soal: Fish atau Meat."
        )]
        public FoodType foodType = FoodType.Fish;
    }


    // ==================================================
    // DAFTAR SOAL
    // ==================================================

    [Header("Daftar Soal")]

    [Tooltip(
        "Daftar soal yang dapat dipilih secara acak saat level dimulai."
    )]
    [SerializeField]
    private List<QuestionData> questions =
        new List<QuestionData>();


    // ==================================================
    // REFERENSI MANAGER
    // ==================================================

    [Header("Referensi Manager")]

    [Tooltip("GameManager yang menerima data soal aktif.")]
    [SerializeField]
    private GameManager gameManager;

    [Tooltip("UIManager untuk menampilkan soal ke layar.")]
    [SerializeField]
    private UIManager uiManager;


    // ==================================================
    // ITEM SUMBER IKAN
    // ==================================================

    [Header("Item Sumber - Fish")]

    [Tooltip("GameObject ikan biasa bernilai 1.")]
    [SerializeField]
    private GameObject fishOneItem;

    [Tooltip("GameObject kardus ikan bernilai 10.")]
    [SerializeField]
    private GameObject fishTenItem;

    [Tooltip("GameObject keranjang ikan bernilai 100.")]
    [SerializeField]
    private GameObject fishHundredItem;


    // ==================================================
    // ITEM SUMBER DAGING
    // ==================================================

    [Header("Item Sumber - Meat")]

    [Tooltip("GameObject daging biasa bernilai 1.")]
    [SerializeField]
    private GameObject meatOneItem;

    [Tooltip("GameObject tumpukan daging di piring bernilai 10.")]
    [SerializeField]
    private GameObject meatTenItem;

    [Tooltip("GameObject peti daging bernilai 100.")]
    [SerializeField]
    private GameObject meatHundredItem;


    // ==================================================
    // DATA SOAL AKTIF
    // ==================================================

    private QuestionData currentQuestion;


    // ==================================================
    // START
    // ==================================================

    private void Start()
    {
        GenerateRandomQuestion();
    }


    // ==================================================
    // GENERATE SOAL RANDOM
    // ==================================================

    public void GenerateRandomQuestion()
    {
        // Pastikan daftar soal tidak kosong.
        if (questions == null || questions.Count == 0)
        {
            Debug.LogError(
                "QuestionGenerator: Daftar soal masih kosong!"
            );

            return;
        }


        // Pilih index secara acak.
        int randomIndex =
            Random.Range(0, questions.Count);


        // Simpan soal yang terpilih.
        currentQuestion =
            questions[randomIndex];


        // Validasi soal.
        if (currentQuestion == null)
        {
            Debug.LogError(
                "QuestionGenerator: Soal yang terpilih bernilai null!"
            );

            return;
        }


        // ==============================================
        // KIRIM SOAL KE GAME MANAGER
        // ==============================================

        if (gameManager != null)
        {
            gameManager.StartLevel(
                currentQuestion.firstNumber,
                currentQuestion.secondNumber
            );
        }
        else
        {
            Debug.LogError(
                "QuestionGenerator: GameManager belum dihubungkan "
                + "di Inspector!"
            );
        }


        // ==============================================
        // TAMPILKAN SOAL DI UI
        // ==============================================

        if (uiManager != null)
        {
            uiManager.DisplayQuestion(
                currentQuestion.firstNumber,
                currentQuestion.secondNumber,
                currentQuestion.foodType
            );
        }
        else
        {
            Debug.LogWarning(
                "QuestionGenerator: UIManager belum dihubungkan "
                + "di Inspector!"
            );
        }


        // ==============================================
        // AKTIFKAN ITEM SESUAI TEMA SOAL
        // ==============================================

        SetActiveFoodItems(
            currentQuestion.foodType
        );


        // ==============================================
        // DEBUG
        // ==============================================

        Debug.Log(
            "Soal random terpilih | "
            + currentQuestion.firstNumber
            + " + "
            + currentQuestion.secondNumber
            + " = "
            + GetCurrentAnswer()
            + " | Tema: "
            + currentQuestion.foodType
        );
    }


    // ==================================================
    // MENGATUR ITEM SESUAI TEMA SOAL
    // ==================================================

    private void SetActiveFoodItems(
        FoodType activeFoodType
    )
    {
        bool useFish =
            activeFoodType == FoodType.Fish;


        // ==============================================
        // ITEM IKAN
        // ==============================================

        SetObjectActive(
            fishOneItem,
            useFish
        );

        SetObjectActive(
            fishTenItem,
            useFish
        );

        SetObjectActive(
            fishHundredItem,
            useFish
        );


        // ==============================================
        // ITEM DAGING
        // ==============================================

        SetObjectActive(
            meatOneItem,
            !useFish
        );

        SetObjectActive(
            meatTenItem,
            !useFish
        );

        SetObjectActive(
            meatHundredItem,
            !useFish
        );


        Debug.Log(
            "Tema item aktif: "
            + activeFoodType
        );
    }


    // ==================================================
    // HELPER SET ACTIVE
    // ==================================================

    private void SetObjectActive(
        GameObject targetObject,
        bool isActive
    )
    {
        if (targetObject != null)
        {
            targetObject.SetActive(isActive);
        }
    }


    // ==================================================
    // MENGAMBIL SOAL AKTIF
    // ==================================================

    public QuestionData GetCurrentQuestion()
    {
        return currentQuestion;
    }


    // ==================================================
    // MENGAMBIL ANGKA PERTAMA
    // ==================================================

    public int GetCurrentFirstNumber()
    {
        if (currentQuestion == null)
        {
            return 0;
        }

        return currentQuestion.firstNumber;
    }


    // ==================================================
    // MENGAMBIL ANGKA KEDUA
    // ==================================================

    public int GetCurrentSecondNumber()
    {
        if (currentQuestion == null)
        {
            return 0;
        }

        return currentQuestion.secondNumber;
    }


    // ==================================================
    // MENGAMBIL JAWABAN BENAR
    // ==================================================

    public int GetCurrentAnswer()
    {
        if (currentQuestion == null)
        {
            return 0;
        }

        return
            currentQuestion.firstNumber
            + currentQuestion.secondNumber;
    }


    // ==================================================
    // MENGAMBIL TEMA MAKANAN AKTIF
    // ==================================================

    public FoodType GetCurrentFoodType()
    {
        if (currentQuestion == null)
        {
            return FoodType.Fish;
        }

        return currentQuestion.foodType;
    }
}