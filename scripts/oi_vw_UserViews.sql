-- Execute against the currently selected Origami database.

/****** Object:  View [dbo].[oi_vw_UserViews]    Script Date: 9/26/2026 5:22:26 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO











CREATE OR ALTER                       VIEW [dbo].[oi_vw_UserViews] 
--WITH SCHEMABINDING
AS
SELECT 
	newid() as [FakeId]
	,'PhysicalPageView' as [Type] 
	,'Physical Page View' as [TypeName]
	,b.Id as [BlogId]
	,ppv.[PhysicalPageId] as [Id]
    ,ppv.[DateCreated]
    ,ppv.[HostAddress]
    ,ppv.[HostName]
    ,ppv.[UserAgent]
    ,ppv.[Platform]
    ,ppv.[Browser]
    ,ppv.[IsMobileDevice]
    ,ppv.[UrlReferrer]
    ,ppv.[Url]
    ,ppv.[Version]
	,ppv.[Location_City]
    ,ppv.[Location_Country]
    ,ppv.[Location_CountryCode]
    ,ppv.[Location_Latitude]
    ,ppv.[Location_Longitude]
    ,ppv.[Location_Region]
    ,ppv.[Location_RegionCode]
    ,ppv.[Location_TimeZone]
	,ppv.[Location_TimeZoneOffset]
    ,ppv.[Location_ZipCode]
    ,ppv.[SocialProfileId]
	,ppv.IsBot
	,ppv.[Admin]
	,ppv.UserId
FROM [dbo].[oi_PhysicalPageViews] ppv
JOIN [dbo].[oi_PhysicalPages] pp ON pp.Id = ppv.PhysicalPageId
JOIN [dbo].[oi_Blogs] b ON b.IsPrimary = 1
WHERE ppv.ContentId IS NULL
UNION
SELECT 
	newid()
	,'PageView'
	,'Page View'
	,c.BlogId
    ,ppv.ContentId
    ,ppv.[DateCreated]
    ,ppv.[HostAddress]
    ,ppv.[HostName]
    ,ppv.[UserAgent]
    ,ppv.[Platform]
    ,ppv.[Browser]
    ,ppv.[IsMobileDevice]
    ,ppv.[UrlReferrer]
    ,ppv.[Url]
    ,ppv.[Version]
	,ppv.[Location_City]
    ,ppv.[Location_Country]
    ,ppv.[Location_CountryCode]
    ,ppv.[Location_Latitude]
    ,ppv.[Location_Longitude]
    ,ppv.[Location_Region]
    ,ppv.[Location_RegionCode]
    ,ppv.[Location_TimeZone]
	,ppv.[Location_TimeZoneOffset]
    ,ppv.[Location_ZipCode]
    ,ppv.[SocialProfileId]
	,ppv.IsBot
	,null
	,null
FROM [dbo].[oi_PhysicalPageViews] ppv
JOIN [dbo].[oi_Contents] c ON c.Id = ppv.ContentId
WHERE c.[Type] = 'OrigamiPage'
UNION
SELECT 
	newid()
	,'PostView'
	,'Post View'
	,c.BlogId
    ,ppv.ContentId
    ,ppv.[DateCreated]
    ,ppv.[HostAddress]
    ,ppv.[HostName]
    ,ppv.[UserAgent]
    ,ppv.[Platform]
    ,ppv.[Browser]
    ,ppv.[IsMobileDevice]
    ,ppv.[UrlReferrer]
    ,ppv.[Url]
    ,ppv.[Version]
	,ppv.[Location_City]
    ,ppv.[Location_Country]
    ,ppv.[Location_CountryCode]
    ,ppv.[Location_Latitude]
    ,ppv.[Location_Longitude]
    ,ppv.[Location_Region]
    ,ppv.[Location_RegionCode]
    ,ppv.[Location_TimeZone]
	,ppv.[Location_TimeZoneOffset]
    ,ppv.[Location_ZipCode]
    ,ppv.[SocialProfileId]
	,ppv.IsBot
	,null
	,null
FROM [dbo].[oi_PhysicalPageViews] ppv
JOIN [dbo].[oi_Contents] c ON c.Id = ppv.ContentId
WHERE c.[Type] = 'OrigamiPost'
UNION
SELECT 
	newid()
	,'VideoView'
	,'Video View'
	,c.BlogId
    ,ppv.ContentId
    ,ppv.[DateCreated]
    ,ppv.[HostAddress]
    ,ppv.[HostName]
    ,ppv.[UserAgent]
    ,ppv.[Platform]
    ,ppv.[Browser]
    ,ppv.[IsMobileDevice]
    ,ppv.[UrlReferrer]
    ,ppv.[Url]
    ,ppv.[Version]
	,ppv.[Location_City]
    ,ppv.[Location_Country]
    ,ppv.[Location_CountryCode]
    ,ppv.[Location_Latitude]
    ,ppv.[Location_Longitude]
    ,ppv.[Location_Region]
    ,ppv.[Location_RegionCode]
    ,ppv.[Location_TimeZone]
	,ppv.[Location_TimeZoneOffset]
    ,ppv.[Location_ZipCode]
    ,ppv.[SocialProfileId]
	,ppv.IsBot
	,null
	,null
FROM [dbo].[oi_PhysicalPageViews] ppv
JOIN [dbo].[oi_Contents] c ON c.Id = ppv.ContentId
WHERE c.[Type] = 'OrigamiVideo'
UNION
SELECT 
	newid()
	,'SpecialPageView'
	,'Special Page View'
	,b.Id
    ,ppv.ContentId
    ,ppv.[DateCreated]
    ,ppv.[HostAddress]
    ,ppv.[HostName]
    ,ppv.[UserAgent]
    ,ppv.[Platform]
    ,ppv.[Browser]
    ,ppv.[IsMobileDevice]
    ,ppv.[UrlReferrer]
    ,ppv.[Url]
    ,ppv.[Version]
	,ppv.[Location_City]
    ,ppv.[Location_Country]
    ,ppv.[Location_CountryCode]
    ,ppv.[Location_Latitude]
    ,ppv.[Location_Longitude]
    ,ppv.[Location_Region]
    ,ppv.[Location_RegionCode]
    ,ppv.[Location_TimeZone]
	,ppv.[Location_TimeZoneOffset]
    ,ppv.[Location_ZipCode]
    ,ppv.[SocialProfileId]
	,ppv.IsBot
	,null
	,null
FROM [dbo].[oi_PhysicalPageViews] ppv
JOIN [dbo].[oi_Contents] c ON c.Id = ppv.ContentId
JOIN [dbo].[oi_Blogs] b ON b.IsPrimary = 1
WHERE c.[Type] = 'OrigamiSpecialPage'
UNION
SELECT 
	newid()
	,'SoftwareRelease'
	,'Software Release'
	,b.Id
    ,ppv.ContentId
    ,ppv.[DateCreated]
    ,ppv.[HostAddress]
    ,ppv.[HostName]
    ,ppv.[UserAgent]
    ,ppv.[Platform]
    ,ppv.[Browser]
    ,ppv.[IsMobileDevice]
    ,ppv.[UrlReferrer]
    ,ppv.[Url]
    ,ppv.[Version]
	,ppv.[Location_City]
    ,ppv.[Location_Country]
    ,ppv.[Location_CountryCode]
    ,ppv.[Location_Latitude]
    ,ppv.[Location_Longitude]
    ,ppv.[Location_Region]
    ,ppv.[Location_RegionCode]
    ,ppv.[Location_TimeZone]
	,ppv.[Location_TimeZoneOffset]
    ,ppv.[Location_ZipCode]
    ,ppv.[SocialProfileId]
	,ppv.IsBot
	,null
	,null
FROM [dbo].[oi_PhysicalPageViews] ppv
JOIN [dbo].[oi_Contents] c ON c.Id = ppv.ContentId
JOIN [dbo].[oi_Blogs] b ON b.IsPrimary = 1
WHERE c.[Type] = 'OrigamiSoftwareRelease'
;
GO


