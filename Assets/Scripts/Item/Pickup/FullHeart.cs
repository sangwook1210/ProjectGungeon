using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullHeart : PickupItem
{
    void Start()
    {
        SetItemData(1);
        outline.outlineSize = 1;
        ItemSpawn();
    }

    protected override void InteractPlayer(Player player)
    {
        base.InteractPlayer(player);
        player.CharHpIncrease(base.value);

        Destroy(gameObject, 0.1f);
    }
}
