Alter PROCEDURE SearchAccounts
    @index int,   
    @searchTerm nvarchar(150),
	@resultsCount int,
	@skipRows int
AS   
    SET NOCOUNT ON;  
	declare @condition nvarchar(250),
			@secCondition nvarchar(250),
			@query nvarchar(2048),
			@secQuery nvarchar(2048),
			@unionQuery nvarchar(2048)
	select	@query = 'SELECT distinct ', 
			@secQuery = 'SELECT distinct '

	select	@condition = '1=1',
			@secCondition = '1=1'

	if(@index = 1)
		Begin
			select	@condition = 'ACTNUMBR_1 like '''+ @searchTerm+'%''',
					@secCondition = 'SUBSTRING(ACTNUMBR_1, 1, 3) like '''+ @searchTerm+'%'''
			set @query = @query + 
			' 0 TemplateID
				,0 DepartmentID
				,0 [Order]
				,0 [DataSource]
				,LTRIM(RTRIM([ACTNUMBR_1])) Fund
				,'''' Dept
				,'''' Program
				,'''' Project
				,'''' BaseElementObjectDetail
				,'''' Description'
			
			set @secQuery = @secQuery + 
			' 0 TemplateID
				,0 DepartmentID
				,0 [Order]
				,0 [DataSource]
				,SUBSTRING(ACTNUMBR_1, 1, 3) Fund
				,'''' Dept
				,'''' Program
				,'''' Project
				,'''' BaseElementObjectDetail
				,'''' Description'
		End
	else 
	if(@index = 2)
		Begin
			select  @condition = 'ACTNUMBR_2 like '''+ @searchTerm+'%''',
					@secCondition = 'SUBSTRING(ACTNUMBR_1, 4, 3) like '''+ @searchTerm+'%'''
			set @query = @query + 
			' 0 TemplateID
				,0 DepartmentID
				,0 [Order]
				,0 [DataSource]
				,'''' Fund
				,LTRIM(RTRIM([ACTNUMBR_2])) Dept
				,'''' Program
				,'''' Project
				,'''' BaseElementObjectDetail
				,'''' Description'
			
			set @secQuery = @secQuery + 
			' 0 TemplateID
				,0 DepartmentID
				,0 [Order]
				,0 [DataSource]
				,'''' Fund
				,SUBSTRING(ACTNUMBR_1, 4, 3) Dept
				,'''' Program
				,'''' Project
				,'''' BaseElementObjectDetail
				,'''' Description'
		End
	else 
	if(@index = 3)
		Begin
			select	@condition = 'ACTNUMBR_3 like '''+ @searchTerm+'%''',
					@secCondition = 'SUBSTRING(ACTNUMBR_1, 7, 2) like '''+ @searchTerm+'%'''
			set @query = @query + 
			' 0 TemplateID
				,0 DepartmentID
				,0 [Order]
				,0 [DataSource]
				,'''' Fund
				,'''' Dept
				,LTRIM(RTRIM([ACTNUMBR_3])) Program
				,'''' Project
				,'''' BaseElementObjectDetail
				,'''' Description'

			set @secQuery = @secQuery + 
			' 0 TemplateID
				,0 DepartmentID
				,0 [Order]
				,0 [DataSource]
				,'''' Fund
				,'''' Dept
				,SUBSTRING(ACTNUMBR_1, 7, 2) Program
				,'''' Project
				,'''' BaseElementObjectDetail
				,'''' Description'
		End
	else 
	if(@index = 4)
		Begin
			select	@condition = 'ACTNUMBR_4 like '''+ @searchTerm+'%''',
					@secCondition = 'ACTNUMBR_2 like '''+ @searchTerm+'%'''
			set @query = @query + 
			' 0 TemplateID
				,0 DepartmentID
				,0 [Order]
				,0 [DataSource]
				,'''' Fund
				,'''' Dept
				,'''' Program
				,LTRIM(RTRIM([ACTNUMBR_4])) Project
				,'''' BaseElementObjectDetail
				,'''' Description'

			set @secQuery = @secQuery + 
			' 0 TemplateID
				,0 DepartmentID
				,0 [Order]
				,0 [DataSource]
				,'''' Fund
				,'''' Dept
				,'''' Program
				,LTRIM(RTRIM([ACTNUMBR_2])) Project
				,'''' BaseElementObjectDetail
				,'''' Description'
		End
	else if(@index = 5)
		Begin
			select	@condition = 'ACTNUMBR_5 like '''+ @searchTerm+'%''',
					@secCondition = 'ACTNUMBR_3 like '''+ @searchTerm+'%'''
			set @query = @query + 
			' 0 TemplateID
				,0 DepartmentID
				,0 [Order]
				,0 [DataSource]
				,'''' Fund
				,'''' Dept
				,'''' Program
				,'''' Project
				,LTRIM(RTRIM([ACTNUMBR_5])) BaseElementObjectDetail
				,'''' Description'

			set @secQuery = @secQuery + 
			' 0 TemplateID
				,0 DepartmentID
				,0 [Order]
				,0 [DataSource]
				,'''' Fund
				,'''' Dept
				,'''' Program
				,'''' Project
				,LTRIM(RTRIM([ACTNUMBR_3])) BaseElementObjectDetail
				,'''' Description'
		End
    else if(@index = 6)
		BEGIN
			select	@condition = 'ACTDESCR like ''%'+ @searchTerm+'%''',
					@secCondition = 'ACTDESCR like '''+ @searchTerm+'%'''
			set @query = @query + 
			' [ACTINDX] TemplateID
				,0 DepartmentID
				,0 [Order]
				,0 [DataSource]
				,LTRIM(RTRIM([ACTNUMBR_1])) Fund
				,LTRIM(RTRIM([ACTNUMBR_2])) Dept
				,LTRIM(RTRIM([ACTNUMBR_3])) Program
				,LTRIM(RTRIM([ACTNUMBR_4])) Project
				,LTRIM(RTRIM([ACTNUMBR_5])) BaseElementObjectDetail
				,''['' + LTRIM(RTRIM([ACTNUMBR_1]))+ ''.'' + LTRIM(RTRIM([ACTNUMBR_2]))+ ''.'' + LTRIM(RTRIM([ACTNUMBR_3]))+ ''.'' + 
				LTRIM(RTRIM([ACTNUMBR_4]))+ ''.'' +LTRIM(RTRIM([ACTNUMBR_5]))+ ''] '' 
				+LTRIM(RTRIM([ACTDESCR])) Description'

			set @secQuery = @secQuery + 
			' 0 TemplateID
				,0 DepartmentID
				,0 [Order]
				,0 [DataSource]
				,SUBSTRING(ACTNUMBR_1, 1, 3) Fund
				,SUBSTRING(ACTNUMBR_1, 4, 3) Dept
				,SUBSTRING(ACTNUMBR_1, 7, 2) Program
				,LTRIM(RTRIM([ACTNUMBR_2])) Project
				,LTRIM(RTRIM([ACTNUMBR_3])) BaseElementObjectDetail
				,''['' + LTRIM(RTRIM([ACTNUMBR_1]))+ ''.'' + LTRIM(RTRIM([ACTNUMBR_2]))+ ''.'' + LTRIM(RTRIM([ACTNUMBR_3]))+ ''.'' + 
				LTRIM(RTRIM([ACTNUMBR_4]))+ ''.'' +LTRIM(RTRIM([ACTNUMBR_5]))+ ''] '' 
				+LTRIM(RTRIM([ACTDESCR])) Description'
		END
	else 
	begin
		set @query = @query + 
			' [ACTINDX] TemplateID
				,0 DepartmentID
				,0 [Order]
				,0 [DataSource]
				,LTRIM(RTRIM([ACTNUMBR_1])) Fund
				,LTRIM(RTRIM([ACTNUMBR_2])) Dept
				,LTRIM(RTRIM([ACTNUMBR_3])) Program
				,LTRIM(RTRIM([ACTNUMBR_4])) Project
				,LTRIM(RTRIM([ACTNUMBR_5])) BaseElementObjectDetail
				,LTRIM(RTRIM([ACTDESCR])) Description'

		set @secQuery = @secQuery + 
			' [ACTINDX] TemplateID
				,0 DepartmentID
				,0 [Order]
				,0 [DataSource]
				,SUBSTRING(ACTNUMBR_1, 1, 3) Fund
				,SUBSTRING(ACTNUMBR_1, 4, 3) Dept
				,SUBSTRING(ACTNUMBR_1, 7, 2) Program
				,LTRIM(RTRIM([ACTNUMBR_2])) Project
				,LTRIM(RTRIM([ACTNUMBR_3])) BaseElementObjectDetail
				,LTRIM(RTRIM([ACTDESCR])) Description'
	end

	set @query = @query + ' FROM [GC].[dbo].[GL00100]
	WHERE Active = 1 and '+@condition 

	set @secQuery = @secQuery + ' FROM [dist].[dbo].[GL00100]
	WHERE Active = 1 and '+@secCondition 

	set @unionQuery = @query + ' union ' + @secQuery --+
	--' ORDER BY TemplateID OFFSET '+CAST(@skipRows AS NVARCHAR(10))+' ROWS FETCH NEXT '+CAST(@resultsCount AS NVARCHAR(10))+' ROWS ONLY'
	--print @unionQuery
	EXECUTE sp_executesql @unionQuery
GO 

--exec SearchAccounts @index = 6, @searchTerm = N'MOBILE HOME FEES', @resultsCount = 200, @skipRows = 0
--exec SearchAccounts @index = 6, @searchTerm = N'PUBLIC', @resultsCount = 200, @skipRows = 0
--exec SearchAccounts @index = 6, @searchTerm = N'BUILDING/FIRE MARSHAL', @resultsCount = 200, @skipRows = 0
--exec SearchAccounts @index = 5, @searchTerm = N'5486', @resultsCount = 200, @skipRows = 0
--exec SearchAccounts @index = 5, @searchTerm = N'5486', @resultsCount = 200, @skipRows = 20
--exec SearchAccounts @index = 4, @searchTerm = N'91', @resultsCount = 2000, @skipRows = 0
--exec SearchAccounts @index = 7, @searchTerm = N'91', @resultsCount = 1000000, @skipRows = 0
