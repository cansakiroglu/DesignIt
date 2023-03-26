using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RaySpawn : MonoBehaviour
{
    [SerializeField] private Transform rayOrigin;
    [SerializeField] private GameObject objectToSpawn;
    private bool isCasting=false;

    void Update()
    {
        if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) > 0.5f && !isCasting){
            Debug.Log("Right Trigger Pressed");
            Cast();
        }

        if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) < 0.5f && isCasting)
        {
            isCasting = false;
        }
    }

    void Cast()
    {
        isCasting = true;
        RaycastHit hit;


        if (Physics.Raycast(rayOrigin.position, rayOrigin.forward, out hit))
        {
            Vector3 spawnPosition = hit.point;
            Quaternion spawnRotation = Quaternion.identity;

            spawnPosition = spawnPosition + Vector3.Scale(objectToSpawn.GetComponent<Renderer>().bounds.size, hit.normal) / 2;
            spawnRotation = Quaternion.FromToRotation(transform.up, hit.normal) * spawnRotation;
            Vector3 size = objectToSpawn.GetComponent<Renderer>().bounds.size;

            if (Physics.OverlapBox(spawnPosition, size / 2f + new Vector3(0.002f, 0.002f, 0.002f)).Length != 1)
            {
                return;
            }

            GameObject hitObject = hit.transform.gameObject;

            GameObject new_obj = Instantiate(objectToSpawn, spawnPosition, spawnRotation);
            
            new_obj.transform.SetParent(hit.transform, true);
            
            BoxCollider boxCollider = new_obj.AddComponent<BoxCollider>();
            Rigidbody rigidbody = new_obj.AddComponent<Rigidbody>();
            Oculus.Interaction.Grabbable grabbable = new_obj.AddComponent<Oculus.Interaction.Grabbable>();
            Oculus.Interaction.CustomTranslateTransformer translateTransformer = new_obj.AddComponent<Oculus.Interaction.CustomTranslateTransformer>();
            Oculus.Interaction.Surfaces.ColliderSurface colliderSurface = new_obj.AddComponent<Oculus.Interaction.Surfaces.ColliderSurface>();
            Oculus.Interaction.RayInteractable rayInteractable = new_obj.AddComponent<Oculus.Interaction.RayInteractable>();
            // Oculus.Interaction.InteractableUnityEventWrapper interactableUnityEventWrapper = new_obj.AddComponent<Oculus.Interaction.InteractableUnityEventWrapper>();
            InformationHolder informationHolder = new_obj.AddComponent<InformationHolder>();

            rigidbody.useGravity = false;
            rigidbody.isKinematic = true;
            rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rigidbody.freezeRotation = true;

            grabbable.InjectOptionalOneGrabTransformer(translateTransformer);
            colliderSurface.InjectCollider(boxCollider);
            rayInteractable.InjectOptionalPointableElement(grabbable);
            rayInteractable.InjectSurface(colliderSurface);
            
            // interactableUnityEventWrapper.InjectInteractableView(rayInteractable);


            Vector3 hitSurfaceNormal = hit.transform.InverseTransformDirection(hit.normal);

            if(Mathf.Abs(hitSurfaceNormal.x) > 0.5) {
                new_obj.GetComponent<InformationHolder>().setStaticAxis(0);
            } else if(Mathf.Abs(hitSurfaceNormal.y) > 0.5) {
                new_obj.GetComponent<InformationHolder>().setStaticAxis(1);
            } else {
                new_obj.GetComponent<InformationHolder>().setStaticAxis(2);
            }
        }
    }

}
