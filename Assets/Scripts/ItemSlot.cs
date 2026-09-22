using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlot : MonoBehaviour
{
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI itemCountText;

    private int itemCount;

    public void Init(Sprite sprite)
    {
        itemImage.sprite = sprite;

        itemCount = 1;
        itemCountText.text = itemCount.ToString();
    }

    public bool IsSameItem(Sprite sprite)
    {
        return itemImage.sprite == sprite;
    }

    public void AddCount()
    {
        itemCount++;
        itemCountText.text = itemCount.ToString();
    }
}