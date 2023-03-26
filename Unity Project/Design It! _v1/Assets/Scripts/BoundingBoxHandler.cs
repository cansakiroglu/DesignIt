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
            Renderer objectRenderer = selectedObject.GetComponent<Renderer>();
            Bounds objectBounds = objectRenderer.bounds;

            // creating a new game object for the bounding box and setting its material
            GameObject boundingBox = Instantiate(Resources.Load("BoundingBox")) as GameObject;
            boundingBox.transform.position = objectBounds.center;
            boundingBox.transform.rotation = selectedObject.transform.rotation;

            // using mesh bounds to get proper AABB
            boundingBox.transform.localScale = selectedObject.GetComponent<MeshFilter>().mesh.bounds.size + new Vector3(0.002f, 0.002f, 0.002f);

            boundingBox.transform.SetParent(selectedObject.transform, true);

            this.boundingBox = boundingBox;
        }
    }
    

    public void HideBoundingBox()
    {
        if (boundingBox != null) {
            boundingBox.SetActive(false);
        }
    }

    public void setSelectedObject(GameObject gameObject){
        this.selectedObject = gameObject;
    }
}
