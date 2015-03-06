CREATE procedure [vc].[procChangeObjectDropedPeriodRange]
	@schemaName varchar(200),
	@objectName varchar(200),
	@eventType varchar(10)
as
begin 

	if @objectName='ObjectDropedPeriodRange' and @schemaName='vc'
		return

	if @eventType not in ('DROP','CREATE')
	begin
		declare @errorMessage varchar(500)=@eventType+' is not valid. Allowed eventType is DROP and CREATE'
		raiserror(@errorMessage,13,1)
		return
	end

	declare @datetimeNow datetime=getdate()
	declare @objectId int 
	declare @lastEventId int
	
	exec [vc].[procGetTrackedObject] @schemaName,@objectName,@datetimeNow,@objectId out
	
	select @lastEventId=Id
	from vc.ObjectDropedPeriodRange
	where ObjectId=@objectId
	and EndDatetime is null

	if @lastEventId is not null and @eventType='CREATE'
		update vc.ObjectDropedPeriodRange
		set EndDatetime=@datetimeNow
		where Id=@lastEventId
	
	if @eventType='DROP'
		insert into vc.ObjectDropedPeriodRange
		(
			[ObjectId],
			[StartDatetime]
		)
		values(@objectId,@datetimeNow)


end
