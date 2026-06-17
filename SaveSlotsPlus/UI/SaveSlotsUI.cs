using HarmonyLib;
using System.IO;
using UnityEngine;
using static SaveSlotsPlus.SSP_Plugin;

namespace SaveSlotsPlus
{
    internal class SaveSlotsUI
    {
        internal const StartMenuButtonType PREVIOUS_BUTTON = (StartMenuButtonType)13;
        internal const StartMenuButtonType NEXT_BUTTON = (StartMenuButtonType)14;
        internal const StartMenuButtonType RENAME_BUTTON = (StartMenuButtonType)15;
        internal const StartMenuButtonType CONFIRM_RENAME_BUTTON = (StartMenuButtonType)16;
        internal const StartMenuButtonType COPY_BUTTON = (StartMenuButtonType)17;
        internal const StartMenuButtonType PASTE_BUTTON = (StartMenuButtonType)18;
        internal const StartMenuButtonType DELETE_BUTTON = (StartMenuButtonType)19;
        internal const StartMenuButtonType CONFIRM_DELETE_BUTTON = (StartMenuButtonType)20;

        internal static TextMesh PageNumTextMesh { get; private set; }        
        internal static Transform FileMenuList { get; private set; }

        private static Transform _renameScroll;
        private static Transform _confirmRenameButton;
        private static Transform _previousButton;
        private static Transform _nextButton;
        private static bool _fileMenuShowing = false;
        private static Transform _list = null;
        private static bool _listWasInactive = false;
        private static bool _copyClicked = false;

        private static readonly Color32 ACTIVE_COLOR = new Color32(0x11, 0x11, 0x13, 0xFF);
        private static readonly Color32 INACTIVE_COLOR = new Color32(0x11, 0x11, 0x13, 0x66);

        private static Material _originalSlotButtonMaterial;
        private const int ORIGINAL_FONT_SIZE = 50;
        private static readonly Vector3 ORIGINAL_FONT_POSITION = new Vector3(0f, 0.0225f, 0.004f);

