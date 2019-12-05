<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="NewMessage.aspx.cs" Inherits="MessageCenter.NewMessage" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <!--Pick messagetype modal-->
    <div class="modal fade" id="pickMessageTypeModal" tabindex="-1" role="dialog" aria-labelledby="userModal" aria-hidden="true"
        data-toggle="modal" data-backdrop="static" data-keyboard="false">
        <div class="modal-dialog" role="document">

            <div class="modal-content">

                <div class="modal-header">
                    <h2 class="modal-title">Vælg beskedtype</h2>

                </div>
                <div class="modal-body">

                    <asp:DropDownList ID="selectMsgTypeDropDownList" runat="server" CssClass="sparkron-dropdown">
                        <asp:ListItem Text="Mail" Value="0" />
                        <asp:ListItem Text="Sms" Value="1" />
                    </asp:DropDownList>

                </div>
                <div class="modal-footer">
                    <asp:Button ID="selectMsgTypeBtn" Text="Godkend" runat="server" OnClick="selectMsgTypeBtn_Click"
                        CssClass="sparkron-submit-btn" CausesValidation="false" />
                </div>
            </div>
        </div>
    </div>
    <script type="text/javascript">
        function openPickMessageTypeModal() {
            $('#pickMessageTypeModal').modal({ show: true });
        }
        function closePickMessageTypeModal() {
            $('#pickMessageTypeModal').modal('hide');
        }
    </script>
    <!--Pick messagetype modal END-->
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
                    <asp:Button ID="UploadFileBtn" Text="Upload Fil" runat="server" OnClick="UploadFileBtn_Click" CssClass="sparkron-submit-btn" CausesValidation="false" />
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

    <!-- newMessage Page Body-->
    <div runat="server" id="messageTemplateBody" class="sparkron-message-body" visible="false">


        <div class="container">

            <div class="message-template-section row">
                <asp:TextBox CssClass="sparkron-input-title message-text-input" runat="server"
                    ID="titleTextBox" BorderStyle="None" TextMode="SingleLine" placeholder="Beskedens Titel - bør ikke indeholde beskedvariabler" autocomplete="off"></asp:TextBox>
            </div>

            <div class="message-template-section row">
                <asp:TextBox CssClass="message-text-input sparkron-input-maintext" runat="server"
                    ID="messageTextTextBox" BorderStyle="None" TextMode="MultiLine" autocomplete="off"
                     placeholder="Beskedindhold - benyt evt. variabler fra den nedenstående tabel. Så kan applikationen automatisk indsætte den tilsvarende information, når beskeden skal afsendes."
                    ></asp:TextBox>
            </div>

            <!-- Attachment section -->
            <div id="AttachmentsSection" runat="server" visible="false">
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

            <asp:Button ID="CreateMessageBtn" Text="Gem beskedskabelon" runat="server" OnClick="CreateMessageBtn_Click" CssClass="sparkron-submit-btn" CausesValidation="false"
                OnClientClick="this.disabled=true;" UseSubmitBehavior="false" />
        </div>
    </div>

    <!--A table listing and descibing the message variables-->
    <div class="container">
        <div class="sparkron-message-body">
            <div class="message-template-section row">
                <asp:Table ID="variablesTable" runat="server" CssClass="message-template-section">
                    <asp:TableRow>
                        <asp:TableCell><h2>Variabel</h2></asp:TableCell>
                        <asp:TableCell><h2>Beskrivelse</h2></asp:TableCell>
                    </asp:TableRow>
                </asp:Table>
            </div>
        </div>
    </div>

    <!--newMessage Page Body END-->



</asp:Content>

