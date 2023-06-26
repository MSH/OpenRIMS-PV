DECLARE @Id int
SELECT @Id = Id   FROM Dataset  WHERE DatasetName = 'Spontaneous Report'

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
DECLARE @dsid int
DECLARE @dscid int
DECLARE @fid int
DECLARE @deid int
DECLARE @dceid int
INSERT [dbo].[Dataset] ([DatasetName], [Active], [InitialiseProcess], [RulesProcess], [Help], [Created], [LastUpdated], [ContextType_Id], [CreatedBy_Id], [UpdatedBy_Id], [IsSystem])
	VALUES ('Spontaneous Report', 1, '', '', 'Suspected adverse drug reaction (ADR) online reporting form', GETDATE(), GETDATE(), 4, 1, 1, 1)
set @dsid = (SELECT @@IDENTITY)

/**************************************************
CATEGORY Patient Information
**************************************************/
INSERT [dbo].[DatasetCategory] ([DatasetCategoryName], [CategoryOrder], [Dataset_Id], [FriendlyName], [Help])
	VALUES ('Patient Information', 1, @dsid, 'Patient Information', 'Please enter some information about the person who had the adverse reaction.') 
set @dscid = (SELECT @@IDENTITY)

-- Initials
INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (1, 5, NULL, NULL, NULL, '', NULL, '', 0, 3) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid])
	VALUES ('Initials', @fid, 1, '', '', 0, '29CD2157-8FB6-4883-A4E6-A4B9EDE6B36B') 
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (1, @dscid, @deid, 0, 0, 'Initials of Patient', 'Enter patient''s initials here OR their ID number and type below.') 
set @dceid = (SELECT @@IDENTITY)

-- Identification Number
INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (0, 30, NULL, NULL, NULL, '', NULL, '', 0, 3) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid])
	VALUES ('Identification Number', @fid, 1, '', '', 0, '5A2E89A9-8240-4665-967D-0C655CF281B7') 
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (2, @dscid, @deid, 1, 0, 'Identification Number', 'Enter patient''s ID number OR enter their initials above..') 
set @dceid = (SELECT @@IDENTITY)

-- Identification Type
INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (0, NULL, NULL, NULL, NULL, '', NULL, '', 0, 2) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('National Identity', 1, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Other', 0, 1, 0, @fid) 
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid])
	VALUES ('Identification Type', @fid, 1, '', '', 0, 'E3CDDEB3-E129-4161-AC91-4DD91CC8AD4B') 
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (3, @dscid, @deid, 1, 0, 'Identification Type', 'If you entered a patient ID number, specify the ID type here.') 
set @dceid = (SELECT @@IDENTITY)

-- Date of Birth
INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (0, NULL, NULL, NULL, NULL, '', NULL, '', 0, 6) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid])
	VALUES ('Date of Birth', @fid, 1, '', '', 0, '0D704069-5C50-4085-8FE1-355BB64EF196') 
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (4, @dscid, @deid, 1, 0, 'Patient Date of Birth', 'Enter the patient''s date of birth here OR enter their age below.') 
set @dceid = (SELECT @@IDENTITY)

-- Age
INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (0, NULL, 0, 140.00, 0.00, '', NULL, '', 0, 4) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid])
	VALUES ('Age', @fid, 1, '', '', 0, 'D314C438-5ABA-4ED2-855D-1A5B22B5A301') 
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (5, @dscid, @deid, 1, 0, 'Age', 'Enter the patient''s age here OR enter their date of birth above.') 
set @dceid = (SELECT @@IDENTITY)

-- Age Unit
INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (0, NULL, NULL, NULL, NULL, '', NULL, '', 0, 2) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Years', 1, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Months', 0, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Weeks', 0, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Days', 0, 0, 0, @fid) 
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid])
	VALUES ('Age Unit', @fid, 1, '', '', 0, '80C219DC-238C-487E-A3D5-8919ABA674B1') 
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (6, @dscid, @deid, 1, 0, 'Age Unit of Measure', 'Enter weeks, months, or years for the patient''s age here.') 
set @dceid = (SELECT @@IDENTITY)

