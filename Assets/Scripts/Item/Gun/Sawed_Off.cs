using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sawed_Off : SemiautomaticGun
{
    public GameObject normalBullet;

    void Start()
    {
        // Sawed_Off Ω∫≈› º≥¡§
        SetItemData(102);

        Bullet = normalBullet;
        base.muzzlePos = transform.GetChild(0);

        // ªÍ≈∫√—
        bulletCount = 4;

        SetGunLocalPos();

        canFire = true;
    }
}
