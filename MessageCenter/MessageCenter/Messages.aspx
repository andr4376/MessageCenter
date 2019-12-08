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
                    <asp:Button ID="btn_Submit_Customer" Text="Vælg Kunde" runat="server" OnClick="btn_Submit_Customer_Click" CssClass="sparkron-submit-btn" CausesValidation="false"/>
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

    <!--Add new attachment modal -->
    <div class="modal fade" id="addAttachmentModal" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">
        <div class="modal-dialog" role="document">

            <div class="modal-content">
                <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                    <span aria-hidden="true">&times;</span>
                </button>
                <div class="modal-header">
                    <h2 class="modal-title">Vælg og Upload fil til vedhæftning</h2>
                </div>
                <div class="modal-body">

                    <asp:FileUpload ID="AttachmentFileUpload" runat="server" />
                    <asp:RequiredFieldValidator ErrorMessage="Vælg en fil først" ControlToValidate="AttachmentFileUpload" runat="server" />

                </div>
                <div class="modal-footer">
                    <asp:Button ID="UploadFileBtn" Text="Upload Fil" runat="server" OnClick="UploadFileBtn_Click" CssClass="sparkron-submit-btn" CausesValidation="false"/>
                </div>
            </div>
        </div>
    </div>
    <script type="text/javascript">
        function openAttachmentModal() {
            $('#addAttachmentModal').modal({ show: true });
        }
        function closeAttachmentModal() {
            $('#addAttachmentModal').modal('hide');
        }
    </script>
    <!--Add new attachment modal END-->

    <!--Mail Message Page Body-->
    <div runat="server" id="mailMessageBody" class="sparkron-message-body" visible="false">

        <div class="container">

            <div class="message-template-section-adresse-input row col-md-6">
                <asp:TextBox CssClass="message-text-input" runat="server" autocomplete="off"
                    ID="customerMailInputText" BorderStyle="None" TextMode="SingleLine" placeholder="Modtagers Email adresse"
                    Text="<%# this.GetReceiverAdresse %>"></asp:TextBox>

            </div>
            <div class="message-template-section-adresse-input row col-md-6">
                <asp:TextBox CssClass="message-text-input" runat="server" autocomplete="off"
                    ID="ccAdressInput" BorderStyle="None" TextMode="SingleLine" placeholder="CC"></asp:TextBox>
            </div>
        </div>
        <div class="container">

            <div class="message-template-section row">
                <asp:TextBox CssClass="sparkron-input-title message-text-input" runat="server"
                    ID="titleTextBox" BorderStyle="None" TextMode="SingleLine" placeholder="Beskedens Titel" autocomplete="off"
                    Text="<%# this.GetTitle %>">></asp:TextBox>
            </div>

            <div class="message-template-section row">
                <asp:TextBox CssClass="message-text-input sparkron-input-maintext" runat="server"
                    ID="messageTextTextBox" BorderStyle="None" TextMode="MultiLine" autocomplete="off"
                    Text="<%# this.GetText %>"></asp:TextBox>
            </div>

            <!-- Attachment section -->
            <div id="AttachmentsSection" runat ="server">
                <div class="message-template-section row">
                <h2>Vedhæftede filer</h2>
                    <asp:Panel runat="server" DefaultButton="DownloadAttachmentBtn">
                    <!-- UpdatePanel tillader at indholdet kan opdateres uden PostBack uden at skulle bruge AJAX -->
                    <asp:UpdatePanel ID="UPAttachments" runat="server">

                        <ContentTemplate>
                            <asp:ListBox
                                ID="listBoxAttachments" runat="server"
                                CssClass="attachments-listbox"></asp:ListBox>


                        </ContentTemplate>

                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="RemoveAttachmentButton" EventName="Click" />
                            
                        </Triggers>
                    </asp:UpdatePanel>
                    </asp:Panel>
                </div>
                <asp:Button ID="DownloadAttachmentBtn" Text="Download" runat="server" OnClick="DownloadAttachmentBtn_Click" CssClass="sparkron-submit-btn-sm" CausesValidation="false" />
                <asp:Button ID="RemoveAttachmentButton" Text="Fjern" runat="server" OnClick="RemoveAttachmentButton_Click" CssClass="sparkron-submit-btn-sm" CausesValidation="false" />
                <asp:Button ID="openNewAttachmentModalBtn" Text="Tilføj Fil" runat="server" OnClick="openNewAttachmentModalBtn_Click" CssClass="sparkron-submit-btn-sm" CausesValidation="false" />
            </div>

            <asp:Button ID="sendMailBtn" Text="Send" runat="server" OnClick="sendMailBtn_Click" CssClass="sparkron-submit-btn" CausesValidation="false"
                OnClientClick="this.disabled=true;" UseSubmitBehavior="false" />
        </div>



    </div>
    <!--Mail Message Page Body END-->

    <!--SMS Message Page Body-->
    <div runat="server" id="smsMessageBody" class="sparkron-message-body" visible="false">

        <div class="container">

            <div class="message-template-section-adresse-input row">
                <asp:TextBox CssClass="message-text-input" runat="server" autocomplete="off"
                    ID="smsPhoneNumber" BorderStyle="None" TextMode="SingleLine" placeholder="Modtagers Mobil Nr."
                    Text="<%# this.GetReceiverAdresse %>"></asp:TextBox>

            </div>

            <div class="message-template-section row">
                <asp:TextBox CssClass="message-text-input sparkron-input-maintext" runat="server"
                    ID="smsContent" BorderStyle="None" TextMode="MultiLine" autocomplete="off"
                    Text="<%# this.GetText %>"></asp:TextBox>
            </div>
            <asp:Button ID="Button1" Text="Send" runat="server" OnClick="sendMailBtn_Click" CssClass="sparkron-submit-btn" CausesValidation="false"
                OnClientClick="this.disabled=true;" UseSubmitBehavior="false" />
        </div>
    </div>
    <!--SMS Message Page Body END-->

</asp:Content>
