<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewReport.aspx.cs" Inherits="PCSReports.Reports.ReportTemplate" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <link rel="stylesheet" href="Content/public/css/reset.css" type="text/css" />
    <link rel="stylesheet" href="Content/public/css/default.css" type="text/css" />
    <link rel="stylesheet" href="Content/public/css/style.css" type="text/css" />
    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.11.1/jquery.min.js"></script>
    <script type="text/javascript" src="Content/public/javascript/zebra_datepicker.js"></script>
    <script type="text/javascript" src="Content/public/javascript/core.js"></script>
    <script type="text/javascript" src="Content/public/javascript/AddDatePicker.js"></script>
    <script type="text/javascript">
$(document).ready(function(){

 if ($.browser.webkit)
 {
    $($(":hidden[id*='DatePickers']").val().split(",")).each(function(i, item){
         var h = $("table[id*='ParametersGrid'] span").filter(function(i) {
             var v = "[" + $(this).text() + "]";
             return (v != null && v.indexOf(item) >= 0);
          }).parent("td").next("td").find("input").datepicker({
           showOn: "button"
           ,buttonImage: '/Reserved.ReportViewerWebControl.axd?OpType=Resource&Name=Microsoft.Reporting.WebForms.calendar.gif'
           ,buttonImageOnly: true
           ,dateFormat: 'mm/dd/yyyy'
           ,changeMonth: true
           ,changeYear: true
           });
     });
  }

});
    </script>
</head >
 
<body style="margin: 0px; padding: 0px;">
    <form id="form1" runat="server">
        <asp:HiddenField ID="DatePickers" runat="server" />
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server" AsyncPostBackTimeOut="36000" EnablePageMethods="true" EnablePartialRendering="true">               
            </asp:ScriptManager>
            <rsweb:ReportViewer id="rvSiteMapping" runat ="server" ShowPrintButton="false"  Width="99%" Height="99%" AsyncRendering="true" ZoomMode="Percent" KeepSessionAlive="true" SizeToReportContent="false" BackColor="LightSkyBlue" BorderColor="LightSkyBlue">
            </rsweb:ReportViewer>  
        </div>
    </form>
</body>
</html>
