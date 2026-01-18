using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Altar : MonoBehaviour
{
    public void ActiveAltar()
    {
        gameObject.GetComponent<Animator>().SetTrigger("Active");
    }

    public void ActiveAnimFinish()
    {
        GameManager.Instance.GameStart = true;
    }
}
