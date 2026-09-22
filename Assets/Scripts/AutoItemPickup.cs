using UnityEngine;

public class AutoItemPickup : MonoBehaviour
{
    [SerializeField] private float pickupRange = 0.5f;
    [SerializeField] private float moveSpeed = 1f;

    private Transform player;
    private bool isMoving = false;
    private GameObject inventoryObject;

    private Sprite itemSprite;

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Character");

        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogError("Character 태그를 가진 플레이어를 찾지 못했습니다!");
        }

        inventoryObject = GameObject.FindGameObjectWithTag("Inventory");

        if (inventoryObject == null)
        {
            Debug.LogError("Inventory 태그를 가진 오브젝트를 찾지 못했습니다!");
        }

        // 아이템의 SpriteRenderer에서 Sprite 가져오기
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            itemSprite = spriteRenderer.sprite;
            Debug.Log("아이템 Sprite 가져옴: " + itemSprite.name);
        }
        else
        {
            Debug.LogError(gameObject.name + "에 SpriteRenderer가 없습니다!");
        }
    }

    private void Update()
    {
        if (player == null)
            return;

        if (!isMoving &&
            Vector3.Distance(transform.position, player.position) <= pickupRange)
        {
            isMoving = true;
        }

        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                player.position,
                moveSpeed * Time.deltaTime
            );

            if (Vector3.Distance(transform.position, player.position) < 0.1f)
            {
                Debug.Log("플레이어에게 아이템 도착!");
                Pickup();
            }
        }
    }

    private void Pickup()
    {
        Debug.Log("아이템 습득!");

        if (inventoryObject == null)
        {
            Debug.LogError("Inventory 오브젝트가 없습니다!");
            return;
        }

        InventoryManager inventory =
            inventoryObject.GetComponent<InventoryManager>();

        if (inventory != null)
        {
            Debug.Log("InventoryManager 찾음!");

            if (itemSprite != null)
            {
                Debug.Log("인벤토리에 넣을 Sprite: " + itemSprite.name);
                inventory.AddItem(itemSprite);
            }
            else
            {
                Debug.LogError("itemSprite가 NULL입니다!");
            }
        }
        else
        {
            Debug.LogError("Inventory 오브젝트에 InventoryManager가 없습니다!");
        }

        Destroy(gameObject);
    }
}