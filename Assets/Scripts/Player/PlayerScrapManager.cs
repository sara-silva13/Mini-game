using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScrapManager : MonoBehaviour
{
    [SerializeField] ScrapDropper scrapDropper;
    [SerializeField] GameObject scrapPreFab;

    private Movement playerMoveInfo;

    private KeyCode scrapInput;

    private Animator anim;

    private bool canPickUp = false;
    private Transform pickUpObjTrans = null;
    private Transform pickUpObj2Trans = null;
    private Transform holdPickUpObj = null;
    private int scrapQty = 0;
    private readonly int scrapMaxQty = 2;

    private float throwForce = 17.5f;

    private void OnTriggerStay(Collider other)
    {
        if (scrapQty < scrapMaxQty && other.gameObject.name.ToLower().Contains("pickup"))
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
        anim = GetComponent<Animator>();
        anim.SetInteger("hands", scrapQty);

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
            {
                // Direction from player to pickup
                Vector3 toObject = (holdPickUpObj.transform.position - transform.position).normalized;
                float facingDot = Vector3.Dot(transform.forward, toObject);

                // Require player to be looking roughly toward the object (e.g. within ~60°)
                if (facingDot <= 0.5f)
                {
                    PickUp();
                    return;
                }

            }

            if (scrapQty > 0)
            {
                Throw();
                return;
            }
        }
    }

    private void PickUp()
    {
        scrapDropper.SpawnAndDrop(holdPickUpObj.transform.position);

        Destroy(holdPickUpObj.gameObject);
        holdPickUpObj = null;

        foreach (Transform child in transform)
        {
            if (child.name.ToLower().Contains("picklocations"))
            {
                Transform grandChild = child.GetChild(0);
                if (grandChild.childCount != 0)
                    grandChild = child.GetChild(1);

                Transform scrapTrans = Instantiate(scrapPreFab, grandChild).transform;

                BoxCollider boxCollider = scrapTrans.transform.GetChild(0).GetComponent<BoxCollider>();
                boxCollider.enabled = false;

                if (pickUpObjTrans == null)
                    pickUpObjTrans = scrapTrans;
                else
                    pickUpObj2Trans = scrapTrans;

                break;
            }
        }

        scrapQty++;
        anim.SetInteger("hands", scrapQty);

        //reset vars
        canPickUp = false;
    }

    private void Throw()
    {
        anim.SetBool("Attake", true);

        Transform objToThrow;

        if (pickUpObj2Trans != null)
            objToThrow = pickUpObj2Trans;
        else
            objToThrow = pickUpObjTrans;

        BoxCollider col = objToThrow.GetComponent<BoxCollider>();
        col.enabled = true;
        col.isTrigger = true;

        bool objOnRight = objToThrow.parent.localPosition.x > 0f;
        int side = objOnRight ? -1 : 1;
        float angle = 10f * side;

        objToThrow.SetParent(null);
        Rigidbody objRb = objToThrow.GetComponent<Rigidbody>();
        objRb.isKinematic = false;
        objRb.useGravity = true;

        Vector3 throwDirection = Quaternion.AngleAxis(angle, Vector3.up) * -transform.forward;
        objRb.AddForce(throwDirection * throwForce, ForceMode.VelocityChange);


        if (pickUpObj2Trans == objToThrow)
            pickUpObj2Trans = null;
        else
            pickUpObjTrans = null;

        scrapQty--;
        anim.SetInteger("hands", scrapQty);
    }
}
