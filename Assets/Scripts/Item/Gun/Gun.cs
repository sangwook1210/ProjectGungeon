using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : Item
{
    protected GunItemData gunItemData;

    protected GameObject gunman;
    protected Character.CharType gunmanType;      // 총의 주인
    protected Transform gunmanTr;

    #region Gun_Stats

    GunType gunType;                   // 사격 형태                       
    public int MagazineSize;     // 탄창 크기 (한 탄창에 들어 있는 탄의 개수)
    public int AmmoCapacity;     // 탄약량 (해당 화기의 최대 보유 탄수)
    private float playerDamage;           // 플레이어가 총을 사용하였을 때의 데미지
    private float enemyDamage;  // enemy가 총을 사용하였을 때의 데미지
    protected float Damage
    {
        get
        {
            if (gunmanType == Character.CharType.Player)
                return playerDamage;
            else
                return enemyDamage;
        }

        set
        {
            playerDamage = value;
        }
    }         // 화력 (탄 하나당 대미지)
    protected float FireRate;       // 딜레이 (연사 시 탄환 간의 시차)
    protected float ReloadTime;     // 재장전 시간
    protected float ShotSpeed;      // 탄속
    protected float Range;          // 사정거리 (1000일 시 무한)
    protected float Force;          // 넉백 (적을 넉백할 수 있는 수치)
    protected float Spread;         // 산탄도 (커서를 기준으로 탄이 퍼지는 정도)

    protected int _remainAmmoInMagazine;  // 현재 탄창에 남아있는 총알의 개수
    public int remainAmmoInMagazine
    {
        get
        {
            return _remainAmmoInMagazine;
        }
        set
        {
            _remainAmmoInMagazine = value;
        }
    }
    public int remainAmmo;           // 총 남아있는 총알의 개수

    #endregion

    protected GameObject Bullet;    // 총알
    protected Vector3 bulletDir;    // 총알이 나가는 방향
    protected Transform muzzlePos;  // 그래픽 상의 총구의 위치
    protected Vector3 realMuzzlePos;  // 실제 총알이 나가는 총구의 위치

    protected float muzzlePosDistance;  // 총의 중심(pivot)으로부터 총구까지의 거리
    protected Vector3 gunLocalPos;  // 총의 local 위치
    protected float gunDepthFrontZ = -2.0f;   // 총이 플레이어 앞에 있을 경우의 Z축 값
    protected float gunDepthBackZ = 0.1f;    // 총이 플레이어 뒤에 있을 경우의 Z축 값
    protected Vector3 gunLocalPosFlipX {
        get
        {
            return new Vector3(-gunLocalPos.x, gunLocalPos.y, gunLocalPos.z);
        }
    }  // FlipX된 총의 local 위치
    protected Vector3 muzzleLocalPos;   // 총구의 local 위치
    protected Vector3 muzzleLocalPosFlipX {
        get
        {
            return new Vector3(-muzzleLocalPos.x, muzzleLocalPos.y, muzzleLocalPos.z);
        }
    }   // FlipX된 총구의 local 위치

    public bool isReloading = false;
    protected bool canFire = true;
    private bool isEquipped = false;

    public Sprite AmmoUI;
    public Sprite EmptyAmmoUI;

    //public GameObject PlayerUI;

    public List<AudioClip> ShotClip;
    public AudioClip ReloadClip;
    public AudioClip EmptyClip;

    public GameObject realMuzzle;

    
    public Vector3 BulletDir
    {
        get
        {
            return bulletDir;
        }

        set
        {
            bulletDir = value;
        }
    }

    /// <summary>
    /// 총의 스탯을 초기화하는 함수
    /// </summary>
    /// <param name="type"> 총의 타입 </param>
    /// <param name="magazineSize"> 탄창 크기 </param>
    /// <param name="ammoCapacity"> 탄약량 </param>
    /// <param name="damage"> 화력 </param>
    /// <param name="fireRate"> 딜레이 </param>
    /// <param name="reloadTime"> 재장전 시간 </param>
    /// <param name="shotSpeed"> 탄속 </param>
    /// <param name="range"> 사정거리 </param>
    /// <param name="force"> 넉백 </param>
    /// <param name="spread"> 산탄도 </param>
    protected void GunInit(string name, int id, ItemType itemtype,Sprite itemSprite,GunType guntype, int magazineSize, int ammoCapacity, float damage, float fireRate, float reloadTime, float shotSpeed, float range, float force, float spread)
    {
        ItemInit(name, id, itemtype, itemSprite);

        gunType = guntype;
        MagazineSize = magazineSize;
        AmmoCapacity = ammoCapacity;
        Damage = damage;
        FireRate = fireRate;
        ReloadTime = reloadTime;
        ShotSpeed = shotSpeed;
        Range = range;
        Force = force;
        Spread = spread;

        _remainAmmoInMagazine = MagazineSize;
        remainAmmo = AmmoCapacity;

        //PlayerUI = GameObject.FindGameObjectWithTag("UIManager");
    }

    protected override void SetItemData(int id)
    {
        SetItemDatabase();

        if (_itemDB != null)
        {
            itemData = _itemDB.FindItemDataWithId(id);
            gunItemData = itemData as GunItemData;

            GunInit(gunItemData.name, gunItemData.id, gunItemData.itemtype, gunItemData.itemSprite,gunItemData.gunType, gunItemData.magazineSize, gunItemData.ammoCapacity, gunItemData.playerDamage,
                gunItemData.fireRate, gunItemData.reloadTime, gunItemData.shotSpeed, gunItemData.range, gunItemData.force, gunItemData.spread);
        }
    }

    // 총의 주인을 설정하는 함수
    public void SetGunman(Character.CharType type)
    {
        gunmanType = type;
    }

    // 총의 주인의 Transform을 받아오는 함수
    public void SetGunmanTransform(GameObject Gunman)
    {
        gunman = Gunman;
        gunmanTr = gunman.transform;
    }

    public void SetEnemyDamage(float _enemyDamage)
    {
        enemyDamage = _enemyDamage;
    }

    // 총알을 발사하는 함수
    public virtual void FireBullet(Vector3 mousePosition) { }

    // 총이 인벤토리에서 선택되었을 때 실행될 함수
    public void EquipGun()
    {
        isEquipped = true;

        gameObject.GetComponent<SpriteRenderer>().sprite = ItemSprite;
    }

    // 총이 인벤토리에서 해제되었을 때 실행될 함수
    public void DestructGun()
    {
        isEquipped = false;
        gameObject.GetComponent<SpriteRenderer>().sprite = null;
    }

    // 총알을 보급하는 함수
    public void SupplyAmmo(float ratio)
    {
        _remainAmmoInMagazine = MagazineSize;
        remainAmmo = (int)Mathf.Round((float)AmmoCapacity * ratio);

        if (gunmanType == Character.CharType.Player && isEquipped)
        {
            PlayerUI.Instance.GetComponent<PlayerUI>().UpdateAmmoUI();
        }
    }

    // 총을 재장전 하는 함수
    public virtual void ReloadMagazine()
    {
        if (gunmanType == Character.CharType.Player && _remainAmmoInMagazine != MagazineSize&&!isReloading&& remainAmmo>0)
        {
            if (!isReloading)
            {
                PlayAudioClip(ReloadClip);
            }

            isReloading = true;
            if (isEquipped)
            {
                gunman.GetComponent<Player>().ReloadUI.GetComponent<ReloadUI>().Reload(ReloadTime);
            }

            StartCoroutine("ReloadCoolTime");
        }
        else if(_remainAmmoInMagazine != MagazineSize&&!isReloading)
        {
            if (!isReloading)
            {
                PlayAudioClip(ReloadClip);
            }
            isReloading = true;
            StartCoroutine("ReloadCoolTime");
        }
    }

    public void StopReload()
    {
        if (isReloading)
        {
            StopCoroutine("ReloadCoolTime");
            isReloading = false;

            if (isEquipped)
            {
                gunman.GetComponent<Player>().ReloadUI.GetComponent<ReloadUI>().DisableReloadUI();
            }
        }
    }

    IEnumerator ReloadCoolTime()
    {
        yield return new WaitForSeconds(ReloadTime);

        if (remainAmmo < MagazineSize)
        {
            _remainAmmoInMagazine = remainAmmo;
        }
        else
        {
            _remainAmmoInMagazine = MagazineSize;
        }
        isReloading = false;

        if (gunmanType == Character.CharType.Player&&isEquipped)
        {
            PlayerUI.Instance.GetComponent<PlayerUI>().UpdateAmmoUI();
        }
        
    }

    // 총과 총구의 로컬 위치를 설정하는 함수
    protected void SetGunLocalPos()
    {
        gunLocalPos = transform.localPosition;

        muzzleLocalPos = muzzlePos.localPosition;
    }

    // 마우스 위치에 따라 총의 Order in Layer를 설정하는 함수
    protected void SetGunOrderInLayer(float mouseDegree)
    {
        // 마우스의 각도가 0보다 작다면 플레이어보다 앞으로 설정
        if (mouseDegree < 0)
        {
            gunLocalPos = new Vector3(gunLocalPos.x, gunLocalPos.y, gunDepthFrontZ);
            gameObject.GetComponent<SpriteRenderer>().sortingOrder = -2;
        }
        // 마우스의 각도가 0보다 같거나 크다면 플레이어보다 뒤로 설정 (기본 위치가 뒤)
        else
        {
            gunLocalPos = new Vector3(gunLocalPos.x, gunLocalPos.y, gunDepthBackZ);
            gameObject.GetComponent<SpriteRenderer>().sortingOrder = 0;
        }
    }

    // 마우스의 위치에 따라 총의 Sprite를 회전시키는 함수
    public void RotateGunSprite(float mouseDegree)
    {
        // 총의 Order in Layer를 설정
        SetGunOrderInLayer(mouseDegree);

        // 마우스의 좌우를 기준으로 총의 위치를 설정

        // 만약 마우스가 플레이어 기준 오른쪽에 있다면
        if (mouseDegree >= -90 && mouseDegree <= 90)
        {
            gameObject.GetComponent<SpriteRenderer>().flipX = false;
            transform.localPosition = gunLocalPos;
            muzzlePos.localPosition = muzzleLocalPos;
        }
        // 만약 마우스가 플레이어 기준 왼쪽에 있다면
        else
        {
            gameObject.GetComponent<SpriteRenderer>().flipX = true;
            transform.localPosition = gunLocalPosFlipX;
            muzzlePos.localPosition = muzzleLocalPosFlipX;

            if (mouseDegree < 0)
            {
                mouseDegree = 180 - Mathf.Abs(mouseDegree);
            }
            else
            {
                mouseDegree = Mathf.Abs(mouseDegree) - 180;
            }
        }

        float reviseDegree = CameraEulerAngle.cameraEulerAngle.x;

        // 마우스의 각도를 기준으로 총의 각도를 설정

        if (mouseDegree >= -5 && mouseDegree < 5)
        {
            transform.eulerAngles = new Vector3(reviseDegree, 0, 0);
        }
        else if (mouseDegree >= 5 && mouseDegree < 15)
        {
            transform.eulerAngles = new Vector3(reviseDegree, 0, 10);
        }
        else if (mouseDegree >= 15 && mouseDegree < 25)
        {
            transform.eulerAngles = new Vector3(reviseDegree, 0, 20);
        }
        else if (mouseDegree >= 25 && mouseDegree < 35)
        {
            transform.eulerAngles = new Vector3(reviseDegree, 0, 30);
        }
        else if (mouseDegree >= 35 && mouseDegree < 45)
        {
            transform.eulerAngles = new Vector3(reviseDegree, 0, 40);
        }
        else if (mouseDegree >= 45 && mouseDegree < 55)
        {
            transform.eulerAngles = new Vector3(reviseDegree, 0, 50);
        }
        else if (mouseDegree >= 55 && mouseDegree < 65)
        {
            transform.eulerAngles = new Vector3(reviseDegree, 0, 60);
        }
        else if (mouseDegree >= 65 && mouseDegree < 75)
        {
            transform.eulerAngles = new Vector3(reviseDegree, 0, 70);
        }
        else if (mouseDegree >= 75 && mouseDegree < 85)
        {
            transform.eulerAngles = new Vector3(reviseDegree, 0, 80);
        }
        else if (mouseDegree >= 85 && mouseDegree <= 90)
        {
            transform.eulerAngles = new Vector3(reviseDegree, 0, 90);
        }
        else if (mouseDegree >= -15 && mouseDegree < -5)
        {
            transform.eulerAngles = new Vector3(reviseDegree, 0, -10);
        }
        else if (mouseDegree >= -25 && mouseDegree < -15)
        {
            transform.eulerAngles = new Vector3(reviseDegree, 0, -20);
        }
        else if (mouseDegree >= -35 && mouseDegree < -25)
        {
            transform.eulerAngles = new Vector3(reviseDegree, 0, -30);
        }
        else if (mouseDegree >= -45 && mouseDegree < -35)
        {
            transform.eulerAngles = new Vector3(reviseDegree, 0, -40);
        }
        else if (mouseDegree >= -55 && mouseDegree < -45)
        {
            transform.eulerAngles = new Vector3(reviseDegree, 0, -50);
        }
        else if (mouseDegree >= -65 && mouseDegree < -55)
        {
            transform.eulerAngles = new Vector3(reviseDegree, 0, -60);
        }
        else if (mouseDegree >= -75 && mouseDegree < -65)
        {
            transform.eulerAngles = new Vector3(reviseDegree, 0, -70);
        }
        else if (mouseDegree >= -85 && mouseDegree < -75)
        {
            transform.eulerAngles = new Vector3(reviseDegree, 0, -80);
        }
        else if (mouseDegree >= -90 && mouseDegree < -85)
        {
            transform.eulerAngles = new Vector3(reviseDegree, 0, -90);
        }
    }

    // 실제 총구의 위치를 계산하는 함수
    protected void SetRealMuzzlePos()
    {
        // muzzlePos 위치로 광선 발사
        Vector3 screenMuzzlePos = CameraEulerAngle.playerCamera.WorldToScreenPoint(muzzlePos.position);      
        Ray RayCamera = CameraEulerAngle.playerCamera.ScreenPointToRay(screenMuzzlePos);

        // 광선에 닿은 오브젝트(hitData) 반환
        if (Physics.Raycast(RayCamera.origin, RayCamera.direction, out RaycastHit hitData, Mathf.Infinity))
        {
            Vector3 RayStartPos = CameraEulerAngle.playerCamera.ScreenToWorldPoint(screenMuzzlePos);
            Vector3 RayHitPos = hitData.point;  // 광선이 오브젝트에 닿은 점의 좌표

            // ray 벡터 위의 플레이어와 높이가 같은 점의 좌표를 realMuzzlePos에 저장
            realMuzzlePos = CameraEulerAngle.FindPointOnVector(gunmanTr.position, RayStartPos, RayHitPos);


            if (gunmanType == Character.CharType.Player)
            {
                Debug.Log("플레이어가 가진 총");
                if (realMuzzle == null)
                {
                    realMuzzle = GameObject.FindGameObjectWithTag("realMuzzle");
                    Debug.Log("리얼머즐 이름: " + realMuzzle.name);
                }
                else
                {
                    Debug.Log("리얼머즐 이름: " + realMuzzle.name);
                    Debug.Log("리얼머즐 바뀜");
                    realMuzzle.transform.position = realMuzzlePos;
                }
            }
            /*
            Debug.Log("리얼머즐 이름: " + realMuzzle.name);
            if (realMuzzle != null)
            {
                Debug.Log("리얼머즐 바뀜");
                realMuzzle.transform.position = realMuzzlePos;
            }*/
        }
    }

    // 산탄시키는 함수
    protected void RecoilBulletDir(Vector3 targetPos)
    {
        // 보정될 각도를 랜덤하게 선택
        float recoilDegree = Random.Range(-Spread / 2, Spread / 2);

        // 플레이어의 스크린 상의 좌표 저장
        Vector3 playerScreenPos = CameraEulerAngle.playerCamera.WorldToScreenPoint(gunmanTr.position);
        // 발사할 목표의 스크린 상의 좌표
        Vector3 targetScreenPos;
        // 플레이어라면 targetPos가 스크린 상의 클릭한 좌표일 것
        if (gunmanType == Character.CharType.Player)
        {
            // 플레이어가 클릭한 스크린 상의 좌표를 저장
            targetScreenPos = targetPos;
        }
        // 플레이어가 아니라면 targetPos가 월드 좌표 상의 목표일 것
        else
        {
            // 목표의 스크린 상의 위치를 저장
            targetScreenPos = CameraEulerAngle.playerCamera.WorldToScreenPoint(targetPos);
        }

        // 플레이어의 스크린 상의 좌표의 z 좌표 초기화
        playerScreenPos = new Vector3(playerScreenPos.x, playerScreenPos.y, 0);

        // 플레이어가 클릭한 좌표의 플레이어의 위치를 원점으로 한 벡터 저장
        Vector3 screenVector = targetScreenPos - playerScreenPos;
        // 위 벡터의 크기 저장
        float screenVectorMagnitude = screenVector.magnitude;

        // 벡터의 화면 상의 각도 저장
        float screenDegree = CameraEulerAngle.CalculateDegreeOnScreen(playerScreenPos, targetScreenPos);

        // 보정된 각도 갱신
        recoilDegree = screenDegree + recoilDegree;

        if (recoilDegree > 180)
        {
            recoilDegree = recoilDegree - 360;
        }
        else if (recoilDegree < -180)
        {
            recoilDegree = 360 - recoilDegree;
        }

        // 보정된 각도를 가지고 플레이어의 스크린 상의 좌표를 원점으로 하는 벡터 생성
        if (recoilDegree >= 0 && recoilDegree < 45)
        {
            screenVector = new Vector3(1, Mathf.Tan(recoilDegree * Mathf.Deg2Rad), 0);
        }
        else if (recoilDegree >= 45 && recoilDegree < 90)
        {
            recoilDegree = 90 - recoilDegree;
            screenVector = new Vector3(Mathf.Tan(recoilDegree * Mathf.Deg2Rad), 1, 0);
        }
        else if (recoilDegree >= 90 && recoilDegree < 135)
        {
            recoilDegree = recoilDegree - 90;
            screenVector = new Vector3(-Mathf.Tan(recoilDegree * Mathf.Deg2Rad), 1, 0);
        }
        else if (recoilDegree >= 135 && recoilDegree <= 180)
        {
            recoilDegree = 180 - recoilDegree;
            screenVector = new Vector3(-1, Mathf.Tan(recoilDegree * Mathf.Deg2Rad), 0);
        }
        else if (recoilDegree < 0 && recoilDegree >= -45)
        {
            screenVector = new Vector3(1, Mathf.Tan(recoilDegree * Mathf.Deg2Rad), 0);
        }
        else if (recoilDegree < -45 && recoilDegree >= -90)
        {
            recoilDegree = 90 + recoilDegree;
            screenVector = new Vector3(Mathf.Tan(recoilDegree * Mathf.Deg2Rad), -1, 0);
        }
        else if (recoilDegree < -90 && recoilDegree >= -135)
        {
            recoilDegree = recoilDegree + 90;
            screenVector = new Vector3(Mathf.Tan(recoilDegree * Mathf.Deg2Rad), -1, 0);
        }
        else if (recoilDegree < -135 && recoilDegree >= -180)
        {
            recoilDegree = recoilDegree + 180;
            screenVector = new Vector3(-1, -Mathf.Tan(recoilDegree * Mathf.Deg2Rad), 0);
        }

        // 보정된 각도의 벡터를 기존의 클릭된 좌표의 벡터 크기에 맞게 보정
        screenVector = screenVector.normalized * screenVectorMagnitude;
        screenVector = screenVector + playerScreenPos;

        SetBulletDir(screenVector);
    }

    private RaycastHit hitData; // 카메라에서 발사한 광선이 닿은 물체의 Data

    // 스크린 상의 발사 위치를 월드 상의 위치로 바꾸는 함수
    protected void SetBulletDir(Vector3 screenVector)
    {
        // 스크린 상의 위치로 광선 발사
        Ray RayCamera = CameraEulerAngle.playerCamera.ScreenPointToRay(screenVector);

        // 광선에 닿은 오브젝트(hitData) 반환
        if (Physics.Raycast(RayCamera.origin, RayCamera.direction, out hitData, Mathf.Infinity))
        {
            Vector3 RayStartPos = CameraEulerAngle.playerCamera.ScreenToWorldPoint(screenVector); // 광선이 시작된 점의 좌표
            Vector3 RayHitPos = hitData.point;  // 광선이 오브젝트에 닿은 점의 좌표

            // ray 벡터 위의 플레이어와 높이가 같은 점의 좌표를 mousePos에 저장
            Vector3 mousePos = CameraEulerAngle.FindPointOnVector(gunmanTr.position, RayStartPos, RayHitPos);

            // 플레이어의 위치를 기준으로 bullet이 발사될 방향을 bulletDir에 저장
            bulletDir = mousePos - gunmanTr.position;
            Vector3 normalizedBulletDir = bulletDir.normalized;
        }
    }

    public virtual void SetDegree() { }
}
