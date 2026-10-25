using TMPro;
using UnityEngine;

public class AnswerSlotGroup : MonoBehaviour
{
    public const int MaxSupportedSlots = 4;

    [Header("Digit Texts - Left to Right (4 slots)")]
    [SerializeField]
    private TMP_Text[] digitTexts = new TMP_Text[MaxSupportedSlots];

    [Header("Active Frames - Left to Right (same length as Digit Texts)")]
    [SerializeField]
    private GameObject[] activeFrames = new GameObject[MaxSupportedSlots];

    /// <summary>
    /// Capacity follows the arrays assigned in the Inspector, with a maximum
    /// of four slots. This is enough for 99 x 99 = 9801.
    /// </summary>
    public int SlotCapacity
    {
        get
        {
            int textCount = digitTexts != null ? digitTexts.Length : 0;
            int frameCount = activeFrames != null ? activeFrames.Length : 0;

            if (textCount <= 0) return Mathf.Clamp(frameCount, 0, MaxSupportedSlots);
            if (frameCount <= 0) return Mathf.Clamp(textCount, 0, MaxSupportedSlots);

            return Mathf.Clamp(Mathf.Min(textCount, frameCount), 0, MaxSupportedSlots);
        }
    }

    public void ClearAll()
    {
        if (digitTexts != null)
        {
            for (int i = 0; i < digitTexts.Length; i++)
            {
                if (digitTexts[i] != null)
                    digitTexts[i].text = string.Empty;
            }
        }

        ClearActiveSlot();
    }

    public void SetDigit(int index, string value)
    {
        if (digitTexts == null || index < 0 || index >= digitTexts.Length || index >= MaxSupportedSlots)
            return;

        if (digitTexts[index] != null)
            digitTexts[index].text = value;
    }

    public void ClearDigit(int index)
    {
        SetDigit(index, string.Empty);
    }

    public void SetActiveSlot(int activeIndex)
    {
        if (activeFrames == null)
            return;

        for (int i = 0; i < activeFrames.Length; i++)
        {
            if (activeFrames[i] != null)
                activeFrames[i].SetActive(i == activeIndex);
        }
    }

    public void ClearActiveSlot()
    {
        if (activeFrames == null)
            return;

        for (int i = 0; i < activeFrames.Length; i++)
        {
            if (activeFrames[i] != null)
                activeFrames[i].SetActive(false);
        }
    }
}
