using System.IO;
using static SaveSlotsPlus.SlotPath;
using static SaveSlotsPlus.SSP_Plugin;

namespace SaveSlotsPlus
{
    internal class FileMenu
    {
        internal static SaveNameInput SaveNameInputText { get; set; }
        private static int _showingListFor = -1;
        private static int _copySlotNum = -1;
        private static bool _awaitingDeleteConfirm = false;

        internal static void SetShowingListFor(int slotNum)
        {
            _showingListFor = slotNum;
        }

        internal static void RenameButtonClicked()
        {
            LogDebug($"Rename button clicked for slot {_showingListFor}");
            if (!SaveSlots.slotsActive[_showingListFor])
                return;

            SaveSlotsUI.ShowRenameInput();
            LogDebug($"Showing rename input for slot {_showingListFor}");
        }

        internal static void ConfirmRenameButtonClicked()
        {
            LogDebug($"Confirm rename button clicked for slot {_showingListFor}");
            SaveNameInputText.SaveName(_showingListFor);
            Paginator.RefreshSlot(_showingListFor);
            SaveSlotsUI.HideRenameInput();

            LogInfo($"Renamed slot {_showingListFor}");
        }

        internal static void CopyButtonClicked()
        {
            LogDebug($"Copy button clicked for slot {_showingListFor}");
            if (!SaveSlots.slotsActive[_showingListFor])
                return;

            _copySlotNum = _showingListFor;
            SaveSlotsUI.HideFileMenu();

            LogInfo($"Set slot {_copySlotNum} to be copied");
        }

        internal static void PasteButtonClicked()
        {
            LogDebug($"Paste button clicked for slot {_showingListFor} with copy slot {_copySlotNum}");
            if (!File.Exists(SlotSavePath(_showingListFor)) && _copySlotNum > -1)
            {
                TryCopy(SlotSavePath(_copySlotNum), SlotSavePath(_showingListFor));
                TryCopy(SlotScreenshotPath(_copySlotNum), SlotScreenshotPath(_showingListFor));
                TryCopy(SlotMetaPath(_copySlotNum), SlotMetaPath(_showingListFor));
                for (int i = 1; i <= 5; i++)                
                    TryCopy(SlotBackupPath(_copySlotNum, i), SlotBackupPath(_showingListFor, i));
                
                var fromFolder = SlotModSavesDirPath(_copySlotNum);
                var toFolder = SlotModSavesDirPath(_showingListFor);
                if (Directory.Exists(fromFolder))
                {
                    Directory.CreateDirectory(toFolder);
                    foreach (var file in Directory.GetFiles(fromFolder))
                    {
                        var destFile = Path.Combine(toFolder, Path.GetFileName(file));
                        File.Copy(file, destFile, overwrite: true);
                    }
                }                

                SaveSlots.slotsActive[_showingListFor] = true;
                SaveSlots.activeSlotsCount = System.Array.FindAll(SaveSlots.slotsActive, s => s).Length;

                Paginator.RefreshSlot(_showingListFor);
                SaveSlotsUI.CopyMaterial(_copySlotNum, _showingListFor);
                SaveSlotsUI.HideFileMenu();

                LogInfo($"Copied slot {_copySlotNum} to slot {_showingListFor}");
            }
        }

        internal static void DeleteButtonClicked()
        {
            LogDebug($"Delete button clicked for slot {_showingListFor}");
            if (!SaveSlots.slotsActive[_showingListFor])
                return;

            if (!_awaitingDeleteConfirm)
            {
                _awaitingDeleteConfirm = true;
            }
            SaveSlotsUI.EnableConfirmDeleteButton();
            LogDebug($"Enabled confirm delete button for slot {_showingListFor}");
        }

        internal static void DeleteButtonConfirmClicked()
        {
            LogDebug($"Confirm delete button clicked for slot {_showingListFor}");
            if (!_awaitingDeleteConfirm)
                return;

            TryDelete(SlotSavePath(_showingListFor));
            TryDelete(SlotScreenshotPath(_showingListFor));
            TryDelete(SlotMetaPath(_showingListFor));
            for (int i = 1; i <= 5; i++)
                TryDelete(SlotBackupPath(_showingListFor, i));

            var modFolder = SlotModSavesDirPath(_showingListFor);
            if (Directory.Exists(modFolder))
                Directory.Delete(modFolder, recursive: true);

            SaveSlots.slotsActive[_showingListFor] = false;
            SaveSlots.activeSlotsCount = System.Array.FindAll(SaveSlots.slotsActive, s => s).Length;
            
            Paginator.RefreshSlot(_showingListFor);
            SaveSlotsUI.ResetMaterial(_showingListFor);
            SaveSlotsUI.HideFileMenu();

            LogInfo($"Deleted slot {_showingListFor}");
        }

        private static void TryCopy(string from, string to)
        {
            if (File.Exists(from))
                File.Copy(from, to, true);
        }

        private static void TryDelete(string path)
        {
            if (File.Exists(path))
                File.Delete(path);
        }

        internal static void ClearAwaitingDeleteConfirm()
        {
            _awaitingDeleteConfirm = false;
        }

        internal static void ResetCopySlotNum()
        {
            _copySlotNum = -1;
        }
    }
}
