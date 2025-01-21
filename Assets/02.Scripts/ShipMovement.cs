using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipMovement : MonoBehaviour
{
    public float moveSpeed = 15f;
    public float rotateSpeed = 30f; //돛대의 회전 속도
    
    public GameObject mast;
    public AnchorController anchorController;
    public CameraController cameraController;
    
    private void Update()
    {
        if (cameraController.isDrag) return;
        
        //돛대 회전 로직
        if (Input.GetKey(KeyCode.Q)) //돛대를 왼쪽으로 회전
        {
            mast.transform.Rotate(Vector3.right, -rotateSpeed * Time.deltaTime );
            cameraController.UpdateCameraDirection(-mast.transform.forward);
        }
        if (Input.GetKey(KeyCode.E)) //돛대를 오른쪽으로 회전
        {
            mast.transform.Rotate(Vector3.right, rotateSpeed * Time.deltaTime );
            cameraController.UpdateCameraDirection(-mast.transform.forward);
        }


        //배 이동로직
        if (anchorController.isAnchorDown) return; //닻이 내려져 있으면 이동 불가
        float h = Input.GetAxis("Horizontal");

        Vector3 moveValue = transform.right * h; 
        transform.position += moveValue * (moveSpeed * Time.deltaTime);

        if (Input.GetKey(KeyCode.Space))
        {
            anchorController.AnchorControl();
        }
        
        
    }
    
}
