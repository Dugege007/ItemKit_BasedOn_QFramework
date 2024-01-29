using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QFramework
{
    public interface IItemKitSaveAndLoader
    {
        void Save(Dictionary<string, SlotGroup> slotGroups);
        void Load(Dictionary<string, SlotGroup> slotGroups);
    }

    public class DefaultItemKitSaverAndLoader : IItemKitSaveAndLoader
    {
        public void Save(Dictionary<string, SlotGroup> slotGroups)
        {

        }

        public void Load(Dictionary<string, SlotGroup> slotGroups)
        {
            
        }
    }
}
