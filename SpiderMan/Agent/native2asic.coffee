_ = {}
`
['Arguments', 'Function', 'String', 'Number', 'Date', 'RegExp'].forEach(function(name) {
	_['is' + name] = function(obj) {
	  return toString.call(obj) == '[object ' + name + ']';
	};
})
`

exports.obj2asciiobj = (data)->
	for key of data
		if data.hasOwnProperty(key) 
			data[key] = exports.str2ascii data[key]
	return data

exports.str2ascii = (str)->
	if not _.isString str then return str
	str.replace /[\u007f-\uffff]/g, (c)->
		"zx"+('0000'+c.charCodeAt(0).toString(16)).slice(-4)

# if task.encoding == "gbk"
#   if _.isArray gbdate
#     _.each gbdate, (item, i)->
#       gbdate[i] = nta.obj2asciiobj item
#   else
#     gbdate = nta.obj2asciiobj gbdate

# static readonly MatchEvaluator replacer = m => ((char)int.Parse(m.Groups[1].Value, NumberStyles.AllowHexSpecifier)).ToString();
# if (task.Encoding == "gbk") {
#     datajson = Regex.Replace(datajson, @"zx([a-fA-F0-9]{4})", replacer);
# }