using TMPro;
using UnityEngine;

public class AnswerSlotGroup : MonoBehaviour
{
    //==================================================
    // SETTINGS
    //==================================================

    public const int SlotCount = 4;


    //==================================================
    // DIGIT TEXT
    //==================================================

    [Header("Digit Texts - Left to Right")]

    [SerializeField]
    private TMP_Text[] digitTexts =
        new TMP_Text[SlotCount];


    //==================================================
    // ACTIVE FRAME
    //==================================================

    [Header("Active Frames - Left to Right")]

    [SerializeField]
    private GameObject[] activeFrames =
        new GameObject[SlotCount];


    //==================================================
    // CLEAR ALL
    //==================================================

    public void ClearAll()
    {
        for (int i = 0; i < SlotCount; i++)
        {
            if (
                digitTexts != null &&
                i < digitTexts.Length &&
                digitTexts[i] != null
            )
            {
                digitTexts[i].text = "";
            }
        }


        ClearActiveSlot();
    }


    //==================================================
    // SET DIGIT
    //==================================================

    public void SetDigit(
        int index,
        string value
    )
    {
        if (
            index < 0 ||
            index >= SlotCount
        )
        {
            return;
        }


        if (
            digitTexts == null ||
            index >= digitTexts.Length
        )
        {
            return;
        }


        if (digitTexts[index] == null)
            return;


        digitTexts[index].text =
            value;
    }


    //==================================================
    // CLEAR DIGIT
    //==================================================

    public void ClearDigit(
        int index
    )
    {
        SetDigit(
            index,
            ""
        );
    }


    //==================================================
    // SET ACTIVE SLOT
    //==================================================

    public void SetActiveSlot(
        int activeIndex
    )
    {
        if (activeFrames == null)
            return;


        for (
            int i = 0;
            i < activeFrames.Length;
            i++
        )
        {
            if (activeFrames[i] == null)
                continue;


            activeFrames[i].SetActive(
                i == activeIndex
            );
        }
    }


    //==================================================
    // CLEAR ACTIVE SLOT
    //==================================================

    public void ClearActiveSlot()
    {
        if (activeFrames == null)
            return;


        foreach (
            GameObject frame
            in activeFrames
        )
        {
            if (frame != null)
            {
                frame.SetActive(false);
            }
        }
    }
}