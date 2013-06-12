window.xhGrab = ->
  form = $('#form1')

  timeSpanbox = $("#timeSpan").parent("td")
  $("#timeSpan").remove()
  timetext = $.trim timeSpanbox.text()
  timetext = timetext.substring 1, timetext.length - 2

  data = 
    item:
      serial: $('#ProductNum1', form).text()
      title: $.trim $('#ProductName1', form).text()
      condition: $('#ProductStep', form).text()
      #type: $('p.status', info).text()
      price: $('#ProductPrice', form).text()
      commission: $('#LBFuwu', form).text()
      endTime: timetext
      bidCount: $('label', form).text()
  _bidRecords = []
  recordTable = $('#tableJPAuction>table', form)
  $('tr', recordTable).slice(0,2).each (i)->
    _bidRecords.push
      user: $('tr:eq(0)', this).text()
      price: $('td:eq(1)', this).text()
      created: $('td:eq(2)', this).text()
      area: $('td:eq(4)', this).text()
  data.item.bidRecords = _bidRecords
  return data