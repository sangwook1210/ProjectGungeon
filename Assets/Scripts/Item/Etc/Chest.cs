using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Chest : InteractableItem
{
    public Animator ChestAnim;

    protected override void InteractableInit(string name, int id, ItemType itemtype, Sprite itemSprite)
    {
        ItemInit(name, id, itemtype,itemSprite);

        outline = transform.GetChild(0).gameObject.GetComponent<SpriteOutline>();
    }

    protected override void InteractPlayer(Player player)
    {
        base.InteractPlayer(player);
        ChestAnim.SetTrigger("Open");
        DropItem();

        base.IsInteractable = false;
    }
    
    // 상자를 열었을 때 아이템을 드롭하는 함수
    protected void DropItem()
    {
        List<ItemData> DropList = _itemDB.FindItemDataWithItemType(ItemType.Passive);
        int ListSize = DropList.Count;

        int setItem = Random.Range(0, ListSize);

        Vector3 SpawnPos = gameObject.transform.position + new Vector3(0, 0, -1.0f);

        GameObject DropItem = Instantiate(DropList[setItem].ItemPrefab, gameObject.transform.position,
            Quaternion.Euler(CameraEulerAngle.cameraEulerAngle.x, CameraEulerAngle.cameraEulerAngle.y, CameraEulerAngle.cameraEulerAngle.z));

        if (DropItem.GetComponent<Rigidbody>() != null)
        {
            DropItem.GetComponent<Rigidbody>().AddForce(new Vector3(0, 4,-2), ForceMode.Impulse);
        }
    }

    void Start()
    {
        SetItemData(10);
        outline.outlineSize = 1;

        
    }
}
