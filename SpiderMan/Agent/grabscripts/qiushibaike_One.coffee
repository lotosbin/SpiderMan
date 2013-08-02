window.spGrab = ->
	_this = $('div.col1:first>.block')
	atcul = _this.children('.bar').children('ul')
	item =
		#Title: $.trim _this.children('.detail').text()
		Content: $.trim _this.children('.content').html()
		#CreatDate: new Date _this.children('.content').attr('title') #now faild, wait phantomjs2.0 fix it
		ThumbUps: $.trim $('li', atcul).first().text()
		ThumbDowns: $.trim $('li', atcul).eq(1).text()
	return item