using UnityEngine;
using QFramework;

namespace QFramework.Example
{
    public class Items
    {
        private static IItem _item_green_sword;
        public static IItem item_green_sword
        {
            get
            {
                if (_item_green_sword == null)
                    _item_green_sword = ItemKit.ItemByKey["item_green_sword"];
                return _item_green_sword;
            }
        }
        public static string item_green_sword_key = "item_green_sword";
        private static IItem _item_iron;
        public static IItem item_iron
        {
            get
            {
                if (_item_iron == null)
                    _item_iron = ItemKit.ItemByKey["item_iron"];
                return _item_iron;
            }
        }
        public static string item_iron_key = "item_iron";
        private static IItem _item_paper;
        public static IItem item_paper
        {
            get
            {
                if (_item_paper == null)
                    _item_paper = ItemKit.ItemByKey["item_paper"];
                return _item_paper;
            }
        }
        public static string item_paper_key = "item_paper";
        private static IItem _item_wood;
        public static IItem item_wood
        {
            get
            {
                if (_item_wood == null)
                    _item_wood = ItemKit.ItemByKey["item_wood"];
                return _item_wood;
            }
        }
        public static string item_wood_key = "item_wood";
    }
}
