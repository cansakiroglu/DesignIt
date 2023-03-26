using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    public Material boundingBoxMaterial;

    void DrawBoundingBox(GameObject selectedObject)
        {
            Renderer objectRenderer = selectedObject.GetComponent<Renderer>();
            Bounds objectBounds = objectRenderer.bounds;

            // Create a new game object for the bounding box and set its material
            GameObject boundingBox = Instantiate(Resources.Load("BoundingBox")) as GameObject;
            boundingBox.transform.position = objectBounds.center;
            boundingBox.transform.rotation = selectedObject.transform.rotation;
            boundingBox.transform.localScale = selectedObject.GetComponent<Renderer>().bounds.size + new Vector3(0.002f, 0.002f, 0.002f);
            // boundingBox.transform.localScale = selectedObject.transform.InverseTransformDirection(objectBounds.size);
            print(boundingBox.transform.localScale);
            print(selectedObject.transform.TransformPoint(boundingBox.transform.localScale));
            print(selectedObject.transform.TransformDirection(boundingBox.transform.localScale));
            print(selectedObject.transform.TransformVector(boundingBox.transform.localScale));
            boundingBox.transform.localScale =  new Vector3(0.32f, 0.36f, 0.32f) + new Vector3(0.005f, 0.005f, 0.005f);
            boundingBox.transform.SetParent(selectedObject.transform, true);
        }

    void Start(){
        DrawBoundingBox(gameObject);
    }
    
    // void SelectObject(GameObject selectedObject)
    // {
    //     // Deselect any previously selected objects
    //     DeselectAllObjects();

    //     // Draw the bounding box around the selected object
    //     DrawBoundingBox(selectedObject);
    // }

    // void Update()
    // {
    //     if (Input.GetMouseButtonDown(0))
    //     {
    //         Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //         RaycastHit hit;

    //         if (Physics.Raycast(ray, out hit))
    //         {
    //             GameObject selectedObject = hit.collider.gameObject;
    //             SelectObject(selectedObject);
    //         }
    //     }
    // }

    void DeselectAllObjects()
    {
        // Destroy any previously created bounding boxes
        GameObject[] boundingBoxes = GameObject.FindGameObjectsWithTag("BoundingBox");

        foreach (GameObject boundingBox in boundingBoxes)
        {
            Destroy(boundingBox);
        }
    }

}
