using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class DetectionAPITest : MonoBehaviour
{
    public Text textArea;
    public void SendRequest(){
        textArea.text = "Sending Request";

        StartCoroutine(GetRequest("http://192.168.137.1:5000"));
    }

    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            yield return webRequest.SendWebRequest();
            textArea.text += "\n\nGot response : " + webRequest.downloadHandler.text;
        }
    }
}