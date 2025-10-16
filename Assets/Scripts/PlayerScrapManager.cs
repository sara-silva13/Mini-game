using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScrapManager : MonoBehaviour
{
    [SerializeField] ScrapDropper scrapDropper;
    [SerializeField] GameObject scrapPreFab;

    private Movement playerMoveInfo;

    private KeyCode scrapInput;

    private bool canPickUp = false;
    private Transform pickUpObjTrans = null;
    private Transform pickUpObj2Trans = null;
    private Transform holdPickUpObj = null;
    private int scrapQty = 0;
    private readonly int scrapMaxQty = 2;

    private float throwForce = 15f;

    private void OnTriggerEnter(Collider other)
    {
        if (scrapQty < scrapMaxQty && other.gameObject.name.ToLower().Contains("pickup")) //!!! Do: Pick-Up only if looking in the direction of the obj being picked
        {
            holdPickUpObj = other.transform.parent.transform;

            canPickUp = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (canPickUp && other.gameObject.name.ToLower().Contains("pickup"))
        {
            holdPickUpObj = null;

            canPickUp = false;
        }
    }

    private void Start()
    {
        playerMoveInfo = GetComponent<Movement>();

        if (playerMoveInfo.playerNumber == 1)
            scrapInput = KeyCode.LeftShift;
        else
            scrapInput = KeyCode.RightShift;
    }

    private void Update()
    {
        if (Input.GetKeyDown(scrapInput))
        {
            if (canPickUp)
                PickUp();
            else if (scrapQty > 0)
                Throw();
        }
    }

    private void PickUp()
    {
        // Direction from player to pickup
        Vector3 toObject = (holdPickUpObj.transform.position - transform.position).normalized;
        float facingDot = Vector3.Dot(transform.forward, toObject);

        // Require player to be looking roughly toward the object (e.g. within ~60°)
        if (facingDot <= 0.5f)
            return;

        scrapDropper.SpawnAndDrop(holdPickUpObj.transform.position);

        Destroy(holdPickUpObj.gameObject);

        foreach (Transform child in transform)
        {
            if (child.name.ToLower().Contains("picklocations"))
            {
                Transform grandChild = child.GetChild(0);
                if (grandChild.childCount != 0)
                    grandChild = child.GetChild(1);

                if (pickUpObjTrans == null)
                {
                    pickUpObjTrans = Instantiate(scrapPreFab, grandChild).transform;
                    BoxCollider boxCollider = pickUpObjTrans.GetComponentInChildren<BoxCollider>();
                    boxCollider.enabled = false;
                }
                else
                {
                    pickUpObj2Trans = Instantiate(scrapPreFab, grandChild).transform;
                    BoxCollider boxCollider = pickUpObj2Trans.GetComponentInChildren<BoxCollider>();
                    boxCollider.enabled = false;
                }



                break;
            }
        }

        scrapQty++;

        //reset vars
        canPickUp = false;
    }

    private void Throw()
    {
        Transform objToThrow = null;

        if (pickUpObj2Trans != null)
            objToThrow = pickUpObj2Trans;
        else
            objToThrow = pickUpObjTrans;

        objToThrow.GetComponent<SphereCollider>().enabled = true;

        bool objOnRight = objToThrow.parent.localPosition.x > 0f;
        int side = objOnRight ? -1 : 1;
        float angle = 10f * side;

        objToThrow.SetParent(null);
        Rigidbody objRb = objToThrow.GetComponent<Rigidbody>();
        objRb.isKinematic = false;
        //objRb.AddForce(transform.forward * (throwForce + playerMoveInfo.moveSpeed), ForceMode.VelocityChange); 

        Vector3 throwDirection = Quaternion.AngleAxis(angle, Vector3.up) * transform.forward;
        objRb.AddForce(throwDirection * throwForce, ForceMode.VelocityChange);


        if (pickUpObj2Trans == objToThrow)
            pickUpObj2Trans = null;
        else
            pickUpObjTrans = null;

        scrapQty--;
    }
}
