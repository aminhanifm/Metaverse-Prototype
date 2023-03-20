using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Sirenix.OdinInspector;
using MetaversePrototype.Tools;

namespace MetaversePrototype.Game
{
    public class MPObjectActive : MonoBehaviourPunCallbacks, IPunObservable
    {

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info){
            var go = gameObject;

            if (stream.IsWriting){  
                stream.SendNext(gameObject.activeSelf);
            } else {
                gameObject.SetActive((bool)stream.ReceiveNext());
            }
        }
    }
}
