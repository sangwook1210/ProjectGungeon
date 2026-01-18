using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SemiautomaticGun : Gun
{
    protected int bulletCount;  // 한 번의 발사에 발사되는 bullet의 수

    public override void FireBullet(Vector3 targetPos)
    {
        // 탄창 안의 총알이 남아있고, 발사 쿨타임이 지났고, 재장전 중이 아니라면
        if (remainAmmoInMagazine > 0 && remainAmmo > 0 && canFire && !isReloading)
        {
            SetRealMuzzlePos(); // 총구의 월드 좌표 상의 위치 설정

            // 단발형 총이라면
            if (bulletCount == 1)
            {
                RecoilBulletDir(targetPos);  // 발사할 각도 보정
                GameObject bullet = Instantiate(Bullet, realMuzzlePos, Quaternion.identity);
                bullet.GetComponent<Bullet>().FireBullet(base.gunmanType, base.bulletDir, base.ShotSpeed, base.Damage, base.Range, base.Force);
            }

            // 샷건형 총이라면
            else if (bulletCount > 1)
            {
                for (int i = 0; i < bulletCount; i++)
                {
                    float reviseShotSpeed = Random.Range(-5f, 0f);
                    RecoilBulletDir(targetPos);  // 발사할 각도 보정
                    GameObject bullet = Instantiate(Bullet, realMuzzlePos, Quaternion.identity);
                    bullet.GetComponent<Bullet>().FireBullet(base.gunmanType, base.bulletDir, base.ShotSpeed + reviseShotSpeed, base.Damage, base.Range, base.Force);

                }
            }

            remainAmmo--;   // 전체 총알의 개수 감수

            _remainAmmoInMagazine--; // 탄창 안의 총알 감소

            canFire = false;

            StartCoroutine(FireCoolTime());
            PlayAudioClip(ShotClip);
        }

        // 탄창에 있는 총알을 다 썼을 때 발사 입력이 들어오면 재장전
        else if (_remainAmmoInMagazine < 1 && remainAmmo > 0 && !isReloading)
        {
            if (gunmanType == Character.CharType.Player)
            {
                gunman.GetComponent<Player>().ReloadGunInven();
            }
        }

        // 남은 총알이 없을 경우
        else if (remainAmmo < 1)
        {
            PlayAudioClip(EmptyClip);
        }
    }

    // 발사 시 총알 간의 딜레이를 기다리는 함수
    IEnumerator FireCoolTime()
    {      
        yield return new WaitForSeconds(FireRate);

        canFire = true;
    }
}
