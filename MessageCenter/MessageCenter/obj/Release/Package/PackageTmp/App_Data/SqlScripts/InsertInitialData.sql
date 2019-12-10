
insert into MessageTemplates VALUES (1,'DEMO - En Mail skabelon som demonstrer data indsætning og word redigering.',
'Dette er en besked skabelon af typen Email!
Denne mail sendes fra den email adresse som tilhører den bruger som er logget ind: [employeeEmail]
Brugeren der er logget ind, hedder [employeeFirstName] til fornavn, og [employeeLastName] til efternavn, og deres
telefon nummer er [employeePhoneNumber].

Denne mail bliver som udgangspunkt sendt til den valgte kundes primære mail ([customerEmail]), men du evt. selv ændre den til din egen mail (eller tilføje den i CC).
Kundens navn er [customerFullName], og personen er [customerAge] år gammel (født den [customerBirthDay]).
Disse informationer blev hentet ud fra kundens cpr nummer [customerCpr].

Både brugeren ([employeeFullName]) som er logget ind og kunden ([customerFullName]) har afdeling i [department].
Vi har udfyldt det vedhæftede Word Dokument med data, og når det bliver sendt eksporteres det som PDF-fil.
Yderligt er der også vedhæftet et dejligt billede.

Du kan tilføje eller slette beskedskabeloner fra forsiden, men kun hvis du er logget in som en adminstrator bruger.
Læs "Om Siden" for mere info! :)

Mvh,
MessageCenter'
,0);


insert into MessageTemplates VALUES (2,'DEMO - En SMS skabelon som demonstrer at MessageCenter kan håndtere flere besked typer',
'Dette er en besked skabelon af typen SMS!

BEMÆRK! MessageCenter kan PT. ikke afsende SMSer, da den stakkels udvikler ikke har råd til sådanne services...

Denne sms sendes fra det telefon nummeret som tilhører den bruger som er logget ind: [employeePhoneNumber]
Brugeren der er logget ind, hedder [employeeFirstName] til fornavn, og [employeeLastName] til efternavn.

Denne SMS bliver som udgangspunkt sendt til den valgte kundes mobil nr. ([customerPhoneNumber]).
Kundens navn er [customerFullName], og personen er [customerAge] år gammel (født den [customerBirthDay]).
Disse informationer blev hentet ud fra kundens cpr nummer [customerCpr].

Du kan tilføje eller slette beskedskabeloner fra forsiden, men kun hvis du er logget in som en adminstrator bruger.
Læs "Om Siden" for mere info! :)

Mvh,
MessageCenter'
,1);