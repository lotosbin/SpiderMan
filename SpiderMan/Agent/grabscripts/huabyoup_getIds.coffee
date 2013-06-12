window.xhGrab = ->
  data = 
  	ids: []
  $("article table td.title>a").each ->
    data.ids.push parseInt $(this).attr('href').match(/\d+/g)[0]
  return data