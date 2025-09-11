# 📚 SPSUL - Tvorba a zkoušení testu pro studenty dálkového studia

## 📝 Úvod
Tato webová aplikace je určena pro **tvorbu a zkoušení testu pro studenty dálkového studia na SPSUL**.  
Systém generuje **náhodné testy pro každého studenta** na základě databáze předem připravených otázek různých typů (výběr odpovědi, textová odpověď aj.).  

Každý student má **unikátní přihlašovací ID**, které zajišťuje bezpečný přístup k testům a výsledkům.

Administrátor či učitel má své přihlašovací údaje s **jménem a heslem**.

---

## 📑 Obsah
1. [Funkce](#-funkce)  
2. [Technologie](#-technologie)  
3. [Použití](#-použití)  
4. [Administrátorské rozhraní](#-administrátorské-rozhraní)  
5. [Autoři](#-autoři)   

---

## 🚀 Funkce
- ✅ Generování náhodných testů pro každého studenta  
- ✅ Různé typy otázek (výběr odpovědi, textová odpověď, apod.)  
- ✅ Individuální přístup pomocí **přihlašovacího ID**  
- ✅ Zobrazení výsledků a historie testů pro studenty  
- ✅ Administrátorské rozhraní pro:
  - přidávání a editaci otázek  
  - správu uživatelů a administrátorů  
  - přístup k výsledkům studentů (podle přidělených práv)  
	
---

## 🛠 Technologie
- **Framework:** ASP.NET MVC (C#)  
- **Databáze:** SQL Server (MSSQL)  
- **ORM:** Entity Framework
- **Frontend:** Razor Pages, Custom Bootstrap, jQuery 
- **Server:** Školní server s IIS  

---

## 🎓 Použití
- **Studenti**:  
  - Přihlášení pomocí přiděleného ID  
  - Spuštění aktuálního testu  
  - Okamžitá zpětná vazba (úspěšnost, hodnocení)  
  - Historie předešlých testů  

- **Učitelé / Administrátoři**:  
  - Správa testových otázek  
  - Tvorba testů  
  - Monitoring výsledků studentů  

---

## 🔐 Administrátorské rozhraní
- Přístupné pouze uživatelům s **rolemi administrátorů**  
- Funkce:  
  - Přidávání / úprava / mazání otázek  
  - Správa studentů a přístupových práv  
  - Zobrazení statistik a výsledků  
  - Přidávání učitelů a nastavování jejích práv

---

## 👥 Autoři
Projekt vyvinut v rámci školního maturitního projektu.  
- Vývojový tým: Filip Éder 
