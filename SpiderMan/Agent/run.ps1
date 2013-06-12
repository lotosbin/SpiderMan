Param($Code)
if ($Code){
	$Code = "?code=$Code"
}
phantomjs xiuhao.coffee http://qing.xiuhao.com/get_task$Code