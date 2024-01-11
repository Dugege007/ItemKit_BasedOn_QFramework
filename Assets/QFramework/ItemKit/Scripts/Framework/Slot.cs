
// Framework 文件夹中保存框架中的概念
namespace QFramework
{
    public class Slot
    {
        public IItem Item;
        public int Count;

        public Slot(IItem item = null, int count = 0)
        {
            Item = item;
            Count = count;
        }
    }
}
