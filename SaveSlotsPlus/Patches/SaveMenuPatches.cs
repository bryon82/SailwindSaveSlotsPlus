using HarmonyLib;
using System.IO;
using UnityEngine;
using static SaveSlotsPlus.Configs;
using static SaveSlotsPlus.SSP_Plugin;

namespace SaveSlotsPlus
{
    internal class SaveMenuPatches
    {
        private static TextMesh _nandTweaksLabelText = null;

        [HarmonyPatch(typeof(SaveSlots), "Awake")]
        private class SaveSlotsPatches
        {
            public static bool Prefix()
            {
                SaveSlots.slotsActive = new bool[Paginator.SLOTS_PER_PAGE * numPages.Value];
                for (int i = 0; i < SaveSlots.slotsActive.Length; i++)
                {
                    SaveSlots.currentSlot = i;
                    if (File.Exists(SaveSlots.GetCurrentSavePath()))
                    {
                        SaveSlots.slotsActive[i] = true;
                        SaveSlots.activeSlotsCount++;
                    }
                }

                LogDebug("SaveSlots: activeSlotsCount is " + SaveSlots.activeSlotsCount);
                return false;
            }
        }

        [HarmonyPatch(typeof(StartMenu))]
        private class StartMenuPatches
        {
            [HarmonyBefore("com.nandbrew.nandtweaks")]
            [HarmonyPostfix]
            [HarmonyPatch("Start")]
            public static void InitializeSaveSlotsUI(StartMenu __instance, GameObject ___saveSlotUI)
            {
                SaveSlotsUI.InitializeUI(___saveSlotUI, __instance.backupSavesUI);
                Paginator.AddMoreSaveSlots(__instance.backupSavesUI, ___saveSlotUI.GetComponentsInChildren<StartMenuButton>(true));
            }

            [HarmonyPostfix]
            [HarmonyPatch("EnableSlotMenu")]
            public static void GetLabelText(GameObject ___saveSlotUI)
            {
                if (_nandTweaksLabelText == null)
                    _nandTweaksLabelText = ___saveSlotUI.transform.Find("label text").GetComponent<TextMesh>();
            }

            [HarmonyPrefix]
            [HarmonyPatch("ButtonClick", typeof(StartMenuButtonType))]
            public static bool ButtonClickPatch(StartMenuButtonType button, GameObject ___saveSlotUI)
            {
                if (button == SaveSlotsUI.PREVIOUS_BUTTON)
                    Paginator.PreviousButtonClicked();

                else if (button == SaveSlotsUI.NEXT_BUTTON)
                    Paginator.NextButtonClicked();

                else if (button == SaveSlotsUI.RENAME_BUTTON)
                    FileMenu.RenameButtonClicked();

                else if (button == SaveSlotsUI.COPY_BUTTON)
                {
                    FileMenu.CopyButtonClicked();
                    SaveSlotsUI.CopyButtonClicked();
                }                    

                else if (button == SaveSlotsUI.PASTE_BUTTON)
                    FileMenu.PasteButtonClicked();

                else if (button == SaveSlotsUI.DELETE_BUTTON)
                    FileMenu.DeleteButtonClicked();

                else if (button == SaveSlotsUI.CONFIRM_DELETE_BUTTON)
                    FileMenu.DeleteButtonConfirmClicked();

                else if (button == SaveSlotsUI.CONFIRM_RENAME_BUTTON)
                    FileMenu.ConfirmRenameButtonClicked();

                else if (button == StartMenuButtonType.Back)
                {
                    FileMenu.ClearAwaitingDeleteConfirm();
                    FileMenu.ResetCopySlotNum();
                    SaveSlotsUI.ClearCopyClicked();
                    SaveSlotsUI.HideFileMenu();
                    SaveSlotsUI.HideRenameInput();
                    return true;
                }

                else
                    return true;

                return false;
            }
        }

        [HarmonyPatch(typeof(StartMenuButton))]
        private class StartMenuButtonPatches
        {
            [HarmonyPostfix]
            [HarmonyPatch("Awake")]
            public static void AwakePatch(StartMenuButton __instance, StartMenuButtonType ___type)
            {
                if (___type == StartMenuButtonType.Slot)                
                    Paginator.RefreshSlotText(__instance);
            }

