using System;
using System.Linq;
using System.Text;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.SceneManagement;

namespace MetaversePrototype.Game
{
    public class MPConnectToServer : MonoBehaviourPunCallbacks
    {
        public TMP_InputField textInputField;

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

            UpdateDebugText("Connected to the master server.");
            
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
            PhotonNetwork.LoadLevel("Prototype Demo");

        }

        public void UpdateDebugText(string text){
            StringBuilder sb = new StringBuilder();
            sb.Append(textInputField.text);
            sb.Append(text);
            textInputField.text = sb.ToString() + Environment.NewLine;
        }
    }    
}
