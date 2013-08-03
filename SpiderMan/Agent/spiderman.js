﻿var CastTesk, agentName, serverUrl, system, websocket, _;

system = require("system");

if (system.args.length !== 3) {
  console.log('serverUrl and agentName is necessary!');
  phantom.exit(1);
}

agentName = system.args[1];

serverUrl = system.args[2];

_ = require('./underscore-min');

websocket = require("webpage").create();

websocket.settings.localToRemoteUrlAccessEnabled = true;

websocket.onConsoleMessage = function(info) {
  return console.log("~Websocket_Console: " + info);
};

websocket.onCallback = function(info) {
  switch (info.command) {
    case "CastTesk":
      return CastTesk(info.task);
  }
};

websocket.injectJs('./jquery.1.10.2.min.js');

websocket.injectJs('./jquery.signalR-1.1.2.min.js');

websocket.includeJs(serverUrl + '/signalr/hubs', function() {
  return websocket.evaluate(function(serverUrl, agentName) {
    var taskHub;
    $.support.cors = false;
    $.connection.hub.url = serverUrl + '/signalr';
    taskHub = $.connection.taskHub;
    $.connection.hub.start().done(function() {
      $.support.cors = true;
      return taskHub.server.registerAgent(agentName);
    });
    return taskHub.client.castTesk = function(task) {
      var castTesk;
      castTesk = {
        command: "CastTesk",
        task: task
      };
      return window.callPhantom(castTesk);
    };
  }, serverUrl, agentName);
});

CastTesk = function(task) {
  var now, pageGrab;
  console.log("~CastTesk: " + JSON.stringify(task));
  pageGrab = require("webpage").create();
  pageGrab.settings.userAgent = 'Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/27.0.1453.93 Safari/537.36';
  pageGrab.settings.loadImages = false;
  pageGrab.onError = function(msg, trace) {
    var msgStack;
    msgStack = [msg];
    if (trace) {
      trace.forEach(function(t) {
        var _ref;
        return msgStack.push(" -> " + t.file + ": " + t.line + ((_ref = t["function"]) != null ? _ref : " (in function '" + t["function"] + {
          "')": ""
        }));
      });
    }
    return console.log('~EvaluateError: ' + msgStack.join("\n"));
  };
  now = Date.now();
  return pageGrab.open(task.url, function(status) {
    var gbdate;
    gbdate = {};
    if (status !== 'success') {
      task.status = 2;
      task.error = 'Unable to access page';
    } else {
      pageGrab.injectJs('jquery.1.10.2.min.js');
      pageGrab.injectJs("grabscripts/" + task.site + "_" + task.commandtype + ".js");
      gbdate = pageGrab.evaluate(function() {
        return spGrab();
      });
      gbdate.GrabDate = now;
      task.spend = (Date.now() - now) / 1000;
      if (!gbdate) {
        task.status = 2;
        task.error = 'gbdate is false';
      } else {
        task.status = 3;
      }
    }
    pageGrab.close();
    return websocket.evaluate(function(serverUrl, task, data) {
      var taskHub, _data, _posturl;
      taskHub = $.connection.taskHub;
      taskHub.server.doneTask(task);
      if (task.status !== 2) {
        _data = JSON.stringify(data);
        _posturl = serverUrl + "/task/post" + task.articletype + task.commandtype;
        if (task.commandtype === "Ids" || task.commandtype === "ListFirst") {
          return $.post(_posturl, {
            datajson: _data,
            taskmodelid: task.taskmodelid
          });
        } else {
          return $.post(_posturl, {
            datajson: _data
          });
        }
      }
    }, serverUrl, task, gbdate);
  });
};
