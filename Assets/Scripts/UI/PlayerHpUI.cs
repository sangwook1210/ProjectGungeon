using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHpUI : MonoBehaviour
{
    public GameObject Heart;
    public GameObject Heart_Full;
    public GameObject Heart_Half;
    public GameObject Heart_Empty;

    public void SetHeart(int Hp)
    {
        if (Hp == 2)
        {
            Heart_Full.SetActive(true);
            Heart_Half.SetActive(false);
            Heart_Empty.SetActive(false);
        }
        else if (Hp == 1)
        {
            Heart_Full.SetActive(false);
            Heart_Half.SetActive(true);
            Heart_Empty.SetActive(false);
        }
        else
        {
            Heart_Full.SetActive(false);
            Heart_Half.SetActive(false);
            Heart_Empty.SetActive(true);
        }

        Heart_Full.GetComponent<Image>().raycastTarget = false;
        Heart_Half.GetComponent<Image>().raycastTarget = false;
        Heart_Empty.GetComponent<Image>().raycastTarget = false;
    }
}
