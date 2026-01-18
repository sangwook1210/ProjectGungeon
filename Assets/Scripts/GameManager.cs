using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using TMPro;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindAnyObjectByType(typeof(GameManager)) as GameManager;
            }
            return _instance;
        }
    }

    public float PlayTime = 0f;

    public bool GameStart = false;
    private bool GamePause = false;
    private bool GameFinish = false;
    private bool PlayerSpawn = false;
    public float ReadyTime = 1.0f;
    private bool CanSpawnEnemy = true;
    private float EnemySpawnCoolTime = 1.0f;


    public GameObject AstarManager;
    public GameObject PlayerCamera;

    public GameObject SpawnObject;
    public GameObject PlayerPrefab;
    public GameObject BulKinPrefab;

    private GameObject Player;
    private GameObject[] EnemyList = new GameObject[4];

    private int _score; // 플레이어의 점수
    public int score
    {
        get
        {
            return _score;
        }
    }
    public TextMeshProUGUI ScoreText;
    public GameObject ResultBook;
    public GameObject FadeEffect;

    public enum MapArea
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight,
    }

    private AudioSource _audioSource;
    public AudioClip BGM;

    private void SetPlayerSpawn()
    {
        PlayerSpawn = true;
    }
    private void SpawnPlayer()
    {
        if (GameStart)
        {
            PlayerCamera.GetComponent<AudioListener>().enabled = false; // 플레
            Player=Instantiate(PlayerPrefab, new Vector3(SpawnObject.transform.position.x, 0.6f, SpawnObject.transform.position.z + 1.5f), Quaternion.identity);

            GameStart = false;
            Invoke("SetPlayerSpawn", ReadyTime);

            
            ScoreText.text = _score.ToString();
        }
    }

    public void IncreaseScore(int plusScore)
    {
        _score += plusScore;
        ScoreText.text = _score.ToString();
    }

    private void SpawnEnemy()
    {
        if (PlayerSpawn && CanSpawnEnemy)
        {
            int EnemyCount = 0;
            int AreaNum = 0;

            for (int i = 0; i < EnemyList.Length; i++)
            {
                if (EnemyList[i] != null)
                    EnemyCount++;
                else
                    AreaNum = i;
            }

            if (EnemyCount < 4)
            {
                Vector3 EnemySpawnPos = AstarManager.GetComponent<Grid>().SelectWalkableNode((MapArea)(AreaNum));

                if (EnemySpawnPos != null)
                {
                    GameObject Enemy = Instantiate(BulKinPrefab, new Vector3(EnemySpawnPos.x, 0.6f, EnemySpawnPos.z), Quaternion.identity);

                    EnemyList[AreaNum] = Enemy;
                    CanSpawnEnemy = false;
                    StartCoroutine(CheckSpawnCool());
                }
            }
        }
    }

    IEnumerator CheckSpawnCool()
    {
        yield return new WaitForSeconds(EnemySpawnCoolTime);
        CanSpawnEnemy = true;
    }

    // 게임이 종료되었을 때 실행될 함수
    public void OnGameFinish()
    {
        // 게임 정지
        Time.timeScale = 0;
        GameFinish = true;
        _audioSource.Stop();
        FadeEffect.SetActive(true); 
        GameObject.FindGameObjectWithTag("UIManager").GetComponent<PlayerUI>().GameFinish();
        ScoreText.gameObject.SetActive(false);

        // 하이스코어 설정
        if (PlayerPrefs.HasKey("HighScore"))
        {
            int highScore = PlayerPrefs.GetInt("HighScore");
            if (highScore < _score)
            {
                highScore = _score;
            }
            PlayerPrefs.SetInt("HighScore", highScore);
        }
        else
        {
            PlayerPrefs.SetInt("HighScore", _score);
        }

        PlayerCamera.GetComponent<CameraPosition>().GameFinish();
    }

    private void OnGamePause()
    {
        // esc 입력
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // 게임이 종료되지 않았다면
            if (!GameFinish)
            {
                // 게임이 일시정지된 상태라면 일시정지 해제
                if (GamePause)
                {
                    GamePause = false;
                    ResultBook.GetComponent<ResultBook>().GamePauseFinish();
                    _audioSource.Play();

                    if (Player != null)
                    {
                        Player.GetComponent<Player>().IsReady = true;
                    }
                }

                // 게임이 플레이중이던 상태라면 일시정지
                else
                {
                    Time.timeScale = 0;
                    GamePause = true;
                    ShowResultBook();
                    _audioSource.Pause();

                    if (Player != null)
                    {
                        Player.GetComponent<Player>().IsReady = false;
                    }
                }
            }
        }
    }

    private void TimeCheck()
    {
        PlayTime += Time.deltaTime;
    }
    public void ShowResultBook()
    {
        if (!ResultBook.activeSelf)
        {
            ResultBook.SetActive(true);
        }
        ResultBook.GetComponent<ResultBook>().SetResultBook(GameFinish);
    }



    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }

        // 프리팹 로드
        PlayerPrefab = Resources.Load<GameObject>("Prefabs/Characters/Player/Player");
        BulKinPrefab = Resources.Load<GameObject>("Prefabs/Characters/Enemy/EnemyNormal/Bullet_Kin");

        Time.timeScale = 1;
    }

    private void Start()
    {

        GameStart = false;
        PlayerSpawn = false;
        CanSpawnEnemy = true;
        SpawnObject = GameObject.FindGameObjectWithTag("PlayerSpawn");
        PlayerCamera = GameObject.FindGameObjectWithTag("MainCamera");
        AstarManager = GameObject.FindGameObjectWithTag("AstarManager");
        SpawnObject.GetComponent<Altar>().ActiveAltar();
        _audioSource = GetComponent<AudioSource>();
        ScoreText.text = "";

        _audioSource.Play();    // bgm 재생
    }

    private void Update()
    {
        SpawnPlayer();
        SpawnEnemy();
        TimeCheck();
        OnGamePause();
    }
}
