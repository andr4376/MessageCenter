<%@ Page Title="Home" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="MessageCenter._Default" MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="jumbotron">
        <h1>ASP.NET</h1>
        <p class="lead">ASP.NET is a free web framework for building great Web sites and Web applications using HTML, CSS, and JavaScript.</p>
        <p><a href="http://www.asp.net" class="btn btn-primary btn-lg">Learn more &raquo;</a></p>
    </div>  
    
    <div class="row">
        <div class="col-md-4">
            <h2>Getting started</h2>
            <p>
                ASP.NET Web Forms lets you build dynamic websites using a familiar drag-and-drop, event-driven model.
            A design surface and hundreds of controls and components let you rapidly build sophisticated, powerful UI-driven sites with data access.
            </p>
            <p>
                <a class="btn btn-default" href="https://go.microsoft.com/fwlink/?LinkId=301948">Learn more &raquo;</a>
            </p>
        </div>
        <div class="col-md-4">
            <h2>Get more libraries</h2>
            <p>
                NuGet is a free Visual Studio extension that makes it easy to add, remove, and update libraries and tools in Visual Studio projects.
            </p>
            <p>
                <a class="btn btn-default" href="https://go.microsoft.com/fwlink/?LinkId=301949">Learn more &raquo;</a>
            </p>
        </div>
       
        <div class="row">

        <div class="col-md-4">
            <h2>Web Hosting</h2>
            <p>
                You can easily find a web hosting company that offers the right mix of features and price for your applications.
            </p>
            <p>
                <a class="btn btn-default" href="https://go.microsoft.com/fwlink/?LinkId=301950">Learn more &raquo;</a>
            </p>
        </div>
    </div>       
     </div>

    <!--række til søge input og listbox med beskeder -->
        <div class ="row">

    <!-- Input til søgning START-->
    <div class="container col-md-2">
        <div>    
            
            <asp:TextBox ID="searchInput" runat="server"  defaultButton="searchBtn" placeholder="Søg efter besked..." CssClass="sparkron-search-input"/>
            <asp:Button ID ="searchBtn" runat="server" OnClick="searchBtn_Click" Text="Søg" CssClass="sparkron-search-btn"/>
            
        </div>       
        
    </div>
     <!-- Input til søgning SLUT-->

     <!-- ListBox som indeholder Besked Skabeloner START-->
    <div class="container col-md-6 msg-box-sizer" style="padding-left:50px">      
        
            <h2 class="sparkron-box-header">
                <asp:Image ID="logo" runat="server" ImageUrl="~/Images/envelope.png" CssClass="mini-logo"/>
                Besked skabeloner</h2>               

            <asp:ListBox 
             ID="listBoxMessageTemplates" runat="server" 
             CssClass ="template-msg-listbox sparkron-box"            
            defaultButton="btn_proceedToMessagePage"></asp:ListBox>         
        
        <!--fortsæt knap-->
             <asp:Button ID="btn_proceedToMessagePage" Text="Fortsæt" runat="server" OnClick="btn_proceedToMessagePage_Click" CssClass="sparkron-submit-btn" />                    
   </div>
     <!-- ListBox som indeholder Besked Skabeloner SLUT-->

    </div>
</asp:Content>
