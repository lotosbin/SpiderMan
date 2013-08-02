Param($Name, $ServerUrl)
if (!$Name){
    $Name = "localdev"
}
if (!$ServerUrl){
    $ServerUrl = "http://localhost:35403"
}
phantomjs spiderman.coffee $Name $ServerUrl