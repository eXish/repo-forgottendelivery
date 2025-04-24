using HarmonyLib;
using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ForgottenDelivery.Patches
{
    [HarmonyPatch(typeof(PhysGrabObjectImpactDetector))]
    internal class ImpactDetectorPatch
    {
        [HarmonyPatch("DestroyObject")]
        [HarmonyPostfix]
        static void DestroyObjectPatch(PhysGrabObjectImpactDetector __instance)
        {
            if (__instance.gameObject.name == "eXDeliveryBoxBig(Clone)" || __instance.gameObject.name == "eXDeliveryBox(Clone)")
            {
                List<Item> items = new List<Item>();
                bool bigPack = __instance.gameObject.name == "eXDeliveryBoxBig(Clone)" ? true : false;
                SemiFunc.itemType[] itemPools = ConfigManager.GetPackageDrops(bigPack);
                string[] itemBlacklist = ConfigManager.GetBlacklist(bigPack);
                foreach (Item value in StatsManager.instance.itemDictionary.Values)
                {
                    if (itemPools.Contains(value.itemType) && !itemBlacklist.Contains(value.itemName.ToLower()))
                        items.Add(value);
                }
                if (items.Count > 0)
                {
                    int index = Random.Range(0, items.Count);
                    if (bigPack)
                        ForgottenDeliveryMod.log.LogInfo($"A big package was destroyed, spawning a {items[index].name}.");
                    else
                        ForgottenDeliveryMod.log.LogInfo($"A regular package was destroyed, spawning a {items[index].name}.");
                    if (SemiFunc.IsMultiplayer())
                        PhotonNetwork.InstantiateRoomObject("Items/" + items[index].prefab.name, __instance.transform.GetChild(0).position, __instance.transform.GetChild(0).rotation);
                    else
                        Object.Instantiate(items[index].prefab, __instance.transform.GetChild(0).position, __instance.transform.GetChild(0).rotation);
                    if (ConfigManager.keepItemsAfterLeaving.Value)
                        StatsManager.instance.ItemPurchase(items[index].itemAssetName);
                }
                else if (bigPack)
                    ForgottenDeliveryMod.log.LogInfo("A big package was destroyed, but the current drop settings do not allow for an item to be dropped by this package.");
                else
                    ForgottenDeliveryMod.log.LogInfo("A regular package was destroyed, but the current drop settings do not allow for an item to be dropped by this package.");
            }
        }
    }
}