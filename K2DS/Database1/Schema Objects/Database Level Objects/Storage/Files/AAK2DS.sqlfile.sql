ALTER DATABASE [$(DatabaseName)]
    ADD FILE (NAME = [AAK2DS], FILENAME = '$(DefaultDataPath)$(DatabaseName).mdf', FILEGROWTH = 1024 KB) TO FILEGROUP [PRIMARY];

