Alter PROCEDURE SearchAccounts
    @index int,   
    @searchTerm nvarchar(150),
	@resultsCount int,
	@skipRows int
AS   
    SET NOCOUNT ON;  
	declare @condition nvarchar(250),
			@query nvarchar(2048)
	set @query = 'SELECT distinct '
	select @condition = '1=1'
	if(@index = 1)
		Begin
			set @condition = 'ACTNUMBR_1 like '''+ @searchTerm+'%'''
			set @query = @query + 
			' 0 TemplateID
				,0 DepartmentID
				,0 [Order]
				,LTRIM(RTRIM([ACTNUMBR_1])) Fund
				,'''' Dept
				,'''' Program
				,'''' Project
				,'''' BaseElementObjectDetail
				,'''' Description'
		End
	else 
	if(@index = 2)
		Begin
			set @condition = 'ACTNUMBR_2 like '''+ @searchTerm+'%'''
			set @query = @query + 
			' 0 TemplateID
				,0 DepartmentID
				,0 [Order]
				,'''' Fund
				,LTRIM(RTRIM([ACTNUMBR_2])) Dept
				,'''' Program
				,'''' Project
				,'''' BaseElementObjectDetail
				,'''' Description'
		End
	else 
	if(@index = 3)
		Begin
			set @condition = 'ACTNUMBR_3 like '''+ @searchTerm+'%'''
			set @query = @query + 
			' 0 TemplateID
				,0 DepartmentID
				,0 [Order]
				,'''' Fund
				,'''' Dept
				,LTRIM(RTRIM([ACTNUMBR_3])) Program
				,'''' Project
				,'''' BaseElementObjectDetail
				,'''' Description'
		End
	else 
	if(@index = 4)
		Begin
			set @condition = 'ACTNUMBR_4 like '''+ @searchTerm+'%'''
			set @query = @query + 
			' 0 TemplateID
				,0 DepartmentID
				,0 [Order]
				,'''' Fund
				,'''' Dept
				,'''' Program
				,LTRIM(RTRIM([ACTNUMBR_4])) Project
				,'''' BaseElementObjectDetail
				,'''' Description'
		End
	else if(@index = 5)
		Begin
			set @condition = 'ACTNUMBR_5 like '''+ @searchTerm+'%'''
			set @query = @query + 
			' 0 TemplateID
				,0 DepartmentID
				,0 [Order]
				,'''' Fund
				,'''' Dept
				,'''' Program
				,'''' Project
				,LTRIM(RTRIM([ACTNUMBR_5])) BaseElementObjectDetail
				,'''' Description'
		End
    else if(@index = 6)
		BEGIN
			set @condition = 'ACTDESCR like ''%'+ @searchTerm+'%'''
			set @query = @query + 
			' [ACTINDX] TemplateID
				,0 DepartmentID
				,0 [Order]
				,LTRIM(RTRIM([ACTNUMBR_1])) Fund
				,LTRIM(RTRIM([ACTNUMBR_2])) Dept
				,LTRIM(RTRIM([ACTNUMBR_3])) Program
				,LTRIM(RTRIM([ACTNUMBR_4])) Project
				,LTRIM(RTRIM([ACTNUMBR_5])) BaseElementObjectDetail
				,''['' + LTRIM(RTRIM([ACTNUMBR_1]))+ ''.'' + LTRIM(RTRIM([ACTNUMBR_2]))+ ''.'' + LTRIM(RTRIM([ACTNUMBR_3]))+ ''.'' + 
				LTRIM(RTRIM([ACTNUMBR_4]))+ ''.'' +LTRIM(RTRIM([ACTNUMBR_5]))+ ''] '' 
				+LTRIM(RTRIM([ACTDESCR])) Description'
		END
	else 
		set @query = @query + 
			' [ACTINDX] TemplateID
				,0 DepartmentID
				,0 [Order]
				,LTRIM(RTRIM([ACTNUMBR_1])) Fund
				,LTRIM(RTRIM([ACTNUMBR_2])) Dept
				,LTRIM(RTRIM([ACTNUMBR_3])) Program
				,LTRIM(RTRIM([ACTNUMBR_4])) Project
				,LTRIM(RTRIM([ACTNUMBR_5])) BaseElementObjectDetail
				,LTRIM(RTRIM([ACTDESCR])) Description'

	set @query = @query + ' FROM [GC].[dbo].[GL00100]
	WHERE Active = 1 and '+@condition +
	' ORDER BY TemplateID OFFSET '+CAST(@skipRows AS NVARCHAR(10))+' ROWS FETCH NEXT '+CAST(@resultsCount AS NVARCHAR(10))+' ROWS ONLY'
	--EXECUTE(@query)
	--print @query
	EXECUTE sp_executesql @query
GO 

--exec SearchAccounts @index = 6, @searchTerm = N'MOBILE HOME FEES', @resultsCount = 200, @skipRows = 0
--exec SearchAccounts @index = 6, @searchTerm = N'BUILDING/FIRE MARSHAL', @resultsCount = 200, @skipRows = 0
--exec SearchAccounts @index = 5, @searchTerm = N'5486', @resultsCount = 200, @skipRows = 0
--exec SearchAccounts @index = 5, @searchTerm = N'5486', @resultsCount = 200, @skipRows = 20
--exec SearchAccounts @index = 4, @searchTerm = N'91', @resultsCount = 2000, @skipRows = 0
--exec SearchAccounts @index = 7, @searchTerm = N'91', @resultsCount = 1000000, @skipRows = 0
