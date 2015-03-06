
var attributes = ["selectAttribute1"];

function fun_addAttribute()
{
    var att = attributes[attributes.length - 1]
    var newAtt = "selectAttribute" + (parseInt(att.replace("selectAttribute", ""))+1);
    //alert(newAtt);
    attributes.push(newAtt);
}







