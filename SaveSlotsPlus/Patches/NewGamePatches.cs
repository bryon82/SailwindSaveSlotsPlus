using HarmonyLib;
using UnityEngine;

namespace SaveSlotsPlus
{
    internal class NewGamePatches
    {
        internal static SaveNameInput SaveGameInputText { get; set; }

        [HarmonyPatch(typeof(StartMenu))]
        private class StartMenuNewGamePatches
        {
            [HarmonyPostfix]
            [HarmonyPatch("Start")]
            public static void OnShowChooseIslandUI(GameObject ___chooseIslandUI)
            {
                NewGameUI.InitializeUI(___chooseIslandUI.transform);
            }

            [HarmonyPrefix]
            [HarmonyPatch("ButtonClick", typeof(StartMenuButtonType))]
            public static bool ButtonClickPatch(StartMenuButtonType button)
            {
                if (button == StartMenuButtonType.NewGame)
                {
                    SaveGameInputText.Activate();
                }

                return true;
            }

            [HarmonyPrefix]
            [HarmonyPatch("StartNewGame")]
            public static bool StartNewGame(GameObject ___chooseIslandUI)
            {
                var saveNameInput = ___chooseIslandUI.GetComponentInChildren<SaveNameInput>();
                saveNameInput.SaveName(SaveSlots.currentSlot);
                return true;
            }
        }
    }
}