-- Weight (kg)
INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (0, NULL, 1, 159.90, 1.10, '', NULL, '', 0, 4) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid])
	VALUES ('Weight  (kg)', @fid, 1, '', '', 0, '985BD25D-54E7-4A24-8636-6DBC0F9C7B96') 
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (7, @dscid, @deid, 1, 0, 'Patient''s weight (kg)', '') 
set @dceid = (SELECT @@IDENTITY)

-- Sex
INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (0, NULL, NULL, NULL, NULL, '', NULL, '', 0, 2) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Male', 1, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Female', 0, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Unknown', 0, 0, 0, @fid) 
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid])
	VALUES ('Sex', @fid, 1, '', '', 0, 'E061D363-534E-4EA4-B6E5-F1C531931B12') 
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (8, @dscid, @deid, 0, 0, 'Sex', '') 
set @dceid = (SELECT @@IDENTITY)

-- Ethnic Group
INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (0, NULL, NULL, NULL, NULL, '', NULL, '', 0, 2) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Asian', 1, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('East Asian', 0, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('South Asian', 0, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Southeast Asian', 0, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Black', 0, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('White', 0, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Middle Eastern', 0, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Other', 0, 1, 0, @fid) 
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid])
	VALUES ('Ethnic Group', @fid, 1, '', '', 0, 'DF6717EF-A674-46DD-B738-859355ECA9A1') 
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (9, @dscid, @deid, 1, 0, 'Ethnic Group of Patient', '') 
set @dceid = (SELECT @@IDENTITY)

/**************************************************
CATEGORY Product Information
**************************************************/
INSERT [dbo].[DatasetCategory] ([DatasetCategoryName], [CategoryOrder], [Dataset_Id], [FriendlyName], [Help])
	VALUES ('Product Information', 2, @dsid, 'Product Information', 'Please enter information about the product you suspect caused the reaction and about other products taken.') 
set @dscid = (SELECT @@IDENTITY)

