<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Messages.aspx.cs" Inherits="MessageCenter.Messages" 
    ValidateRequest="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <!--Pick user Modal-->
    <div class="modal fade" id="pickUserModal" tabindex="-1" role="dialog" aria-labelledby="userModal" aria-hidden="true"
        data-toggle="modal" data-backdrop="static" data-keyboard="false">
        <div class="modal-dialog" role="document">

            <div class="modal-content">

                <div class="modal-header">
                    <h2 class="modal-title">Vælg beskedmodtager</h2>

                </div>
                <div class="modal-body">

                    <!-- CPR input -->
                    <div class="login-box container">
                        <div class="row">
                            <asp:Panel runat="server" DefaultButton="searchBtnCustomer">
                                <asp:TextBox ID="customerCprInput" runat="server" placeholder="CPR" CssClass="sparkron-search-input" autocomplete="off"></asp:TextBox>
                                <asp:Button ID="searchBtnCustomer" runat="server" OnClick="searchBtnCustomer_Click"
                                    Text="Søg" CssClass="sparkron-search-btn" CausesValidation="False" />
                            </asp:Panel>
                        </div>
                        <div class="row">
                            <!-- UpdatePanel tillader at indholdet kan opdateres uden PostBack uden at skulle bruge AJAX -->
                            <asp:UpdatePanel ID="UPCustomersListbox" runat="server">

                                <ContentTemplate>
                                    <asp:ListBox
                                        ID="listBoxCustomers" runat="server"
                                        CssClass="template-msg-listbox sparkron-customer-box"></asp:ListBox>
                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="listBoxCustomers"></asp:RequiredFieldValidator>
                                </ContentTemplate>

                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="searchBtnCustomer" EventName="Click" />
                                </Triggers>
                            </asp:UpdatePanel>
                        </div>

                    </div>


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
        function closePickUserModal() {
            $('#pickUserModal').modal('hide');
        }
    </script>
    <!--Pick user Modal END-->

    <!--Message Page Body-->
    <div runat="server" id="messageBody" class="sparkron-message-body" visible="false">

        <div class="container">

            <div class="message-template-section row">
                <asp:TextBox CssClass="sparkron-input-title message-text-input" runat="server"
                    ID="titleTextBox" BorderStyle="None" TextMode="SingleLine"
                    text="<%# this.GetTitle %>">></asp:TextBox>
            </div>
            <div class="message-template-section row">
                <asp:TextBox CssClass="message-text-input sparkron-input-maintext" runat="server" 
                    ID="messageTextTextBox" BorderStyle="None" TextMode="MultiLine"
                    text="<%# this.GetText %>"></asp:TextBox>
            </div>
                    <asp:Button ID="sendMailBtn" Text="Send" runat="server" OnClick="sendMailBtn_Click" CssClass="sparkron-submit-btn" CausesValidation="false"/>
            
        </div>
    </div>
    <!--Message Page Body END-->


</asp:Content>
