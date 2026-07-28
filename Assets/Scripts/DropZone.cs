using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler
{
    // ==================================================
    // REFERENSI MANAGER
    // ==================================================

    [Header("Referensi Manager")]

    [Tooltip("GameManager untuk menambahkan nilai item ke jawaban.")]
    [SerializeField]
    private GameManager gameManager;

    [Tooltip("QuestionGenerator untuk mengecek tema makanan aktif.")]
    [SerializeField]
    private QuestionGenerator questionGenerator;

    [Tooltip("UndoManager untuk mencatat item yang berhasil masuk.")]
    [SerializeField]
    private UndoManager undoManager;


    // ==================================================
    // REFERENSI AREA JAWABAN
    // ==================================================

    [Header("Area Jawaban")]

    [Tooltip(
        "Transform tempat clone item diletakkan setelah berhasil di-drop. " +
        "Biasanya isi dengan GameObject AnswerContent."
    )]
    [SerializeField]
    private Transform answerContent;


    // ==================================================
    // PENGATURAN DROP
    // ==================================================

    [Header("Pengaturan Drop")]

    [Tooltip(
        "Jika aktif, item dengan tema yang tidak sesuai soal akan ditolak."
    )]
    [SerializeField]
    private bool validateFoodType = true;


    // ==================================================
    // AWAKE
    // ==================================================

    private void Awake()
    {
        // Cari GameManager otomatis jika belum dihubungkan.
        if (gameManager == null)
        {
            gameManager = GameManager.Instance;
        }

        // Jika Answer Content belum diisi,
        // gunakan Transform DropZone ini sendiri.
        if (answerContent == null)
        {
            answerContent = transform;
        }
    }


    // ==================================================
    // ITEM DI-DROP KE AREA JAWABAN
    // ==================================================

    public void OnDrop(PointerEventData eventData)
    {
        // Pastikan object yang memulai drag tersedia.
        if (eventData.pointerDrag == null)
        {
            Debug.LogWarning(
                "DropZone: pointerDrag tidak ditemukan."
            );

            return;
        }


        // Ambil DragItem dari item sumber.
        DragItem sourceDragItem =
            eventData.pointerDrag.GetComponent<DragItem>();


        if (sourceDragItem == null)
        {
            Debug.LogWarning(
                "DropZone: Object yang di-drag tidak memiliki DragItem."
            );

            return;
        }


        // ==============================================
        // AMBIL CLONE YANG SEDANG AKTIF
        // ==============================================

        DragItem droppedItem =
            sourceDragItem.GetActiveClone();


        if (droppedItem == null)
        {
            Debug.LogWarning(
                "DropZone: Clone aktif tidak ditemukan."
            );

            return;
        }


        // ==============================================
        // VALIDASI TEMA MAKANAN
        // ==============================================

        if (validateFoodType && questionGenerator != null)
        {
            FoodType activeFoodType =
                questionGenerator.GetCurrentFoodType();


            if (droppedItem.foodType != activeFoodType)
            {
                Debug.LogWarning(
                    "Item ditolak | Tema soal: "
                    + activeFoodType
                    + " | Item yang dimasukkan: "
                    + droppedItem.foodType
                );

                return;
            }
        }


        // ==============================================
        // AMBIL NILAI ITEM
        // ==============================================

        int itemValue =
            droppedItem.GetItemValue();


        // Validasi nilai.
        if (
            itemValue != 1 &&
            itemValue != 10 &&
            itemValue != 100
        )
        {
            Debug.LogWarning(
                "DropZone: Nilai item tidak valid: "
                + itemValue
            );

            return;
        }


        // ==============================================
        // PINDAHKAN CLONE KE ANSWER CONTENT
        // ==============================================

        droppedItem.transform.SetParent(
            answerContent,
            true
        );


        // ==============================================
        // TANDAI ITEM BERHASIL DI-DROP
        // ==============================================

        droppedItem.MarkAsDropped();

        // ==============================================
        // MAINKAN SUARA DROP
        // ==============================================

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayDropSound();
        }


        // ==============================================
        // TAMBAHKAN NILAI KE GAME MANAGER
        // ==============================================

        if (gameManager == null)
        {
            gameManager = GameManager.Instance;
        }


        if (gameManager != null)
        {
            gameManager.AddValueToAnswer(
                itemValue
            );
        }
        else
        {
            Debug.LogError(
                "DropZone: GameManager tidak ditemukan!"
            );

            return;
        }


        // ==============================================
        // DAFTARKAN ITEM KE UNDO MANAGER
        // ==============================================

        if (undoManager != null)
        {
            undoManager.RegisterItem(
                droppedItem
            );
        }


        // ==============================================
        // DEBUG
        // ==============================================

        Debug.Log(
            "Item berhasil masuk DropZone"
            + " | Food Type: "
            + droppedItem.foodType
            + " | Place Value: "
            + droppedItem.placeValue
            + " | Nilai: "
            + itemValue
            + " | Current Answer: "
            + gameManager.GetCurrentAnswer()
        );
    }
}