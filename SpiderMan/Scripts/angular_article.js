var ArticleListCtrl, app, calculateLayout, getArticleStatusInt;

app = angular.module('spiderman', []).config([
  '$routeProvider', function($routeProvider) {
    var articlesConfig;
    articlesConfig = {
      templateUrl: '/Partials/index.html',
      controller: ArticleListCtrl
    };
    return $routeProvider.when('/', articlesConfig).when('/:article', articlesConfig).when('/:article/:boxerOrId', articlesConfig).otherwise({
      redirectTo: '/articles'
    });
  }
]);

app.directive('ngShortcut', function() {
  return {
    restrict: 'A',
    replace: true,
    scope: true,
    link: function(scope, iElement, iAttrs) {
      return $(document).on('keypress', function(e) {
        return scope.$apply(scope.keyPressed(e));
      });
    }
  };
});

getArticleStatusInt = function(name) {
  switch (name) {
    case "verifying":
      return 0;
    case "available":
      return 1;
    case "disabled":
      return 2;
    default:
      return 0;
  }
};

calculateLayout = function() {
  return $('#itemlist').height($(window).height() - 20);
};

ArticleListCtrl = function($scope, $http, $routeParams) {
  var AutoSelectFirst, articleType, boxerOrId, pager;
  $scope.$on('$viewContentLoaded', function() {
    var lazyLayout;
    calculateLayout();
    lazyLayout = _.debounce(calculateLayout, 500);
    return $(window).resize(lazyLayout);
  });
  articleType = $routeParams.article ? $routeParams.article : 'ggpttcard';
  boxerOrId = $routeParams.boxerOrId ? $routeParams.boxerOrId : 'verifying';
  $scope.articleStatusInt = getArticleStatusInt(boxerOrId);
  $('#menu>li').removeClass('selected').filter('.article-' + $scope.article).addClass('selected');
  $('#menu>li>a.extand').hide().removeClass('selected');
  if (boxerOrId === 'available' || boxerOrId === 'disable') {
    $('#menu>li.selected>a.extand').show().filter(":contains('" + (boxerOrId.substring(0, 3)) + "')").addClass('selected');
  }
  pager = 0;
  $http.get("/api/" + articleType + "/" + boxerOrId).success(function(data) {
    if (_.isArray(data)) {
      pager = 1;
      data[0].viewing = true;
      $scope.view = data[0];
      $scope.articles = data;
      if (data.length === 30) {
        return $scope.hasmore = true;
      }
    } else {
      $scope.view = data;
      return $scope.viewmodel = 'single';
    }
  });
  $scope.Loadmore = function() {
    return $http.get("/api/" + articleType + "/" + boxerOrId + "/" + pager).success(function(data) {
      $scope.articles = _.union($scope.articles, data);
      pager++;
      if ($scope.articles.length >= pager * 30) {
        return $scope.hasmore = true;
      }
    });
  };
  $scope.ViewOne = function(articleId) {
    var one;
    _.each($scope.articles, function(item) {
      return item.viewing = false;
    });
    one = _.where($scope.articles, {
      Id: articleId
    })[0];
    one.viewing = true;
    return $scope.view = one;
  };
  $scope.Checkbox = function(articleId) {
    var one, _ref;
    one = _.where($scope.articles, {
      Id: articleId
    })[0];
    return one.checked = (_ref = one.checked) != null ? _ref : {
      "false": true
    };
  };
  $scope.SelectAll = function() {
    return _.each($scope.articles, function(item) {
      return item.checked = true;
    });
  };
  $scope.UnSelectAll = function() {
    return _.each($scope.articles, function(item) {
      return item.checked = false;
    });
  };
  AutoSelectFirst = function() {
    var lis;
    lis = $('#itemlist>li:visible');
    if (!lis.filter('.viewing').size()) {
      return lis.first().click();
    }
  };
  $scope.Available = function() {
    var PushToAjaxlist, ajaxlist;
    ajaxlist = [];
    PushToAjaxlist = function(item) {
      item.Status = 1;
      return ajaxlist.push($.ajax({
        url: "/api/" + articleType + "/" + item.Id,
        type: 'PUT',
        data: item
      }));
    };
    _.each($scope.articles, function(item) {
      if (item.checked) {
        return PushToAjaxlist(item);
      }
    });
    if (ajaxlist.length === 0) {
      PushToAjaxlist($scope.view);
    }
    return $.when.apply($, ajaxlist).done(function() {
      return AutoSelectFirst();
    }).fail(function() {
      return alert('fail!');
    });
  };
  $scope.Disabled = function() {
    var PushToAjaxlist, ajaxlist;
    ajaxlist = [];
    PushToAjaxlist = function(item) {
      item.Status = 2;
      return ajaxlist.push($.ajax({
        url: "/api/" + articleType + "/" + item.Id,
        type: 'PUT',
        data: item
      }));
    };
    _.each($scope.articles, function(item) {
      if (item.checked) {
        return PushToAjaxlist(item);
      }
    });
    if (ajaxlist.length === 0) {
      PushToAjaxlist($scope.view);
    }
    return $.when.apply($, ajaxlist).done(function() {
      return AutoSelectFirst();
    }).fail(function() {
      return alert('fail!');
    });
  };
  $scope.GoVerify = function() {
    var PushToAjaxlist, ajaxlist;
    ajaxlist = [];
    PushToAjaxlist = function(item) {
      item.Status = 0;
      return ajaxlist.push($.ajax({
        url: "/api/" + articleType + "/" + item.Id,
        type: 'PUT',
        data: item
      }));
    };
    _.each($scope.articles, function(item) {
      if (item.checked) {
        return PushToAjaxlist(item);
      }
    });
    if (ajaxlist.length === 0) {
      PushToAjaxlist($scope.view);
    }
    return $.when.apply($, ajaxlist).done(function() {
      return AutoSelectFirst();
    }).fail(function() {
      return alert('fail!');
    });
  };
  $scope.Delete = function() {
    var PushToAjaxlist, ajaxlist;
    ajaxlist = [];
    PushToAjaxlist = function(item) {
      ajaxlist.push($.ajax({
        url: "/api/" + articleType + "/" + item.Id,
        type: 'Delete'
      }));
      return $scope.articles = _.without($scope.articles, item);
    };
    _.each($scope.articles, function(item) {
      if (item.checked) {
        return PushToAjaxlist(item);
      }
    });
    if (ajaxlist.length === 0) {
      PushToAjaxlist($scope.view);
    }
    return $.when.apply($, ajaxlist).done(function() {
      return AutoSelectFirst();
    }).fail(function() {
      return alert('fail!');
    });
  };
  $scope.keyPressed = function(e) {
    switch (e.which) {
      case 97:
        return $scope.Available();
      case 100:
        return $scope.Disabled();
      case 118:
        return $scope.GoVerify();
      case 101:
        return $scope.Delete();
    }
  };
};

ArticleListCtrl.$inject = ['$scope', '$http', '$routeParams'];
