using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Player : Character
{
    public Camera PlayerCamera; // 플레이어의 카메라 게임오브젝트
    protected Rigidbody player_rigid;

    protected CharType playerType = CharType.Player;
    protected float playerHp = 6f;  // 플레이어의 Hp
    protected float playerSpeed = 9.0f;    // 플레이어의 속도

    private Vector3 DodgeDir;   // 구르는 방향
    private readonly float DodgeSpeed = 7.0f;    // 구르는 속도
    bool DodgeStart = false;    // Dodge가 시작되었는지를 저장할 변수
    bool ReservedDodge = false; // 다음 Dodge가 예약되었는지를 저장할 변수
    bool DodgeAlmostFinish = false; // Dodge가 거의 끝난 상태를 나타낼 변수
    bool IsDodgeInvinsible = false;   // Dodge 중 무적 상태를 나타낼 변수
    Vector3 ReservedDodgeDir;   // 다음 예약된 Dodge의 이동 방향을 저장할 변수

    public bool IsIdle = true;
    public bool IsWalk = false;
    public bool IsDodge = false;   // 플레이어가 구르기(무적) 상태인지를 나타낼 변수
    private readonly float ColInvincibleTime = 1.2f; // 피격 무적 시간
    private bool IsColInvincible = false;  // 플레이어가 피격 무적 상태인지를 나타낼 변수

    public SpriteRenderer PlayerSprite; // 플레이어의 SpriteRenderer
    public Animator playerAnim; // 플레이어의 animatior
    public GameObject ReloadUI; // Reload를 하는 UI
    //public GameObject PlayerUI; // Player의 상태 정보를 나타내는 UI
    public GameObject playerHand;   // 플레이어의 오른쪽 손 오브젝트

    private Vector3 gunPos = new Vector3(0.333f, -0.312f, 0.1f);
    private Vector3 rightHandPos;

    public GameObject currentGun;  //현재 착용 중인 총
    public List<GameObject> GunInventory = new List<GameObject>();  // 플레이어의 총 인벤토리
    public int currentGunNum;
    public List<ItemData> ItemInventory = new List<ItemData>(); // 플레이어의 아이템 인벤토리

    public AudioClip dodgeClip;
    public AudioClip leapClip;

    // 8방위
    public enum CardinalPoint
    {
        N,NE,E,SE,S,SW,W,NW
    }
    private CardinalPoint Vector2Cardinal(Vector3 playerdir)
    {
        CardinalPoint returnCardinal;

        if (playerdir.x == 0 && playerdir.z > 0)
        {
            returnCardinal = CardinalPoint.N;
        }
        else if (playerdir.x > 0 && playerdir.z > 0)
        {
            returnCardinal = CardinalPoint.NE;
        }
        else if (playerdir.x> 0 && playerdir.z == 0)
        {
            returnCardinal = CardinalPoint.E;
        }
        else if (playerdir.x > 0 && playerdir.z < 0)
        {
            returnCardinal = CardinalPoint.SE;
        }
        else if (playerdir.x ==0  && playerdir.z < 0)
        {
            returnCardinal = CardinalPoint.S;
        }
        else if (playerdir.x < 0 && playerdir.z < 0)
        {
            returnCardinal = CardinalPoint.SW;
        }
        else if (playerdir.x < 0 && playerdir.z == 0)
        {
            returnCardinal = CardinalPoint.W;
        }
        else
        {
            returnCardinal = CardinalPoint.NW;
        }

        return returnCardinal;
    }
    private Vector3 Cardinal2Vector(CardinalPoint cardinal)
    {
        Vector3 returnVector;

        if (cardinal == CardinalPoint.N)
        {
            returnVector = new(0, 0, 1);
        }
        else if (cardinal == CardinalPoint.NE)
        {
            returnVector = new(1, 0, 1);
        }
        else if (cardinal == CardinalPoint.E)
        {
            returnVector = new(1, 0, 0);
        }
        else if (cardinal == CardinalPoint.SE)
        {
            returnVector = new(1, 0, -1);
        }
        else if (cardinal == CardinalPoint.S)
        {
            returnVector = new(0, 0, -1);
        }
        else if (cardinal == CardinalPoint.SW)
        {
            returnVector = new(-1, 0, -1);
        }
        else if (cardinal == CardinalPoint.W)
        {
            returnVector = new(-1, 0, 0);
        }
        else
        {
            returnVector = new(-1, 0, 1);
        }

        return returnVector;
    }

    public void GetReady()
    {
        IsReady = true;
    }

    // 플레이어의 정보를 초기화하는 함수
    protected void PlayerInit(CharType playerType, float playerHp, float playerSpeed)
    {
        CharInit(playerType, playerHp, playerSpeed);
    }

    // 플레이어의 최대 체력이 변동되었을 때 실행될 함수
    public void PlayerMaxHpChange(bool Plus, float amount, bool FullHealth)
    {
        if (Plus)
        {
            MaxHp += amount;
            Hp += amount;
        }
        else
        {
            MaxHp -= amount;

            if (Hp > MaxHp)
            {
                Hp = MaxHp;
            }
        }

        if (FullHealth)
        {
            Hp = MaxHp;
        }

        PlayerUI.Instance.OnPlayerHpChange(Hp, MaxHp);
    }

    public void CharHpIncrease(float value)
    {
        Hp += value;

        if (Hp > MaxHp)
            Hp = MaxHp;

        PlayerUI.Instance.OnPlayerHpChange(Hp, MaxHp);
    }

    protected override void CharHpDecrease(float damage)
    {
        base.CharHpDecrease(damage);
        PlayerUI.Instance.OnPlayerHpChange(Hp, MaxHp);
    }

    protected override void CharHit(Collider hitObject)
    {
        if (hitObject.gameObject.CompareTag("Bullet") && !IsDodgeInvinsible && !IsColInvincible)
        {
            float damage = hitObject.GetComponent<Bullet>().BulletHit(charType);
            CharHpDecrease(damage);
            if (hitObject.GetComponent<Bullet>().gunman != charType)
            {
                PlayerCamera.GetComponent<CameraPosition>().ShakeCamera();
            }
            if (hitObject.GetComponent<Bullet>().gunman != CharType.Player)
            {
                StartCoroutine(ColInvincible());
                PlayerUI.Instance.OnHitUI();
            }
        }
        else if (hitObject.gameObject.CompareTag("Enemy") && !IsDodgeInvinsible && !IsColInvincible)
        {
            float damage = hitObject.GetComponent<Enemy>().EnemyColDamage;
            CharHpDecrease(damage);
            PlayerCamera.GetComponent<CameraPosition>().ShakeCamera();
            StartCoroutine(ColInvincible());
            PlayerUI.Instance.OnHitUI();
        }

    }

    protected override void CharDead()
    {
        IsReady = false;
        GameManager.Instance.OnGameFinish();
    }
    public void PlayerDeadAnim()
    {
        playerAnim.updateMode = AnimatorUpdateMode.UnscaledTime;

        playerHand.SetActive(false);
        currentGun.SetActive(false);
        playerAnim.SetBool("Dead", true);
    }

    private void Awake()
    {
        PlayerInit(playerType, playerHp, playerSpeed);

        player_rigid = GetComponent<Rigidbody>();
    }

    private void SetPlayerState(bool idle, bool walk, bool dodge)
    {
        IsIdle = idle;
        IsWalk = walk;
        IsDodge = dodge;
    }

    private void SetPlayerAnimState(bool idle, bool walk, bool dodge)
    {
        playerAnim.SetBool("Idle", idle);
        playerAnim.SetBool("Walk", walk);
        playerAnim.SetBool("Dodge", dodge);
    }

    // PlayerAnimator의 트리거들을 초기화하는 함수
    private void ResetPlayerAnimTrigger()
    {
        playerAnim.ResetTrigger("Front_Right");
        playerAnim.ResetTrigger("Front");
        playerAnim.ResetTrigger("Back_Right");
        playerAnim.ResetTrigger("Back");

        playerAnim.ResetTrigger("Walk_Front_Right");
        playerAnim.ResetTrigger("Walk_Front");
        playerAnim.ResetTrigger("Walk_Back_Right");
        playerAnim.ResetTrigger("Walk_Back");

        playerAnim.ResetTrigger("Dodge_Back");
        playerAnim.ResetTrigger("Dodge_Back_Right");
        playerAnim.ResetTrigger("Dodge_Front");
        playerAnim.ResetTrigger("Dodge_Front_Right");
    }

    public Transform[] rayPoints=new Transform[6];

    // 플레이어가 이동 중에 벽에 부딪혔는지를 체크하는 함수
    private Vector3 CheckHitWall(Vector3 movement)
    {
        float rayDistance = 0.55f;  // ray의 거리
        Vector3 returnMove = movement;

        // 세로 방향 이동
        if (Vector2Cardinal(movement) == CardinalPoint.N || Vector2Cardinal(movement) == CardinalPoint.S)
        {
            rayDistance = CameraEulerAngle.ReviseMagnitude(rayDistance, movement);

            for (int i = 0; i < rayPoints.Length; i += 2)
            {
                if (Physics.Raycast(rayPoints[i].position, movement, out RaycastHit hit, rayDistance))
                {
                    if (hit.collider.CompareTag("Wall"))
                        returnMove = new(0, 0, 0);
                }
            }
        }
        // 가로 방향 이동
        else if (Vector2Cardinal(movement) == CardinalPoint.E || Vector2Cardinal(movement) == CardinalPoint.W)
        {
            for (int i = 1; i < rayPoints.Length; i += 2)
            {
                if (Physics.Raycast(rayPoints[i].position, movement, out RaycastHit hit, rayDistance))
                {
                    if (hit.collider.CompareTag("Wall"))
                        returnMove = new(0, 0, 0);
                }
            }
        }
        // 대각선 이동중이라면
        else
        {
            // 대각선의 크기(rayDistance*1.41)을 보정
            rayDistance = CameraEulerAngle.ReviseMagnitude(rayDistance * 1.41f, movement);

            if (Physics.Raycast(transform.position, movement, out RaycastHit hit, rayDistance))
            {
                // 대각선으로 발사한 ray가 벽에 맞았다면
                if (hit.collider.CompareTag("Wall"))
                {
                    // 동서남북 4방향으로 CheckHitWall 진행
                    Vector3[] CheckCardinal = new Vector3[4];
                    CheckCardinal[0] = CheckHitWall(new Vector3(0, 0, 1));  // N
                    CheckCardinal[1] = CheckHitWall(new Vector3(0, 0, -1)); // S
                    CheckCardinal[2] = CheckHitWall(new Vector3(1, 0, 0));  // E
                    CheckCardinal[3] = CheckHitWall(new Vector3(-1, 0, 0)); // W

                    // 움직임의 방향이 북동쪽일 때
                    if (Vector2Cardinal(movement) == CardinalPoint.NE)
                    {
                        if (CheckCardinal[0] == new Vector3(0, 0, 0) && CheckCardinal[2] == new Vector3(0, 0, 0))
                        {
                            returnMove = new(0, 0, 0);
                        }
                        else if (CheckCardinal[0] == new Vector3(0, 0, 0))
                        {
                            returnMove = new(returnMove.x, 0, 0);
                        }
                        else
                        {
                            returnMove = new(0, 0, returnMove.z);
                        }
                    }
                    // 움직임의 방향이 남동쪽일 때
                    else if (Vector2Cardinal(movement) == CardinalPoint.SE)
                    {
                        if (CheckCardinal[1] == new Vector3(0, 0, 0) && CheckCardinal[2] == new Vector3(0, 0, 0))
                        {
                            returnMove = new(0, 0, 0);
                        }
                        else if (CheckCardinal[1] == new Vector3(0, 0, 0))
                        {
                            returnMove = new(returnMove.x, 0, 0);
                        }
                        else
                        {
                            returnMove = new(0, 0, returnMove.z);
                        }
                    }
                    // 움직임의 방향이 남서쪽일 때
                    else if (Vector2Cardinal(movement) == CardinalPoint.SW)
                    {
                        if (CheckCardinal[1] == new Vector3(0, 0, 0) && CheckCardinal[3] == new Vector3(0, 0, 0))
                        {
                            returnMove = new(0, 0, 0);
                        }
                        else if (CheckCardinal[1] == new Vector3(0, 0, 0))
                        {
                            returnMove = new(returnMove.x, 0, 0);
                        }
                        else
                        {
                            returnMove = new(0, 0, returnMove.z);
                        }
                    }
                    // 움직임의 방향이 북서쪽일 때
                    else
                    {
                        if (CheckCardinal[0] == new Vector3(0, 0, 0) && CheckCardinal[3] == new Vector3(0, 0, 0))
                        {
                            returnMove = new(0, 0, 0);
                        }
                        else if (CheckCardinal[0] == new Vector3(0, 0, 0))
                        {
                            returnMove = new(returnMove.x, 0, 0);
                        }
                        else
                        {
                            returnMove = new(0, 0, returnMove.z);
                        }
                    }
                }
            }
        }

        return returnMove;
    }

    // 플레이어가 이동하는 함수
    private void Move()
    {
        // wasd 입력 감지
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        // 벡터의 정규화 진행
        Vector3 movement = new Vector3(h, 0, v);

        // 구르기 중이 아닐 경우 이동
        if (!IsDodge)
        {
            // 움직임 입력이 없을 경우 Idle 상태
            if (movement == Vector3.zero)
            {
                SetPlayerState(true, false, false);
            }
            // 움직임 입력이 있을 경우 Move 상태
            else
            {
                SetPlayerState(false, true, false);

                movement = Speed * Time.fixedDeltaTime * movement.normalized;

                // 플레이어의 z축 움직임 보정
                movement.z = CameraEulerAngle.ReviseZAxis(movement.z);

                // 벽이 있을 경우 움직이지 않음
                movement = CheckHitWall(movement);
                player_rigid.MovePosition(transform.position + movement);
            }
        }
    }


    // 플레이어 구르기를 입력받는 함수
    private void Dodge()
    {
        // wasd 입력 감지
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // 벡터의 정규화 진행
        Vector3 _dodgeDir = new Vector3(h, 0, v);

        // 마우스 우클릭 입력이 들어왔고 Dodge 상태 중이 아니고 입력 벡터가 0,0,0이 아니라면
        if (Input.GetMouseButtonDown(1) && !IsDodge && (_dodgeDir != new Vector3(0, 0, 0)))
        {
            IsDodge = true;
            DodgeStart = true;
            ReservedDodge = false;
            DodgeDir = _dodgeDir;
            playerAnim.SetBool("Dodge", true);
        }

        // Dodge가 거의 끝난 상태 중일 때 키보드 방향 입력과 마우스 우클릭 입력이 들어왔다면
        if (Input.GetMouseButtonDown(1) && DodgeAlmostFinish 
            && (_dodgeDir != new Vector3(0, 0, 0)) && !ReservedDodge)
        {
            // 다음 구르기 예약
            ReservedDodge = true;
            ReservedDodgeDir = _dodgeDir;
        }
    }

    // 구르기로 인해 플레이어가 이동하는 함수
    private void DodgeMove()
    {
        // 구르기를 시작할 때 한번만 실행될 함수
        if (DodgeStart)
        {
            if (ReservedDodge)
            {
                DodgeDir = ReservedDodgeDir;
            }

            // 구르기 시작 시 총 재장전 취소
            foreach (GameObject gun in GunInventory)
            {
                gun.GetComponent<Gun>().StopReload();
            }

            SetPlayerState(false, false, true);
            //playerAnim.SetBool("Dodge", true);
            IsDodgeInvinsible = true;

            if (DodgeDir.x > 0)
            {
                PlayerSprite.flipX = false;
            }
            else
            {
                PlayerSprite.flipX = true;
            }

            ResetPlayerAnimTrigger();

            if (DodgeDir.x == 0 && DodgeDir.z > 0)
            {
                playerAnim.SetTrigger("Dodge_Back");
            }
            else if ((DodgeDir.x > 0 && DodgeDir.z > 0) || (DodgeDir.x < 0 && DodgeDir.z > 0))
            {
                playerAnim.SetTrigger("Dodge_Back_Right");
            }
            else if (DodgeDir.x == 0 && DodgeDir.z < 0)
            {
                playerAnim.SetTrigger("Dodge_Front");
            }
            else
            {
                playerAnim.SetTrigger("Dodge_Front_Right");
            }

            SetGunAlpha(false);
            DodgeStart = false;
            PlayAudioClip(dodgeClip);
            ReservedDodge = false;
        }

        // 구르기 중일 때 플레이어가 움직이는 함수
        if (IsDodge)
        {
            Vector3 DodgeMove;

            DodgeMove = DodgeSpeed * Time.fixedDeltaTime * DodgeDir.normalized;

            // 플레이어의 z축 움직임 보정
            DodgeMove.z = CameraEulerAngle.ReviseZAxis(DodgeMove.z);

            // 벽이 부딪혔을 경우 움직이지 않음
            DodgeMove = CheckHitWall(DodgeMove);
            player_rigid.MovePosition(transform.position + DodgeMove);        
        }
    }

    private void SetGunAlpha(bool visible)
    {
        SpriteRenderer gunSprite = currentGun.GetComponent<SpriteRenderer>();
        SpriteRenderer handSprite = playerHand.GetComponent<SpriteRenderer>();

        if (visible)
        {
            gunSprite.color = new Vector4(gunSprite.color.r, gunSprite.color.g, gunSprite.color.b, 1);
            handSprite.color = new Vector4(handSprite.color.r, handSprite.color.g, handSprite.color.b, 1);
        }
        else
        {
            gunSprite.color = new Vector4(gunSprite.color.r, gunSprite.color.g, gunSprite.color.b, 0);
            handSprite.color = new Vector4(handSprite.color.r, handSprite.color.g, handSprite.color.b, 0);
        }
    }

    // 구르기 애니메이션이 끝났을 때 실행될 함수
    public void DodgeFinish()
    {
        // 다음 구르기가 예약되어있다면
        if (ReservedDodge)
        {
            IsDodge = true;
            DodgeStart = true;
            DodgeAlmostFinish = false;
            playerAnim.SetBool("Dodge", true);
        }

        // 다음 구르기가 예약되어있지 않다면
        else
        {
            SetPlayerState(false, false, false);
            SetGunAlpha(true);
            DodgeAlmostFinish = false;
            ReservedDodge = false;
            ReservedDodgeDir = Vector3.zero;
        }
    }

    // 구르기 무적이 끝났을 때 실행될 함수
    public void DodgeInvinsibleFinish()
    {
        DodgeAlmostFinish = true;
        IsDodgeInvinsible = false;
    }

    // 피격 무적을 실행하는 함수
    private IEnumerator ColInvincible()
    {
        float SetPlayerAlphaByTime(float _time, float _maxTime, float _minTime, bool isDescent)
        {
            float time = _time - _minTime;
            float maxTime = _maxTime - _minTime;
            float color_a;

            if (isDescent)
                color_a = 1 - 1 / maxTime * time;
            else
                color_a = 1 / maxTime * time;


            return color_a;
        }

        IsColInvincible = true;

        float time = 0;

        float color_a;
        float minTime, maxTime;
        bool isDescent = true;  // alpha값이 내려가는 중이라면 true, 올라가는 중이라면 false를 저장할 변수

        float blinkNum = 5; // 최대 몇 번 반짝일지 저장할 변수

        minTime = 0;
        maxTime = minTime + 1 / (blinkNum * 2) * ColInvincibleTime;

        while (true)
        {
            if (time >= maxTime)
            {
                minTime = maxTime;
                maxTime = minTime + 1 / (blinkNum * 2) * ColInvincibleTime;
                isDescent = !isDescent;
            }

            color_a = SetPlayerAlphaByTime(time, maxTime, minTime, isDescent);
            PlayerSprite.color = new Vector4(PlayerSprite.color.r, PlayerSprite.color.g, PlayerSprite.color.b, color_a);

            if (time > ColInvincibleTime)
                break;

            time += Time.deltaTime;

            yield return null;
        }
        
        PlayerSprite.color = new Vector4(PlayerSprite.color.r, PlayerSprite.color.g, PlayerSprite.color.b, 1.0f);
        IsColInvincible = false;
    }

    // 플레이어가 총 발사
    private void Fire()
    {
        // 좌클릭 입력이 들어왔을 경우 총을 발사
        if (Input.GetMouseButtonDown(0)&&!IsDodge)
        {
            currentGun.GetComponent<Gun>().SetGunmanTransform(gameObject);
            currentGun.GetComponent<Gun>().FireBullet(Input.mousePosition);

            PlayerUI.Instance.UpdateAmmoUI();
        }
    }

    private void Reload()
    {
        // 장전 키 입력이 들어오면 장전
        if (Input.GetKeyDown(KeyCode.R))
        {
            ReloadGunInven();
        }
    }

    public void ReloadGunInven()
    {
        foreach (GameObject gun in GunInventory)
        {
            gun.GetComponent<Gun>().ReloadMagazine();
        }
    }

    private float mouseDegree;  // 마우스의 스크린에서의 각도

    // 마우스의 화면 상의 각도를 계산하는 함수
    private void FindMouseDegree(Vector3 InputMousePos)
    {
        // 플레이어의 스크린 상의 좌표 저장
        Vector3 playerScreenPos = CameraEulerAngle.playerCamera.WorldToScreenPoint(transform.position);
        // 플레이어가 클릭한 스크린 상의 좌표저장
        Vector3 clickedScreenPos = Input.mousePosition;

        mouseDegree = CameraEulerAngle.CalculateDegreeOnScreen(playerScreenPos, clickedScreenPos);
    }

    // 마우스의 위치에 따라 플레이어 스프라이트를 회전시키는 함수
    private void RotatePlayerSprite(Vector3 InputMousePos)
    {
        // 마우스 각도 계산
        FindMouseDegree(InputMousePos);

        SetPlayerAnimState(IsIdle, IsWalk, IsDodge);


        // 마우스 각도에 따라 Idle 상태일 때의 애니메이션 설정
        if (IsIdle&&!IsDodge)
        {
            ResetPlayerAnimTrigger();

            if (mouseDegree >= -60 && mouseDegree < 30)
            {
                playerAnim.SetTrigger("Front_Right");
            }
            else if (mouseDegree >= -120 && mouseDegree < -60)
            {
                playerAnim.SetTrigger("Front");
            }
            else if (mouseDegree >= 30 && mouseDegree < 60)
            {
                playerAnim.SetTrigger("Back_Right");
            }
            else if (mouseDegree >= 60 && mouseDegree < 120)
            {
                playerAnim.SetTrigger("Back");
            }
            else if (mouseDegree >= 120 && mouseDegree < 150)
            {
                playerAnim.SetTrigger("Back_Right");
            }
            else
            {
                playerAnim.SetTrigger("Front_Right");
            }
        }
        // 마우스 각도에 따라 Walk 상태일 때의 애니메이션 설정
        else if (IsWalk&&!IsDodge)
        {
            ResetPlayerAnimTrigger();

            if (mouseDegree >= -60 && mouseDegree < 30)
            {
                playerAnim.SetTrigger("Walk_Front_Right");
            }
            else if (mouseDegree >= -120 && mouseDegree < -60)
            {
                playerAnim.SetTrigger("Walk_Front");
            }
            else if (mouseDegree >= 30 && mouseDegree < 60)
            {
                playerAnim.SetTrigger("Walk_Back_Right");
            }
            else if (mouseDegree >= 60 && mouseDegree < 120)
            {
                playerAnim.SetTrigger("Walk_Back");
            }
            else if (mouseDegree >= 120 && mouseDegree < 150)
            {
                playerAnim.SetTrigger("Walk_Back_Right");
            }
            else
            {
                playerAnim.SetTrigger("Walk_Front_Right");
            }
        }

        if (!IsDodge)
        {
            // 마우스가 플레이어 기준 오른쪽에 있을 때
            if (mouseDegree >= -90 && mouseDegree <= 90)
            {
                PlayerSprite.flipX = false;
                playerHand.transform.localPosition = rightHandPos;
            }

            // 마우스가 플레이어 기준 왼쪽에 있을 때
            else
            {
                PlayerSprite.flipX = true;
                playerHand.transform.localPosition = new Vector3(-rightHandPos.x, rightHandPos.y, rightHandPos.z);
            }

            if (mouseDegree < 0)
            {
                playerHand.transform.localPosition = new Vector3(playerHand.transform.localPosition.x, playerHand.transform.localPosition.y, -2.1f);
                playerHand.GetComponent<SpriteRenderer>().sortingOrder = -2;
            }
            else
            {
                playerHand.transform.localPosition = new Vector3(playerHand.transform.localPosition.x, playerHand.transform.localPosition.y, 0f);
                playerHand.GetComponent<SpriteRenderer>().sortingOrder = 0;
            }

            currentGun.GetComponent<Gun>().SetGunmanTransform(gameObject);
            currentGun.GetComponent<Gun>().RotateGunSprite(mouseDegree);
        }
    }

    // 새로운 아이템을 주웠을 때 실행될 함수
    public void GetNewItem(GameObject item)
    {
        ItemInventory.Add(_itemDB.FindItemDataWithId(item.GetComponent<Item>().id));
    }

    // 새로운 총을 주웠을 때 실행될 함수
    public void GetNewGun(GameObject gun)
    {
        GameObject newGun = gun;
        newGun.GetComponent<Gun>().SetGunman(charType);

        GunInventory.Add(newGun);
    }

    // 숫자 키 코드
    private readonly KeyCode[] numKeyCodes = {
    KeyCode.Alpha1,
    KeyCode.Alpha2,
    KeyCode.Alpha3,
    KeyCode.Alpha4,
    KeyCode.Alpha5,
    KeyCode.Alpha6,
    KeyCode.Alpha7,
    KeyCode.Alpha8,
    KeyCode.Alpha9,
    };

    // 숫자 키를 이용하여 총을 변경하는 함수
    private void CurrentGunNumChange()
    {
        for (int i = 0; i < numKeyCodes.Length; i++)
        {
            if (Input.GetKeyDown(numKeyCodes[i]))
            {
                // 입력 받은 키와 currentGunNum이 일치하지 않고 입력 받은 키가 인벤토리 내의 숫자일 경우
                if (currentGunNum != i && i < GunInventory.Count)
                {
                    // 총 변경 시 재장전 취소
                    foreach (GameObject gun in GunInventory)
                    {
                        gun.GetComponent<Gun>().StopReload();
                    }

                    GunInventory[currentGunNum].GetComponent<Gun>().DestructGun();
                    currentGunNum = i;
                    GunInventory[currentGunNum].GetComponent<Gun>().EquipGun();
                    currentGun = GunInventory[currentGunNum];
                    PlayerUI.Instance.SetGunUI(currentGunNum);
                    PlayerUI.Instance.Full_Frame.GetComponent<FullFrameUI>().ChangeUpAnim();

                    break;
                }
            }
        }
    }

    // 마우스 휠을 이용하여 총을 변경하는 함수
    private void CurrentGunWheelChange()
    {
        float wheelInput = Input.GetAxis("Mouse ScrollWheel");

        // 휠을 위로
        if (wheelInput > 0)
        {
            // 총 변경 시 재장전 취소
            foreach (GameObject gun in GunInventory)
            {
                gun.GetComponent<Gun>().StopReload();
            }

            if (currentGunNum+1 < GunInventory.Count)
            {
                GunInventory[currentGunNum++].GetComponent<Gun>().DestructGun();
            }
            else
            {
                GunInventory[currentGunNum].GetComponent<Gun>().DestructGun();
                currentGunNum = 0;
            }

            GunInventory[currentGunNum].GetComponent<Gun>().EquipGun();
            currentGun = GunInventory[currentGunNum];
            PlayerUI.Instance.SetGunUI(currentGunNum);
            PlayerUI.Instance.Full_Frame.GetComponent<FullFrameUI>().ChangeUpAnim();
        }

        // 휠을 아래로
        else if( wheelInput<0)
        {
            // 총 변경 시 재장전 취소
            foreach (GameObject gun in GunInventory)
            {
                gun.GetComponent<Gun>().StopReload();
            }

            if (currentGunNum -1 >=0)
            {
                GunInventory[currentGunNum--].GetComponent<Gun>().DestructGun();              
            }
            else
            {
                GunInventory[currentGunNum].GetComponent<Gun>().DestructGun();
                currentGunNum = GunInventory.Count - 1;
            }

            GunInventory[currentGunNum].GetComponent<Gun>().EquipGun();
            currentGun = GunInventory[currentGunNum];
            PlayerUI.Instance.SetGunUI(currentGunNum);
            PlayerUI.Instance.Full_Frame.GetComponent<FullFrameUI>().ChangeDownAnim();
        }
    }

    // 총알을 보급하는 함수
    public void SupplyAmmo(float ratio)
    {
        currentGun.GetComponent<Gun>().SupplyAmmo(ratio);
    }

    private void Start()
    {
        GunInventory.Add(transform.GetChild(1).transform.GetChild(0).gameObject);
        GunInventory.Add(transform.GetChild(1).transform.GetChild(1).gameObject);


        currentGunNum = 0;
        currentGun = GunInventory[currentGunNum];
        currentGun.GetComponent<Gun>().EquipGun();
        GunInventory[1].GetComponent<Gun>().DestructGun();

        foreach (GameObject gun in GunInventory)
        {
            gun.GetComponent<Gun>().SetGunman(charType);
            gun.transform.localPosition = gunPos;
        }

        rightHandPos = playerHand.transform.localPosition;

        PlayerCamera = Camera.main;
        PlayerCamera.GetComponent<CameraPosition>().ConnectPlayer(gameObject);

        //PlayerUI = GameObject.FindGameObjectWithTag("UIManager");
        PlayerUI.Instance.ConnectPlayer(gameObject);

        Invoke("GetReady", GameManager.Instance.ReadyTime);
    }

    private void Update()
    {
        if (IsReady)
        {
            Dodge();

            Fire();
            Reload();

            CurrentGunNumChange();
            CurrentGunWheelChange();
        }
    }

    private void FixedUpdate()
    {
        if (IsReady)
        {
            Move();
            DodgeMove();
            RotatePlayerSprite(Input.mousePosition);
        }
    }
}
