using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundingBoxHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject selectedObject;
    private GameObject boundingBox;
    public void ShowBoundingBox()
    {
        if (boundingBox != null) {
            boundingBox.SetActive(true);
        } else {
            // Bounds objectBounds = selectedObject.GetComponent<Collider>().bounds;
            BoxCollider objectCollider = selectedObject.GetComponent<BoxCollider>();
            // creating a new game object for the bounding box and setting its material
            GameObject boundingBox = Instantiate(Resources.Load("BoundingBox")) as GameObject;
            // boundingBox.transform.position = objectBounds.center;
            boundingBox.transform.position = objectCollider.center;
            // boundingBox.transform.rotation = selectedObject.transform.rotation;

            Vector3 initsize = selectedObject.GetComponent<InformationHolder>().getInitSize();
            Vector3 parentscale = selectedObject.transform.lossyScale;
            // using mesh bounds to get proper AABB
            boundingBox.transform.localScale = new Vector3(initsize.x / parentscale.x, initsize.y / parentscale.y, initsize.z / parentscale.z);
            boundingBox.transform.localScale += boundingBox.transform.localScale * 0.1f;

            boundingBox.transform.SetParent(selectedObject.transform, false);

            this.boundingBox = boundingBox;
        }
    }

    public void Start()
    {
        // ShowBoundingBox();
    }

    public void Update()
    {
        if (OVRInput.Get(OVRInput.Button.Two) && boundingBox.activeSelf){
            Destroy(gameObject);
        }
    }
    

    public void HideBoundingBox()
    {
        if (boundingBox != null) {
            boundingBox.SetActive(false);
        }
    }

    public GameObject getBoundingBox()
    {
        return boundingBox;
    }

    public void setSelectedObject(GameObject gameObject){
        this.selectedObject = gameObject;
    }
}
