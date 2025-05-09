using System.Collections;
using System.Net.NetworkInformation;
using JetBrains.Annotations;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float moveSpeed;

    private int PathIndex = 0;
    private Vector3[] path;
    private void Start()
    {
        StartCoroutine(hihi());
    }

    private IEnumerator hihi()
    {
        while(true)
        {
            PathRequestManager.RequestPath(this.transform.position, target.position, OnPathFound);
            yield return new WaitForSeconds(0.2f);
        }
    }

    private void OnPathFound(Vector3[] path, bool success)
    {
        if (!success || path.Length == 0) return;
        
        this.path = path;
        StopCoroutine("FollowPath");
        StartCoroutine("FollowPath");
    }

    private IEnumerator FollowPath()
    {
        PathIndex = 0;
        while (true)
        {
            if(transform.position == path[PathIndex])
            {
                PathIndex++;
                if (PathIndex >= path.Length) break;
            }
            transform.position = Vector3.MoveTowards(transform.position, path[PathIndex], Time.deltaTime * moveSpeed);
            yield return null;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        if (path == null) return;
        for(int i = PathIndex; i < path.Length; i++)
        {
            if(i == PathIndex)
            {
                Gizmos.DrawLine(transform.position, path[i]);
            }
            else
            {
                Gizmos.DrawLine(path[i - 1], path[i]);
            }
            Gizmos.DrawCube(path[i], Vector3.one * 0.2f);
        }
    }

}
