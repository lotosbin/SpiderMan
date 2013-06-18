exports.obj2asciiobj = (data)->
	for key of data
		if data.hasOwnProperty(key) 
			data[key] = exports.str2ascii data[key]
	return data

exports.str2ascii = (str)->
	str.replace /[\u007f-\uffff]/g, (c)->
		'\u'+('0000'+c.charCodeAt(0).toString(16)).slice(-4)