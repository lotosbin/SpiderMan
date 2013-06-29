var ArticleListCtrl;

angular.module('spiderman', []).config([
  '$routeProvider', function($routeProvider) {
    var articlesConfig;
    articlesConfig = {
      templateUrl: '/Partials/index.html',
      controller: ArticleListCtrl
    };
    return $routeProvider.when('/', articlesConfig).when('/:article', articlesConfig).when('/:article/:boxer', articlesConfig).otherwise({
      redirectTo: '/articles'
    });
  }
]);

ArticleListCtrl = function($scope, $http, $routeParams) {
  var articles;
  $scope.article = $routeParams.article ? $routeParams.article : 'huanle';
  $scope.boxer = $routeParams.boxer ? $routeParams.boxer : 'verifying';
  articles = [];
  $http.get("/api/" + $scope.article + "/" + $scope.boxer).success(function(data) {
    if (_.isArray(data)) {
      $scope.articles = articles = data;
      return $scope.view = data[0];
    } else {
      $scope.view = data;
      return $scope.viewmodel = 'single';
    }
  });
  $scope.ViewOne = function(articleId) {
    return $scope.view = _.where(articles, {
      id: articleId
    })[0];
  };
  $scope.GetMore = function(nextIndex) {
    return $http.get("/api/" + $scope.article + "/" + $scope.inbox).success(function(data) {
      return $scope.articles = _.union($scope.articles, data);
    });
  };
};

ArticleListCtrl.$inject = ['$scope', '$http', '$routeParams'];
