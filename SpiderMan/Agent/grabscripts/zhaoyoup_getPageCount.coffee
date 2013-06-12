window.xhGrab = ->
	data = 
		count: parseInt $("#page_div>li:not('.total_count'):last").text()
	return data