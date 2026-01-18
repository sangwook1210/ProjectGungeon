using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    // 플레이어 GameObject
    protected GameObject Player;
    // Enemy와 플레이어 사이의 각도
    protected float PlayerDegree;
    // Enemy와 플레이어 사이의 거리
    protected float PlayerDis;
    // Enemy로부터 플레이어의 방향
    protected Vector3 PlayerDir;

    // Enemy의 RigidBody
    protected Rigidbody EnemyRigid;

    // Enemy가 총을 쏴 줄 데미지
    protected float EnemyBulletDamage;
    // Enemy가 Player와 충돌하였을 때 줄 데미지
    public float EnemyColDamage;

    // Enemy가 진행할 방향
    protected Vector3 EnemyDir;
    // Enemy가 움직이는 벡터
    protected Vector3 EnemyMovement;

    // Enemy Sprite
    public SpriteRenderer EnemySprite;
    // Enemy가 사용할 총
    protected GameObject Gun;
    // Enemy의 손
    public GameObject enemyHand;
    protected Vector3 rightHandPos;

    public Animator enemyAnim;
    protected bool IsIdle;
    protected bool IsWalk;
    protected bool IsHit = false;
    protected bool _IsDead = false;
    public bool IsDead { get { return _IsDead; } }

    // 왼쪽 이동 애니메이션일 경우
    bool IsAnimLeft = false;

    public List<AudioClip> DeathClip;


    // Enemy의 정보를 초기화하는 함수
    protected void EnemyInit(CharType enemyType, float enemyHp, float enemySpeed, float enemyDamage, float collisionDamage, float alertDis, GameObject gun)
    {
        CharInit(enemyType, enemyHp, enemySpeed);
        EnemyBulletDamage = enemyDamage;
        EnemyColDamage = collisionDamage;
        AlertDis = alertDis;
        Gun = gun;

        EnemyRigid = GetComponent<Rigidbody>();
    }

    // 플레이어를 찾는 함수
    protected void FindPlayer()
    {
        Player = GameObject.FindGameObjectWithTag("Player");    
    }

    // Enemy와 플레이어 사이의 각도를 설정하는 함수
    protected void SetPlayerDegree()
    {
        // 플레이어의 스크린 상의 좌표 저장
        Vector3 playerScreenPos = CameraEulerAngle.playerCamera.WorldToScreenPoint(Player.transform.position);
        // Enemy의 스크린 상의 좌표 저장
        Vector3 enemyScreenPos = CameraEulerAngle.playerCamera.WorldToScreenPoint(transform.position);

        PlayerDegree = CameraEulerAngle.CalculateDegreeOnScreen(enemyScreenPos, playerScreenPos);
    }

    // 마우스의 위치에 따라 스프라이트를 회전시키는 함수
    protected void RotateEnemySprite()
    {
        // 플레이어 각도 계산
        SetPlayerDegree();

        if (IsIdle)
        {
            if (PlayerDegree >= -120 && PlayerDegree < -60)
            {
                enemyAnim.SetTrigger("Idle_Front");
            }
            else if (PlayerDegree >= 30 && PlayerDegree < 150)
            {
                enemyAnim.SetTrigger("Idle_Back");
            }
            else
            {
                enemyAnim.SetTrigger("Idle_Front_Right");
            }
        }
        else if (IsWalk)
        {
            if (PlayerDegree >= -120 && PlayerDegree < -60)
            {
                enemyAnim.SetTrigger("Walk_Front");
            }
            else if (PlayerDegree >= 30 && PlayerDegree < 150)
            {
                enemyAnim.SetTrigger("Walk_Back");
            }
            else
            {
                enemyAnim.SetTrigger("Walk_Left");
                IsAnimLeft = true;
            }
        }


        if (!IsHit)
        {
            // player가 enemy 기준 오른쪽에 있을 때
            if ((PlayerDegree >= -90 && PlayerDegree <= 90))
            {
                if (!IsAnimLeft)
                {
                    EnemySprite.flipX = false;

                }
                else
                {
                    EnemySprite.flipX = true;
                }

                enemyHand.transform.localPosition = rightHandPos;
            }
            // player가 enemy 기준 왼쪽에 있을 때
            else
            {
                if (!IsAnimLeft)
                {
                    EnemySprite.flipX = true;
                }
                else
                {
                    EnemySprite.flipX = false;
                }

                enemyHand.transform.localPosition = new Vector3(-rightHandPos.x, rightHandPos.y, rightHandPos.z);
            }
        }

        if (PlayerDegree < 0)
            enemyHand.transform.localPosition = new Vector3(enemyHand.transform.localPosition.x, enemyHand.transform.localPosition.y, -2.1f);
        else
            enemyHand.transform.localPosition = new Vector3(enemyHand.transform.localPosition.x, enemyHand.transform.localPosition.y, 0f);

        Gun.GetComponent<Gun>().SetGunmanTransform(gameObject);
        Gun.GetComponent<Gun>().RotateGunSprite(PlayerDegree);

        IsAnimLeft = false;
    }

    #region Enemy move

    // Enemy가 플레이어와 떨어져 있을 거리
    protected float AlertDis;
    // 플레이어와 Enemy 사이의 각도에 따라 보정된 AlertDis
    protected float RevisedAlertDis;

    Vector3[] path;
    int targetIndex;
    // 가로로 한 노드만큼 이동할 때 걸리는 시간
    float TimeForOneNode
    {
        get
        {
            return 1 / Speed;
        }
    }
    // 대각선으로 한 노드만큼 이동할 때 걸리는 시간
    float TimeForOneNodeDiagonol
    {
        get
        {
            return Mathf.Sqrt(2) / Speed;
        }
    }
    // 새로운 경로를 탐색할 수 있는 상황인지를 저장할 변수
    protected bool canFindNewPath = true;
    // 이동할 경로의 어떤 부분이 대각선 이동인지를 저장할 변수
    bool[] isPathDiagonol;

    // 어떤 부분이 대각선 이동인지 체크
    void CheckDiagonol(Vector3[] path)
    {
        isPathDiagonol = new bool[path.Length];

        for(int i=0;i<path.Length; i++)
        {
            if (path[i].x == 0 || path[i].z == 0)
            {
                isPathDiagonol[i] = false;
            }
            else
            {
                isPathDiagonol[i]= true;
            }
        }
    }

    public void OnPathFound(Vector3[] newpath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = newpath;
            CheckDiagonol(path);

            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    // 평행 이동에 걸리는 시간인지 대각선 이동에 걸리는 시간인지를 정하는 함수
    float SetDuration(bool isDiagonol)
    {
        if (isDiagonol)
        {
            canFindNewPath = false;
            return TimeForOneNodeDiagonol;
        }
        else
        {
            canFindNewPath = true;
            return TimeForOneNode;
        }
    }

    // 경로를 따라가는 함수
    IEnumerator FollowPath()
    {
        Vector3 currentWayPoint = path[0];
        bool currentPathDiagonol = isPathDiagonol[0];
        float currentDuration = SetDuration(currentPathDiagonol);


        Vector3 movement;
        float time = 0;
        targetIndex = 0;

        while (true)
        {
            // 움직일 수 있다면
            if (IsWalk&&!_IsDead&&!IsHit)
            {
                // 한 node만큼 이동했다면 다음 노드로 이동
                if (time > currentDuration)
                {
                    time = 0;
                    targetIndex++;

                    if (targetIndex >= path.Length)
                    {
                        yield break;
                    }

                    currentWayPoint = path[targetIndex];
                    currentPathDiagonol = isPathDiagonol[targetIndex];
                    currentDuration = SetDuration(currentPathDiagonol);
                }

                time += Time.fixedDeltaTime;
                movement = currentWayPoint;
                movement = movement.normalized * Speed * Time.fixedDeltaTime;
                movement.z = CameraEulerAngle.ReviseZAxis(movement.z);
                EnemyRigid.MovePosition(transform.position + movement);
            }

            yield return new WaitForFixedUpdate();

        }
    }

    protected void SetAlertDis()
    {
        PlayerDir = Player.transform.position - transform.position; // 플레이어 위치의 Enemy로부터의 방향 설정
        PlayerDir = new Vector3(PlayerDir.x, 0, PlayerDir.z);
        PlayerDis = PlayerDir.magnitude;    // Enemy와 플레이어 사이의 거리 측정
        RevisedAlertDis = CameraEulerAngle.ReviseMagnitude(AlertDis, PlayerDir); // 플레이어와 Enemy 사이의 각도에 따라 Enemy가 멈출 거리 보정

        if (PlayerDis >= RevisedAlertDis)
        {
            IsIdle = false;
            IsWalk = true;

            enemyAnim.SetBool("IsIdle", false);
            enemyAnim.SetBool("IsWalk", true);
        }
        else
        {
            IsIdle = true;
            IsWalk = false;

            enemyAnim.SetBool("IsIdle", true);
            enemyAnim.SetBool("IsWalk", false);
        }
    }

    #endregion

    // Enemy의 Spawn이 끝났을 때 실행될 함수
    public virtual void EnemySpawn()
    {
        IsReady = true;

        enemyHand.GetComponent<SpriteRenderer>().color = new Vector4(1, 1, 1, 1);
        Gun.GetComponent<SpriteRenderer>().color = new Vector4(1, 1, 1, 1);
    }

    protected virtual void EnemyFireBullet() { }

    protected override void CharDead()
    {
        Destroy(gameObject);
    }

    // Enemy가 죽은 이후 남은 시체에 관련된 함수
    public virtual void EnemyCorpse()    { }
}
