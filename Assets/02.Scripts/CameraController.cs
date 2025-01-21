using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform shipTarget; //배의 포지션
    private Transform currentTarget; //현재 카메라 타겟
    public Vector3 offset = new Vector3(0, 4.33f, -6f); //초기 카메라 오프셋
    public float speed = 5f;

    private bool isFollow = false; //중복 새 날리기 제어
    public bool isDrag = false; //드래그 중일때는 카메라는 MastSlingshot에서만 z축제어
    private Vector3 lookDirection; //바라보는 방향

    private void Start()
    {
        currentTarget = shipTarget;
        lookDirection = -shipTarget.right; //배의 pivot에서 카메라의 정면이 왼쪽
    }

    private void LateUpdate()
    {
        if (isDrag) return;
        Vector3 targetPos = currentTarget.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPos, speed * Time.deltaTime);

        Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, speed * Time.deltaTime);
    }

    public void UpdateCameraDirection(Vector3 newDirection)
    {
        lookDirection = newDirection;
    }
    
    public void FollowParrot(Transform parrotTransform)
    {
        if (isFollow) return;
        StartCoroutine(CameraFollowParrot(parrotTransform));
    }

    IEnumerator CameraFollowParrot(Transform parrotTrasnform)
    {
        isFollow = true;
        currentTarget = parrotTrasnform;

        yield return new WaitForSeconds(3f);

        currentTarget = shipTarget;
        isFollow = false;
    }
}
