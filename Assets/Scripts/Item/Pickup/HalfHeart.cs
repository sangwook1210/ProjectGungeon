using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HalfHeart : PickupItem
{
    void Start()
    {
        SetItemData(0);
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
