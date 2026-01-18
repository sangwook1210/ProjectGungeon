using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveItem : InteractableItem
{
    protected PassiveItemData passiveItemData;
    protected float value;
    protected float InteractTime = 1.0f;

    protected void PassiveInit(string name, int id, ItemType itemtype, Sprite itemSprite,float value)
    {
        InteractableInit(name, id, itemtype, itemSprite);
        this.value = value;

        base.IsInteractable = false;
        Invoke("SetInteractable", InteractTime);
    }

    protected override void SetItemData(int id)
    {
        SetItemDatabase();

        if (_itemDB != null)
        {
            itemData = _itemDB.FindItemDataWithId(id);
            passiveItemData = itemData as PassiveItemData;

            PassiveInit(passiveItemData.name, passiveItemData.id, passiveItemData.itemtype, passiveItemData.itemSprite,passiveItemData.value);
        }
    }

    // 상호작용 가능하게 만드는 함수
    protected void SetInteractable()
    {
        base.IsInteractable = true;
    }

    protected override void InteractPlayer(Player player)
    {
        base.InteractPlayer(player);
        player.GetNewItem(gameObject);
    }
}