-- Product Information Table
INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (0, NULL, NULL, NULL, NULL, '', NULL, '', 0, 7) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid])
	VALUES ('Product Information', @fid, 1, '', '', 0, '712CA632-0CD0-4418-9176-FB0B95AEE8A1') 
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (10, @dscid, @deid, 0, 0, 'Name of product', 'Name of product.') 
set @dceid = (SELECT @@IDENTITY)

		-- Product
		INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
			VALUES (1, 100, NULL, NULL, NULL, '', NULL, '', 0, 3) 
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder, FriendlyName, Help)
			VALUES ('Product', @fid, @deid, '', '', 0, 1, 'Product Name', 'Enter the brand or generic name') 

		-- Product Suspected
		INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
			VALUES (1, NULL, NULL, NULL, NULL, '', NULL, '', 0, 5) 
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder, FriendlyName, Help)
			VALUES ('Product Suspected', @fid, @deid, '', '', 0, 2, 'Is the product suspected?', '') 

		-- Drug strength
		INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
			VALUES (0, NULL, 0, 99999999.00, 1.00, '', NULL, '', 0, 4) 
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder, FriendlyName, Help)
			VALUES ('Drug Strength', @fid, @deid, '', '', 0, 3, 'Drug Strength', '') 

		-- Drug strength unit
		INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
			VALUES (0, NULL, NULL, NULL, NULL, '', NULL, '', 0, 2) 
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('milligrams (mg)', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('milligrams/milliliters (mg/ml)', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('grams (gm)', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('kilograms (kg)', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('micrograms (mcg)', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('milliliters (ml)', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('liters (l)', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('milliequivalents (meq)', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('percent (%)', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('drops (gtt)', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('Other', 0, 1, 0, @fid) 
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder, FriendlyName, Help)
			VALUES ('Drug strength unit', @fid, @deid, '', '', 0, 4, 'Unit of strength', '') 

		-- Dose Number
		INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
			VALUES (0, NULL, 0, 99999999.00, 1.00, '', NULL, '', 0, 4) 
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder, FriendlyName, Help)
			VALUES ('Dose Number', @fid, @deid, '', '', 0, 5, 'Dose number', '') 

		-- Dose Unit
		INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
			VALUES (0, NULL, NULL, NULL, NULL, '', NULL, '', 0, 2) 
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('tablet(s)', 1, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('capsule(s)', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('drop(s)', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('teaspoon(s)', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('tablespoon(s)', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('milliliter(s)', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('suppository(ies)', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('injection(s)', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('puff(s)', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('inhalation(s)', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('patch(es)', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('Other', 0, 1, 0, @fid) 
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder, FriendlyName, Help)
			VALUES ('Dose Unit', @fid, @deid, '', '', 0, 6, 'Unit of dosage', '') 

		-- Drug route of administration
		INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
			VALUES (0, NULL, NULL, NULL, NULL, '', NULL, '', 0, 2) 
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('By mouth', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('Taken under the tongue', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('Applied to a surface, usually skin', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('Inhalation', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('Applied as a medicated patch to skin', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('Given into/under the skin', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('Into a vein', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('Into a muscle', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('Into the ear', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('Into the eye', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('Rectal', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('Vaginal', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('Other', 0, 1, 0, @fid) 
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder, FriendlyName, Help)
			VALUES ('Drug route of administration', @fid, @deid, '', '', 0, 7, 'Drug route of administration', '') 

		-- Drug Start Date
		INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
			VALUES (0, NULL, NULL, NULL, NULL, '', NULL, '', 0, 6) 
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder, FriendlyName, Help)
			VALUES ('Drug Start Date', @fid, @deid, '', '', 0, 8, 'Date drug usage started', '') 

		-- Drug End Date
		INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
			VALUES (0, NULL, NULL, NULL, NULL, '', NULL, '', 0, 6) 
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder, FriendlyName, Help)
			VALUES ('Drug End Date', @fid, @deid, '', '', 0, 9, 'Date drug usage ended', '') 

		-- Drug Treatment Duration
		INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
			VALUES (0, NULL, 0, 99999.00, 1.00, '', NULL, '', 0, 4) 
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder, FriendlyName, Help)
			VALUES ('Drug Treatment Duration', @fid, @deid, '', '', 0, 10, 'Duration of usage', '') 

		-- Drug Treatment Duration Unit
		INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
			VALUES (0, NULL, NULL, NULL, NULL, '', NULL, '', 0, 2) 
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
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder, FriendlyName, Help)
			VALUES ('Drug Treatment Duration Unit', @fid, @deid, '', '', 0, 11, 'Unit of duration', '') 

		-- Drug Indication
		INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
			VALUES (0, 250, NULL, NULL, NULL, '', NULL, '', 0, 3) 
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder, FriendlyName, Help)
			VALUES ('Drug Indication', @fid, @deid, '', '', 0, 12, 'Indication for drug usage', '') 

		-- Product Frequency
		INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
			VALUES (0, 50, NULL, NULL, NULL, '', NULL, '', 0, 3) 
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder, FriendlyName, Help)
			VALUES ('Product Frequency', @fid, @deid, '', '', 0, 12, 'Frequency of product usage', '') 
			
		-- Product Batch Number
		INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
			VALUES (0, 25, NULL, NULL, NULL, '', NULL, '', 0, 3) 
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder, FriendlyName, Help)
			VALUES ('Product Batch Number', @fid, @deid, '', '', 0, 12, 'Product batch number', '') 
						
		-- Actions taken with product
		INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
			VALUES (0, NULL, NULL, NULL, NULL, '', NULL, '', 0, 2) 
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('Product withdrawn', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('Dose reduced', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('Dose increased', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('Dose not changed', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('Unknown', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('Not applicable', 0, 0, 0, @fid) 
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder, FriendlyName, Help)
			VALUES ('Actions taken with product', @fid, @deid, '', '', 0, 13, 'Actions taken with product', '') 

		-- Product challenge
		INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
			VALUES (0, NULL, NULL, NULL, NULL, '', NULL, '', 0, 2) 
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('Yes', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('No', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('Unknown', 0, 0, 0, @fid) 
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder, FriendlyName, Help)
			VALUES ('Product challenge', @fid, @deid, '', '', 0, 14, 'Was there a challenge to the usage of the product?', '') 

		-- Product rechallenge
		INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
			VALUES (0, NULL, NULL, NULL, NULL, '', NULL, '', 0, 2) 
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('Yes', 1, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('No', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('Didn''t restart', 0, 0, 0, @fid) 
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder, FriendlyName, Help)
			VALUES ('Product rechallenge', @fid, @deid, '', '', 0, 15, 'Was there a rechallenge to the usage of the product?', '') 

/**************************************************
CATEGORY Reaction and Treatment
**************************************************/
INSERT [dbo].[DatasetCategory] ([DatasetCategoryName], [CategoryOrder], [Dataset_Id], [FriendlyName], [Help])
	VALUES ('Reaction and Treatment', 3, @dsid, 'Reaction and Treatment', 'Enter information about what happened and how it was treated.') 
set @dscid = (SELECT @@IDENTITY)

-- Description of reaction
INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (1, 500, NULL, NULL, NULL, '', NULL, '', 0, 3) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid])
	VALUES ('Description of reaction', @fid, 1, '', '', 0, 'ACD938A4-76D1-44CE-A070-2B8DF0FE9E0F') 
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (84, @dscid, @deid, 0, 0, 'Description of reaction', '') 
set @dceid = (SELECT @@IDENTITY)

-- Reaction start date
INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (0, NULL, NULL, NULL, NULL, '', NULL, '', 0, 6) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid])
	VALUES ('Reaction known start date', @fid, 1, '', '', 0, 'F5EEB382-D4A5-41A1-A447-37D5ECA50B99') 
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (85, @dscid, @deid, 0, 0, 'Start date of reaction', 'Enter the start date of the reaction OR enter the estimated start date in the next field.') 
set @dceid = (SELECT @@IDENTITY)

-- Reaction estimated start date
INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (0, NULL, NULL, NULL, NULL, '', NULL, '', 0, 6) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid])
	VALUES ('Reaction estimated start date', @fid, 1, '', '', 0, '67F2C8CA-503B-498C-9B7E-561BAE4DFA52') 
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (90, @dscid, @deid, 0, 0, 'Estimated start date of reaction', 'If you don''t know the exact start date of the reaction, enter the estimated start date here.') 
set @dceid = (SELECT @@IDENTITY)

-- Reaction serious details
INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (0, NULL, NULL, NULL, NULL, '', NULL, '', 0, 3) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Resulted in death', 0, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Is life-threatening', 0, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Requires inpatient hospitalization or prolongation of existing hospitalization', 0, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Results in persistent or significant disability/incapacity (as per reporter''s opinion)', 0, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Is a congenital anomaly/birth defect', 0, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Other medically important condition', 0, 1, 0, @fid) 
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid])
	VALUES ('Reaction serious details', @fid, 1, '', '', 0, '302C07C9-B0E0-46AB-9EF8-5D5C2F756BF1') 
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (91, @dscid, @deid, 0, 0, 'Did any of these reactions happen?', '') 
set @dceid = (SELECT @@IDENTITY)

-- Treatment given for reaction
INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (0, NULL, NULL, NULL, NULL, '', NULL, '', 0, 5) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid])
	VALUES ('Treatment given for reaction', @fid, 1, '', '', 0, '3A84016D-9A58-464F-A316-027A095291CE') 
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (93, @dscid, @deid, 0, 0, 'Was treatment given for the reaction?', '') 
set @dceid = (SELECT @@IDENTITY)

-- Treatment given for reaction details
INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (0, 500, NULL, NULL, NULL, '', NULL, '', 0, 3) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid])
	VALUES ('Treatment given for reaction details', @fid, 1, '', '', 0, '24DE20BE-AB13-487B-B679-D62D6FEE8814') 
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (94, @dscid, @deid, 0, 0, 'What treatment was given for the reaction?', '') 
set @dceid = (SELECT @@IDENTITY)

-- Outcome of reaction
INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (0, NULL, NULL, NULL, NULL, '', NULL, '', 0, 2) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Recovered/resolved', 0, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Recovering/resolving', 0, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Not recovered/not resolved', 0, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Recovered/resolved with permanent complications', 0, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Fatal', 0, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Unknown', 0, 0, 0, @fid) 
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid])
	VALUES ('Outcome of reaction', @fid, 1, '', '', 0, '976F6C53-78F2-4007-8F39-54057E554EEB') 
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (95, @dscid, @deid, 0, 0, 'What was the outcome of the reaction?', '') 
set @dceid = (SELECT @@IDENTITY)

-- Reaction date of recovery
INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (0, NULL, NULL, NULL, NULL, '', NULL, '', 0, 6) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid])
	VALUES ('Reaction date of recovery', @fid, 1, '', '', 0, 'F977C2F8-C7DD-4AFE-BCAA-1C06BD54D155') 
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (96, @dscid, @deid, 0, 0, 'What was the date of recovery from the reaction?', '') 
set @dceid = (SELECT @@IDENTITY)

-- Reaction date of death
INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (0, NULL, NULL, NULL, NULL, '', NULL, '', 0, 6) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid])
	VALUES ('Reaction date of death', @fid, 1, '', '', 0, '8B15C037-9C92-4AD4-A8F4-6C4042D40D9D') 
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (97, @dscid, @deid, 0, 0, 'Enter date if patient died from the reaction', '') 
set @dceid = (SELECT @@IDENTITY)

-- Reaction other relevant info
INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (0, 500, NULL, NULL, NULL, '', NULL, '', 0, 3) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid])
	VALUES ('Reaction other relevant info', @fid, 1, '', '', 0, '7BBEC54B-65C3-4BA6-B5A7-83C3C473F803') 
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (98, @dscid, @deid, 0, 0, 'Other relevant information', 'For example, does the patient have other medical problems?') 
set @dceid = (SELECT @@IDENTITY)

/**************************************************
CATEGORY Test Results
**************************************************/
INSERT [dbo].[DatasetCategory] ([DatasetCategoryName], [CategoryOrder], [Dataset_Id], [FriendlyName], [Help])
	VALUES ('Test Results', 4, @dsid, 'Test Results', 'Enter information about any tests done for the reaction, along with the results.') 
set @dscid = (SELECT @@IDENTITY)

-- Test Results Table
INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (0, NULL, NULL, NULL, NULL, '', NULL, '', 0, 7) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid])
	VALUES ('Test Results', @fid, 1, '', '', 0, '12D7089D-1603-4309-99DE-60F20F9A005E') 
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (10, @dscid, @deid, 0, 0, 'Test Results', 'Test Results') 
set @dceid = (SELECT @@IDENTITY)

	-- Test Date
	INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
		VALUES (1, NULL, NULL, NULL, NULL, '', NULL, '', 0, 6) 
	set @fid = (SELECT @@IDENTITY)
	INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder, FriendlyName, Help)
		VALUES ('Test Date', @fid, @deid, '', '', 0, 1, 'Date the test was conducted', '') 

	-- Test Name
	INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
		VALUES (1, 100, NULL, NULL, NULL, '', NULL, '', 0, 3) 
	set @fid = (SELECT @@IDENTITY)
	INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder, FriendlyName, Help)
		VALUES ('Test Name', @fid, @deid, '', '', 0, 2, 'Name of the test', '') 

	-- Test Result
	INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
		VALUES (1, 50, NULL, NULL, NULL, '', NULL, '', 0, 3) 
	set @fid = (SELECT @@IDENTITY)
	INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder, FriendlyName, Help)
		VALUES ('Test Result', @fid, @deid, '', '', 0, 3, 'Result of the test', '') 

	-- Test Unit
	INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
		VALUES (0, 35, NULL, NULL, NULL, '', NULL, '', 0, 3) 
	set @fid = (SELECT @@IDENTITY)
	INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder, FriendlyName, Help)
		VALUES ('Test Unit', @fid, @deid, '', '', 0, 4, 'Unit of the test result', '') 

	-- Low Test Range
	INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
		VALUES (0, 50, NULL, NULL, NULL, '', NULL, '', 0, 3) 
	set @fid = (SELECT @@IDENTITY)
	INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder, FriendlyName, Help)
		VALUES ('Low Test Range', @fid, @deid, '', '', 0, 5, 'Lower limit of the test result', '') 

	-- High Test Range
	INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
		VALUES (0, 50, NULL, NULL, NULL, '', NULL, '', 0, 3) 
	set @fid = (SELECT @@IDENTITY)
	INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder, FriendlyName, Help)
		VALUES ('High Test Range', @fid, @deid, '', '', 0, 6, 'Upper limit of the test result', '') 

	-- More Information
	INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
		VALUES (0, NULL, NULL, NULL, NULL, '', NULL, '', 0, 2) 
	set @fid = (SELECT @@IDENTITY)
	INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
		VALUES ('Yes', 1, 0, 0, @fid) 
	INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
		VALUES ('No', 0, 0, 0, @fid) 
	INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder, FriendlyName, Help)
		VALUES ('More Information', @fid, @deid, '', '', 0, 7, 'Any additional information', '') 

