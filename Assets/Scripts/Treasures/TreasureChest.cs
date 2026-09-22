using UnityEngine;

public class TreasureChest : MonoBehaviour
{
    [SerializeField] Stage stage;
    [SerializeField] int treasureIndex;
    [SerializeField] Transform spawnPoint;

    private bool isOpened = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("들어온 오브젝트: " + other.gameObject.name);
        Debug.Log("현재 태그: " + other.gameObject.tag);
        Debug.Log("부모: " + other.transform.parent?.name);
        Debug.Log("루트: " + other.transform.root.name);
        Debug.Log("루트 태그: " + other.transform.root.tag);

        if (!other.transform.root.CompareTag("Player"))
        {
            Debug.Log("Player 태그가 아님!");
            return;
        }

        Debug.Log("Player 태그 확인!");
        OpenChest();
    }

    private void OpenChest()
    {
        if (isOpened)
            return;

        isOpened = true;

        Treasure treasure = stage.stageTreasures[treasureIndex];

        Debug.Log("생성할 아이템: " + treasure.treasureItemName);
        Debug.Log("프리팹: " + treasure.treasureItemPrefab);

        Instantiate(
            treasure.treasureItemPrefab,
            spawnPoint.position,
            Quaternion.identity
        );
    }
}