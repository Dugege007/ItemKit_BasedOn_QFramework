using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    [CreateAssetMenu(menuName = "@ItemKit/Create Item Language Package")]
    public class ItemLanguagePackage : ScriptableObject
    {
        [LabelText("物品数据")]
        public ItemConfigGroup ItemConfigGroup;

        [Title("本地化物品")]
        public List<LocalItem> LocalItems = new List<LocalItem>();

        private void OnValidate()
        {
            if (ItemConfigGroup && LocalItems.Count == 0)
            {
                foreach (var item in ItemConfigGroup.ItemConfigs)
                {
                    LocalItems.Add(new LocalItem()
                    {
                        Key = item.Key,
                        Name = item.Name,
                        Description = item.Description,
                    });
                }
            }
        }

        [Serializable] // 此处不用 SO，是因为该类只用于查询一些信息，而不需要直接拿到此文件（如果写成 SO，还要处理增删改查）
        public class LocalItem
        {
            public string Key;
            [LabelText("名称")]
            public string Name;
            [LabelText("描述")]
            public string Description;
        }
    }
}
