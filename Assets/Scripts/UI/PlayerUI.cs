using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    private static PlayerUI _instance;
    public static PlayerUI Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindAnyObjectByType(typeof(PlayerUI)) as PlayerUI;
            }
            return _instance;
        }
    }

    public GameObject Player;

    #region Hp UI

    private float PlayerHp;
    private float PlayerMaxHp;

    public GameObject HeartPrefab;
    private List<GameObject> HeartUI = new List<GameObject>();
    public RectTransform HeartTr;
    private float HeartMargin = 80f;
    
    private void InitializeHeartList()
    {
        int Hp = (int)PlayerMaxHp;

        for (int i = 0; i < Hp / 2; i++)
        {
            GameObject Heart = Instantiate(HeartPrefab);
            Heart.transform.SetParent(HeartTr);
            Heart.transform.localPosition = new Vector3(i * HeartMargin, 0, 0);
            HeartUI.Add(Heart);
        }
    }

    // 하트가 빈 하트인지 꽉찬 하트인지를 나타낼 함수
    private void SetHeartImg()
    {
        float EmptyHp = PlayerMaxHp - PlayerHp;

        float HpCount = PlayerHp;

        for (int i = 0; i < HeartUI.Count; i++)
        {
            if (HpCount > 1)
            {
                HeartUI[i].GetComponent<PlayerHpUI>().SetHeart(2);
                HpCount -= 2;
            }
            else if (HpCount == 1)
            {
                HeartUI[i].GetComponent<PlayerHpUI>().SetHeart(1);
                HpCount -= 1;
            }
            else
            {
                HeartUI[i].GetComponent<PlayerHpUI>().SetHeart(0);
            }
        }
    }

    // 플레이어의 최대 최력이 변동되었을 경우
    private void OnPlayerMaxHpChage(bool Plus, float amount)
    {
        // 최대 체력이 증가하였다면
        if (Plus)
        {
            for (int i = 0; i < amount / 2; i++)
            {
                GameObject Heart = Instantiate(HeartPrefab);
                Heart.transform.SetParent(HeartTr);
                Heart.transform.localPosition = new Vector3(HeartUI.Count * HeartMargin, 0, 0);
                HeartUI.Add(Heart);
            }
        }
        // 최대 체력이 감소하였다면
        else
        {
            for (int i = 0; i < amount / 2; i++)
            {
                int DestroyHeartIndex = HeartUI.Count - 1;
                GameObject DestroyHeart = HeartUI[DestroyHeartIndex];
                HeartUI.RemoveAt(DestroyHeartIndex);
                Destroy(DestroyHeart);
            }
        }
    }

    // 플레이어의 체력이 변동되었을 때 사용할 함수
    public void OnPlayerHpChange(float HP, float MaxHP)
    {
        PlayerHp = HP;

        // 플레이어의 최대 체력이 변동되었다면
        if (PlayerMaxHp != MaxHP)
        {
            bool Plus;
            float amount;

            if(PlayerMaxHp<MaxHP)
            {
                Plus = true;
                amount = MaxHP - PlayerMaxHp;
            }
            else
            {
                Plus = false;
                amount = PlayerMaxHp - MaxHP;
            }

            OnPlayerMaxHpChage(Plus, amount);
            PlayerMaxHp = MaxHP;
        }

        SetHeartImg();
    }

    #endregion

    #region Gun Inventory UI

    public GameObject GunInven;

    public Image Full_Frame;
    public Image Additional_Frame;
    public Image Current_Gun;

    public TextMeshProUGUI Remain_Ammo;
    public TextMeshProUGUI Current_Gun_Name;

    public GameObject MagazineUI;

    public List<GameObject> GunInventory = new List<GameObject>();
    GameObject currentGun;
    public int currentGunNum;

    // Gun Inventory 동기화 함수
    public void SetGunInven(List<GameObject> gunInven)
    {
        GunInventory = gunInven;
        Full_Frame.GetComponent<FullFrameUI>().GunInventory = gunInven;
        Additional_Frame.GetComponent<AddiFrameUI>().GunInventory = gunInven;
    }

    public void SetGunUI(int gunNum)
    {
        currentGunNum = gunNum;
        currentGun = GunInventory[currentGunNum];

        // Frame UI 설정
        Full_Frame.GetComponent<FullFrameUI>().SetCurrentGun(currentGunNum);
        Additional_Frame.GetComponent<AddiFrameUI>().SetCurrentGun(currentGunNum);

        // 탄창 UI 설정
        MagazineUI.GetComponent<MagazineUI>().SetMagazineUI(currentGun);

        // 남은 총알 수 text 설정
        SetRemainAmmoText();
        // 현재 총 이름 설정
        SetGunNameText();
    }
    
    // Ammo와 관련된 UI 업데이트
    public void UpdateAmmoUI()
    {
        MagazineUI.GetComponent<MagazineUI>().SetMagazineAmmoUI(currentGun);
        SetRemainAmmoText();
    }

    // Remain Ammo Text 업데이트
    private void SetRemainAmmoText()
    {
        float remainAmmo = currentGun.GetComponent<Gun>().remainAmmo;
        float AmmoCapacity = currentGun.GetComponent<Gun>().AmmoCapacity;

        string remainAmmoText = remainAmmo.ToString() + " / " + AmmoCapacity.ToString();

        Remain_Ammo.text = remainAmmoText;
    }

    // Current Gun Name Text 업데이트
    public void SetGunNameText()
    {
        Current_Gun_Name.text = currentGun.GetComponent<Gun>().itemName;
        Current_Gun_Name.GetComponent<Animator>().SetTrigger("Disappear");
    }

    #endregion

    public Animator HitEffectAnim;

    public void OnHitUI()
    {
        if (HitEffectAnim != null)
        {
            HitEffectAnim.SetTrigger("Blink");
        }
    }

    private void ActiveSprite()
    {
        Full_Frame.GetComponent<FullFrameUI>().ActiveFullFrameUI();
        Additional_Frame.GetComponent<Image>().color = new Vector4(1, 1, 1, 1);
        MagazineUI.GetComponent<MagazineUI>().ActiveMagazineUI();
        Remain_Ammo.GetComponent<TextMeshProUGUI>().color= new Vector4(1, 1, 1, 1);
    }

    // 플레이어가 생성되었을 때 플레이어와 연결하는 함수
    public void ConnectPlayer(GameObject player)
    {
        Player = player;

        PlayerHp = Player.GetComponent<Player>().Hp;
        PlayerMaxHp = Player.GetComponent<Player>().MaxHp;
        InitializeHeartList();
        SetHeartImg();

        ActiveSprite();
        SetGunInven(player.GetComponent<Player>().GunInventory);
        SetGunUI(player.GetComponent<Player>().currentGunNum);
    }

    public void GameFinish()
    {
        HeartTr.gameObject.SetActive(false);
        GunInven.SetActive(false);
    }
}
