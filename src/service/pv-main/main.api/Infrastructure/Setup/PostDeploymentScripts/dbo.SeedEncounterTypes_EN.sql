/**************************************************************************************************************************
**
**	Function: SEED ENCOUNTER TYPES
**
***************************************************************************************************************************/

SET IDENTITY_INSERT [dbo].[EncounterType] ON 
INSERT [dbo].[EncounterType] ([Id], [Description], [Help]) VALUES (1, N'Pre-Treatment Visit', '')
INSERT [dbo].[EncounterType] ([Id], [Description], [Help]) VALUES (2, N'Treatment Initiation Visit', '')
INSERT [dbo].[EncounterType] ([Id], [Description], [Help]) VALUES (3, N'Scheduled Follow-Up Visit', '')
INSERT [dbo].[EncounterType] ([Id], [Description], [Help]) VALUES (4, N'Unscheduled Visit', '')
SET IDENTITY_INSERT [dbo].[EncounterType] OFF