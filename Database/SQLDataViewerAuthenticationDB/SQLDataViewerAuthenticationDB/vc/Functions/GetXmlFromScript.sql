CREATE function [vc].[GetXmlFromScript](@script nvarchar(max))
returns xml
begin

	set @script=replace(@script,'&','&amp;')
	set @script=replace(@script,'"','&quot;')
	set @script=replace(@script,'<','&lt;')
	set @script=replace(@script,'>','&gt;')
	
	
    return (CONVERT([xml],((('<script>'+char((10)))+@script)+char((10)))+'</script>',(0)))
	

end
