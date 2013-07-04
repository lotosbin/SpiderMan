﻿var _;

_ = {};


['Arguments', 'Function', 'String', 'Number', 'Date', 'RegExp'].forEach(function(name) {
	_['is' + name] = function(obj) {
	  return toString.call(obj) == '[object ' + name + ']';
	};
})
;


exports.obj2asciiobj = function(data) {
  var key;
  for (key in data) {
    if (data.hasOwnProperty(key)) {
      data[key] = exports.str2ascii(data[key]);
    }
  }
  return data;
};

exports.str2ascii = function(str) {
  if (!_.isString(str)) {
    return str;
  }
  return str.replace(/[\u007f-\uffff]/g, function(c) {
    return "zx" + ('0000' + c.charCodeAt(0).toString(16)).slice(-4);
  });
};
