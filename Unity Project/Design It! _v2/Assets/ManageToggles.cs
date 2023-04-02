using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ManageToggles : MonoBehaviour
{
    // Start is called before the first frame update
    public int numToggles=10;
    public string searchKeyword = "chair";
    void Start()
    {
        // //get the ToggleGroup component of this object
        // UnityEngine.UI.ToggleGroup toggleGroup = GetComponent<UnityEngine.UI.ToggleGroup>();
        // //instantiate the toggles
        // for(int i=0; i<numToggles; i++){
        //     Debug.Log("Instantiating toggle");
        //     GameObject newToggle = Instantiate(Resources.Load("Toggle")) as GameObject;
        //     newToggle.transform.SetParent(transform, false);
        //     newToggle.transform.localPosition = new Vector3(0, 0, 0);
        //     newToggle.transform.localScale = new Vector3(1, 1, 1);
        //     //set the toggle group
        //     newToggle.GetComponent<UnityEngine.UI.Toggle>().group = toggleGroup;
        //     //assign random color
        //     newToggle.transform.GetChild(2).GetComponent<UnityEngine.UI.Image>().color = new Color(Random.value, Random.value, Random.value, 1.0f);
        // }

        //sleep for 5 seconds
        StartCoroutine(Sleeper());

        downloadSketchfabThumbnails(searchKeyword);
        
    }

    IEnumerator Sleeper(){
        Debug.Log("Sleeping for 5 seconds");
        yield return new WaitForSeconds(5);
        Debug.Log("Awake");
    }


//     void downloadSketchfabThumbnails(){
//         bool enableCache = true;
//         SketchfabAPI.AuthorizeWithAPIToken("0d0c5741ed93477986ae00986540961b");
//         UnityWebRequestSketchfabModelList.Parameters p = new UnityWebRequestSketchfabModelList.Parameters();
//         p.downloadable = true;

//         string searchKeyword = "chair";
//         List<SketchfabModel> m_ModelList = new List<SketchfabModel>();
//         SketchfabAPI.ModelSearch(((SketchfabResponse<SketchfabModelList> _answer) =>
//             {
//                 SketchfabResponse<SketchfabModelList> ans = _answer;
//                 m_ModelList = ans.Object.Models; 
//                 Debug.Log("m_ModelList.Count IN response function: " + m_ModelList.Count);
//                 foreach (SketchfabModel model in m_ModelList)
//                 {
//                     Debug.Log("model.Name: " + model.Name+"model thumbnail url: "+model.Thumbnails.ClosestThumbnailToSizeWithoutGoingBelow(100,100).Url);
//                     //Download the thumbnail
//                     StartCoroutine(DownloadThumbnailImage(model.Thumbnails.ClosestThumbnailToSizeWithoutGoingBelow(100,100).Url));
                   
//                 }

//             }), p, searchKeyword);

//         Debug.Log("m_ModelList.Count: " + m_ModelList.Count);
//         SketchfabThumbnail first_thumbnail= m_ModelList[0].Thumbnails.ClosestThumbnailToSizeWithoutGoingBelow(100,100);
//         Debug.Log("first_thumbnail.Url: " + first_thumbnail.Url);




//     }

//     IEnumerator DownloadThumbnailImage(string url)
// {
//         UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
//     yield return www.SendWebRequest();

//     if (www.result != UnityWebRequest.Result.Success)
//     {
//         Debug.Log(www.error);
//     }
//     else
//     {
//         Texture2D texture = DownloadHandlerTexture.GetContent(www);
//         // Do something with the downloaded thumbnail image
//         //Print the texture size
//         Debug.Log("Texture size: " + texture.width + "x" + texture.height);

//     }
// }

void downloadSketchfabThumbnails(string searchKeyword){
    bool enableCache = true;
    SketchfabAPI.AuthorizeWithAPIToken("0d0c5741ed93477986ae00986540961b");
    UnityWebRequestSketchfabModelList.Parameters p = new UnityWebRequestSketchfabModelList.Parameters();
    p.downloadable = true;

    // string searchKeyword = "chair";

    List<SketchfabModel> m_ModelList = new List<SketchfabModel>();
    List<Texture2D> m_ThumbnailTextures = new List<Texture2D>(); // New list to hold downloaded thumbnail textures

    SketchfabAPI.ModelSearch(((SketchfabResponse<SketchfabModelList> _answer) =>
        {
            SketchfabResponse<SketchfabModelList> ans = _answer;
            m_ModelList = ans.Object.Models; 
            Debug.Log("m_ModelList.Count IN response function: " + m_ModelList.Count);

            // Loop through each SketchfabModel object in m_ModelList and download its thumbnail image
            foreach (SketchfabModel model in m_ModelList)
            {
                string thumbnailUrl = model.Thumbnails.ClosestThumbnailToSizeWithoutGoingBelow(100,100).Url;
                StartCoroutine(DownloadThumbnailImage(thumbnailUrl, (Texture2D texture) =>
                    {
                        m_ThumbnailTextures.Add(texture); // Add downloaded texture to list

                        //get the uid to download the model
                        string uid = model.Uid;
                        create_toggle_with_texture(texture,uid); // Create a toggle with the downloaded texture
                    }));
            }
        // Debug.Log("m_ThumbnailTextures.Count: " + m_ThumbnailTextures.Count);
        
        }), p, searchKeyword);
}

IEnumerator DownloadThumbnailImage(string url, System.Action<Texture2D> callback)
{
    UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
    yield return www.SendWebRequest();

    if (www.result != UnityWebRequest.Result.Success)
    {
        Debug.Log(www.error);
    }
    else
    {
        Texture2D texture = DownloadHandlerTexture.GetContent(www);
        callback(texture); // Call the callback function with the downloaded texture
    }
}

void create_toggle_with_texture(Texture2D texture, string model_uid){
            
    UnityEngine.UI.ToggleGroup toggleGroup = GetComponent<UnityEngine.UI.ToggleGroup>();
    GameObject newToggle = Instantiate(Resources.Load("Toggle")) as GameObject;
    newToggle.transform.SetParent(transform, false);
    newToggle.transform.localPosition = new Vector3(0, 0, 0);
    newToggle.transform.localScale = new Vector3(1, 1, 1);
    //set the toggle group
    newToggle.GetComponent<UnityEngine.UI.Toggle>().group = toggleGroup;
    
    //get the ToggleDeselect script and set the model uid
    newToggle.GetComponent<Oculus.Interaction.ToggleDeselect>().model_uid = model_uid;

    //set the

    //assign the texture to the toggle
    newToggle.transform.GetChild(2).GetComponent<UnityEngine.UI.Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);

    //make the background white
    newToggle.transform.GetChild(2).GetComponent<UnityEngine.UI.Image>().color = new Color(1, 1, 1, 1.0f);


}



    // Update is called once per frame
    void Update()
    {
        
    }
}
