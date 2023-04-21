using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnabler : MonoBehaviour
{
    public Oculus.Interaction.RayInteractable rayInteractable;

    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.SecondaryThumbstick)){
            rayInteractable.enabled = !rayInteractable.enabled;
        }
    }

}
