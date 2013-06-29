system = require("system")
if system.args.length isnt 3
  console.log 'serverUrl and agentName is necessary!'
  phantom.exit 1
agentName = system.args[1]
serverUrl = system.args[2]
_ = require './underscore-min'
nta = require './native2asic'
datehelp = require './datehelp'

websocket = require("webpage").create()
websocket.settings.localToRemoteUrlAccessEnabled = true
websocket.onConsoleMessage = (info) -> console.log "~Websocket_Console: " + info
websocket.onCallback = (info) ->
  switch info.command
    when "CastTesk"
      CastTesk info.task
websocket.injectJs './jquery.1.8.3.min.js'
websocket.injectJs './jquery.signalR-1.1.1.min.js'
websocket.includeJs serverUrl + '/signalr/hubs', ->
  websocket.evaluate (serverUrl, agentName)->
    $.support.cors = false #todo: don't understand now
    $.connection.hub.url = serverUrl + '/signalr'
    taskHub = $.connection.taskHub
    $.connection.hub.start().done ->
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

  task.grabdate = datehelp.DisplayNowDate()
  now = Date.now()
  pageGrab.open encodeURI(task.url), (status) -> 
    gbdate = {}
    if status isnt 'success'
      task.status = 'fail'
      task.error = 'Unable to access page'
      #console.log '~Unable to access page'
    else
      pageGrab.injectJs 'jquery.1.8.3.min.js'
      pageGrab.injectJs "grabscripts/#{task.site}_#{task.command}.js"
      gbdate = pageGrab.evaluate ->
        return spGrab()
      task.spend = (Date.now() - now)
      if task.encoding == "gbk"
        if _.isArray gbdate
          _.each gbdate, (item, i)->
            gbdate[i] = nta.obj2asciiobj item
        else
          gbdate = nta.obj2asciiobj gbdate
    pageGrab.close()
    task = JSON.stringify task
    gbdate = JSON.stringify gbdate
    console.log "~PostData: " + gbdate
    websocket.evaluate (task, data)->
      taskHub = $.connection.taskHub
      taskHub.server.postData task, data
    , task, gbdate