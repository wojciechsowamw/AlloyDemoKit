<%@ Page Language="c#" Codebehind="MedicoverTools.aspx.cs" AutoEventWireup="False" Inherits="AlloyDemoKit.AdminTools.MedicoverTools" Title="Medicover Tools" %>

<head>
<!-- Mimic Internet Explorer 7 -->
<meta http-equiv="X-UA-Compatible" content="IE=EmulateIE7" /><link rel="stylesheet" type="text/css" href="/EPiServer/Shell/8.6.1.0/ClientResources/epi/themes/legacy/ShellCore.css" />
<script type="text/javascript" src="/EPiServer/Shell/8.6.1.0/ClientResources/ShellCore.js"></script>
<link rel="stylesheet" type="text/css" href="/EPiServer/Shell/8.6.1.0/ClientResources/epi/themes/legacy/ShellCoreLightTheme.css" />
<script type="text/javascript" src="/EPiServer/CMS/8.6.1.0/ClientResources/ReportCenter/ReportCenter.js"></script>

<link type="text/css" rel="stylesheet" href="/EPiServer/CMS/8.6.1.0/ClientResources/Epi/Base/CMS.css" />
<link href="../../../App_Themes/Default/Styles/system.css" type="text/css" rel="stylesheet" /><link href="../../../App_Themes/Default/Styles/ToolButton.css" type="text/css" rel="stylesheet" />
</head>

<body>

<div class="epi-contentContainer epi-padding">
<div class="epi-contentArea">
    <h1 class="EP-prefix">Employee Settings</h1>
    <p class="EP-systemInfo">Defines data import file names for Employee Import job.  System expects files are in the App_Data folder.
        <br />You may Optionally create test data to use.
    </p>
</div>
                              
<div id="FullRegion_MainRegion_listPanel">
<form runat="server" id="medicoverTools">
   
    
    </form>
    <asp:Label runat="server" ID="OutputMessage" ForeColor="#FF6600"></asp:Label>
    
    Page Id: &nbsp;
    <asp:TextBox runat="server" id="pageIdTextBox" Width="250"></asp:TextBox>
    <div class="epi-buttonDefault">
        <span class="epi-cmsButton">
            <asp:Button runat="server" CssClass="epi-cmsButton-text epi-cmsButton-tools epi-cmsButton-Save" id="SetVisibility" Text="Save" title="Save" OnClick="SetVisibility_Click"  />
        </span>
        &nbsp;&nbsp;
        
    </div>
    </div>
</div>
</body>
