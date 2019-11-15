<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Messages.aspx.cs" Inherits="MessageCenter.Messages" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <!--Pick user Modal-->
    <div class="modal fade" id="pickUserModal" tabindex="-1" role="dialog" aria-labelledby="userModal" aria-hidden="true"
        data-toggle="modal" data-backdrop="static" data-keyboard="false">
        <div class="modal-dialog" role="document">

            <div class="modal-content">

                <div class="modal-header">
                    <h2 class="modal-title" id="userModal">Login</h2>

                </div>
                <div class="modal-body">

                    <!-- CPR input -->
                    <asp:Panel runat="server" DefaultButton="btn_Submit_User">
                        <div class="login-box container">
                            <div class="row">
                                <asp:TextBox ID="customerCprInput" runat="server" placeholder="CPR" CssClass="sparkron-search-input"></asp:TextBox>
                                <asp:Button ID="searchBtnCustomer" runat="server" OnClick="searchBtnCustomer_Click"
                                    Text="Søg" CssClass="sparkron-search-btn" CausesValidation="False" />
                            </div>
                            <div class="row">
                                <!-- UpdatePanel tillader at indholdet kan opdateres uden PostBack uden at skulle bruge AJAX -->
                                <asp:UpdatePanel ID="UPCustomersListbox" runat="server">

                                    <ContentTemplate>
                                        <asp:ListBox
                                            ID="listBoxCustomers" runat="server"
                                            CssClass="template-msg-listbox sparkron-customer-box"></asp:ListBox>
                                    </ContentTemplate>

                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="btn_Submit_User" EventName="Click" />
                                    </Triggers>
                                </asp:UpdatePanel>
                            </div>

                        </div>
                    </asp:Panel>


                </div>
        <div class="modal-footer">
            <asp:Button ID="btn_Submit_User" Text="Vælg Kunde" runat="server" OnClick="btn_Submit_User_Click" CssClass="sparkron-submit-btn" />
        </div>
    </div>
    </div>
    </div>
    <script type="text/javascript">
        function openPickUserModal() {
            $('#pickUserModal').modal({ show: true });
        }
    </script>
    <!--Pick user Modal END-->


</asp:Content>
