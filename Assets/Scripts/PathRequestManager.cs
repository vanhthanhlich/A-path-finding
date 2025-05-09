using UnityEngine;
using System.Collections.Generic;
using System;

public class PathRequestManager : MonoBehaviour
{
    
    private PathFinding pathFinding;
    private Queue<PathRequest> pathRequestsQueue = new Queue<PathRequest>();
    private static PathRequestManager instance;

    private PathRequest currentPathRequest;
    private bool isProcessingPath = false;

    private void Awake()
    {
        pathFinding = GetComponent<PathFinding>();
        instance = this;
    }

    public static void RequestPath(Vector3 startPos, Vector3 targetPos, Action<Vector3[], bool> callback)
    {
        PathRequest newPathRequest = new PathRequest(startPos, targetPos, callback);
        instance.pathRequestsQueue.Enqueue(newPathRequest);
        instance.TryProcessNext();    
    }

    private void TryProcessNext()
    {
        if(!isProcessingPath && pathRequestsQueue.Count > 0)
        {
            currentPathRequest = pathRequestsQueue.Dequeue();

            isProcessingPath = true;
            pathFinding.StartFindPath(currentPathRequest.startPos, currentPathRequest.targetPos);
        }
    }

    public void FinishProcessPath(Vector3[] path, bool success)
    {
        isProcessingPath = false;
        currentPathRequest.callback(path, success);
        
        TryProcessNext();
    }

    struct PathRequest
    {
        public Vector3 startPos;
        public Vector3 targetPos;
        public Action<Vector3[], bool> callback;

        public PathRequest(Vector3 startPos, Vector3 targetPos, Action<Vector3[], bool> cb)
        {
            this.startPos = startPos;
            this.targetPos = targetPos;
            this.callback = cb;
        }

    }

}
