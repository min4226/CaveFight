using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DropDown : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    [SerializeField] GameObject backgroundPanel1;
    [SerializeField] GameObject backgroundPanel2;
    [SerializeField] GameObject backgroundPanel3;
    [SerializeField] GameObject backgroundPanel4;
    private void Start()
    {
        CreateDropdown();

        dropdown.onValueChanged.AddListener(SelectBackground);

        SelectBackground(dropdown.value);
    }

    private void CreateDropdown()
    {
        dropdown.ClearOptions();

        List<string> options = new List<string>();

        // enum을 쓰고 있기 때문에 enum.getvalues를 사용
        foreach (BackGroundType background in Enum.GetValues(typeof(BackGroundType)))
        {
            options.Add(background.ToString());
        }

        dropdown.AddOptions(options);
    }

    public void SelectBackground(int index)
    {
        // 일단 전부 끄기
        backgroundPanel1.SetActive(false);
        backgroundPanel2.SetActive(false);
        backgroundPanel3.SetActive(false);
        backgroundPanel4.SetActive(false);

        switch (index)
        {
            case 0:
                backgroundPanel1.SetActive(true);
                break;

            case 1:
                backgroundPanel2.SetActive(true);
                break;

            case 2:
                backgroundPanel3.SetActive(true);
                break;

            case 3:
                backgroundPanel4.SetActive(true);
                break;
        }
    }
}
