window.xhGrab = ->
  data = 
  	ids: []
  $('#center_list_id h2.toolong>a').each ->
    data.ids.push parseInt $(this).attr('href').match(/\d+/g)[0]
  return data