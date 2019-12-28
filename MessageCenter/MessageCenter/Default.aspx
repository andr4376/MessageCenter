<%@ Page Title="Home" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs"
    Inherits="MessageCenter._Default" MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">


    <!--række til søge input og listbox med beskeder -->
    <div class="row">
        <!-- Input til søgning START-->
        <div class="container col-md-2 search-group">
            <div>
                <!-- Panel, så Enter kan bruges til at søge-->
                <asp:Panel runat="server" DefaultButton="searchBtn">
                    <asp:TextBox ID="searchInput" runat="server" placeholder="Søg efter besked..." CssClass="sparkron-search-input" autocomplete="off" />
                    <asp:Button ID="searchBtn" runat="server" OnClick="searchBtn_Click" Text="Søg" CssClass="sparkron-search-btn" CausesValidation="False" />
                </asp:Panel>
            </div>
        </div>
        <!-- Input til søgning SLUT-->

        <!-- ListBox som indeholder Besked Skabeloner START-->
        <div class="container col-md-8" style="padding-left: 50px">
            <h2 class="sparkron-box-header">
                <asp:Image ID="logo" runat="server" ImageUrl="~/Images/envelope.png" CssClass="mini-logo"  />

                <asp:ImageButton ID="addNewMessageBtn" runat="server" 
                    ImageUrl="~/Images/addNew.png" CssClass="mini-logo-right" 
                    OnClick="addNewMessageBtn_Click" Visible="<%#ShowAdminInterface%>"
                    CausesValidation="false"
                     ToolTip="Tilføj ny besked skabelon"/>

                <asp:ImageButton ID="removeMessageTemplate" runat="server"
                    ImageUrl="~/Images/delete.png" CssClass="mini-logo-right" 
                    OnClick="removeMessageTemplate_Click" Visible="<%#ShowAdminInterface%>"
                    CausesValidation="false"
                    ToolTip="Slet den valgte besked skabelon"/>
                Besked skabeloner
            </h2>

            <!-- Panel, så Enter kan vælge listbox item -->
            <asp:Panel runat="server" DefaultButton="btn_proceedToMessagePage">

                <!-- UpdatePanel tillader at indholdet kan opdateres uden PostBack uden at skulle bruge AJAX -->
                <asp:UpdatePanel ID="UPListbox" runat="server">
                    <ContentTemplate>
                        <asp:ListBox
                            ID="listBoxMessageTemplates" runat="server"
                            CssClass="template-msg-listbox sparkron-box"></asp:ListBox>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="searchBtn" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="removeMessageTemplate" EventName="Click" />
                    </Triggers>
                </asp:UpdatePanel>

            </asp:Panel>
            

            

            <!--fortsæt knap-->
            <asp:Button ID="btn_proceedToMessagePage" Text="Fortsæt" runat="server" OnClick="btn_proceedToMessagePage_Click" CssClass="sparkron-submit-btn" CausesValidation="false" />
        </div>
        <!-- ListBox som indeholder Besked Skabeloner SLUT-->
    </div>


    

</asp:Content>
