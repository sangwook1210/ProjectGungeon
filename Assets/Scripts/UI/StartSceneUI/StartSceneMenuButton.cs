using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.SceneManagement;

public class StartSceneMenuButton : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    public bool IsSelected;
    public TextMeshProUGUI ButtonText;
    private float gray = 0.5f;
    public AudioSource ButtonAudio;
    public AudioClip SelectClip;

    bool StartSceneStart = true;

    public void OnPlayButtonClick()
    {
        ButtonAudio.PlayOneShot(SelectClip);
        SceneManager.LoadScene("PlayScene");

    }

    public void OnExitButtonClick()
    {
        ButtonAudio.PlayOneShot(SelectClip);
        Application.Quit();
    }

    public void OnSelected(bool selected)
    {
        IsSelected = selected;

        if (IsSelected)
        {
            if (!StartSceneStart)
            {
                ButtonAudio.PlayOneShot(SelectClip);
            }
            SetWhite();
            StartSceneStart = false;
        }
        else
        {
            SetGray();
            StartSceneStart = false;
        }
    }

    public void SetWhite()
    {
        ButtonText.color = new Vector4(1, 1, 1, 1);
    }
    public void SetGray()
    {
        ButtonText.color = new Vector4(gray, gray, gray, 1);

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetWhite();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!IsSelected)
        {
            SetGray();
        }
    }

    private void Start()
    {
        ButtonText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        ButtonAudio = gameObject.GetComponent<AudioSource>();
        ButtonAudio.volume = 0.4f;
    }
}
