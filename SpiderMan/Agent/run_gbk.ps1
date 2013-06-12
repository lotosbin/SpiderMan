Param($Code)
if ($Code){
	$Code = "?code=$Code"
}
phantomjs --script-encoding=gbk --output-encoding=gbk xiuhao.coffee http://qing.xiuhao.com/get_task$Code