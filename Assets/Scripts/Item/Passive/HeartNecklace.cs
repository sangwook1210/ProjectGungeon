using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartNecklace : PassiveItem
{
    protected override void InteractPlayer(Player player)
    {
        base.InteractPlayer(player);

        player.PlayerMaxHpChange(true, base.value, true);

        Destroy(gameObject, 0.1f);
    }

    private void Start()
    {
        SetItemData(11);
        outline.outlineSize = 1;
    }
}
