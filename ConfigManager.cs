using BepInEx.Configuration;
using System.Linq;

namespace ForgottenDelivery
{
    internal class ConfigManager
    {
        public static ConfigEntry<int> maxSpawnsPerLocation;
        public static ConfigEntry<int> spawnChance;
        public static ConfigEntry<int> chanceForBigPackage;

        public static ConfigEntry<string> packageDrops;
        public static ConfigEntry<string> bigPackageDrops;
        public static ConfigEntry<string> packageBlacklist;
        public static ConfigEntry<string> bigPackageBlacklist;

        private readonly static string[] itemTypes = { "drone", "orb", "cart", "upgrade", "crystal", "grenade", "melee", "healthpack", "gun", "tracker", "mine", "pocketcart" };

        public static void Init()
        {
            maxSpawnsPerLocation = ForgottenDeliveryMod.instance.Config.Bind("Spawn Settings", "maxSpawnsPerLocation", 1, "The maximum number of packages to spawn per location.");
            spawnChance = ForgottenDeliveryMod.instance.Config.Bind("Spawn Settings", "spawnChance", 50, "How rare it is for a package to spawn. Can be set to a minimum of 0 (never) and a maximum of 100 (guaranteed).");
            chanceForBigPackage = ForgottenDeliveryMod.instance.Config.Bind("Spawn Settings", "chanceForBigPackage", 25, "How rare it is for a spawned package to be big. Can be set to a minimum of 0 (never) and a maximum of 100 (guaranteed).");

            packageDrops = ForgottenDeliveryMod.instance.Config.Bind("Drop Settings", "packageDrops", "drone;orb;upgrade;crystal;grenade;healthpack;mine", "The item types that can be dropped by a regular package. This setting is case insenstive and you can chain item types with semicolons. All types are listed in the README.");
            bigPackageDrops = ForgottenDeliveryMod.instance.Config.Bind("Drop Settings", "bigPackageDrops", "cart;melee;gun;tracker;pocketcart", "The item types that can be dropped by a big package. This setting is case insenstive and you can chain item types with semicolons. All types are listed in the README.");
            packageBlacklist = ForgottenDeliveryMod.instance.Config.Bind("Drop Settings", "packageBlacklist", "", "The items that regular packages should be banned from dropping. This setting is case insenstive and you can chain item names with semicolons. Item names should be exactly as they appear in game.");
            bigPackageBlacklist = ForgottenDeliveryMod.instance.Config.Bind("Drop Settings", "bigPackageBlacklist", "", "The items that big packages should be banned from dropping. This setting is case insenstive and you can chain item names with semicolons. Item names should be exactly as they appear in game.");
        }

        public static bool ValidatePackageDrops(bool big)
        {
            string[] setting = big ? bigPackageDrops.Value.ToLower().Split(';') : packageDrops.Value.ToLower().Split(';');
            for (int i = 0; i < setting.Length; i++)
            {
                if (!itemTypes.Contains(setting[i]))
                    return false;
            }
            return true;
        }

        public static SemiFunc.itemType[] GetPackageDrops(bool big)
        {
            string bigDrops = bigPackageDrops.Value;
            if (!ValidatePackageDrops(true))
                bigDrops = (string)bigPackageDrops.DefaultValue;
            string normalDrops = packageDrops.Value;
            if (!ValidatePackageDrops(false))
                normalDrops = (string)packageDrops.DefaultValue;
            string[] setting = big ? bigDrops.ToLower().Split(';') : normalDrops.ToLower().Split(';');
            SemiFunc.itemType[] items = new SemiFunc.itemType[setting.Length];
            for (int i = 0; i < setting.Length; i++)
            {
                switch (setting[i])
                {
                    case "drone":
                        items[i] = SemiFunc.itemType.drone;
                        break;
                    case "orb":
                        items[i] = SemiFunc.itemType.orb;
                        break;
                    case "cart":
                        items[i] = SemiFunc.itemType.cart;
                        break;
                    case "upgrade":
                        items[i] = SemiFunc.itemType.item_upgrade;
                        break;
                    case "crystal":
                        items[i] = SemiFunc.itemType.power_crystal;
                        break;
                    case "grenade":
                        items[i] = SemiFunc.itemType.grenade;
                        break;
                    case "melee":
                        items[i] = SemiFunc.itemType.melee;
                        break;
                    case "healthpack":
                        items[i] = SemiFunc.itemType.healthPack;
                        break;
                    case "gun":
                        items[i] = SemiFunc.itemType.gun;
                        break;
                    case "tracker":
                        items[i] = SemiFunc.itemType.tracker;
                        break;
                    case "mine":
                        items[i] = SemiFunc.itemType.mine;
                        break;
                    default:
                        items[i] = SemiFunc.itemType.pocket_cart;
                        break;
                }
            }
            return items;
        }

        public static string[] GetBlacklist(bool big)
        {
            return big ? bigPackageBlacklist.Value.ToLower().Split(';') : packageBlacklist.Value.ToLower().Split(';');
        }
    }
}
