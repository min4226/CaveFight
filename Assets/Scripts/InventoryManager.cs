using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private Transform itemContent;
    [SerializeField] private GameObject itemSlotPrefab;

    public void AddItem(Sprite sprite)
    {
        // 이미 같은 아이템이 있는지 확인
        foreach (Transform child in itemContent)
        {
            ItemSlot itemSlot = child.GetComponent<ItemSlot>();

            if (itemSlot != null && itemSlot.IsSameItem(sprite))
            {
                // 같은 아이템이면 개수만 증가
                itemSlot.AddCount();
                return;
            }
        }

        // 같은 아이템이 없다면 새로운 슬롯 생성
        GameObject newSlot = Instantiate(itemSlotPrefab, itemContent);

        ItemSlot newItemSlot = newSlot.GetComponent<ItemSlot>();

        if (newItemSlot != null)
        {
            newItemSlot.Init(sprite);
        }
    }
}