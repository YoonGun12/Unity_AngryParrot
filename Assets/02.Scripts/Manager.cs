using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour
{
     public GameObject TitleUI;
       public GameObject GamePlayUI;
       public GameObject VictoryUI;
       
       [Header("BGM")]
       private AudioSource bgmPlayer;
       public AudioClip bgm;
    
       [Header("SFX")] 
       public AudioClip[] sfxClips;
       private AudioSource[] SFXPlayers;
       public int channels;
       private int channelIndex;
       private float sfxVolume = 0.5f;
    
       public enum Sfx{AnchorSFX, ExplosionSFX, Parrot, Dog, Elephant, Cow, Horse, StartBtn, RubberBand};
    
       private void Start()
       {
          GamePlayUI.SetActive(false);
          TitleUI.SetActive(true);
          bgmPlayer = GetComponent<AudioSource>();
          bgmPlayer.clip = bgm;
          bgmPlayer.Play();
          sfxInit();
    
       }
    
       public void GameStart()
       {
          TitleUI.SetActive(false);
          GamePlayUI.SetActive(true);
          PlaySfx(Sfx.StartBtn);
          
       }
    
       void sfxInit()
       {
          GameObject sfxObject = new GameObject("SFXPlayer");
          sfxObject.transform.parent = transform;
          SFXPlayers = new AudioSource[channels];
    
          for (int i = 0; i < SFXPlayers.Length; i++)
          {
             SFXPlayers[i] = sfxObject.AddComponent<AudioSource>();
             SFXPlayers[i].volume = sfxVolume;
             SFXPlayers[i].playOnAwake = false;
          }
       }
    
       public void PlaySfx(Sfx sfx)
       {
          for (int i = 0; i < SFXPlayers.Length; i++)
          {
             int loopIndex = (i + channelIndex) % SFXPlayers.Length;
             
             if(SFXPlayers[loopIndex].isPlaying) continue;
    
             channelIndex = loopIndex;
             SFXPlayers[loopIndex].clip = sfxClips[(int)sfx];
             SFXPlayers[loopIndex].Play();
             break;
          }
       }

       public void Retry()
       {
          SceneManager.LoadScene(0);
       }
}
