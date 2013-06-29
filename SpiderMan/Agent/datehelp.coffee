exports.DisplayNowDate = ->
  time = new Date()
  yyyy = time.getFullYear()
  mm = time.getMonth()+1
  dd = time.getDate()
  hour = time.getHours()
  minute = time.getMinutes()
  seconds = time.getSeconds()
  return yyyy+'-'+mm+'-'+dd+' '+hour+':'+minute+':'+seconds