# Programsko inženjerstvo

# Opis projekta
Ovaj projekt je reultat timskog rada u sklopu projeknog zadatka kolegija [Programsko inženjerstvo](https://www.fer.unizg.hr/predmet/proinz) na Fakultetu elektrotehnike i računarstva Sveučilišta u Zagrebu.

Kao projektni zadatak izvodit će se aplikacija za pomoć pri učenju u obliku AI asistenta. Koristeći materijale tekstualnih oblika u PDF formatu, AI asistent bi kreirao brojan sadržaj s kojim bi korisnik mogao bolje shvatiti određenu temu uz osjećaj personaliziranog podučavanja. Uz studente aplikaciju bi koristili i edukatori koji bi imali mogućnost objavljivanja svog sadržaja, resursa i materijala, također u PDF formatu, koje može koristiti student ili ih AI asistent može koristiti kao izvor podataka. Nad edukatorima nadgledaju i recenzenti (engl. Reviewers) čija je uloga provjeravati točnost sadržaja koji su dodali edukatori i po potrebi ih izbrisati. Administratori imaju mogućnosti dodavanje oznaka u sustav koje se koriste za označavanje PDF dokumenata (engl. Tags) te dodijeljivanje uloga korisniku i brisanje korisnika iz sustava. 

Studentu aplikacija služi za spremište i upravljanje svojim materijalima. Kada će student htjeti interakciju s AI asistentom, generirali bi se resursi u obliku raznih flash kartica što bi studentu pomoglo pri savladavanju određenih tema. Kao izvore podataka bi se koristili upravo materijali koji su spremljeni na aplikaciju, tako da se svakome studentu može kreirati točno taj sadržaj koji njemu treba.

AI asistent je izveden pomoću Google Gemini 1.5 Flash modela. Model će primati dokumente (PDF format), te na temelju njih stvoriti sadržaj, zadatke i ostale resurse koji bi pomogli studentu. Student sam bira dokumente iz kojih želi generirati sadržaj.

Uz studenta, aplikacijom se mogu koristiti i edukatori koji bi imali mogućnost kreiranja i dodavanja svojih materijala za koje oni smatraju da bi mogli pomoći studentu, te student može spremiti dodatne materijale što bi omogućilo AI asistentu da se služi s tim materijalima pri generiranju sadržaja.

Materijale edukatora kontroliraju recenzenti. Recenzenti mogu izbrisati javni materijal koji smatraju da ne zadovoljava zahtjeve kvalitete. Pri brisanju recenzenti ostavljaju povratnu poruku kako bi edukator koji je vlasnik materijala bio upućen u razloge brisanja.

Administratori nadgledaju rad korisnika u sustavu i dodavaju oznake za PDF dokumente. Administrator je zadužen za dodavanje uloge korisniku te za brisanje korisnika ako smatra da njihovo ponašanje ugrožava sustav.

Svi korisnici aplikacije se moraju prijaviti i potvrditi svoj identitet, te je u tu svrhu korištena OAuth2 autentifikacija. Koristit će se principi responzivnog dizajna za dinamičku prilagodbu korisničkog sučelja različitim veličinama i razlučivostima zaslona.

Aplikacija također sadrži kalendar preko kojega korisnici mogu upravljati svojim rasporedom sati. Mogu staviti svoj predefinirani raspored iz .csv datoteke ili izraditi svoj klikanjem na željene datume.

Svim korisnicima koji imaju neku od uloga sustava (Administrator, Edukator, Recenzent, Student) dozvoljeno je upravljati personaliziranim kalendarom.

Studenti mogu ostaviti recenzije o materijalima. Te recenzije studenti i ostali korisnici mogu koristiti u svrhu procijenjivanja vjerodostojnosti materijala.

# Cilj i motivacija? 
Ideja iza projekta je napraviti aplikaciju koja može pomoći prosječnome studentu/učeniku uz pomoć AI asistenta raditi prilagođene materijale za učenje te učiniti učenje pristupačnijim,
također s dodatkom edukatora u sistem ova aplikacija može se lagano prilagoditi na rad u učionici gdje edukator radi i bira koje materijale učenici/studenti trebaju koristiti za učenje za ispit.

# Funkcijski zahtjevi
| ID zahtjeva | Opis | Prioritet | Izvor | Kriteriji prihvaćanja |
|---|---|---|---|---|
| F-001 | Sustav omogućuje korisnicima registraciju putem e-mail adrese ili OAuth2 autentifikacije. | Visok | Zahtjev dionika | Korisnik može kreirati račun putem e-maila, primiti potvrdu i uspješno se prijaviti. |
| F-002 | Sustav omogućuje studentima dodavanje vlastitih materijala (PDF) u osobni profil. | Visok | Zahtjev dionika | Student može uspješno prenijeti materijale i pregledati ih unutar aplikacije. |
| F-003 | Sustav omogućuje Studentima generiranje personaliziranih “flashcards”-a iz odabranih materijala putem AI asistenta. | Visok | Specifikacija projekta | Na temelju priloženih materijala, AI kreira prilagođeni sadržaj za korisnika. |
| F-004 | Edukatori mogu objavljivati dodatne obrazovne resurse dostupne studentima. | Srednji | Povratne informacije korisnika | Edukator može uspješno dodati resurse, a studenti ih mogu pregledati i koristiti. |
| F-005 | Revieweri mogu pregledati i brisati sadržaj objavljen od strane edukatora. | Visok | Zahtjev dionika | Reviewer može uspješno pregledati i potvrditi/odbiti dodane resurse. |
| F-006 | Aplikacija omogućuje svim korisnicima osim default usera pregled rasporeda ispita kroz kalendar. | Srednji | Specifikacija projekta | Svi osim default usera može dodati termine ispita u kalendar i pregledati ih kasnije. |
| F-007 | Sustav omogućuje studentima ocjenjivanje materijala. | Srednji | Specifikacija projekta | Student može ostaviti recenziju na materijal. |
| F-008 | Svi osim default usera mogu uploadati kalendar u obliku CSV datoteke. | Srednji | Zahtjev dionika | Svi osim default usera mogu uspješno prenijeti CSV datoteku, a sustav je prikazuje u osobnom kalendaru unutar aplikacije. |
| F-009 | Korisnici mogu brisati vlastiti sadržaj s aplikacije | Visok | Zahtjev dionika | Svi korisnici koji mogu staviti sadržaj na aplikaciju mogu ga i izbrisati. |
| F-010 | Administrator može odobravati nove edukatore i rewievere na sustavu. | Visok | Zahtjev dionika | Administrator može pregledati i odobriti/odbiti zahtjeve edukatora za pridruživanje sustavu. |
| F-011 | Administrator može izbrisati korisnike sa sustava | Srednji | Zahtjev dionika | Administrator briše korisnika i sve što je korisnik stavio na sustav. |
| F-012 | Pregledavanje i dodavanje tag-ova od strane administratora. | Srednji | Zahtjev dionika | Administrator treba moći kreirati i brisati tag-ove. |
| F-013 | Default user se može prijaviti na role | Visoki | Zahtjev dionika | Default user može prijaviti za Studenta ili poslati prijavu za role Edukatora i Reviewera. |
| F-014 | Student može pregledati javne i svoje osobne materijale | Visoki | Zahtjev dionika | Student može pregledati javne i svoje osobne materijale tako da ih direktno otvori u browseru ili preuzme na vlastiti uređaj, ovisno o uređaju. |
| F-015 | Edukator može pregledavati javno dostupne materijale. | Visoki | Zahtjev dionika | Edukator može pregledavati javno dostupne materijale tako da ih direktno otvori u browseru ili preuzme na vlastiti uređaj, ovisno o uređaju. |
| F-016 | Recenzent može pregledavati javno dostupne materijale. | Visoki | Zahtjev dionika | Recezent može pregledavati javno dostupne materijale tako da ih direktno otvori u browseru ili preuzme na vlastiti uređaj, ovisno o uređaju. |
| F-017 | Administrator može pregledavati javno dostupne materijale. | Visoki | Zahtjev dionika | Administrator može pregledavati javno dostupne materijale tako da ih direktno otvori u browseru ili preuzme na vlastiti uređaj, ovisno o uređaju. |



# Tehnologije

Frontend - Razor, Bootstrap

Backend - ASP .NET

Database - SQLite

Project Management - JIRA + Confluence + Github

# Članovi tima 

Filip Belina - Project Lead

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
