using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulKinAnim : MonoBehaviour
{
    private Enemy_BulletKin bulKinScript;

    public void EnemyCorpse()
    {
        bulKinScript.EnemyCorpse();
    }

    public void EnemySpawn()
    {
        bulKinScript.EnemySpawn();
    }

    void Start()
    {
        bulKinScript = transform.GetComponentInParent<Enemy_BulletKin>();
    }
}
