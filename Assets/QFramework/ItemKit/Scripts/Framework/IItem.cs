﻿
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
        public Sprite GetIcon { get; }
    }
}