CREATE TRIGGER [TrigVersionControlMonitor]
ON DATABASE
FOR DDL_DATABASE_LEVEL_EVENTS
AS
SET NOCOUNT ON
SET ANSI_PADDING ON

declare @EventType varchar(100)
declare @SchemaName varchar(100)
declare @ObjectName varchar(100)
declare @ObjectType varchar(100)
declare @EventDataText nvarchar(max)
declare @spId smallint
declare @eventDataXml xml=EVENTDATA()
declare @currentDateTime datetime=GETDATE()
declare @objectId int


declare @versionId int
declare @revisionNo int


SELECT 
 @EventType = @eventDataXml.value('(/EVENT_INSTANCE/EventType)[1]','nvarchar(max)')  
,@SchemaName = @eventDataXml.value('(/EVENT_INSTANCE/SchemaName)[1]','nvarchar(max)')  
,@ObjectName = @eventDataXml.value('(/EVENT_INSTANCE/ObjectName)[1]','nvarchar(max)')
,@ObjectType = @eventDataXml.value('(/EVENT_INSTANCE/ObjectType)[1]','nvarchar(max)')   
,@EventDataText = @eventDataXml.value('(/EVENT_INSTANCE/TSQLCommand/CommandText)[1]','nvarchar(max)')
,@spId=@eventDataXml.value('(/EVENT_INSTANCE/SPID)[1]','smallint')



--------------- version control on table script-----------------

if(@ObjectType='TABLE')
begin

	if(@EventType like 'DROP%')
	begin
		if  @ObjectName like 'Tmp_%'
			return;
		else 
		begin 
			exec vc.procChangeObjectDropedPeriodRange @schemaName,@objectName,'DROP'
			return
		end
	end
	if(@EventType like 'ALTER%' and @EventDataText like '%LOCK_ESCALATION%')
		return;
	if((@EventType like 'CREATE%' or @EventType like 'ALTER%') and @ObjectName like 'Tmp_%')
		return;
	
	if @EventType like 'CREATE%'
		exec vc.procChangeObjectDropedPeriodRange @schemaName,@objectName,'CREATE'

	declare @newTableScript nvarchar(max)
	declare @tableFullName varchar(1000)='['+@SchemaName+'].['+@ObjectName+']'
	declare @actualObjectName varchar(1000)=@ObjectName
	
	
	if(@EventType='RENAME')
	begin
			declare @newTableName varchar(1000)
			set @newTableName = @eventDataXml.value('(/EVENT_INSTANCE/NewObjectName)[1]','varchar(1000)')
			if('Tmp_'+@newTableName=@ObjectName)
			begin
				set @actualObjectName=@newTableName
			end
			else
			begin
				exec vc.procGetTrackedObject @SchemaName,@ObjectName,@currentDateTime,@objectId out
				exec vc.procRenameObject @objectId,@newTableName,@currentDateTime,@newTableScript out
				exec vc.procLoadNewVersion 
					@objectId,
					@objectType,
					@newTableScript,
					@spid,
					@currentDateTime

				return
			end
	end
	set @tableFullName='['+@SchemaName+'].['+@actualObjectName+']'
	
	exec vc.procScriptTable @TableName=@tableFullName,@TableScript=@newTableScript out
	exec vc.procGetTrackedObject @SchemaName,@actualObjectName,@currentDateTime,@objectId out
	exec vc.procLoadNewVersion 
		@objectId,
		@objectType,
		@newTableScript,
		@spid,
		@currentDateTime
	
	return
end



---------------------------------------------------------------


--------------Version control on Procedure, function and views---------------------



if(@ObjectType not in ('PROCEDURE','FUNCTION','VIEW') )
	return;

if @EventType like 'CREATE%'
	exec vc.procChangeObjectDropedPeriodRange @schemaName,@objectName,'CREATE'

if @EventType like 'DROP%'
begin 
	exec vc.procChangeObjectDropedPeriodRange @schemaName,@objectName,'DROP'
	return
end


if not (
@EventType like 'ALTER%' or 
@EventType like 'CREATE%' or
@EventType like 'RENAME%' 
)
	return;

exec vc.procGetTrackedObject @SchemaName,@ObjectName,@currentDateTime,@objectId out

if(@EventType='RENAME')
begin
	
	declare @newObjectName varchar(1000)
	declare @renamedObjectScript nvarchar(max)
	set @newObjectName = @eventDataXml.value('(/EVENT_INSTANCE/NewObjectName)[1]','varchar(1000)')
	exec vc.procRenameObject @objectId,@newObjectName,@currentDateTime,@renamedObjectScript out
	exec vc.procLoadNewVersion 
		@objectId,
		@objectType,
		@renamedObjectScript,
		@spid,
		@currentDateTime

	return
end



exec vc.procLoadNewVersion 
	@objectId,
	@objectType,
	@EventDataText,
	@spid,
	@currentDateTime

