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
      $.post serverUrl + "/task/postdata",
        taskjson: '{"articletype":0,"command":"getList","commandtype":1,"error":"","id":"48f2caef-cec5-4236-875d-85defd1fcbc1","site":"qiushibaike","spend":2779,"status":1,"url":"zxczxczxc"}'
        datajson: '<img src="http://zxvsdfsa" />asda<p>sadasd</p>sdasd<a title="cvdsfsdaf" href="http://www.google.com">xcvxcv</a>，<a><img src="http://zxvsdfsa" /></a>vxcv'
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
  pageGrab.open encodeURI(task.url), (status) -> 
    gbdate = {}
    if status isnt 'success'
      task.status = 2 #Fail
      task.error = 'Unable to access page'
    else
      pageGrab.injectJs 'jquery.1.10.2.min.js'
      pageGrab.injectJs "grabscripts/#{task.site}_#{task.command}.js"
      gbdate = pageGrab.evaluate ->
        return spGrab()
      task.spend = (Date.now() - now)
    pageGrab.close()
    websocket.evaluate (serverUrl, task, data)->
      _task = JSON.stringify task
      taskHub = $.connection.taskHub
      taskHub.server.doneTask task
      if task.status != 2 #Fail
        _data = JSON.stringify data
        console.log "PostData: " + _data
        console.log "TaskData: " + _task
        #使用signalr有内容长度限制
        $.support.cors = true
        $.post serverUrl + "/task/postdata",
          taskjson: _task
          datajson: _data
    , serverUrl, task, gbdate