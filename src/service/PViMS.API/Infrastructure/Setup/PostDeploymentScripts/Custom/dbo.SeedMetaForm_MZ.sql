SET IDENTITY_INSERT [dbo].[MetaForm] ON 
GO
INSERT [dbo].[MetaForm] ([Id], [metaform_guid], [FormName], [MetaDefinition], [IsSystem], [ActionName]) VALUES (1, N'7d85c5de-301b-4613-a4f8-fd360300eaeb', N'TREATMENT START FORM A', N'
{
	"form": {
		"category": {
			"Element": [
				{
					"_contextType": "CustomAttribute",
					"_typeName": "Patient",
					"_contextLookUp": "Patient Identity Number",
					"_elementName": "NID Number"
				},
				{
					"_contextType": "FirstClass",
					"_typeName": "Patient",
					"_contextLookUp": "Surname",
					"_elementName": "Patient Initials"
				}
			],
			"_name": "General Details",
			"_displayType": "horizontal-2"
		},
		"_version": "0.1"
	}
}
			', 0, N'Form A')
GO
INSERT [dbo].[MetaForm] ([Id], [metaform_guid], [FormName], [MetaDefinition], [IsSystem], [ActionName]) VALUES (2, N'3066bf4b-88f6-4daa-90a6-16b045604332', N'FOLLOW-UP FORM (Men and Women) FORM B', N'
{
	"form": {
		"category": {
			"Element": [
				{
					"_contextType": "CustomAttribute",
					"_typeName": "Patient",
					"_contextLookUp": "Patient Identity Number",
					"_elementName": "NID Number"
				},
				{
					"_contextType": "FirstClass",
					"_typeName": "Patient",
					"_contextLookUp": "Surname",
					"_elementName": "Patient Initials"
				}
			],
			"_name": "General Details",
			"_displayType": "horizontal-2"
		},
		"_version": "0.1"
	}
}
			', 0, N'Form B')
GO
INSERT [dbo].[MetaForm] ([Id], [metaform_guid], [FormName], [MetaDefinition], [IsSystem], [ActionName]) VALUES (3, N'4d2a9a2b-5b3f-4276-95dd-9ca7fb3e3c39', N'BIRTH OUTCOME AND NEWBORN SCREENING FORM C', N'
{
	"form": {
		"category": {
			"Element": [
				{
					"_contextType": "CustomAttribute",
					"_typeName": "Patient",
					"_contextLookUp": "Patient Identity Number",
					"_elementName": "NID Number"
				},
				{
					"_contextType": "FirstClass",
					"_typeName": "Patient",
					"_contextLookUp": "Surname",
					"_elementName": "Patient Initials"
				}
			],
			"_name": "General Details",
			"_displayType": "horizontal-2"
		},
		"_version": "0.1"
	}
}
			', 0, N'Form C')
GO
SET IDENTITY_INSERT [dbo].[MetaForm] OFF
GO
