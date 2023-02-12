using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Agora.Rtc;

#if (UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
using UnityEngine.Android;
#endif


public class NewBehaviourScript : MonoBehaviour
{
    // Fill in your app ID.
    private string _appID = "3025b360b09d457582f0fe5b5ba78972";
    // Fill in your channel name.
    private string _channelName = "bil496agorachannel";
    // Fill in the temporary token you obtained from Agora Console.
    private string _token = "007eJxTYFjp1FMT7sygZdWbaylbIGkU9GprWMaR7psPSlln7TI0/KPAYGxgZJpkbGaQZGCZYmJqbmphlGaQlmqaZJqUaG5haW5UUH4tuSGQkeFEzyEGRigE8YUYkjJzTCzNEtPzixKTMxLz8lJzGBgAaIsjAQ==";
    // A variable to save the remote user uid.
    private uint remoteUid;
    internal VideoSurface LocalView;
    internal VideoSurface RemoteView;
    internal IRtcEngine RtcEngine;


    // Start is called before the first frame update
    #if (UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
    private ArrayList permissionList = new ArrayList() { Permission.Camera, Permission.Microphone };
    #endif

    void Start()
    {
        Debug.Log("Start called from NewBehaviourScript");
        SetupVideoSDKEngine();
        InitEventHandler();
        SetupUI();
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckPermissions();
        // check if the 'V' key is pressed
        if (Input.GetKeyDown(KeyCode.V))
        {
            // Make the canvas visible or invisible.
            GameObject.Find("Canvas").GetComponent<Canvas>().enabled = !GameObject.Find("Canvas").GetComponent<Canvas>().enabled;
            Debug.Log("Canvas visibility toggled");
        }



        // check if the 'J' key is pressed
        if (Input.GetKeyDown(KeyCode.J))
        {
            // Join the channel.
            Join();
            Debug.Log("Joined channel");
        }

        // check if the 'L' key is pressed
        if (Input.GetKeyDown(KeyCode.L))
        {
            // Leave the channel.
            Leave();
            Debug.Log("Left channel");
        }

    }

    private void CheckPermissions() {
    #if (UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
    foreach (string permission in permissionList)
    {
        if (!Permission.HasUserAuthorizedPermission(permission))
        {
            Permission.RequestUserPermission(permission);
        }
    }
    #endif
}

    private void SetupUI()
        {
            GameObject.Find("Canvas").GetComponent<Canvas>().enabled = false;
            GameObject go = GameObject.Find("LocalView");
            LocalView = go.AddComponent<VideoSurface>();
            go.transform.Rotate(0.0f, 0.0f, 180.0f);
            go = GameObject.Find("RemoteView");
            RemoteView = go.AddComponent<VideoSurface>();
            go.transform.Rotate(0.0f, 0.0f, 180.0f);
            go = GameObject.Find("Leave");
            go.GetComponent<Button>().onClick.AddListener(Leave);
            go = GameObject.Find("Join");
            go.GetComponent<Button>().onClick.AddListener(Join);
        }


    private void SetupVideoSDKEngine()
        {
            // Create an instance of the video SDK.
            RtcEngine = Agora.Rtc.RtcEngine.CreateAgoraRtcEngine();
            // Specify the context configuration to initialize the created instance.
            RtcEngineContext context = new RtcEngineContext(_appID, 0,
            CHANNEL_PROFILE_TYPE.CHANNEL_PROFILE_COMMUNICATION,AUDIO_SCENARIO_TYPE.AUDIO_SCENARIO_DEFAULT);
            // Initialize the instance.
            RtcEngine.Initialize(context);
            
        }

    private void InitEventHandler()
        {
            // Creates a UserEventHandler instance.
            UserEventHandler handler = new UserEventHandler(this);
            RtcEngine.InitEventHandler(handler);
        }


    public void Join()
{
    // Enable the video module.
    RtcEngine.EnableVideo();
    // Set the user role as broadcaster.
    RtcEngine.SetClientRole(CLIENT_ROLE_TYPE.CLIENT_ROLE_BROADCASTER);
    // Set the local video view.
    LocalView.SetForUser(0, "", VIDEO_SOURCE_TYPE.VIDEO_SOURCE_CAMERA);
    // Start rendering local video.
    LocalView.SetEnable(true);
    // Join a channel.
    RtcEngine.JoinChannel(_token, _channelName);
}




internal class UserEventHandler : IRtcEngineEventHandler
{
    private readonly NewBehaviourScript _videoSample;

    internal UserEventHandler(NewBehaviourScript videoSample)
    {
        _videoSample = videoSample;
    }
    // This callback is triggered when the local user joins the channel.
    public override void OnJoinChannelSuccess(RtcConnection connection, int elapsed)
    {
        Debug.Log("You joined channel: " +connection.channelId);
    }

    public override void OnUserJoined(RtcConnection connection, uint uid, int elapsed)
    {
        // Setup remote view.
        _videoSample.RemoteView.SetForUser(uid, connection.channelId, VIDEO_SOURCE_TYPE.VIDEO_SOURCE_REMOTE);
        // Save the remote user ID in a variable.
        _videoSample.remoteUid = uid;
    }

    // This callback is triggered when a remote user leaves the channel or drops offline.
    public override void OnUserOffline(RtcConnection connection, uint uid, USER_OFFLINE_REASON_TYPE reason)
    {
        _videoSample.RemoteView.SetEnable(false);
    }





}

public void Leave()
{
    // Leaves the channel.
    RtcEngine.LeaveChannel();
    // Disable the video modules.
    RtcEngine.DisableVideo();
    // Stops rendering the remote video.
    RemoteView.SetEnable(false);
    // Stops rendering the local video.
    LocalView.SetEnable(false);
}



    
void OnApplicationQuit()
{
    if (RtcEngine != null)
    {
        Leave();
        RtcEngine.Dispose();
        RtcEngine = null;
    }
}




}
