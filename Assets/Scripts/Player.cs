﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // config params
    [Header("Player")]
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float padding = 1f;
    [SerializeField] int health = 100;

    [Header("Player Projectile")]
    [SerializeField] GameObject laserPrefab;
    [SerializeField] float projectileSpeed = 10f;
    [SerializeField] float projectileFiringPeriod = 0.5f;

    [Header("Effects")]
    [SerializeField] GameObject explosionVFX;
    [SerializeField] float durationOfExplosion = 2f;
    [SerializeField] AudioClip playerDeathSFX;
    [Range(0, 1)] [SerializeField] float deathSoundVolume = 0.5f;
    [SerializeField] AudioClip laserSFX;
    [Range(0, 1)] [SerializeField] float laserVolume = 0.5f;

    Coroutine firingCoroutine;

    float xMin;
    float xMax;

    float yMin;
    float yMax;


    // Start is called before the first frame update
    void Start()
    {
        SetUpMoveBoundaries();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Fire();
    }

    private void Fire()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            firingCoroutine = StartCoroutine(FireContinuously());
        }
        if (Input.GetButtonUp("Fire1"))
        {
            StopCoroutine(firingCoroutine);
        }
    }

    IEnumerator FireContinuously()
    {
        while (true)
        {
        GameObject laser = Instantiate(
        laserPrefab, transform.position, Quaternion.identity
        ) as GameObject;
        laser.GetComponent<Rigidbody2D>().velocity = new Vector2(0, projectileSpeed);
        AudioSource.PlayClipAtPoint(laserSFX, Camera.main.transform.position, laserVolume);
        yield return new WaitForSeconds(projectileFiringPeriod);
        }
    }

    private void SetUpMoveBoundaries()
    {
        Camera gameCamera = Camera.main;
        xMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x + padding;
        xMax = gameCamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x - padding;

        yMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).y + padding;
        yMax = gameCamera.ViewportToWorldPoint(new Vector3(0, 1, 0)).y - padding;


    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        DamageDealer damageDealer = collision.gameObject.GetComponent<DamageDealer>();
        if (!damageDealer) { return; }
        ProcessHit(damageDealer);
    }

    private void ProcessHit(DamageDealer damageDealer)
    {
        health -= damageDealer.GetDamage();
        damageDealer.Hit();

        if (health <= 0)
        {
            playerDies();
        }
    }

    private void playerDies()
    {
        Destroy(gameObject);
        GameObject explosion = Instantiate(explosionVFX, transform.position, transform.rotation) as GameObject;
        Destroy(explosion, durationOfExplosion);
        AudioSource.PlayClipAtPoint(playerDeathSFX, Camera.main.transform.position, deathSoundVolume);
    }

    private void Move()
    {
        var deltaX = Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeed;
        var deltaY = Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed;

        var newXPos = Mathf.Clamp(transform.position.x + deltaX, xMin,xMax);
        var newYPos = Mathf.Clamp(transform.position.y + deltaY,yMin, yMax);

        transform.position = new Vector2(newXPos, newYPos);
    }
}