using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractableItem : Item
{
    protected bool IsInteractable = true;   // 상호작용 가능한지
    protected bool IsInteracted = false;    // 상호작용 되었는지

    protected InteractableItemData interactableItemData;

    protected SpriteOutline outline;    // 외곽선 스크립트

    protected virtual void InteractableInit(string name, int id, ItemType itemtype, Sprite itemSprite)
    {
        ItemInit(name, id, itemtype, itemSprite);

        outline = gameObject.GetComponent<SpriteOutline>();
    }

    protected override void SetItemData(int id)
    {
        SetItemDatabase();

        if (_itemDB != null)
        {
            itemData = _itemDB.FindItemDataWithId(id);
            interactableItemData = itemData as InteractableItemData;

            InteractableInit(interactableItemData.name, interactableItemData.id, interactableItemData.itemtype,interactableItemData.itemSprite);
        }
    }

    // 플레이어와 상호작용하였을 때 실행될 함수
    protected virtual void InteractPlayer(Player player)
    {
        PlayAudioClip(PickUpClip);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")&&IsInteractable)
        {
            outline.showOutline = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            outline.showOutline = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (IsInteractable)
            {
                outline.showOutline = true;
            }
            if (Input.GetAxisRaw("Interact") == 1 && !IsInteracted&&IsInteractable)
            {
                InteractPlayer(other.gameObject.GetComponent<Player>());
                IsInteracted = true;
            }
        }
    }
}
