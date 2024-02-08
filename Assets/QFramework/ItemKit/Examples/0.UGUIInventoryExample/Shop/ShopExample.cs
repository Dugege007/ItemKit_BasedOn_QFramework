using UnityEngine;
using QFramework;
using UnityEngine.UI;

// 1.请在菜单 编辑器扩展/Namespace Settings 里设置命名空间
// 2.命名空间更改后，生成代码之后，需要把逻辑代码文件（非 Designer）的命名空间手动更改
namespace QFramework.Example
{
    public partial class ShopExample : ViewController
    {
        public class ShopByItem
        {
            public IItem Item { get; set; }
            public int Price { get; set; }
        }

        public static BindableProperty<int> Coin = new(100);

        private void Awake()
        {
            // 加载
            Coin.Value = PlayerPrefs.GetInt(nameof(Coin), 100);

            // 保存
            Coin.Register(coin =>
            {
                PlayerPrefs.SetInt(nameof(Coin), coin);

            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void Start()
        {
            UIShopItem.Hide();

            int price = 10;

            UIShopItem shopItem = UIShopItem.InstantiateWithParent(ShopItemRoot);
            shopItem.UISlot.InitWithData(new Slot(Items.item_iron, 1, ItemKit.GetSlotGroupByKey("商店")));
            shopItem.Name.text = Items.item_iron.GetName;
            shopItem.Description.text = Items.item_iron.GetDescription;
            shopItem.PriceText.text = price.ToString();
            shopItem.BtnBuy.onClick.AddListener(() =>
            {
                ItemKit.GetSlotGroupByKey("物品栏").AddItem(Items.item_iron);
                Coin.Value -= price;
            });

            Coin.RegisterWithInitValue(coin =>
            {
                if (coin >= price)
                {
                    shopItem.BtnBuy.interactable = true;
                    shopItem.PriceText.color = Color.green;
                }
                else
                {
                    shopItem.BtnBuy.interactable = false;
                    shopItem.PriceText.color = Color.red;
                }

            }).UnRegisterWhenGameObjectDestroyed(shopItem.gameObject);

            shopItem.Show();
        }

        private void OnGUI()
        {
            IMGUIHelper.SetDesignResolution(640, 360);
            GUILayout.Label("Coin: " + Coin.Value);
        }
    }
}
