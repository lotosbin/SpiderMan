system = require("system")
if system.args.length isnt 3
  console.log 'serverUrl and agentName is necessary!'
  phantom.exit 1
agentName = system.args[1]
serverUrl = system.args[2]
_ = require './underscore-min'

websocket = require("webpage").create()
websocket.settings.localToRemoteUrlAccessEnabled = true
websocket.onConsoleMessage = (info) -> console.log "~Websocket_Console: " + info
websocket.onCallback = (info) ->
  switch info.command
    when "CastTesk"
      CastTesk info.task
websocket.injectJs './jquery.1.10.2.min.js'
websocket.injectJs './jquery.signalR-1.1.2.min.js'
websocket.includeJs serverUrl + '/signalr/hubs', ->
  websocket.evaluate (serverUrl, agentName)->
    $.support.cors = false #todo: don't understand now
    $.connection.hub.url = serverUrl + '/signalr'
    taskHub = $.connection.taskHub
    $.connection.hub.start().done ->
      $.support.cors = true
      #console.log taskHub.connection.id
      taskHub.server.registerAgent agentName
    taskHub.client.castTesk = (task) ->
      castTesk =
        command: "CastTesk"
        task: task
      window.callPhantom castTesk
  , serverUrl, agentName

CastTesk = (task)->
  console.log "~CastTesk: " + JSON.stringify task
  pageGrab = require("webpage").create()
  pageGrab.settings.userAgent = 'Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/27.0.1453.93 Safari/537.36'
  pageGrab.settings.loadImages = false
  pageGrab.onError = (msg, trace) ->
    msgStack = [msg]
    if trace
      trace.forEach (t)->
        msgStack.push " -> " + t.file + ": " + t.line + (t.function ? " (in function '" + t.function + "')" : "")
    console.log '~EvaluateError: ' + msgStack.join("\n")

  now = Date.now()
  pageGrab.open task.url, (status) -> #encodeURI(task.url)
    gbdate = {}
    if status isnt 'success'
      task.status = 2 #Fail
      task.error = 'Unable to access page'
    else
      pageGrab.injectJs 'jquery.1.10.2.min.js'
      pageGrab.injectJs "grabscripts/#{task.site}_#{task.commandtype}.js"
      gbdate = pageGrab.evaluate ->
        return spGrab()
      gbdate.GrabDate = now
      task.spend = (Date.now() - now)/1000
      if not gbdate
        task.status = 2 #Fail
        task.error = 'gbdate is false'
      else
        task.status = 3 #Done
    pageGrab.close()
    websocket.evaluate (serverUrl, task, data)->
      taskHub = $.connection.taskHub
      taskHub.server.doneTask task
      if task.status != 2 #not Fail
        _data = JSON.stringify data
        #console.log "PostData: " + _data
        _posturl = serverUrl + "/task/post" + task.articletype + task.commandtype
        if task.commandtype == "Ids" || task.commandtype == "ListFirst"
          $.post _posturl,
            datajson: _data
            taskmodelid: task.taskmodelid
        else
          $.post _posturl,
            datajson: _data
    , serverUrl, task, gbdate