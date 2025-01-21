using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class SpawnManager : MonoBehaviour
{
    public GameObject[] enemyPrefabs;
    public BoxCollider island;
    public int enemyCount = 5;
    public TextMeshProUGUI enemyCountText;
    public Image parrotImage;
    public Sprite[] parrotCatains;
    public Manager manger;
    
    
    private int remainEnemies = 0;
    private string[] parrotCatainWords =
    {
        "Arrr! {count} landlubbers left! Time to send 'em to Davy Jones' locker!",
        "{count} still kickin'? Let's turn 'em into chum!",
        "Squawk! {count} left! Blast 'em to smithereens, cap'n!",
        "Yarr! Only {count} left! They won't know what hit 'em!",
        "Aye aye, cap'n! {count} varmints await their doom! Fly into battle!"
    };

    private void Start()
    {
        UpdateEnemyUI();
    }

    public void SpawnEnemies() //UI 버튼과 연결
    {
        for (int i = 0; i < enemyCount; i++)
        {
            GameObject randomEnemy = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            
            Vector3 spawnPosition = new Vector3(
                Random.Range(island.bounds.min.x, island.bounds.max.x),
                island.bounds.max.y + 10f, 
                Random.Range(island.bounds.min.z, island.bounds.max.z));

            Quaternion randomRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
            
            
            GameObject spawnedEnemy = Instantiate(randomEnemy, spawnPosition, randomRotation);
            EnemyController enemyController = spawnedEnemy.GetComponent<EnemyController>();
            enemyController.OnEnemyDeactivated += HandleEnemyDeactivated;

            remainEnemies++;
        }
        manger.PlaySfx(Manager.Sfx.Dog);
        manger.PlaySfx(Manager.Sfx.Cow);
        manger.PlaySfx(Manager.Sfx.Elephant);
        manger.PlaySfx(Manager.Sfx.Horse);
        UpdateEnemyUI();
    }

    private void HandleEnemyDeactivated()
    {
        remainEnemies--;
        UpdateEnemyUI();
        if (remainEnemies <= 0)
        {
            manger.VictoryUI.SetActive(true);
        }
    }

    private void UpdateEnemyUI()
    {
        enemyCountText.text = parrotCatainWords[(Random.Range(0, parrotCatainWords.Length))]
            .Replace("{count}", remainEnemies.ToString());

        parrotImage.sprite = parrotCatains[Random.Range(0, parrotCatains.Length)];
    }
    
}
