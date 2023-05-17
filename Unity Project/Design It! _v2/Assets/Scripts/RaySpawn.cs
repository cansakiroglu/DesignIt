using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RaySpawn : MonoBehaviour
{
    [SerializeField] private Transform rayOrigin;
    [SerializeField] public GameObject objectToSpawn;
    [SerializeField] private LayerMask IgnoreMe;
    [SerializeField] private GameObject inventory_menu;
    
    [SerializeField] private ManageToggles toggle_manager;

    [SerializeField] private SketchfabVoiceController voiceController;

    private bool isCasting=false;
    private bool is_menu_open=false;

    void Update()
    {
        is_menu_open = inventory_menu.activeSelf;

        if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) > 0.5f && !isCasting){
            Debug.Log("Right Trigger Pressed");
            print(objectToSpawn);
            if(objectToSpawn != null){
                Cast();
            }
        }

        if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) < 0.5f && isCasting)
        {
            isCasting = false;
        }


        if (OVRInput.GetDown(OVRInput.Button.One)) // A button
        {
            if (!inventory_menu.activeSelf){
                if (!voiceController.appVoiceExperience.Active)
                    voiceController.SetActivation(true);
            }
            else{
                inventory_menu.SetActive(false);
            }

        }
    }

    public void OpenMenu(string object_name)
    {
        toggle_manager.downloadSketchfabThumbnails(object_name);
        inventory_menu.SetActive(true);
    }

    private void Cast()
    {
        isCasting = true;
        RaycastHit hit;

        if(is_menu_open){
            return;
        }


        if (Physics.Raycast(rayOrigin.position, rayOrigin.forward, out hit, 100, ~IgnoreMe))
        {

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
            // GameObject new_obj = Instantiate(objectToSpawn, new Vector3(0, 0, 0), Quaternion.identity);
            GameObject new_obj = Instantiate(objectToSpawn, hit.point, Quaternion.identity);
            // new_obj.transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * Quaternion.identity;
            
            new_obj.SetActive(false);
            new_obj.transform.SetParent(hit.transform, true);

            BoxCollider boxCollider = new_obj.AddComponent<BoxCollider>();
            Rigidbody rigidbody = new_obj.AddComponent<Rigidbody>();
            Oculus.Interaction.CustomGrabbable grabbable = new_obj.AddComponent<Oculus.Interaction.CustomGrabbable>();
            Oculus.Interaction.CustomTranslateTransformer translateTransformer = new_obj.AddComponent<Oculus.Interaction.CustomTranslateTransformer>();
            Oculus.Interaction.OneGrabRotateTransformer oneGrabRotateTransformer = new_obj.AddComponent<Oculus.Interaction.OneGrabRotateTransformer>();
            Oculus.Interaction.CustomTwoGrabPlaneTransformer twoGrabPlaneTransformer = new_obj.AddComponent<Oculus.Interaction.CustomTwoGrabPlaneTransformer>();
            Oculus.Interaction.Surfaces.ColliderSurface colliderSurface = new_obj.AddComponent<Oculus.Interaction.Surfaces.ColliderSurface>();
            Oculus.Interaction.RayInteractable rayInteractable = new_obj.AddComponent<Oculus.Interaction.RayInteractable>();
            Oculus.Interaction.GrabInteractable grabInteractable = new_obj.AddComponent<Oculus.Interaction.GrabInteractable>(); // some initialization errors
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

            new_obj.SetActive(true);

            Vector3 hitSurfaceNormal = hit.transform.InverseTransformDirection(hit.normal);

            if(Mathf.Abs(hitSurfaceNormal.x) > 0.5) {
                informationHolder.setStaticAxis(0);
            } else if(Mathf.Abs(hitSurfaceNormal.y) > 0.5) {
                informationHolder.setStaticAxis(1);
            } else {
                informationHolder.setStaticAxis(2);
            }

            ColliderFitToChildren(new_obj);

            // resizing the object for suitable size
            BoxCollider parent_collider = hit.transform.GetComponent<BoxCollider>();
            Vector3 boxcollider_bounds_size_scaled = Vector3.Scale(boxCollider.bounds.size, new_obj.transform.lossyScale);
            Vector3 parentcollider_bounds_size_scaled = Vector3.Scale(parent_collider.bounds.size, new_obj.transform.lossyScale);
            while(Mathf.Max(boxcollider_bounds_size_scaled.x, boxcollider_bounds_size_scaled.y, boxcollider_bounds_size_scaled.z) * 5 > Mathf.Max(parentcollider_bounds_size_scaled.x, parentcollider_bounds_size_scaled.y, parentcollider_bounds_size_scaled.z)){
                new_obj.transform.localScale = new_obj.transform.localScale / 2;
                boxcollider_bounds_size_scaled = Vector3.Scale(boxCollider.bounds.size, new_obj.transform.lossyScale);
                parentcollider_bounds_size_scaled = Vector3.Scale(parent_collider.bounds.size, parent_collider.transform.lossyScale);
            }

            Vector3 boxcollider_size_scaled = Vector3.Scale(boxCollider.size, new_obj.transform.lossyScale);
            Vector3 boxcollider_center_scaled = Vector3.Scale(boxCollider.center, new_obj.transform.lossyScale);
            informationHolder.setInitSize(boxcollider_size_scaled);

            print(new_obj.transform.position);
            print(new_obj.transform.localPosition);
            print(boxCollider.bounds.center);
            print(boxCollider.center);

            // new_obj.transform.localPosition = new Vector3(-boxcollider_center_scaled.x, -boxcollider_center_scaled.y, -boxcollider_center_scaled.z);

            print(new_obj.GetComponent<InformationHolder>().getStaticAxis());
            print(new_obj.transform.localPosition.y - boxcollider_center_scaled.y);

            // switch(new_obj.GetComponent<InformationHolder>().getStaticAxis()){
            //     case 0:
            //         new_obj.transform.localPosition =  new Vector3((new_obj.transform.localPosition.x - boxcollider_center_scaled.x) + boxcollider_size_scaled.x / 2, new_obj.transform.localPosition.y, new_obj.transform.localPosition.z);
            //         break;
            //     case 1:
            //         new_obj.transform.localPosition =  new Vector3(new_obj.transform.localPosition.x, (new_obj.transform.localPosition.y - boxcollider_center_scaled.y) + boxcollider_size_scaled.y / 2, new_obj.transform.localPosition.z);
            //         break;
            //     case 2:
            //         new_obj.transform.localPosition =  new Vector3(new_obj.transform.localPosition.x, new_obj.transform.localPosition.y, (new_obj.transform.localPosition.z - boxcollider_center_scaled.z) + boxcollider_size_scaled.z / 2);
            //         break;
            // }


            // new_obj.transform.localPosition -= Vector3.Scale(boxcollider_center_scaled, informationHolder.getStaticAxisVector()); 

            // new_obj.transform.localPosition -= boxcollider_center_scaled;
            Vector3 center_local = hit.transform.InverseTransformDirection(boxcollider_center_scaled);
            new_obj.transform.localPosition -= Vector3.Scale(new Vector3(Mathf.Abs(center_local.x), Mathf.Abs(center_local.y), Mathf.Abs(center_local.z)), informationHolder.getStaticAxisVector());
            // new_obj.transform.localPosition += boxcollider_size_scaled / 2;

            // new_obj.transform.localPosition -= Vector3.Scale(boxcollider_center_scaled, informationHolder.getStaticAxisVector());
            Vector3 boxcollider_size_scaled_local = hit.transform.InverseTransformDirection(boxcollider_size_scaled);
            new_obj.transform.localPosition += Vector3.Scale(new Vector3(Mathf.Abs(boxcollider_size_scaled_local.x), Mathf.Abs(boxcollider_size_scaled_local.y), Mathf.Abs(boxcollider_size_scaled_local.z)) / 2, informationHolder.getStaticAxisVector());
            // new_obj.transform.localPosition = new Vector3(new_obj.transform.localPosition.x, new_obj.transform.localPosition.y, 0);
            
            // print(new_obj.transform.localPosition);


            // boxCollider.center += boxcollider_center_scaled;
            // boxCollider.center -= Vector3.Scale(boxcollider_size_scaled / 2, informationHolder.getStaticAxisVector());

            // boxCollider.center.Set(boxCollider.center.x-new_obj.transform.localPosition.x, boxCollider.center.y-new_obj.transform.localPosition.y, boxCollider.center.z-new_obj.transform.localPosition.z);

            Quaternion spawnRotation = Quaternion.FromToRotation(transform.up, hit.normal) * Quaternion.identity;
            new_obj.transform.rotation = spawnRotation;

        }
      
    }

    private void ColliderFitToChildren(GameObject rootGameObject) {

        bool hasBounds = false;
        Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);

        bounds.center = rootGameObject.transform.position;

        ColliderFitToChildrenSub(ref bounds, rootGameObject, ref hasBounds);

        BoxCollider collider = rootGameObject.GetComponent<BoxCollider>();

        // collider.center = bounds.center - rootGameObject.transform.position;

        Vector3 final_size = new Vector3(0, 0, 0);

        final_size = bounds.size;
        // collider.size = Vector3.Scale(final_size, rootGameObject.transform.lossyScale);
        collider.size = final_size;

        print(bounds.center);
        print(rootGameObject.transform.InverseTransformDirection(bounds.center));
        print(rootGameObject.transform.InverseTransformPoint(bounds.center));
        print(rootGameObject.transform.InverseTransformVector(bounds.center));

        collider.center = rootGameObject.transform.InverseTransformPoint(bounds.center);
    }

    private void ColliderFitToChildrenSub(ref Bounds bounds, GameObject child, ref bool hasBounds){
        print(child.name);
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
            ColliderFitToChildrenSub(ref bounds, child.transform.GetChild(i).gameObject, ref hasBounds);
        }
    }
}
