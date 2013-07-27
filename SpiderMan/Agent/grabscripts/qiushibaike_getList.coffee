window.spGrab = ->
	data = []
	$('div.col1:first>.block').each ->
		_this = $(this)
		atcul = _this.children('.bar').children('ul')
		item = 
			ProviderId: _this.attr('id').match(/\d+/g)[0]
			Content: $.trim _this.children('.content').html() + $.trim _this.children('.thumb').html()
			#BrithDate: new Date _this.children('.content').attr('title') #now faild, wait phantomjs2.0 fix it
			ThumbUps: $.trim $('li', atcul).first().text()
			ThumbDowns: $.trim $('li', atcul).eq(1).text()
		data.push item
	return data