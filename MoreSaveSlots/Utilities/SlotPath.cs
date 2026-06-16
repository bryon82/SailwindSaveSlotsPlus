using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MoreSaveSlots
{
    internal class SlotPath
    {
        internal static string SlotSavePath(int slot)
        {
            return $"{Application.persistentDataPath}/slot{slot}.save";
        }

        internal static string SlotBackupPath(int slot, int backupNum)
        {
            return $"{Application.persistentDataPath}/slot{slot}_backup{backupNum}.save";
        }

        internal static string SlotMetaPath(int slot)
        {
            return $"{Application.persistentDataPath}/slot{slot}.save.meta";
        }

        internal static string SlotScreenshotPath(int slot)
        {
            return $"{Application.persistentDataPath}/slot{slot}.save.png";
        }

        internal static string SlotModSavesDirPath(int slot)
        {
            return $"{Application.persistentDataPath}/slot{slot}";
        }        
    }
}
