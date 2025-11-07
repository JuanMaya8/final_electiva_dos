using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public Transform playerCamera;
    // Alcance del disparo
    public float shotDistance = 10f;
    public float impactForce = 5f;
    public LayerMask shotMask;
    public GameObject destroyEffect;
    public ParticleSystem shootParticles;
    public GameObject hitEffect;
    [Header("Audio")]
    public AudioClip hitSound; // Sonido al impactar
    public AudioSource audioSource; // Fuente de audio

    private RaycastHit showRaycastHit;

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        shootParticles.Play();
        //Para lanzar el raycast tomamos la posicion de la camara, apuntando la direccion que mira la camara
        if (Physics.Raycast(playerCamera.position, playerCamera.forward, out showRaycastHit, shotDistance, shotMask))
        {
            Debug.Log("Shot hit: " + showRaycastHit.collider.name);
            Instantiate(hitEffect, showRaycastHit.point, Quaternion.LookRotation(showRaycastHit.normal), showRaycastHit.transform);

            if (showRaycastHit.collider.GetComponent<Rigidbody>() != null)
            {
                showRaycastHit.collider.GetComponent<Rigidbody>().AddForce(-showRaycastHit.normal * impactForce);
            }

            // Reproducir sonido de impacto
            if (hitSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(hitSound);
            }

            if (showRaycastHit.collider.CompareTag("Barrel"))
            {
                //LevelManager.instance.levelScore++;
                Instantiate(destroyEffect, showRaycastHit.point, Quaternion.LookRotation(showRaycastHit.normal));
                Destroy(showRaycastHit.collider.gameObject);
            }
        }
    }
}