using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Enemy_BulletKin : EnemyNormal
{
    protected CharType bulletKinType = CharType.Normal;
    protected float bulletKinHp = 15f;      //15f
    protected float bulletKinDamage = 1f;
    protected float bulletKinColDamage = 1f;
    protected float bulletKinSpeed = 3f;
    protected float bulletKinFireCoolTime;
    protected float bulletKinAlertDis = 7.0f;
    protected int bulletKinDropPercent = 30;

    private int bulletKinScore = 1;

    public GameObject Magnum;

    

    protected override void CharDead()
    {
        if (!_IsDead)
        {
            // 죽음 설정
            _IsDead = true;
            enemyAnim.SetBool("IsDead", true);

            // 손과 총 사라지게 하기
            enemyHand.SetActive(false);
            Gun.SetActive(false);

            // 넉백 설정
            EnemyKnockBack(BulletForce * 1.5f, BulletDir, _IsDead);

            // 애니메이션 설정
            int DeathAnimNum = Random.Range(0, 5);
            string DeathAnimTrigger = "Death" + DeathAnimNum.ToString();
            enemyAnim.SetTrigger(DeathAnimTrigger);

            GameManager.Instance.IncreaseScore(EnemyNormalScore);

            PlayAudioClip(DeathClip);
        }
    }

    


    private void Start()
    {
        bulletKinFireCoolTime = Random.Range(1.5f, 3.0f);

        EnemyNormalInit(bulletKinType, bulletKinHp, bulletKinSpeed, bulletKinDamage, bulletKinColDamage, bulletKinAlertDis, Magnum, bulletKinFireCoolTime,bulletKinDropPercent,bulletKinScore);
        FindPlayer();

        Gun.GetComponent<Gun>().SetGunman(charType);
        Gun.GetComponent<Gun>().SetEnemyDamage(EnemyBulletDamage);

        rightHandPos = enemyHand.transform.localPosition;
    }

    private void Update()
    {
        if (!_IsDead&&IsReady)
        {
            EnemyFireBullet();
            SetAlertDis();
        }
    }

    private void FixedUpdate()
    {
        // 경로 찾기 요청
        if (!_IsDead&&IsReady)
        {
            PathRequestManager.RequestPath(gameObject, transform.position, Player.transform.position, OnPathFound);

            RotateEnemySprite();
        }
    }
}
