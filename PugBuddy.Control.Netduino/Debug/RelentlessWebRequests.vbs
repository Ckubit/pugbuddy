errorCount=0
intHighNumber = 0
intLowNumber = 60


   
do while 1>0
'on error resume next


'-----------------------------------------------------------------------------------------------------------------------------------
Randomize 
intNumber = Int((intHighNumber - intLowNumber + 1) * Rnd + intLowNumber)

set objNetwork = createobject("wscript.network")
Set objExplorer = wscript.CreateObject ("InternetExplorer.Application")
step1="http://192.168.1.12:8080/Forward&0&0&Right&" & intNumber & "&" & intNumber

CookiePath1 = "C:\Documents and Settings\" & objNetwork.Username & "\Local Settings\Temporary Internet Files\"
'CookiePath2 = "C:\Documents and Settings\" & objNetwork.Username & "\Cookies\"
CookiePath2 = "C:\Documents and Settings\ckubit\Cookies\"
logout=false
Quit=true
objExplorer.Visible = 1
Dirpath=CookiePath1
call clearCookies (DirPath)
Dirpath=CookiePath2


wscript.echo step1

'----------------Step 0--------------------------------------------------------------------------------------------------------------	
objExplorer.Navigate "about:blank" 
maxBusyState=10
BusyState=0
do while objExplorer.Busy
   wscript.sleep 200
   wscript.echo "obj .Busy"
   if BusyState >= maxBusyState then
			wscript.echo "exiting do - BusyState"
		   objExplorer.Quit()
		   exit do
   end if
   BusyState=BusyState+1
loop

'----------------Step 1--------------------------------------------------------------------------------------------------------------	
objExplorer.Navigate step1
maxReadyState=10
ReadyState=0
do while objExplorer.READYSTATE < 4
   wscript.sleep 500
   if ReadyState >= maxReadyState then
			wscript.echo "exiting do - Readystate"
           ' wscript.echo objExplorer.document.nodeName
		    wscript.sleep 200
           objExplorer.Quit()
		   exit do
   end if
   ReadyState=ReadyState+1
   wscript.echo "Readystate < 4  "
loop
wscript.sleep 500
wscript.echo objExplorer.statustext & ", " & objExplorer.LocationName



Dirpath=CookiePath1
call clearCookies (DirPath)


if Quit=true then
   objExplorer.Quit()
end if


wscript.sleep 200
loop

sub clearCookies(DirPath)
    on error resume next
	Set wshShell = WScript.CreateObject ("WSCript.shell")
	wshshell.run "iecache.exe /delete", 6, True
	set wshshell = nothing
end sub






