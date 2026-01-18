using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Gun, Pickup, Passive, Etc
}

[System.Serializable]
public class ItemData
{

    [Header("=== 아이템 정보 ===")]
    [Tooltip("아이템의 이름")] public string name;
    [Tooltip("아이템 고유 번호")] public int id;
    [Tooltip("아이템 종류")] public ItemType itemtype;
    [Tooltip("아이템 스프라이트")] public Sprite itemSprite;

    public GameObject ItemPrefab;


    public ItemData(string name, int id,  ItemType itemtype, string prefabPath, Sprite sprite)
    {
        this.name = name;
        this.id = id;
        this.itemtype = itemtype;

        this.ItemPrefab = Resources.Load<GameObject>(prefabPath);
        this.itemSprite = sprite;
    }
}
