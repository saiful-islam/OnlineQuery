CREATE procedure [vc].[procAddSnapshot]
@name varchar(200),
@description varchar(4000)
as
begin 

	declare @maxVersionId int 
	select @maxVersionId=max(versionId) 
	from vc.ObjectVersions

	insert into vc.Snapshots
	(
		Name,
		Description,
		SnapshotDatetime,
		MaxVersionId
	)
	values(@name,@description,GETDATE(),@maxVersionId)

end