            [HarmonyPostfix]
            [HarmonyPatch("ExtraLateUpdate")]
            public static void ExtraLateUpdatePatch(StartMenuButton __instance)
            {
                if (__instance.GetPrivateField<StartMenuButtonType>("type") != StartMenuButtonType.Slot)
                    return;

                if (__instance.GetPrivateField<bool>("isLookedAt") && Input.GetMouseButtonDown(1))
                {                    
                    SaveSlotsUI.ToggleFileMenu(__instance);
                }
            }
        }

        [HarmonyPatch(typeof(BackupSavesListUI))]
        private class BackupSavesListUIPatches
        {            
            [HarmonyPrefix]
            [HarmonyPatch("ShowList")]
            public static bool ShowList(StartMenuButton parentSlotButton, BackupSavesListUI __instance, ref int ___showingListFor)
            {
                if (___showingListFor == parentSlotButton.saveSlot)
                {
                    return false;
                }

                SaveSlotsUI.HideFileMenu();
                SaveSlotsUI.HideRenameInput();
                FileMenu.ClearAwaitingDeleteConfirm();
                FileMenu.SetShowingListFor(parentSlotButton.saveSlot);

                ___showingListFor = parentSlotButton.saveSlot;
                var active = false;
                __instance.transform.position = parentSlotButton.transform.parent.position;
                if (parentSlotButton.saveSlot % 6 <= 2)
                {
                    __instance.transform.Translate(Vector3.up * __instance.topRowOffset);
                }

                StartMenuButton[] array = __instance.buttons;
                foreach (StartMenuButton startMenuButton in array)
                {
                    var backupPath = SaveSlots.GetBackupPath(parentSlotButton.saveSlot, startMenuButton.saveSlot);
                    if (File.Exists(backupPath))
                    {
                        active = true;
                        var lastWriteTime = File.GetLastWriteTime(backupPath);
                        var buttonText = lastWriteTime.ToShortTimeString() + "\n" + lastWriteTime.ToShortDateString();
                        startMenuButton.SetButtonText(buttonText);
                        startMenuButton.transform.parent.gameObject.SetActive(true);
                    }
                    else
                    {
                        startMenuButton.transform.parent.gameObject.SetActive(false);
                    }
                }

                __instance.list.gameObject.SetActive(active);
                return false;
            }

            [HarmonyPrefix]
            [HarmonyPatch("Update")]
            public static bool Update(BackupSavesListUI __instance, int ___showingListFor, Vector3 ___backButtonInitialPos, Vector3 ___backButtonAltPos)
            {
                if (!(__instance.currentLookedAtSlot == null))
                {
                    __instance.ShowList(__instance.currentLookedAtSlot);
                }

                Vector3 b;
                if (___showingListFor % 6 == 4 && (__instance.list.gameObject.activeInHierarchy || SaveSlotsUI.FileMenuList.gameObject.activeInHierarchy))
                {
                    SaveSlotsUI.PageNumTextMesh.gameObject.SetActive(false);
                    b = ___backButtonAltPos;
                }
                else
                {
                    SaveSlotsUI.PageNumTextMesh.gameObject.SetActive(true);
                    b = ___backButtonInitialPos;
                }
                __instance.backButton.parent.localPosition = Vector3.Lerp(__instance.backButton.parent.localPosition, b, Time.deltaTime * 8f);

                if (___showingListFor % 6 == 1 && (__instance.list.gameObject.activeInHierarchy || SaveSlotsUI.FileMenuList.gameObject.activeInHierarchy))
                {
                    __instance.chooseSlotText.SetActive(false);
                    if (_nandTweaksLabelText != null && _nandTweaksLabelText.text != "")
                        _nandTweaksLabelText.text = "";
                }
                else
                {
                    __instance.chooseSlotText.SetActive(true);
                    if (_nandTweaksLabelText != null && _nandTweaksLabelText.text != "Continue")
                        _nandTweaksLabelText.text = "Continue";
                }

                return false;
            }
        }
    }
}
