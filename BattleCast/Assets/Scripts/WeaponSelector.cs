using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;




public class WeaponSelector : NetworkBehaviour
{

    string weaponCollide;
    PlayerMovement2D player = null;
    public GameObject weap = null;
    bool rightPos = false;
    bool playerFlipSave = true;
    NetworkVariable<float> pos;
    Vector3 mouse_position;
    float angle;
    float mouse_distance;
    float angle_souris;
    // Start is called before the first frame update
    void Start()
    {
        player = gameObject.GetComponent<PlayerMovement2D>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (IsServer)
        {
            UpdateServer();
        }
        if (IsClient)
        {
            UpdateClient();
        }

    }


    private void UpdateServer()
    {
        if (weaponCollide != null)
        {
            Debug.Log("WeaponCollide != null");
            weap = GameObject.Find(weaponCollide);
            weap.transform.SetParent(this.transform, this.transform);
            weap.GetComponent<BoxCollider2D>().enabled = false;

            weaponCollide = null;
        }
        if (weap != null && player.flip.Value != playerFlipSave)
        {
            weap.GetComponent<SpriteRenderer>().flipX = playerFlipSave;
            playerFlipSave = player.flip.Value;

            float direction = player.flip.Value ? 1 : -1;
            weap.transform.localPosition = new Vector2(direction * 0.2f, -0.25f);
        }
        if (weap != null && pos != null) {
            
            weap.transform.rotation = Quaternion.Euler(new Vector3(0F, 0F, pos.Value));

        }
    }

    private void UpdateClient()
    {
        mouse_position = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float direction = player.flip.Value ? 1 : -1;
        angle_souris = Mathf.Atan2(direction*mouse_position.y,direction* mouse_position.x) * Mathf.Rad2Deg;
        UpdateClientWeaponServerRPC(angle_souris);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Weapon")
        {
            weaponCollide = collision.gameObject.name;
        }
        
    }

    [ServerRpc]
    public void UpdateClientWeaponServerRPC(float newMouseAngle)
    {
        if (weap != null)
        {
            pos.Value = newMouseAngle;
            Debug.Log(pos.Value + " 1");
            Debug.Log(newMouseAngle + " 2");
        }
    } 
}


