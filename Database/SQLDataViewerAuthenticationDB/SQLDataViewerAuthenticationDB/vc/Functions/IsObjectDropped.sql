CREATE function [vc].[IsObjectDropped](@objectId int,@datetime datetime)
returns int
begin
	if exists 
	(
		select 
		* 
		from 
		vc.ObjectDropedPeriodRange
		where ObjectId=@objectId
		and
		( 
			(
				@datetime is not null and
				@datetime>=StartDatetime and  
				(EndDatetime is null or @datetime<EndDatetime)
			)

			or 
			(
				@datetime is null and EndDatetime is null
			)
		)
	)
	return 1

	return 0

end
