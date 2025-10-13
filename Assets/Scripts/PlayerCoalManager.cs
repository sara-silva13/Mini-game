using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCoalManager : MonoBehaviour
{
    private Movement playerMoveInfo;

    private bool canPickUp = false;
    private Transform pickUpObjTrans = null;
    private bool hasCoal = false;

    private float throwForce = 5f;

    private void OnTriggerEnter(Collider other)
    {
        if (!hasCoal && other.gameObject.name.ToLower().Contains("pickup")) //!!! Do: Pick-Up only if looking in the direction of the obj being picked
        {
            pickUpObjTrans = other.transform.parent.transform;

            canPickUp = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (canPickUp && other.gameObject.name.ToLower().Contains("pickup"))
        {
            pickUpObjTrans = null;
            canPickUp = false;
        }
    }

    private void Start()
    {
        playerMoveInfo = GetComponent<Movement>();
    }

    private void Update()
    {
        bool coalInput = (playerMoveInfo.playerNumber == 1 && Input.GetKeyDown(KeyCode.Space)) ||
                         (playerMoveInfo.playerNumber == 2 && Input.GetKeyDown(KeyCode.KeypadPlus));
        
        if (coalInput)
        {
            if (hasCoal)
                Throw();
            else if (canPickUp)
                PickUp();
        }
    }

    private void PickUp()
    {
        //Destroy Pick-Up Area From Child
        foreach (Transform child in pickUpObjTrans)
        {
            if (child.name.ToLower().Contains("area"))
            {
                child.GetComponent<BoxCollider>().isTrigger = false;
                Destroy(child.gameObject);
                break;
            }
        }

        //Make object picked follow player
        pickUpObjTrans.SetParent(transform);
        hasCoal = true;

        //reset vars
        canPickUp = false;
    }

    private void Throw()
    {
        pickUpObjTrans.GetComponent<SphereCollider>().enabled = true;

        pickUpObjTrans.SetParent(null);
        Rigidbody objRb = pickUpObjTrans.GetComponent<Rigidbody>();
        objRb.isKinematic = false;
        objRb.AddForce(transform.forward * (throwForce + playerMoveInfo.moveSpeed), ForceMode.VelocityChange); //!!!confirm if moveSpeed changes after hits

        pickUpObjTrans = null;
        hasCoal = false;
    }
}
