using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Budget_Revolver : SemiautomaticGun
{
    public GameObject normalBullet;

    void Start()
    {
        // Budget_Revolver ½ºÅÝ ¼³Á¤
        SetItemData(101);

        Bullet = normalBullet;
        base.muzzlePos = transform.GetChild(0);


        // ´Ü¹ßÇü ÃÑ
        bulletCount = 1;

        SetGunLocalPos();

        canFire = true;
    }
}