/**************************************************
CATEGORY Reporter Information
**************************************************/
INSERT [dbo].[DatasetCategory] ([DatasetCategoryName], [CategoryOrder], [Dataset_Id], [FriendlyName], [Help])
	VALUES ('Reporter Information', 5, @dsid, 'Reporter Information', 'Enter information about the person reporting the reaction.') 
set @dscid = (SELECT @@IDENTITY)

-- Reporter Name
INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (1, 60, NULL, NULL, NULL, '', NULL, '', 0, 3) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid])
	VALUES ('Reporter Name', @fid, 1, '', '', 0, '926A07E1-8B83-41CA-8949-739717924AD9') 
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (91, @dscid, @deid, 1, 0, 'Name or initials of person reporting information', '') 
set @dceid = (SELECT @@IDENTITY)

-- Reporter Telephone Number
INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (0, 100, NULL, NULL, NULL, '', NULL, '', 0, 3) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid])
	VALUES ('Reporter Telephone Number', @fid, 1, '', '', 0, '1AC02BD6-5C24-4A37-9742-C6B868ED985D') 
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (93, @dscid, @deid, 0, 0, 'Contact telephone number for reporter', '') 
set @dceid = (SELECT @@IDENTITY)

-- Reporter E-mail Address
INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (0, 100, NULL, NULL, NULL, '', NULL, '', 0, 3) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid])
	VALUES ('Reporter E-mail Address', @fid, 1, '', '', 0, 'FFDA770F-DADE-4F6E-B39A-D5E929AEDE2E') 
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (93, @dscid, @deid, 0, 0, 'Contact e-mail address for reporter', '') 
set @dceid = (SELECT @@IDENTITY)

