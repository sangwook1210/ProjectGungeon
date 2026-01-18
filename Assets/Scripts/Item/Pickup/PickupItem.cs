using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PickupItem : InteractableItem
{
    protected PickupItemData pickupItemData;
    protected float value;


    protected float existTime = 30.0f;

    protected void PickupInit(string name, int id, ItemType itemtype, Sprite itemSprite,float value)
    {
        InteractableInit(name, id, itemtype, itemSprite);
        this.value = value;

        Destroy(gameObject, existTime);
    }

    protected override void SetItemData(int id)
    {
        SetItemDatabase();

        if (_itemDB != null)
        {
            itemData = _itemDB.FindItemDataWithId(id);
            pickupItemData = itemData as PickupItemData;

            PickupInit(pickupItemData.name, pickupItemData.id, pickupItemData.itemtype, pickupItemData.itemSprite,pickupItemData.value);
        }
    }
}
