using BepInEx;
using HarmonyLib;
using System.Reflection;
using TotallyNormalMod.Behaviours;

namespace TotallyNormalMod
{

    [BepInPlugin(Constants.GUID, Constants.NAME, Constants.VERS)]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin Instance;
        private void Awake() { Harmony.CreateAndPatchAll(typeof(Plugin).Assembly, Constants.GUID); gameObject.AddComponent<Initializer>(); }

        public static bool BasePatch(MethodBase __originalMethod) => false;
    }
    public class Constants { public const string GUID = "kino.totallynormalmod", NAME = "TotallyNormalMod", VERS = "1.0.0"; }
}