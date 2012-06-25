@echo Removing temporary files (FBICRY)

@del *.resharper.user
@rem del *.resharper
@del *.cache
rem @del /A:H *.suo
@rmdir /S/Q "_ReSharper.CRYFORCE [CRYpto engine]"
@rmdir /S/Q "_ReSharper.RSACryptoPad_CRYFORCE"
@rmdir /S/Q "_ReSharper.CRYFORCE.Test"
@rmdir /S/Q "_ReSharper.EventArgsGeneric"
@rmdir /S/Q "_ReSharper.FBICRYcmd"
@rmdir /S/Q "_ReSharper.HashLib"
@rmdir /S/Q "_ReSharper.HashLibQualityTest"

@del *.xml
@del *.user

@for /D %%i in ("CRYFORCE [CRYpto engine].prj" RSACryptoPad_CRYFORCE.prj CRYFORCE.Test.prj EventArgsGeneric.prj FBICRYcmd.prj) do @(
	@echo Project: %%i
	@cd %%i
	@del *.xml
	@del *.user
	@del *.snk
	@rmdir /S/Q bin
	@rmdir /S/Q obj
	@cd ..
)

@echo Removing temporary files (HashLib)
@cd HashLib.prj

@for /D %%i in (Examples HashLib HashLibOutputDataGenerator HashLibQualityTest HashLibTest ) do @(
	@echo Project: %%i
	@cd %%i
	@del *.xml
	@del *.user
	@del *.snk
	@rmdir /S/Q bin
	@rmdir /S/Q obj
	@cd ..
)