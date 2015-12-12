--Slownik Typy u¿ytkowników (1)
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'GlobalAdmin', 1, GETDATE(), null, 0, 1);
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'SuperAdmin', 1, GETDATE(), null, 0, 1);
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'Admin', 1, GETDATE(), null, 0, 1);

--Slownik S³ownik ról. (2)
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'Zarz¹dzanie SuperAdministratorami', 1, GETDATE(), null, 0, 2);
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'Wyznaczanie administratorów', 1, GETDATE(), null, 0, 2);
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'Definiowanie organizacji roku akademickiego', 1, GETDATE(), null, 0, 2);
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'Zarz¹dzanie struktur¹ uczelni', 1, GETDATE(), null, 0, 2);
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'Obs³uga archiwum', 1, GETDATE(), null, 0, 2);
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'Wprowadzanie podgrup i grup zajêciowych', 1, GETDATE(), null, 0, 2);
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'Definiowanie nauczycieli akademickich', 1, GETDATE(), null, 0, 2);
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'Edycja planu zajêæ dla konkretnej grupy', 1, GETDATE(), null, 0, 2);
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'Aktualizacja globalnego planu', 1, GETDATE(), null, 0, 2);

--Slownik Poziomy studiów. (3)
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'I stopnia (in¿ynierskie)', 1, GETDATE(), null, 0, 3);
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'I stopnia (licencjackie)', 1, GETDATE(), null, 0, 3);
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'II stopnia (magisterskie)', 1, GETDATE(), null, 0, 3);
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'III stopnia (doktoranckie)', 1, GETDATE(), null, 0, 3);
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'jednolite magisterskie', 1, GETDATE(), null, 0, 3);
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'podyplomowe', 1, GETDATE(), null, 0, 3);

--Slownik Typy studiów. (4)
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'stacjonarne', 1, GETDATE(), null, 0, 4);
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'niestacjonarne', 1, GETDATE(), null, 0, 4);

--Slownik Typy podgrup. (5)
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'rocznik', 1, GETDATE(), null, 0, 5);
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'podgrupa', 1, GETDATE(), null, 0, 5);

--Slownik Typy semestrów. (6)
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'letni', 1, GETDATE(), null, 0, 6);
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'zimowy', 1, GETDATE(), null, 0, 6);

--Slownik Stopnie naukowe nauczycieli. (7)
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'lic.', 1, GETDATE(), null, 0, 7);
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'in¿.', 1, GETDATE(), null, 0, 7);
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'mgr', 1, GETDATE(), null, 0, 7);
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'mgr in¿.', 1, GETDATE(), null, 0, 7);
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'dr', 1, GETDATE(), null, 0, 7);
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'dr hab.', 1, GETDATE(), null, 0, 7);
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'prof.', 1, GETDATE(), null, 0, 7);

--Slownik typów stanowisk nauczycieli. (8)
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'lektor', 1, GETDATE(), null, 0, 8);
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'instruktor', 1, GETDATE(), null, 0, 8);
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'instruktor', 1, GETDATE(), null, 0, 8);
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'wyk³adowca', 1, GETDATE(), null, 0, 8);
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'starszy wyk³adowca', 1, GETDATE(), null, 0, 8);
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'docent', 1, GETDATE(), null, 0, 8);
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'asystent', 1, GETDATE(), null, 0, 8);
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'adiunkt', 1, GETDATE(), null, 0, 8);
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'prof. nadzyczajny', 1, GETDATE(), null, 0, 8);
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'prof. zwyczajny', 1, GETDATE(), null, 0, 8);