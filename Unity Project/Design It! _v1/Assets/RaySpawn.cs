using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class RaySpawn : MonoBehaviour
{
    [SerializeField] private Transform origin;
    [SerializeField] private Transform ammo;
    private bool isShooting=false;



    // Update is called once per frame
    void Update()
    {
        DrawRaycast();
       //check if right trigger is pressed using the OVR api
        if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) > 0.5f && !isShooting ){
            Debug.Log("Right Trigger Pressed");
            Shoot();
        }

        if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) < 0.5f && isShooting)
        {
            isShooting = false;
        }
        
        
        
    }

    void Shoot()
    {
        isShooting = true;
        RaycastHit hit;


        if (Physics.Raycast(origin.position, origin.forward, out hit))
        {
            Debug.Log("Hit registered");
            Debug.Log(hit.point);
            Debug.Log("Hit transform:");
            Debug.Log(hit.transform.position);
            Debug.Log("Hit transform - hit point:");
            Debug.Log(hit.point - hit.transform.position);
            
            //find the object which the raycast hit
            GameObject hitObject = hit.transform.gameObject;
        


            // GameObject floor = GameObject.Find("FloorPlanePrefab");
            // Debug.Log("Floor transform:");
            // Debug.Log(floor.transform);
            
            Instantiate(ammo, hit.point, Quaternion.identity, hit.transform);
        }
    }

    void DrawRaycast(){
        //Draw a raycast in the scene view
        Debug.DrawRay(origin.position, origin.forward * 100, Color.red);

    }
}