-- Reporter Profession
INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (0, NULL, NULL, NULL, NULL, '', NULL, '', 0, 2) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Physician', 0, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Pharmacist', 0, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Consumer or other non-health professional', 0, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Other Health Professional', 0, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Lawyer', 0, 0, 0, @fid) 
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid])
	VALUES ('Reporter Profession', @fid, 1, '', '', 0, '79ABA1EC-3979-4BA2-80AF-1F817C8243B9') 
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (94, @dscid, @deid, 1, 0, 'Profession of reporter', '') 
set @dceid = (SELECT @@IDENTITY)

-- Report Reference Number
INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (0, 30, NULL, NULL, NULL, '', NULL, '', 0, 3) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid])
	VALUES ('Report Reference Number', @fid, 1, '', '', 0, 'C2EDD7A9-8031-4836-99A0-32C84D99A0AC') 
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (96, @dscid, @deid, 0, 0, 'Report reference number (if any)', '') 
set @dceid = (SELECT @@IDENTITY)

-- Reporter Place of Practice
INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (0, 50, NULL, NULL, NULL, '', NULL, '', 0, 3) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid])
	VALUES ('Reporter Place of Practice', @fid, 1, '', '', 0, 'A517AAE8-76BD-41F0-8FEE-BD45FCE4EBC8') 
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (98, @dscid, @deid, 1, 0, 'Reporter place of practise', '') 
set @dceid = (SELECT @@IDENTITY)

