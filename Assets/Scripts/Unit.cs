using System.Collections;
using System.Net.NetworkInformation;
using JetBrains.Annotations;
using Unity.Burst.Intrinsics;
using Unity.Mathematics;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float moveSpeed;

    private Pathh path;
    public float turnDist;
    
    private void Update()
    {
        PathRequestManager.RequestPath(this.transform.position, target.position, OnPathFound);
    }

    private void LookToward(Vector3 position)
    {
        Vector3 idk = position - transform.position;
        float deg = Mathf.Atan2(idk.x, idk.z) * Mathf.Rad2Deg;
        
        transform.rotation = Quaternion.Euler(-90, 0, deg);
    }

    private IEnumerator hihi()
    {
        while(true)
        {
            PathRequestManager.RequestPath(this.transform.position, target.position, OnPathFound);
            yield return new WaitForSeconds(0.2f);
        }
    }

    private void OnPathFound(Vector3[] wpoints, bool success)
    {
        if (!success || wpoints.Length == 0) return;
        
        path = new Pathh(wpoints, transform.position, turnDist);

        StopCoroutine("FollowPath");
        StartCoroutine("FollowPath");
    }

    private IEnumerator FollowPath()
    {
        while (true)
        {
            yield return null;
        }
    }

    private void OnDrawGizmos()
    {
        if (path != null) path.DrawWithGizmos();
    }

}
