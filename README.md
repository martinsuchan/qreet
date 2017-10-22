QR EET
======

QR EET popisuje standard pro snadné zpracování údajů na účtenkách registrovaných v systému EET.

Motivace
-------

Počátkem října 2017 byla spuštěna [Účtenkovka](https://www.uctenkovka.cz/), loterie, kde jako losy slouží účtenky zaregistrované v [systému EET](http://www.etrzby.cz/). Pro zaregistrování účtenky je ovšem třeba opsat velké množství údajů do systému, což není zrovna jednoduché. Ač existuje i mobilní aplikace Účtenkovka s funkcí OCR (rozpoznání textu v obrázku), z vlastní zkušenosti tato funkce [skoro nikdy nefunguje](https://www.zive.cz/bleskovky/jak-registrovat-uctenku-do-eet-loterie-uctenkovka/sc-4-a-189792/default.aspx).  
Tento problém si dal za cíl vyřešit projekt QR EET: navrhnout jednoduchý a úsporný datový formát EET účtenky pro snadné ověření či registraci do Účtenkovky, například pomocí jednoho malého QR kódu.

Popis formátu QR EET verze 1.0
-------

Při návrhu QR EET bylo cílem nevynalézat znova kolo, ale inspirovat se tam, kde již podobný formát funguje.
**QR EET formát je tak identický s [formátem SPAYD](https://qr-platba.cz/pro-vyvojare/), který byl navržen pro potřeby [QR Platby](https://qr-platba.cz/).**

Formát řetězce je navržen tak, aby byl kompaktní, co se velikosti obsažených dat týče. Výhodou navrženého formátu je relativně dobrá lidská čitelnost a potenciální rozšiřitelnost o specifické atributy.
Řetězec může obsahovat libovolné znaky ze znakové sady ISO-8859-1 (znaková sada pro binární QR kód). Pro efektivní uložení do QR kódu doporučujeme sestavit řetězec tak, aby obsahoval pouze následující znaky:
```
0–9
A–Z [pouze velká písmena]
mezera
*, ., :
```
Při zachování znaků výhradně z uvedené množiny bude použit tzv. alfanumerický formát QR kódu. Množina znaků používaná v klíčích a řídících strukturách navrženého formátu je proto volena právě z této množiny tak, aby nebylo zabráněno dosažení maximální možné efektivity uložení informací o účtence do QR kódů. Bude-li v hodnotě kteréhokoli pole použit znak z jiné množiny, než je uvedena výše, bude použit tzv. binární formát QR kódu.

QR kód by měl být pro tištěná media generován s [úrovní kontroly chyb aspoň M](http://www.qrcode.com/en/about/error_correction.html) (obnovitelnost 15%).

Řetězec je vždy zahájen fixní hlavičkou EET*, **zde je rozdíl proti QR platbě, která používá hlavičku SPD**. Následuje verze protokolu (dvě čísla oddělená tečkou) ukončená hvězdičkou, např. 1.0*. Následně řetězec obsahuje jednotlivé atributy účtenky ve formátu:

    ${klíč}:${hodnota}*
Tedy klíč je od hodnoty oddělen dvojtečkou, hodnota je zakončena hvězdičkou.


**${klíč}**
Klíč daného atributu je vždy zapsán velkými znaky z množiny znaků [A-Z-]. Seznam přípustných klíčů (základní sada atributů) je uveden v Tabulce níže. Formát může být rozšířen o proprietární klíče, které mají např. lokální význam pro konkrétní lokalitu nebo instituci.

**${hodnota}**
Hodnota daného atributu může obsahovat libovolné znaky, ale musí být zároveň v přípustném formátu pro dané pole – viz. popis formátu hodnoty v Tabulce. Hodnota nesmí být obklopena bílými znaky (tj. za “:” a před “*” nesmí být bílé znaky) a nesmí obsahovat znak * (hvězdička). Hodnota může obsahovat znak : (dvojtečka).
Hodnota může obsahovat speciální znaky kódované pomocí URL kódování. Díky tomuto mechanismu je možné kódovat libovolné znaky z UTF-8, hvězdičku je tedy možno do hodnoty zahrnout pomocí zápisu %2A.

Pokud bude hodnota obsahovat více znaků, než připouští formát, bude zpracován pouze formátem specifikovaný počet znaků zleva, ostatní budou ignorovány.

**Tabulka s klíči a hodnotami formátu QR EET verze 1.0**

| Klíč     | Povinný | Formát | Hodnota | Příklad klíče a hodnoty |
| -------- | ------- | ------ | ------- | ------------------------------ |
| FIK      | ANO*    | 16 znaků z množiny [A-F0-9] | FIK kód, prvních 16 znaků (3 skupiny) bez mezer či pomlček | FIK:0D68FDDC306C9D48 |
| BKP      | ANO*    | 16 znaků z množiny [A-F0-9] | BKP kód, prvních 16 znaků (2 skupiny) bez mezer či pomlček | BKP:A6CF0448FC2C806C |
| DIC      | ANO     | 8-10 číslic            | DIČ, bez předpony "CZ" | DIC:00685976 |
| KC       | ANO     | 1-10 znaků z množiny [0-9.] | Cena na účtence. Desetinné číslo, max. 2 desetinné cifry, Tečka jako oddělovač desetinných míst. Maximální možná hodnota je 9 999 999.99 | KC:227.79 |
| DT       | ANO     | 12 číslic | Datum a čas tržby ve formátu YYYYMMDDhhmm, formát ISO 8601 | DT:201710131429 |
| R        | NE      | 1 symbol B nebo Z | Režim tržby [B]ěžný nebo [Z]jednodušený | R:B |

*U Běžného režimu tržby je pro úspěšné zpracování účtenky třeba poskytnout buď prvních 16 znaků z FIK kódu nebo prvních 16 znaků z BKP kódu. U Zjednodušeného režimu je třeba poskytnout prvních 16 znaků z BKP kódu.
Pokud není uveden klíč R pro režim tržby, předpokládá se Běžný režim tržby.


Příklad v praxi
---------------
Fotografie níže ukazuje výřez ze skutečné účtenky zaregistrované v sytému EET. Takto účtenku vyfotil telefon Nexus 5X a z tohoto snímku nedokázala aplikace Účtenkovka vyčíst žádný údaj.

![EET účtenka](https://i.imgur.com/sxgvWc5.jpg)

**Údaje vyplněné ručně pro Účtenkovku**

![QR EET kód](https://i.imgur.com/NF0WXQQ.png)

**QR EET řetězec odpovídající této účtence**
```
EET*1.0*BKP:DE7AB57EF9F1B523*DIC:45316872*KC:117*DT:201710101844
```
**QR EET kód odpovídající této účtence**

![QR EET kód](https://i.imgur.com/9xwEFbQ.png)


Vzorová implementace
--------------------
Tento Git repozitář obsahuje [vzorovou implementaci QR EET standardu v jazyce C#](https://github.com/martinsuchan/qreet/blob/master/Source/QREET.Lib/EetReceipt.cs), [základní unit testy](https://github.com/martinsuchan/qreet/blob/master/Source/QREET.Test/EetToQrTests.cs) a [demo aplikaci](https://github.com/martinsuchan/qreet/tree/master/Source/QREET.UWP) demonstrující převod EET údajů na QR EET formát a zpět.  
U vzorové implementace budu rád za jakékoliv připomínky a zejména implementace i pro jiné platformy a jazyky.


Demo aplikace
-------------
Pro účely otestování QR EET formátu jsem vytvořil jednoduchou aplikaci pro Windows 10, která je **dostupná ve Windows Store**:  
https://www.microsoft.com/store/apps/9pcwdgr58lm3  

Cílem aplikace je **demonstrovat snadný převod zadaných EET údajů na QR EET formát a reprezentaci tohto formátu pomocí QR kódu**.  
Aplikace také podporuje skenování QR kódů a dekódování nalezeného QR EET řetězce na vstupní EET hodnoty.  
Aplikace je dostupná pro Windows 10 osobní počítače, tablety a telefony. Je možné jí zbuildovat i ze zdrojových kódů [dostupných v tomto Git repozitáři](https://github.com/martinsuchan/qreet/tree/master/Source/QREET.UWP). Aplikace nesbírá žádné osobní údaje a neposílá žádná data na internet.

![QR EET Demo aplikace](https://i.imgur.com/CzBFFCY.png)



Otázky a odpovědi
---------------

**Kdo stojí za projektem QR EET?**  
Za projektem momentálně stojím jen já, [Martin Suchan](https://twitter.com/martinsuchan). QR EET jsem vytvořil ve volném čase jako osobní výzvu navrhnout systém, který by zjednodušil lidem Účtenkovku a obecně použití EET. Pokud mě chcete kontaktovat, můžete mi napsat na Twitteru nebo na email [jméno]@[příjmení].cz.

**K čemu je to dobré, když aplikace ani účtenky tyto QR kódy neobsahují?**  
Je to tak trochu začarovaný kruh. Aplikace Účtenkovka nepodporuje QR kódy, protože nejsou na účtenkách a účtenky je nepodporují, protože pro ně není žádné využití. Pro zavedení QR kódů je nezbytné, aby někdo udělal ten první krok a tím prvním krokem je v tomto případě jednoduchý a jasný standard, který lze snadno zapojit do aplikace nebo pokladny na EET.

**Jsem vývojář, jak mohu pomoci?**  
QR EET standard je otevřený, a budu jen rád, když se do rozvoje zapojí i další lidé třeba připomínkováním dokumentace nebo vytvořením knihovny pro generování/čtení QR EET kódů pro Javu, Swift, JavaScript, PHP, C++ a pod. Inspirací mohou být podobné [knihovny pro QR Platbu zde na GitHubu](https://github.com/spayd).

**Jaký je dlouhodobý cíl projektu QR EET?**  
Zapojení obchodníků do systému EET vedlo mimo jiné i k tomu, že účtenky často narostly o několik cm na délku jen proto, aby se na ně vešla řada nových povinných údajů ze systému EET. Přál bych si, aby do budoucna nebylo třeba na účtenky uvádět mnoho řádků nesrozumitelných údajů, ale třeba jen jeden malý QR kód, a účtenky se tak opět o něco zkrátily. Aby se toto ale stalo realitou, budou nutné minimálně nové metodické pokyny k EET a možná i novela zákona o Elektronické Evidenci Tržeb.
