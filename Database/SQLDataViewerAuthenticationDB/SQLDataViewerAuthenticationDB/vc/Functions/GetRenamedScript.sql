create function [vc].[GetRenamedScript]
(
	@schema varchar(200),
	@oldName varchar(200),
	@newName varchar(200),
	@objectScript nvarchar(max)
)
returns nvarchar(max)
begin
	
	declare @newFullName varchar(500)=quotename(@schema)+'.'+quotename(@newName)


	declare @lastPatIndex int=charindex(@oldName,@objectScript)
	if @lastPatIndex=0
		set @lastPatIndex=charindex(quotename(@oldName),@objectScript)

	declare @oldFullNames table
	(
		Id int,
		OldFullName varchar(500)
	)

	insert into  @oldFullNames
	values
	(6,quotename(@schema)+'.'+quotename(@oldName)),
	(5,@schema+'.'+quotename(@oldName)),
	(4,@schema+'.'+@oldName),
	(3,quotename(@schema)+'.'+@oldName),
	(2,quotename(@oldName)),
	(1,@oldName)


	declare @max int
	select @max=max(id) from @oldFullNames

	while(@max>0)
	begin
		
		declare @oldFullName varchar(400)
		declare @index int

		select @oldFullName=OldFullName from @oldFullNames where Id=@max
		set @max=@max-1

		set @index=charindex(@oldFullName,@objectScript)
		
		if @index=0 or @index>@lastPatIndex
			continue
		else 
		begin 
			set @objectScript=stuff(@objectScript,@index,len(@oldFullName),@newFullName)
			break
		end

	end

	

	return @objectScript

end
