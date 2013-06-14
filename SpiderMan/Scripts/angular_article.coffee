angular.module('spiderman', []).config(['$routeProvider', ($routeProvider) ->
    articlesConfig = {templateUrl: '/Partials/index.html', controller: ArticleListCtrl}
    $routeProvider
        .when('/', articlesConfig)
        .when('/:article', articlesConfig)
        .when('/:article/:boxer', articlesConfig)
        .otherwise({redirectTo: '/articles'})
])
ArticleListCtrl = ($scope, $http, $routeParams)->
    $scope.article = 'huanle' if not $routeParams.article
    $scope.boxer = 'inbox' if not $routeParams.boxer
    $http.get("/api/#{$scope.article}/#{$scope.boxer}").success (data)->
        if _.isArray(data)
            $scope.articles = data
            $scope.view = data[0]
        else
            $scope.view = data
            $scope.viewmodel = 'single'

    $scope.SinglePage = (articleId)->
        #window.location = '/article/item/' + articleIda
    $scope.GetMore = (nextIndex)->
        $http.get("/api/#{$scope.article}/#{$scope.inbox}").success (data)->
            $scope.articles = _.union $scope.articles, data
    return
        
ArticleListCtrl.$inject = ['$scope', '$http', '$routeParams'];