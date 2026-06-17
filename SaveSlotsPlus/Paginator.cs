using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using static SaveSlotsPlus.Configs;
using static SaveSlotsPlus.SSP_Plugin;

namespace SaveSlotsPlus
{
    internal class Paginator
    {
        internal const int SLOTS_PER_PAGE = 6;
        private static int _currentPage = 0;
        private static readonly List<GameObject> _allSlotRoots = new List<GameObject>();
        private static BackupSavesListUI _backUpSavesListUI;        

        public static void AddMoreSaveSlots(BackupSavesListUI backupSavesListUI, StartMenuButton[] existingButtons)
        {
            _backUpSavesListUI = backupSavesListUI;            
            var slotButtons = existingButtons
            .Where(b => b.GetPrivateField<StartMenuButtonType>("type") == StartMenuButtonType.Slot)
            .OrderBy(b => b.saveSlot)
            .ToList();

            foreach (var btn in slotButtons)
            {
                _allSlotRoots.Add(btn.transform.parent.gameObject);
            }
            for (int slot = SLOTS_PER_PAGE; slot < SLOTS_PER_PAGE * numPages.Value; slot++)
            {
                var templateIndex = slot % SLOTS_PER_PAGE;
                var template = _allSlotRoots[templateIndex];

                var clone = GameObject.Instantiate(template, template.transform.parent);
                clone.name = $"SaveSlot_{slot}";

                var btn = clone.GetComponentInChildren<StartMenuButton>();
                if (btn != null)
                    btn.saveSlot = slot;

                clone.SetActive(false);
                _allSlotRoots.Add(clone);
            }
        }

        public static void RefreshSlotText(StartMenuButton btn)
        {
            if (SaveSlots.slotsActive != null
                && btn.saveSlot < SaveSlots.slotsActive.Length
                && SaveSlots.slotsActive[btn.saveSlot])
            {
                var timestamp = btn.transform.parent.GetChild(1).GetComponent<TextMesh>().text;
                var name = SaveNameInput.LoadName(btn.saveSlot);
                var displayText = name != null ? $"{name}\n{timestamp}" : timestamp;
                btn.SetButtonText(displayText);
            }
            else
            {
                btn.SetButtonText("(empty)");
            }
        }

        public static void PreviousButtonClicked()
        {            
            SaveSlotsUI.HideFileMenu();
            SaveSlotsUI.HideRenameInput();
            _backUpSavesListUI.HideList();
            _currentPage = Mathf.Max(0, _currentPage - 1);
            SaveSlotsUI.SetChangePageTextColor(SaveSlotsUI.PREVIOUS_BUTTON, true, true);
            if (_currentPage == 0)            
                SaveSlotsUI.SetChangePageTextColor(SaveSlotsUI.PREVIOUS_BUTTON, false);
            SaveSlotsUI.UpdatePageNumText(_currentPage);
            ShowPage(_currentPage);
            LogDebug($"Previous button clicked, showing page {_currentPage}");
        }

        public static void NextButtonClicked()
        {
            SaveSlotsUI.HideFileMenu();
            SaveSlotsUI.HideRenameInput();
            _backUpSavesListUI.HideList();
            _currentPage = Mathf.Min(numPages.Value - 1, _currentPage + 1);
            SaveSlotsUI.SetChangePageTextColor(SaveSlotsUI.NEXT_BUTTON, true, true);
            if (_currentPage == numPages.Value - 1)
                SaveSlotsUI.SetChangePageTextColor(SaveSlotsUI.NEXT_BUTTON, false);
            SaveSlotsUI.UpdatePageNumText(_currentPage);
            ShowPage(_currentPage);
            LogDebug($"Next button clicked, showing page {_currentPage}");
        }

        private static void ShowPage(int page)
        {
            foreach (var root in _allSlotRoots)
                root.SetActive(false);

            _currentPage = page;
            var startSlot = page * SLOTS_PER_PAGE;
            var endSlot = Mathf.Min(startSlot + SLOTS_PER_PAGE, SLOTS_PER_PAGE * numPages.Value);

            for (int i = startSlot; i < endSlot; i++)
                _allSlotRoots[i].SetActive(true);
        }

        internal static void RefreshSlot(int slot)
        {
            var btn = GetStartMenuButtonForSlot(slot);
            if (btn != null)
            {
                var lastWriteTime = File.GetLastWriteTime(SaveSlots.GetSlotSavePath(slot));
                var buttonText = $"{lastWriteTime:t}\n{lastWriteTime:d}";
                btn.SetButtonText(buttonText);
            }

            RefreshSlotText(btn);            
        }

        internal static StartMenuButton GetStartMenuButtonForSlot(int slot)
        {
            if (slot < _allSlotRoots.Count)
            {
                return _allSlotRoots[slot].GetComponentInChildren<StartMenuButton>();
            }
            return null;
        }
    }
}
