# Programsko inženjerstvo

# Opis projekta
Ovaj projekt je reultat timskog rada u sklopu projeknog zadatka kolegija [Programsko inženjerstvo](https://www.fer.unizg.hr/predmet/proinz) na Fakultetu elektrotehnike i računarstva Sveučilišta u Zagrebu.

Kao projektni zadatak izvodit će se aplikacija za pomoć pri učenju u obliku AI asistenta. Koristeći materijale tekstualnih oblika poput PDF, AI asistent bi kreirao brojan sadržaj s kojim bi se korisnik mogao bolje shvatiti određenu temu uz osjećaj personaliziranog podučavanja. Uz studente aplikaciju bi koristili i edukatori koji bi imali mogućnost objavljivanja svog sadržaja, resursa i materijala koje može koristiti student ili ih AI asistent može koristiti kao izvor podataka. Nad edukatorima nadgledaju i administratori čija je uloga provjeravati točnost sadržaja koji su dodali edukatori. 

Studentu aplikacija služi za spremište i upravljanje svojim materijalima. Kada će student htjeti interakciju s AI asistentom, generirali bi se resursi u obliku raznih zadataka, „flashcards“-ova, te ostalih oblika učenja koje bi studentu pomogle pri savladavanju određenih tema. Kao izvore podataka bi se koristili upravo materijali koji su spremljeni na aplikaciju, tako da se svakome studentu može kreirati točno taj sadržaj koji njemu treba.

AI asistent je izveden pomoću Google Gemini 1.5 Flash modela. Model će primati dokumente (poput PDF-a), te na temelju njih stvoriti sadržaj, zadatke i ostale resurse koji bi pomogli studentu. Student sam bira dokumente iz kojih želi generirati sadržaj.

Uz studenta, aplikacijom se mogu koristiti i edukatori koji bi imali mogućnost kreiranja i dodavanja svojih materijala za koje oni smatraju da bi mogli pomoći studentu, te se student može pretplatiti na dodatne materijale što bi omogućilo AI asistentu da se služi s tim materijalima pri generiranju sadržaja.

Edukatore nadzire administrator koji se bavi pregledom sadržaja i rada edukatora, te se bavi prijavama, poput neispravnog sadržaja ili sličnog. Oni bi odobravali edukatore, resurse i bavili time da je sadržaj aplikacije i rad edukatora u skladu sa pravilima ponašanja.

Svi korisnici aplikacije se moraju prijaviti i potvrditi svoj identitet, te je u tu svrhu korištena OAuth2 autentifikacija. Responzivnost će biti ostvarena pomoću modernih alata poput JavaScript-a i Bootstrap CSS-a. U tu svrhu, koristit će se principi responzivnog dizajna za dinamičku prilagodbu korisničkog sučelja različitim veličinama i razlučivostima zaslona.

Aplikacija također sadrži kalendar preko kojega studenti mogu upravljati svoj raspored sati. Mogu staviti svoj predefinirani raspored iz .csv datoteke ili izraditi svoj klikanjem na željene datume.

Studenti mogu ostaviti recenzije o materijalima, na bazi tih recenzija se odabiru najprimjereniji materijali za učenje.

# Cilj i motivacija? 
Ideja iza projekta je napraviti aplikaciju koja može pomoći prosječnome studentu/učeniku uz pomoć AI asistenta raditi prilagođene materijale za učenje te učiniti učenje pristupačnijim,
također s dodatkom edukatora u sistem ova aplikacija može se lagano prilagoditi na rad u učionici gdje edukator radi i bira koje materijale učenici/studenti trebaju koristiti za učenje za ispit.

# Funkcijski zahtjevi
| ID zahtjeva | Opis | Prioritet | Izvor | Kriteriji prihvaćanja |
|-------------|------|-----------|-------|------------------------|
| F-001 | Sustav omogućuje korisnicima registraciju putem e-mail adrese ili OAuth2 autentifikacije. | Visok | Zahtjev dionika | Korisnik može kreirati račun putem e-maila, primiti potvrdu i uspješno se prijaviti. |
| F-002 | Sustav omogućuje studentima dodavanje vlastitih materijala (PDF, tekstualne datoteke) u osobni profil. | Visok | Zahtjev dionika | Student može uspješno prenijeti materijale i pregledati ih unutar aplikacije. |
| F-003 | Sustav omogućuje generiranje personaliziranih zadataka i “flashcards”-a iz odabranih materijala putem AI asistenta. | Visok | Specifikacija projekta | Na temelju priloženih materijala, AI kreira prilagođeni sadržaj za korisnika. |
| F-004 | Edukatori mogu objavljivati dodatne obrazovne resurse dostupne studentima. | Srednji | Povratne informacije korisnika | Edukator može uspješno dodati resurse, a studenti ih mogu pregledati i koristiti. |
| F-005 | Revieweri mogu pregledati i brisati sadržaj objavljen od strane edukatora. | Visok | Zahtjev dionika | Reviewer može uspješno pregledati i potvrditi/odbiti dodane resurse. |
| F-006 | Aplikacija omogućuje studentima pregled rasporeda ispita kroz kalendar. | Srednji | Specifikacija projekta | Student može dodati termine ispita u kalendar i pregledati ih kasnije. |
| F-007 | Sustav omogućuje korisnicima ocjenjivanje materijala. | Srednji | Specifikacija projekta | Korisnik može ostaviti recenziju na materijal, a najbolji materijali se prikazuju kao preporučeni. |
| F-008 | Student može uploadati kalendar u obliku CSV datoteke. | Srednji | Zahtjev dionika | Student može uspješno prenijeti CSV datoteku, a sustav je prikazuje u osobnom kalendaru unutar aplikacije. |
| F-009 | Korisnici mogu brisati vlastiti sadržaj s aplikacije. | Visok | Zahtjev dionika | Korisnici mogu uspješno ukloniti svoj preneseni sadržaj iz aplikacije. |
| F-010 | Administrator može odobravati nove edukatore na sustavu. | Visok | Zahtjev dionika | Administrator može pregledati i odobriti/odbiti zahtjeve edukatora za pridruživanje sustavu. |
| F-011 | Reviewer i administrator nadgledaju i brišu nepremjerene i neispravne recenzije. | Nizak | Zahtjev dionika | Reviewer i administrator mogu brisati recenzije. |



# Tehnologije

Frontend - jQuery, Bootstrap

Backend - ASP .NET

Database - SQLite

Project Management - JIRA + Confluence

# Članovi tima 

Filip Belina - Prject Lead

Jakov Lovaković - Backend Team Lead

Jan Lalić - Backend Engineer

Leo Marušić - DevOps Engineer

Mislav Marinović - Lead design

Martin Šainčević - Frontend Team Lead

Jan Badel - Frontend Engineer



# Kontribucije

Kontribucije su vidljive i opisane u Confluence dokumentaciji.



# 📝 Kodeks ponašanja [![Contributor Covenant](https://img.shields.io/badge/Contributor%20Covenant-2.1-4baaaa.svg)](CODE_OF_CONDUCT.md)
Kao studenti sigurno ste upoznati s minimumom prihvatljivog ponašanja definiran u [KODEKS PONAŠANJA STUDENATA FAKULTETA ELEKTROTEHNIKE I RAČUNARSTVA SVEUČILIŠTA U ZAGREBU](https://www.fer.hr/_download/repository/Kodeks_ponasanja_studenata_FER-a_procisceni_tekst_2016%5B1%5D.pdf), te dodatnim naputcima za timski rad na predmetu [Programsko inženjerstvo](https://wwww.fer.hr).
Očekujemo da ćete poštovati [etički kodeks IEEE-a](https://www.ieee.org/about/corporate/governance/p7-8.html) koji ima važnu obrazovnu funkciju sa svrhom postavljanja najviših standarda integriteta, odgovornog ponašanja i etičkog ponašanja u profesionalnim aktivnosti. Time profesionalna zajednica programskih inženjera definira opća načela koja definiranju  moralni karakter, donošenje važnih poslovnih odluka i uspostavljanje jasnih moralnih očekivanja za sve pripadnike zajenice.

Kodeks ponašanja skup je provedivih pravila koja služe za jasnu komunikaciju očekivanja i zahtjeva za rad zajednice/tima. Njime se jasno definiraju obaveze, prava, neprihvatljiva ponašanja te  odgovarajuće posljedice (za razliku od etičkog kodeksa). U ovom repozitoriju dan je jedan od široko prihvačenih kodeks ponašanja za rad u zajednici otvorenog koda.

Pri početku projekta podijeljene su uloge u timu te opisana očekivanja od svake uloge, tokom rađenja projekta svako se držao dodijeljene uloge.

# 📝 Licenca
Važeča (1)
[![CC BY-NC-SA 4.0][cc-by-nc-sa-shield]][cc-by-nc-sa]

Ovaj repozitorij sadrži otvoreni obrazovni sadržaji (eng. Open Educational Resources)  i licenciran je prema pravilima Creative Commons licencije koja omogućava da preuzmete djelo, podijelite ga s drugima uz 
uvjet da navođenja autora, ne upotrebljavate ga u komercijalne svrhe te dijelite pod istim uvjetima [Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International License HR][cc-by-nc-sa].
>
> ### Napomena:
>
> Svi paketi distribuiraju se pod vlastitim licencama.
> Svi upotrijebleni materijali  (slike, modeli, animacije, ...) distribuiraju se pod vlastitim licencama.

[![CC BY-NC-SA 4.0][cc-by-nc-sa-image]][cc-by-nc-sa]

[cc-by-nc-sa]: https://creativecommons.org/licenses/by-nc/4.0/deed.hr 
[cc-by-nc-sa-image]: https://licensebuttons.net/l/by-nc-sa/4.0/88x31.png
[cc-by-nc-sa-shield]: https://img.shields.io/badge/License-CC%20BY--NC--SA%204.0-lightgrey.svg

Orginal [![cc0-1.0][cc0-1.0-shield]][cc0-1.0]
>
>COPYING: All the content within this repository is dedicated to the public domain under the CC0 1.0 Universal (CC0 1.0) Public Domain Dedication.
>
[![CC0-1.0][cc0-1.0-image]][cc0-1.0]

[cc0-1.0]: https://creativecommons.org/licenses/by/1.0/deed.en
[cc0-1.0-image]: https://licensebuttons.net/l/by/1.0/88x31.png
[cc0-1.0-shield]: https://img.shields.io/badge/License-CC0--1.0-lightgrey.svg

### Reference na licenciranje repozitorija
