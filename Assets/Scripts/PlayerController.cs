using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour {
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public GameObject playerCamera;

    private bool triggeringAnotherPlayer;
    private GameObject otherPlayer;

    void Start()
    {
        if (isLocalPlayer == true)
        {
            playerCamera.SetActive(true);
        }
        else
        {
            playerCamera.SetActive(false);
        }
    }

    // Update is called once per frame
    [ClientCallback]
    void Update()
    {
        //If player is not local return
        if (!isLocalPlayer)
        {
            return;
        }

        var x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;
        var z = Input.GetAxis("Vertical") * Time.deltaTime * 3.0f;

        transform.Rotate(0, x, 0);
        transform.Translate(0, 0, z);

        if (Input.GetMouseButtonDown(0))
        {
            CmdFire();
        }

        else if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            CmdJump();
        }

        //Checking if player is local 2nd time
        if (isLocalPlayer == true)
        {
            if (Input.GetKeyDown(KeyCode.D))
            {
                this.transform.Translate(Vector3.right * Time.deltaTime * 3f);
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                this.transform.Translate(Vector3.up * Time.deltaTime * 30f);
            }

            if (triggeringAnotherPlayer && Input.GetKeyDown(KeyCode.E))
            {
                print(this.gameObject.name + " is colliding with " + otherPlayer.name);
                Destroy(otherPlayer);

            }

        }

    }


    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            triggeringAnotherPlayer = true;
            otherPlayer = other.gameObject;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            triggeringAnotherPlayer = false;
            otherPlayer = null;
        }
    }


    [Command]
    public void CmdJump ()
    {


    }

    [Command]
    void CmdFire()
    {
        //Create the Bullet from the Bullet Prefab
        var bullet = (GameObject)Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);

        //Add velocity to the bullet
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 10;

        //Spawn bullet on Clients
        NetworkServer.Spawn(bullet);

        //Destroy the bullet after 2 seconds
        Destroy(bullet, 2.0f);

    }

    public override void OnStartLocalPlayer()
    {
        GetComponent<MeshRenderer>().material.color = Color.blue;
    }






}
