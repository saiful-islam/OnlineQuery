CREATE procedure [vc].[procLoadNewVersion]
@objectId int,
@objectType varchar(100),
@objectScript nvarchar(max),
@spid smallint,
@updateDatetime datetime
as
begin
	/* loading new version */
	declare @objectName varchar(1000)
	declare @schemaName varchar(100)
	declare @firstVersionId int
	declare @hostName nvarchar(128)
	declare @loginName nvarchar(128)
	declare @programName nvarchar(128)
	declare @netAddress nchar(12)
	declare @netLibrary nchar(12)
	declare @comment nvarchar(max)
	declare @prevObjectScript nvarchar(max)
	declare @versionId int
	declare @revisionNo int

	select 
		@objectName=ObjectName,
		@schemaName=SchemaName	
	from vc.TrackedObjects
	where ObjectId=@objectId	

	select @versionId=MAX(VersionId) from 
	vc.ObjectVersions 
	where ObjectId=@objectId 
	
	
	select
		@revisionNo=RevisionNo,
		@firstVersionId=FirstVersionId,
		@prevObjectScript=ObjectScript from vc.ObjectVersions
	where VersionId=@versionId

	if(@prevObjectScript=@objectScript COLLATE SQL_Latin1_General_CP1_CS_AS)
		goto printLabel
	
	set @revisionNo=isnull(@revisionNo,0)+1

	
	select 
		@hostName=hostname,
		@loginName=loginame,
		@programName=program_name,
		@netAddress=net_address,
		@netLibrary=net_library
	from sys.sysprocesses
	where spid=@spId


	---------extracting the comment----------

	declare @startIndex int=charindex('/*',@objectScript)
	declare @endIndex int
	if(@startIndex>0)
		set @endIndex=charindex('*/',@objectScript,@startIndex)
	
	if(@startIndex>0 and @endIndex>@startIndex)
		set @comment=SUBSTRING(@objectScript,@startIndex+2,@endIndex-@startIndex-2)
	

	------------------------------------------


insert into vc.ObjectVersions
(
	FirstVersionId,
	PrevVersionId, 
	VersionDatetime, 
	RevisionNo,
	Comment, 
	ObjectId, 
	SchemaName, 
	ObjectName, 
	ObjectType, 
	ObjectScript, 
	HostName, 
	LoginName, 
	ProgramName, 
	NetAddress, 
	NetLibrary, 
	SpId
)
values
(
	@firstVersionId,
	@versionId,
	@updateDatetime,
	@revisionNo,
	@comment,
	@objectId,
	@SchemaName,
	@objectName,
	@ObjectType,
	@objectScript,
	@hostName,
	@loginName,
	@programName,
	@netAddress,
	@netLibrary,
	@spId
)

declare @newVersionId int=scope_identity()

if(@versionId is null)
	update vc.ObjectVersions
	set FirstVersionId=@newVersionId
	where VersionId=@newVersionId
else 
	update vc.ObjectVersions
	set NextVersionId=@newVersionId
	where VersionId=@versionId
	
	
set @versionId=@newVersionId

printLabel:
print 'Command completed successfully
From VersionControl: Saved as a version successfully. 
Object name: '+quotename(@schemaName)+'.'+quotename(@objectName)+'
VersionId: '+cast(@versionId as varchar)+'
Revision No: '+cast(@revisionNo as varchar)


	
end
