using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveItemData : InteractableItemData
{
    public int value;   // 아이템을 얻었을 때 플레이어가 무언가를 얻을 값을 저장할 변수

    public PassiveItemData(string name, int id, ItemType itemtype, int value, string prefabPath, Sprite sprite) : base(name, id, itemtype, prefabPath, sprite)
    {
        this.value = value;
    }
}
