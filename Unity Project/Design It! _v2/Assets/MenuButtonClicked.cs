using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class MenuButtonClicked : MonoBehaviour

{

    public string model_uid_to_download = "";
    
    public Oculus.Interaction.ToggleDeselect[] toggles;
    
    [SerializeField] private RaySpawn ray_spawn;
    
    
    // Start is called before the first frame update
    void Start()
    {
        print(gameObject.transform.parent.name);
        //add onClick function to the button
        // GetComponent<UnityEngine.UI.Button>().onClick.AddListener(CustomOnClick);

        if (gameObject.name == "DownloadButton"){
            GetComponent<UnityEngine.UI.Button>().onClick.AddListener(DownloadOnClick);
        }
        if (gameObject.name == "SetActiveButton"){
            GetComponent<UnityEngine.UI.Button>().onClick.AddListener(SetActiveOnClick);
        }
        
        
    }

    //add onClick function to the button
    public void DownloadOnClick(){
        Debug.Log("Download button clicked");
        foreach (Oculus.Interaction.ToggleDeselect toggle in toggles){
            Debug.Log("toggle.isOn: "+toggle.isOn);
            if (toggle.isOn){
                Debug.Log("Toggle is on");
                model_uid_to_download = toggle.model_uid;
                Debug.Log("model_uid_to_download: "+model_uid_to_download);
                DownloadModel(model_uid_to_download,toggle);
                break;
            }
        }
    }


    public void SetActiveOnClick(){
        Debug.Log("Set Active button clicked");
    }

    void DownloadModel(string _uid, Oculus.Interaction.ToggleDeselect toggle){
          // This first call will get the model information
            bool enableCache = false;
            SketchfabAPI.AuthorizeWithAPIToken("0d0c5741ed93477986ae00986540961b");

            SketchfabAPI.GetModel(_uid, (resp) =>
            {
                // This second call will get the model information, download it and instantiate it
                SketchfabModelImporter.Import(resp.Object, (obj) =>
                {
                    if(obj != null)
                    {
                        toggle.correspondingAmmo= obj;
                        toggle.already_downloaded = true;
                        obj.SetActive(false);
                        // Here you can do anything you like to obj (A unity game object containing the sketchfab model)
                    }
                }, enableCache);
            }, enableCache);

    }

    void setActiveAmmo(){


    }


    // Update is called once per frame
    void Update()
    {

        toggles= GameObject.FindWithTag("menu_content").GetComponentsInChildren<Oculus.Interaction.ToggleDeselect>();
        // Debug.Log("toggles.Length: "+toggles.Length);

        foreach (Oculus.Interaction.ToggleDeselect toggle in toggles){
            
            if (gameObject.name == "DownloadButton"){
                if (toggle.isOn && toggle.already_downloaded){
                // Debug.Log("Toggle is on and already downloaded");
                
                GetComponent<UnityEngine.UI.Button>().interactable = false;
                GetComponent<UnityEngine.UI.Text>().text = "Downloaded";
                GetComponent<UnityEngine.UI.Text>().color = Color.green;
                ray_spawn.objectToSpawn = toggle.correspondingAmmo;
                break;

            }else if (toggle.isOn && !toggle.already_downloaded){
                // Debug.Log("Toggle is on and not downloaded");
                GetComponent<UnityEngine.UI.Button>().interactable = true;
                GetComponent<UnityEngine.UI.Text>().text = "Download";
                GetComponent<UnityEngine.UI.Text>().color = Color.white;
                break;
            }

            }

            if (gameObject.name=="SetActiveButton"){
                if (toggle.isOn){
                    // Debug.Log("Toggle is on");
                    GetComponent<UnityEngine.UI.Button>().interactable = true;
                    GetComponent<UnityEngine.UI.Text>().text = "Set Active";
                    GetComponent<UnityEngine.UI.Text>().color = Color.white;
                    break;
                }else{
                    GetComponent<UnityEngine.UI.Button>().interactable = false;
                    GetComponent<UnityEngine.UI.Text>().text = "Set Active";
                    GetComponent<UnityEngine.UI.Text>().color = Color.white;
                    break;
                }
            } //Probs going to delete this part
              
                
                
            }

        }

    }




