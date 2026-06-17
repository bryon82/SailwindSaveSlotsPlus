using BepInEx.Configuration;

namespace SaveSlotsPlus
{
    internal class Configs
    {
        internal static ConfigEntry<int> numPages;

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
        }
    }
}
