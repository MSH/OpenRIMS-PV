/**************************************************************************************************************************
**
**	Function: SEED ENCOUNTER TYPES : Mozambique Implementation
**
***************************************************************************************************************************/

SET IDENTITY_INSERT [dbo].[EncounterType] ON 
INSERT [dbo].[EncounterType] ([Id], [Description], [Help]) VALUES (1, N'Visita de início de tratamento', '')
SET IDENTITY_INSERT [dbo].[EncounterType] OFF
INSERT [dbo].[EncounterType] ([Id], [Description], [Help]) VALUES (2, N'Visita de seguimento', '')
INSERT [dbo].[EncounterType] ([Id], [Description], [Help]) VALUES (3, N'Consulta de resultado de nascimento', '')
SET IDENTITY_INSERT [dbo].[EncounterType] OFF