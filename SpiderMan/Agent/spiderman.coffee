system = require("system")
if system.args.length isnt 3
  console.log 'serverUrl and agentName is necessary!'
  phantom.exit 1
agentName = system.args[1]
serverUrl = system.args[2]
_ = require 'underscore-min'
fs = require 'fs'

websocket = require("webpage").create()
websocket.settings.localToRemoteUrlAccessEnabled = true
websocket.onConsoleMessage = (msg) ->
  console.log "----- Websocket_Console: " + msg

websocket.injectJs 'jquery.1.8.3.min.js'
websocket.injectJs 'jquery.signalR-1.1.1.js'
websocket.includeJs serverUrl + '/signalr/hubs', ->
  websocket.evaluate (serverUrl)->
    $.connection.hub.url = serverUrl + '/signalr'
    taskHub = $.connection.taskHub
    $.connection.hub.start().done ->
      taskHub.server.joinGroup('agent');
      taskHub.server.registerAgent agentName
    taskHub.client.processTesk = (task) ->
      pageGrab = require("webpage").create()
      #pageGrab.settings.userAgent = 'Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/27.0.1453.93 Safari/537.36'
      pageGrab.settings.loadImages = false
      pageGrab.onError = (msg, trace) ->
        msgStack = ["ERROR: " + msg]
        if trace
          msgStack.push "TRACE:"
          trace.forEach (t)->
            msgStack.push " -> " + t.file + ": " + t.line + (t.function ? " (in function '" + t.function + "')" : "")
        task.status = 'fail'
        task.error = msgStack.join("\n")
        taskHub.server.GrabData task, {}
      
      time = Date.now()
      pageGrab.open encodeURI(task.url), (status) -> 
        if status isnt 'success'
          task.status = 'fail'
          task.error = 'Unable to access page'
          taskHub.server.GrabData task, {}
        else
          pageGrab.injectJs 'jquery.1.8.3.min.js'
          pageGrab.injectJs "grabscripts/#{task.site}_#{task.command}.js"
          gbdate = pageGrab.evaluate ->
            return xhGrab()
          try
            GrabDone_time = (Date.now() - time)/1000
            task.spend = GrabDone_time
            gbdate = JSON.stringify gbdate
            taskHub.server.PostGrabData task, gbdate
          catch e
            task.status = 'fail'
            task.error = e.toString()
            taskHub.server.GrabData task, {}
        pageGrab.close()
  , serverUrl