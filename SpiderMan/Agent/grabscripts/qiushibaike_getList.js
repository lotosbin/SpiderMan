
window.spGrab = function() {
  var data;
  data = [];
  $('div.col1:first>.block').each(function() {
    var atcul, item, _this;
    _this = $(this);
    atcul = _this.children('.bar').children('ul');
    item = {
      ProviderId: _this.attr('id').match(/\d+/g)[0],
      Content: $.trim(_this.children('.content').html() + $.trim(_this.children('.thumb').html())),
      ThumbUps: $.trim($('li', atcul).first().text()),
      ThumbDowns: $.trim($('li', atcul).eq(1).text())
    };
    return data.push(item);
  });
  return data;
};
