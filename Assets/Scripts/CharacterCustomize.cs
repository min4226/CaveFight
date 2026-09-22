using UnityEngine;
using UnityEngine.UI;
public class CharacterCustomize : MonoBehaviour
{
    [Header("Character Sprite")]
    Image hairRenderer;
    Image topRenderer;
    Image bottomRenderer;
    Image shoesRenderer;

    [Header("Hair")]
    [SerializeField] Sprite[] hairSprites;

    [Header("Top")]
    [SerializeField] Sprite[] topSprites;

    [Header("Bottom")]
    [SerializeField] Sprite[] bottomSprites;

    [Header("Shoes")]
    [SerializeField] Sprite[] shoesSprites;

    
    int hairIndex;
    int topIndex;
    int bottomIndex;
    int shoesIndex;

    void Awake()
    {
        GameObject character = GameObject.FindWithTag("Character");

        Transform hair = character.transform.Find("Hair");
        Transform top = character.transform.Find("Tops");
        Transform bottom = character.transform.Find("Bottoms");
        Transform shoes = character.transform.Find("Shoes");

        
        if (hair != null) hairRenderer = hair.GetComponent<Image>();
        if (top != null) topRenderer = top.GetComponent<Image>();
        if (bottom != null) bottomRenderer = bottom.GetComponent<Image>();
        if (shoes != null) shoesRenderer = shoes.GetComponent<Image>();
    }
    public void ChangeHair(int index)
    {
        if (index < 0 || index >= hairSprites.Length)
            return;

        hairIndex = index;
        hairRenderer.sprite = hairSprites[index];
    }

    public void ChangeTop(int index)
    {
        if (index < 0 || index >= topSprites.Length)
            return;

        topIndex = index;
        topRenderer.sprite = topSprites[index];
    }

    public void ChangeBottom(int index)
    {
        if (index < 0 || index >= bottomSprites.Length)
            return;

        bottomIndex = index;
        bottomRenderer.sprite = bottomSprites[index];
    }

    public void ChangeShoes(int index)
    {
        if (index < 0 || index >= shoesSprites.Length)
            return;

        shoesIndex = index;
        shoesRenderer.sprite = shoesSprites[index];
    }
    public void PreviousHair()
    {
        hairIndex--;

        if (hairIndex < 0)
            hairIndex = hairSprites.Length - 1;

        hairRenderer.sprite = hairSprites[hairIndex];
    }

    public void NextHair()
    {
        hairIndex++;

        if (hairIndex >= hairSprites.Length)
            hairIndex = 0;

        hairRenderer.sprite = hairSprites[hairIndex];
    }
    public void PreviousTop()
    {
        topIndex--;

        if (topIndex < 0)
            topIndex = topSprites.Length - 1;

        topRenderer.sprite = topSprites[topIndex];
    }

    public void NextTop()
    {
        topIndex++;

        if (topIndex >= topSprites.Length)
            topIndex = 0;

        topRenderer.sprite = topSprites[topIndex];
    }
    public void PreviousBottom()
    {
        bottomIndex--;

        if (bottomIndex < 0)
            bottomIndex = bottomSprites.Length - 1;

        bottomRenderer.sprite = bottomSprites[bottomIndex];
    }

    public void NextBottom()
    {
        bottomIndex++;

        if (bottomIndex >= bottomSprites.Length)
            bottomIndex = 0;

        bottomRenderer.sprite = bottomSprites[bottomIndex];
    }
    public void PreviousShoes()
    {
        shoesIndex--;

        if (shoesIndex < 0)
            shoesIndex = shoesSprites.Length - 1;

        shoesRenderer.sprite = shoesSprites[shoesIndex];
    }

    public void NextShoes()
    {
        shoesIndex++;

        if (shoesIndex >= shoesSprites.Length)
            shoesIndex = 0;

        shoesRenderer.sprite = shoesSprites[shoesIndex];
    }

    public void SaveCustomization()
    {
        
        PlayerPrefs.SetInt("SavedHairIndex", hairIndex);
        PlayerPrefs.SetInt("SavedTopIndex", topIndex);
        PlayerPrefs.SetInt("SavedBottomIndex", bottomIndex);
        PlayerPrefs.SetInt("SavedShoesIndex", shoesIndex);

        
        PlayerPrefs.Save();

        Debug.Log("캐릭터 커스터마이징 정보가 저장되었습니다!");
    }

    public void LoadCustomization()
    {
        hairIndex = PlayerPrefs.GetInt("SavedHairIndex", 0);
        topIndex = PlayerPrefs.GetInt("SavedTopIndex", 0);
        bottomIndex = PlayerPrefs.GetInt("SavedBottomIndex", 0);
        shoesIndex = PlayerPrefs.GetInt("SavedShoesIndex", 0);

        
        ApplyCustomization();
    }

    private void ApplyCustomization()
    {
        if (hairSprites != null && hairSprites.Length > hairIndex && hairRenderer != null)
            hairRenderer.sprite = hairSprites[hairIndex];

        if (topSprites != null && topSprites.Length > topIndex && topRenderer != null)
            topRenderer.sprite = topSprites[topIndex];

        if (bottomSprites != null && bottomSprites.Length > bottomIndex && bottomRenderer != null)
            bottomRenderer.sprite = bottomSprites[bottomIndex];

        if (shoesSprites != null && shoesSprites.Length > shoesIndex && shoesRenderer != null)
            shoesRenderer.sprite = shoesSprites[shoesIndex];
    }
}
