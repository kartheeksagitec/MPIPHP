ALTER DATABASE [$(DatabaseName)]
    ADD FILE (NAME = [MPIPHP_Test], FILENAME = '$(Path2)$(DatabaseName).mdf', FILEGROWTH = 1024 KB) TO FILEGROUP [PRIMARY];

