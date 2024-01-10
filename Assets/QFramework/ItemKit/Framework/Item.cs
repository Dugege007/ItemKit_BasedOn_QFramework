
namespace QFramework
{
    public class Item
    {
        public string Key;  // 或者是ID，用于查找
        public string Name;

        public Item(string key, string name)
        {
            Key = key;
            Name = name;
        }
    }
}
