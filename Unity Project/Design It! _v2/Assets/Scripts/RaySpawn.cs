using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RaySpawn : MonoBehaviour
{
    [SerializeField] private Transform rayOrigin;
    [SerializeField] public GameObject objectToSpawn;

    [SerializeField] private LayerMask IgnoreMe;
    private bool isCasting=false;

    public bool is_menu_open = false;

    GameObject inventory_menu;

    void Start(){
        inventory_menu = GameObject.FindWithTag("menu_canvas");
    }

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

        //check if the X button is pressed
        if (OVRInput.GetDown(OVRInput.Button.Two))
        {
            Debug.Log("X button pressed");
            is_menu_open = !is_menu_open;
            inventory_menu.SetActive(is_menu_open);
        }
    }

    void Cast()
    {
        isCasting = true;
        RaycastHit hit;

        if(is_menu_open){
            return;
        }


        if (Physics.Raycast(rayOrigin.position, rayOrigin.forward, out hit, 100, ~IgnoreMe))
        {
            // return; // Adding this here temporarily to avoid the rest of the code

            bool enableCache = true;

            // SketchfabAPI.AuthorizeWithAPIToken("0d0c5741ed93477986ae00986540961b");
            // SketchfabAPI.GetModel("acd1cef307b94803846d624b251a4e63", (resp) =>
            // {
            //     // This second call will get the model information, download it and instantiate it
            //     SketchfabModelImporter.Import(resp.Object, (new_obj) =>
            //     {
            //         if(new_obj != null)
            //         {
            //             // Here you can do anything you like to obj (A unity game object containing the sketchfab model)

            Debug.Log("Object to spawn: "+objectToSpawn.name);
            GameObject new_obj = Instantiate(objectToSpawn, new Vector3(0, 0, 0), Quaternion.identity);
            new_obj.SetActive(true);
            new_obj.transform.SetParent(hit.transform, true);

            BoxCollider boxCollider = new_obj.AddComponent<BoxCollider>();
            Rigidbody rigidbody = new_obj.AddComponent<Rigidbody>();
            Oculus.Interaction.CustomGrabbable grabbable = new_obj.AddComponent<Oculus.Interaction.CustomGrabbable>();
            Oculus.Interaction.CustomTranslateTransformer translateTransformer = new_obj.AddComponent<Oculus.Interaction.CustomTranslateTransformer>();
            Oculus.Interaction.OneGrabRotateTransformer oneGrabRotateTransformer = new_obj.AddComponent<Oculus.Interaction.OneGrabRotateTransformer>();
            Oculus.Interaction.CustomTwoGrabPlaneTransformer twoGrabPlaneTransformer = new_obj.AddComponent<Oculus.Interaction.CustomTwoGrabPlaneTransformer>();
            Oculus.Interaction.Surfaces.ColliderSurface colliderSurface = new_obj.AddComponent<Oculus.Interaction.Surfaces.ColliderSurface>();
            Oculus.Interaction.RayInteractable rayInteractable = new_obj.AddComponent<Oculus.Interaction.RayInteractable>();    
            Oculus.Interaction.GrabInteractable grabInteractable = new_obj.AddComponent<Oculus.Interaction.GrabInteractable>();
            Oculus.Interaction.InteractableUnityEventWrapper interactableUnityEventWrapper = new_obj.AddComponent<Oculus.Interaction.InteractableUnityEventWrapper>();
            InformationHolder informationHolder = new_obj.AddComponent<InformationHolder>();
            BoundingBoxHandler boundingBoxHandler = new_obj.AddComponent<BoundingBoxHandler>();

            boundingBoxHandler.setSelectedObject(new_obj);

            interactableUnityEventWrapper.InjectInteractableView(rayInteractable);
            interactableUnityEventWrapper.WhenHover.AddListener(boundingBoxHandler.ShowBoundingBox);
            interactableUnityEventWrapper.WhenUnhover.AddListener(boundingBoxHandler.HideBoundingBox);


            rigidbody.useGravity = false;
            rigidbody.isKinematic = true;
            rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rigidbody.freezeRotation = true;

            grabbable.InjectOptionalOneGrabTransformerLeft(oneGrabRotateTransformer);
            grabbable.InjectOptionalOneGrabTransformerRight(translateTransformer);
            grabbable.InjectOptionalTwoGrabTransformer(twoGrabPlaneTransformer);
            twoGrabPlaneTransformer.InjectOptionalPlaneTransform(hit.transform);
            colliderSurface.InjectCollider(boxCollider);
            rayInteractable.InjectOptionalPointableElement(grabbable);
            rayInteractable.InjectSurface(colliderSurface);
            
            grabInteractable.InjectRigidbody(rigidbody);
            grabInteractable.InjectPointableElement(grabbable);


            Vector3 hitSurfaceNormal = hit.transform.InverseTransformDirection(hit.normal);

            if(Mathf.Abs(hitSurfaceNormal.x) > 0.5) {
                informationHolder.setStaticAxis(0);
            } else if(Mathf.Abs(hitSurfaceNormal.y) > 0.5) {
                informationHolder.setStaticAxis(1);
            } else {
                informationHolder.setStaticAxis(2);
            }

            ColliderFitToChildren(new_obj);
            informationHolder.setInitSize(boxCollider.bounds.size);

            print(boxCollider.bounds);

            var bounds = boxCollider.bounds;
            var x = bounds.size.x * new_obj.transform.lossyScale.x;
            var y = bounds.size.y * new_obj.transform.lossyScale.y;
            var z = bounds.size.z * new_obj.transform.lossyScale.z;

            Vector3 spawnPosition = hit.point;
            Quaternion spawnRotation = Quaternion.FromToRotation(transform.up, hit.normal) * Quaternion.identity;
            new_obj.transform.position = spawnPosition + Vector3.Scale(new Vector3(x, y, z), hit.normal) / 2;
            new_obj.transform.rotation = spawnRotation;

            
            //         }
            //     }, enableCache);
            // }, enableCache);
        }
      
    }

    private void ColliderFitToChildren(GameObject rootGameObject) {

        bool hasBounds = false;
        Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);

        bounds = ColliderFitToChildrenSub(bounds, rootGameObject, hasBounds);

        BoxCollider collider = rootGameObject.GetComponent<BoxCollider>();
        collider.center = bounds.center - rootGameObject.transform.position;
        collider.size = bounds.size;

    }

    private Bounds ColliderFitToChildrenSub(Bounds bounds, GameObject child, bool hasBounds){
        Renderer renderer = child.transform.GetComponent<Renderer>();
        if (renderer != null) {
            if (hasBounds) {
                bounds.Encapsulate(renderer.bounds);
            }
            else {
                bounds = renderer.bounds;
                hasBounds = true;
            }
        }
        for (int i = 0; i < child.transform.childCount; ++i) {
            Renderer childRenderer = child.transform.GetChild(i).GetComponent<Renderer>();
            if (childRenderer != null) {
                if (hasBounds) {
                    bounds.Encapsulate(childRenderer.bounds);
                }
                else {
                    bounds = childRenderer.bounds;
                    hasBounds = true;
                }
            }
            bounds = ColliderFitToChildrenSub(bounds, child.transform.GetChild(i).gameObject, hasBounds);
        }

        return bounds;
    }






}
