#!/bin/sh
echo -n "Compiling..."
if [ -f RunUO.exe ]; then
	rm RunUO.exe
fi
if [ "$1" = "-d" ]; then
	gmcs /define:DEBUG /define:Framework_3_5 /define:MONO /win32icon:Server/runuo.ico /r:MCommons.dll /r:System.Drawing.dll /r:System.Xml.dll /r:System.Xml.Linq /optimize /unsafe /nowarn:0618 /out:RunUO.exe /recurse:Server/*.cs 
else
	gmcs /define:Framework_3_5 /define:MONO /win32icon:Server/runuo.ico /r:MCommons.dll /r:Ultima.dll /r:System.Drawing.dll /r:System.Xml.dll /r:System.Xml.Linq /optimize /unsafe /nowarn:0618 /out:RunUO.exe /recurse:Server/*.cs 
fi
echo "done!"
