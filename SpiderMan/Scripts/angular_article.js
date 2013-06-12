var ArticleListCtrl;

angular.module('spiderman', []).config([
  '$routeProvider', function($routeProvider) {
    var articlesConfig;
    articlesConfig = {
      templateUrl: '/Viewpartials/index.html',
      controller: ArticleListCtrl
    };
    return $routeProvider.when('/articles', articlesConfig).when('/articles/:article', articlesConfig).when('/articles/:article/:inbox', articlesConfig).otherwise({
      redirectTo: '/articles'
    });
  }
]);

ArticleListCtrl = function($scope, $http, $routeParams) {
  if (!$routeParams.article) {
    $scope.article = 'huanle';
  }
  if (!$routeParams.inbox) {
    $scope.inbox = 'inbox';
  }
  $http.get("/api/articles/" + $scope.article + "/" + $scope.inbox).success(function(data) {
    return $scope.articles = data;
  });
  $scope.GoSinglePage = function(articleId) {
    return window.location = '/article/item/' + articleId;
  };
  $scope.GetMore = function(nextIndex) {
    return $http.get("/api/articles/" + $scope.article + "/" + $scope.inbox).success(function(data) {
      return $scope.articles = _.union($scope.articles, data);
    });
  };
};

ArticleListCtrl.$inject = ['$scope', '$http', '$routeParams'];
