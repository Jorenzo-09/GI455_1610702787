var websocket = require('ws')

const sqlite3 = require('sqlite3').verbose();

var websocketSever = new websocket.Server ({port : 25500}, ()=>{
    console.log("Jorenzo sever is running")
})

//var wsList = []
var roomList = []

let db = new sqlite3.Database('./database/chatDB.db', sqlite3.OPEN_CREATE | sqlite3.OPEN_READWRITE, (err)=>{
    if(err) throw err

    console.log('Connected to database. ')


})

websocketSever.on("connection",(ws)=>{
    //LobbyZone
    ws.on("message",(data)=>{

        var toJsonObj = JSON.parse(data)
            
        console.log(toJsonObj)       

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

            // Register
            
        }
        else if(toJsonObj.eventName == "Msg")//Message
        {

            var resultJson =
                {
                    eventName: "Msg",
                    data: toJsonObj.data
                }

            
                var strResult = JSON.stringify(resultJson)
                ws.send(strResult)

            Boardcast(ws,data)

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
        else if(toJsonObj.eventName == "Register")
        {
            var toJsonObjData = JSON.parse(toJsonObj.data)

            var sqlInsert = "INSERT INTO UserData (UserID,Name,Password) VALUES('"+toJsonObjData.userID+"','"+toJsonObjData.name+"','"+toJsonObjData.password+"')"

            db.all(sqlInsert, (err,rows)=>{
                if(err) // Regis Fail
                {
                    var callbackMsg = {
                        eventName:"Register",
                        data:"Fail"
                    }
        
                    var toJsonStr = JSON.stringify(callbackMsg)
                    console.log("[0]" + toJsonStr)
                    ws.send(toJsonStr)

                }
                
                else // Regis Success
                {
                    var callbackMsg = {
                        eventName:"Register",
                        data:"Success"
                    }
        
                    var toJsonStr = JSON.stringify(callbackMsg)
                    console.log("[1]" + toJsonStr)
                    ws.send(toJsonStr)
                }
            })
        }
        else if(toJsonObj.eventName == "Login")
        {
            var toJsonObjData = JSON.parse(toJsonObj.data)
            
            var sqlSelect = "SELECT * FROM UserData WHERE UserID = '"+toJsonObjData.userID+"' AND Password = '"+toJsonObjData.password+"'" // login
            
            

            db.all(sqlSelect, (err,rows)=>{
            
            
                if(err)
                {
                    console.log("[0]"+err)
                    
                }
                else
                {
                    if(rows.length > 0)
                    {
                        
                        console.log("--------1-------")
                        console.log(rows)
                        console.log("--------1-------")

                        
                        var callbackMsg = {
                            eventName:"Login",
                            data:rows[0].Name
                        }
        
                        var toJsonStr = JSON.stringify(callbackMsg)
                        ws.send(toJsonStr)                        
        
                    }
                    else
                    {
                        var callbackMsg = {
                            eventName:"Login",
                            data:"Fail"
                        }
        
                        var toJsonStr = JSON.stringify(callbackMsg)                                               
                        
                        ws.send(toJsonStr)
                    }
        
                    
                }
                
            })
        }
            
        
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

function Boardcast(ws,data)
{
    var roomNumber = -1;

    for(var i = 0; i < roomList.length; i++)
    {
        for(var j = 0; j < roomList[i].wsList.length; j++)
        {
            if(roomList[i].wsList[j] == ws)
            {
                roomNumber = i;
                break;
            }
        }
    }

    for(var i = 0;i<roomList[roomNumber].wsList.length; i++)
    {
        roomList[roomNumber].wsList[i].send(data)
    }
    
}