using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ManageToggles : MonoBehaviour
{
    // Start is called before the first frame update
    public int numToggles=10;
    public string searchKeyword = "chair";

public void downloadSketchfabThumbnails(string searchKeyword){
    bool enableCache = true;
    SketchfabAPI.AuthorizeWithAPIToken("0d0c5741ed93477986ae00986540961b");
    UnityWebRequestSketchfabModelList.Parameters p = new UnityWebRequestSketchfabModelList.Parameters();
    p.downloadable = true;

    foreach (Transform child in transform) {
        GameObject.Destroy(child.gameObject);
    }

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

        searchKeyword = "laptop";
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

private void create_toggle_with_texture(Texture2D texture, string model_uid){
            
    UnityEngine.UI.ToggleGroup toggleGroup = GetComponent<UnityEngine.UI.ToggleGroup>();
    GameObject newToggle = Instantiate(Resources.Load("Toggle")) as GameObject;

    newToggle.transform.SetParent(transform, false);
    newToggle.transform.localPosition = new Vector3(0, 0, 0);
    newToggle.transform.localScale = new Vector3(1, 1, 1);
    //set the toggle group
    newToggle.GetComponent<UnityEngine.UI.Toggle>().group = toggleGroup;
    
    //get the ToggleDeselect script and set the model uid
    newToggle.GetComponent<Oculus.Interaction.ToggleDeselect>().model_uid = model_uid;

    //assign the texture to the toggle
    newToggle.transform.GetChild(2).GetComponent<UnityEngine.UI.Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);

    //make the background white
    newToggle.transform.GetChild(2).GetComponent<UnityEngine.UI.Image>().color = new Color(1, 1, 1, 1.0f);

}

}
