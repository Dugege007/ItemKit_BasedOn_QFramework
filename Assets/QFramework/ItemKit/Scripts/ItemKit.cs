using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    public class ItemKit
    {
        public static IItemKitSaveAndLoader SaverAndLoader = new DefaultItemKitSaverAndLoader();
        public static IItemKitLoader Loader = new DefaultItemKitLoader();

        public static Dictionary<string, SlotGroup> mSlotGroupByKey = new Dictionary<string, SlotGroup>();
        public static Dictionary<string, IItem> ItemByKey = new Dictionary<string, IItem>();

        public static UISlot CurrentSlotPointerOn = null;

        public static SlotGroup CreateSlotGroup(string key)
        {
            if (HasSlotGroup(key))
            {
                Debug.Log($"槽位组 '{key}' 已存在。");
                return mSlotGroupByKey[key];
            }
            else
            {
                SlotGroup slotGroup = new SlotGroup() { Key = key };
                mSlotGroupByKey.Add(key, slotGroup);
                Debug.Log($"槽位组 '{key}' 已添加。");
                return slotGroup;
            }
        }

        public static bool HasSlotGroup(string key)
        {
            return mSlotGroupByKey.ContainsKey(key);
        }

        public static SlotGroup GetSlotGroupByKey(string key)
        {
            return mSlotGroupByKey[key];
        }

        public static void LoadItemDatabase(string databaseName)
        {
            ItemDatabase database = Loader.LoadItemDatabase(databaseName);
            if (database == null)
                Debug.LogError("database为空");

            foreach (var config in database.ItemConfigs)
            {
                if(config == null)
                    Debug.LogError("config为空");

                AddItemConfig(config);
            }
        }

        // 异步加载，协程方式
        public static IEnumerator LoadItemDatabaseAsync(string databaseName)
        {
            bool done = false;

            Loader.LoadItemDatabaseAsync(databaseName, database =>
            {
                foreach (var config in database.ItemConfigs)
                    AddItemConfig(config);

                done = true;
            });

            while (!done)
                yield return null;
        }

        // 当前语言
        public const string DefaultLanguage = "DefaultLanguage";
        public static string CurrentLanguage { get; private set; } = DefaultLanguage;

        public static void LoadItemLanguagePackage(string languagePackageName)
        {
            if (languagePackageName == DefaultLanguage)
            {
                CurrentLanguage = DefaultLanguage;
                foreach (var item in ItemByKey.Values)
                {
                    item.LocalItem = null;
                }
            }
            else
            {
                CurrentLanguage = languagePackageName;

                ItemLanguagePackage languagePackage = Loader.LoadLanguagePackage(languagePackageName);
                foreach (var localItem in languagePackage.LocalItems)
                {
                    if (ItemByKey.TryGetValue(localItem.Key, out var item))
                    {
                        item.LocalItem = localItem;
                    }
                }
            }
        }

        // 异步加载语言包，协程方式
        public static IEnumerator LoadItemLanguagePackageAsync(string languagePackageName)
        {
            bool done = false;

            if (languagePackageName == DefaultLanguage)
            {
                CurrentLanguage = DefaultLanguage;
                foreach (var item in ItemByKey.Values)
                {
                    item.LocalItem = null;
                }

                done = true;
            }
            else
            {
                CurrentLanguage = languagePackageName;

                Loader.LoadLanguagePackageAsync(languagePackageName, languagePackage =>
                {
                    foreach (var localItem in languagePackage.LocalItems)
                    {
                        if (ItemByKey.TryGetValue(localItem.Key, out var item))
                        {
                            item.LocalItem = localItem;
                        }
                    }

                    done = true;
                });
            }

            while (!done)
                yield return null;
        }

        public static void AddItemConfig(IItem itemConfig)
        {
            if (!ItemByKey.ContainsKey(itemConfig.GetKey))
            {
                ItemByKey.Add(itemConfig.GetKey, itemConfig);
            }
            else
            {
                Debug.Log($"尝试添加的键 '{itemConfig.GetKey}' 已存在。");
            }
        }

        public static void Save() => SaverAndLoader.Save(mSlotGroupByKey);

        public static void Load() => SaverAndLoader.Load(mSlotGroupByKey);

        public static void Clear() => SaverAndLoader.Clear();
    }
}
