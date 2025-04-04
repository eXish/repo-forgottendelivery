using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using REPOLib.Modules;
using UnityEngine;

namespace ForgottenDelivery
{
    [BepInPlugin(mGUID, mName, mVersion)]
    [BepInDependency(REPOLib.MyPluginInfo.PLUGIN_GUID)]
    public class ForgottenDeliveryMod : BaseUnityPlugin
    {
        const string mGUID = "eXish.ForgottenDelivery";
        const string mName = "ForgottenDelivery";
        const string mVersion = "1.0.0";

        readonly Harmony harmony = new Harmony(mGUID);

        internal static ForgottenDeliveryMod instance;
        internal static ManualLogSource log;
        internal static GameObject packagePrefab;
        internal static GameObject packageBigPrefab;

        void Awake()
        {
            if (instance == null)
                instance = this;

            log = Logger;

            ConfigManager.Init();
            if (ConfigManager.maxSpawnsPerLocation.Value < 0)
                log.LogWarning($"The value \"{ConfigManager.maxSpawnsPerLocation.Value}\" is not valid for setting \"maxSpawnsPerLocation\"! The default will be used instead.");
            if (ConfigManager.spawnChance.Value < 0 || ConfigManager.spawnChance.Value > 100)
                log.LogWarning($"The value \"{ConfigManager.spawnChance.Value}\" is not valid for setting \"spawnChance\"! The default will be used instead.");
            if (ConfigManager.chanceForBigPackage.Value < 0 || ConfigManager.chanceForBigPackage.Value > 100)
                log.LogWarning($"The value \"{ConfigManager.chanceForBigPackage.Value}\" is not valid for setting \"chanceForBigPackage\"! The default will be used instead.");
            if (!ConfigManager.ValidatePackageDrops(false))
                log.LogWarning($"The value \"{ConfigManager.packageDrops.Value}\" is not valid for setting \"packageDrops\"! The default will be used instead.");
            if (!ConfigManager.ValidatePackageDrops(true))
                log.LogWarning($"The value \"{ConfigManager.bigPackageDrops.Value}\" is not valid for setting \"bigPackageDrops\"! The default will be used instead.");

            string modLocation = Info.Location.TrimEnd("ForgottenDelivery.dll".ToCharArray());
            AssetBundle bundle = AssetBundle.LoadFromFile(modLocation + "forgottendelivery");
            if (bundle != null)
            {
                packagePrefab = bundle.LoadAsset<GameObject>("Assets/ForgottenDelivery/eXDeliveryBox.prefab");
                packageBigPrefab = bundle.LoadAsset<GameObject>("Assets/ForgottenDelivery/eXDeliveryBoxBig.prefab");
                NetworkPrefabs.RegisterNetworkPrefab(packagePrefab);
                NetworkPrefabs.RegisterNetworkPrefab(packageBigPrefab);
            }
            else
                log.LogError("Unable to locate the asset file! Delivery packages will not spawn.");

            harmony.PatchAll();

            log.LogInfo($"{mName}-{mVersion} loaded!");
        }
    }
}
