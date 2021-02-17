using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : MonoBehaviour
{
    public bool bIsOnDestination = false;
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("End"))
        {
            bIsOnDestination = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("End"))
        {
            bIsOnDestination = false;
        }
    }
}
