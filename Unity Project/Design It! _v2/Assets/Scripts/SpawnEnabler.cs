using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnabler : MonoBehaviour
{
    // Start is called before the first frame update

    public Oculus.Interaction.RayInteractable rayInteractable;
    
    private bool changing;
    void Update()
    {
        if (OVRInput.Get(OVRInput.Button.Two) && !changing){
            changing = true;
            if (rayInteractable.enabled) {
                rayInteractable.enabled = false;
            } else {
                rayInteractable.enabled = true;
            }
            changing = false;
        }
    }

}
