SET IDENTITY_INSERT [dbo].[Accounts] ON
INSERT INTO [dbo].[Accounts] ([Id], [Name], [Balance], [PaycheckContribution], [RequiredSavings], [BalanceSurplus], [ExcludeFromSurplus], [SuggestedPaycheckContribution], [IsPoolAccount], [IsMandatory], [BalanceLimit], [UserId]) VALUES (131, N'Pool', CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), 0, CAST(0.00 AS Decimal(18, 2)), 1, 0, CAST(0.00 AS Decimal(18, 2)), N'98d2c048-22a9-4c2d-bc96-77d23f1ce52d')
INSERT INTO [dbo].[Accounts] ([Id], [Name], [Balance], [PaycheckContribution], [RequiredSavings], [BalanceSurplus], [ExcludeFromSurplus], [SuggestedPaycheckContribution], [IsPoolAccount], [IsMandatory], [BalanceLimit], [UserId]) VALUES (132, N'Rent', CAST(0.00 AS Decimal(18, 2)), CAST(600.00 AS Decimal(18, 2)), CAST(1200.00 AS Decimal(18, 2)), CAST(-1200.00 AS Decimal(18, 2)), 0, CAST(0.00 AS Decimal(18, 2)), 0, 0, CAST(0.00 AS Decimal(18, 2)), N'98d2c048-22a9-4c2d-bc96-77d23f1ce52d')
INSERT INTO [dbo].[Accounts] ([Id], [Name], [Balance], [PaycheckContribution], [RequiredSavings], [BalanceSurplus], [ExcludeFromSurplus], [SuggestedPaycheckContribution], [IsPoolAccount], [IsMandatory], [BalanceLimit], [UserId]) VALUES (133, N'Insurance', CAST(0.00 AS Decimal(18, 2)), CAST(62.50 AS Decimal(18, 2)), CAST(587.50 AS Decimal(18, 2)), CAST(-587.50 AS Decimal(18, 2)), 0, CAST(0.00 AS Decimal(18, 2)), 0, 0, CAST(0.00 AS Decimal(18, 2)), N'98d2c048-22a9-4c2d-bc96-77d23f1ce52d')
INSERT INTO [dbo].[Accounts] ([Id], [Name], [Balance], [PaycheckContribution], [RequiredSavings], [BalanceSurplus], [ExcludeFromSurplus], [SuggestedPaycheckContribution], [IsPoolAccount], [IsMandatory], [BalanceLimit], [UserId]) VALUES (134, N'Discretionary', CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), 0, CAST(0.00 AS Decimal(18, 2)), 0, 0, CAST(0.00 AS Decimal(18, 2)), N'98d2c048-22a9-4c2d-bc96-77d23f1ce52d')
INSERT INTO [dbo].[Accounts] ([Id], [Name], [Balance], [PaycheckContribution], [RequiredSavings], [BalanceSurplus], [ExcludeFromSurplus], [SuggestedPaycheckContribution], [IsPoolAccount], [IsMandatory], [BalanceLimit], [UserId]) VALUES (135, N'Auto Payment', CAST(0.00 AS Decimal(18, 2)), CAST(100.00 AS Decimal(18, 2)), CAST(100.00 AS Decimal(18, 2)), CAST(-100.00 AS Decimal(18, 2)), 0, CAST(0.00 AS Decimal(18, 2)), 0, 0, CAST(0.00 AS Decimal(18, 2)), N'98d2c048-22a9-4c2d-bc96-77d23f1ce52d')
SET IDENTITY_INSERT [dbo].[Accounts] OFF

SET IDENTITY_INSERT [dbo].[Bills] ON
INSERT INTO [dbo].[Bills] ([Id], [Name], [DueDate], [AmountDue], [PaymentFrequency], [AccountId], [UserId]) VALUES (1, N'Rent', N'2020-01-01 00:00:00', CAST(1200.00 AS Decimal(18, 2)), 4, 132, N'98d2c048-22a9-4c2d-bc96-77d23f1ce52d')
INSERT INTO [dbo].[Bills] ([Id], [Name], [DueDate], [AmountDue], [PaymentFrequency], [AccountId], [UserId]) VALUES (2, N'Auto Insurance', N'2020-03-01 00:00:00', CAST(600.00 AS Decimal(18, 2)), 7, 133, N'98d2c048-22a9-4c2d-bc96-77d23f1ce52d')
INSERT INTO [dbo].[Bills] ([Id], [Name], [DueDate], [AmountDue], [PaymentFrequency], [AccountId], [UserId]) VALUES (3, N'Life Insurance', N'2020-08-01 00:00:00', CAST(300.00 AS Decimal(18, 2)), 8, 133, N'98d2c048-22a9-4c2d-bc96-77d23f1ce52d')
INSERT INTO [dbo].[Bills] ([Id], [Name], [DueDate], [AmountDue], [PaymentFrequency], [AccountId], [UserId]) VALUES (4, N'Car Payment', N'2020-01-17 00:00:00', CAST(200.00 AS Decimal(18, 2)), 4, 135, N'98d2c048-22a9-4c2d-bc96-77d23f1ce52d')
SET IDENTITY_INSERT [dbo].[Bills] OFF

SET IDENTITY_INSERT [dbo].[Expenses] ON
INSERT INTO [dbo].[Expenses] ([Id], [Name], [Amount], [Due], [BillId], [IsPaid], [CreditAccountId], [UserId]) VALUES (1, N'Rent', CAST(1200.00 AS Decimal(18, 2)), N'2020-01-01 00:00:00', 1, 0, 0, N'98d2c048-22a9-4c2d-bc96-77d23f1ce52d')
INSERT INTO [dbo].[Expenses] ([Id], [Name], [Amount], [Due], [BillId], [IsPaid], [CreditAccountId], [UserId]) VALUES (2, N'Auto Insurance', CAST(600.00 AS Decimal(18, 2)), N'2020-03-01 00:00:00', 2, 0, 0, N'98d2c048-22a9-4c2d-bc96-77d23f1ce52d')
INSERT INTO [dbo].[Expenses] ([Id], [Name], [Amount], [Due], [BillId], [IsPaid], [CreditAccountId], [UserId]) VALUES (3, N'Life Insurance', CAST(300.00 AS Decimal(18, 2)), N'2020-08-01 00:00:00', 3, 0, 0, N'98d2c048-22a9-4c2d-bc96-77d23f1ce52d')
INSERT INTO [dbo].[Expenses] ([Id], [Name], [Amount], [Due], [BillId], [IsPaid], [CreditAccountId], [UserId]) VALUES (4, N'Car Payment', CAST(200.00 AS Decimal(18, 2)), N'2020-01-17 00:00:00', 4, 0, 0, N'98d2c048-22a9-4c2d-bc96-77d23f1ce52d')
SET IDENTITY_INSERT [dbo].[Expenses] OFF
