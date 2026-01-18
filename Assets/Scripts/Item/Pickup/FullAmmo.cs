using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullAmmo : PickupItem
{
    void Start()
    {
        SetItemData(2);
        outline.outlineSize = 8;
        ItemSpawn();
    }

    protected override void InteractPlayer(Player player)
    {
        base.InteractPlayer(player);
        player.SupplyAmmo(base.value);

        Destroy(gameObject, 0.1f);
    }
}
