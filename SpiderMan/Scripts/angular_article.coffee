app = angular.module('spiderman', []).config(['$routeProvider', ($routeProvider) ->
    articlesConfig = {templateUrl: '/Partials/index.html', controller: ArticleListCtrl}
    $routeProvider
        .when('/', articlesConfig)
        .when('/:article', articlesConfig)
        .when('/:article/:boxerOrId', articlesConfig)
        .otherwise({redirectTo: '/articles'})
])
app.directive 'ngShortcut', -> #http://plnkr.co/edit/cBHOuHyrHz1Gk7zzYVX8
    restrict: 'A',
    replace: true,
    scope: true,
    link: (scope, iElement, iAttrs)->
        $(document).on 'keypress', (e)->
            scope.$apply scope.keyPressed(e)

getArticleStatusInt = (name)->
    switch name
        when "verifying" then 0
        when "available" then 1
        when "disabled" then 2
        else 0

calculateLayout = ->
    $('#itemlist').height $(window).height() - 20
ArticleListCtrl = ($scope, $http, $routeParams)->
    $scope.$on '$viewContentLoaded', ->
        calculateLayout()
        lazyLayout = _.debounce calculateLayout, 500
        $(window).resize lazyLayout
    articleType = if $routeParams.article then $routeParams.article else 'huanle'
    boxerOrId = if $routeParams.boxerOrId then $routeParams.boxerOrId else 'verifying'
    $scope.articleStatusInt = getArticleStatusInt boxerOrId

    $('#menu>li').removeClass('selected').filter('.article-' + $scope.article).addClass 'selected'
    $('#menu>li>a.extand').hide().removeClass('selected')
    if boxerOrId is 'available' or boxerOrId is 'disable'
        $('#menu>li.selected>a.extand').show().filter(":contains('#{boxerOrId.substring(0,3)}')").addClass('selected')

    pager = 0
    $http.get("/api/#{articleType}/#{boxerOrId}").success (data)->
        if _.isArray(data)
            data[0].viewing = true
            $scope.articles = data
            $scope.view = data[0]
            if data.length == 30
                $scope.hasmore = true
        else
            $scope.view = data
            $scope.viewmodel = 'single'
    $scope.Loadmore = ->
        $http.get("/api/#{articleType}/#{boxerOrId}/#{pager}").success (data)->
            data[0].viewing = true
            $scope.articles = data
            $scope.view = data[0]
            if data.length == 100
                $scope.hasmore = true
    $scope.ViewOne = (articleId)->
        _.each $scope.articles, (item)-> item.viewing = false
        one = _.where($scope.articles, {Id: articleId})[0]
        one.viewing = true
        $scope.view = one
    $scope.Checkbox = (articleId)->
        one = _.where($scope.articles, {Id: articleId})[0]
        one.checked = one.checked ? false : true
    $scope.SelectAll = ->
        _.each $scope.articles, (item)-> item.checked = true
    $scope.UnSelectAll = ->
        _.each $scope.articles, (item)-> item.checked = false

    AutoSelectFirst = ->
        lis = $('#itemlist>li:visible')
        if not lis.filter('.viewing').size() then lis.first().click()
    $scope.Available = ->
        ajaxlist = []
        PushToAjaxlist = (item)->
            item.Status = 1 #Available
            ajaxlist.push $.ajax
                url: "/api/#{articleType}/#{item.Id}"
                type: 'PUT'
                data: item
        _.each $scope.articles, (item)-> 
            if item.checked then PushToAjaxlist item
        if ajaxlist.length == 0 then PushToAjaxlist $scope.view
        $.when.apply($, ajaxlist).done(-> AutoSelectFirst()).fail(-> alert 'fail!')
    $scope.Disabled = ->
        ajaxlist = []
        PushToAjaxlist = (item)->
            item.Status = 2 #Disabled
            ajaxlist.push $.ajax
                url: "/api/#{articleType}/#{item.Id}"
                type: 'PUT'
                data: item
        _.each $scope.articles, (item)-> 
            if item.checked then PushToAjaxlist item
        if ajaxlist.length == 0 then PushToAjaxlist $scope.view
        $.when.apply($, ajaxlist).done(-> AutoSelectFirst()).fail(-> alert 'fail!')
    $scope.GoVerify = ->
        ajaxlist = []
        PushToAjaxlist = (item)->
            item.Status = 0 #Disabled
            ajaxlist.push $.ajax
                url: "/api/#{articleType}/#{item.Id}"
                type: 'PUT'
                data: item
        _.each $scope.articles, (item)-> 
            if item.checked then PushToAjaxlist item
        if ajaxlist.length == 0 then PushToAjaxlist $scope.view
        $.when.apply($, ajaxlist).done(-> AutoSelectFirst()).fail(-> alert 'fail!')
    $scope.Delete = ->
        ajaxlist = []
        PushToAjaxlist = (item)->
            ajaxlist.push $.ajax
                url: "/api/#{articleType}/#{item.Id}"
                type: 'Delete'
            $scope.articles = _.without $scope.articles, item
        _.each $scope.articles, (item)-> 
            if item.checked then PushToAjaxlist item
        if ajaxlist.length == 0 then PushToAjaxlist $scope.view
        $.when.apply($, ajaxlist).done(-> AutoSelectFirst()).fail(-> alert 'fail!')

    $scope.keyPressed = (e)->
        #alert e.which
        switch e.which
            when 97 then $scope.Available() #a
            when 100 then $scope.Disabled() #d
            when 118 then $scope.GoVerify() #v
            when 101 then $scope.Delete() #e

    return

ArticleListCtrl.$inject = ['$scope', '$http', '$routeParams'];