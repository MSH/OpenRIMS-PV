DECLARE @Id int
SELECT @Id = Id   FROM Dataset  WHERE DatasetName = 'E2B(R2) ICH Report'

IF(@Id IS NOT NULL) BEGIN
	
	DECLARE @VersionErrorMessage VARCHAR(1024)
	SET @VersionErrorMessage = 'This script has already been executed.'
	RAISERROR(@VersionErrorMessage,16,1)
	RETURN;
	
END

SET NOCOUNT ON

BEGIN TRAN A1

/**************************************************
DATASET
**************************************************/
DECLARE @elementname varchar(100)

DECLARE @dsid int
DECLARE @dmid int
DECLARE @dmsid int
DECLARE @dsxid int
DECLARE @dsxnid int
DECLARE @dsxcnid int
DECLARE @dsxcnid2 int
DECLARE @dsxcnid3 int
DECLARE @dsxcnid4 int
DECLARE @dsxenid int
INSERT [dbo].[Dataset] ([DatasetName], [Active], [InitialiseProcess], [RulesProcess], [Help], [Created], [LastUpdated], [ContextType_Id], [CreatedBy_Id], [UpdatedBy_Id], [EncounterTypeWorkPlan_Id], [IsSystem])
	VALUES (N'E2B(R2) ICH Report', 1, NULL, NULL, N'ICH ICSR E2B v2 Dataset', GETDATE(), GETDATE(), 5, 1, 1, NULL, 1)
set @dsid = (SELECT @@IDENTITY)

-- base xml
INSERT [dbo].[DatasetXml] ([Description], Created, LastUpdated, CreatedBy_Id, UpdatedBy_Id)
	VALUES ('E2B(R2) XML', GETDATE(), GETDATE(), 1, 1)
set @dsxid = (SELECT @@IDENTITY)
UPDATE Dataset SET DatasetXml_Id = @dsxid where Id = @dsid

-- base node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('ichicsr', 1, NULL, GETDATE(), GETDATE(), NULL, 1, NULL, 1, @dsxid)
set @dsxnid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetXmlAttribute] (AttributeName, AttributeValue, Created, LastUpdated, CreatedBy_Id, DatasetElement_Id, ParentNode_Id, UpdatedBy_Id)
	VALUES ('lang', 'en', GETDATE(), GETDATE(), 1, NULL, @dsxnid, 1)

/**************************************************
CATEGORY ichicsrmessageheader
**************************************************/
DECLARE @dscid int
INSERT [dbo].[DatasetCategory] ([DatasetCategoryName], [CategoryOrder], [Dataset_Id], [System], [Acute], [Chronic], [Public]) 
	VALUES (N'Message Header', 1, @dsid, 1, 0, 0, 0)
set @dscid = (SELECT @@IDENTITY)

-- category node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('ichicsrmessageheader', 2, NULL, GETDATE(), GETDATE(), @dsxnid, 1, NULL, 1, @dsxid)
set @dsxcnid = (SELECT @@IDENTITY)

-- messagetype
DECLARE @fid int
DECLARE @deid int
DECLARE @desid int
DECLARE @dceid int
DECLARE @sdceid int
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 50, NULL, 8)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Message Type', @fid, 1, NULL, 'ICHICSR', 1, '866CE390-4850-43E0-9C85-A4A66F70904A')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('messagetype', 2, NULL, GETDATE(), GETDATE(), @dsxcnid, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- messageformatversion
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 10, NULL, 8)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Message Format Version', @fid, 1, NULL, '2.1', 1, '5E404E74-8839-4411-A4AC-41BE6B79548A')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('messageformatversion', 2, NULL, GETDATE(), GETDATE(), @dsxcnid, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- messageformatrelease
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 10, NULL, 8)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Message Format Release', @fid, 1, NULL, '1.0', 1, 'E7B5701F-47E0-4766-849C-9D0C120C2632')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('messageformatrelease', 2, NULL, GETDATE(), GETDATE(), @dsxcnid, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- messagenumb
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 10, NULL, 8)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Message Number', @fid, 1, NULL, NULL, 1, '7FF710CB-C08C-4C35-925E-484B983F2135')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('messagenumb', 2, NULL, GETDATE(), GETDATE(), @dsxcnid, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- messagesenderidentifier
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 50, NULL, 8)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Message Sender Identifier', @fid, 1, NULL, 'FDA', 1, 'DDA070A6-701A-4339-AABB-14C56A9A33F2')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('messagesenderidentifier', 2, NULL, GETDATE(), GETDATE(), @dsxcnid, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- messagereceiveridentifier
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 50, NULL, 8)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Message Receiver Identifier', @fid, 1, NULL, 'UMC', 1, '13C51F23-A041-44F7-8E7C-E5433FF7E88D')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('messagereceiveridentifier', 2, NULL, GETDATE(), GETDATE(), @dsxcnid, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- messagedateformat
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 10, NULL, 8)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Message Date Format', @fid, 1, NULL, '204', 1, '6890F8FD-3033-4AD6-B38C-4A1A9E359AAE')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('messagedateformat', 2, NULL, GETDATE(), GETDATE(), @dsxcnid, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- messagedate
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 10, NULL, 8)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Message Date', @fid, 1, NULL, NULL, 1, '693614B6-D5D5-457E-A03B-EAAFA66E6FBD')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('messagedate', 2, NULL, GETDATE(), GETDATE(), @dsxcnid, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)


/**************************************************
CATEGORY safetyreport
**************************************************/
INSERT [dbo].[DatasetCategory] ([DatasetCategoryName], [CategoryOrder], [Dataset_Id]) 
	VALUES (N'Safety Report', 1, @dsid)
set @dscid = (SELECT @@IDENTITY)

-- category node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('safetyreport', 2, NULL, GETDATE(), GETDATE(), @dsxnid, 1, NULL, 1, @dsxid)
set @dsxcnid = (SELECT @@IDENTITY)

