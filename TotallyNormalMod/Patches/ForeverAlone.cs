using GorillaNetworking;
using HarmonyLib;
using System.Reflection;

namespace TotallyNormalMod.Patches
{
    [HarmonyPatch(typeof(GorillaNetworkJoinTrigger), "OnBoxTriggered")]
    public class GorillaNetworkJoin
    {
        static bool Prefix(MethodBase __originalMethod)
        {
            return Plugin.BasePatch(__originalMethod);
        }
    }

    [HarmonyPatch(typeof(GorillaNetworkLeaveRoomTrigger), "OnBoxTriggered")]
    public class GorillaNetworkLeave
    {
        static bool Prefix(MethodBase __originalMethod)
        {
            return Plugin.BasePatch(__originalMethod);
        }
    }

    [HarmonyPatch(typeof(GorillaNetworkLeaveTutorialTrigger), "OnBoxTriggered")]
    public class GorillaNetworkLeaveTutorial
    {
        static bool Prefix(MethodBase __originalMethod)
        {
            return Plugin.BasePatch(__originalMethod);
        }
    }
}
