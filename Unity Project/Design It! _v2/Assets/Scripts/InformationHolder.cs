using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InformationHolder : MonoBehaviour
{

    private int staticAxis;

    public void setStaticAxis(int axis){
        staticAxis = axis;
    }

    public int getStaticAxis(){
        return staticAxis;
    }

    public Vector3 getStaticAxisVector(){
        Vector3 toReturn = new Vector3(0f, 0f, 0f);
        if (staticAxis == 0){
            toReturn.x = 1;
        } else if (staticAxis == 1){
            toReturn.y = 1;
        } else{
            toReturn.z = 1;
        }

        return toReturn;
    }
}
