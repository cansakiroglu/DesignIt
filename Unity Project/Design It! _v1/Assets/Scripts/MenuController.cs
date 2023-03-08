using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Auth0;
public class MenuController : MonoBehaviour
{

    public GameObject welcomeMsgObj;
    private Text welcomeMsg;

    public void Start(){
        welcomeMsg = welcomeMsgObj.GetComponent<Text>();
    }


    public void StartBtn(){
        SceneManager.LoadScene("SecondScene");
    }

    private void OnEnable()
    {
        updatePersonalizedWelcomeMsg();
    }



    private async void updatePersonalizedWelcomeMsg(){
        Auth0.Api.Credentials.Credentials userCred = await AuthManager.Instance.Credentials.GetCredentials();
        Auth0.AuthenticationApi.Models.UserInfo userInfo = await AuthManager.Instance.Auth0.GetUserInfoAsync(userCred.AccessToken);
        updateWelcomeMsg(userInfo.NickName);
        Debug.Log(userInfo.NickName);
    }

    private void updateWelcomeMsg(string userName){
        if (userName != null){
            welcomeMsg.text = "Welcome - " + userName;
        } else {
            welcomeMsg.text = "Welcome";
        }
    }
    
}
