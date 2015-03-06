CREATE procedure [vc].[procLoadAllObjectsToVc]
as
begin
	
	declare @schemaName varchar(200)
	declare @objectName varchar(200)
	declare @objectType varchar(200)

	declare curObjects cursor for
	select 
		SPECIFIC_SCHEMA as SchemaName,
		SPECIFIC_NAME as ObjectName,
		ROUTINE_TYPE as ObjectType
	from INFORMATION_SCHEMA.ROUTINES

	union 

	select 
		TABLE_SCHEMA as SchemaName,
		TABLE_NAME as ObjectName,
		case TABLE_TYPE when 'BASE TABLE' then 'TABLE' else 'VIEW' end as ObjectType
	from 
	INFORMATION_SCHEMA.TABLES 

	open curObjects

	declare @datetimeNow datetime
	declare @objectId int
	declare @objectScript nvarchar(max)
	declare @objectFullName varchar(500)
	declare @oldObjectName varchar(200)

	fetch next from curObjects into 
		@schemaName,
		@objectName,
		@objectType



	while(@@FETCH_STATUS=0)
	begin
		
		set @objectFullName=quoteName(@schemaName)+'.'+quotename(@objectName)
		set @objectScript=null
		set @oldObjectName=null
		set @objectId=null
		set @datetimeNow=getdate()



		if @objectType='TABLE'
		begin
			
			exec [vc].[procScriptTable] 
				@TableName=@objectFullName,
				@TableScript=@objectScript out

		end
		else 
		begin 
			select @objectScript=definition
			from sys.all_sql_modules
			where object_id=OBJECT_ID(@objectFullName)
		end

		if @objectScript is null
			goto fetchNext


		select @objectId=ObjectId 
		from vc.TrackedObjects
		where SchemaName=@schemaName
		and ObjectName=@objectName
		
		


		if @objectId is null
		begin 
			insert into vc.TrackedObjects
			(
				[SchemaName],
				ObjectName,
				LastUpdateDate
			)
			values
			(
				@schemaName,
				@objectName,
				@datetimeNow
			)
			set @objectId=SCOPE_IDENTITY()
		end


		select @oldObjectName=ObjectName 
		from vc.ObjectVersions 
		where ObjectId=@objectId
		and NextVersionId is null

		if @oldObjectName is not null and @oldObjectName<>@objectName
		begin 
			set @objectScript=vc.GetRenamedScript(@schemaName,@oldObjectName,@objectName,@objectScript)
		end

		exec [vc].[procLoadNewVersion]
			@objectId,
			@objectType,
			@objectScript,
			@@spid,
			@datetimeNow 
			
		
		fetchNext:
		fetch next from curObjects 
		into @schemaName,@objectName,@objectType
	
	end


	close curObjects
	deallocate curObjects

end