-- Keep Reporter Confidential
INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (0, NULL, NULL, NULL, NULL, '', NULL, '', 0, 5) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid])
	VALUES ('Keep Reporter Confidential', @fid, 1, '', '', 0, '63AE2D15-029B-4D7C-87BA-16C52A67A909') 
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (100, @dscid, @deid, 0, 0, 'Keep reporter confidential?', 'Do you want your identity kept confidential except to be contacted by the national medical regulatory authority or the World Health Organization if they need additional information?') 
set @dceid = (SELECT @@IDENTITY)

/**************************************************
SET ORDERING
**************************************************/
-- Category order
--UPDATE a set a.CategoryOrder = a.Row# from (SELECT ds.DatasetName, dc.DatasetCategoryName, dc.CategoryOrder, ROW_NUMBER() OVER(ORDER BY dc.Id, dc.CategoryOrder, dc.DatasetCategoryName ASC) AS Row#
--	FROM Dataset ds
--	INNER JOIN DatasetCategory dc ON ds.Id = dc.Dataset_Id 
--where ds.Id = @Id and dc.System = 0) as a

---- Element order
--UPDATE a set a.FieldOrder = a.Row# from (SELECT ds.DatasetName, dc.DatasetCategoryName, dc.CategoryOrder, de.Id AS DatasetElementID, de.ElementName, dce.FieldOrder, de.DefaultValue, dce.Acute, dce.Chronic, dce.[System], f.MaxLength, f.Decimals, ft.Description, ROW_NUMBER() OVER(ORDER BY dc.CategoryOrder, dc.DatasetCategoryName, dce.Id, dce.FieldOrder ASC) AS Row#
--	FROM Dataset ds
--	INNER JOIN DatasetCategory dc ON ds.Id = dc.Dataset_Id 
--	INNER JOIN DatasetCategoryElement dce ON dc.Id = dce.DatasetCategory_Id 
--	INNER JOIN DatasetElement de ON dce.DatasetElement_Id = de.Id 
--	INNER JOIN Field f ON de.Field_Id = f.Id
--	INNER JOIN FieldType ft ON f.FieldType_Id = ft.Id
--where ds.Id = @Id and dc.System = 0) as a

SELECT ds.DatasetName, dc.DatasetCategoryName, de.ElementName FROM Dataset ds
	INNER JOIN DatasetCategory dc ON ds.Id = dc.Dataset_Id 
	INNER JOIN DatasetCategoryElement dce ON dc.Id = dce.DatasetCategory_Id 
	INNER JOIN DatasetElement de ON dce.DatasetElement_Id = de.Id 
where ds.Id = @dsid
	
--ROLLBACK TRAN A1
COMMIT TRAN A1

