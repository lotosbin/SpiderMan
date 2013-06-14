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
  if (!$routeParams.article) {
    $scope.article = 'huanle';
  }
  if (!$routeParams.boxer) {
    $scope.boxer = 'inbox';
  }
  $http.get("/api/" + $scope.article + "/" + $scope.boxer).success(function(data) {
    if (_.isArray(data)) {
      $scope.articles = data;
      return $scope.view = data[0];
    } else {
      $scope.view = data;
      return $scope.viewmodel = 'single';
    }
  });
  $scope.SinglePage = function(articleId) {};
  $scope.GetMore = function(nextIndex) {
    return $http.get("/api/" + $scope.article + "/" + $scope.inbox).success(function(data) {
      return $scope.articles = _.union($scope.articles, data);
    });
  };
};

ArticleListCtrl.$inject = ['$scope', '$http', '$routeParams'];
