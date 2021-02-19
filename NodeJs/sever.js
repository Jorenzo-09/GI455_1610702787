var websocket = require('ws')

var websocketSever = new websocket.Server ({port : 25500}, ()=>{
    console.log("Jorenzo sever is running")
})

//var wsList = []
var roomList = []
websocketSever.on("connection",(ws)=>{
    //LobbyZone
    ws.on("message",(data)=>{

        console.log(data)

        var toJsonObj = JSON.parse(data)

        if(toJsonObj.eventName == "CreateRoom")//CreateRoom
        {                                
            var isFoundRoom = false

            for(var i =0 ; i < roomList.length; i ++)
            {
                if(roomList[i].roomName == toJsonObj.data)
                {
                    console.log(roomList[i].roomName+"---"+toJsonObj.data)
                    isFoundRoom = true
                    break;
                }
            }

            if(isFoundRoom == true)
            {
                //Create Fail

                var resultJson =
                {
                    eventName: "CreateRoom",
                    data: "Fail"
                }
                var strResult = JSON.stringify(resultJson)
                ws.send(strResult)
                
                

                console.log("Client create room fail.")
            }
            else
            {
                //Create Room Complete

                var newRoom =
                {
                    roomName: toJsonObj.data,
                    wsList: []
                }

                newRoom.wsList.push(ws)

                roomList.push(newRoom)

                console.log(roomList.length)

                /////                  
                var resultJson =
                {
                    eventName: "CreateRoom",
                    data: "Success"
                }
                var strResult = JSON.stringify(resultJson)
                ws.send(strResult)

                console.log("Client create room success.")
                /////
                    
            }
            console.log("client request CreateRoom ["+toJsonObj.data+"]")
                
            //for(var i =o ; i < roomList.length;i ++)
            //{
            //    console.log(roomList[i].roomName)
            //}
        }
           
        else if(toJsonObj.eventName == "JoinRoom")//joinRoom
        {
            var isFoundRoom = false

            for(var i =0 ; i < roomList.length; i ++)
            {
                if(roomList[i].roomName == toJsonObj.data)
                {
                    console.log(roomList[i].roomName+"---"+toJsonObj.data)

                    //join
                    roomList[i].wsList.push(ws)

                    isFoundRoom = true
                    break;
                }
            }

            if(isFoundRoom == true)
            {
                //Can Join 
                
                var resultJson =
                {
                    eventName: "JoinRoom",
                    data: "Success"
                }
                var strResult = JSON.stringify(resultJson)
                ws.send(strResult)
                
            }
            else // Join fail
            {
                var resultJson =
                {
                    eventName: "JoinRoom",
                    data: "Fail"
                }
                var strResult = JSON.stringify(resultJson)
                ws.send(strResult)
            }



            console.log("client request JoinRoom")
        }

        else if(toJsonObj.eventName == "LeaveRoom")
        {
           var isLeaveSuccess = false

            for (var i = 0; i < roomList.length; i++)
            {
                for (var j = 0; j < roomList[i].wsList.length;j++)
                {
                    roomList[i].wsList.splice(j,1)

                    if(roomList[i].wsList.length <= 0)
                    {
                        roomList.splice(i ,1)
                    }
                    isLeaveSuccess = true
                    break;
                }
            }

            if(isLeaveSuccess)
            {
                var resultJson =
                {
                    eventName: "LeaveRoom",
                    data: "Success"
                }
                var strResult = JSON.stringify(resultJson)
                ws.send(strResult)

                console.log("Leave room success")
            }
            else
            {
                

                console.log("Leave room fail")
            }
        }
            
        //Boardcast(data)
    })
        
    console.log('client connected.')

    //wsList.push(ws)

    ws.on("close", ()=>{
        
        //wsList = ArrayRemove(wsList,ws)
        console.log("client disconnected.")

        var isLeaveSuccess = false

        for (var i = 0; i < roomList.length; i++)
        {
            for (var j = 0; j < roomList[i].wsList.length; j++)
            {

                if(ws == roomList[i].wsList[j])
                {

                    roomList[i].wsList.splice(j, 1)

                    isLeaveSuccess = true

                    if(roomList[i].wsList.length <= 0)
                    {
                        roomList.splice(i ,1)
                    }
                    break;
                }
            }
        }
        
        
    })
})



/*function ArrayRemove(arr,value)
{
    return arr.filter((element)=>{
        return element != value
    })
}*/

function Boardcast(data)
{
    for(var i = 0; i < wsList.length; i++)
    {
        
        wsList[i].send(data)
    }
    
}