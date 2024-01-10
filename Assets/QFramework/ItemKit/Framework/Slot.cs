
// Framework 文件夹中保存框架中的概念
namespace QFramework
{
    public class Slot
    {
        public Item Item;
        public int Count;

        public Slot(Item item, int count = 0)
        {
            Item = item;
            Count = count;
        }
    }
}
