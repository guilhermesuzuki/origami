/****** Object:  StoredProcedure [dbo].[usp_GetHistoryByFormat]    Script Date: 9/25/2026 3:21:01 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE OR ALTER PROCEDURE [dbo].[usp_GetHistoryByFormat] 
	@blog uniqueidentifier,
	@start datetime,
	@end datetime,
	@format varchar(30)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT t.DateCreated, t.IsBot, t.BlogId INTO #userViews 
	FROM dbo.oi_vw_UserViews as t
	WHERE t.BlogId = @blog AND t.DateCreated BETWEEN @start AND @end;

	IF @format = 'dd [hh tt]'
	BEGIN
		SELECT 
			FORMAT(v.DateCreated, @format) as [Description]
			, IsBot
			, COUNT(1) as [Count]
		FROM #userViews as v
		GROUP BY DATEPART(DAY, v.DateCreated), DATEPART(HOUR, v.DateCreated), FORMAT(v.DateCreated, @format), IsBot
		ORDER BY DATEPART(DAY, v.DateCreated), DATEPART(HOUR, v.DateCreated);
	END
	ELSE BEGIN
		SELECT 
			FORMAT(v.DateCreated, @format) as [Description]
			, IsBot
			, COUNT(1) as [Count]
		FROM #userViews as v
		GROUP BY FORMAT(v.DateCreated, @format), IsBot
		ORDER BY 1;
	END
END
GO