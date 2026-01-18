using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartSceneManager : MonoBehaviour
{
    public List<Button> Buttons = new List<Button>();
    private int SelectedButtonNum = 0;

    public AudioSource StartSceneAudio;

    // 방향키로 메뉴를 선택하는 함수
    private void SetSelectedButton()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (SelectedButtonNum == 0)
            {
                SelectedButtonNum = Buttons.Count - 1;
            }
            else
            {
                SelectedButtonNum--;
            }

            SetButtonSelected();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (SelectedButtonNum == Buttons.Count-1)
            {
                SelectedButtonNum = 0;
            }
            else
            {
                SelectedButtonNum++;
            }

            SetButtonSelected();
        }
    }

    // 버튼이 선택되었을 때를 설정하는 함수
    private void SetButtonSelected()
    {
        for(int i=0; i<Buttons.Count;i++)
        {
            if (i == SelectedButtonNum)
            {
                Buttons[i].GetComponent<StartSceneMenuButton>().OnSelected(true);
            }
            else
            {
                Buttons[i].GetComponent<StartSceneMenuButton>().OnSelected(false);
            }
        }
    }

    // 엔터 키로 선택된 버튼을 클릭하는 함수
    private void ButtonClick()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Buttons[SelectedButtonNum].onClick.Invoke();
        }
    }

    void Start()
    {
        Time.timeScale = 1;

        SelectedButtonNum = 0;
        SetButtonSelected();

        StartSceneAudio.Play();
    }

    void Update()
    {
        SetSelectedButton();
        ButtonClick();
    }
}
