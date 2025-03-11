using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ControlLockedPieceVFX : MonoBehaviour
{
    public static ControlLockedPieceVFX Instance;
    [SerializeField] private AudioSource source;
    [SerializeField] GameObject vfxGameObject;

    ControlLockedPieceVFX()
    {
        Instance = this;
    }
    
    public void TriggerLockedPieceVFX(GameObject piece)
    {
        source = GetComponent<AudioSource>();
        transform.position = piece.transform.position;
        vfxGameObject.SetActive (true);
        source.Play();
        Handheld.Vibrate();
        StartCoroutine(WaitForSound());
    }
    
    IEnumerator WaitForSound()
    {
        //Wait Until Sound has finished playing
        while (source.isPlaying)
        {
            yield return null;
        }

        //Auidio has finished playing, disable GameObject
        vfxGameObject.SetActive(false);
    }
}
