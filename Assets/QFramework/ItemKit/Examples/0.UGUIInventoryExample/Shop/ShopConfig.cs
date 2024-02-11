using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework.Example
{
    public class ShopConfig : MonoBehaviour
    {
        [LabelText("…ÃµÍ√˚")]
        public string ShopName;

        public List<ShopBuyItem> BuyItems;

        private void Start()
        {
        }
    }
}