<%@ Page Title="Om Siden" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="About.aspx.cs" Inherits="MessageCenter.About" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <h2><%: Title %></h2>
    <p>
        Denne Web Applikation er udviklet som en del af min (Andreas Kirkegaard Jensen) hovedopgave, på erhvervsakademiet Dania's datamatiker linje i Grenaa, og er udviklet i samarbejde med Sparekassen Kronjyllands "Processer og Effektivisering" afdeling
        <br />
        <br />
        Applikationen er en "besked central", hvor medarbejdere kan sende beskeder ud til deres kunder, hvor besked indholdet er formuleret på forhånd, og applikationen automatisk indsætter passende informationer (som f.eks navn, alder og telefon numre).
        Appen kan også rediger denne data ind i Word Dokumenter, og automatisk sende dem som PDF filer (ikke understøttet i Azure versionen).
    </p>

    <h3>Demo video:</h3>
    <iframe width="560" height="315" src="https://www.youtube.com/embed/Xn9592Vx4uU" frameborder="0" allow="accelerometer; autoplay; encrypted-media; gyroscope; picture-in-picture" allowfullscreen></iframe>
    <br />
    <br />
    <!--Collapsable panels-->
    <div class="panel-group">
        <div class="panel panel-default">
            <div class="panel-heading">
                <h4 class="panel-title">
                    <a data-toggle="collapse" data-parent="" href="#collapse1">Sådan bruges siden - beskrivelse</a>

                </h4>
            </div>
            <div id="collapse1" class="panel-collapse collapse in">

                <div class="panel-body">

                    <p>
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
                        <br />
                        Adminstratorbrugere vil yderligt have mulighed for at tilføje nye beskedskabeloner, ved at klikke på den grå knap under listen af eksisterende skabeloner (vises kun hvis du er logget ind som adminstrator)
        De kan også fjerne valgte beskedskabeloner ved at trykke "Slet Besked".
                    </p>
                </div>

            </div>
        </div>
        <div class="panel panel-default">
            <div class="panel-heading">
                <h4 class="panel-title">
                    <a data-toggle="collapse" href="#collapse2">Sådan bruges siden - punktform</a>
                </h4>
            </div>
            <div id="collapse2" class="panel-collapse collapse">
                <div class="panel-body">
                    <ul>
                        <li>
                            <h4>Klik "Log ind" i navigationsbaren, og indtast en test brugers login</h4>
                            <ul>
                                <li>F.eks ("T210672" + "andr4376") for en adminstrator bruger</li>
                                <li>F.eks ("T196543" + "andr4376") for en normal bruger</li>
                                <li>Adminstrator brugere kan slette og tilføje beskedskabeloner!</li>
                                <li>Se "Andreas' hovedopgave info" sektionen for flere</li>
                            </ul>
                        </li>

                        <li>
                            <h4>Naviger til forsiden via "Hjem" knappen og marker en besked skabelon</h4>
                            <ul>
                                <li>Du kan bekræfte dit valg ved at trykke "Fortsæt", dobbeltklikke eller trykke på "Enter" tasten</li>
                            </ul>
                        </li>
                        <li>
                            <h4>Vælg en kunde i pop-up vinduet</h4>
                            <ul>
                                <li>Dette er en liste af kunder tilknyttet din login bruger</li>
                                <li>Du kan søge efter navn eller cpr nummer</li>
                            </ul>
                        </li>

                        <li>
                            <h4>Den valgte besked skabelons indhold bliver udfyldt med dit bruger data og kundens data</h4>
                            <ul>
                                <li>Applikationen indsætter automatisk værdier som navne og emails ect.</li>
                                <li>Besked skabeloner er enten mails eller sms'er - de er forskellige (mails har fx. vedhæftede filer)</li>
                                <li>Det samme gør den for vedhæftede Word Dokumenter! (dog ikke i versionen der er hostet på Azure)</li>
                                <li>Du kan ændre beskedens indhold og adresse og evt. tilføje eller fjerne vedhæftede filer</li>

                            </ul>
                        </li>

                        <li>
                            <h4>Klik send når du er tilfreds med indholdet</h4>
                            <ul>
                                <li>Word Dokumenter bliver automatisk afsendt som PDF filer (ikke i Azure versionen)</li>
                                <li>SMS'er er pt. ikke understøttet i denne prototype</li>
                            </ul>
                        </li>
                    </ul>

                </div>


            </div>
        </div>
        <div class="panel panel-default">
            <div class="panel-heading">
                <h4 class="panel-title">
                    <a data-toggle="collapse" href="#collapse3">Andreas' hovedopgave info!</a>
                </h4>
            </div>
            <div id="collapse3" class="panel-collapse collapse">
                <div class="panel-body">

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
                    <p>BEMÆRK! Siden kan pt. ikke sende Sms'er, da sådan en service ville koste penge at tilføje, og servicen vil alligevel skulle fjernes hvis applikationen skal fortsætte sin udvikling. </p>

                    <br />
                    Du kan prøve at tilføje eller fjerne beskeder på forsiden, men husk at logge ind som adminstrator først! Beskeder med titlen "DEMO" kan ikke fjernes, da de skal bevares i sammenhæng med min eksamen.
    <br />
                    <br />
                    <p>Hvis du af en eller anden årsag ikke kan logge ind, eller ikke ser listen med kunder når du vælger en beskedskabelon, er det fordi at siden ikke kan få forbindelse til den API appen henter sit "bank data" fra :(</p>
                    <br />

                </div>
            </div>
        </div>
    </div>
    <!--Collapsable panels END-->
</asp:Content>
