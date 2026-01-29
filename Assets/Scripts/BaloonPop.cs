using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaloonPop : MonoBehaviour
{
    public GameObject popEffect;
    public GameObject BaloonMesh;
    public AudioClip popSound;   // Optional: assign a pop sound
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>(); 
    }

    private void OnMouseDown()
    {
        PopBalloon();
    }

    void PopBalloon()
    {
        if (popEffect != null)
            Instantiate(popEffect, transform.position, Quaternion.identity);

        if (audioSource != null && popSound != null)
            audioSource.PlayOneShot(popSound);

        // Destroy(gameObject); // Destroy the balloon
        BaloonMesh.SetActive(false);
        // FIXED: Removed free gold from balloons - premium currency should be premium-only
        // Gold should only come from: IAP purchases, rewarded ads, or premium features
        // This maintains premium currency exclusivity per GDD requirements
        // int RandomNumber = Random.Range(3,10);
        // GameManager.gameManagerInstance.AddGold(RandomNumber);
        // UIManager.Instance.UpdateUI(UIUpdateType.Gold);
        
        // TODO: Add clear reward explanation or remove balloon mechanic entirely
        // Option: Show info popup explaining balloon purpose, or make it give soft currency instead
        GetComponent<BoxCollider>().enabled = false;
        Invoke("ResetBaloon",10);
    }

    public void ResetBaloon()
    {
        BaloonMesh.SetActive(true);
        GetComponent<BoxCollider>().enabled = true;
    }

}
