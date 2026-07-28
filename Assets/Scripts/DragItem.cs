using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragItem : MonoBehaviour,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler
{
    // ==================================================
    // IDENTITAS ITEM
    // ==================================================

    [Header("Identitas Item")]

    [Tooltip("Jenis makanan: Fish atau Meat.")]
    public FoodType foodType = FoodType.Fish;

    [Tooltip("Nilai item: 1, 10, atau 100.")]
    public PlaceValue placeValue = PlaceValue.One;


    // ==================================================
    // STATUS ITEM
    // ==================================================

    [Header("Status Item")]

    [Tooltip("Centang jika object ini adalah item sumber di kotak hijau.")]
    public bool isSourceItem = true;


    // ==================================================
    // KOMPONEN UI
    // ==================================================

    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;


    // ==================================================
    // DATA INTERNAL
    // ==================================================

    // Clone yang sedang diseret oleh source item.
    private DragItem activeClone;

    // Status apakah item berhasil masuk DropZone.
    private bool isDropped = false;


    // ==================================================
    // AWAKE
    // ==================================================

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        canvas = GetComponentInParent<Canvas>();

        canvasGroup = GetComponent<CanvasGroup>();

        // Tambahkan CanvasGroup otomatis jika belum ada.
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        // Validasi Canvas.
        if (canvas == null)
        {
            Debug.LogError(
                "DragItem pada " + gameObject.name +
                " tidak menemukan Canvas!"
            );
        }
    }


    // ==================================================
    // MULAI DRAG
    // ==================================================

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Hanya item sumber yang dapat membuat clone.
        if (isSourceItem)
        {
            // Mainkan suara ketika item mulai diambil.
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayDragSound();
            }

            // Buat clone untuk diseret.
            CreateClone(eventData);
        }
    }


    // ==================================================
    // SEDANG DRAG
    // ==================================================

    public void OnDrag(PointerEventData eventData)
    {
        if (isSourceItem && activeClone != null)
        {
            activeClone.MoveWithPointer(eventData);
        }
    }


    // ==================================================
    // SELESAI DRAG
    // ==================================================

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isSourceItem && activeClone != null)
        {
            activeClone.EndCloneDrag();

            activeClone = null;
        }
    }


    // ==================================================
    // MEMBUAT CLONE
    // ==================================================

    private void CreateClone(PointerEventData eventData)
    {
        if (canvas == null)
        {
            return;
        }


        // Membuat clone dari source item.
        GameObject cloneObject = Instantiate(
            gameObject,
            canvas.transform
        );


        // Mengambil komponen DragItem dari clone.
        activeClone =
            cloneObject.GetComponent<DragItem>();


        if (activeClone == null)
        {
            Debug.LogError(
                "Clone tidak memiliki komponen DragItem!"
            );

            Destroy(cloneObject);

            return;
        }


        // Clone bukan source.
        activeClone.isSourceItem = false;

        // Clone belum berhasil masuk DropZone.
        activeClone.isDropped = false;

        // Pastikan clone menggunakan Canvas yang sama.
        activeClone.canvas = canvas;

        // Ambil komponen clone.
        activeClone.rectTransform =
            cloneObject.GetComponent<RectTransform>();

        activeClone.canvasGroup =
            cloneObject.GetComponent<CanvasGroup>();


        // Supaya raycast dapat melewati clone
        // dan mendeteksi DropZone di bawahnya.
        activeClone.canvasGroup.blocksRaycasts = false;


        // Tampilkan clone paling depan.
        cloneObject.transform.SetAsLastSibling();


        // Posisikan clone tepat di posisi pointer.
        SetPositionToPointer(
            activeClone.rectTransform,
            eventData
        );


        Debug.Log(
            "Clone dibuat | Food Type: "
            + activeClone.foodType
            + " | Place Value: "
            + activeClone.placeValue
            + " | Nilai: "
            + activeClone.GetItemValue()
        );
    }


    // ==================================================
    // MENGGERAKKAN ITEM
    // ==================================================

    private void MoveWithPointer(
        PointerEventData eventData
    )
    {
        if (rectTransform == null || canvas == null)
        {
            return;
        }

        rectTransform.anchoredPosition +=
            eventData.delta / canvas.scaleFactor;
    }


    // ==================================================
    // POSISI ITEM KE POINTER
    // ==================================================

    private void SetPositionToPointer(
        RectTransform targetRect,
        PointerEventData eventData
    )
    {
        RectTransform canvasRect =
            canvas.transform as RectTransform;


        if (
            RectTransformUtility
                .ScreenPointToLocalPointInRectangle(
                    canvasRect,
                    eventData.position,
                    eventData.pressEventCamera,
                    out Vector2 localPosition
                )
        )
        {
            targetRect.anchoredPosition =
                localPosition;
        }
    }


    // ==================================================
    // SELESAI DRAG CLONE
    // ==================================================

    private void EndCloneDrag()
    {
        canvasGroup.blocksRaycasts = true;


        // Jika tidak masuk ke DropZone,
        // clone dihapus.
        if (!isDropped)
        {
            Debug.Log(
                "Item tidak masuk DropZone, clone dihapus."
            );

            Destroy(gameObject);
        }
    }


    // ==================================================
    // DIPANGGIL OLEH DROPZONE
    // ==================================================

    public void MarkAsDropped()
    {
        isDropped = true;
    }


    // ==================================================
    // MENGAMBIL STATUS DROP
    // ==================================================

    public bool IsDropped()
    {
        return isDropped;
    }


    // ==================================================
    // MENGAMBIL CLONE YANG SEDANG AKTIF
    // ==================================================

    public DragItem GetActiveClone()
    {
        return activeClone;
    }


    // ==================================================
    // MENGAMBIL NILAI ITEM
    // ==================================================

    public int GetItemValue()
    {
        return (int)placeValue;
    }
}