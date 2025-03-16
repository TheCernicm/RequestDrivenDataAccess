## nahrávačka - jednoduchá aplikace v c# pro nahrávání souborů
---

### Úvod
---
Tato aplikace byla vytvořena, pro odzkoušení redis a nahrání souborů. 
Aplikace přijímá soubor a vrací pouze 10 prvních slov v daném souboru. 
Toto má simulovat nějakou složitější činost, která se bude odehrávat na pozadí.

### Instalace
---

Uvnitř `pokusHttp\FileUpload` zavolat příkaz `docker-compose up`
Po doběhnutí dockeru aplikace běží na localhost:5000


### Použití
---
Soubory se vloží do aplikace přes ui v `http://localhost:5000/home/index`, `http://localhost:5000`, nebo přes post http://localhost:5000/api/file.
Formát na post je:
- text: userName
- text: uploadFormat 
- text: wantedFormat
- file: file 

Uživatel může získat první soubor ve frontě přes get `http://localhost:5000/api/file`, nebo přes  `http://localhost:5000/home/getfile`. Tímto způsobem uživatel dostane poslední upravený soubor z databáze. 
Čímž ho i odstraní. Aplikace dokáže zpracovávat až 4 soubory najednou. 

### Dodatečné informace
---

Aplikace má v sobě dvě redis fronty, jednu pro tasky, další pro zobrazení. V těchto frontách se nachází pouze identifikátory.
Metadata a soubory jsou uloženy zvlášť a dostupné přes identifikátor. 

Funkce na opravení souborů je velmi jednoduchá a není důležitá pro aplikace, má jen simulovat nějakou činnost aplikace.

### Poznámky
---
docker-compose.yml vygenerované od visual studia mi blbne s .env, a tedy pro přístě si ho radší nastavím docker-compose.yml sám.
