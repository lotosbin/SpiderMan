#!/bin/bash
code="";
if [ -n $1 ] 
then
	code="?code=$1"
fi
url="http://qing.xiuhao.com/get_task$code"
phantomjs xiuhao.coffee  $url
