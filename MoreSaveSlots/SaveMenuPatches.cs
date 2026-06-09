using HarmonyLib;
using System;
using System.IO;
using UnityEngine;
using static MoreSaveSlots.Configs;
using static MoreSaveSlots.MSS_Plugin;

namespace MoreSaveSlots
{
    internal class SaveMenuPatches
    {
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
                SaveSlotsUI.InitializeUI(___saveSlotUI);
                Paginator.AddMoreSaveSlots(__instance.backupSavesUI, ___saveSlotUI.GetComponentsInChildren<StartMenuButton>(true));
            }

            [HarmonyPrefix]
            [HarmonyPatch("ButtonClick", typeof(StartMenuButtonType))]
            public static bool ButtonClickPatch(StartMenuButtonType button)
            {
                return Paginator.ButtonClick(button);
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

                ___showingListFor = parentSlotButton.saveSlot;
                bool active = false;
                __instance.transform.position = parentSlotButton.transform.parent.position;
                if (parentSlotButton.saveSlot % 6 <= 2)
                {
                    __instance.transform.Translate(Vector3.up * __instance.topRowOffset);
                }

                StartMenuButton[] array = __instance.buttons;
                foreach (StartMenuButton startMenuButton in array)
                {
                    string backupPath = SaveSlots.GetBackupPath(parentSlotButton.saveSlot, startMenuButton.saveSlot);
                    if (File.Exists(backupPath))
                    {
                        active = true;
                        DateTime lastWriteTime = File.GetLastWriteTime(backupPath);
                        string buttonText = lastWriteTime.ToShortTimeString() + "\n" + lastWriteTime.ToShortDateString();
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
                if (___showingListFor % 6 == 4 && __instance.list.gameObject.activeInHierarchy)
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

                if (___showingListFor % 6 == 1 && __instance.list.gameObject.activeInHierarchy)
                {
                    __instance.chooseSlotText.SetActive(false);
                }
                else
                {
                    __instance.chooseSlotText.SetActive(true);
                }

                return false;
            }
        }
    }
}