        internal static void InitializeUI(GameObject saveSlotUI, BackupSavesListUI backupSavesUI)
        {
            _originalSlotButtonMaterial = saveSlotUI.transform.GetChild(2).GetChild(0).GetComponent<MeshRenderer>().material;

            // move open save folder button up
            saveSlotUI.transform.GetChild(12).localPosition = new Vector3(1.157f, 1.12f, 0.033f);

            // previous button
            _previousButton = GameObject.Instantiate(saveSlotUI.transform.GetChild(10), saveSlotUI.transform);
            _previousButton.name = "previous button";
            _previousButton.localPosition = new Vector3(-1.45f, 0.5f, 0f);
            _previousButton.localEulerAngles = new Vector3(0f, 180f, 0f);
            _previousButton.localScale = new Vector3(1f, 1f, 1f);
            _previousButton.GetChild(1).GetComponent<TextMesh>().text = "previous";
            _previousButton.GetChild(2).GetComponent<TextMesh>().text = "←";
            _previousButton.GetChild(0).GetComponent<StartMenuButton>().SetPrivateField("type", PREVIOUS_BUTTON);
            SetChangePageTextColor(PREVIOUS_BUTTON, false);

            // next button
            _nextButton = GameObject.Instantiate(saveSlotUI.transform.GetChild(10), saveSlotUI.transform);
            _nextButton.name = "next button";
            _nextButton.localPosition = new Vector3(1.45f, 0.5f, 0f);
            _nextButton.localEulerAngles = new Vector3(0f, 180f, 0f);
            _nextButton.localScale = new Vector3(1f, 1f, 1f);
            _nextButton.GetChild(1).GetComponent<TextMesh>().text = "next";
            _nextButton.GetChild(2).GetComponent<TextMesh>().text = "→";
            _nextButton.GetChild(0).GetComponent<StartMenuButton>().SetPrivateField("type", NEXT_BUTTON);

            // confirm rename button
            _confirmRenameButton = GameObject.Instantiate(saveSlotUI.transform.GetChild(10), saveSlotUI.transform);
            _confirmRenameButton.name = "confirm rename button";
            _confirmRenameButton.localPosition = new Vector3(0f, -0.2f, 0f);
            _confirmRenameButton.localEulerAngles = new Vector3(0f, 180f, 0f);
            _confirmRenameButton.localScale = new Vector3(0.9f, 0.9f, 0.9f);
            var crbText = _confirmRenameButton.GetChild(2);
            crbText.localScale = new Vector3(0.0135f, 0.0135f, 0.0135f);
            crbText.localPosition = new Vector3(0.0028f, 0.025f, 0.0009f);
            var crbTextMesh = crbText.GetComponent<TextMesh>();
            crbTextMesh.text = "confirm\nrename";
            crbTextMesh.fontSize = 60;            
            _confirmRenameButton.GetChild(0).GetComponent<StartMenuButton>().SetPrivateField("type", CONFIRM_RENAME_BUTTON);
            GameObject.Destroy(_confirmRenameButton.GetChild(1).gameObject);
            _confirmRenameButton.gameObject.SetActive(false);

            var textTemplate = saveSlotUI.transform.GetChild(3);

            // page num text
            var pageNumText = GameObject.Instantiate(textTemplate, saveSlotUI.transform);
            pageNumText.name = "page num text";
            pageNumText.localPosition = new Vector3(0f, 0.5f, 0f);
            pageNumText.localEulerAngles = new Vector3(0f, 0f, 0f);
            pageNumText.localScale = new Vector3(0.0165f, 0.0177f, 0.0177f);
            PageNumTextMesh = pageNumText.GetComponent<TextMesh>();
            PageNumTextMesh.text = "page 1";

            // rename scroll
            _renameScroll = GameObject.Instantiate(saveSlotUI.transform.GetChild(0), saveSlotUI.transform);
            _renameScroll.name = "bg rename";
            _renameScroll.transform.localScale = new Vector3(0.6f, 4f, 0.42f);
            _renameScroll.transform.localPosition = new Vector3(0.15f, 0.087f, 0.06f);

            // rename label
            var label = GameObject.Instantiate(textTemplate, _renameScroll);
            label.name = "label";
            label.transform.localPosition = new Vector3(-0.17f, 0.04f, 0.1f);
            label.transform.localEulerAngles = new Vector3(0f, 180f, 270f);
            label.transform.localScale = new Vector3(0.004f, 0.035f, 0.0194f);
            var labelMesh = label.GetComponent<TextMesh>();
            labelMesh.text = "name your save:";
            labelMesh.anchor = TextAnchor.MiddleCenter;
            labelMesh.alignment = TextAlignment.Center;

            // rename input
            var input = GameObject.Instantiate(textTemplate, _renameScroll);
            input.name = "input text";
            input.transform.localPosition = new Vector3(0.05f, 0.04f, 0.1f);
            input.transform.localEulerAngles = new Vector3(0f, 180f, 270f);
            input.transform.localScale = new Vector3(0.005f, 0.035f, 0.0194f);
            var inputMesh = input.GetComponent<TextMesh>();
            inputMesh.text = "";
            inputMesh.anchor = TextAnchor.MiddleCenter;
            inputMesh.alignment = TextAlignment.Center;
            inputMesh.fontStyle = FontStyle.Normal;

            FileMenu.SaveNameInputText = _renameScroll.gameObject.AddComponent<SaveNameInput>();
            FileMenu.SaveNameInputText.displayMesh = inputMesh;

            _renameScroll.gameObject.SetActive(false);

            FileMenuList = GameObject.Instantiate(backupSavesUI.list, backupSavesUI.list.parent);
            FileMenuList.name = "file menu list";
            FileMenuList.GetChild(6).GetComponent<TextMesh>().text = "file menu";

            // rename button
            var renameButton = FileMenuList.GetChild(0);
            renameButton.name = "rename button";
            renameButton.GetChild(1).GetComponent<TextMesh>().text = "rename";
            renameButton.GetChild(0).GetComponent<StartMenuButton>().SetPrivateField("type", RENAME_BUTTON);

            // copy button
            var copyButton = FileMenuList.GetChild(1);
            copyButton.name = "copy button";
            copyButton.GetChild(1).GetComponent<TextMesh>().text = "copy";
            copyButton.GetChild(0).GetComponent<StartMenuButton>().SetPrivateField("type", COPY_BUTTON);

            // paste button
            var pasteButton = FileMenuList.GetChild(2);
            pasteButton.name = "paste button";
            pasteButton.GetChild(1).GetComponent<TextMesh>().text = "paste";
            pasteButton.GetChild(0).GetComponent<StartMenuButton>().SetPrivateField("type", PASTE_BUTTON);

            // delete button
            var deleteButton = FileMenuList.GetChild(3);
            deleteButton.name = "delete button";
            deleteButton.GetChild(1).GetComponent<TextMesh>().text = "delete";
            deleteButton.GetChild(0).GetComponent<StartMenuButton>().SetPrivateField("type", DELETE_BUTTON);

            // confirm delete button
            var confirmDeleteButton = FileMenuList.GetChild(4);
            confirmDeleteButton.name = "confirm delete button";
            confirmDeleteButton.GetChild(1).GetComponent<TextMesh>().text = "confirm\ndelete";
            confirmDeleteButton.GetChild(0).GetComponent<StartMenuButton>().SetPrivateField("type", CONFIRM_DELETE_BUTTON);
        }        

