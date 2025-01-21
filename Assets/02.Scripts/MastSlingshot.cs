using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class MastSlingshot : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [Header("Scripts, Camera")]
    [SerializeField]private AnchorController anchorController;
    [SerializeField]private CameraController cameraController;
    public Manager manager;
    private Camera mainCamera;
    
    
    [Header("Slingshot Settings")]
    [SerializeField] private GameObject parrotPrefab;
    [SerializeField] private Transform slingshotLeft, slingshotRight, slingshotCenter; //새총의 중심 및 좌우 앵커 포인트
    [SerializeField] private float maxPullDistance; //새총 최대 당김 거리
    
    [Header("Sail Settings")]
    [SerializeField] private Transform sail;
    [SerializeField] private Transform leftEnd, rightEnd; //돛의 왼쪽, 오른쪽 끝단
    
    [Header("LineRenderer Settings")]
    [SerializeField] private LineRenderer leftLine, rightLine;
    [SerializeField] private LineRenderer trajectoryLine; // 궤적 표시용
    [SerializeField] private float timeStep = 0.1f;//궤적 계산 시간 간격
    [SerializeField] private int maxSteps = 20; //궤적 계산 최대 단계
    
    [Header("Slingshot State")]
    private Vector3 startPos; //새총 초기 위치
    private Vector3 pullPos; //새총 당긴 위치
    private bool isPulled = false;

    private void Awake()
    {
        mainCamera = Camera.main;
        
        //고무줄의 초기 위치 0과 1을 잡아줌
        leftLine.SetPosition(0, slingshotLeft.position);
        rightLine.SetPosition(0, slingshotRight.position);
        ResetSlingshot(); //새총의 위치 초기화
    }

    private void Update()
    {
        leftLine.SetPosition(0, slingshotLeft.position);
        rightLine.SetPosition(0, slingshotRight.position);
        leftLine.SetPosition(1, leftEnd.position);
        rightLine.SetPosition(1, rightEnd.position);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!anchorController.isAnchorDown) return;
        isPulled = true;
        startPos = slingshotCenter.position; //새총의 초기 위치 저장
        manager.PlaySfx(Manager.Sfx.RubberBand);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isPulled) return;

        cameraController.isDrag = true;
        //마우스 위치를 기준으로 새총 당김 위치 계산
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y,
            mainCamera.WorldToScreenPoint(startPos).z));
        pullPos = new Vector3(mouseWorldPos.x, mouseWorldPos.y, pullPos.z);

        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollInput != 0)
        {
            pullPos.z += scrollInput * 5f;
            pullPos.z = Mathf.Clamp(pullPos.z, -maxPullDistance, maxPullDistance);
        }

        pullPos.z = Mathf.MoveTowards(pullPos.z, pullPos.z, 10f * Time.deltaTime);

        //당김거리제한
        Vector3 pullDirection = startPos - pullPos;
        if (pullDirection.magnitude > maxPullDistance)
        {
            pullPos = startPos - pullDirection.normalized * maxPullDistance;
        }

        UpdateCameraPosition(pullDirection.magnitude);
       
        //새총 위치와 줄 업데이트
        sail.position = pullPos;
        leftLine.SetPosition(1, leftEnd.position);
        rightLine.SetPosition(1, rightEnd.position);
        
        //궤적 계산
        UpdateTrajectory(pullDirection * pullDirection.magnitude * 5f); 
        
    }
   
    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isPulled) return;

        cameraController.isDrag = false;
        isPulled = false;
        trajectoryLine.positionCount = 0; //궤적 초기화
        
        //앵무새 생성 및 발사
        GameObject parrot = Instantiate(parrotPrefab, slingshotCenter.position, Quaternion.identity);
        Rigidbody rigid = parrot.GetComponent<Rigidbody>();
        Animator anim = parrot.GetComponent<Animator>();
        Vector3 forceDirection = startPos - pullPos;
        if (rigid != null)
        {
            rigid.isKinematic = false;
            rigid.AddForce(forceDirection * forceDirection.magnitude * 5f, ForceMode.Impulse);
            anim.Play("Flying");
        }
        cameraController.FollowParrot(parrot.transform);
        manager.PlaySfx(Manager.Sfx.Parrot);

        //새총을 초기화
        StartCoroutine(AfterDrag());
    }

    private void UpdateCameraPosition(float pullMagnitude)
    {
        float baseZ = -6f;
        float maxZOffset = -10f;

        float targetZ = Mathf.Lerp(baseZ, maxZOffset, pullMagnitude / maxPullDistance);

        Vector3 cameraPos = mainCamera.transform.position;
        cameraPos.z = targetZ;
        mainCamera.transform.position = cameraPos;
    }
    
    IEnumerator AfterDrag()
    {
        //새총을 원래 위치로 부드럽게 복귀
        float lerpSpeed = 20f;
        while (Vector3.Distance(sail.position, startPos) > 0.01f)
        {
            sail.position = Vector3.Lerp(sail.position, startPos, lerpSpeed * Time.deltaTime);
            leftLine.SetPosition(1, leftEnd.position);
            rightLine.SetPosition(1, rightEnd.position);

            yield return null;
        }
        
        ResetSlingshot();
    }

    void UpdateTrajectory(Vector3 force)
    {
        //발사 궤적 시각화
        List<Vector3> trajectoryPoints = CalculateTrajectory(force);
        trajectoryLine.positionCount = trajectoryPoints.Count;

        for (int i = 0; i < trajectoryPoints.Count; i++)
        {
            trajectoryLine.SetPosition(i, trajectoryPoints[i]);
        }
    }

    List<Vector3> CalculateTrajectory(Vector3 force) //예상궤적을 계산
    {
        List<Vector3> trajectory = new List<Vector3>();
        Vector3 position = slingshotCenter.position;
        Vector3 velocity = force / parrotPrefab.GetComponent<Rigidbody>().mass;

        for (int i = 0; i < maxSteps; i++)
        {
            float time = i * timeStep;
            Vector3 point = position + velocity * time + Physics.gravity * (0.5f * time * time);
            trajectory.Add(point);
            if (CheckCollision(position, point, out Vector3 hitPoint)) //충돌지점에서 궤적을 중지
            {
                trajectory.Add(hitPoint);
                break;
            }

            position = point;
        }

        return trajectory;
    }

    private bool CheckCollision(Vector3 start, Vector3 end, out Vector3 hitPoint) //궤적 점 사이에서 충돌이 발생하는지 확인, 충돌하지 않으면 다음계산 반복
    {
        hitPoint = end;
        Vector3 direction = end - start;
        float distance = direction.magnitude;

        if (Physics.Raycast(start, direction.normalized, out RaycastHit hit, distance))
        {
            hitPoint = hit.point;
            return true;
        }

        return false;
    }
    
    private void ResetSlingshot() //돛과 고무줄의 위치를 초기위치로 설정
    {
        sail.position = slingshotCenter.position;
        leftLine.SetPosition(0, slingshotLeft.position);
        rightLine.SetPosition(0, slingshotRight.position);
        leftLine.SetPosition(1, leftEnd.position);
        rightLine.SetPosition(1, rightEnd.position);
        pullPos.z = 0;
    }

    
}
