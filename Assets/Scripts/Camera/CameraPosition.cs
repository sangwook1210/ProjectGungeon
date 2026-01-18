using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPosition : MonoBehaviour
{
    // 카메라가 따라다닐 플레이어 오브젝트
    public GameObject Player;

    // 플레이어 카메라
    public Camera playerCamera;
    // 기본 카메라
    public Camera baseCamera;
    // 카메라의 위치 설정
    private Vector3 cameraPos;
    // 기본 카메라의 위치
    private Vector3 baseCameraPos;
    // 마우스의 위치
    private Vector3 mousePos;

    // 플레이어의 위치에 더해질 크기
    private float cameraPos_x = 0f;
    private float cameraPos_y = 30f;
    private float cameraPos_z = -30f;

    // 카메라 진동
    private float shakeTime = 0.3f;
    private float shakeAmount = 5;
    private float shakeSpeed = 3;

    bool gameFinish = false;
    bool canMove = false;   // playerCamera가 움직일 수 있는지를 저장할 변수

    public GameObject DeathReticle;

    // 플레이어의 위치를 기준으로 base Camera의 위치를 설정하는 함수
    private void SetBaseCameraPos()
    {
        baseCameraPos = new Vector3(cameraPos_x, cameraPos_y, cameraPos_z);
        baseCameraPos += Player.transform.position;
        baseCamera.transform.position = baseCameraPos;
    }

    // Base 카메라의 마우스 위치를 기준으로 playerCamera의 위치를 설정하는 함수
    private void SetPlayerCameraPos(Vector3 mousePosition)
    {
        Vector3 mousePosDis = mousePosition - baseCamera.transform.position;    // 마우스의 월드좌표와 baseCamera의 월드좌표 상의 거리

        if (mousePosDis.magnitude > 0.2)    // 거리가 0.2보다 크다면 마우스 위치로 playerCamera가 이동
        {
            transform.position = Vector3.Lerp(transform.position, mousePosition, 0.3f);
        }
        else    // 거리가 0.2 이하라면 baseCamera의 위치로 playerCamera가 이동
        {
            transform.position = Vector3.Lerp(transform.position, baseCamera.transform.position, 0.3f);
        }
    }

    private void setCanMove()
    {
        canMove = true;
    }

    public void ConnectPlayer(GameObject player)
    {
        Player = player;
        Invoke("setCanMove", GameManager.Instance.ReadyTime);
    }

    // 카메라를 흔드는 함수
    public void ShakeCamera()
    {
        StartCoroutine(Shake());
    }

    IEnumerator Shake()
    {
        Vector3 originPos = transform.position;
        float elapseTime = 0;

        while (elapseTime < shakeTime)
        {
            Vector3 randomPoint = originPos + Random.insideUnitSphere * shakeAmount;
            transform.position = Vector3.Lerp(transform.position, randomPoint, Time.deltaTime * shakeSpeed);

            yield return null;
            elapseTime += Time.deltaTime;
        }
    }

    public void GameFinish()
    {
        gameFinish = true;

        StartCoroutine(FinishCameraMove());
    }
    private IEnumerator FinishCameraMove()
    {
        while (Mathf.Abs((transform.position - baseCamera.transform.position).magnitude) > 0.005f)
        {
            // 카메라 확대
            gameObject.GetComponent<Camera>().orthographicSize = Mathf.Lerp(gameObject.GetComponent<Camera>().orthographicSize, 6, 0.01f);
            // 카메라 위치를 플레이어 기준으로 이동
            transform.position = Vector3.Lerp(transform.position, baseCamera.transform.position, 0.01f);

            yield return null;
        }

        // 카메라 연출 종료 후 방아쇠 연출 진행
        DeathReticle.SetActive(true);
    }

    private void FixedUpdate()
    {
        if (canMove&&!gameFinish)
        {
            SetBaseCameraPos();

            // base 카메라를 통해 마우스 입력의 월드 포지션을 받아옴
            mousePos = baseCamera.ScreenToWorldPoint(Input.mousePosition);
            SetPlayerCameraPos(mousePos);
        }
    }
}
