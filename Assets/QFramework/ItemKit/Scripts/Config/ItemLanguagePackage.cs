using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
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
            else if (ItemConfigGroup && LocalItems.Count > 0)
            {
                // itemDB : item_iron、item_paper
                // itemLanguagePackage : 
                LocalItems.RemoveAll(item => ItemConfigGroup.ItemConfigs.All(i => i.Key != item.Key));

                // 将 ItemConfigGroup 中的内容添加过来
                List<LocalItem> newLocalItems = new List<LocalItem>();
                foreach (var item in ItemConfigGroup.ItemConfigs)
                {
                    LocalItem localItem2Add = LocalItems.FirstOrDefault(localItem => localItem.Key == item.Key);
                    if (localItem2Add != null)
                    {
                        newLocalItems.Add(localItem2Add);
                    }
                    else
                    {
                        newLocalItems.Add(new LocalItem()
                        {
                            Icon = item.Icon,
                            Key = item.Key,
                            Name = item.Name,
                            Description = item.Description,
                        });
                    }
                }

                LocalItems = newLocalItems;
            }
            else if (ItemConfigGroup == null)
            {
                LocalItems.Clear();
            }
        }

        [Serializable] // 此处不用 SO，是因为该类只用于查询一些信息，而不需要直接拿到此文件（如果写成 SO，还要处理增删改查）
        public class LocalItem
        {
            [HideLabel]
            [PreviewField(48, ObjectFieldAlignment.Left)]
            [HorizontalGroup("LocalItem", 54), VerticalGroup("LocalItem/left")]
            public Sprite Icon;

            [VerticalGroup("LocalItem/right")]
            [LabelWidth(60)]
            public string Key;

            [VerticalGroup("LocalItem/right")]
            [LabelText("名称"), LabelWidth(60)]
            public string Name;

            [VerticalGroup("LocalItem/right")]
            [LabelText("描述"), LabelWidth(60)]
            [TextArea(minLines: 1, maxLines: 4)]
            public string Description;
        }
    }
}
