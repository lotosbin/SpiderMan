
window.spGrab = function() {
  var data;
  data = [];
  $('div.col1:first>.block').each(function() {
    var atcul, item, _this;
    _this = $(this);
    atcul = _this.children('.bar').children('ul');
    item = {
      Title: $.trim(_this.children('.detail').text()),
      Content: $.trim(_this.children('.content').html()),
      ThumbUps: $.trim($('li', atcul).first().text()),
      ThumbDowns: $.trim($('li', atcul).eq(1).text())
    };
    return data.push(item);
  });
  return data;
};
