using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathRequestManager : MonoBehaviour
{
    Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
    PathRequest currentPathRequest;

    static PathRequestManager instance;
    Pathfinding pathfinding;

    bool isProcessingPath;

    private void Awake()
    {
        instance = this;
        pathfinding = GetComponent<Pathfinding>();
    }

    // 길찾기 요청
    public static void RequestPath(GameObject requestObj, Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback)
    {
        // 새로운 길찾기 요청을 큐에 넣고 길찾기 시도
        PathRequest newRequest = new PathRequest(requestObj, pathStart, pathEnd, callback);
        instance.pathRequestQueue.Enqueue(newRequest);
        instance.TryProcessNext();
    }

    // 다음 길찾기 시도
    void TryProcessNext()
    {
        // 길찾기 중이 아니고, 큐에 남은 요청이 1 이상이라면
        if (!isProcessingPath && pathRequestQueue.Count > 0)
        {
            // 큐의 요청 가져오기
            currentPathRequest = pathRequestQueue.Dequeue();

            // 길찾기 요청한 오브젝트가 파괴되지 않았다면
            if (currentPathRequest.requestObj != null)
            {
                // 길찾기 진행
                isProcessingPath = true;
                pathfinding.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd);
            }
        }
    }

    public void FinishedProcessingPath(Vector3[] path, bool sucess)
    {
        // 길찾기를 요청한 오브젝트가 삭제되지 않았다면
        if (currentPathRequest.requestObj != null)
        {
            currentPathRequest.callback(path, sucess);
        }

        // 다음 길찾기 시도
        isProcessingPath = false;
        TryProcessNext();
    }

    struct PathRequest
    {
        public GameObject requestObj;
        public Vector3 pathStart;
        public Vector3 pathEnd;
        public Action<Vector3[], bool> callback;

        public PathRequest(GameObject _requestObj, Vector3 _start, Vector3 _end, Action<Vector3[], bool> _callback)
        {
            requestObj = _requestObj;
            pathStart = _start;
            pathEnd = _end;
            callback = _callback;
        }
    }
}
