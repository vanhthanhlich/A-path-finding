using System;
using System.Collections;
using System.ComponentModel;
using System.Net.NetworkInformation;
using JetBrains.Annotations;
using Unity.Burst.Intrinsics;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UIElements;

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
    public Vector3 velocity;

    private void Awake()
    {
        transform.rotation = Quaternion.Euler(-90, 0, 0);
        velocity = -transform.up;
    }

    private void Start()
    {
        StartCoroutine(hihi());
    }

    private void LookToward(Vector3 position)
    {
        Vector3 dir = (position - transform.position).normalized;
        velocity = Vector3.Lerp(velocity, dir, Time.deltaTime * turnSpeed);
        
        float deg = Mathf.Atan2(velocity.x, velocity.z) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(-90, 0, deg);
    }

    void DrawRay(Vector3 position, Vector3 direction)
    {
        Gizmos.color = Color.red;
        Debug.DrawRay(position, direction);
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
                LookToward(path.lookPoints[index]);        
                transform.position += (moveSpeed * speedPercent * Time.deltaTime * velocity);
            }

            yield return null;
        }
    }

    private void OnDrawGizmos()
    {
        //Vector3 dir = (target.position - transform.position).normalized;
        //DrawRay(transform.position, dir);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + 10 * velocity);

        if (path != null) {
            path.DrawWithGizmos();
        }
    }

}
