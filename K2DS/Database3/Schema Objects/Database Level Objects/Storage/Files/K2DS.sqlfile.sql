ALTER DATABASE [$(DatabaseName)]
    ADD FILE (NAME = [K2DS], FILENAME = '$(DefaultDataPath)$(DatabaseName).mdf', FILEGROWTH = 1024 KB) TO FILEGROUP [PRIMARY];

