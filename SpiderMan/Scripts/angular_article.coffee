angular.module('spiderman', []).config(['$routeProvider', ($routeProvider) ->
    articlesConfig = {templateUrl: '/Partials/index.html', controller: ArticleListCtrl}
    $routeProvider
        .when('/', articlesConfig)
        .when('/:article', articlesConfig)
        .when('/:article/:boxer', articlesConfig)
        .otherwise({redirectTo: '/articles'})
])
ArticleListCtrl = ($scope, $http, $routeParams)->
    $scope.article = if $routeParams.article then $routeParams.article else 'huanle'
    $scope.boxer = if $routeParams.boxer then $routeParams.boxer else 'verifying'
    articles = []
    $http.get("/api/#{$scope.article}/#{$scope.boxer}").success (data)->
        if _.isArray(data)
            $scope.articles = articles = data
            $scope.view = data[0]
        else
            $scope.view = data
            $scope.viewmodel = 'single'

    $scope.ViewOne = (articleId)->
        $scope.view = _.where(articles, {id: articleId})[0]
    $scope.GetMore = (nextIndex)->
        $http.get("/api/#{$scope.article}/#{$scope.inbox}").success (data)->
            $scope.articles = _.union $scope.articles, data
    return

ArticleListCtrl.$inject = ['$scope', '$http', '$routeParams'];