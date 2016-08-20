CREATE TABLE #TableNames(
    Id Int Identity(1,1) Primary Key NOT NULL,
    Name NVARCHAR(50)  NOT NULL
);

INSERT INTO #TableNames(Name)
select name from sysobjects where xtype='u';

DECLaRE @NAMESPACEX NVARCHAR(200) = '';
DECLARE @TName NVARCHAR(50) = '';
DECLARE @SB NVARCHAR(4000)='';
DECLARE @IDX INT = 1;
DECLARE @CNT INT;
SELECT @CNT=COUNT(1) FROM #TableNames;
DECLARE @IDX2 INT = 1;
DECLARE @CNT2 INT;
DECLARE @NAME NVARCHAR(50);
DECLARE @XTYPE NVARCHAR(50);
DECLARE @LENGTH NVARCHAR(50);
DECLARE @COLLATION NVARCHAR(50);

WHILE @IDX<=@CNT
BEGIN
SELECT TOP 1 @TName=Q.Name FROM (SELECT ROW_NUMBER() OVER(ORDER BY Id) AS RW,Name FROM #TableNames) AS Q WHERE Q.RW=@IDX;
SELECT @CNT2=COUNT(1) from syscolumns where id=OBJECT_ID(@TName);
SET @SB+= 'using System;'+CHAR(10)+CHAR(10);
SET @SB+= 'namespace '+@NAMESPACEX+CHAR(10);
SET @SB+= '{'+CHAR(10);
SET @SB+= '    public class '+@TName+CHAR(10);
SET @SB+= '    {'+CHAR(10);
WHILE @IDX2<=@CNT2
BEGIN
SELECT TOP 1 @NAME=Q.name,@XTYPE=(CASE Q.xtype WHEN 56 THEN 'int' WHEN 61 THEN 'DateTime' ELSE 'string' END),@LENGTH=CAST(Q.[length] AS NVARCHAR(10)),@COLLATION=Q.collation FROM (SELECT ROW_NUMBER() OVER(ORDER BY colorder) AS RW,name,xtype,[length],collation FROM syscolumns where id=OBJECT_ID(@TName)) AS Q WHERE Q.RW=@IDX2;
SET @SB+= '        public '+@XTYPE+' '+@NAME+' {get;set;}'+CHAR(10)+CHAR(10);
SET @IDX2+=1;
END
SET @SB+= '    }'+CHAR(10);
SET @SB+= '}'+CHAR(10)+CHAR(10);
SET @CNT2=0;
SEt @IDX2=0;
SET @IDX+=1;
END

PRINT(@SB);
DROP TABLE #TableNames