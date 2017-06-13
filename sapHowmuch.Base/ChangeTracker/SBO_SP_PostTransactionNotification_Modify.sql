-- Filter object type
IF @object_type IN ('2', '4', '17', '15') -- BP, Items, Orders, Deliveries
BEGIN

DECLARE @timestamp INT
, @expired INT
, @startdate DATE = '2017-06-01' -- 배포일?
, @key NVARCHAR(30)

SET @timestamp = DATEDIFF(SECOND, @startdate, GETDATE())
SET @expired = DATEDIFF(SECOND, @startdate, DATEADD(DAY, -7, GETDATE))
SET @key = CAST(@list_of_cols_val_tab_del AS NVARCHAR(30));

-- clean up old records, expired or same key/object
DELETE FROM [@SAPHOWMUCH_CHANGETRACKER] WHERE ([U_SHM_CT_Key] = @key AND [U_SHM_CT_Obj] = CAST(@object_type AS NVARCHAR(30))) OR CAST([Code] AS INT) < @expired

-- since [Code] is unique key, we have to ensure its unique
WHILE EXISTS (SELECT 1 FROM [@SAPHOWMUCH_CHANGETRACKER] WHERE [Code] = @timestamp)
BEGIN
	SET @timestamp = @timestamp + 1
END

INSERT INTO [@SAPHOWMUCH_CHANGETRACKER]
VALUES (@timestamp, @timestamp, @key, CAST(@object_type AS NVARCHAR(30)))

END
