<%@ Page Title="Contact" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Contact.aspx.cs" Inherits="MessageCenter.Contact" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">


    <div class="container">
        <div class="row">

            <div class="col-md-4 message-template-section" style="max-width:400px; padding:25px; margin-top:50px;">
                <div style="padding-left: 50px;">
                    <asp:Image ID="logo" runat="server" ImageUrl="~/Images/andreas.jpg" CssClass="profile-img" />

                    <h2>Andreas Kirkegaard Jensen</h2>
                    <h3>Udvikler / IT-support</h3>
                    <address>
                        Datamatikerstuderende<br />
                        Erhvervsakademi Dania, Grenaa<br />
                    </address>

                    <address>
                        <strong>Mail:</strong>   <a href="mailto:andr4376@gmail.com">andr4376@gmail.com</a><br />
                        <strong>Telefon:</strong> 40 96 50 01
                    </address>
                </div>
            </div>


            <div class="col-md-4 message-template-section" style="max-width:400px; margin-top:50px; padding:25px;">
                <div style="padding-left: 50px;">
                    <asp:Image ID="Image1" runat="server" ImageUrl="~/Images/shh.jpg" CssClass="profile-img" />

                    <h2>Søren Høholt</h2>
                    <h3>Forretningsansvarlig</h3>
                    <address>
                        Afdelingsdirektør 
                        <br />
                        Processer & Effektivisering<br />
                    </address>

                    <address>
                        <strong>Mail:</strong>   <a href="mailto:test@sparkron.dk">testmail@sparkron.dk</a><br />
                        <strong>Telefon:</strong> xx xx xx xx
                    </address>
                </div>
            </div>

           
        </div>
    </div>

</asp:Content>