        internal static void ShowRenameInput()
        {
            _renameScroll.gameObject.SetActive(true);
            FileMenu.SaveNameInputText.Activate();
            _confirmRenameButton.gameObject.SetActive(true);
            HideFileMenu();
        }

        internal static void HideRenameInput()
        {
            _renameScroll.gameObject.SetActive(false);
            FileMenu.SaveNameInputText.Deactivate();
            _confirmRenameButton.gameObject.SetActive(false);
        }

        internal static void HideFileMenu()
        {
            _fileMenuShowing = false;
            _listWasInactive = false;
            FileMenuList.gameObject.SetActive(false);
        }

        internal static void ToggleFileMenu(StartMenuButton startMenuButton)
        {
            LogDebug("Toggling file menu");
            if (_list == null)
            {
                _list = startMenuButton.GetPrivateField<StartMenu>("menu").backupSavesUI.transform.GetChild(0);
            }

            if (!_fileMenuShowing)
            {
                _fileMenuShowing = true;
                FileMenuList.gameObject.SetActive(true);
                if (SaveSlots.slotsActive[startMenuButton.saveSlot])
                {
                    SetActiveSaveSlotTextColors();
                }
                else
                {
                    SetNotActiveSaveSlotColors();
                }

                if (!_list.gameObject.activeInHierarchy)
                {
                    _listWasInactive = true;
                    return;
                }                    

                _list.gameObject.SetActive(false);
            }
            else
            {
                _fileMenuShowing = false;
                FileMenuList.gameObject.SetActive(false);

                if (_listWasInactive)
                {
                    _list.gameObject.SetActive(false);
                    _listWasInactive = false;
                    return;
                }
                
                _list.gameObject.SetActive(true);
            }
        }

        private static void SetActiveSaveSlotTextColors()
        {
            FileMenuList.GetChild(0).GetChild(1).GetComponent<TextMesh>().color = ACTIVE_COLOR;
            FileMenuList.GetChild(1).GetChild(1).GetComponent<TextMesh>().color = ACTIVE_COLOR;
            FileMenuList.GetChild(2).GetChild(1).GetComponent<TextMesh>().color = INACTIVE_COLOR;
            FileMenuList.GetChild(3).GetChild(1).GetComponent<TextMesh>().color = ACTIVE_COLOR;
            FileMenuList.GetChild(4).GetChild(1).GetComponent<TextMesh>().color = INACTIVE_COLOR;
        }

