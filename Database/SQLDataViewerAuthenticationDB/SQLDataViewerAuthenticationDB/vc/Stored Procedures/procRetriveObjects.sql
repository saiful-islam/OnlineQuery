CREATE procedure [vc].[procRetriveObjects]
@snapshotId int=null,
@datetime datetime=null
as
begin
	

	if @snapshotId is not null
	begin
		declare @maxVersionId int
		declare @snapShotDatetime datetime 
		select 
			@maxVersionId=MaxVersionId, 
			@snapShotDatetime=SnapshotDatetime
		from vc.Snapshots
		where Id=@snapshotId

		select 
			a.*
		from 
		vc.objectVersions a,
		(
			select 
				ObjectId,
				max(VersionId) versionId
			from 
			ObjectVersions a
			where a.VersionId<=@maxVersionId
			group by ObjectId
		) b
		where a.ObjectId=b.ObjectId
		and a.VersionId=b.versionId
		and vc.IsObjectDropped(a.ObjectId,@snapShotDatetime)=0
		order by a.ObjectType,a.SchemaName,a.ObjectName
	
	end


	
	else if @datetime is not null
	begin 
		select 
			a.*
		from 
		vc.objectVersions a,
		(
			select 
				ObjectId,
				max(VersionId) versionId
			from 
			ObjectVersions a
			where a.VersionDatetime<=@datetime
			group by ObjectId
		) b
		where a.ObjectId=b.ObjectId
		and a.VersionId=b.versionId
		and vc.IsObjectDropped(a.ObjectId,@datetime)=0
		order by a.ObjectType,a.SchemaName,a.ObjectName
		return
	end

	else 
		select 
		*
		from 
		vc.ObjectVersions a
		where NextVersionId is null
		and vc.IsObjectDropped(a.ObjectId,null)=0
		order by a.ObjectType,a.SchemaName,a.ObjectName


end
