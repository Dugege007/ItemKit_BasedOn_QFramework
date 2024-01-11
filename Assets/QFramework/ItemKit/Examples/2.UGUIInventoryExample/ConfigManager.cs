using System;
using UnityEngine;

namespace QFramework.Example
{
    // 定义一个ConfigManager类，用于管理游戏内的配置
    public class ConfigManager
    {
        // 定义一个静态的 Lazy<IItem> 变量 Iron，用于懒加载 Iron 配置
        // Lazy<IItem> 意味着这个变量的值将在第一次访问时被初始化
        public static Lazy<IItem> Iron = new Lazy<IItem>(() =>
            // 使用 Unity 的 Resources.Load 方法来加载名为 Iron 的 ItemConfig 资源
            // 这里的 Lambda 表达式定义了一个匿名函数，用于初始化 Lazy 变量
            Resources.Load<ItemConfig>("Iron"));

        // 同上
        public static Lazy<IItem> GreenSword = new Lazy<IItem>(() => Resources.Load<ItemConfig>("GreenSword"));
    }
}
