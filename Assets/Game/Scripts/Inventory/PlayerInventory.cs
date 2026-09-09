using System.Collections.Generic;
using UnityEngine;

namespace LightJam
{
    /// <summary>
    /// 简易物品栏：用字符串 ID 记录已获得的线索/钥匙，供解密条件判断。
    /// </summary>
    public class PlayerInventory : MonoBehaviour
    {
        public static PlayerInventory Instance { get; private set; }

        readonly HashSet<string> items = new HashSet<string>();

        void Awake()
        {
            Instance = this;
        }

        void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        public bool Has(string itemId)
        {
            return !string.IsNullOrEmpty(itemId) && items.Contains(itemId);
        }

        public void Add(string itemId)
        {
            if (string.IsNullOrEmpty(itemId) || items.Contains(itemId))
                return;

            items.Add(itemId);
            Debug.Log($"[物品栏] 获得：{itemId}");
        }

        public void Remove(string itemId)
        {
            if (string.IsNullOrEmpty(itemId))
                return;
            items.Remove(itemId);
        }
    }
}
