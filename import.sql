GO
CREATE VIEW [dbo].[QuestionRow] AS
                SELECT  
                    Q.QuestionId, 
                    Q.Header, 
                    Q.Description,
                    CONCAT(
                        ISNULL((SELECT STRING_AGG(DistinctTitles.Shortcut, ' ') WITHIN GROUP (ORDER BY DistinctTitles.Shortcut)
                                FROM (SELECT DISTINCT TT.TeacherId, Titles.TitleId, Titles.Shortcut
                                    FROM TeacherTitles AS TT
                                    JOIN Titles ON Titles.TitleId = TT.TitleId
                                    WHERE TT.TeacherId = T.TeacherId) AS DistinctTitles), ''),
                        ' ',
                        T.FirstName, 
                        ' ',
                        T.LastName
                    ) AS CreatorName,
                    COUNT(QuestionOptions.QuestionId) AS OptionCount,
                    QuestionTypes.Name AS 'QuestionTypeName',
                    StudentFields.Name AS 'FieldName',
                    Q.IsActive
            FROM Questions AS Q
            JOIN Teachers AS T ON Q.CreatorId = T.TeacherId
            LEFT JOIN QuestionOptions ON Q.QuestionId = QuestionOptions.QuestionId
            LEFT JOIN QuestionTypes ON Q.QuestionTypeId = QuestionTypes.QuestionTypeId
            LEFT JOIN StudentFields ON Q.FieldId = StudentFields.StudentFieldId
            GROUP BY Q.QuestionId, Q.Header, T.FirstName, T.LastName, T.TeacherId, QuestionTypes.Name, StudentFields.Name, Q.IsActive, Q.Description
END


GO 
IF OBJECT_ID(N'[dbo].[Sessions]', N'U') IS NULL
        BEGIN
            CREATE TABLE [dbo].[Sessions](
                [Id] NVARCHAR(449) NOT NULL,
                [Value] VARBINARY(MAX) NOT NULL,
                [ExpiresAtTime] DATETIMEOFFSET NOT NULL,
                [SlidingExpirationInSeconds] BIGINT NULL,
                [AbsoluteExpiration] DATETIMEOFFSET NULL,
                CONSTRAINT [PK_Sessions] PRIMARY KEY ([Id])
            );
            CREATE INDEX [IX_Sessions_ExpiresAtTime] ON [dbo].[Sessions] ([ExpiresAtTime]);
        END
END