        private static void SetNotActiveSaveSlotColors()
        {
            FileMenuList.GetChild(0).GetChild(1).GetComponent<TextMesh>().color = INACTIVE_COLOR;
            FileMenuList.GetChild(1).GetChild(1).GetComponent<TextMesh>().color = INACTIVE_COLOR;
            FileMenuList.GetChild(2).GetChild(1).GetComponent<TextMesh>().color = _copyClicked ? ACTIVE_COLOR : INACTIVE_COLOR;
            FileMenuList.GetChild(3).GetChild(1).GetComponent<TextMesh>().color = INACTIVE_COLOR;
            FileMenuList.GetChild(4).GetChild(1).GetComponent<TextMesh>().color = INACTIVE_COLOR;
        }

        internal static void SetChangePageTextColor(StartMenuButtonType type, bool isActive, bool changeBoth = false)
        {
            var t = type == PREVIOUS_BUTTON ? _previousButton : _nextButton;
            var color = isActive ? ACTIVE_COLOR : INACTIVE_COLOR;
            TextMesh[] textMeshes;
            if (!changeBoth)
            {
                textMeshes = t.GetComponentsInChildren<TextMesh>();
            }
            else
            {
                textMeshes = _previousButton.GetComponentsInChildren<TextMesh>();
                textMeshes.AddRangeToArray(_nextButton.GetComponentsInChildren<TextMesh>());
            }

            foreach (var textMesh in textMeshes)
            {
                textMesh.color = color;
            }
        }

        internal static void EnableConfirmDeleteButton()
        {
            FileMenuList.GetChild(4).GetChild(1).GetComponent<TextMesh>().color = ACTIVE_COLOR;
        }

        internal static void UpdatePageNumText(int pageNum)
        {
            PageNumTextMesh.text = $"page {pageNum + 1}";
        }

        internal static void CopyButtonClicked()
        {
            _copyClicked = true;
        }

        internal static void ClearCopyClicked()
        {
            _copyClicked = false;
        }

        internal static void CopyMaterial(int fromSlot, int toSlot)
        {
            var path = SlotPath.SlotScreenshotPath(fromSlot);
            var fromButton = Paginator.GetStartMenuButtonForSlot(fromSlot);
            var toButton = Paginator.GetStartMenuButtonForSlot(toSlot);            
            var toRenderer = toButton.GetComponent<MeshRenderer>();
            var toTextMesh = toButton.transform.parent.GetComponentInChildren<TextMesh>();

            if (File.Exists(path))
            {
                toRenderer.material = fromButton.GetComponent<MeshRenderer>().material;
                toTextMesh.color = new Color(1f, 0.8f, 0.6f);
                toTextMesh.fontSize = 45;
                toTextMesh.transform.localPosition = new Vector3(0f, -0.1f, 0.04f);
            }
            else
            {
                toButton.GetComponent<MeshRenderer>().material = _originalSlotButtonMaterial;
                toTextMesh.color = ACTIVE_COLOR;
                toTextMesh.fontSize = ORIGINAL_FONT_SIZE;
                toTextMesh.transform.localPosition = ORIGINAL_FONT_POSITION;
            }
        }

        internal static void ResetMaterial(int slot)
        {
            var button = Paginator.GetStartMenuButtonForSlot(slot);
            button.GetComponent<MeshRenderer>().material = _originalSlotButtonMaterial;

            var buttonTextMesh = button.transform.parent.GetComponentInChildren<TextMesh>();
            buttonTextMesh.color = ACTIVE_COLOR;
            buttonTextMesh.fontSize = ORIGINAL_FONT_SIZE;
            buttonTextMesh.transform.localPosition = ORIGINAL_FONT_POSITION;
        }
    }
}
