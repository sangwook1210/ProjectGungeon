using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MagazineUI : MonoBehaviour
{
    public Image BottomImage;
    public Image TopImage;
    public Image Ammo;  // 총알 이미지
    private List<Image> AmmoImages = new List<Image>();
    bool newAmmoImage = false;  // 총이 바뀌어서 새로운 총알 이미지를 준비해야 함을 저장하는 변수

    private Sprite AmmoSprite;  // 해당 총의 총알 스프라이트
    private Sprite EmptyAmmoSprite;

    private int remainAmmo; // 해당 총의 현재 탄창에 남아있는 총알의 수
    private int magazineSize;   // 해당 총의 탄창 사이즈

    private float AmmoSpriteHeight;
    private float AmmoMargin = 10;  // Ammo 간의 간격

    // Magazine UI를 세팅하는 함수
    public void SetMagazineUI(GameObject gun)
    {
        GameObject Gun = gun;

        AmmoSprite = Gun.GetComponent<Gun>().AmmoUI;
        EmptyAmmoSprite = Gun.GetComponent<Gun>().EmptyAmmoUI;

        Ammo.sprite = AmmoSprite;
        Ammo.SetNativeSize();
        AmmoSpriteHeight = Ammo.rectTransform.sizeDelta.y;

        // 기존 AmmoImages List 초기화
        foreach (Image image in AmmoImages)
        {
            Destroy(image.gameObject);
        }
        AmmoImages.Clear();

        newAmmoImage = true;
        SetMagazineAmmoUI(Gun);
    }

    public void SetMagazineAmmoUI(GameObject gun)
    {
        remainAmmo = gun.GetComponent<Gun>().remainAmmoInMagazine;
        magazineSize = gun.GetComponent<Gun>().MagazineSize;

        float current_Height = BottomImage.rectTransform.sizeDelta.y;

        for (int i = 0; i < magazineSize; i++)
        {
            current_Height += AmmoMargin;

            Sprite currentSprite;

            // 총알의 유무에 따라 스프라이트 설정
            if (i < remainAmmo)
                currentSprite = AmmoSprite;
            else
                currentSprite = EmptyAmmoSprite;

            if (i == 0)
            {
                Ammo.rectTransform.anchoredPosition = new Vector3(0, current_Height, 0);
                Ammo.sprite = currentSprite;
            }
            else
            {
                if (newAmmoImage)
                {
                    current_Height += AmmoSpriteHeight;
                    Image AmmoPrefab = Instantiate(Ammo);
                    AmmoPrefab.transform.SetParent(transform);
                    AmmoPrefab.rectTransform.anchoredPosition = new Vector3(0, current_Height, 0);
                    AmmoPrefab.sprite = currentSprite;

                    AmmoImages.Add(AmmoPrefab);
                }
                else
                {
                    current_Height += AmmoSpriteHeight;
                    AmmoImages[i - 1].sprite = currentSprite;
                }
            }
        }

        current_Height += AmmoMargin;
        current_Height += AmmoSpriteHeight;

        TopImage.rectTransform.anchoredPosition = new Vector3(0, current_Height, 0);

        newAmmoImage = false;
    }

    public void ActiveMagazineUI()
    {
        BottomImage.color = new Vector4(1, 1, 1, 1);
        TopImage.color = new Vector4(1, 1, 1, 1);
        Ammo.color = new Vector4(1, 1, 1, 1);
    }
}
