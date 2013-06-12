angular.module('spiderman', []).config(['$routeProvider', ($routeProvider) ->
    articlesConfig = {templateUrl: '/Viewpartials/index.html', controller: ArticleListCtrl}
    $routeProvider
        .when('/articles', articlesConfig)
        .when('/articles/:article', articlesConfig)
        .when('/articles/:article/:inbox', articlesConfig)
        .otherwise({redirectTo: '/articles'})
])
ArticleListCtrl = ($scope, $http, $routeParams)->
    $scope.article = 'huanle' if not $routeParams.article
    $scope.inbox = 'inbox' if not $routeParams.inbox
    $http.get("/api/articles/#{$scope.article}/#{$scope.inbox}").success (data)->
        $scope.articles = data

    $scope.GoSinglePage = (articleId)->
        window.location = '/article/item/' + articleId
    $scope.GetMore = (nextIndex)->
        $http.get("/api/articles/#{$scope.article}/#{$scope.inbox}").success (data)->
            $scope.articles = _.union $scope.articles, data
    return
        
ArticleListCtrl.$inject = ['$scope', '$http', '$routeParams'];