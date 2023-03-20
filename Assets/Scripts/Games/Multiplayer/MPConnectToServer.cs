using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace MetaversePrototype.Game
{
    public class MPConnectToServer : MonoBehaviourPunCallbacks
    {
        public TMP_InputField textInputField;
        public Slider progressBar;
        public TextMeshProUGUI progressText;
        public bool useAssetBundle = true;

        #region Photon Settings
        private const byte maxPlayer = 4;
        public List<GameObject> prefabs;

        #endregion

        private void Awake() {
            DefaultPool pool = PhotonNetwork.PrefabPool as DefaultPool;
            foreach (GameObject obj in prefabs)
            {
                pool.ResourceCache.Add(obj.name, obj);
            }
        }

        private void Start() {
            PhotonNetwork.ConnectUsingSettings();
        }

        public override void OnConnected()
        {
            base.OnConnected();

            UpdateDebugText("Connect to the server sucessfully!");
        }

        public override void OnConnectedToMaster()
        {
            base.OnConnected();
            
            RoomOptions option = new RoomOptions();
            option.MaxPlayers = maxPlayer;
            PhotonNetwork.JoinRandomOrCreateRoom(roomOptions: option);
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            base.OnJoinRandomFailed(returnCode, message);

            UpdateDebugText(message + " (error code: " + returnCode +")");
        }

        public override void OnCreatedRoom()
        {
            base.OnCreatedRoom();

            UpdateDebugText("Sucessfully creted a room.");
        }

        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();
            
            PhotonNetwork.IsMessageQueueRunning = false;

            UpdateDebugText("Joining a room..");
            
            if(useAssetBundle){
                StartCoroutine(LoadBundles());
            } else {
                // Dont forget to enable scene in build!
                PhotonNetwork.LoadLevel("Prototype Demo");
            }
        }
        protected const string urlAndroid = "https://drive.google.com/uc?export=download&id=1sS__LYUNDYHDKddmzCcQidDwjXv1KD-a";
        protected const string urlWindow = "https://drive.google.com/uc?export=download&id=1Df3jS53xZAPxhaq9LyQr2B_zzu887DMU";
        protected const string urlWebGL = "https://drive.google.com/uc?export=download&id=1vNEihUPFX9nkr-vHYXR0NHjZgJ_lJ3kq";
        protected const string bundleName = "prototype demo";
        IEnumerator LoadBundles(){
            string url = string.Empty;

            switch (Application.platform)
            {
                case RuntimePlatform.WindowsPlayer:
                    url = urlWindow;
                    break;
                case RuntimePlatform.WebGLPlayer:
                    url = urlWebGL;
                    break;
                case RuntimePlatform.Android:
                    url = urlAndroid;
                    break;
                default:
                    url = urlAndroid;
                    break;
            }
            
            UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(url);
            // print(url);
            StartCoroutine(WaitForResponse(www));
            yield return www.SendWebRequest();
    
            if (www.result != UnityWebRequest.Result.Success) {
                UpdateDebugText("Failed joining a room. ("+ www.error + ")");
                yield return new WaitForSeconds(5);
                UpdateDebugText("Connection timeout");
                PhotonNetwork.Disconnect();
                UpdateDebugText("Try to reopen the game");
            }
            else {
                AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(www);
                string[] scenePaths = bundle.GetAllScenePaths();
                PhotonNetwork.LoadLevel(scenePaths[0]);

                // Wait until the scene has loaded before starting to receive RPCs
                if (!PhotonNetwork.IsMasterClient)
                {
                    float timeout = 5.0f;  // Timeout in seconds
                    float timer = 0.0f;

                    while (!AllScenesLoaded(scenePaths) && timer < timeout)
                    {
                        timer += Time.deltaTime;
                        yield return null;
                    }

                    if (!AllScenesLoaded(scenePaths))
                    {
                        // Timeout: Master client takes over and loads the level
                        if (PhotonNetwork.IsMasterClient)
                        {
                            PhotonNetwork.LoadLevel(scenePaths[0]);
                        }
                        else
                        {
                            UpdateDebugText("Timeout waiting for other clients to load level.");
                        }
                    }
                }

                // Now that the scene has loaded, start receiving RPCs
                PhotonNetwork.IsMessageQueueRunning = true;
            }
        }

        bool AllScenesLoaded(string[] scenePaths)
        {
            foreach (string scenePath in scenePaths)
            {
                if (!SceneManager.GetSceneByName(scenePath).isLoaded)
                {
                    return false;
                }
            }
            return true;
        }

        IEnumerator WaitForResponse(UnityWebRequest request)
        {
            while (!request.isDone)
            {
                // print(request.downloadProgress);
                progressBar.value = request.downloadProgress;
                progressText.text = (request.downloadProgress * 100).ToString("0") + "%";
                yield return new WaitForSeconds(0.2f);
            }
        }

        public void UpdateDebugText(string text){
            StringBuilder sb = new StringBuilder();
            sb.Append(textInputField.text);
            sb.Append(text);
            textInputField.text = sb.ToString() + Environment.NewLine + Environment.NewLine;
        }
    }    
}
