using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Character.CharType gunman;

    private float Speed;    // 탄환의 속도
    private float minSpeed = 0f;   // 탄환의 최소 속도
    private float maxSpeed = 50f;  // 탄환의 최대 속도

    private float damage;
    public float Damage
    {
        get
        {
            return damage;
        }

        set
        {
            if (value > minDamage && value <= maxDamage)
            {
                damage = value;
            }
            else
            {
                Debug.LogError($"허용되지 않은 damage 값입니다: {value}, {gunman}");
                damage = minDamage;
            }
        }
    }
    private float minDamage=0f;
    private float maxDamage = 100.0f;

    private float Range;    // 탄환이 최대로 날아갈 수 있는 거리
    private float Distance; // 탄환이 현재 날아간 거리
    public float Force; // 탄환이 가진 힘

    public Vector3 BulletDir;
    private Vector3 BulletStartPos;

    [SerializeField] private Rigidbody BulletRigid;

    // Bullet을 발사할 방향을 정규화하는 함수
    private Vector3 SetBulletDir(Vector3 bulletDir)
    {
        Vector3 BulletDir = new Vector3(0, 0, 0);

        BulletDir.Set(bulletDir.x, 0, bulletDir.z);
        BulletDir = BulletDir.normalized;

        return BulletDir;
    }

    // Bullet의 속도를 설정하는 함수
    private float SetSpeed(float bulletSpeed)
    {
        float speed;

        // 다른 스크립트로부터 전달받은 bullet의 speed가 허용되지 않은 값인지 검사

        // 허용된 값일 경우
        if (bulletSpeed > minSpeed && bulletSpeed <= maxSpeed)
        {
            speed = bulletSpeed;

            // speed를 카메라 각도에 따라 보정
            speed = CameraEulerAngle.ReviseMagnitude(speed, BulletDir);
        }

        // 허용되지 않은 값일 경우 오류 출력
        else
        {
            Debug.LogError($"허용되지 않은 speed 값입니다: {bulletSpeed}");
            speed = minSpeed;
        }

        return speed;
    }

    private float SetRange(float bulletRange)
    {
        float range = CameraEulerAngle.ReviseMagnitude(bulletRange, BulletDir);
        return range;
    }

    // Bullet을 발사하는 함수
    public void FireBullet(Character.CharType type, Vector3 bulletDir, float bulletSpeed, float bulletDamage, float bulletRange, float bulletForce)
    {
        // Bullet을 쏜 캐릭터 설정
        gunman = type;

        // Bullet의 Damage 설정
        Damage = bulletDamage;

        // Bullet을 발사할 방향 정규화
        BulletDir = SetBulletDir(bulletDir);

        // Bullet의 속도 보정
        Speed = SetSpeed(bulletSpeed);

        // Bullet의 사정거리 보정
        Range = SetRange(bulletRange);

        // Bullet의 Force 설정
        Force = bulletForce;

        // Bullet 발사
        BulletRigid.AddForce(BulletDir * Speed, ForceMode.Impulse);
    }

    // 탄환이 사라질 때 실행될 함수
    public void DestroyBullet(bool IsHit, float time)
    {
        // 만약 적에게 적중하여 사라진다면
        if (IsHit)
        {
            Destroy(gameObject, time);
        }

        // 적에게 적중하지 않고 시작이 다 되어 사라진다면
        else
        {
            Destroy(gameObject, time);
        }
    }

    private float NormalHit()
    {
        if (gunman == Character.CharType.Normal || gunman == Character.CharType.Boss)
        {
            return 0f;
        }
        else
        {

            DestroyBullet(true, 0f);
            return Damage;

        }
    }
    private float NormalHit(GameObject hitObject)
    {
        if (gunman == Character.CharType.Normal || gunman == Character.CharType.Boss)
        {
            return 0f;
        }
        else
        {
            if (!hitObject.GetComponent<Enemy>().IsDead)
            {
                DestroyBullet(true, 0f);
                return Damage;
            }
            else
            {
                return 0f;
            }
        }
    }

    private float BossHit()
    {
        if (gunman == Character.CharType.Normal || gunman == Character.CharType.Boss)
        {
            return 0f;
        }
        else
        {
            DestroyBullet(true, 0f);
            return Damage;
        }
    }

    private void WallHit()
    {

        DestroyBullet(true, 0f);
    }

    private float PlayerHit()
    {

        // 만약 Player가 Bullet에 맞았는데 쏜 사람이 Player였다면 0 데미지를 반환하고 Bullet을 없애지 않음
        if (gunman == Character.CharType.Player)
        {
            return 0f;
        }
        else
        {
            DestroyBullet(true, 0f);
            return Damage;
        }
    }

    // Character들이 Bullet에 닿았을 때 호출될 함수
    public float BulletHit(Character.CharType type)
    {
        if (type == Character.CharType.Normal)
        {
            return NormalHit();
        }
        else if (type == Character.CharType.Boss)
        {
            return BossHit();
        }
        else if (type == Character.CharType.Wall)
        {
            WallHit();
            return Damage;
        }
        else if (type == Character.CharType.Player)
        {
            return PlayerHit();
        }
        else
        {
            return 0f;
        }
    }

    public float BulletHit(Character.CharType type, GameObject hitObject)
    {
        if (type == Character.CharType.Normal)
        {
            return NormalHit(hitObject);
        }
        else if (type == Character.CharType.Boss)
        {
            return BossHit();
        }
        else if (type == Character.CharType.Wall)
        {
            WallHit();
            return Damage;
        }
        else if (type == Character.CharType.Player)
        {
            return PlayerHit();
        }
        else
        {
            return 0f;
        }
    }

    private void Start()
    {
        BulletStartPos = transform.position;
    }

    private void FixedUpdate()
    {
        Distance = Vector3.Distance(BulletStartPos, transform.position);

        if (Distance >= Range)
        {
            DestroyBullet(false, 0f);
        }
    }
}
