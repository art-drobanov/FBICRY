@echo Removing temporary files (FBICRY)

@del *.resharper.user
@rem del *.resharper
@del *.cache
rem @del /A:H *.suo
@rmdir /S/Q "_ReSharper.CRYFORCE [CRYpto engine]"
@rmdir /S/Q "_ReSharper.EventArgs_Generic"
@rmdir /S/Q "_ReSharper.FBICRYcmd"
@rmdir /S/Q "_ReSharper.HashLib"
@rmdir /S/Q "_ReSharper.HashLibQualityTest"

@del *.xml
@del *.user

@for /D %%i in ("CRYFORCE [CRYpto engine].prj" EventArgs_Generic.prj FBICRYcmd.prj) do @(
	@echo Project: %%i
	@cd %%i
	@del *.xml
	@del *.user
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
	@rmdir /S/Q bin
	@rmdir /S/Q obj
	@cd ..
)