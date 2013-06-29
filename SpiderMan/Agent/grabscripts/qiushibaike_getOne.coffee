window.spGrab = ->
	_this = $('div.col1:first>.block')
	atcul = _this.children('.bar').children('ul')
	item = 
		title: $.trim _this.children('.detail').text()
		content: $.trim _this.children('.content').text()
		creatdate: _this.children('.content').attr('title')
		thumbups: $.trim $('li', atcul).first().text()
		thumbdowns: $.trim $('li', atcul).eq(1).text()
	return item