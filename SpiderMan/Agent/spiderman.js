var CastTesk, agentName, serverUrl, system, websocket, _;

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
      $.post(serverUrl + "/task/postdata", {
        taskjson: '{"articletype":0,"command":"getList","commandtype":1,"error":"","id":"48f2caef-cec5-4236-875d-85defd1fcbc1","site":"qiushibaike","spend":2779,"status":1,"url":"zxczxczxc"}',
        datajson: '<img src="http://zxvsdfsa" />asda<p>sadasd</p>sdasd<a title="cvdsfsdaf" href="http://www.google.com">xcvxcv</a>，<a><img src="http://zxvsdfsa" /></a>vxcv'
      });
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
  return pageGrab.open(encodeURI(task.url), function(status) {
    var gbdate;
    gbdate = {};
    if (status !== 'success') {
      task.status = 2;
      task.error = 'Unable to access page';
    } else {
      pageGrab.injectJs('jquery.1.10.2.min.js');
      pageGrab.injectJs("grabscripts/" + task.site + "_" + task.command + ".js");
      gbdate = pageGrab.evaluate(function() {
        return spGrab();
      });
      task.spend = Date.now() - now;
    }
    pageGrab.close();
    return websocket.evaluate(function(serverUrl, task, data) {
      var taskHub, _data, _task;
      _task = JSON.stringify(task);
      taskHub = $.connection.taskHub;
      taskHub.server.doneTask(task);
      if (task.status !== 2) {
        _data = JSON.stringify(data);
        console.log("PostData: " + _data);
        console.log("TaskData: " + _task);
        $.support.cors = true;
        return $.post(serverUrl + "/task/postdata", {
          taskjson: _task,
          datajson: _data
        });
      }
    }, serverUrl, task, gbdate);
  });
};
