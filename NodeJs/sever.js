var websocket = require('ws')

var websocketSever = new websocket.Server ({port : 25500}, ()=>{
    console.log("Jorenzo sever is running")
})

var wsList = []

websocketSever.on("connection",(ws,rq)=>{

    console.log('client connected.')

    wsList.push(ws)

    ws.on("message",(data)=>{
        console.log("send from client : " + data)
        Boardcast(data,ws)
    })

    ws.on("close", ()=>{
        
        wsList = ArrayRemove(wsList,ws)
        console.log("client disconnected.")
    })
})



function ArrayRemove(arr,value)
{
    return arr.filter((element)=>{
        return element != value
    })
}

function Boardcast(data,me)
{
    for(var i = 0; i < wsList.length; i++)
    {
        if(me != wsList[i])
        {
            wsList[i].send(data)
        }
                
    }
    
}