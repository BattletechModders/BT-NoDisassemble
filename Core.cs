using System;
using System.Reflection;
using Harmony;
using Newtonsoft.Json;

namespace NoDisassemble
{
    public static partial class Core
    {
        public const string ModName = "NoDisassemble";
        public const string ModId   = "com.joelmeador.NoDisassemble";

        internal static Settings ModSettings = new Settings();
        internal static string ModDirectory;

        public static void Init(string directory, string settingsJson)
        {
            ModDirectory = directory;
            try
            {
                ModSettings = JsonConvert.DeserializeObject<Settings>(settingsJson);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                ModSettings = new Settings();
            }
            HarmonyInstance.DEBUG = ModSettings.Debug;
            var harmony = HarmonyInstance.Create(ModId);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}