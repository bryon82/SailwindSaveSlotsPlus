using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using static MoreSaveSlots.Configs;
using static MoreSaveSlots.MSS_Plugin;

namespace MoreSaveSlots
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
                {
                    btn.saveSlot = slot;
                    RefreshSlotText(btn);
                }

                clone.SetActive(false);
                _allSlotRoots.Add(clone);
            }
        }

        private static void RefreshSlotText(StartMenuButton btn)
        {
            if (SaveSlots.slotsActive != null
                && btn.saveSlot < SaveSlots.slotsActive.Length
                && SaveSlots.slotsActive[btn.saveSlot])
            {
                var t = File.GetLastWriteTime(SaveSlots.GetSlotSavePath(btn.saveSlot));
                btn.SetButtonText(t.ToShortTimeString() + "\n" + t.ToShortDateString());
            }
            else
            {
                btn.SetButtonText("(empty)");
            }
        }

        public static bool ButtonClick(StartMenuButtonType button)
        {
            if (button == SaveSlotsUI.PREVIOUS_BUTTON)
            {
                _backUpSavesListUI.HideList();
                _currentPage = Mathf.Max(0, _currentPage - 1);
                SaveSlotsUI.UpdatePageNumText(_currentPage);
                ShowPage(_currentPage);
                return false;
            }

            if (button == SaveSlotsUI.NEXT_BUTTON)
            {
                _backUpSavesListUI.HideList();
                _currentPage = Mathf.Min(numPages.Value - 1, _currentPage + 1);
                SaveSlotsUI.UpdatePageNumText(_currentPage);
                ShowPage(_currentPage);
                return false;
            }

            return true;
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
    }
}
