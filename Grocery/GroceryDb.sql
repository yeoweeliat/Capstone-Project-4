
USE [GroceryDb]
GO

CREATE TABLE [Products] (
	[ProductId] uniqueidentifier NOT NULL
	, [ProductName] varchar(80) NOT NULL
	, [Quantity] int NOT NULL
		CONSTRAINT [DF_Products_Quantity] DEFAULT (0)
	, [SellingPricePerUnit] money NOT NULL
		CONSTRAINT [DF_Products_SellingPricePerUnit] DEFAULT (0.0)
	, [Image] varbinary(MAX) NULL
	
	, [CreatedByUserId] uniqueidentifier NOT NULL
	, [UpdatedByUserId] uniqueidentifier NULL
	, [LastUpdatedOn] datetime NOT NULL
		CONSTRAINT [DF_Products_LastUpdatedOn] DEFAULT ( getdate() )

	, CONSTRAINT [PK_Products] 
			PRIMARY KEY( [ProductId] ASC )
	, CONSTRAINT [FK_Products_AspNetUsers_CreatedByUserId] 
			FOREIGN KEY( [CreatedByUserId] ) REFERENCES [AspNetUsers] ([Id])
	, CONSTRAINT [FK_Products_AspNetUsers_UpdatedByUserId] 
			FOREIGN KEY( [UpdatedByUserId] ) REFERENCES [AspNetUsers] ([Id]))
GO
