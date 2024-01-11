using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    [CreateAssetMenu(menuName = "@ItemKit/Create Item")]
    public class ItemConfig : ScriptableObject, IItem
    {
        public string Key;
        public string Name;
        public Sprite Icon;

        public string GetKey => Key;
        public string GetName => Name;
        public Sprite GetSprite => Icon;
    }
}
