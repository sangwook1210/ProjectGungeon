using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddiFrameUI : MonoBehaviour
{
    public List<GameObject> GunInventory = new List<GameObject>();
    public GameObject currentGun;
    public int currentGunNum;

    public Image AddiFrameImg;
    public Image NextGunImg;

    public Sprite FullFrameSprite;
    public Sprite AddiFrameSprite;

    public Animator FullFrameAnim;
    public Animator AddiFrameAnim;

    public void SetCurrentGun(int gunNum)
    {
        currentGunNum = gunNum;
        currentGun = GunInventory[currentGunNum];
    }

    public void ChangeUpStart()
    {
        AddiFrameImg.sprite = FullFrameSprite;
        AddiFrameImg.SetNativeSize();

        NextGunImg.sprite = currentGun.GetComponent<Gun>().ItemSprite;
        NextGunImg.SetNativeSize();
        NextGunImg.color=new Vector4(1,1,1,1);
    }

    public void ChangeDownStart()
    {
        AddiFrameImg.sprite = FullFrameSprite;
        AddiFrameImg.SetNativeSize();

        if (currentGunNum + 1 < GunInventory.Count)
        {
            currentGunNum++;
        }
        else
        {
            currentGunNum = 0;
        }
        currentGun = GunInventory[currentGunNum];
        NextGunImg.sprite = currentGun.GetComponent<Gun>().ItemSprite;
        NextGunImg.SetNativeSize();
        NextGunImg.color = new Vector4(1, 1, 1, 1);
    }

    public void ChangeFinish()
    {
        AddiFrameImg.sprite = AddiFrameSprite;
        AddiFrameImg.SetNativeSize();

        NextGunImg.color = new Vector4(1, 1, 1, 0);

        //AddiFrameAnim.SetBool("ChangeUp_0to9", false);
    }
}
