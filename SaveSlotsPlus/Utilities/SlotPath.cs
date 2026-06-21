using UnityEngine;
using static SaveSlotsPlus.SSP_Plugin;

namespace SaveSlotsPlus
{
    internal class SlotPath
    {
        private static string _basePath;

        internal static void SetBasePath()
        {
            _basePath = Application.persistentDataPath;

            if (PortableSavesDetected)
            {
                _basePath = $"{Application.dataPath}/Saves";
            }
        }

        internal static string SlotSavePath(int slot)
        {
            return $"{_basePath}/slot{slot}.save";
        }

        internal static string SlotBackupPath(int slot, int backupNum)
        {
            return $"{_basePath}/slot{slot}_backup{backupNum}.save";
        }

        internal static string SlotMetaPath(int slot)
        {
            return $"{_basePath}/slot{slot}.save.meta";
        }

        internal static string SlotScreenshotPath(int slot)
        {
            return $"{_basePath}/slot{slot}.save.png";
        }

        internal static string SlotModSavesDirPath(int slot)
        {
            return $"{_basePath}/slot{slot}";
        }
    }
}
