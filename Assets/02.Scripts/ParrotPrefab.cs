using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParrotPrefab : MonoBehaviour
{
    public GameObject explosionPrefab;
    private Animator anim;

    private Manager manager;
    private void Awake()
    {
        manager = FindObjectOfType<Manager>(); //public으로 바인딩하려하는데 안돼서;;;
        anim = GetComponent<Animator>();
    }


    private void OnCollisionEnter(Collision other)
    {
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        manager.PlaySfx(Manager.Sfx.ExplosionSFX);
        anim.SetBool("isDead", true);
    }
}
