using System;
using System.Linq;
using System.Text;
using System.Collections;
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

        #endregion

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

            UpdateDebugText("Joining a room..");
            
            if(useAssetBundle){
                StartCoroutine(LoadBundles());
            } else {
                // Dont forget to enable scene in build!
                PhotonNetwork.LoadLevel("Prototype Demo");
            }
        }
        protected const string url = "https://firebasestorage.googleapis.com/v0/b/metaverse-prototype.appspot.com/o/prototype%20demo?alt=media&token=61ef6b4c-387a-44d1-b37b-9189b851dd45";
        protected const string bundleName = "prototype demo";
        IEnumerator LoadBundles(){
    
            UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(url);
            StartCoroutine(WaitForResponse(www));
            yield return www.SendWebRequest();
    
            if (www.result != UnityWebRequest.Result.Success) {
                UpdateDebugText("Failed joining a room.");
            }
            else {
                AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(www);
                string[] scenePaths = bundle.GetAllScenePaths();
                PhotonNetwork.LoadLevel(scenePaths[0]);
            }
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
