using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupItemData : InteractableItemData
{
    public float value;   // 아이템을 주웠을 때 플레이어가 무언가를 회복할 값을 저장할 변수

    public PickupItemData(string name, int id, ItemType itemtype, float value, string prefabPath, Sprite sprite) : base(name, id, itemtype,prefabPath, sprite)
    {
        this.value = value;
    }
}
