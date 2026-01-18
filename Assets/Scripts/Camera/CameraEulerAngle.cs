using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEulerAngle : MonoBehaviour
{
    public static Camera playerCamera;
    public static Vector3 cameraEulerAngle = new(45, 0, 0); // 카메라의 오일러각 설정

    // 카메라의 오일러각에 따른 sin값을 람다식으로 설정
    public static float sinEulerAngle => Mathf.Sin(Mathf.Deg2Rad * cameraEulerAngle.x);
    // 화면 좌표계에서의 45도 각도가 월드 좌표계에서 몇 도인지를 저장할 변수
    public static float Screen45ToWorld => Mathf.Atan(ReviseZAxis(1)) * Mathf.Rad2Deg;
    // Screen45ToWorld의 절반 값이 몇 도인지를 저장할 변수
    public static float Screen45ToWorldHalf => Screen45ToWorld / 2;
    // 90도에서 Screen45ToWorld를 뺀 값의 절반 값이 몇 도인지를 저장할 변수
    public static float Screen45ToWorldCompHalf => (90 - Screen45ToWorld) / 2;

    /// <summary>
    /// z축의 움직임을 보정할 때 사용
    /// </summary>
    /// <param name="originZ"> 보정할 z축 값 </param>
    /// <returns> 보정할 원본 z축 값 / sin </returns>
    public static float ReviseZAxis(float originZ)
    {
        return originZ / sinEulerAngle;
    }

    /// <summary>
    /// 원본 길이(크기, 속도)를 정규화된 벡터에 따라 보정할 때 사용
    /// </summary>
    /// <param name="originMagnitude"> 원본 길이 </param>
    /// <param name="originVector"> 원본 벡터 </param>
    /// <returns> 보정할 원본 길이 / √(x^2+(z*sin)^2) </returns>
    public static float ReviseMagnitude(float originMagnitude, Vector3 originVector)
    {
        Vector3 normalizedVector = originVector.normalized;

        return originMagnitude / Mathf.Sqrt(Mathf.Pow(normalizedVector.x, 2) + Mathf.Pow(normalizedVector.z * sinEulerAngle, 2));
    }

    // 플레이어가 화면 상에서 클릭한 점의 화면 상의 각도를 반환하는 함수
    public static float CalculateDegreeOnScreen(Vector3 playerScreenPos, Vector3 clickedScreenPos)
    {
        // 플레이어의 스크린 상의 좌표의 z 좌표 초기화
        playerScreenPos = new Vector3(playerScreenPos.x, playerScreenPos.y, 0);

        // 플레이어가 클릭한 좌표의 플레이어의 위치를 원점으로 한 벡터 저장
        Vector3 screenVector = clickedScreenPos - playerScreenPos;
        // 위 벡터의 크기 저장
        float screenVectorMagnitude = screenVector.magnitude;

        // 벡터의 화면 상의 각도 저장
        float screenDegree = Mathf.Atan2(screenVector.y, screenVector.x) * Mathf.Rad2Deg;

        return screenDegree;
    }

    // 카메라로부터 ray가 발사되었을 때, 플레이어(Tr)의 y축 높이와 같은 직선 위의 점을 반환하는 함수
    public static Vector3 FindPointOnVector(Vector3 Tr, Vector3 StartPoint, Vector3 EndPoint)
    {
        float y = Tr.y;
        float x = (y - StartPoint.y) / (EndPoint.y - StartPoint.y) * (EndPoint.x - StartPoint.x) + StartPoint.x;
        float z = (y - StartPoint.y) / (EndPoint.y - StartPoint.y) * (EndPoint.z - StartPoint.z) + StartPoint.z;

        return new Vector3(x, y, z);
    }

    private void Start()
    {
        // 카메라 설정
        playerCamera = Camera.main;

        // 카메라의 오일러 각 설정
        gameObject.transform.eulerAngles = cameraEulerAngle;
    }
}
