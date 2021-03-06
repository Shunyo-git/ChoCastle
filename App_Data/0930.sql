USE [ChoCastleDB]
GO
/****** Object:  StoredProcedure [dbo].[sp_set_main_file]    Script Date: 2021/9/30 上午 02:55:49 ******/
DROP PROCEDURE IF EXISTS [dbo].[sp_set_main_file]
GO
/****** Object:  StoredProcedure [dbo].[sp_insert_file]    Script Date: 2021/9/30 上午 02:55:49 ******/
DROP PROCEDURE IF EXISTS [dbo].[sp_insert_file]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_product_photos]    Script Date: 2021/9/30 上午 02:55:49 ******/
DROP PROCEDURE IF EXISTS [dbo].[sp_get_product_photos]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_product_mainPhoto]    Script Date: 2021/9/30 上午 02:55:49 ******/
DROP PROCEDURE IF EXISTS [dbo].[sp_get_product_mainPhoto]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_file_details]    Script Date: 2021/9/30 上午 02:55:49 ******/
DROP PROCEDURE IF EXISTS [dbo].[sp_get_file_details]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_all_files]    Script Date: 2021/9/30 上午 02:55:49 ******/
DROP PROCEDURE IF EXISTS [dbo].[sp_get_all_files]
GO
/****** Object:  StoredProcedure [dbo].[sp_delete_file]    Script Date: 2021/9/30 上午 02:55:49 ******/
DROP PROCEDURE IF EXISTS [dbo].[sp_delete_file]
GO
/****** Object:  StoredProcedure [dbo].[sp_delete_file]    Script Date: 2021/9/30 上午 02:55:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_delete_file]
	@file_id INT
AS
BEGIN
/****** Script for SelectTopNRows command from SSMS  ******/
	DELETE FROM [dbo].[ProductImages]
	WHERE [ProductImages].[file_id] = @file_id   
	 
END

GO
/****** Object:  StoredProcedure [dbo].[sp_get_all_files]    Script Date: 2021/9/30 上午 02:55:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_get_all_files]
	
AS


BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT Img.[file_id]
		  ,Img.[file_name]
		  ,Img.[file_ext]
		   ,Img.[file_base6]
			 ,Img.[ProductID]
			,Img.[isMain]
			,Img.[SortID]
			,P.ProductName
	FROM [dbo].[ProductImages] Img Left JOIN  [dbo].Products P
	ON Img.ProductID = P.ProductID 
END

GO
/****** Object:  StoredProcedure [dbo].[sp_get_file_details]    Script Date: 2021/9/30 上午 02:55:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_get_file_details] 
	@file_id INT
AS
BEGIN
/****** Script for SelectTopNRows command from SSMS  ******/

		SELECT Img.[file_id]
		  ,Img.[file_name]
		  ,Img.[file_ext]
		   ,Img.[file_base6]
			 ,Img.[ProductID]
			,Img.[isMain]
			,Img.[SortID]
			,P.ProductName
	FROM [dbo].[ProductImages] Img Left JOIN  [dbo].Products P
	ON Img.ProductID = P.ProductID 
	WHERE Img.[file_id] = @file_id
END

GO
/****** Object:  StoredProcedure [dbo].[sp_get_product_mainPhoto]    Script Date: 2021/9/30 上午 02:55:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_get_product_mainPhoto]
	@ProductID INT
AS
BEGIN
/****** Script for SelectTopNRows command from SSMS  ******/
	SELECT [file_id]
		  ,[file_name]
		  ,[file_ext]
		  ,[file_base6]
		   ,[ProductID]
		   ,[isMain]
			,[SortID]
	FROM [dbo].[ProductImages]
	WHERE [ProductImages].[ProductID] = @ProductID  AND [isMain] = 1
	ORDER BY SortID
END


GO
/****** Object:  StoredProcedure [dbo].[sp_get_product_photos]    Script Date: 2021/9/30 上午 02:55:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_get_product_photos] 
	@ProductID INT
AS
BEGIN
/****** Script for SelectTopNRows command from SSMS  ******/
	SELECT [file_id]
		  ,[file_name]
		  ,[file_ext]
		  ,[file_base6]
		   ,[ProductID]
		   ,[isMain]
			,[SortID]
	FROM [dbo].[ProductImages]
	WHERE [ProductImages].[ProductID] = @ProductID
	ORDER BY SortID
END

/****** Object:  StoredProcedure [dbo].[sp_get_product_mainPhoto] ******/
SET ANSI_NULLS ON
GO
/****** Object:  StoredProcedure [dbo].[sp_insert_file]    Script Date: 2021/9/30 上午 02:55:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_insert_file]
	@file_name NVARCHAR(MAX),
	@file_ext NVARCHAR(MAX),
	@file_base64 NVARCHAR(MAX),
	@ProductID int	 ,
	@isMain int ,
	@SortID int
AS
 
 DECLARE @Error int
/****** Script for SelectTopNRows command from SSMS  ******/
	INSERT INTO [dbo].[ProductImages]
           ([file_name]
           ,[file_ext]
           ,[file_base6]
		    ,[ProductID]
			,[isMain]
			,[SortID])
     VALUES
           (@file_name
           ,@file_ext
           ,@file_base64
		   ,@ProductID 
			,@isMain
			,@SortID )

SET @Error = @@ERROR
IF @Error != 0 GOTO ERROR_HANDLER
 RETURN  @@IDENTITY  

ERROR_HANDLER:
	IF @@TRANCOUNT != 0 ROLLBACK TRANSACTION
	RETURN @Error

			 
 

GO
/****** Object:  StoredProcedure [dbo].[sp_set_main_file]    Script Date: 2021/9/30 上午 02:55:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Sean>
-- Create date: <2021/9/24>
-- Description:	<SP_ShoppingCart_GetCartIdByMemberID>
-- =============================================
CREATE PROCEDURE  [dbo].[sp_set_main_file]
	(
	@file_id int  
	)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	DECLARE @ProductID int
	SET @ProductID = 0 

	 IF @file_id > 0
	 BEGIN
		SELECT   @ProductID  = ProductID  
		FROM              ProductImages
		WHERE          (file_id = @file_id)
	END

		IF @ProductID  > 0
	BEGIN
		UPDATE ProductImages SET isMain = 0  WHERE  ProductID=@ProductID  
		UPDATE ProductImages SET isMain = 1  WHERE  file_id=@file_id 
	END

END
	 RETURN  @@ERROR  
GO
