using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnim : MonoBehaviour
{
    private Player playerScript;

    public void DodgeInvinsibleFinish()
    {
        playerScript.DodgeInvinsibleFinish();
    }

    public void DodgeFinish()
    {
        playerScript.DodgeFinish();
    }

    public void ShowResultBook()
    {
        GameManager.Instance.ShowResultBook();
    }

    void Start()
    {
        playerScript = transform.GetComponentInParent<Player>();
    }
}
