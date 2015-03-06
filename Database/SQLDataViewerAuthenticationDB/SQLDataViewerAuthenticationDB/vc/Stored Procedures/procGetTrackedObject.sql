CREATE procedure [vc].[procGetTrackedObject]
@schemaName varchar(100),
@objectName varchar(1000),
@updateDatetime datetime,
@objectId int output
as
begin

select @objectId=ObjectId from vc.TrackedObjects
where SchemaName=@SchemaName
and ObjectName=@objectName


if(@objectId is null)
begin
	insert into vc.TrackedObjects
	(
		SchemaName,
		ObjectName,
		LastUpdateDate
	)
	values
	(@SchemaName,@ObjectName,@updateDatetime)
	
	set @objectId= SCOPE_IDENTITY()
end	
else 
	update vc.TrackedObjects
	set LastUpdateDate=@updateDatetime
	where ObjectId=@objectId

end
