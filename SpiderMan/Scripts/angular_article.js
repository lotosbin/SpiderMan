var ArticleListCtrl;

angular.module('spiderman', []).config([
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

ArticleListCtrl = function($scope, $http, $routeParams) {
  var articles, pager;
  $scope.$on('$viewContentLoaded', function() {
    return $('#itemlist').height($(window).height() - 20);
  });
  $scope.article = $routeParams.article ? $routeParams.article : 'huanle';
  $scope.boxerOrId = $routeParams.boxerOrId ? $routeParams.boxerOrId : 'verifying';
  $('#menu>li').removeClass('selected').filter('.article-' + $scope.article).addClass('selected');
  $('#menu>li>a.extand').hide().removeClass('selected');
  if ($scope.boxerOrId === 'available' || $scope.boxerOrId === 'disable') {
    $('#menu>li.selected>a.extand').show().filter(":contains('" + ($scope.boxerOrId.substring(0, 3)) + "')").addClass('selected');
  }
  articles = [];
  pager = 0;
  $http.get("/api/" + $scope.article + "/" + $scope.boxerOrId).success(function(data) {
    if (_.isArray(data)) {
      data[0].viewing = true;
      $scope.articles = articles = data;
      $scope.view = data[0];
      if (data.length === 100) {
        return $scope.hasmore = true;
      }
    } else {
      $scope.view = data;
      return $scope.viewmodel = 'single';
    }
  });
  $scope.Loadmore = function() {
    return $http.get("/api/" + $scope.article + "/" + $scope.boxerOrId + "/").success(function(data) {
      data[0].viewing = true;
      $scope.articles = articles = data;
      $scope.view = data[0];
      if (data.length === 100) {
        return $scope.hasmore = true;
      }
    });
  };
  $scope.ViewOne = function(articleId) {
    var one;
    _.each(articles, function(item) {
      return item.viewing = false;
    });
    one = _.where(articles, {
      Id: articleId
    })[0];
    one.viewing = true;
    return $scope.view = one;
  };
  $scope.Checkbox = function(articleId) {
    var one, _ref;
    one = _.where(articles, {
      Id: articleId
    })[0];
    return one.checked = (_ref = one.checked) != null ? _ref : {
      "false": true
    };
  };
  $scope.SelectAll = function() {
    return _.each(articles, function(item) {
      return item.checked = true;
    });
  };
  $scope.UnSelectAll = function() {
    return _.each(articles, function(item) {
      return item.checked = false;
    });
  };
  $scope.Available = function() {
    return _.each(articles, function(item) {
      if (item.checked) {
        item.Status = 1;
        return $.ajax({
          url: "/api/" + $scope.article + "/" + item.Id,
          type: 'PUT',
          data: item
        }).done(function() {
          return alert('ok');
        }).fail(function() {
          return alert('fail');
        });
      }
    });
  };
};

ArticleListCtrl.$inject = ['$scope', '$http', '$routeParams'];
