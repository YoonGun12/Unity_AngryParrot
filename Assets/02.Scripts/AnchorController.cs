using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using TMPro;

public class AnchorController : MonoBehaviour
{
    public Transform anchor;
    public float anchorDropSpeed = 3f; //닻 이동속도
    public float anchorDropDistance = 5f; //닻 이동거리

    public bool isAnchorDown = false;  
    private Vector3 initialAnchorPos; //닻의 초기 위치
    private bool isDrop = false; //닻이 이동중인지

    [SerializeField] private TextMeshProUGUI anchorStateText;
    public Manager manager;
    

    private void Start()
    {
        initialAnchorPos = anchor.localPosition;
        UpdateAnchorText();
    }

    public void AnchorControl() //UI 닻 버튼과 연결
    {
        if (isDrop) return;
        isAnchorDown = !isAnchorDown;
        UpdateAnchorText();
        StartCoroutine(MoveAnchor());
        manager.PlaySfx(Manager.Sfx.AnchorSFX);
        
    }

    IEnumerator MoveAnchor()
    {
        isDrop = true;

        Vector3 targetPos; //닻이 향하는 위치 (초기위치, 내려간 위치)
        if (isAnchorDown) 
        {
            targetPos = initialAnchorPos + Vector3.down * anchorDropDistance; 
        }
        else //닻이 바다 아래 있다면 목표 위치는 닻의 초기 위치
        {
            targetPos = initialAnchorPos;
        }

        //닻의 이동 로직
        while (Vector3.Distance(anchor.localPosition, targetPos) > 0.01f)
        {
            anchor.localPosition = Vector3.Lerp(anchor.localPosition, targetPos, anchorDropSpeed * Time.deltaTime);
            yield return null;
        }

        anchor.localPosition = targetPos;
        isDrop = false;

    }

    private void UpdateAnchorText()
    {
        anchorStateText.text = isAnchorDown ? "<color=#FF0000><size=150%>Raise</size></color>\nthe\nAnchor!" : "<color=#00FF00><size=150%>Drop</size></color>\nthe\nAnchor!";
    }
}
