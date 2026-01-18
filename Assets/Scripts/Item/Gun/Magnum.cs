using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnum : SemiautomaticGun
{
    public GameObject normalBullet;

    void Start()
    {
        // Magnum ½ºÅÝ ¼³Á¤
        SetItemData(100);

        Bullet = normalBullet;
        base.muzzlePos = transform.GetChild(0);

        // ´Ü¹ßÇü ÃÑ
        bulletCount = 1;

        SetGunLocalPos();

        canFire = true;
    }

    
}
