window.xhGrab = ->
  text = $('.textArea')
  $('p.time>time', text).remove()
  data = 
    item:
      serial: $('p', text).eq(1).text().match(/\d+/g)[0]
      title: $.trim $('h4', text).text()
      condition: $.trim $('p', text).eq(0).text().split('：')[1]
      type: $.trim $('p', text).eq(2).text().split('：')[1]
      price: $('#currentPrice').text()
      commission: $('#serviceFee').text()
      endTime:  $.trim $('p.time', text).text().split('：')[1]
      bidCount: $('a.tips', text).text().match(/\d+/g)[0]
  _bidRecords = []
  recordTable = $('table.nTable')
  $('tr', recordTable).each (i)->
    if i == 0 then return true
    if i > 3 then return false
    _bidRecords.push
      user: $.trim $('td', this).eq(5).text()
      price: $('td', this).eq(1).text().match(/\d*\.\d{0,2}|\d+/g)[0]
      created: $.trim $('td', this).eq(2).text()
      area: $('td', this).eq(4).text()
  data.item.bidRecords = _bidRecords
  return data