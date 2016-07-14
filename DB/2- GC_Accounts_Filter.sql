Create PROCEDURE SearchGLAccounts
    @Fund nvarchar(150) = NULL,
	@Dept nvarchar(150) = NULL,
	@Program nvarchar(150) = NULL,
	@Project nvarchar(150) = NULL,
	@BaseElementObjectDetail nvarchar(150) = NULL,
	@Description nvarchar(150) = NULL,
	@resultsCount int = 100,
	@skipRows int = 0
AS   
    SET NOCOUNT ON;  

			SELECT [ACTINDX] TemplateID
				,0 DepartmentID
				,0 [Order]
				,LTRIM(RTRIM([ACTNUMBR_1])) Fund
				,LTRIM(RTRIM([ACTNUMBR_2])) Dept
				,LTRIM(RTRIM([ACTNUMBR_3])) Program
				,LTRIM(RTRIM([ACTNUMBR_4])) Project
				,LTRIM(RTRIM([ACTNUMBR_5])) BaseElementObjectDetail
				,LTRIM(RTRIM([ACTDESCR])) Description
				FROM [GC].[dbo].[GL00100]
				Where Active = 1 
				AND (@Fund is null or [ACTNUMBR_1] like ''+ @Fund +'%')
				AND (@Dept is null or [ACTNUMBR_2] like ''+ @Dept +'%')
				AND (@Program is null or [ACTNUMBR_3] like ''+ @Program +'%')
				AND (@Project is null or [ACTNUMBR_4] like ''+ @Project +'%')
				AND (@BaseElementObjectDetail is null or [ACTNUMBR_5] like ''+ @BaseElementObjectDetail +'%')
				AND (@Description is null or ACTDESCR like '%'+ @Description +'%')
				ORDER BY TemplateID OFFSET @skipRows ROWS FETCH NEXT @resultsCount ROWS ONLY

			SELECT Count(*) as RowsCount
				FROM [GC].[dbo].[GL00100]
				Where Active = 1 
				AND (@Fund is null or [ACTNUMBR_1] like ''+ @Fund +'%')
				AND (@Dept is null or [ACTNUMBR_2] like ''+ @Dept +'%')
				AND (@Program is null or [ACTNUMBR_3] like ''+ @Program +'%')
				AND (@Project is null or [ACTNUMBR_4] like ''+ @Project +'%')
				AND (@BaseElementObjectDetail is null or [ACTNUMBR_5] like ''+ @BaseElementObjectDetail +'%')
				AND (@Description is null or ACTDESCR like '%'+ @Description +'%')

GO 

--exec SearchGLAccounts @Description = N'Protec', @resultsCount = 200, @skipRows = 0
--exec SearchGLAccounts @Description = N'Protec', @resultsCount = 2, @skipRows = 2
--exec SearchGLAccounts @Description = N'Protec', @resultsCount = 2, @skipRows = 4
--exec SearchGLAccounts @Description = N'Protec', @Fund = '001', @resultsCount = 200, @skipRows = 0
--exec SearchGLAccounts @Description = N'Protec', @Fund = '001', @Project = '9116', @resultsCount = 200, @skipRows = 0
--exec SearchGLAccounts @Description = N'Protec', @BaseElementObjectDetail = '338210002', @resultsCount = 200, @skipRows = 0
