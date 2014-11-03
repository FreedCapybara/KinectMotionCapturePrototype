param(
	[Parameter(Mandatory=$true)][string]$AnimationClassName
)

cd .\src
& 'C:\Program Files\Java\jdk1.7.0_51\bin\javac.exe' -cp ..\AliceLib\alicecore.jar -d ..\build .\edu\calvin\cs\alicekinect\*
cd ..\build
& 'C:\Program Files\Java\jdk1.7.0_51\bin\jar.exe' -cf .\$AnimationClassName.jar .\edu
