using UnityEngine;

namespace MoreSaveSlots
{
    internal class SaveSlotsUI
    {
        internal const StartMenuButtonType PREVIOUS_BUTTON = (StartMenuButtonType)13;
        internal const StartMenuButtonType NEXT_BUTTON = (StartMenuButtonType)14;
        internal static TextMesh PageNumTextMesh { get; private set; }

        internal static void InitializeUI(GameObject saveSlotUI)
        {
            // move open save folder button up
            saveSlotUI.transform.GetChild(12).localPosition = new Vector3(1.157f, 1.12f, 0.033f);

            var previousButton = GameObject.Instantiate(saveSlotUI.transform.GetChild(10));
            previousButton.name = "previous button";
            previousButton.SetParent(saveSlotUI.transform);
            previousButton.localPosition = new Vector3(-1.45f, 0.5f, 0f);
            previousButton.localEulerAngles = new Vector3(0f, 180f, 0f);
            previousButton.localScale = new Vector3(1f, 1f, 1f);
            previousButton.GetChild(1).GetComponent<TextMesh>().text = "Previous";
            previousButton.GetChild(2).GetComponent<TextMesh>().text = "←";
            previousButton.GetChild(0).GetComponent<StartMenuButton>().SetPrivateField("type", PREVIOUS_BUTTON);

            var nextButton = GameObject.Instantiate(saveSlotUI.transform.GetChild(10));
            nextButton.name = "next button";
            nextButton.SetParent(saveSlotUI.transform);
            nextButton.localPosition = new Vector3(1.45f, 0.5f, 0f);
            nextButton.localEulerAngles = new Vector3(0f, 180f, 0f);
            nextButton.localScale = new Vector3(1f, 1f, 1f);
            nextButton.GetChild(1).GetComponent<TextMesh>().text = "Next";
            nextButton.GetChild(2).GetComponent<TextMesh>().text = "→";
            nextButton.GetChild(0).GetComponent<StartMenuButton>().SetPrivateField("type", NEXT_BUTTON);

            var pageNumText = GameObject.Instantiate(saveSlotUI.transform.GetChild(3));
            pageNumText.name = "page num text";
            pageNumText.SetParent(saveSlotUI.transform);
            pageNumText.localPosition = new Vector3(0f, 0.5f, 0f);
            pageNumText.localEulerAngles = new Vector3(0f, 0f, 0f);
            pageNumText.localScale = new Vector3(0.0165f, 0.0177f, 0.0177f);
            PageNumTextMesh = pageNumText.GetComponent<TextMesh>();
            PageNumTextMesh.text = "page 1";
        }

        internal static void UpdatePageNumText(int pageNum)
        {
            PageNumTextMesh.text = $"page {pageNum + 1}";
        }
    }
}
