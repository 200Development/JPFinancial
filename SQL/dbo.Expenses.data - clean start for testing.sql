SET IDENTITY_INSERT [dbo].[Expenses] ON
INSERT INTO [dbo].[Expenses] ([Id], [Name], [Amount], [Due], [BillId], [IsPaid], [CreditAccountId], [UserId]) VALUES (1, N'Renters Insurance', CAST(150.00 AS Decimal(18, 2)), N'2019-10-14 00:00:00', 1, 0, 0, N'f6339304-121e-493b-8d30-a9742b7ddbc8')
INSERT INTO [dbo].[Expenses] ([Id], [Name], [Amount], [Due], [BillId], [IsPaid], [CreditAccountId], [UserId]) VALUES (2, N'Life Insurance', CAST(300.00 AS Decimal(18, 2)), N'2019-08-14 00:00:00', 2, 0, 0, N'f6339304-121e-493b-8d30-a9742b7ddbc8')
INSERT INTO [dbo].[Expenses] ([Id], [Name], [Amount], [Due], [BillId], [IsPaid], [CreditAccountId], [UserId]) VALUES (3, N'Auto Insurance', CAST(600.00 AS Decimal(18, 2)), N'2019-09-14 00:00:00', 3, 0, 0, N'f6339304-121e-493b-8d30-a9742b7ddbc8')
INSERT INTO [dbo].[Expenses] ([Id], [Name], [Amount], [Due], [BillId], [IsPaid], [CreditAccountId], [UserId]) VALUES (4, N'Rent', CAST(1000.00 AS Decimal(18, 2)), N'2019-08-01 00:00:00', 4, 0, 0, N'f6339304-121e-493b-8d30-a9742b7ddbc8')
INSERT INTO [dbo].[Expenses] ([Id], [Name], [Amount], [Due], [BillId], [IsPaid], [CreditAccountId], [UserId]) VALUES (5, N'Groceries', CAST(50.00 AS Decimal(18, 2)), N'2019-07-17 00:00:00', 5, 0, 0, N'f6339304-121e-493b-8d30-a9742b7ddbc8')
INSERT INTO [dbo].[Expenses] ([Id], [Name], [Amount], [Due], [BillId], [IsPaid], [CreditAccountId], [UserId]) VALUES (6, N'Gas', CAST(20.00 AS Decimal(18, 2)), N'2019-07-18 00:00:00', 6, 0, 0, N'f6339304-121e-493b-8d30-a9742b7ddbc8')
INSERT INTO [dbo].[Expenses] ([Id], [Name], [Amount], [Due], [BillId], [IsPaid], [CreditAccountId], [UserId]) VALUES (7, N'Soda', CAST(5.00 AS Decimal(18, 2)), N'2019-07-15 00:00:00', 7, 0, 0, N'f6339304-121e-493b-8d30-a9742b7ddbc8')
SET IDENTITY_INSERT [dbo].[Expenses] OFF
