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
}