-- safetyreportversion
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 10, NULL, 8)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Safety Report Version', @fid, 1, NULL, '1', 1, 'A3FA7A76-1699-44D5-8ADF-8ACD0A0D869C')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('safetyreportversion', 2, NULL, GETDATE(), GETDATE(), @dsxcnid, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- safetyreportid
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 20, NULL, 8)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Safety Report ID', @fid, 1, NULL, NULL, 0, '6799CAD0-2A65-48A5-8734-0090D7C2D85E')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('safetyreportid', 2, NULL, GETDATE(), GETDATE(), @dsxcnid, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- primarysourcecountry
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 10, NULL, 8)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Primary Source Country', @fid, 1, NULL, 'PH', 0, 'E822D795-78F5-443F-AB22-A24D5D42A025')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('primarysourcecountry', 2, NULL, GETDATE(), GETDATE(), @dsxcnid, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- occurcountry
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 10, NULL, 8)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Occur Country', @fid, 1, NULL, 'PH', 0, '959DAD0E-3839-4F13-959C-B16967585009')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('occurcountry', 2, NULL, GETDATE(), GETDATE(), @dsxcnid, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- transmissiondateformat
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 10, NULL, 8)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Transmission Date Format', @fid, 1, NULL, '102', 1, '199A76B2-E436-4CB2-8909-500570D52C01')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('transmissiondateformat', 2, NULL, GETDATE(), GETDATE(), @dsxcnid, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- transmissiondate
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 10, NULL, 8)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Transmission Date', @fid, 1, NULL, NULL, 1, '9C92D382-03AF-4A52-9A2F-04A46ADA0F7E')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('transmissiondate', 2, NULL, GETDATE(), GETDATE(), @dsxcnid, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- reporttype
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 1, NULL, 8)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Report Type', @fid, 1, NULL, NULL, 1, 'AE53FEB2-FF27-4CD5-AD54-C3FFED1490B5')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('reporttype', 2, NULL, GETDATE(), GETDATE(), @dsxcnid, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- serious
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('1=Yes', 1, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('2=No', 0, 0, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Serious', @fid, 1, NULL, NULL, 0, '510EB752-2D75-4DC3-8502-A4FCDC8A621A')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
set @dceid = (SELECT @@IDENTITY)
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('serious', 2, NULL, GETDATE(), GETDATE(), @dsxcnid, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)
-- mapping
INSERT [dbo].[DatasetMapping] (Tag, MappingType, MappingOption, DestinationElement_Id, SourceElement_Id)
	VALUES ('Spontaneous', 1, '', @dceid, (select top 1 dce.Id from [dbo].[DatasetCategoryElement] dce inner join [dbo].[DatasetElement] de on dce.DatasetElement_Id = de.Id where de.ElementName = 'Reaction serious'))
set @dmid = (SELECT @@IDENTITY)
-- mapping values
INSERT [dbo].[DatasetMappingValue] (SourceValue, DestinationValue, Active, Mapping_Id)
	SELECT 'Yes', '1=Yes', 1, @dmid UNION
	SELECT 'No', '2=No', 1, @dmid

-- seriousnessdeath 
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('1=Yes', 1, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('2=No', 0, 0, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Seriousness Death', @fid, 1, NULL, NULL, 0, 'B4EA6CBF-2D9C-482D-918A-36ABB0C96EFA')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('seriousnessdeath', 2, NULL, GETDATE(), GETDATE(), @dsxcnid, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- seriousnesslifethreatening 
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('1=Yes', 1, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('2=No', 0, 0, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Seriousness Life Threatening', @fid, 1, NULL, NULL, 0, '26C6F08E-B80B-411E-BFDC-0506FE102253')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('seriousnesslifethreatening', 2, NULL, GETDATE(), GETDATE(), @dsxcnid, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- seriousnesshospitalization 
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('1=Yes', 1, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('2=No', 0, 0, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Seriousness Hospitalization', @fid, 1, NULL, NULL, 0, '837154A9-D088-41C6-A9E2-8A0231128496')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('seriousnesshospitalization', 2, NULL, GETDATE(), GETDATE(), @dsxcnid, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- seriousnessdisabling 
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('1=Yes', 1, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('2=No', 0, 0, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Seriousness Disabling', @fid, 1, NULL, NULL, 0, 'DDEBDEC0-2A90-49C7-970E-B7855CFDF19D')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('seriousnessdisabling', 2, NULL, GETDATE(), GETDATE(), @dsxcnid, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- seriousnesscongenitalanomali 
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('1=Yes', 1, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('2=No', 0, 0, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Seriousness Congenital Anomaly', @fid, 1, NULL, NULL, 0, 'DF89C98B-1D2A-4C8E-A753-02E265841F4F')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('seriousnesscongenitalanomali', 2, NULL, GETDATE(), GETDATE(), @dsxcnid, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- seriousnessother 
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('1=Yes', 1, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('2=No', 0, 0, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Seriousness Other', @fid, 1, NULL, NULL, 0, '33A75547-EF1B-42FB-8768-CD6EC52B24F8')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('seriousnessother', 2, NULL, GETDATE(), GETDATE(), @dsxcnid, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- receivedateformat
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 10, NULL, 8)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Receive Date Format', @fid, 1, NULL, '102', 1, 'C322AAFA-35FE-481C-856A-8ED4BE358AA6')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('receivedateformat', 2, NULL, GETDATE(), GETDATE(), @dsxcnid, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- receivedate
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 6)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Date report was first received', @fid, 1, NULL, NULL, 0, '65ADEF15-961A-4558-B864-7832D276E0E3')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('receivedate', 2, NULL, GETDATE(), GETDATE(), @dsxcnid, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- receiptdateformat
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 10, NULL, 8)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Receipt Date Format', @fid, 1, NULL, '102', 1, '3BECDC2B-603C-4BF7-AC9E-32DD8DA7CB40')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('receiptdateformat', 2, NULL, GETDATE(), GETDATE(), @dsxcnid, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- receiptdate
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 6)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Date of most recent info', @fid, 1, NULL, NULL, 0, 'A10C704D-BC1D-445E-B084-9426A91DB63B')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('receiptdate', 2, NULL, GETDATE(), GETDATE(), @dsxcnid, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- additionaldocument
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('1=Yes', 1, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('2=No', 0, 0, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Additional Document', @fid, 1, NULL, '2', 0, '9E9AE1E3-77B5-42B8-828E-067E2A4CAC6E')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('additionaldocument', 2, NULL, GETDATE(), GETDATE(), @dsxcnid, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- documentlist
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 100, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Document List', @fid, 1, NULL, '', 0, '073EF8E2-9DE3-48BC-86F6-69AF83FAB42A')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('documentlist', 2, NULL, GETDATE(), GETDATE(), @dsxcnid, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- fulfillexpeditecriteria
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('1=Yes', 1, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('2=No', 0, 0, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Fulfill Expedite Criteria', @fid, 1, NULL, '2', 0, '0C5B7ECB-88DF-4C48-9590-178A377025BC')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('fulfillexpeditecriteria', 2, NULL, GETDATE(), GETDATE(), @dsxcnid, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- duplicate
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 10, NULL, 8)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Duplicate', @fid, 1, NULL, '', 1, '5529B157-7905-4CDB-B9F5-30C894830E41')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('duplicate', 2, NULL, GETDATE(), GETDATE(), @dsxcnid, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- casenullification 
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('1=Yes', 1, 0, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Case Nullification', @fid, 1, NULL, NULL, 0, '949AA82C-9866-4A45-9EA8-8FCC5DF5E996')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('casenullification', 2, NULL, GETDATE(), GETDATE(), @dsxcnid, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- nullificationreason
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 200, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Nullification Reason', @fid, 1, NULL, NULL, 0, 'B83A21F1-D92C-442F-AB47-93D61B3031C3')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('nullificationreason', 2, NULL, GETDATE(), GETDATE(), @dsxcnid, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- medicallyconfirm
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('1=Yes', 1, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('2=No', 0, 0, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Medically Confirm', @fid, 1, NULL, NULL, 0, 'E65F6313-2BC2-44BE-9D00-53DE611A9C02')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('medicallyconfirm', 2, NULL, GETDATE(), GETDATE(), @dsxcnid, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

/**************************************************
CATEGORY primarysource
**************************************************/
INSERT [dbo].[DatasetCategory] ([DatasetCategoryName], [CategoryOrder], [Dataset_Id]) 
	VALUES (N'Primary Source', 1, @dsid)
set @dscid = (SELECT @@IDENTITY)

-- category node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('primarysource', 2, NULL, GETDATE(), GETDATE(), @dsxcnid, 1, NULL, 1, @dsxid)
set @dsxcnid2 = (SELECT @@IDENTITY)

-- reportertitle
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 50, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Reporter Title', @fid, 1, NULL, NULL, 0, 'A63EC911-84A2-4A8A-9DDB-412E35250F63')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('reportertitle', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- reportergivename
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 35, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Reporter Given Name', @fid, 1, NULL, NULL, 0, 'C35D5F5A-D539-4EEE-B080-FF384D5FBE08')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('reportergivename', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- reportermiddlename
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 15, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Reporter Middle Name', @fid, 1, NULL, NULL, 0, '8AC90E6C-7A7D-4170-94BC-75CB66904815')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('reportermiddlename', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- reporterfamilyname
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 50, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Reporter Family Name', @fid, 1, NULL, NULL, 0, 'F214C619-EE0E-433E-8F52-83469778E418')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('reporterfamilyname', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- reporterorganization
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 60, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Reporter Organization', @fid, 1, NULL, NULL, 0, '8C854641-2961-4B01-B8DF-FD19AEEB8497')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
set @dceid = (SELECT @@IDENTITY)
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('reporterorganization', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)
-- mapping
INSERT [dbo].[DatasetMapping] (Tag, MappingType, MappingOption, DestinationElement_Id, SourceElement_Id)
	VALUES ('Spontaneous', 0, '', @dceid, (select top 1 dce.Id from [dbo].[DatasetCategoryElement] dce inner join [dbo].[DatasetElement] de on dce.DatasetElement_Id = de.Id where de.ElementName = 'Reporter Place of Practice'))

-- reporterdepartment
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 60, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Reporter Department', @fid, 1, NULL, NULL, 0, '75E35243-B45D-4F64-9E23-82E0571F1AB5')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('reporterdepartment', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- reporterstreet
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 100, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Reporter Street', @fid, 1, NULL, NULL, 0, 'F00AEFE1-337D-4B06-8BDF-5106A585213E')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
set @dceid = (SELECT @@IDENTITY)
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('reporterstreet', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- reportercity
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 35, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Reporter City', @fid, 1, NULL, NULL, 0, '457EE471-E110-4900-AB68-FAF54D31E375')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('reportercity', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- reporterstate
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 40, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Reporter State', @fid, 1, NULL, NULL, 0, 'BFBC780E-BF99-42C2-A446-A05014211D88')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('reporterstate', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- reporterpostcode
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 15, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Reporter Postcode', @fid, 1, NULL, NULL, 0, '216236EC-CD5D-4FBE-881C-12D8A847D2BA')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('reporterpostcode', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- reportercountry
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 2, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Reporter Country', @fid, 1, NULL, 'PH', 0, 'B6DB6E8D-075B-4DAB-A278-F61329FEEA92')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('reportercountry', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- qualification
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('1=Physician', 1, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('2=Pharmacist', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('3=Other Health Professional', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('4=Lawyer', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('5=Consumer or other non health professional', 0, 0, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Qualification', @fid, 1, NULL, NULL, 0, '1D59E85E-66AF-4E70-B779-6AB873DE1E84')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
set @dceid = (SELECT @@IDENTITY)
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('qualification', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)
-- mapping
INSERT [dbo].[DatasetMapping] (Tag, MappingType, MappingOption, DestinationElement_Id, SourceElement_Id)
	VALUES ('Spontaneous', 1, '', @dceid, (select top 1 dce.Id from [dbo].[DatasetCategoryElement] dce inner join [dbo].[DatasetElement] de on dce.DatasetElement_Id = de.Id where de.ElementName = 'Reporter Profession'))
set @dmid = (SELECT @@IDENTITY)
-- mapping values
INSERT [dbo].[DatasetMappingValue] (SourceValue, DestinationValue, Active, Mapping_Id)
	SELECT 'Physician', '1=Physician', 1, @dmid UNION
	SELECT 'Pharmacist', '2=Pharmacist', 1, @dmid UNION
	SELECT 'Other Health Professional', '3=Other Health Professional', 1, @dmid UNION
	SELECT 'Consumer or other non-health professional', '5=Consumer or other non health professional', 1, @dmid UNION
	SELECT 'Lawyer', '4=Lawyer', 1, @dmid

-- studyname
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 100, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Study Name', @fid, 1, NULL, NULL, 0, '18C32C1D-5836-4989-8FB5-E6BA9D1929EC')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('studyname', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- sponsorstudynumb
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 35, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Sponsor Study Number', @fid, 1, NULL, NULL, 0, '6C515A14-C80F-40BA-8EC6-E7F63D76DDA3')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
set @dceid = (SELECT @@IDENTITY)
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('sponsorstudynumb', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- observestudytype
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('1=Clinical trials', 1, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('2=Individual patient use', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('3=Other studies', 0, 0, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Observation Study Type', @fid, 1, NULL, NULL, 0, '3FC0A86F-91B6-4FC3-B0BB-CE816EB8B678')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('observestudytype', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

/**************************************************
CATEGORY sender
**************************************************/
INSERT [dbo].[DatasetCategory] ([DatasetCategoryName], [CategoryOrder], [Dataset_Id]) 
	VALUES (N'Sender', 1, @dsid)
set @dscid = (SELECT @@IDENTITY)

-- category node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('sender', 2, NULL, GETDATE(), GETDATE(), @dsxcnid, 1, NULL, 1, @dsxid)
set @dsxcnid2 = (SELECT @@IDENTITY)

-- sendertype
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('1=Pharmaceutical Company', 1, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('2=Regulatory Authority', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('3=Health professional', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('4=Regional Pharmacovigilance Center', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('5=WHO Collaborating Center for International Drug Monitoring', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('6=Other', 0, 1, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Sender Type', @fid, 1, NULL, NULL, 0, '7FC33D4A-52FB-4D8C-B335-9FDCB34B5ADB')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('sendertype', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- senderorganization
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 60, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Sender Organization', @fid, 1, NULL, NULL, 0, '8A424851-9207-4FE2-9185-91058FC5B52C')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('senderorganization', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- senderdepartment
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 60, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Sender Department', @fid, 1, NULL, NULL, 0, '3B839633-209C-4A3F-8335-261E2BB46FEB')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('senderdepartment', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- sendertitle
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 10, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Sender Title', @fid, 1, NULL, NULL, 0, '99511301-5C65-401C-A556-CAA1BDBCEA2C')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('sendertitle', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- sendergivename
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 35, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Sender Given Name', @fid, 1, NULL, NULL, 0, '8F60CDCC-5F14-4EE2-8F9A-2981321E2B64')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('sendergivename', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- sendermiddlename
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 15, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Sender Middle Name', @fid, 1, NULL, NULL, 0, '24472874-1541-4C2A-9222-5064AD729A95')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('sendermiddlename', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- senderfamilyname
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 35, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Sender Family Name', @fid, 1, NULL, NULL, 0, '3A035EF1-518A-4B51-9645-98474B4EF239')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('senderfamilyname', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- senderstreetaddress
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 100, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Sender Street Address', @fid, 1, NULL, NULL, 0, '8AB7F535-2E22-4623-8276-9AEF2F1521CD')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('senderstreetaddress', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- sendercity
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 35, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Sender City', @fid, 1, NULL, NULL, 0, '244CFD5D-31F8-477F-A7E5-577B74F6C56A')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('sendercity', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- senderstate
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 40, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Sender State', @fid, 1, NULL, NULL, 0, '96003CE9-6D13-4F7D-BB39-B545AC4427CE')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('senderstate', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- senderpostcode
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 15, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Sender Postcode', @fid, 1, NULL, NULL, 0, '9655D5B8-08B2-44CA-A461-2AE9009769A0')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('senderpostcode', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- sendercountry
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 2, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Sender Country', @fid, 1, NULL, 'PH', 0, 'A7A751A1-6276-4F6E-B44D-D7ECB5ED50A6')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('sendercountrycode', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- sendertel
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 10, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Sender Tel Number', @fid, 1, NULL, NULL, 0, '5FAC3FDA-568F-4014-BDF8-8D8E93A35952')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('sendertel', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- sendertelextension
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 5, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Sender Tel Extension', @fid, 1, NULL, NULL, 0, '3F3037D3-61B5-418B-93B9-9C71F4AEB6CB')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('sendertelextension', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- sendertelcountrycode
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 3, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Sender Tel Country Code', @fid, 1, NULL, NULL, 0, '2A2FC1FA-4A1E-482C-9C61-B3DF0F87BBCE')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('sendertelcountrycode', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- senderfax
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 10, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Sender Fax', @fid, 1, NULL, NULL, 0, '85C0BB0F-FF3A-4BC8-BE4A-EC09169C9996')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('senderfax', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- senderfaxextension
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 5, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Sender Fax Extension', @fid, 1, NULL, NULL, 0, '56325F68-BCB2-448B-8DAD-E546DE8DE3E1')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('senderfaxextension', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- senderfaxcountrycode
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 3, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Sender Fax Country Code', @fid, 1, NULL, NULL, 0, '15A9946C-10D6-401A-ADBE-2A17C066F1BA')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('senderfaxcountrycode', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- senderemailaddress
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 100, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Sender Email Address', @fid, 1, NULL, NULL, 0, 'BB8572C3-0256-4CB3-A125-6BDAEC29001C')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('senderemailaddress', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

/**************************************************
CATEGORY receiver
**************************************************/
INSERT [dbo].[DatasetCategory] ([DatasetCategoryName], [CategoryOrder], [Dataset_Id]) 
	VALUES (N'Receiver', 1, @dsid)
set @dscid = (SELECT @@IDENTITY)

-- category node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('receiver', 2, NULL, GETDATE(), GETDATE(), @dsxcnid, 1, NULL, 1, @dsxid)
set @dsxcnid2 = (SELECT @@IDENTITY)

-- receivertype
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('1=Pharmaceutical Company', 1, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('2=Regulatory Authority', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('3=Health professional', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('4=Regional Pharmacovigilance Center', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('5=WHO Collaborating Center for International Drug Monitoring', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('6=Other', 0, 1, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Receiver Type', @fid, 1, NULL, NULL, 0, '049C0849-598E-4857-B9F2-13E5AD7A1036')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('receivertype', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- receiverorganization
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 60, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Receiver Organization', @fid, 1, NULL, NULL, 0, 'DE780DB3-57A1-410B-BA37-A298A315D957')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('receiverorganization', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- receiverdepartment
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 60, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Receiver Department', @fid, 1, NULL, NULL, 0, '612635C6-A35A-4A7B-8489-90BC89DDE264')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('receiverdepartment', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- receivertitle
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 10, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Receiver Title', @fid, 1, NULL, NULL, 0, '87099047-E3C3-437A-8189-F879AA545D8A')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('receivertitle', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- receivergivename
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 35, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Receiver Given Name', @fid, 1, NULL, NULL, 0, '95C21AD7-2B4A-4776-9D39-11D483E83515')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('receivergivename', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- receivermiddlename
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 15, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Receiver Middle Name', @fid, 1, NULL, NULL, 0, 'E5173A39-C87E-4344-8AED-125989F3901A')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('receivermiddlename', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- receiverfamilyname
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 35, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Receiver Family Name', @fid, 1, NULL, NULL, 0, 'FB794A91-46C6-4D35-89A2-F294E73B8192')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('receiverfamilyname', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- receiverstreetaddress
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 100, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Receiver Street Address', @fid, 1, NULL, NULL, 0, '9F9F377D-544F-470B-B679-8D0F804F7589')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('receiverstreetaddress', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- receivercity
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 35, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Receiver City', @fid, 1, NULL, NULL, 0, '6952040B-FDAF-41A8-8C13-AC3A498815A6')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('receivercity', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- receiverstate
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 40, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Receiver State', @fid, 1, NULL, NULL, 0, 'C859C881-375E-4718-A78C-4D406760EB53')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('receiverstate', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- receiverpostcode
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 15, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Receiver Postcode', @fid, 1, NULL, NULL, 0, '0FE8D275-877E-4FAC-BD38-72B9A3DD4FBD')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('receiverpostcode', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- receivercountry
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 2, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Receiver Country', @fid, 1, NULL, 'SE', 0, '13ACE7B7-3AD9-4624-A624-BA2D24B4E1B5')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('receivercountrycode', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- receivertel
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 10, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Receiver Tel', @fid, 1, NULL, NULL, 0, 'F88E3A34-55FE-4040-811E-BA0224F5F7CF')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('receivertel', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- receivertelextension
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 5, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Receiver Tel Extension', @fid, 1, NULL, NULL, 0, '9C5BAF56-00CF-4836-BB6F-BC8ABAADFB15')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('receivertelextension', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- receivertelcountrycode
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 3, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Receiver Tel Country Code', @fid, 1, NULL, NULL, 0, '9F9D2525-1F9C-44BE-8E50-EC80290B0A4C')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('receivertelcountrycode', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- receiverfax
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 10, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Receiver Fax', @fid, 1, NULL, NULL, 0, '592D3456-1F6E-4473-8672-3529C4D80E15')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('receiverfax', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- receiverfaxextension
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 5, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Receiver Fax Extension', @fid, 1, NULL, NULL, 0, '3BFBAE6D-8F3C-42FD-BF13-B1F9DA59A4D0')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('receiverfaxextension', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- receiverfaxcountrycode
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 3, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Receiver Fax Country Code', @fid, 1, NULL, NULL, 0, 'E42384B7-73BD-4E9D-804B-A8A0F87B056D')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('receiverfaxcountrycode', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- receiveremailaddress
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 100, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Receiver Email Address', @fid, 1, NULL, NULL, 0, '479F57AB-6224-422E-84B0-26030A3A761E')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('receiveremailaddress', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

/**************************************************
CATEGORY patient
**************************************************/
INSERT [dbo].[DatasetCategory] ([DatasetCategoryName], [CategoryOrder], [Dataset_Id]) 
	VALUES (N'Patient', 1, @dsid)
set @dscid = (SELECT @@IDENTITY)

-- category node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('patient', 2, NULL, GETDATE(), GETDATE(), @dsxcnid, 1, NULL, 1, @dsxid)
set @dsxcnid2 = (SELECT @@IDENTITY)

-- patientinitial
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 10, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Patient Initial', @fid, 1, NULL, NULL, 0, 'A0BEAB3A-0B0A-457E-B190-1B66FE60CA73')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
set @dceid = (SELECT @@IDENTITY)
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('patientinitial', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)
-- mapping
INSERT [dbo].[DatasetMapping] (Tag, MappingType, MappingOption, DestinationElement_Id, SourceElement_Id)
	VALUES ('Spontaneous', 0, '', @dceid, (select top 1 dce.Id from [dbo].[DatasetCategoryElement] dce inner join [dbo].[DatasetElement] de on dce.DatasetElement_Id = de.Id where de.ElementName = 'Initials'))

-- patientgpmedicalrecordnumb
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 20, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Patient GP Medical Record Number', @fid, 1, NULL, NULL, 0, '5282F84F-064A-40A3-A0C2-087316A70763')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
set @dceid = (SELECT @@IDENTITY)
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('patientgpmedicalrecordnumb', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)
-- mapping
INSERT [dbo].[DatasetMapping] (Tag, MappingType, MappingOption, DestinationElement_Id, PropertyPath, Property)
	VALUES ('Active', 2, '', @dceid, 'Patient', 'Medical Record Number')

-- patientspecialistrecordnumb
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 20, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Patient Specialist Record Number', @fid, 1, NULL, NULL, 0, 'A2B070BF-135A-48D2-9ECC-89314F609E34')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('patientspecialistrecordnumb', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 20, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Patient Hospital Record Number', @fid, 1, NULL, NULL, 0, '9EB91985-550B-4AE2-B8F7-74A31F3E9737')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('patienthospitalrecordnumb', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 20, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Patient Investigation Number', @fid, 1, NULL, NULL, 0, '5ABFCFE7-93D5-4819-B154-9D26AF0A372A')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('patientinvestigationnumb', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 10, NULL, 8)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Patient Birthdate Format', @fid, 1, NULL, '102', 1, '7BA95D72-126B-4886-9FC7-8099A2A0D494')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('patientbirthdateformat', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 6)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Patient Birthdate', @fid, 1, NULL, NULL, 0, '4F71B7F4-4317-4680-B3A3-9C1C1F72AD6A')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('patientbirthdate', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [Decimals], [MaxSize], [MinSize], [FieldType_Id]) 
	VALUES (0, 0, 0, 999999, 1, 4)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Patient Onset Age', @fid, 1, NULL, NULL, 0, 'E10C259B-DD2C-4F19-9D41-16FDDF9C5807')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
set @dceid = (SELECT @@IDENTITY)
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('patientonsetage', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)
-- mapping
INSERT [dbo].[DatasetMapping] (Tag, MappingType, MappingOption, DestinationElement_Id, SourceElement_Id)
	VALUES ('Spontaneous', 0, '', @dceid, (select top 1 dce.Id from [dbo].[DatasetCategoryElement] dce inner join [dbo].[DatasetElement] de on dce.DatasetElement_Id = de.Id where de.ElementName = 'Age'))

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('800=Decade', 1, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('801=Year', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('802=Month', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('803=Week', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('804=Day', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('805=Hour', 0, 1, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Patient Onset Age Unit', @fid, 1, NULL, NULL, 0, 'CA9B94C2-E1EF-407B-87C3-181224AF637A')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
set @dceid = (SELECT @@IDENTITY)
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('patientonsetageunit', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)
-- mapping
INSERT [dbo].[DatasetMapping] (Tag, MappingType, MappingOption, DestinationElement_Id, SourceElement_Id)
	VALUES ('Spontaneous', 1, '', @dceid, (select top 1 dce.Id from [dbo].[DatasetCategoryElement] dce inner join [dbo].[DatasetElement] de on dce.DatasetElement_Id = de.Id where de.ElementName = 'Age Unit'))
set @dmid = (SELECT @@IDENTITY)
-- mapping values
INSERT [dbo].[DatasetMappingValue] (SourceValue, DestinationValue, Active, Mapping_Id)
	SELECT 'Years', '801=Year', 1, @dmid UNION
	SELECT 'Months', '802=Month', 1, @dmid UNION
	SELECT 'Weeks', '803=Week', 1, @dmid UNION
	SELECT 'Days', '804=Day', 1, @dmid

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [Decimals], [MaxSize], [MinSize], [FieldType_Id]) 
	VALUES (0, 0, 0, 50, 1, 4)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Gestation Period', @fid, 1, NULL, NULL, 0, 'B6BE9689-B6B2-4FCF-8918-664AFC91A4E0')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('gestationperiod', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('802=Month', 1, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('803=Week', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('804=Day', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('810=Trimester', 0, 0, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Gestation Period Unit', @fid, 1, NULL, NULL, 0, '1F174413-2A1E-45BD-B5C4-0C8F5DFFBFF4')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('gestationperiodunit', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('1=Neonate', 1, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('2=Infant', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('3=Child', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('4=Adolescent', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('5=Adult', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('6=Elderly', 0, 0, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Patient Age Group', @fid, 1, NULL, NULL, 0, '3BBF80E4-4FDA-4462-BDFE-DD621A43C144')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('patientagegroup', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [Decimals], [MaxSize], [MinSize], [FieldType_Id]) 
	VALUES (0, 0, 1, 200.0, 1.0, 4)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Patient Weight', @fid, 1, NULL, NULL, 0, '89A6E687-A220-4319-AAC1-AFBB55C81873')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
set @dceid = (SELECT @@IDENTITY)
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('patientweight', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)
-- mapping
INSERT [dbo].[DatasetMapping] (Tag, MappingType, MappingOption, DestinationElement_Id, SourceElement_Id)
	VALUES ('Spontaneous', 0, '', @dceid, (select top 1 dce.Id from [dbo].[DatasetCategoryElement] dce inner join [dbo].[DatasetElement] de on dce.DatasetElement_Id = de.Id inner join dbo.[DatasetCategory] dc on dce.DatasetCategory_Id = dc.Id where dc.DatasetCategoryName = 'Patient Information' and de.ElementName = 'Weight (kg)'))

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [Decimals], [MaxSize], [MinSize], [FieldType_Id]) 
	VALUES (0, 0, 0, 300, 1, 4)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Patient Height', @fid, 1, NULL, NULL, 0, '40DAD435-8282-4B3E-B65E-3478FF55D028')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('patientheight', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('1=Male', 1, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('2=Female', 0, 0, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Patient Sex', @fid, 1, NULL, NULL, 0, '59498520-172C-42BC-B30C-E249F94E412A')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
set @dceid = (SELECT @@IDENTITY)
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('patientsex', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)
-- mapping 
INSERT [dbo].[DatasetMapping] (Tag, MappingType, MappingOption, DestinationElement_Id, SourceElement_Id)
	VALUES ('Spontaneous', 1, '', @dceid, (select top 1 dce.Id from [dbo].[DatasetCategoryElement] dce inner join [dbo].[DatasetElement] de on dce.DatasetElement_Id = de.Id where de.ElementName = 'Sex'))
set @dmid = (SELECT @@IDENTITY)
-- mapping values
INSERT [dbo].[DatasetMappingValue] (SourceValue, DestinationValue, Active, Mapping_Id)
	SELECT 'Male', '1=Male', 1, @dmid UNION
	SELECT 'Female', '2=Female', 1, @dmid
-- mapping 
INSERT [dbo].[DatasetMapping] (Tag, MappingType, MappingOption, DestinationElement_Id, PropertyPath, Property)
	VALUES ('Active', 3, '', @dceid, 'Patient', 'Gender')
set @dmid = (SELECT @@IDENTITY)
-- mapping values
INSERT [dbo].[DatasetMappingValue] (SourceValue, DestinationValue, Active, Mapping_Id)
	SELECT '1', '1=Male', 1, @dmid UNION
	SELECT '2', '2=Female', 1, @dmid

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 10, NULL, 8)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Last Menstrual Date Format', @fid, 1, NULL, '102', 1, '075635FC-AFD3-470D-A1FD-FB878933B277')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('lastmenstrualdateformat', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 6)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Patient Last Menstrual Date', @fid, 1, NULL, NULL, 0, '93253F91-60D1-4161-AF1A-F3ABDD140CB9')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('patientlastmenstrualdate', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 500, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Patient Medical History Text', @fid, 1, NULL, NULL, 0, '74A67806-3CF8-4A18-AE26-7CA5CB39E184')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('patientmedicalhistorytext', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 500, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Results Tests Procedures', @fid, 1, NULL, NULL, 0, '46C122C7-1E12-4419-9855-7A541B599A6D')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('resultstestsprocedures', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

/**************************************************
CATEGORY medicalhistoryepisode
**************************************************/
INSERT [dbo].[DatasetCategory] ([DatasetCategoryName], [CategoryOrder], [Dataset_Id]) 
	VALUES (N'Medical History Episode', 1, @dsid)
set @dscid = (SELECT @@IDENTITY)

-- Medical History Episode Table
INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (0, NULL, NULL, NULL, NULL, '', NULL, '', 0, 7) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid])
	VALUES ('Medical History Episodes', @fid, 1, '', '', 0, 'EA92BEF5-8667-4A0D-9D4F-D48ACC906C4B') 
set @deid = (SELECT @@IDENTITY)
-- category node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('medicalhistoryepisode', 3, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxcnid3 = (SELECT @@IDENTITY)

INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (10, @dscid, @deid, 0, 0, 'Medical History Episodes', 'Medical History Episodes') 
set @dceid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
			VALUES (0, 0, 10, NULL, 8)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Patient Episode Name MedDRA Version', @fid, @deid, '', '23.0', 1, 1)
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('patientepisodenamemeddraversion', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
			VALUES (0, 0, 250, NULL, 3)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Patient Episode Name', @fid, @deid, '', '', 0, 2)
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('patientepisodename', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
			VALUES (0, 0, 10, NULL, 8)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Patient Medical Start Date Format', @fid, @deid, '', '102', 1, 3)
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('patientmedicalstartdateformat', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
			VALUES (0, 0, 6)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Patient Medical Start Date', @fid, @deid, '', '', 0, 4)
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('patientmedicalstartdate', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
			VALUES (0, 0, 2)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('1=Yes', 1, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('2=No', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('3=Unknown', 0, 0, 0, @fid)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Patient Medical Continue', @fid, @deid, '', '', 0, 5)
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('patientmedicalcontinue', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
			VALUES (0, 0, 10, NULL, 8)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Patient Medical End Date Format', @fid, @deid, '', '102', 1, 6)
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('patientmedicalenddateformat', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
			VALUES (0, 0, 6)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Patient Medical End Date', @fid, @deid, '', '', 0, 7)
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('patientmedicalenddate', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
			VALUES (0, 0, 100, NULL, 3)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Patient Medical Comment', @fid, @deid, '', '', 0, 8)
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('patientmedicalcomment', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

/**************************************************
CATEGORY patientpastdrugtherapy
**************************************************/
INSERT [dbo].[DatasetCategory] ([DatasetCategoryName], [CategoryOrder], [Dataset_Id]) 
	VALUES (N'Past Drug Therapy', 1, @dsid)
set @dscid = (SELECT @@IDENTITY)

-- Past Drug Therapy Table
INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (0, NULL, NULL, NULL, NULL, '', NULL, '', 0, 7) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid])
	VALUES ('Past Drug Therapy', @fid, 1, '', '', 0, '52EE53B4-F1A5-41D8-9903-FF8A07C0360D') 
set @deid = (SELECT @@IDENTITY)

-- category node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('patientpastdrugtherapy', 3, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxcnid3 = (SELECT @@IDENTITY)

INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (10, @dscid, @deid, 0, 0, 'Past Drug Therapy', 'Past Drug Therapy') 
set @dceid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
			VALUES (0, 0, 100, NULL, 3)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Drug Name', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('patientdrugname', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
			VALUES (0, 0, 10, NULL, 8)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Drug Start Date Format', @fid, @deid, '', '102', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('patientdrugstartdateformat', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
			VALUES (0, 0, 6)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Drug Start Date', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('patientdrugstartdate', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
			VALUES (0, 0, 10, NULL, 8)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Drug End Date Format', @fid, @deid, '', '102', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('patientdrugenddateformat', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
			VALUES (0, 0, 6)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Drug End Date', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('patientdrugenddate', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
			VALUES (0, 0, 10, NULL, 8)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Indication MedDRA Version', @fid, @deid, '', '23.0', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('patientindicationmeddraversion', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
			VALUES (0, 0, 250, NULL, 3)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Indication', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('patientdrugindication', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
			VALUES (0, 0, 10, NULL, 8)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Drug Reaction MedDRA Version', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('patientdrgreactionmeddraversion', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
			VALUES (0, 0, 250, NULL, 3)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Drug Reaction', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('patientdrugreaction', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

/**************************************************
CATEGORY patientdeath
**************************************************/
INSERT [dbo].[DatasetCategory] ([DatasetCategoryName], [CategoryOrder], [Dataset_Id]) 
	VALUES (N'Patient Death', 1, @dsid)
set @dscid = (SELECT @@IDENTITY)

-- category node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('patientdeath', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, NULL, 1, @dsxid)
set @dsxcnid3 = (SELECT @@IDENTITY)

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 10, NULL, 8)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Patient Death Date Format', @fid, 1, NULL, '102', 1, '5FCB1E71-F295-4913-B795-5C8607E411E2')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('patientdeathdateformat', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 6)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Patient Death Date', @fid, 1, NULL, NULL, 0, '58EC2E37-ECA4-4C9A-9423-88755D392CB1')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
set @dceid = (SELECT @@IDENTITY)
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('patientdeathdate', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)
-- mapping
INSERT [dbo].[DatasetMapping] (Tag, MappingType, MappingOption, DestinationElement_Id, SourceElement_Id)
	VALUES ('Spontaneous', 0, 'yyyyMMdd', @dceid, (select top 1 dce.Id from [dbo].[DatasetCategoryElement] dce inner join [dbo].[DatasetElement] de on dce.DatasetElement_Id = de.Id where de.ElementName = 'Reaction date of death'))
-- mapping
INSERT [dbo].[DatasetMapping] (Tag, MappingType, MappingOption, DestinationElement_Id, PropertyPath, Property)
	VALUES ('Active', 2, 'yyyyMMdd', @dceid, '', 'Date of Death')

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('1=Yes', 1, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('2=No', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('3=Unknown', 0, 0, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Patient Autopsy', @fid, 1, NULL, NULL, 0, 'BCE172A8-F4DD-4372-8ACC-9A4598DABDA1')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
set @dceid = (SELECT @@IDENTITY)
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('patientautopsyyesno', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)
-- mapping 
INSERT [dbo].[DatasetMapping] (Tag, MappingType, MappingOption, DestinationElement_Id, PropertyPath, Property)
	VALUES ('Active', 3, '', @dceid, '', 'Autopsy Done')
set @dmid = (SELECT @@IDENTITY)
-- mapping values
INSERT [dbo].[DatasetMappingValue] (SourceValue, DestinationValue, Active, Mapping_Id)
	SELECT 'Yes', '1=Yes', 1, @dmid UNION
	SELECT 'No', '2=No', 1, @dmid

-- category node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('patientdeathcause', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, NULL, 1, @dsxid)
set @dsxcnid4 = (SELECT @@IDENTITY)

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 10, NULL, 8)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('MedDRA version for reported cause(s) of death', @fid, 1, NULL, '23.0', 1, '7160878D-8AE0-477A-84F6-BCE195EC4653')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('patientdeathreportmeddraversion', 2, NULL, GETDATE(), GETDATE(), @dsxcnid4, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 250, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Patient Death Report', @fid, 1, NULL, NULL, 0, '6EAB448F-34F0-4B15-86CE-5423BA5EB7CC')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('patientdeathreport', 2, NULL, GETDATE(), GETDATE(), @dsxcnid4, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- category node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('patientautopsy', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, NULL, 1, @dsxid)
set @dsxcnid4 = (SELECT @@IDENTITY)

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 10, NULL, 8)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Patient Determined Autopsy MedDRA Version', @fid, 1, NULL, '23.0', 1, 'FC7863E3-8480-465E-B56D-1401AB19FCF9')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('patientdetermautopsmeddraversion', 2, NULL, GETDATE(), GETDATE(), @dsxcnid4, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 250, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Patient Determine Autopsy', @fid, 1, NULL, NULL, 0, '037E2870-DAFC-4D64-93FC-88159A6FCFB6')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('patientdetermineautopsy', 2, NULL, GETDATE(), GETDATE(), @dsxcnid4, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

/**************************************************
CATEGORY reaction
**************************************************/
INSERT [dbo].[DatasetCategory] ([DatasetCategoryName], [CategoryOrder], [Dataset_Id]) 
	VALUES (N'Reaction', 1, @dsid)
set @dscid = (SELECT @@IDENTITY)

-- category node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('reaction', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, NULL, 1, @dsxid)
set @dsxcnid3 = (SELECT @@IDENTITY)

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 200, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Primary Source Reaction', @fid, 1, NULL, NULL, 0, 'B83251CA-441B-4E80-B5D7-51B9DB3724CD')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
set @dceid = (SELECT @@IDENTITY)
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('primarysourcereaction', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)
-- mapping
INSERT [dbo].[DatasetMapping] (Tag, MappingType, MappingOption, DestinationElement_Id, SourceElement_Id)
	VALUES ('Spontaneous', 0, '', @dceid, (select top 1 dce.Id from [dbo].[DatasetCategoryElement] dce inner join [dbo].[DatasetElement] de on dce.DatasetElement_Id = de.Id where de.ElementName = 'Description of reaction'))
-- mapping
INSERT [dbo].[DatasetMapping] (Tag, MappingType, MappingOption, DestinationElement_Id, PropertyPath, Property)
	VALUES ('Active', 4, '', @dceid, '', 'SourceDescription')

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 10, NULL, 8)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Reaction MedDRA Version LLT', @fid, 1, NULL, '23.0', 1, '97DAA650-CDEA-4D26-A253-16F80223082E')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('reactionmeddraversionllt', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 250, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Reaction MedDRA LLT', @fid, 1, NULL, NULL, 0, 'C8DD9A5E-BD9A-488D-8ABF-171271F5D370')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('reactionmeddrallt', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 10, NULL, 8)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Reaction MedDRA Version PT', @fid, 1, NULL, '23.0', 1, '62787E91-5A92-47C3-9A4E-03D4CAB32046')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('reactionmeddraversionpt', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 250, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Reaction MedDRA PT', @fid, 1, NULL, NULL, 0, 'F2B8A131-4110-43A5-8BD5-C87F847CF0D8')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('reactionmeddrapt', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('1=Yes, highlighted by the reporter, NOT serious', 1, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('2=No, not highlighted by the reporter, NOT serious', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('3=Yes, highlighted by the reporter, SERIOUS', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('4=No, not highlighted by the reporter, SERIOUS', 0, 0, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Term Highlighted', @fid, 1, NULL, NULL, 0, 'A14050C2-304F-472E-A520-05F73059EC6F')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('termhighlighted', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 10, NULL, 8)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Reaction Start Date Format', @fid, 1, NULL, '102', 1, '86A75138-C2D9-411D-ADA4-0F9A972AD8B3')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('reactionstartdateformat', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 6)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Reaction Start Date', @fid, 1, NULL, NULL, 0, '1EAD9E11-60E6-4B27-9A4D-4B296B169E90')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('reactionstartdate', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 10, NULL, 8)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Reaction End Date Format', @fid, 1, NULL, '102', 1, '4BF4AF26-B6BF-4E23-94C7-672A015669D4')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('reactionenddateformat', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 6)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Reaction End Date', @fid, 1, NULL, NULL, 0, '3A0F240E-8B36-48F6-9527-77E55F6E7CF1')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
set @dceid = (SELECT @@IDENTITY)
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('reactionenddate', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)
-- mapping
INSERT [dbo].[DatasetMapping] (Tag, MappingType, MappingOption, DestinationElement_Id, SourceElement_Id)
	VALUES ('Spontaneous', 0, 'yyyyMMdd', @dceid, (select top 1 dce.Id from [dbo].[DatasetCategoryElement] dce inner join [dbo].[DatasetElement] de on dce.DatasetElement_Id = de.Id where de.ElementName = 'Reaction date of recovery'))

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [Decimals], [MaxSize], [MinSize], [FieldType_Id]) 
	VALUES (0, 0, 0, 150, 0, 4)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Reaction Duration', @fid, 1, NULL, NULL, 0, '0712C664-2ADD-44C0-B8D5-B6E83FB01F42')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('reactionduration', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('800=Decade', 1, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('801=Year', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('802=Month', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('803=Week', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('804=Day', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('805=Hour', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('806=Minute', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('807=Second', 0, 0, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Reaction Duration Unit', @fid, 1, NULL, NULL, 0, 'F96E702D-DCC5-455A-AB45-CAEFF25BF82A')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('reactiondurationunit', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [Decimals], [MaxSize], [MinSize], [FieldType_Id]) 
	VALUES (0, 0, 0, 150, 1, 4)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Reaction First Time', @fid, 1, NULL, NULL, 0, '81FAC14E-28C9-4069-9C9D-8C22D2691545')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('reactionfirsttime', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('800=Decade', 1, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('801=Year', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('802=Month', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('803=Week', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('804=Day', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('805=Hour', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('806=Minute', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('807=Second', 0, 0, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Reaction First Time Unit', @fid, 1, NULL, NULL, 0, '4D4D6E6C-DCCA-458D-AF77-AB570A26AAE8')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('reactionfirsttimeunit', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [Decimals], [MaxSize], [MinSize], [FieldType_Id]) 
	VALUES (0, 0, 0, 150, 1, 4)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Reaction Last Time', @fid, 1, NULL, NULL, 0, '1474BBD8-FC66-4CBB-9DC8-BA865FC434DE')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('reactionlasttime', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('800=Decade', 1, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('801=Year', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('802=Month', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('803=Week', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('804=Day', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('805=Hour', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('806=Minute', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('807=Second', 0, 0, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Reaction Last Time Unit', @fid, 1, NULL, NULL, 0, '31A741CE-BA2C-432C-9FCD-61CE8A6D9E4E')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('reactionlasttimeunit', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('1=recovered/resolved', 1, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('2=recovering/resolving', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('3=not recovered/not resolved', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('4=recovered/resolved with sequelae', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('5=fatal', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('6=unknown', 0, 0, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Reaction Outcome', @fid, 1, NULL, NULL, 0, '315C0769-0453-4C5C-A9CD-2644F7B6CE17')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
set @dceid = (SELECT @@IDENTITY)
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('reactionoutcome', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)
-- mapping
INSERT [dbo].[DatasetMapping] (Tag, MappingType, MappingOption, DestinationElement_Id, SourceElement_Id)
	VALUES ('Spontaneous', 1, '', @dceid, (select top 1 dce.Id from [dbo].[DatasetCategoryElement] dce inner join [dbo].[DatasetElement] de on dce.DatasetElement_Id = de.Id where de.ElementName = 'Outcome of reaction'))
set @dmid = (SELECT @@IDENTITY)
-- mapping values
INSERT [dbo].[DatasetMappingValue] (SourceValue, DestinationValue, Active, Mapping_Id)
	SELECT 'Recovering/resolving', '2=recovering/resolving', 1, @dmid UNION
	SELECT 'Not recovered/not resolved', '3=not recovered/not resolved', 1, @dmid UNION
	SELECT 'Recovered/resolved with permanent complications', '4=recovered/resolved with sequelae', 1, @dmid UNION
	SELECT 'Fatal', '5=fatal', 1, @dmid UNION
	SELECT 'Recovered/resolved', '1=recovered/resolved', 1, @dmid UNION
	SELECT 'Unknown', '6=unknown', 1, @dmid
-- mapping
INSERT [dbo].[DatasetMapping] (Tag, MappingType, MappingOption, DestinationElement_Id, PropertyPath, Property)
	VALUES ('Active', 3, '', @dceid, '', 'Outcome')
set @dmid = (SELECT @@IDENTITY)
-- mapping values
INSERT [dbo].[DatasetMappingValue] (SourceValue, DestinationValue, Active, Mapping_Id)
	SELECT '4', '2=recovering/resolving', 1, @dmid UNION
	SELECT '3', '5=fatal', 1, @dmid UNION
	SELECT '2', '4=recovered/resolved with sequelae', 1, @dmid UNION
	SELECT '5', '3=not recovered/not resolved', 1, @dmid UNION
	SELECT '1', '1=recovered/resolved', 1, @dmid UNION
	SELECT '6', '6=unknown', 1, @dmid

/**************************************************
CATEGORY test
**************************************************/
INSERT [dbo].[DatasetCategory] ([DatasetCategoryName], [CategoryOrder], [Dataset_Id]) 
	VALUES (N'Test', 1, @dsid)
set @dscid = (SELECT @@IDENTITY)

-- Test Table
INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (0, NULL, NULL, NULL, NULL, '', NULL, '', 0, 7) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid])
	VALUES ('Test History', @fid, 1, '', '', 0, '693A2E8C-B041-46E7-8687-0A42E6B3C82E') 
set @deid = (SELECT @@IDENTITY)

-- category node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('test', 3, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxcnid3 = (SELECT @@IDENTITY)

INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (10, @dscid, @deid, 0, 0, 'Test History', 'Test History') 
set @dceid = (SELECT @@IDENTITY)

		set @sdceid = (select top 1 dce.Id from [dbo].[DatasetCategoryElement] dce inner join [dbo].[DatasetElement] de on dce.DatasetElement_Id = de.Id where de.ElementName = 'Test Results')
		-- mapping
		INSERT [dbo].[DatasetMapping] (Tag, MappingType, MappingOption, DestinationElement_Id, SourceElement_Id)
			VALUES ('Spontaneous', 0, '', @dceid, @sdceid)
		set @dmid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
			VALUES (0, 0, 10, NULL, 8)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Test Date Format', @fid, @deid, '', '102', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('testdateformat', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
			VALUES (0, 0, 6)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Test Date', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('testdate', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)
		-- submapping
		INSERT [dbo].[DatasetMappingSub] (MappingType, MappingOption, DestinationElement_Id, Mapping_Id, SourceElement_Id)
			VALUES (0, 'yyyyMMdd', @desid, @dmid, (select top 1 desu.Id from [dbo].[DatasetElementSub] desu inner join [dbo].[DatasetElement] de on desu.DatasetElement_Id = de.Id where de.ElementName = 'Test Results' and desu.ElementName = 'Test Date'))
		set @dmsid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
			VALUES (0, 0, 100, NULL, 3)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Test Name', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('testname', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)
		-- submapping
		INSERT [dbo].[DatasetMappingSub] (MappingType, MappingOption, DestinationElement_Id, Mapping_Id, SourceElement_Id)
			VALUES (0, '', @desid, @dmid, (select top 1 desu.Id from [dbo].[DatasetElementSub] desu inner join [dbo].[DatasetElement] de on desu.DatasetElement_Id = de.Id where de.ElementName = 'Test Results' and desu.ElementName = 'Test Name'))
		set @dmsid = (SELECT @@IDENTITY)
		
		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
			VALUES (0, 0, 50, NULL, 3)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Test Result', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('testresult', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)
		-- submapping
		INSERT [dbo].[DatasetMappingSub] (MappingType, MappingOption, DestinationElement_Id, Mapping_Id, SourceElement_Id)
			VALUES (0, '', @desid, @dmid, (select top 1 desu.Id from [dbo].[DatasetElementSub] desu inner join [dbo].[DatasetElement] de on desu.DatasetElement_Id = de.Id where de.ElementName = 'Test Results' and desu.ElementName = 'Test Result'))
		set @dmsid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
			VALUES (0, 0, 35, NULL, 3)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Test Unit', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('testunit', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)
		-- submapping
		INSERT [dbo].[DatasetMappingSub] (MappingType, MappingOption, DestinationElement_Id, Mapping_Id, SourceElement_Id)
			VALUES (0, '', @desid, @dmid, (select top 1 desu.Id from [dbo].[DatasetElementSub] desu inner join [dbo].[DatasetElement] de on desu.DatasetElement_Id = de.Id where de.ElementName = 'Test Results' and desu.ElementName = 'Test Unit'))
		set @dmsid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
			VALUES (0, 0, 50, NULL, 3)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Low Test Range', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('lowtestrange', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)
		-- submapping
		INSERT [dbo].[DatasetMappingSub] (MappingType, MappingOption, DestinationElement_Id, Mapping_Id, SourceElement_Id)
			VALUES (0, '', @desid, @dmid, (select top 1 desu.Id from [dbo].[DatasetElementSub] desu inner join [dbo].[DatasetElement] de on desu.DatasetElement_Id = de.Id where de.ElementName = 'Test Results' and desu.ElementName = 'Low Test Range'))
		set @dmsid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
			VALUES (0, 0, 50, NULL, 3)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('High Test Range', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('hightestrange', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)
		-- submapping
		INSERT [dbo].[DatasetMappingSub] (MappingType, MappingOption, DestinationElement_Id, Mapping_Id, SourceElement_Id)
			VALUES (0, '', @desid, @dmid, (select top 1 desu.Id from [dbo].[DatasetElementSub] desu inner join [dbo].[DatasetElement] de on desu.DatasetElement_Id = de.Id where de.ElementName = 'Test Results' and desu.ElementName = 'High Test Range'))
		set @dmsid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
			VALUES (0, 0, 2)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('1=Yes', 1, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('2=No', 0, 0, 0, @fid)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('More Information', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('moreinformation', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)
		-- submapping
		INSERT [dbo].[DatasetMappingSub] (MappingType, MappingOption, DestinationElement_Id, Mapping_Id, SourceElement_Id)
			VALUES (1, '', @desid, @dmid, (select top 1 desu.Id from [dbo].[DatasetElementSub] desu inner join [dbo].[DatasetElement] de on desu.DatasetElement_Id = de.Id where de.ElementName = 'Test Results' and desu.ElementName = 'More Information'))
		set @dmsid = (SELECT @@IDENTITY)
		-- mapping values
		INSERT [dbo].[DatasetMappingValue] (SourceValue, DestinationValue, Active, Mapping_Id, SubMapping_Id)
			SELECT 'Yes', '1=Yes', 1, @dmid, @dmsid UNION
			SELECT 'No', '2=No', 1, @dmid, @dmsid

/**************************************************
CATEGORY drug
**************************************************/
INSERT [dbo].[DatasetCategory] ([DatasetCategoryName], [CategoryOrder], [Dataset_Id]) 
	VALUES (N'Drug', 1, @dsid)
set @dscid = (SELECT @@IDENTITY)

-- Medicinal Products Table
INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (0, NULL, NULL, NULL, NULL, '', NULL, '', 0, 7) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid])
	VALUES ('Medicinal Products', @fid, 1, '', '', 0, 'E033BDE8-EDC8-43FF-A6B0-DEA6D6FA581C') 
set @deid = (SELECT @@IDENTITY)
-- category node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('drug', 3, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxcnid3 = (SELECT @@IDENTITY)

INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (10, @dscid, @deid, 0, 0, 'Medicinal Products', 'Medicinal Products') 
set @dceid = (SELECT @@IDENTITY)

		set @sdceid = (select top 1 dce.Id from [dbo].[DatasetCategoryElement] dce inner join [dbo].[DatasetElement] de on dce.DatasetElement_Id = de.Id where de.ElementName = 'Product Information')
		-- mapping
		INSERT [dbo].[DatasetMapping] (Tag, MappingType, MappingOption, DestinationElement_Id, SourceElement_Id)
			VALUES ('Spontaneous', 0, '', @dceid, @sdceid)
		set @dmid = (SELECT @@IDENTITY)
		
		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
			VALUES (0, 0, 2)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('1=Suspect', 1, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('2=Concomitant', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('3=Interacting', 0, 0, 0, @fid)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Drug Characterization', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('drugcharacterization', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)
		-- submapping
		INSERT [dbo].[DatasetMappingSub] (MappingType, MappingOption, DestinationElement_Id, Mapping_Id, SourceElement_Id)
			VALUES (1, '', @desid, @dmid, (select top 1 desu.Id from [dbo].[DatasetElementSub] desu inner join [dbo].[DatasetElement] de on desu.DatasetElement_Id = de.Id where de.ElementName = 'Product Information' and desu.ElementName = 'Product Suspected'))
		set @dmsid = (SELECT @@IDENTITY)
		-- mapping values
		INSERT [dbo].[DatasetMappingValue] (SourceValue, DestinationValue, Active, Mapping_Id, SubMapping_Id)
			SELECT 'Yes', '1=Suspect', 1, @dmid, @dmsid UNION
			SELECT 'No', '2=Concomitant', 1, @dmid, @dmsid

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
			VALUES (0, 0, 70, NULL, 3)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Medicinal Product', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('medicinalproduct', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)
		-- submapping
		INSERT [dbo].[DatasetMappingSub] (MappingType, MappingOption, DestinationElement_Id, Mapping_Id, SourceElement_Id)
			VALUES (0, '', @desid, @dmid, (select top 1 desu.Id from [dbo].[DatasetElementSub] desu inner join [dbo].[DatasetElement] de on desu.DatasetElement_Id = de.Id where de.ElementName = 'Product Information' and desu.ElementName = 'Product'))
		set @dmsid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
			VALUES (0, 0, 2, NULL, 3)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Obtain Drug Country', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('obtaindrugcountry', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
			VALUES (0, 0, 35, NULL, 3)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Batch Number', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('drugbatchnumb', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)
		-- submapping
		INSERT [dbo].[DatasetMappingSub] (MappingType, MappingOption, DestinationElement_Id, Mapping_Id, SourceElement_Id)
			VALUES (0, '', @desid, @dmid, (select top 1 desu.Id from [dbo].[DatasetElementSub] desu inner join [dbo].[DatasetElement] de on desu.DatasetElement_Id = de.Id where de.ElementName = 'Product Information' and desu.ElementName = 'Product batch Number'))
		set @dmsid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
			VALUES (0, 0, 35, NULL, 3)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Authorization Number', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('drugauthorizationnumb', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
			VALUES (0, 0, 2, NULL, 3)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Authorization Country', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('drugauthorizationcountry', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
			VALUES (0, 0, 60, NULL, 3)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Authorization Holder', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('drugauthorizationholder', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [Decimals], [MaxSize], [MinSize], [FieldType_Id]) 
			VALUES (0, 0, 0, 99999999, 1, 4)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Structured Dosage', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('drugstructuredosagenumb', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)
		-- submapping
		INSERT [dbo].[DatasetMappingSub] (MappingType, MappingOption, DestinationElement_Id, Mapping_Id, SourceElement_Id)
			VALUES (0, '', @desid, @dmid, (select top 1 desu.Id from [dbo].[DatasetElementSub] desu inner join [dbo].[DatasetElement] de on desu.DatasetElement_Id = de.Id where de.ElementName = 'Product Information' and desu.ElementName = 'Dose Number'))
		set @dmsid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
			VALUES (0, 0, 2)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('001=kg kilogram(s)', 1, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('002=G gram(s)', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('003=Mg milligram(s)', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('004=ug microgram(s)', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('005=ng nanogram(s)', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('006=pg picogram(s)', 0, 1, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('007=mg/kg milligram(s)/kilogram', 0, 1, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('008=ug/kg microgram(s)/kilogram', 0, 1, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('009=mg/m 2 milligram(s)/sq. meter', 0, 1, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('010=ug/ m 2 microgram(s)/ sq. Meter', 0, 1, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('011=l litre(s)', 0, 1, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('012=ml millilitre(s)', 0, 1, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('013=ul microlitre(s)', 0, 1, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('014=Bq becquerel(s)', 0, 1, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('015=GBq gigabecquerel(s)', 0, 1, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('016=MBq megabecquerel(s)', 0, 1, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('017=Kbq kilobecquerel(s)', 0, 1, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('018=Ci curie(s)', 0, 1, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('019=MCi millicurie(s)', 0, 1, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('020=uCi microcurie(s)', 0, 1, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('021=NCi nanocurie(s)', 0, 1, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('022=Mol mole(s)', 0, 1, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('023=Mmol millimole(s)', 0, 1, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('024=umol micromole(s)', 0, 1, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('025=Iu international unit(s)', 0, 1, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('026=Kiu iu(1000s)', 0, 1, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('027=Miu iu(1,000,000s)', 0, 1, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('028=iu/kg iu/kilogram', 0, 1, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('029=Meq milliequivalent(s)', 0, 1, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('030=% percent', 0, 1, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('031=Gtt drop(s)', 0, 1, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('032=DF dosage form', 0, 1, 0, @fid)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Structured Dosage Unit', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('drugstructuredosageunit', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)
		-- submapping
		INSERT [dbo].[DatasetMappingSub] (MappingType, MappingOption, DestinationElement_Id, Mapping_Id, SourceElement_Id)
			VALUES (1, '', @desid, @dmid, (select top 1 desu.Id from [dbo].[DatasetElementSub] desu inner join [dbo].[DatasetElement] de on desu.DatasetElement_Id = de.Id where de.ElementName = 'Product Information' and desu.ElementName = 'Dose Unit'))
		set @dmsid = (SELECT @@IDENTITY)
		-- mapping values
		INSERT [dbo].[DatasetMappingValue] (SourceValue, DestinationValue, Active, Mapping_Id, SubMapping_Id)
			SELECT 'tablet(s)', '032=DF dosage form', 1, @dmid, @dmsid UNION
			SELECT 'capsule(s)', '032=DF dosage form', 1, @dmid, @dmsid UNION
			SELECT 'drop(s)', '031=Gtt drop(s)', 1, @dmid, @dmsid UNION
			SELECT 'teaspoon(s)', '032=DF dosage form', 1, @dmid, @dmsid UNION
			SELECT 'tablespoon(s)', '032=DF dosage form', 1, @dmid, @dmsid UNION
			SELECT 'milliliter(s)', '012=ml millilitre(s)', 1, @dmid, @dmsid UNION
			SELECT 'suppository(ies)', '032=DF dosage form', 1, @dmid, @dmsid UNION
			SELECT 'injection(s)', '032=DF dosage form', 1, @dmid, @dmsid UNION
			SELECT 'puff(s)', '032=DF dosage form', 1, @dmid, @dmsid UNION
			SELECT 'inhalation(s)', '032=DF dosage form', 1, @dmid, @dmsid UNION
			SELECT 'patch(es)', '032=DF dosage form', 1, @dmid, @dmsid UNION
			SELECT 'Other', '032=DF dosage form', 1, @dmid, @dmsid UNION
			SELECT 'gram(s)', '002=G gram(s)', 1, @dmid, @dmsid UNION
			SELECT 'milligram(s)', '003=Mg milligram(s)', 1, @dmid, @dmsid UNION
			SELECT 'Milligram/m(sqr)', '009=mg/m 2 milligram(s)/sq. meter', 1, @dmid, @dmsid

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [Decimals], [MaxSize], [MinSize], [FieldType_Id]) 
			VALUES (0, 0, 0, 999, 1, 4)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Number Seperate Dosages', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('drugseparatedosagenumb', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [Decimals], [MaxSize], [MinSize], [FieldType_Id]) 
			VALUES (0, 0, 0, 999, 1, 4)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Number Units In Interval', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('drugintervaldosageunitnumb', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
			VALUES (0, 0, 2)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('800=Decade', 1, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('801=Year', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('802=Month', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('803=Week', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('804=Day', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('805=Hour', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('806=Minute', 0, 0, 0, @fid)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Interval Definition', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('drugintervaldosagedefinition', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [Decimals], [MaxSize], [MinSize], [FieldType_Id]) 
			VALUES (0, 0, 0, 9999999999, 1, 4)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Cumulative Dose to First Number', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('drugcumulativedosagenumb', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [Decimals], [MaxSize], [MinSize], [FieldType_Id]) 
			VALUES (0, 0, 0, 999, 1, 4)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Cumulative Dose to First Unit', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('drugcumulativedosageunit', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
			VALUES (0, 0, 100, NULL, 3)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Drug Dosage Text', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('drugdosagetext', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
			VALUES (0, 0, 50, NULL, 3)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Drug Dosage Form', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('drugdosageform', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
			VALUES (0, 0, 2)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('001=Auricular (otic)', 1, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('002=Buccal', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('003=Cutaneous', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('004=Dental', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('005=Endocervical', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('006=Endosinusial', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('007=Endotracheal', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('008=Epidural', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('009=Extra-amniotic', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('010=Hemodialysis', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('011=Intra corpus cavernosum', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('012=Intra-amniotic', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('013=Intra-arterial', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('014=Intra-articular', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('015=Intra-uterine', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('016=Intracardiac', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('017=Intracavernous', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('018=Intracerebral', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('019=Intracervical', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('020=Intracisternal', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('021=Intracorneal', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('022=Intracoronary', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('023=Intradermal', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('024=Intradiscal (intraspinal)', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('025=Intrahepatic', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('026=Intralesional', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('027=Intralymphatic', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('028=Intramedullar (bone marrow)', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('029=Intrameningeal', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('030=Intramuscular', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('031=Intraocular', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('032=Intrapericardial', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('033=Intraperitoneal', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('034=Intrapleural', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('035=Intrasynovial', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('036=Intratumor', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('037=Intrathecal', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('038=Intrathoracic', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('039=Intratracheal', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('040=Intravenous bolus', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('041=Intravenous drip', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('042=Intravenous (not otherwise specified)', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('043=Intravesical', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('044=Iontophoresis', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('045=Nasal', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('046=Occlusive dressing technique', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('047=Ophthalmic', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('048=Oral', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('049=Oropharingeal', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('050=Other', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('051=Parenteral', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('052=Periarticular', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('053=Perineural', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('054=Rectal', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('055=Respiratory (inhalation)', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('056=Retrobulbar', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('057=Sunconjunctival', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('058=Subcutaneous', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('059=Subdermal', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('060=Sublingual', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('061=Topical', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('062=Transdermal', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('063=Transmammary', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('064=Transplacental', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('065=Unknown', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('066=Urethral', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('067=Vaginal', 0, 0, 0, @fid)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Drug Administration Route', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('drugadministrationroute', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)
		-- submapping
		INSERT [dbo].[DatasetMappingSub] (MappingType, MappingOption, DestinationElement_Id, Mapping_Id, SourceElement_Id)
			VALUES (1, '', @desid, @dmid, (select top 1 desu.Id from [dbo].[DatasetElementSub] desu inner join [dbo].[DatasetElement] de on desu.DatasetElement_Id = de.Id where de.ElementName = 'Product Information' and desu.ElementName = 'Drug route of administration'))
		set @dmsid = (SELECT @@IDENTITY)
		-- mapping values
		INSERT [dbo].[DatasetMappingValue] (SourceValue, DestinationValue, Active, Mapping_Id, SubMapping_Id)
			SELECT 'By mouth', '048=Oral', 1, @dmid, @dmsid UNION
			SELECT 'Taken under the tongue', '060=Sublingual', 1, @dmid, @dmsid UNION
			SELECT 'Applied to a surface, usually skin', '003=Cutaneous', 1, @dmid, @dmsid UNION
			SELECT 'Inhalation', '065=Unknown', 1, @dmid, @dmsid UNION
			SELECT 'Applied as a medicated patch to skin', '062=Transdermal', 1, @dmid, @dmsid UNION
			SELECT 'Given into/under the skin', '058=Subcutaneous', 1, @dmid, @dmsid UNION
			SELECT 'Into a vein', '042=Intravenous (not otherwise specified)', 1, @dmid, @dmsid UNION
			SELECT 'Into a muscle', '030=Intramuscular', 1, @dmid, @dmsid UNION
			SELECT 'Into the ear', '001=Auricular (otic)', 1, @dmid, @dmsid UNION
			SELECT 'Into the eye', '031=Intraocular', 1, @dmid, @dmsid UNION
			SELECT 'Rectal', '054=Rectal', 1, @dmid, @dmsid UNION
			SELECT 'Vaginal', '067=Vaginal', 1, @dmid, @dmsid UNION
			SELECT 'Other', '050=Other', 1, @dmid, @dmsid		

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
			VALUES (0, 0, 2)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('001=Auricular (otic)', 1, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('002=Buccal', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('003=Cutaneous', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('004=Dental', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('005=Endocervical', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('006=Endosinusial', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('007=Endotracheal', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('008=Epidural', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('009=Extra-amniotic', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('010=Hemodialysis', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('011=Intra corpus cavernosum', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('012=Intra-amniotic', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('013=Intra-arterial', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('014=Intra-articular', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('015=Intra-uterine', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('016=Intracardiac', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('017=Intracavernous', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('018=Intracerebral', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('019=Intracervical', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('020=Intracisternal', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('021=Intracorneal', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('022=Intracoronary', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('023=Intradermal', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('024=Intradiscal (intraspinal)', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('025=Intrahepatic', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('026=Intralesional', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('027=Intralymphatic', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('028=Intramedullar (bone marrow)', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('029=Intrameningeal', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('030=Intramuscular', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('031=Intraocular', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('032=Intrapericardial', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('033=Intraperitoneal', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('034=Intrapleural', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('035=Intrasynovial', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('036=Intratumor', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('037=Intrathecal', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('038=Intrathoracic', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('039=Intratracheal', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('040=Intravenous bolus', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('041=Intravenous drip', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('042=Intravenous (not otherwise specified)', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('043=Intravesical', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('044=Iontophoresis', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('045=Nasal', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('046=Occlusive dressing technique', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('047=Ophthalmic', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('048=Oral', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('049=Oropharingeal', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('050=Other', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('051=Parenteral', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('052=Periarticular', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('053=Perineural', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('054=Rectal', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('055=Respiratory (inhalation)', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('056=Retrobulbar', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('057=Sunconjunctival', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('058=Subcutaneous', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('059=Subdermal', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('060=Sublingual', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('061=Topical', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('062=Transdermal', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('063=Transmammary', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('064=Transplacental', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('065=Unknown', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('066=Urethral', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('067=Vaginal', 0, 0, 0, @fid)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Drug Paradministration', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('drugparadministration', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [Decimals], [MaxSize], [MinSize], [FieldType_Id]) 
			VALUES (0, 0, 0, 999, 1, 4)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Reaction Gestation Period', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('reactiongestationperiod', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
			VALUES (0, 0, 2)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('802=Month', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('803=Week', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('804=Day', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('810=Trimester', 0, 1, 0, @fid)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Reaction Gestation Period Unit', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('reactiongestationperiodunit', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)
			
		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
			VALUES (0, 0, 10, NULL, 8)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Drug Indication MedDRA Version', @fid, @deid, '', '23.0', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('drugindicationmeddraversion', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
			VALUES (0, 0, 250, NULL, 3)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Drug Indication', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('drugindication', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)
		-- submapping
		INSERT [dbo].[DatasetMappingSub] (MappingType, MappingOption, DestinationElement_Id, Mapping_Id, SourceElement_Id)
			VALUES (0, '', @desid, @dmid, (select top 1 desu.Id from [dbo].[DatasetElementSub] desu inner join [dbo].[DatasetElement] de on desu.DatasetElement_Id = de.Id where de.ElementName = 'Product Information' and desu.ElementName = 'Drug Indication'))
		set @dmsid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
			VALUES (0, 0, 10, NULL, 8)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Drug Start Date Format', @fid, @deid, '', '102', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('drugstartdateformat', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
			VALUES (0, 0, 6)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Drug Start Date', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('drugstartdate', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)
		-- submapping
		INSERT [dbo].[DatasetMappingSub] (MappingType, MappingOption, DestinationElement_Id, Mapping_Id, SourceElement_Id)
			VALUES (0, 'yyyyMMdd', @desid, @dmid, (select top 1 desu.Id from [dbo].[DatasetElementSub] desu inner join [dbo].[DatasetElement] de on desu.DatasetElement_Id = de.Id where de.ElementName = 'Product Information' and desu.ElementName = 'Drug Start Date'))
		set @dmsid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [Decimals], [MaxSize], [MinSize], [FieldType_Id]) 
			VALUES (0, 0, 0, 99999, 1, 4)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Drug Start Period', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('drugstartperiod', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
			VALUES (0, 0, 2)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('801=Year', 1, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('802=Month', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('803=Week', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('804=Day', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('805=Hour', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('806=Minute', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('807=Second', 0, 0, 0, @fid)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Drug Start Period Unit', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('drugstartperiodunit', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [Decimals], [MaxSize], [MinSize], [FieldType_Id]) 
			VALUES (0, 0, 0, 99999, 1, 4)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Drug Last Period', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('druglastperiod', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
			VALUES (0, 0, 2)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('801=Year', 1, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('802=Month', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('803=Week', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('804=Day', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('805=Hour', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('806=Minute', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('807=Second', 0, 0, 0, @fid)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Drug Last Period Unit', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('druglastperiodunit', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
			VALUES (0, 0, 10, NULL, 8)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Drug End Date Format', @fid, @deid, '', '102', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('drugenddateformat', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
			VALUES (0, 0, 6)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Drug End Date', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('drugenddate', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)
		-- submapping
		INSERT [dbo].[DatasetMappingSub] (MappingType, MappingOption, DestinationElement_Id, Mapping_Id, SourceElement_Id)
			VALUES (0, 'yyyyMMdd', @desid, @dmid, (select top 1 desu.Id from [dbo].[DatasetElementSub] desu inner join [dbo].[DatasetElement] de on desu.DatasetElement_Id = de.Id where de.ElementName = 'Product Information' and desu.ElementName = 'Drug End Date'))
		set @dmsid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [Decimals], [MaxSize], [MinSize], [FieldType_Id]) 
			VALUES (0, 0, 0, 99999, 0, 4)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Drug Treatment Duration', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('drugtreatmentduration', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
			VALUES (0, 0, 2)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('801=Year', 1, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('802=Month', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('803=Week', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('804=Day', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('805=Hour', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('806=Minute', 0, 0, 0, @fid)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Drug Treatment Duration Unit', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('drugtreatmentdurationunit', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
			VALUES (0, 0, 2)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('1=Drug withdrawn', 1, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('2=Dose reduced', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('3=Dose increased', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('4=Dose not changed', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('5=Unknown', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('6=Not applicable', 0, 0, 0, @fid)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Drug Action', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('actiondrug', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)
		-- submapping
		INSERT [dbo].[DatasetMappingSub] (MappingType, MappingOption, DestinationElement_Id, Mapping_Id, SourceElement_Id)
			VALUES (1, '', @desid, @dmid, (select top 1 desu.Id from [dbo].[DatasetElementSub] desu inner join [dbo].[DatasetElement] de on desu.DatasetElement_Id = de.Id where de.ElementName = 'Product Information' and desu.ElementName = 'Actions taken with product'))
		set @dmsid = (SELECT @@IDENTITY)
		-- mapping values
		INSERT [dbo].[DatasetMappingValue] (SourceValue, DestinationValue, Active, Mapping_Id, SubMapping_Id)
			SELECT 'Product withdrawn', '1=Drug withdrawn', 1, @dmid, @dmsid UNION
			SELECT 'Dose reduced', '2=Dose reduced', 1, @dmid, @dmsid UNION
			SELECT 'Dose increased', '3=Dose increased', 1, @dmid, @dmsid UNION
			SELECT 'Dose not changed', '4=Dose not changed', 1, @dmid, @dmsid UNION
			SELECT 'Unknown', '5=Unknown', 1, @dmid, @dmsid UNION
			SELECT 'Not applicable', '6=Not applicable', 1, @dmid, @dmsid

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
			VALUES (0, 0, 2)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('1=Yes', 1, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('2=No', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('3=Unknown', 0, 0, 0, @fid)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Recurrence Administration', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('drugrecurreadministration', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
			VALUES (0, 0, 250, NULL, 3)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Additional Information', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('drugadditional', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		-- category node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('activesubstance', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, NULL, 1, @dsxid)
		set @dsxcnid4 = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
			VALUES (0, 0, 250, NULL, 3)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Active Substance Name', @fid, @deid, '', '', 0, 2)
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('activesubstancename', 2, NULL, GETDATE(), GETDATE(), @dsxcnid4, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		-- category node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('drugreactionrelatedness', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, NULL, 1, @dsxid)
		set @dsxcnid4 = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
			VALUES (0, 0, 10, NULL, 8)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Drug Reaction Assessment MedDRA Version', @fid, @deid, '', '23.0', 1, 1)
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('drugreactionassesmeddraversion', 2, NULL, GETDATE(), GETDATE(), @dsxcnid4, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
			VALUES (0, 0, 250, NULL, 3)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Drug Reaction Assessment', @fid, @deid, '', '', 0, 2)
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('drugreactionasses', 2, NULL, GETDATE(), GETDATE(), @dsxcnid4, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
			VALUES (0, 0, 250, NULL, 3)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Source of Assessment', @fid, @deid, '', '', 0, 2)
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('drugassessmentsource', 2, NULL, GETDATE(), GETDATE(), @dsxcnid4, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
			VALUES (0, 0, 2)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('WHO Causality Scale', 1, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('Naranjo Causality Scale', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('Unknown', 0, 0, 0, @fid)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Method of Assessment', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('drugassessmentmethod', 2, NULL, GETDATE(), GETDATE(), @dsxcnid4, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
			VALUES (0, 0, 35, NULL, 3)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Assessment Result', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('drugresult', 2, NULL, GETDATE(), GETDATE(), @dsxcnid4, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

/**************************************************
CATEGORY summary
**************************************************/
INSERT [dbo].[DatasetCategory] ([DatasetCategoryName], [CategoryOrder], [Dataset_Id]) 
	VALUES (N'Summary', 1, @dsid)
set @dscid = (SELECT @@IDENTITY)

-- category node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('summary', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, NULL, 1, @dsxid)
set @dsxcnid3 = (SELECT @@IDENTITY)

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 1000, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Narrative Include Clinical', @fid, 1, NULL, NULL, 0, '8FCD116E-FDB6-4DD2-BB3C-2309FFF18259')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
set @dceid = (SELECT @@IDENTITY)
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('narrativeincludeclinical', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)
-- mapping
INSERT [dbo].[DatasetMapping] (Tag, MappingType, MappingOption, DestinationElement_Id, PropertyPath, Property)
	VALUES ('Active', 2, '', @dceid, 'PatientClinicalEvent', 'Comments')
INSERT [dbo].[DatasetMapping] (Tag, MappingType, MappingOption, DestinationElement_Id, PropertyPath, Property)
	VALUES ('Active', 4, '', @dceid, '', 'SourceDescription')

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 500, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Reporter Comment', @fid, 1, NULL, NULL, 0, '9A22BCE4-CD6F-4582-919B-D8B37A2953A8')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
set @dceid = (SELECT @@IDENTITY)
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('reportercomment', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 10, NULL, 8)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Sender Diagnosis MedDRA Version', @fid, 1, NULL, '', 1, '95708474-7779-4006-8C40-E391256F0A5F')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('senderdiagnosismeddraversion', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 250, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Sender Diagnosis', @fid, 1, NULL, NULL, 0, 'FE91B984-30F0-4CE8-A15E-49056C761CB6')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('senderdiagnosis', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 1000, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Sender Comment', @fid, 1, NULL, NULL, 0, '866A2E7B-7D5D-4BFD-8BBA-0A9470107548')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('sendercomment', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

UPDATE dc set [Public] = 0
	FROM Dataset ds
	INNER JOIN DatasetCategory dc ON ds.Id = dc.Dataset_Id 
	INNER JOIN DatasetCategoryElement dce ON dc.Id = dce.DatasetCategory_Id 
	INNER JOIN DatasetElement de ON dce.DatasetElement_Id = de.Id 
	INNER JOIN Field f ON de.Field_Id = f.Id
	INNER JOIN FieldType ft ON f.FieldType_Id = ft.Id
	LEFT JOIN FieldValue fv ON f.Id = fv.Field_Id 
where ds.DatasetName = 'E2B(R2) ICH Report'

UPDATE dce set [Public] = 0
	FROM Dataset ds
	INNER JOIN DatasetCategory dc ON ds.Id = dc.Dataset_Id 
	INNER JOIN DatasetCategoryElement dce ON dc.Id = dce.DatasetCategory_Id 
	INNER JOIN DatasetElement de ON dce.DatasetElement_Id = de.Id 
	INNER JOIN Field f ON de.Field_Id = f.Id
	INNER JOIN FieldType ft ON f.FieldType_Id = ft.Id
	LEFT JOIN FieldValue fv ON f.Id = fv.Field_Id 
where ds.DatasetName = 'E2B(R2) ICH Report'

UPDATE dc set [Public] = 1
	FROM Dataset ds
	INNER JOIN DatasetCategory dc ON ds.Id = dc.Dataset_Id 
	INNER JOIN DatasetCategoryElement dce ON dc.Id = dce.DatasetCategory_Id 
	INNER JOIN DatasetElement de ON dce.DatasetElement_Id = de.Id 
	INNER JOIN Field f ON de.Field_Id = f.Id
	INNER JOIN FieldType ft ON f.FieldType_Id = ft.Id
	LEFT JOIN FieldValue fv ON f.Id = fv.Field_Id 
where ds.DatasetName = 'E2B(R2) ICH Report'
	and dc.DatasetCategoryName in ('Safety Report', 'Primary Source', 'Patient', 'Patient Death', 'Reaction', 'Test', 'Test (2)', 'Test (3)', 'Drug (1)', 'Drug (2)', 'Drug (3)', 'Drug (4)', 'Drug (5)', 'Drug (6)', 'Summary')

UPDATE dce set [Public] = 1
	FROM Dataset ds
	INNER JOIN DatasetCategory dc ON ds.Id = dc.Dataset_Id 
	INNER JOIN DatasetCategoryElement dce ON dc.Id = dce.DatasetCategory_Id 
	INNER JOIN DatasetElement de ON dce.DatasetElement_Id = de.Id 
	INNER JOIN Field f ON de.Field_Id = f.Id
	INNER JOIN FieldType ft ON f.FieldType_Id = ft.Id
	LEFT JOIN FieldValue fv ON f.Id = fv.Field_Id 
where ds.DatasetName = 'E2B(R2) ICH Report' and dc.DatasetCategoryName = 'Safety Report'
	and de.ElementName in ('Serious', 'Seriousness Death', 'Seriousness Life Threatening', 'Seriousness Hospitalization', 'Seriousness Disabling', 'Seriousness Congenital Anomaly', 'Seriousness Other')
	
UPDATE dce set [Public] = 1
	FROM Dataset ds
	INNER JOIN DatasetCategory dc ON ds.Id = dc.Dataset_Id 
	INNER JOIN DatasetCategoryElement dce ON dc.Id = dce.DatasetCategory_Id 
	INNER JOIN DatasetElement de ON dce.DatasetElement_Id = de.Id 
	INNER JOIN Field f ON de.Field_Id = f.Id
	INNER JOIN FieldType ft ON f.FieldType_Id = ft.Id
	LEFT JOIN FieldValue fv ON f.Id = fv.Field_Id 
where ds.DatasetName = 'E2B(R2) ICH Report' and dc.DatasetCategoryName = 'Primary Source'
	and de.ElementName in ('Reporter Title', 'Reporter Given Name', 'Reporter Middle Name', 'Reporter Family Name', 'Reporter Street', 'Reporter City', 'Reporter State', 'Reporter Postcode', 'Reporter Country', 'Qualification')

UPDATE dce set [Public] = 1
	FROM Dataset ds
	INNER JOIN DatasetCategory dc ON ds.Id = dc.Dataset_Id 
	INNER JOIN DatasetCategoryElement dce ON dc.Id = dce.DatasetCategory_Id 
	INNER JOIN DatasetElement de ON dce.DatasetElement_Id = de.Id 
	INNER JOIN Field f ON de.Field_Id = f.Id
	INNER JOIN FieldType ft ON f.FieldType_Id = ft.Id
	LEFT JOIN FieldValue fv ON f.Id = fv.Field_Id 
where ds.DatasetName = 'E2B(R2) ICH Report' and dc.DatasetCategoryName = 'Patient'
	and de.ElementName in ('Patient Initial', 'Patient Hospital Record Number', 'Patient Birthdate', 'Patient Onset Age', 'Patient Onset Age Unit', 'Gestation Period', 'Gestation Period Unit', 'Patient Age Group', 'Patient Weight', 'Patient Height', 'Patient Sex', 'Patient Last Menstrual Date')

UPDATE dce set [Public] = 1
	FROM Dataset ds
	INNER JOIN DatasetCategory dc ON ds.Id = dc.Dataset_Id 
	INNER JOIN DatasetCategoryElement dce ON dc.Id = dce.DatasetCategory_Id 
	INNER JOIN DatasetElement de ON dce.DatasetElement_Id = de.Id 
	INNER JOIN Field f ON de.Field_Id = f.Id
	INNER JOIN FieldType ft ON f.FieldType_Id = ft.Id
	LEFT JOIN FieldValue fv ON f.Id = fv.Field_Id 
where ds.DatasetName = 'E2B(R2) ICH Report' and dc.DatasetCategoryName = 'Patient Death'
	and de.ElementName in ('Patient Deathdate', 'Patient Autopsy')

UPDATE dce set [Public] = 1
	FROM Dataset ds
	INNER JOIN DatasetCategory dc ON ds.Id = dc.Dataset_Id 
	INNER JOIN DatasetCategoryElement dce ON dc.Id = dce.DatasetCategory_Id 
	INNER JOIN DatasetElement de ON dce.DatasetElement_Id = de.Id 
	INNER JOIN Field f ON de.Field_Id = f.Id
	INNER JOIN FieldType ft ON f.FieldType_Id = ft.Id
	LEFT JOIN FieldValue fv ON f.Id = fv.Field_Id 
where ds.DatasetName = 'E2B(R2) ICH Report' and dc.DatasetCategoryName = 'Reaction'
	and de.ElementName in ('Primary Source Reaction', 'Reaction Start Date', 'Reaction End Date', 'Reaction Outcome')

UPDATE dce set [Public] = 1
	FROM Dataset ds
	INNER JOIN DatasetCategory dc ON ds.Id = dc.Dataset_Id 
	INNER JOIN DatasetCategoryElement dce ON dc.Id = dce.DatasetCategory_Id 
	INNER JOIN DatasetElement de ON dce.DatasetElement_Id = de.Id 
	INNER JOIN Field f ON de.Field_Id = f.Id
	INNER JOIN FieldType ft ON f.FieldType_Id = ft.Id
	LEFT JOIN FieldValue fv ON f.Id = fv.Field_Id 
where ds.DatasetName = 'E2B(R2) ICH Report' and dc.DatasetCategoryName in ('Test', 'Test (2)', 'Test (3)')
	and de.ElementName in ('Test Date', 'Test Name', 'Test Result', 'Test Unit', 'Low Test Range', 'High Test Range', 'More Information', 'Test Date (2)', 'Test Name (2)', 'Test Result (2)', 'Test Unit (2)', 'Low Test Range (2)', 'High Test Range (2)', 'More Information (2)', 'Test Date (3)', 'Test Name (3)', 'Test Result (3)', 'Test Unit (3)', 'Low Test Range (3)', 'High Test Range (3)', 'More Information (3)')

UPDATE dce set [Public] = 1
	FROM Dataset ds
	INNER JOIN DatasetCategory dc ON ds.Id = dc.Dataset_Id 
	INNER JOIN DatasetCategoryElement dce ON dc.Id = dce.DatasetCategory_Id 
	INNER JOIN DatasetElement de ON dce.DatasetElement_Id = de.Id 
	INNER JOIN Field f ON de.Field_Id = f.Id
	INNER JOIN FieldType ft ON f.FieldType_Id = ft.Id
	LEFT JOIN FieldValue fv ON f.Id = fv.Field_Id 
where ds.DatasetName = 'E2B(R2) ICH Report' and dc.DatasetCategoryName in ('Drug (1)', 'Drug (2)', 'Drug (3)', 'Drug (4)', 'Drug (5)', 'Drug (6)')
	and de.ElementName in ('Drug Characterization (1)', 'Medicinal Product (1)', 'Dose Number (1)', 'Dose Unit (1)', 'Drug Dosage Form (1)', 'Drug Administration Route (1)', 'Drug Indication (1)', 'Drug Start Date (1)', 'Drug End Date (1)',
	'Drug Characterization (2)', 'Medicinal Product (2)', 'Dose Number (2)', 'Dose Unit (2)', 'Drug Dosage Form (2)', 'Drug Administration Route (2)', 'Drug Indication (2)', 'Drug Start Date (2)', 'Drug End Date (2)',
	'Drug Characterization (3)', 'Medicinal Product (3)', 'Dose Number (3)', 'Dose Unit (3)', 'Drug Dosage Form (3)', 'Drug Administration Route (3)', 'Drug Indication (3)', 'Drug Start Date (3)', 'Drug End Date (3)',
	'Drug Characterization (4)', 'Medicinal Product (4)', 'Dose Number (4)', 'Dose Unit (4)', 'Drug Dosage Form (4)', 'Drug Administration Route (4)', 'Drug Indication (4)', 'Drug Start Date (4)', 'Drug End Date (4)',
	'Drug Characterization (5)', 'Medicinal Product (5)', 'Dose Number (5)', 'Dose Unit (5)', 'Drug Dosage Form (5)', 'Drug Administration Route (5)', 'Drug Indication (5)', 'Drug Start Date (5)', 'Drug End Date (5)',
	'Drug Characterization (6)', 'Medicinal Product (6)', 'Dose Number (6)', 'Dose Unit (6)', 'Drug Dosage Form (6)', 'Drug Administration Route (6)', 'Drug Indication (6)', 'Drug Start Date (6)', 'Drug End Date (6)')

UPDATE dce set [Public] = 1
	FROM Dataset ds
	INNER JOIN DatasetCategory dc ON ds.Id = dc.Dataset_Id 
	INNER JOIN DatasetCategoryElement dce ON dc.Id = dce.DatasetCategory_Id 
	INNER JOIN DatasetElement de ON dce.DatasetElement_Id = de.Id 
	INNER JOIN Field f ON de.Field_Id = f.Id
	INNER JOIN FieldType ft ON f.FieldType_Id = ft.Id
	LEFT JOIN FieldValue fv ON f.Id = fv.Field_Id 
where ds.DatasetName = 'E2B(R2) ICH Report' and dc.DatasetCategoryName = 'Summary'
	and de.ElementName in ('Reporter Comment')

--SELECT ds.DatasetName, dc.DatasetCategoryName, de.ElementName FROM Dataset ds
--	INNER JOIN DatasetCategory dc ON ds.Id = dc.Dataset_Id 
--	INNER JOIN DatasetCategoryElement dce ON dc.Id = dce.DatasetCategory_Id 
--	INNER JOIN DatasetElement de ON dce.DatasetElement_Id = de.Id 
--where ds.Id = @dsid
	
--ROLLBACK TRAN A1
COMMIT TRAN A1


