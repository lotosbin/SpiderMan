system = require("system")
if system.args.length isnt 3
  console.log 'serverUrl and agentName is necessary!'
  phantom.exit 1
agentName = system.args[1]
serverUrl = system.args[2]
_ = require './underscore-min'
fs = require 'fs'

websocket = require("webpage").create()
websocket.settings.localToRemoteUrlAccessEnabled = true
websocket.onConsoleMessage = (info) -> console.log "~ Websocket_Console: " + info
websocket.onCallback = (info) ->
  switch info.command
    when "CastTesk"
      console.log "~CastTesk: " + JSON.stringify info.task
      CastTesk info.task
websocket.injectJs './jquery.1.8.3.min.js'
websocket.injectJs './jquery.signalR-1.1.1.min.js'
websocket.includeJs serverUrl + '/signalr/hubs', ->
  websocket.evaluate (serverUrl, agentName)->
    $.support.cors = false #todo: don't understand
    $.connection.hub.url = serverUrl + '/signalr' #cross domain
    taskHub = $.connection.taskHub
    $.connection.hub.start().done ->
      console.log taskHub.connection.id
      taskHub.server.joinGroup 'agent'
      taskHub.server.registerAgent agentName
    taskHub.client.castTesk = (task) ->
      castTesk = 
        command: "CastTesk"
        task: task
      window.callPhantom castTesk
  , serverUrl, agentName

CastTesk = (task)->
  pageGrab = require("webpage").create()
  pageGrab.settings.userAgent = 'Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/27.0.1453.93 Safari/537.36'
  pageGrab.settings.loadImages = false

  pageGrab.onError = (msg, trace) ->
    msgStack = [msg]
    if trace
      trace.forEach (t)->
        msgStack.push " -> " + t.file + ": " + t.line + (t.function ? " (in function '" + t.function + "')" : "")
    task.status = 'fail'
    task.error = msgStack.join("\n")
    PostData task, {}
    pageGrab.close()

  task.grabdate = Date.now()
  pageGrab.open encodeURI(task.url), (status) -> 
    if status isnt 'success'
      task.status = 'fail'
      task.error = 'Unable to access page'
      PostData task, {}
    else
      pageGrab.injectJs 'jquery.1.8.3.min.js'
      pageGrab.injectJs "grabscripts/#{task.site}_#{task.command}.js"
      gbdate = pageGrab.evaluate ->
        return xhGrab()
      GrabDone_time = (Date.now() - task.grabdate)/1000
      task.spend = GrabDone_time
      PostData task, gbdate
    pageGrab.close()

postsocket = require("webpage").create()
postsocket.settings.localToRemoteUrlAccessEnabled = true
postsocket.onConsoleMessage = (info) -> console.log "~ Postsocket_Console: " + info
postsocket.injectJs './jquery.1.8.3.min.js'
postsocket.injectJs './jquery.signalR-1.1.1.min.js'
postsocket.includeJs serverUrl + '/signalr/hubs'
PostData = (task, data)->
  postsocket.evaluate (task, data, serverUrl)->
    $.support.cors = false #todo: don't understand
    $.connection.hub.url = serverUrl + '/signalr'
    taskHub = $.connection.taskHub
    console.log "CastTesk 1: " + (JSON.stringify task)
    $.connection.hub.start().done ->
      taskHub.server.postData (JSON.stringify task), (JSON.stringify data)
  , task, data, serverUrl