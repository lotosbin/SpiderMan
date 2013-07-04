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
  var articles;
  $scope.$on('$viewContentLoaded', function() {
    return $('#itemlist').height($(window).height() - 20);
  });
  $scope.article = $routeParams.article ? $routeParams.article : 'huanle';
  $scope.boxerOrId = $routeParams.boxerOrId ? $routeParams.boxerOrId : 'verifying';
  $('#menu>li').removeClass('selected').filter('.article-' + $scope.article).addClass('selected');
  articles = [];
  $http.get("/api/" + $scope.article + "/" + $scope.boxerOrId).success(function(data) {
    if (_.isArray(data)) {
      data[0].viewing = true;
      $scope.articles = articles = data;
      return $scope.view = data[0];
    } else {
      $scope.view = data;
      return $scope.viewmodel = 'single';
    }
  });
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
