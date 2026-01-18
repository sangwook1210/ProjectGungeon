using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyNormal : Enemy
{
    protected int EnemyNormalScore;
    protected float FireCoolTime;
    protected bool CanFire = true;
    protected float HitTime = 0.3f;

    protected Vector3 BulletDir;    // EnemyNormal을 죽인 Bullet의 발사 방향
    protected float BulletForce;    // EnemyNormal을 죽인 Bullet의 Force

    public Sprite[] HitSprite = new Sprite[5];

    protected int DropPercent = 100;

    protected void EnemyNormalInit(CharType enemyType, float enemyHp, float enemySpeed, float enemyDamage, float collisionDamage, float alertDis, 
        GameObject gun, float fireCoolTime, int dropPrecent, int score)
    {
        EnemyInit(enemyType, enemyHp, enemySpeed, enemyDamage, collisionDamage, alertDis, gun);

        FireCoolTime = fireCoolTime;
        DropPercent = dropPrecent;
        EnemyNormalScore = score;
    }

    protected override void EnemyFireBullet()
    {
        // 총알을 쏠 수 있고 총알을 맞은 상태가 아니라면
        if (CanFire&&!IsHit)
        {
            if (Gun.GetComponent<Gun>().remainAmmoInMagazine != 0)
            {
                Gun.GetComponent<Gun>().SetGunmanTransform(gameObject);
                Gun.GetComponent<Gun>().FireBullet(Player.transform.position);

                CanFire = false;
                StartCoroutine(EnemyFireCoolTime());
            }
            else
            {
                if (!Gun.GetComponent<Gun>().isReloading)
                {
                    Gun.GetComponent<Gun>().ReloadMagazine();
                }
            }
        }
    }

    IEnumerator EnemyFireCoolTime()
    {
        yield return new WaitForSeconds(FireCoolTime);
        CanFire = true;
    }

    protected override void CharHit(Collider hitObject)
    {
        if (hitObject.gameObject.CompareTag("Bullet"))
        {
            float damage = hitObject.GetComponent<Bullet>().BulletHit(charType, gameObject);
            BulletDir = hitObject.GetComponent<Bullet>().BulletDir;
            BulletForce = hitObject.GetComponent<Bullet>().Force;
            CharHpDecrease(damage);
        }
    }

    protected override void CharHpDecrease(float damage)
    {
        Hp -= damage;

        if (Hp <= 0f)
        {
            CharDead();
        }
        if(damage!=0&&Hp>0)
        {
            IsHit = true;
            EnemyKnockBack(BulletForce,BulletDir, _IsDead);
        }
    }

    protected void EnemyKnockBack(float force, Vector3 dir, bool isDead)
    {
        EnemyRigid.isKinematic = false;
        EnemyRigid.AddForce(CameraEulerAngle.ReviseMagnitude(force,dir)*dir.normalized, ForceMode.Impulse);

        if (!_IsDead)
        {
            StartCoroutine(EnemyHitTime(dir));
        }
    }

    IEnumerator EnemyHitTime(Vector3 dir)
    {
        enemyAnim.enabled = false;

        if (dir.x > 0)
        {
            EnemySprite.flipX = false;
        }
        else
        {
            EnemySprite.flipX = true;
        }

        int HitSpriteNum = Random.Range(0, 5);
        EnemySprite.sprite = HitSprite[HitSpriteNum];

        yield return new WaitForSeconds(HitTime);
        IsHit = false;
        enemyAnim.enabled = true;
        EnemyRigid.velocity = Vector3.zero;
        EnemyRigid.isKinematic = true;
    }

    // 확률에 따라 아이템 타입이 Pickup인 아이템을 드롭하는 함수
    protected void EnemyNormalDropItem()
    {
        int CheckDrop = Random.Range(0,100);  // 0~99

        if (CheckDrop < DropPercent)
        {
            List<ItemData> DropList = _itemDB.FindItemDataWithItemType(ItemType.Pickup);
            int ListSize = DropList.Count;

            int setItem=Random.Range(0,ListSize);

            GameObject DropItem = Instantiate(DropList[setItem].ItemPrefab, gameObject.transform.position,Quaternion.identity );

            if (DropItem.GetComponent<Rigidbody>() != null)
            {
                DropItem.GetComponent<Rigidbody>().AddForce(new Vector3(0, 4,-2), ForceMode.Impulse);
            }
        }
    }

    // Enemy가 죽었을 때 시체로 변하는 함수
    public override void EnemyCorpse()
    {
        EnemySprite.color = new Vector4(0.6f, 0.6f, 0.6f, 1f);
        EnemySprite.sortingOrder = -2;
        EnemyRigid.Sleep();
        gameObject.GetComponent<CapsuleCollider>().enabled = false;

        // 아이템 드롭
        EnemyNormalDropItem();

        Destroy(gameObject, 10.0f);
    }
}
