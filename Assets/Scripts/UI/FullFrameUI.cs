using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FullFrameUI : MonoBehaviour
{
    public List<GameObject> GunInventory = new List<GameObject>();
    public GameObject currentGun;
    public int currentGunNum;

    public Image currentGunImg;

    public Animator FullFrameAnim;
    public Animator AddiFrameAnim;

    private bool IsAnim = false;

    public void ChangeUpAnim()
    {
        if (!IsAnim)
        {
            FullFrameAnim.SetTrigger("Change_Up");
            AddiFrameAnim.SetTrigger("Change_Up_0to9");
            IsAnim = true;
        }
    }

    public void ChangeDownAnim()
    {
        if (!IsAnim)
        {
            FullFrameAnim.SetTrigger("Change_Down");
            AddiFrameAnim.SetTrigger("Change_Down_9to0");
            IsAnim = true;

            SetCurrentGunImage();
        }
    }

    public void ChangeAnimFin()
    {
        IsAnim = false;

        SetCurrentGunImage();
    }

    public void SetCurrentGun(int gunNum)
    {
        currentGunNum = gunNum;
        currentGun = GunInventory[currentGunNum];

        SetCurrentGunImage();
    }

    private void SetCurrentGunImage()
    {
        currentGunImg.sprite = currentGun.GetComponent<Gun>().ItemSprite;
        currentGunImg.SetNativeSize();
    }

    public void ActiveFullFrameUI()
    {
        gameObject.GetComponent<Image>().color = new Vector4(1, 1, 1, 1);
        currentGunImg.GetComponent<Image>().color = new Vector4(1, 1, 1, 1);
    }
}
