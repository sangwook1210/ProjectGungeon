using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public enum GunType // 사격 형태
{
    Semiautomatic,  // 반자동
    Automatic,  // 자동
    Burst,  // 점사형 (파열형)
    Charged,    // 충전형
    Beam    // 빔형
}

public class GunItemData : ItemData
{
    public int magazineSize;     // 탄창 크기 (한 탄창에 들어 있는 탄의 개수)
    public int ammoCapacity;     // 탄약량 (해당 화기의 최대 보유 탄수)
    public float playerDamage;           // 플레이어가 총을 사용하였을 때의 데미지
    public float fireRate;       // 딜레이 (연사 시 탄환 간의 시차)
    public float reloadTime;     // 재장전 시간
    public float shotSpeed;      // 탄속
    public float range;          // 사정거리 (1000일 시 무한)
    public float force;          // 넉백 (적을 넉백할 수 있는 수치)
    public float spread;         // 산탄도 (커서를 기준으로 탄이 퍼지는 정도)

    public GunType gunType;        // 사격 형태

    public GunItemData(string name, int id, ItemType itemtype,GunType guntype, int magazineSize, int ammoCapacity, float damage,
        float fireRate, float reloadTime, float shotSpeed, float range, float force, float spread, string prefabPath, Sprite sprite) : base(name, id,  itemtype, prefabPath,sprite)
    {
        this.gunType = guntype;
        this.magazineSize = magazineSize;
        this.ammoCapacity = ammoCapacity;
        this.playerDamage = damage;
        this.fireRate = fireRate;
        this.reloadTime = reloadTime;
        this.shotSpeed = shotSpeed;
        this.range = range;
        this.force = force;
        this.spread = spread;
    }
}
