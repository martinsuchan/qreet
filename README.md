QR EET
======

QR EET popisuje standard pro snadné zpracování údajů na účtenkách registrovaných v systému EET.

Motivace
-------

Počátkem října 2017 byla spuštěna [Účtenkovka](https://www.uctenkovka.cz/), loterie, kde jako losy slouží účtenky zaregistrované v [systému EET](http://www.etrzby.cz/). Pro zaregistrování účtenky je ovšem třeba opsat velké množství údajů do systému, což není zrovna jednoduché. Ač existuje i mobilní aplikace Účtenkovka s funkcí OCR – rozpoznání textu na obrázku, z vlastní zkušenosti tato funkce [skoro nikdy nefunguje](https://www.zive.cz/bleskovky/jak-registrovat-uctenku-do-eet-loterie-uctenkovka/sc-4-a-189792/default.aspx).  
Tento problém si dal za cíl vyřešit projekt QR EET, navrhnout jednoduchý a úsporný formát, pomocí kterého bude možné načíst všechny potřebné údaje Účtenkovku pomocí jednoho malého QR kódu.

Popis formátu QR EET
-------

Při návrhu QR EET bylo cílem nevynalézat znova kolo, ale inspirovat se tam, kde již podobný formát funguje.
**QR EET formát je tak identický s [formátem SPAYD](https://qr-platba.cz/pro-vyvojare/), který byl navržen pro [QR Platbu](https://qr-platba.cz/).**

Formát řetězce je navržen tak, aby byl kompaktní, co se velikosti obsažených dat týče. Výhodou navrženého formátu je relativně dobrá lidská čitelnost a potenciální rozšiřitelnost o specifické atributy.
Řetězec může obsahovat libovolné znaky ze znakové sady ISO-8859-1 (znaková sada pro binární QR kód). Pro efektivní uložení do QR kódu doporučujeme sestavit řetězec tak, aby obsahoval pouze následující znaky:
```
0–9
A–Z [pouze velká písmena]
mezera
*, ., :
```
Při zachování znaků výhradně z uvedené množiny bude použit tzv. alfanumerický formát QR kódu. Množina znaků používaná v klíčích a řídících strukturách navrženého formátu je proto volena právě z této množiny tak, aby nebylo zabráněno dosažení maximální možné efektivity uložení informací o účtence do QR kódů. Bude-li v hodnotě kteréhokoli pole použit znak z jiné množiny, než je uvedena výše, bude použit tzv. binární formát QR kódu.

QR kód by měl být pro tištěná media generován s úrovní kontroly chyb M (obnovitelnost 15%).

Řetězec je vždy zahájen fixní hlavičkou EET*, **zde je rozdíl proti QR platbě, která používá hlavičku SPD**. Následuje verze protokolu (dvě čísla oddělená tečkou) ukončená hvězdičkou, např. 1.0*. Následně řetězec obsahuje jednotlivé atributy účtenky ve formátu:

    ${klíč}:${hodnota}*
Tedy klíč je od hodnoty oddělen dvojtečkou, hodnota je zakončena hvězdičkou.


**${klíč}**
Klíč daného atributu je vždy zapsán velkými znaky z množiny znaků [A-Z-]. Seznam přípustných klíčů (základní sada atributů) je uveden v Tabulce níže. Formát může být rozšířen o proprietární klíče, které mají např. lokální význam pro konkrétní lokalitu nebo instituci.

**${hodnota}**
Hodnota daného atributu může obsahovat libovolné znaky, ale musí být zároveň v přípustném formátu pro dané pole – viz. popis formátu hodnoty v Tabulce. Hodnota nesmí být obklopena bílými znaky (tj. za “:” a před “*” nesmí být bílé znaky) a nesmí obsahovat znak * (hvězdička). Hodnota může obsahovat znak : (dvojtečka).
Hodnota může obsahovat speciální znaky kódované pomocí URL kódování. Díky tomuto mechanismu je možné kódovat libovolné znaky z UTF-8, hvězdičku je tedy možno do hodnoty zahrnout pomocí zápisu %2A.

Pokud bude hodnota obsahovat více znaků, než připouští formát, bude zpracován pouze formátem specifikovaný počet znaků zleva, ostatní budou ignorovány.

**Tabulka s klíči a hodnotami formátu QR EET**

| Klíč     | Povinný | Formát | Hodnota | Příklad klíče a hodnoty |
| -------- | ------- | ------ | ------- | ------------------------------ |
| FIK      | ANO*    | 16 znaků z množiny [A-F0-9] | FIK kód, prvních 16 znaků (3 skupiny) bez mezer či pomlček | FIK:0D68FDDC306C9D48 |
| BKP      | ANO*    | 16 znaků z množiny [A-F0-9] | BKP kód, prvních 16 znaků (2 skupiny) bez mezer či pomlček | BKP:A6CF0448FC2C806C |
| DIC      | ANO     | 8-10 číslic            | DIČ, bez předpony "CZ" | DIC:00685976 |
| KC       | ANO     | 1-10 znaků z množiny [0-9.] | Cena na účtence. Desetinné číslo, max. 2 desetinné cifry, Tečka jako oddělovač desetinných míst. Maximální hodnota odpovídá částce 9 999 999.99 | KC:227.79 |
| DT       | ANO     | 12 číslic | Datum a čas tržby ve formátu YYYYMMDDhhmm, formát ISO 8601 | DT:201710131429 |
| R        | NE      | 1 symbol B nebo Z | Režim tržby [B]ěžný nebo [Z]jednodušený | R:B |

*Pro úspěšné zpracování účtenky je třeba poskytnout buď prvních 16 znaků z FIK kódu nebo prvních 16 znaků z  BKP kódu.
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


Otázky a odpovědi
---------------

**Kdo stojí za projektem QR EET?**  
Za projektem momentálně stojím jen já, [Martin Suchan](https://twitter.com/martinsuchan). QR EET jsem vytvořil ve volném čase jako osobní výzvu navrhnout systém, který by zjednodušil lidem Účtenkovku. Pokud mě chcete kontaktovat, můžete mi napsat na Twitteru nebo na email [jméno]@[příjmení].cz.

**Jsem vývojář, jak mohu pomoci?**  
QR EET standard je otevřený, a budu jen rád, když se do rozvoje zapojí i další lidé třeba připomínkováním dokumentace nebo vytvořením knihovny pro generování/čtení QR EET kódů pro Javu, Swift, JavaScript, PHP, C++ a pod. Inspirací mohou být podobné [knihovny pro QR Platbu zde na GitHubu](https://github.com/spayd).

**K čemu je to dobré, když aplikace ani účtenky tyto QR kódy neobsahují?**  
Je to tak trochu začarovaný kruh. Aplikace Účtenkovka nepodporuje QR kódy, protože nejsou na účtenkách a účtenky je nepodporují, protože pro ně není žádné využití. Pro zavedení QR kódů je tedy třeba, aby někdo udělal ten první krok a tím prvním krokem je v tomto případě jednoduchý standard, který lze snadno zapojit do aplikace nebo pokladny na EET.
