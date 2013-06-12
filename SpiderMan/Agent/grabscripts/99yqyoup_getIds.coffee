window.xhGrab = ->
	data = 
		ids: []
	$("#myform>table>tbody>tr").slice(1).each ->
		data.ids.push $('td', this).eq(1).find('a').attr('href').match(/\d+/g)[0]
	return data