using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Charger : MonoBehaviour
{
    public int charge = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.ToLower().Contains("coal"))
        {
            if(charge < 5)
                charge++;
            
            Destroy(other.gameObject);
        }
    }
}
