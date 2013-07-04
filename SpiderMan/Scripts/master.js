
define(function(require, exports, module) {
  require('../front_net/module-zic/form_snippet')($, _, JSON);
  require('../front_net/module/artDialog5/amd/artDialog5');
  require('../front_net/bootstrap/amd/bootstrap-alert');
  require('../front_net/bootstrap/amd/bootstrap-tooltip');
  require('../front_net/bootstrap/amd/bootstrap-popover');
  require('../front_net/bootstrap/amd/bootstrap-dropdown');
  require('../front_net/bootstrap/amd-ext/bootstrap-dropdown-hover');
  require('../front_net/module/ms/jq_validate');
  require('../front_net/module/ms/jq_validate_unobtrusive');
  require('../front_net/module/ms/jq_unobtrusive_ajax');
  require('../front_net/module/noty/amd/jquery.noty')($);
  require('../front_net/module/noty/amd/theme_default')($);
  require('../front_net/module/noty/amd/topCenter')($);
  $.ajaxSetup({
    type: "POST",
    error: function(e) {
      var nowNoty, response;
      if (e.status === 403) {
        exports.dialogin(this);
        return;
      }
      response = JSON.parse(e.responseText);
      if (response.error) {
        $.noty.closeAll();
        return nowNoty = $.notyRenderer.init({
          text: response.error,
          type: 'error',
          timeout: 5000,
          layout: 'topCenter'
        });
      }
    }
  });
  return exports.dialog_config = {
    fixed: true,
    lock: true,
    opacity: 0.1
  };
});
