using System;
using System.Collections;
using System.Net.NetworkInformation;
using JetBrains.Annotations;
using Unity.Burst.Intrinsics;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float turnDist;
    [SerializeField] private float turnSpeed;
    [SerializeField] private float distanceThreshold;
    [SerializeField] private float searchCoolDown;
    [SerializeField] private float stoppingDist;

    private Pathh path;


    private void Start()
    {
        StartCoroutine(hihi());
    }

    private void LookToward(Vector3 position)
    {
        Vector3 idk = position - transform.position;
        float deg = Mathf.Atan2(idk.x, idk.z) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, deg);
    }

    private IEnumerator hihi()
    {
        PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
        Vector3 oldPosition = target.position;

        while(true)
        {
            yield return new WaitForSeconds(searchCoolDown);

            if((oldPosition -  target.position).sqrMagnitude > distanceThreshold * distanceThreshold)
            {
                PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
                oldPosition = target.position;
            }
        }
    }

    private void OnPathFound(Vector3[] wpoints, bool success)
    {
        if (!success || wpoints.Length == 0) return;
        
        path = new Pathh(wpoints, transform.position, turnDist, stoppingDist);

        StopCoroutine("FollowPath");
        StartCoroutine("FollowPath");
    }

    private IEnumerator FollowPath()
    {
        int index = 0;
        bool followingPath = true;

        while (followingPath)
        {
            while (path.turnBoundaries[index].HasCrossedLine(transform.position))
            {
                index++;
                if(index == path.Length)
                {
                    followingPath = false;
                    break;
                }
            }
            
            float speedPercent = 1;
            if(index >= path.slowDownIndex && stoppingDist > 0)
            {
                speedPercent = Mathf.Clamp01(Vector2.Distance(transform.position, target.position) / stoppingDist);
                if (speedPercent < 0.01) followingPath = false;
            }

            if (followingPath)
            {

                Quaternion rot = Quaternion.LookRotation(path.lookPoints[index] - transform.position);

                transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * turnSpeed);
                transform.Translate(moveSpeed * speedPercent * Time.deltaTime * Vector3.forward, Space.Self);
            }

            yield return null;
        }
    }

    private void OnDrawGizmos()
    {
        if (path != null) {
            path.DrawWithGizmos();
        }
    }

}
