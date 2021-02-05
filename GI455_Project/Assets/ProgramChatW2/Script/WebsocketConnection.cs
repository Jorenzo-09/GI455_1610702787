using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

namespace ProgramChatW2
{
    public class WebsocketConnection : MonoBehaviour
    {
        private WebSocket websocket;
        public Text userText;
        public Text severText;
        public Text massageText;
        public Text ipCom;
        public Text portCom;
        public GameObject main1;

        public Text inputName;
        public Text playerName;
        public Text checkWrong;

        public GameObject createText;
        public GameObject createText2;
        public GameObject parent;

        List<GameObject> saveMessage = new List<GameObject>();
        List<GameObject> saveMessage0 = new List<GameObject>();

        float y = 160;

        // Start is called before the first frame update
        void Start()
        {
            websocket = new WebSocket("ws://127.0.0.1:25500/");

            websocket.OnMessage += OnMessage;

            websocket.Connect();

            
        }

        // Update is called once per frame
        void Update()
        {
            playerName.text = inputName.text;

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
            websocket.Send(userText.text = massageText.text);
            y = 160;

            for (var j = 0; j < saveMessage.Count; j++)
            {
                saveMessage[j].transform.position= new Vector3(520, y+=15, 0);

            }
            var i = Instantiate(createText, new Vector3(520, 160, 0), Quaternion.identity, parent.transform);
            i.GetComponent<Text>().text = userText.text;
            saveMessage.Add(i);
            Debug.Log(saveMessage.Count);

            


        }
        public void OnMessage(object sender,MessageEventArgs messageEventArgs)
        {
            Debug.Log("Message from sever : " + messageEventArgs.Data);
            severText.text = messageEventArgs.Data;
          

           
            

        }

        public void NextScene()
        {
            if (ipCom.text == "127.0.0.1" && portCom.text == "25500")
            {
                main1.SetActive(false);
                Debug.Log("Connected Complete...");
            }
            else
            {
                Debug.Log("IP or Port is wrong!! Please try again");
                checkWrong.text = "IP or Port is wrong!! Please try again";
            }
        }
    }
}


