window.xhGrab = ->
  info = $('#info')
  data = 
    item:
      category: $('#breadcrumb a:first').text()
      serial: $('p.No', info).text().substr(1)
      title: $.trim $('span.name', info).text()
      condition: $('span.character', info).text()
      type: $.trim $('p.status', info).text()
      price: $('p.currentPrice', info).text()
      commission: $('span.servicePrice', info).text()
      endTime: $.trim $('p.time', info).text().split('&nbsp;')[0]
      bidCount: $('span.bidTimes', info).text()
  _bidRecords = []
  recordTable = $('table.recordTable', info)
  $('tr', recordTable).each (i)->
    if i > 2 then return false
    _bidRecords.push
      user: $.trim $('.r_nickName', this).text()
      price: $('td:eq(1)', this).text()
      created: $('td:eq(2)', this).text()
      area: $('td.toolong', this).text()
  data.item.bidRecords = _bidRecords
  return data