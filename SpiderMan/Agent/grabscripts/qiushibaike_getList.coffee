window.spGrab = ->
	data = []
	$('div.col1:first>.block').slice(2,3).each ->
		_this = $(this)
		atcul = _this.children('.bar').children('ul')
		item = 
			title: $.trim _this.children('.detail').text()
			content: $.trim _this.children('.content').text()
			creatdate: _this.children('.content').attr('title')
			thumbups: $.trim $('li', atcul).first().text()
			thumbdowns: $.trim $('li', atcul).eq(1).text()
		data.push item
	return data