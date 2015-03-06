CREATE procedure [vc].[procRenameObject]
@objectId int,
@newObjectName varchar(100),
@updateDateTime datetime,
@objectScript nvarchar(max) out
as
begin
	
	declare @def nvarchar(max)
	declare @schema varchar(200)
	declare @oldName varchar(200)
	declare @versionId int

	select 
		@versionId=[VersionId],
		@def=ltrim([ObjectScript]),
		@oldName=ObjectName,
		@schema=SchemaName 
	from vc.ObjectVersions
	where ObjectId=@objectId
	and NextVersionId is null
	set @objectScript=vc.GetRenamedScript(@schema,@oldName,@newObjectName,@def)
	
	update vc.TrackedObjects
	set ObjectName=@newObjectName,
	LastUpdateDate=@updateDateTime
	where ObjectId=@objectId


end
