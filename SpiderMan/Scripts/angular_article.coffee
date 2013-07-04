angular.module('spiderman', []).config(['$routeProvider', ($routeProvider) ->
    articlesConfig = {templateUrl: '/Partials/index.html', controller: ArticleListCtrl}
    $routeProvider
        .when('/', articlesConfig)
        .when('/:article', articlesConfig)
        .when('/:article/:boxerOrId', articlesConfig)
        .otherwise({redirectTo: '/articles'})
])
ArticleListCtrl = ($scope, $http, $routeParams)->
    $scope.$on '$viewContentLoaded', ->
        $('#itemlist').height $(window).height() - 20
    $scope.article = if $routeParams.article then $routeParams.article else 'huanle'
    $scope.boxerOrId = if $routeParams.boxerOrId then $routeParams.boxerOrId else 'verifying'

    $('#menu>li').removeClass('selected').filter('.article-' + $scope.article).addClass 'selected'
    $('#menu>li>a.extand').hide().removeClass('selected')
    if $scope.boxerOrId is 'available' or $scope.boxerOrId is 'disable'
        $('#menu>li.selected>a.extand').show().filter(":contains('#{$scope.boxerOrId.substring(0,3)}')").addClass('selected')

    articles = []
    pager = 0
    $http.get("/api/#{$scope.article}/#{$scope.boxerOrId}").success (data)->
        if _.isArray(data)
            data[0].viewing = true
            $scope.articles = articles = data
            $scope.view = data[0]
            if data.length == 100
                $scope.hasmore = true
        else
            $scope.view = data
            $scope.viewmodel = 'single'
    $scope.Loadmore = ->
        $http.get("/api/#{$scope.article}/#{$scope.boxerOrId}/#{}").success (data)->
            data[0].viewing = true
            $scope.articles = articles = data
            $scope.view = data[0]
            if data.length == 100
                $scope.hasmore = true
    $scope.ViewOne = (articleId)->
        _.each articles, (item)-> item.viewing = false
        one = _.where(articles, {Id: articleId})[0]
        one.viewing = true
        $scope.view = one
    $scope.Checkbox = (articleId)->
        one = _.where(articles, {Id: articleId})[0]
        one.checked = one.checked ? false : true
    $scope.SelectAll = ()->
        _.each articles, (item)-> item.checked = true
    $scope.UnSelectAll = ()->
        _.each articles, (item)-> item.checked = false
    $scope.Available = ()->
        _.each articles, (item)-> 
            if item.checked
                item.Status = 1 #Available
                $.ajax
                    url: "/api/#{$scope.article}/#{item.Id}"
                    type: 'PUT'
                    data: item
                .done ->
                    alert 'ok'
                .fail ->
                    alert 'fail'
    return

ArticleListCtrl.$inject = ['$scope', '$http', '$routeParams'];