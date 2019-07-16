SET IDENTITY_INSERT [dbo].[Bills] ON
INSERT INTO [dbo].[Bills] ([Id], [Name], [DueDate], [AmountDue], [PaymentFrequency], [AccountId], [UserId]) VALUES (1, N'Renters Insurance', N'2019-10-14 00:00:00', CAST(150.00 AS Decimal(18, 2)), 8, 93, N'f6339304-121e-493b-8d30-a9742b7ddbc8')
INSERT INTO [dbo].[Bills] ([Id], [Name], [DueDate], [AmountDue], [PaymentFrequency], [AccountId], [UserId]) VALUES (2, N'Life Insurance', N'2019-08-14 00:00:00', CAST(300.00 AS Decimal(18, 2)), 8, 93, N'f6339304-121e-493b-8d30-a9742b7ddbc8')
INSERT INTO [dbo].[Bills] ([Id], [Name], [DueDate], [AmountDue], [PaymentFrequency], [AccountId], [UserId]) VALUES (3, N'Auto Insurance', N'2019-09-14 00:00:00', CAST(600.00 AS Decimal(18, 2)), 7, 93, N'f6339304-121e-493b-8d30-a9742b7ddbc8')
INSERT INTO [dbo].[Bills] ([Id], [Name], [DueDate], [AmountDue], [PaymentFrequency], [AccountId], [UserId]) VALUES (4, N'Rent', N'2019-08-01 00:00:00', CAST(1000.00 AS Decimal(18, 2)), 4, 98, N'f6339304-121e-493b-8d30-a9742b7ddbc8')
INSERT INTO [dbo].[Bills] ([Id], [Name], [DueDate], [AmountDue], [PaymentFrequency], [AccountId], [UserId]) VALUES (5, N'Groceries', N'2019-07-17 00:00:00', CAST(50.00 AS Decimal(18, 2)), 3, 96, N'f6339304-121e-493b-8d30-a9742b7ddbc8')
INSERT INTO [dbo].[Bills] ([Id], [Name], [DueDate], [AmountDue], [PaymentFrequency], [AccountId], [UserId]) VALUES (6, N'Gas', N'2019-07-18 00:00:00', CAST(20.00 AS Decimal(18, 2)), 2, 95, N'f6339304-121e-493b-8d30-a9742b7ddbc8')
INSERT INTO [dbo].[Bills] ([Id], [Name], [DueDate], [AmountDue], [PaymentFrequency], [AccountId], [UserId]) VALUES (7, N'Soda', N'2019-07-15 00:00:00', CAST(5.00 AS Decimal(18, 2)), 1, 94, N'f6339304-121e-493b-8d30-a9742b7ddbc8')
SET IDENTITY_INSERT [dbo].[Bills] OFF
