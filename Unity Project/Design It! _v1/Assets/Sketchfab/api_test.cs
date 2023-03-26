using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class api_test : MonoBehaviour
{

    public string Email;
    public string Password;
    public string ModelUIDToDownload;


    // Start is called before the first frame update
    void Start()
    {
        // SketchfabAPI.GetAccessToken(Email, Password, (SketchfabResponse<SketchfabAccessToken> answer) =>
        // {
        //     if(answer.Success)
        //     { 
        //         SketchfabAPI.AuthorizeWithAccessToken(answer.Object);
        //         DownloadModel();
        //     }
        //     else
        //     {
        //         Debug.LogError(answer.ErrorMessage);
        //     }

        // });
        SketchfabAPI.AuthorizeWithAPIToken("0d0c5741ed93477986ae00986540961b");
        SearchModel();
    }

    private void SearchModel(){

        UnityWebRequestSketchfabModelList.Parameters p = new UnityWebRequestSketchfabModelList.Parameters();
        p.downloadable = true;
        string searchKeyword = "Chair";
        SketchfabAPI.ModelSearch(((SketchfabResponse<SketchfabModelList> _answer) =>
        {
            SketchfabResponse<SketchfabModelList> ans = _answer;
            List<SketchfabModel> m_ModelList = ans.Object.Models;
            print(m_ModelList[0].Uid);
            print(m_ModelList[0].Thumbnails.ToString());
            DownloadModel(m_ModelList[0].Uid);
        }), p, searchKeyword);

        
    }


    private void DownloadModel(string _uid){
        // This first call will get the model information
        SketchfabAPI.GetModel(_uid, (resp) =>
        {
            // This second call will get the model information, download it and instantiate it
            SketchfabModelImporter.Import(resp.Object, (obj) =>
            {
                if(obj != null)
                {
                    // Here you can do anything you like to obj (A unity game object containing the sketchfab model)
                }
            });
        });
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
