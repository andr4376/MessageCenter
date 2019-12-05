<%@ Page Title="Om Siden" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="About.aspx.cs" Inherits="MessageCenter.About" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%: Title %>.</h2>

    <h3>Sådan bruger du siden:</h3>
    <p>
        Denne side har til formål at gøre det nemmere for kunderådgivere at sende beskeder ud til deres kunder.<br />
        For at benytte siden bør du først logge ind, ved brug af "Login" knappen i menu baren. Du kan derefter vælge en beskedskabelon på forsiden for at navigere<br />
        til siden for den specefikke besked. OBS! En besked kan have forskellige typer, som F.eks. Email eller Sms - brugergrænsefladen er forskellig for hver type.<br />
        <br />
        Når du har navigeret til siden for den bestemte skabelon vil du stødde på et vindue hvor du skal vælge modtageren af beskeden - dette er en liste over alle dine kunder. Vælg din ønskede modtager, og siden vil derefter
        automatisk udfylde beskedskabelonen med information som er specefikt for dig og din kunde - Siden vil automatisk indtaste kundens mail / telefonnr som modtager, og din som afsender.
        <br />
        Siden vil også indsætte kundens navn og information i skabelonens tekst indhold, hvor det er passende, og udfylder endda også denne information i vedhæftede Word dokumenter. OBS! når en mail sendes, bliver Word Dokumenter automatisk afsendt som PDF filer.<br />
        <br />
        Hvis du har valgt en Mail, har du mulighed for at fjerne og tilføje vedhæftede filer, og du kan også downloade filer for at se præcist hvad det er du sender. Hvis du evt. gerne vil tilføje noget tekst eller omskrive noget 
        i et word dokument, kan du downloade filen, fjern filen fra "vedhæftede filer", ændre i indholdet, og uploade det igen. 
        <br />
        Adminstratorbrugere vil yderligt have mulighed for at tilføje nye beskedskabeloner, ved at klikke på den grå knap under listen af eksisterende skabeloner (vises kun hvis du er logget ind som adminstrator) 
    </p>
    <br />
    <br />

    <h3>Andreas' hovedopgave info:</h3>

    <p>
        Siden kører ikke på rigtig bankdata - så hvis du gerne vil prøve sidens funktionalitet kan du prøve at logge ind med en af de testbrugere som er i den nedenstående tabel
        Husk at kun en adminstratorbruger kan tilføje nye beskedskabeloner.
    <br />
    </p>
    <table style="width: 100%">
        <tr>
            <th>TUser</th>
            <th>Password</th>
            <th>Navn</th>
            <th>Amin</th>
        </tr>
        <tr>
            <td>T210672</td>
            <td>andr4376</td>
            <th>Knud Andersen</th>
            <th>JA</th>

        </tr>
        <tr>
            <td>T200454</td>
            <td>andr4376</td>
            <th>Svend-Erik Hammershoej</th>
            <th>JA</th>
        </tr>
        <tr>
            <td>T196543</td>
            <td>andr4376</td>
            <th>Bjarke Hansemann</th>
            <th>NEJ</th>
        </tr>
        <tr>
            <td>T210345</td>
            <td>andr4376</td>
            <th>Bo Risom Bitzer</th>
            <th>NEJ</th>
        </tr>
    </table>

    <br />
    <br />
    <p>
        Hvis du gerne vil sende en besked til dig selv, kan du ændre modtager emailen på beskedsiden, eller tilføje din email adresse med komma separering. Du kan evt. også taste den ind i "CC" feltet
    </p>
    <br />
    <p>BEMÆRK! Siden kan pt. ikke sende Sms'er</p>

    <br />
    <br />
    <p>Hvis du af en eller anden årsag ikke kan logge ind, eller ikke ser listen med kunder når du vælger en beskedskabelon, er det fordi at siden ikke kan få forbindelse til den API appen henter sit "bank data" fra :(</p>
    <br />
    <p></p>


</asp:Content>
