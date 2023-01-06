using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace Timescale
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        static ManualLogSource logger;
        public static ConfigEntry<KeyboardShortcut> openMenuKey;

        private void Awake()
        {
            openMenuKey = Config.Bind("General", "Open Menu Key", new KeyboardShortcut(KeyCode.F2), "Key to open timescale menu");

            // Plugin startup logic
            logger = Logger;
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded");
            Logger.LogInfo($"Patching...");
            Harmony.CreateAndPatchAll(typeof(Plugin));
            Logger.LogInfo($"Patched");
        }

        [HarmonyPatch(typeof(PlayerManager), "Start")]
        [HarmonyPrefix]
        static void Start_Prefix()
        {
            new GameObject("__Timescale__").AddComponent<TimescaleMain>().Init(openMenuKey);
        }
    }


    internal class TimescaleMain : MonoBehaviour
    {
        private bool visible;
        private float value = 1f;
        static ConfigEntry<KeyboardShortcut> openMenuKey;

        public void Init(ConfigEntry<KeyboardShortcut> menuKey)
        {
            openMenuKey = menuKey;
        }

        private void OnGUI()
        {
            if (this.visible)
            {
                GUI.Box(new Rect(50f, 50f, 450f, 40f), "");
                GUI.Label(new Rect(75f, 60f, 100f, 40f), "Timescale:");
                this.value = GUI.HorizontalSlider(new Rect(175f, 62.5f, 300f, 20f), this.value, 1f, 60f);
                Time.timeScale = this.value;
            }
        }

        private void Update()
        {
            if (openMenuKey.Value.IsDown())
            {
                this.visible = !this.visible;
                if (this.visible)
                {
                    CursorManager.Instance.ShowCursor();
                    return;
                }
                CursorManager.Instance.HideCursor();
            }
        }
    }
}
