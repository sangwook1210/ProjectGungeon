using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : Enemy
{
    protected float wallHp = 10000f;
    protected float wallSpeed = 0f;

    private void Start()
    {
        base.IsReady = true;
        base.CharInit(CharType.Wall, wallHp, wallSpeed);
    }

    // Wall은 Enemy로 분류되어 있지만 데미지가 들어가는 코드가 들어가있지 않다
    protected override void CharHit(Collider hitObject)
    {
        if (hitObject.gameObject.CompareTag("Bullet"))
        {
            hitObject.GetComponent<Bullet>().BulletHit(base.charType);
        }
    }
}
