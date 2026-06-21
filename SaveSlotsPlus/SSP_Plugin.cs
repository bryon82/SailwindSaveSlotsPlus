using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;

namespace SaveSlotsPlus
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    [BepInDependency(PORTABLE_SAVES_GUID, BepInDependency.DependencyFlags.SoftDependency)]
    public class SSP_Plugin : BaseUnityPlugin
    {
        public const string PLUGIN_GUID = "com.raddude.saveslotsplus";
        public const string PLUGIN_NAME = "SaveSlotsPlus";
        public const string PLUGIN_VERSION = "2.2.0";

        public const string PORTABLE_SAVES_GUID = "com.nandbrew.PortableSaves";

        internal static bool PortableSavesDetected { get; private set; } = false;

        internal static SSP_Plugin Instance { get; private set; }
        private static ManualLogSource _logger;

        internal static void LogDebug(string message) => _logger.LogDebug(message);
        internal static void LogInfo(string message) => _logger.LogInfo(message);
        internal static void LogWarning(string message) => _logger.LogWarning(message);
        internal static void LogError(string message) => _logger.LogError(message);

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            _logger = Logger;

            foreach (var plugin in Chainloader.PluginInfos)
            {
                var metadata = plugin.Value.Metadata;
                if (metadata.GUID.Equals(PORTABLE_SAVES_GUID))
                {
                    LogInfo("PortableSaves mod found");
                    PortableSavesDetected = true;
                    break;
                }
            }

            SlotPath.SetBasePath();
            Configs.InitializeConfigs();
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), PLUGIN_GUID);
        }
    }
}
