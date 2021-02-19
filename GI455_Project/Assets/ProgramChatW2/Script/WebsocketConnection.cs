using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

namespace ProgramChatW2
{
    public class WebsocketConnection : MonoBehaviour
    {
        struct MessageData
        {
            public string username;
            public string message;

            public MessageData(string username, string message)
            {
                this.username = username;
                this.message = message;
            }
        }

        struct SocketEvent
        {
            public string eventName; 
            public string data; //data

            public SocketEvent(string eventName,string data)
            {
                this.eventName = eventName;
                this.data = data;
            }
        }
        private WebSocket websocket;
        public Text userText;
        public Text severText;
        public InputField massageText;
        public Text ipCom;
        public Text portCom;
        public GameObject main1;

        public Text inputName;
        public Text playerName;
        public Text checkWrong;

        public InputField createRoom;
        public InputField joinRoom;
        

        string saveDataText;
        string lastMessageMeSend;

        public GameObject panelCreatFail;
        public GameObject panelJoinFail;
        public GameObject panelLobby;

        //public GameObject createText;
        //public GameObject createText2;
        //public GameObject parent;


        

        // Start is called before the first frame update
        void Start()
        {
            

            
        }

        // Update is called once per frame
        void Update()
        {
            DataFormSever();
            playerName.text = inputName.text;
        }



        public void DataFormSever()
        {
            if (saveDataText != null && saveDataText != "")
            {
                SocketEvent receiveMessageData = JsonUtility.FromJson<SocketEvent>(saveDataText);

                if (receiveMessageData.eventName == "User")
                {
                    if (receiveMessageData.data == lastMessageMeSend)
                    {
                        userText.text += receiveMessageData.eventName + " : " + receiveMessageData.data + "\n";
                        severText.text += "\n";
                    }
                    else
                    {
                        severText.text += receiveMessageData.eventName + " : " + receiveMessageData.data + "\n";
                        userText.text += "\n";
                    }
                }
                else if (receiveMessageData.eventName == "CreateRoom" && receiveMessageData.data == "Success")
                {
                    panelLobby.SetActive(false);

                }
                else if (receiveMessageData.eventName == "CreateRoom" && receiveMessageData.data == "Fail")
                {
                    panelCreatFail.SetActive(true);
                }
                else if (receiveMessageData.eventName == "JoinRoom" && receiveMessageData.data == "Fail")
                {
                    panelJoinFail.SetActive(true);
                }
                else if (receiveMessageData.eventName == "JoinRoom" && receiveMessageData.data == "Success")
                {
                    panelLobby.SetActive(false);
                }
                else if (receiveMessageData.eventName == "LeaveRoom")
                {
                    Debug.Log("Leave Room Success");
                    panelLobby.SetActive(true);
                }

                saveDataText = "";
            }

        }

        public void OnDestroy()
        {
            if(websocket != null)
            {
                websocket.Close();
            }
        }

        public void ClickSend()
        {
            
            if (massageText.text == "" || websocket.ReadyState != WebSocketState.Open)                     
                return;
                          

            SocketEvent socketEvent = new SocketEvent("User", massageText.text);
            

            string toJsonStr = JsonUtility.ToJson(socketEvent);
            

            //saveDataText = massageText.text;

            websocket.Send(toJsonStr);
            Debug.Log(toJsonStr);
            lastMessageMeSend = massageText.text;
            massageText.text = "";

        }
        public void OnMessage(object sender,MessageEventArgs messageEventArgs)
        {
            Debug.Log("Message from sever : " + messageEventArgs.Data);

            saveDataText = messageEventArgs.Data;

        }

        public void NextScene()
        {
            if (ipCom.text == "127.0.0.1" && portCom.text == "25500")
            {
                websocket = new WebSocket("ws://127.0.0.1:25500/");

                websocket.OnMessage += OnMessage;

                websocket.Connect();

                main1.SetActive(false);


                Debug.Log("Connected Complete...");

                
            }
            else
            {
                Debug.Log("IP or Port is wrong!! Please try again");
                checkWrong.text = "IP or Port is wrong!! Please try again";
            }
        }

        public void CreateRoom()
        {
            if (string.IsNullOrEmpty(createRoom.text) == false)
            {
                SocketEvent socketEvent = new SocketEvent("CreateRoom", createRoom.text);

                string toJsonstr = JsonUtility.ToJson(socketEvent);

                websocket.Send(toJsonstr);

                createRoom.text = "";
            }
        }

        public void JoinRoom()
        {
            if(string.IsNullOrEmpty(joinRoom.text)== false)
            {
                SocketEvent socketEvent = new SocketEvent("JoinRoom", joinRoom.text);

                string toJsonstr = JsonUtility.ToJson(socketEvent);

                websocket.Send(toJsonstr);

                joinRoom.text = "";
            }
        }

        public void LeaveRoom()
        {
            SocketEvent socketEvent = new SocketEvent("LeaveRoom", "");

            string toJsonstr = JsonUtility.ToJson(socketEvent);

            websocket.Send(toJsonstr);
        }

        public void FailCreateRoom()
        {
            panelCreatFail.SetActive(false);
        }

        public void FailJoinRoom()
        {
            panelJoinFail.SetActive(false);
        }

        
    }
}


