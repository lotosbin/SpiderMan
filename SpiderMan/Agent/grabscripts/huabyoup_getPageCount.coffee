window.xhGrab = ->
	pagenos = $("div.page>a[pageno]")
	data = 
		count: parseInt pagenos.eq(pagenos.length-2).text()
	return data