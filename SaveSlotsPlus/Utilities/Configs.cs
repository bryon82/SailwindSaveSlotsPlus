using BepInEx.Configuration;
using UnityEngine;

namespace SaveSlotsPlus
{
    internal class Configs
    {
        internal static ConfigEntry<int> numPages;
        internal static ConfigEntry<KeyCode> altClickKey;
        internal static ConfigEntry<int> saveNameMaxLength;

        internal static void InitializeConfigs()
        {
            var config = SSP_Plugin.Instance.Config;

            numPages = config.Bind(
                "Settings",
                "Number of save slot pages<br>(must restart game)",
                3,
                new ConfigDescription(
                    "The number of pages of save slots. Each page has 6 slots.",
                    new AcceptableValueRange<int>(1, 10)));
            altClickKey = config.Bind(
                "Settings",
                "Alt Click Key",
                KeyCode.Mouse1,
                "Key to click on a save slot to open the file menu.");
            saveNameMaxLength = config.Bind(
                "Settings",
                "Save Name Max Length",
                15,
                new ConfigDescription(
                    "The maximum length of a save name.",
                    new AcceptableValueRange<int>(1, 20)));
        }
    }
}
