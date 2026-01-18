using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ResultBook : MonoBehaviour
{
    private List<GameObject> GunInventory = new List<GameObject>();
    private List<ItemData> ItemInventory = new List<ItemData>();

    public List<GameObject> Pages = new List<GameObject>(); // 페이지
    int currentPage;    // 현재 페이지
    public List<GameObject> Pages_LR = new List<GameObject>();  // 페이지의 좌우

    private bool canInteract = true;    // 버튼을 한번만 작동시키기 위한 변수

    public TextMeshProUGUI ScoreText;
    public TextMeshProUGUI HighScoreText;
    public TextMeshProUGUI TimeText;
    public TextMeshProUGUI StateText1;
    public TextMeshProUGUI StateText2;

    public Transform GunArea;
    public Transform ItemArea;
    public Transform GunAreaStartPos;
    public Transform ItemAreaStartPos;
    public GameObject ItemImagePrefab;

    public GameObject PrevPageButton;

    public Animator ResultBookAnim;
    public AudioSource ResultBookAudio;
    public AudioClip FlipPageClip;
    public AudioClip SelectClip;

    private void SetInventory()
    {
        GameObject Player = GameObject.FindGameObjectWithTag("Player");
        if (Player != null)
        {
            GunInventory = Player.GetComponent<Player>().GunInventory;
            ItemInventory = Player.GetComponent<Player>().ItemInventory;
        }
    }
    private void SetPages_LR()
    {
        for (int i = 0; i < Pages.Count; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                Pages_LR.Add(Pages[i].transform.GetChild(j).gameObject);
            }
        }
    }

    #region ResultBook Animation

    public void NextPage()
    {
        if (canInteract)
        {
            currentPage++;

            ResultBookAudio.PlayOneShot(FlipPageClip);
            ResultBookAnim.SetTrigger("FlipNext");
            canInteract = false;
        }
        
    }
    public void PrevPage()
    {
        if (canInteract)
        {
            currentPage--;

            ResultBookAudio.PlayOneShot(FlipPageClip);
            ResultBookAnim.SetTrigger("FlipPrev");
            canInteract = false;
        }
    }

    public void FlipPageAnim1()
    {
        Pages_LR[currentPage * 2 - 1].SetActive(false);
    }
    public void FlipPageAnim2()
    {
        Pages_LR[currentPage * 2 - 2].SetActive(false);
        FlipPageAnimFinish();
        Pages_LR[currentPage * 2 + 1].SetActive(true);
        Pages_LR[currentPage * 2 ].SetActive(false);
    }
    public void FlipPageAnim3()
    {       
        Pages_LR[currentPage * 2].SetActive(true);
    }
    public void FlipPageAnim4()
    {
        Pages_LR[currentPage * 2 +2].SetActive(false);
    }
    public void FlipPageAnim5()
    {
        Pages_LR[currentPage * 2 + 3].SetActive(false);
        FlipPageAnimFinish();
        Pages_LR[currentPage * 2].SetActive(true);
        Pages_LR[currentPage * 2+1].SetActive(false);
    }
    public void FlipPageAnim6()
    {
        Pages_LR[currentPage * 2 + 1].SetActive(true);
    }

    public void FlipPageAnimFinish()
    {
        for (int i = 0; i < Pages.Count; i++)
        {
            if (i == currentPage)
            {
                Pages[i].SetActive(true);
            }
            else
            {
                Pages[i].SetActive(false);

            }
        }

        canInteract = true;
    }

    public void DisappearAnimFinish()
    {
        PrevPageButton.SetActive(true);
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }

    #endregion

    private void SetResultText()
    {
        ScoreText.text = GameManager.Instance.score.ToString();
        HighScoreText.text = PlayerPrefs.GetInt("HighScore").ToString();

        TimeText.text = "";
        int time = Mathf.RoundToInt(GameManager.Instance.PlayTime);
        int hour = 0;
        int minute = 0;
        int second = 0;
        if (time >= 3600) {
            hour = Mathf.FloorToInt(time / 3600);
            time -= hour * 3600;
            TimeText.text = TimeText.text+hour.ToString() + "시간\n";
        }
        if (time > 60)
        {
            minute = Mathf.FloorToInt(time / 60);
            time -= minute * 60;
            TimeText.text = TimeText.text + minute.ToString() + "분 ";
        }
        second = time;
        TimeText.text = TimeText.text + second.ToString() + "초";
    }

    private void SetPlayerStateText(bool gameFinish)
    {
        if (gameFinish)
        {
            StateText1.text = "플레이어 사망";
            StateText2.text = "플레이어 사망";
            StateText1.color = new Vector4(0.95f, 0.4f, 0.4f, 1);
            StateText2.color = new Vector4(0.95f, 0.4f, 0.4f, 1);
        }
        else
        {
            StateText1.text = "플레이어 생존 중";
            StateText2.text = "플레이어 생존 중";
            StateText1.color = new Vector4(1, 1, 1, 1);
            StateText2.color = new Vector4(1, 1, 1, 1);
        }
    }

    private void SetGunArea()
    {
        // 이미 생성된 image가 있다면
        if (GunArea.transform.childCount != 1)
        {
            int childnum = GunArea.transform.childCount;
            for (int i=1;i< childnum; i++)
            {
                Destroy(GunArea.transform.GetChild(i).gameObject);
            }
        }

        Vector3 GunAreaPos = GunAreaStartPos.position;

        for(int i=0;i<GunInventory.Count;i++)
        {
            if (i == 0)
            {
                GameObject ItemObject = Instantiate(ItemImagePrefab);
                ItemObject.transform.SetParent(GunArea);
                Image ItemImage = ItemObject.GetComponent<Image>();
                ItemImage.sprite = GunInventory[i].GetComponent<Gun>().ItemSprite;
                ItemImage.SetNativeSize();
                ItemImage.rectTransform.position = GunAreaPos;
                GunAreaPos = new(GunAreaPos.x + ItemImage.rectTransform.sizeDelta.x / 2, GunAreaPos.y, GunAreaPos.z);
            }
            else
            {
                GameObject ItemObject = Instantiate(ItemImagePrefab);
                ItemObject.transform.SetParent(GunArea);
                Image ItemImage = ItemObject.GetComponent<Image>();
                ItemImage.sprite = GunInventory[i].GetComponent<Gun>().ItemSprite;
                ItemImage.SetNativeSize();
                GunAreaPos = new(GunAreaPos.x + ItemImage.rectTransform.sizeDelta.x / 2, GunAreaPos.y, GunAreaPos.z);
                ItemImage.rectTransform.position = GunAreaPos;
                GunAreaPos = new(GunAreaPos.x + ItemImage.rectTransform.sizeDelta.x / 2, GunAreaPos.y, GunAreaPos.z);
            }
        }
    }

    private void SetItemArea()
    {
        // 이미 생성된 image가 있다면
        if (ItemArea.transform.childCount != 1)
        {
            int childnum = ItemArea.transform.childCount;
            for (int i = 1; i < childnum; i++)
            {
                Destroy(ItemArea.transform.GetChild(i).gameObject);
            }
        }

        Vector3 ItemAreaPos = ItemAreaStartPos.position;

        for (int i = 0; i < ItemInventory.Count; i++)
        {
            if (i == 0)
            {
                GameObject ItemObject = Instantiate(ItemImagePrefab);
                ItemObject.transform.SetParent(ItemArea);
                Image ItemImage = ItemObject.GetComponent<Image>();
                ItemImage.sprite = ItemInventory[i].itemSprite;
                ItemImage.SetNativeSize();
                ItemImage.rectTransform.position = ItemAreaPos;
                ItemAreaPos = new(ItemAreaPos.x + ItemImage.rectTransform.sizeDelta.x / 2, ItemAreaPos.y, ItemAreaPos.z);
            }
            else
            {
                GameObject ItemObject = Instantiate(ItemImagePrefab);
                ItemObject.transform.SetParent(ItemArea);
                Image ItemImage = ItemObject.GetComponent<Image>();
                ItemImage.sprite = ItemInventory[i].itemSprite;
                ItemImage.SetNativeSize();
                ItemAreaPos= new(ItemAreaPos.x + ItemImage.rectTransform.sizeDelta.x / 2, ItemAreaPos.y, ItemAreaPos.z);
                ItemImage.rectTransform.position = ItemAreaPos;
                ItemAreaPos = new(ItemAreaPos.x + ItemImage.rectTransform.sizeDelta.x / 2, ItemAreaPos.y, ItemAreaPos.z);
            }
        }
    }

    public void TitleButtonDown()
    {
        ResultBookAudio.PlayOneShot(SelectClip);
        SceneManager.LoadScene("StartScene");
    }
    public void RestartButtonDown()
    {
        ResultBookAudio.PlayOneShot(SelectClip);
        SceneManager.LoadScene("PlayScene");
    }
    public void QuitButtonDown()
    {
        ResultBookAudio.PlayOneShot(SelectClip);
        Application.Quit();
    }

    public void SetResultBook(bool gameFinish)
    {
        ResultBookAnim.Rebind();

        // 게임이 종료되었다면
        if (gameFinish)
        {
            currentPage = 0;         
        }
        // 게임이 일시정지되었다면
        else
        {
            currentPage = 1;
            PrevPageButton.SetActive(false);
        }

        SetInventory();
        SetPages_LR();
        FlipPageAnimFinish();
        SetResultText();
        SetGunArea();
        SetItemArea();
        SetPlayerStateText(gameFinish);
    }

    public void GamePauseFinish()
    {
        ResultBookAnim.SetTrigger("Disappear");
    }
}
