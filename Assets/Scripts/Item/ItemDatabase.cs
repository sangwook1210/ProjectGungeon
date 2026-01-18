using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    private List<ItemData> items = new List<ItemData>();

    // id를 이용하여 itemData 리스트에서 원하는 아이템을 찾는 함수
    public ItemData FindItemDataWithId(int id)
    {
        ItemData returnItemData = null;

        foreach(ItemData itemdata in items)
        {
            if (itemdata.id == id)
            {
                returnItemData= itemdata;
                break;
            }
        }

        return returnItemData;
    }

    // ItemType을 이용하여 itemData 리스트에서 원하는 아이템의 리스트를 찾는 함수
    public List<ItemData> FindItemDataWithItemType(ItemType type)
    {
        List<ItemData> ItemDataList = new List<ItemData>();

        foreach(ItemData itemdata in items)
        {
            if(itemdata.itemtype== type)
            {
                ItemDataList.Add(itemdata);
            }
        }

        return ItemDataList;
    }
    private Sprite[] PropSprites;
    private Sprite[] GunSprites1;

    private void Awake()
    {
        PropSprites = Resources.LoadAll<Sprite>("Sprites/Items/Etc/TX Props");
        GunSprites1 = Resources.LoadAll<Sprite>("Sprites/Items/Gun/guns1");

        items.Add(new PickupItemData("절반 하트", 0, ItemType.Pickup, 1, "Prefabs/Items/Pickup/HalfHeart", Resources.Load<Sprite>("Sprites/Items/Pickup/Heart_Half")));
        items.Add(new PickupItemData("하트", 1, ItemType.Pickup, 2, "Prefabs/Items/Pickup/FullHeart", Resources.Load<Sprite>("Sprites/Items/Pickup/Heart_Full")));
        items.Add(new PickupItemData("탄약 보급", 2, ItemType.Pickup, 1, "Prefabs/Items/Pickup/FullAmmo", Resources.Load<Sprite>("Sprites/Items/Pickup/Ammo")));

        items.Add(new InteractableItemData("아이템 상자", 10, ItemType.Etc, "Prefabs/Items/Etc/Chest", PropSprites[9]));
        items.Add(new PassiveItemData("하트 목걸이", 11, ItemType.Passive, 2, "Prefabs/Items/Passive/Heart Necklace", Resources.Load<Sprite>("Sprites/Items/Passive/Heart Necklace")));

        items.Add(new GunItemData("매그넘", 100, ItemType.Gun, GunType.Semiautomatic, 6, 140, 13, 0.15f, 1f, 10, 15, 10, 7, "Prefabs/Items/Guns/Magnum", Resources.Load<Sprite>("Sprites/Items/Gun/Magnum")));
        items.Add(new GunItemData("저가형 리볼버", 101, ItemType.Gun, GunType.Semiautomatic, 5, 100, 6, 0.15f, 0.9f, 17, 18, 2, 10, "Prefabs/Items/Guns/Budget_Revolver", Resources.Load<Sprite>("Sprites/Items/Gun/Budget_Revolver")));
        items.Add(new GunItemData("총신 개조 산탄총", 102, ItemType.Gun, GunType.Semiautomatic, 6, 60, 4, 0.5f, 1.2f, 20, 10, 2, 25, "Prefabs/Items/Guns/Sawed_Off", GunSprites1[0]));
    }
}
