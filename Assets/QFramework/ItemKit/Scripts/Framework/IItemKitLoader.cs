using System;
using UnityEngine;

namespace QFramework
{
    public interface IItemKitLoader
    {
        ItemDatabase LoadItemDatabase(string databaseName);
        ItemLanguagePackage LoadLanguagePackage(string languagePackageName);

        // 异步加载，用回调的方式
        void LoadItemDatabaseAsync(string databaseName, Action<ItemDatabase> onLoadFinish);
        void LoadLanguagePackageAsync(string languagePackageName, Action<ItemLanguagePackage> onLoadFinish);
    }

    public class DefaultItemKitLoader : IItemKitLoader
    {
        public ItemDatabase LoadItemDatabase(string databaseName)
        {
            return Resources.Load<ItemDatabase>(databaseName);
        }

        public ItemLanguagePackage LoadLanguagePackage(string languagePackageName)
        {
            return Resources.Load<ItemLanguagePackage>(languagePackageName);
        }

        public void LoadItemDatabaseAsync(string databaseName, Action<ItemDatabase> onLoadFinish)
        {
            // 最基本的异步加载方式
            ResourceRequest request = Resources.LoadAsync<ItemDatabase>(databaseName);
            request.completed += operation =>
            {
                onLoadFinish(request.asset as ItemDatabase);
            };
        }

        public void LoadLanguagePackageAsync(string languagePackageName, Action<ItemLanguagePackage> onLoadFinish)
        {
            ResourceRequest request = Resources.LoadAsync<ItemLanguagePackage>(languagePackageName);
            request.completed += operation =>
            {
                onLoadFinish(request.asset as ItemLanguagePackage);
            };
        }
    }
}
