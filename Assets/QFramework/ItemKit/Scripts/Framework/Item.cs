
using UnityEngine;

namespace QFramework
{
    // 由于之前的 Item 类和新创建的 ItemConfig 类，两者有很多相同的功能
    // 但是 Item 类没有继承自任何类，ItemConfig 类继承自 ScriptableObject 类，无法提取他们共同的父类
    // 这时可以通过接口的方式来提取他们的共同功能
    public interface IItem
    {
        public string GetKey { get; }
        public string GetName { get; }
        public Sprite GetSprite { get; }
    }

    public class Item : IItem
    {
        public string Key;  // 或者是ID，用于查找
        public string Name;

        public Item(string key, string name)
        {
            Key = key;
            Name = name;
        }

        public string GetKey => Key;

        public string GetName => Name;

        public Sprite GetSprite => null;
    }
}
