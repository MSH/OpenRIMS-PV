EXECUTE dbo.DatabaseBackup 
		@Databases = 'PViMS.Demo', 
		@Directory = 'C:\AppData\Backups',
		@BackupType = 'FULL',
		@Verify = 'Y',
		@CheckSum = 'Y',
		@CleanupTime = 120,
		@CleanupMode = 'AFTER_BACKUP'
