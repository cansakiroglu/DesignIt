using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class RayObjectDetector : MonoBehaviour
{
    [SerializeField] private Transform rayOrigin;

    [SerializeField] private LayerMask IgnoreMe;

    [SerializeField] private OVRPassthroughLayer passthroughLayer;

    [SerializeField] private Oculus.Voice.AppVoiceExperience appVoiceExperience;

    [SerializeField] private TMPro.TMP_Text text;
    bool isXPressed = false;
    bool isYPressed = false;

    bool captureStarted = false;

    int waitFrame = 0;

    private GameObject hit_gameobject; 

    private void OnEnable()
    {
        appVoiceExperience.VoiceEvents.OnFullTranscription.AddListener(OnRequestFullTranscript);
    }

    private void OnRequestFullTranscript(string transcript){
        hit_gameobject.transform.name = transcript.ToLower();
        Debug.Log("Gameobject with ID : " + hit_gameobject.GetInstanceID() + " set to " + hit_gameobject.transform.name);
        text.SetText(transcript.ToLower());
        isXPressed = false;
    }


    // Start is called before the first frame update
    void Start()
    {
        passthroughLayer.hidden = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.Get(OVRInput.Button.Three) && !isXPressed){

            isXPressed = true;

            Debug.Log("X Pressed");
            RaycastHit hit;
            if (Physics.Raycast(rayOrigin.position, rayOrigin.forward, out hit, 100, ~IgnoreMe)){
                hit_gameobject = hit.transform.gameObject;
            }
            Debug.Log("Gameobject with ID : " + hit_gameobject.GetInstanceID() + " got hit.");
        }

        if (isXPressed && waitFrame > 0){
            waitFrame--;
        }

        if (isXPressed && waitFrame == 0){
            appVoiceExperience.Activate();
            text.SetText("Listening");
        }

        if (captureStarted && waitFrame > 0){
            waitFrame--;
        }

        if (captureStarted && waitFrame == 0){
            print("Sending Request");

            UnityWebRequest webRequest = UnityWebRequest.Get("http://192.168.137.1:5000");
            AsyncOperation asyncOp = webRequest.SendWebRequest();
            // yield return StartCoroutine(GetRequest("http://192.168.137.1:5000"));
            while (!asyncOp.isDone)

            hit_gameobject.name = webRequest.downloadHandler.text;
            hit_gameobject.transform.parent.parent.gameObject.name = webRequest.downloadHandler.text;

            text.SetText(webRequest.downloadHandler.text);

            // pop op result

            passthroughLayer.hidden = true;
            isYPressed = false;
            captureStarted = false;
        }

        if (OVRInput.Get(OVRInput.Button.Four) && !isYPressed){
            
            isYPressed = true;

            Debug.Log("Y Pressed");
            RaycastHit hit;
            if (Physics.Raycast(rayOrigin.position, rayOrigin.forward, out hit, 100, ~IgnoreMe)){
                hit_gameobject = hit.transform.gameObject;
                // print(hit.transform.name);
                if (hit.transform.name == "GenericPrefab"){
                    print("HITTED GENERICOBJECT");
                    passthroughLayer.hidden = false;
                    waitFrame = 50;
                    captureStarted = true;
                } else{
                    isYPressed = false;
                }

                print(hit.transform.name);
                print(hit.transform.gameObject.GetInstanceID());

            }
        } 
        
    }
    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            yield return webRequest.SendWebRequest();
            print("\n\nGot response : " + webRequest.downloadHandler.text);
        }
    }
}
