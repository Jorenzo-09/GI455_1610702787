using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

namespace ProgramChatW2
{
    public class WebsocketConnection : MonoBehaviour
    {

        struct Messager
        {
            public string username;
            public string message;

            public Messager(string username,string message)
            {
                this.username = username;
                this.message = message;
            }
        }
        struct DataUser
        {
            public string userID;
            public string name;
            public string password;
            public string repassword;

            public DataUser(string userID, string name,string password,string repassword)
            {
                this.userID = userID;
                this.name = name;
                this.password = password;
                this.repassword = repassword;
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

        // Regis
        public InputField IDnameRegis;
        public InputField nameRegis;
        public InputField passwordRegis;
        public InputField rePasswordRgis;

        public Text regisFail;

        // Login
        public InputField iDNameLogin;
        public InputField passwordLogin;

        public Text loginFail;
        
        //
        string saveDataText;
        string lastMessageMeSend;

        //Lobby
        public Text playerNameLobby;

        //Panel
        public GameObject panelCreatFail;
        public GameObject panelJoinFail;
        public GameObject panelLobby;
        public GameObject panelRegister;
        public GameObject panelRegisterFail;
        public GameObject panelLogin;
        public GameObject panelLoginFail;

        //public GameObject createText;
        //public GameObject createText2;
        //public GameObject parent;

        string saveName;
        

        // Start is called before the first frame update
        void Start()
        {
            websocket = new WebSocket("ws://127.0.0.1:25500/");

            websocket.OnMessage += OnMessage;

            websocket.Connect();

            Debug.Log("Connected Complete...");
        }

        // Update is called once per frame
        void Update()
        {
            DataFormSever();
             
        }

        public void DataFormSever()
        {
            if (saveDataText != null && saveDataText != "")
            {
                SocketEvent receiveMessageData = JsonUtility.FromJson<SocketEvent>(saveDataText);

                if (receiveMessageData.eventName == "Msg")
                {
                    Messager _receiveMessageData1 = JsonUtility.FromJson<Messager>(receiveMessageData.data);
                    if (_receiveMessageData1.username == saveName)
                    {
                        userText.text += _receiveMessageData1.username + " : " + _receiveMessageData1.message + "\n";
                        severText.text += "\n";
                    }
                    else
                    {
                        severText.text += _receiveMessageData1.username + " : " + _receiveMessageData1.message + "\n";
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

                    userText.text = "";
                    severText.text = "";

                }
                else if (receiveMessageData.eventName == "Register" && receiveMessageData.data == "Fail")
                {                    
                    panelRegisterFail.SetActive(true);
                    regisFail.text = "This userID is already in the system..";
                }
                else if (receiveMessageData.eventName == "Register" && receiveMessageData.data == "Success")
                {
                    panelRegister.SetActive(false);
                }
                else if (receiveMessageData.eventName == "Login" )//&& receiveMessageData.data == "Fail")
                {
                    if(receiveMessageData.data == "Fail")
                    {
                        panelLoginFail.SetActive(true);
                        loginFail.text = "ID name or Password is wrong Please Log-in again.";
                    }
                    else // Login Success
                    {
                        panelLogin.SetActive(false);

                        playerNameLobby.text = "User Name : "+ receiveMessageData.data;
                        saveName = receiveMessageData.data;
                        playerName.text = saveName;
                    }
                    
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
            Messager messager = new Messager(saveName, massageText.text);

            string toJsonStr0 = JsonUtility.ToJson(messager);

            SocketEvent socketEvent = new SocketEvent("Msg", toJsonStr0);
            
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

                main1.SetActive(false);   
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

        public void ClickRegister()
        {
            DataUser dataUser = new DataUser(IDnameRegis.text, nameRegis.text, passwordRegis.text,rePasswordRgis.text);

            string dataJsonstr = JsonUtility.ToJson(dataUser);

            SocketEvent socketEvent = new SocketEvent("Register",dataJsonstr);

            string toJsonstr = JsonUtility.ToJson(socketEvent);

            if (IDnameRegis.text == "" || nameRegis.text == "" || passwordRegis.text == "" || rePasswordRgis.text == "")
            {
                panelRegisterFail.SetActive(true);
                regisFail.text = "Please Input all field.";
            }
            else if (passwordRegis.text != rePasswordRgis.text)
            {
                panelRegisterFail.SetActive(true);
                regisFail.text = "Password not match.";
            }
            else
            {
                websocket.Send(toJsonstr);
            }           
        }

        public void ClickLogin()
        {
            DataUser dataUser = new DataUser(iDNameLogin.text,"",passwordLogin.text,"");

            string dataJsonstr = JsonUtility.ToJson(dataUser);

            SocketEvent socketEvent = new SocketEvent("Login", dataJsonstr);

            string toJsonstr = JsonUtility.ToJson(socketEvent);

            if (iDNameLogin.text == "" || passwordLogin.text == "")
            {
                panelLoginFail.SetActive(true);
                loginFail.text = "Please Input all field.";
            }
            else
            {
                websocket.Send(toJsonstr);
                Debug.Log("Do it");
            }
            iDNameLogin.text = "";
            passwordLogin.text = "";
           
        }

        public void RegisterRoom()
        {
            panelRegister.SetActive(true);
        }

        public void RegisterFail()
        {
            panelRegisterFail.SetActive(false);
        }

        public void FailCreateRoom()
        {
            panelCreatFail.SetActive(false);
        }

        public void FailJoinRoom()
        {
            panelJoinFail.SetActive(false);
        }
        public void FailLoginFail()
        {
            panelLoginFail.SetActive(false);
        }

        
    }
}


