using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UndoManager : MonoBehaviour
{
    // ==================================================
    // REFERENSI MANAGER
    // ==================================================

    [Header("Referensi Manager")]

    [Tooltip("GameManager untuk mengurangi atau mereset nilai jawaban.")]
    [SerializeField]
    private GameManager gameManager;


    // ==================================================
    // REFERENSI AREA JAWABAN
    // ==================================================

    [Header("Area Jawaban")]

    [Tooltip(
        "Transform tempat semua clone item jawaban disimpan. " +
        "Hubungkan dengan AnswerContent."
    )]
    [SerializeField]
    private Transform answerContent;


    // ==================================================
    // RIWAYAT ITEM
    // ==================================================

    private Stack<DragItem> itemHistory =
        new Stack<DragItem>();


    // ==================================================
    // AWAKE
    // ==================================================

    private void Awake()
    {
        if (gameManager == null)
        {
            gameManager = GameManager.Instance;
        }
    }


    // ==================================================
    // MENDAFTARKAN ITEM
    // ==================================================

    public void RegisterItem(DragItem item)
    {
        if (item == null)
        {
            Debug.LogWarning(
                "UndoManager: Item yang ingin didaftarkan bernilai null."
            );

            return;
        }


        itemHistory.Push(item);


        Debug.Log(
            "Item didaftarkan ke UndoManager"
            + " | Food Type: "
            + item.foodType
            + " | Place Value: "
            + item.placeValue
            + " | Nilai: "
            + item.GetItemValue()
            + " | Jumlah history: "
            + itemHistory.Count
        );
    }


    // ==================================================
    // UNDO ITEM TERAKHIR
    // ==================================================

    public void UndoLastItem()
    {
        // Tidak ada item yang bisa di-undo.
        if (itemHistory.Count == 0)
        {
            Debug.Log(
                "UndoManager: Tidak ada item yang dapat di-undo."
            );

            return;
        }


        // Ambil item terakhir.
        DragItem lastItem =
            itemHistory.Pop();


        // Lewati item null jika ada.
        while (
            lastItem == null &&
            itemHistory.Count > 0
        )
        {
            lastItem = itemHistory.Pop();
        }


        if (lastItem == null)
        {
            Debug.LogWarning(
                "UndoManager: Tidak ditemukan item valid untuk di-undo."
            );

            return;
        }


        // Ambil data sebelum item dihancurkan.
        int itemValue =
            lastItem.GetItemValue();

        FoodType foodType =
            lastItem.foodType;

        PlaceValue placeValue =
            lastItem.placeValue;


        // Cari GameManager jika referensi belum tersedia.
        if (gameManager == null)
        {
            gameManager = GameManager.Instance;
        }


        if (gameManager == null)
        {
            Debug.LogError(
                "UndoManager: GameManager tidak ditemukan!"
            );

            return;
        }


        // Kurangi nilai jawaban.
        gameManager.RemoveValueFromAnswer(
            itemValue
        );


        // Hapus item visual.
        Destroy(lastItem.gameObject);


        // ==============================================
        // MAINKAN SUARA UNDO
        // ==============================================

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayUndoSound();
        }


        Debug.Log(
            "UNDO berhasil"
            + " | Food Type: "
            + foodType
            + " | Place Value: "
            + placeValue
            + " | Nilai dikurangi: "
            + itemValue
            + " | Current Answer: "
            + gameManager.GetCurrentAnswer()
        );
    }


    // ==================================================
    // RESET SEMUA ITEM
    // ==================================================

    public void ResetAllItems()
    {
        // Cek apakah memang ada jawaban sebelum reset.
        bool hasAnswer =
            itemHistory.Count > 0;


        // Kosongkan riwayat.
        itemHistory.Clear();


        // Hapus semua item di AnswerContent.
        if (answerContent != null)
        {
            for (
                int i = answerContent.childCount - 1;
                i >= 0;
                i--
            )
            {
                Transform child =
                    answerContent.GetChild(i);

                if (child != null)
                {
                    Destroy(child.gameObject);
                }
            }
        }
        else
        {
            Debug.LogWarning(
                "UndoManager: AnswerContent belum dihubungkan "
                + "di Inspector."
            );
        }


        // Cari GameManager jika diperlukan.
        if (gameManager == null)
        {
            gameManager = GameManager.Instance;
        }


        if (gameManager != null)
        {
            gameManager.ResetAnswer();
        }
        else
        {
            Debug.LogError(
                "UndoManager: GameManager tidak ditemukan!"
            );

            return;
        }


        // ==============================================
        // MAINKAN SUARA RESET
        // Hanya jika sebelumnya memang ada jawaban.
        // ==============================================

        if (
            hasAnswer &&
            AudioManager.Instance != null
        )
        {
            AudioManager.Instance.PlayResetSound();
        }


        Debug.Log(
            "RESET berhasil"
            + " | Semua item dihapus"
            + " | Current Answer = 0"
        );
    }


    // ==================================================
    // MENGAMBIL JUMLAH HISTORY
    // ==================================================

    public int GetHistoryCount()
    {
        return itemHistory.Count;
    }


    // ==================================================
    // CEK APAKAH BISA UNDO
    // ==================================================

    public bool CanUndo()
    {
        return itemHistory.Count > 0;
    }
}