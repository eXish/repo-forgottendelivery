using HarmonyLib;
using REPOLib.Modules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ForgottenDelivery.Patches
{
    [HarmonyPatch(typeof(ValuableDirector))]
    internal class ValuableDirectorPatch
    {
        [HarmonyPatch("VolumesAndSwitchSetup")]
        [HarmonyPostfix]
        static void VolumesAndSwitchSetupPatch()
        {
            if (SemiFunc.RunIsLevel())
            {
                List<ValuableVolume> volumes = Object.FindObjectsOfType<ValuableVolume>(includeInactive: false).Where(x => x.VolumeType == ValuableVolume.Type.Medium).ToList();
                List<ValuableVolume> volumesBig = Object.FindObjectsOfType<ValuableVolume>(includeInactive: false).Where(x => x.VolumeType == ValuableVolume.Type.Wide).ToList();
                ForgottenDeliveryMod.log.LogInfo($"Found {volumes.Count} suitable volumes for spawning regular packages.");
                ForgottenDeliveryMod.log.LogInfo($"Found {volumesBig.Count} suitable volumes for spawning big packages.");
                int chance = ConfigManager.spawnChance.Value;
                if (chance < 0 || chance > 100)
                    chance = (int)ConfigManager.spawnChance.DefaultValue;
                int bigChance = ConfigManager.chanceForBigPackage.Value;
                if (bigChance < 0 || bigChance > 100)
                    bigChance = (int)ConfigManager.chanceForBigPackage.DefaultValue;
                int maxSpawns = ConfigManager.maxSpawnsPerLocation.Value;
                if (maxSpawns < 0)
                    maxSpawns = (int)ConfigManager.maxSpawnsPerLocation.DefaultValue;
                int totalSpawns = 0;
                int totalSpawnsBig = 0;
                for (int i = 0; i < maxSpawns; i++)
                {
                    if (chance > Random.Range(0, 100))
                    {
                        if (bigChance > Random.Range(0, 100) && volumesBig.Count > 0)
                        {
                            int index = Random.Range(0, volumesBig.Count);
                            ValuablePropSwitch swt = volumesBig[index].GetComponentInParent<ValuablePropSwitch>();
                            if (swt != null)
                            {
                                swt.PropParent.SetActive(false);
                                swt.ValuableParent.SetActive(true);
                            }
                            NetworkPrefabs.SpawnNetworkPrefab("eXDeliveryBoxBig", volumesBig[index].transform.position, volumesBig[index].transform.rotation);
                            volumesBig.Remove(volumesBig[index]);
                            totalSpawnsBig++;
                        }
                        else if (volumes.Count > 0)
                        {
                            int index = Random.Range(0, volumes.Count);
                            ValuablePropSwitch swt = volumes[index].GetComponentInParent<ValuablePropSwitch>();
                            if (swt != null)
                            {
                                swt.PropParent.SetActive(false);
                                swt.ValuableParent.SetActive(true);
                            }
                            NetworkPrefabs.SpawnNetworkPrefab("eXDeliveryBox", volumes[index].transform.position, volumes[index].transform.rotation);
                            volumes.Remove(volumes[index]);
                            totalSpawns++;
                        }
                    }
                }
                ForgottenDeliveryMod.log.LogInfo($"A total of {totalSpawns} regular packages were spawned.");
                ForgottenDeliveryMod.log.LogInfo($"A total of {totalSpawnsBig} big packages were spawned.");
            }
        }
    }
}