using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectCollision : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Chão: " + other.transform.parent.gameObject.name);
    }

    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint contact = collision.GetContact(0);
        Vector3 normal = contact.normal;
        Vector3 contactPoint = contact.point;

        Debug.DrawRay(contactPoint, normal * 1f, Color.red, 3f);       // surface normal
    }

}
