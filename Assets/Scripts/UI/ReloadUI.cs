using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadUI : MonoBehaviour
{
    public GameObject ReloadBar;

    public SpriteRenderer ReloadSprite;
    public SpriteRenderer ReloadBarSprite;

    Vector4 ActiveColor = new Vector4(1, 1, 1, 1);
    Vector4 DisableColor = new Vector4(1, 1, 1, 0);

    float ReloadBarAbsoluteVal;
    Vector3 ReloadBarStartingPos;   // ReloadBar의 시작 위치


    // ReloadUI를 화면에 보이게 하는 함수
    private void ActiveReloadUI()
    {
        ReloadSprite.color = ActiveColor;
        ReloadBarSprite.color = ActiveColor;

        ReloadBar.transform.localPosition = ReloadBarStartingPos;
    }

    // ReloadUI를 화면에 보이지 않게 하는 함수
    public void DisableReloadUI()
    {
        ReloadSprite.color = DisableColor;
        ReloadBarSprite.color = DisableColor;
    }

    private void SetReloadBar(float TimeP)
    {
        float ReloadBarLocalPosX = ReloadBarAbsoluteVal * 2 * TimeP / 100 - ReloadBarAbsoluteVal;

        ReloadBar.transform.localPosition = new Vector3(ReloadBarLocalPosX, 0, 0);
    }

    // 외부에서 호출될 Reload 함수
    public void Reload(float reloadTime)
    {
        ActiveReloadUI();   // ReloadUI를 보이게 설정
        StartCoroutine(ReloadAnim(reloadTime)); // ReloadUI 애니메이션 재생
    }
    
    IEnumerator ReloadAnim(float reloadTime)
    {
        float ReloadTime = reloadTime;
        float time = 0;

        while (time <= ReloadTime)
        {
            time += Time.deltaTime;
            SetReloadBar(time * 100 / ReloadTime);

            yield return null;
        }

        DisableReloadUI();
    }

    private void Start()
    {
        ReloadBarAbsoluteVal = 0.875f;
        ReloadBarStartingPos = new Vector3(-ReloadBarAbsoluteVal, 0, 0);

        DisableReloadUI();
    }
}
