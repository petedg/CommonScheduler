--Slownik Typy u�ytkownik�w (1)
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'GlobalAdmin', 1, GETDATE(), null, 0, 1);
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'SuperAdmin', 1, GETDATE(), null, 0, 1);
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'Admin', 1, GETDATE(), null, 0, 1);

--Slownik S�ownik r�l. (2)
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'Zarz�dzanie SuperAdministratorami', 1, GETDATE(), null, 0, 2);
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'Wyznaczanie administrator�w', 1, GETDATE(), null, 0, 2);
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'Definiowanie organizacji roku akademickiego', 1, GETDATE(), null, 0, 2);
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'Zarz�dzanie struktur� uczelni', 1, GETDATE(), null, 0, 2);
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'Obs�uga archiwum', 1, GETDATE(), null, 0, 2);
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'Wprowadzanie podgrup i grup zaj�ciowych', 1, GETDATE(), null, 0, 2);
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'Definiowanie nauczycieli akademickich', 1, GETDATE(), null, 0, 2);
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'Edycja planu zaj�� dla konkretnej grupy', 1, GETDATE(), null, 0, 2);
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'Aktualizacja globalnego planu', 1, GETDATE(), null, 0, 2);

--Slownik Poziomy studi�w. (3)
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'I stopnia (in�ynierskie)', 1, GETDATE(), null, 0, 3);
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'I stopnia (licencjackie)', 1, GETDATE(), null, 0, 3);
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'II stopnia (magisterskie)', 1, GETDATE(), null, 0, 3);
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'III stopnia (doktoranckie)', 1, GETDATE(), null, 0, 3);
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'jednolite magisterskie', 1, GETDATE(), null, 0, 3);
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'podyplomowe', 1, GETDATE(), null, 0, 3);

--Slownik Typy studi�w. (4)
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'stacjonarne', 1, GETDATE(), null, 0, 4);
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'niestacjonarne', 1, GETDATE(), null, 0, 4);

--Slownik Typy podgrup. (5)
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'rocznik', 1, GETDATE(), null, 0, 5);
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'podgrupa', 1, GETDATE(), null, 0, 5);

--Slownik Typy semestr�w. (6)
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'letni', 1, GETDATE(), null, 0, 6);
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'zimowy', 1, GETDATE(), null, 0, 6);

--Slownik Stopnie naukowe nauczycieli. (7)
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'lic.', 1, GETDATE(), null, 0, 7);
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'in�.', 1, GETDATE(), null, 0, 7);
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'mgr', 1, GETDATE(), null, 0, 7);
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'mgr in�.', 1, GETDATE(), null, 0, 7);
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'dr', 1, GETDATE(), null, 0, 7);
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'dr hab.', 1, GETDATE(), null, 0, 7);
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'prof.', 1, GETDATE(), null, 0, 7);

--Slownik typ�w stanowisk nauczycieli. (8)
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'lektor', 1, GETDATE(), null, 0, 8);
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'instruktor', 1, GETDATE(), null, 0, 8);
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'instruktor', 1, GETDATE(), null, 0, 8);
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'wyk�adowca', 1, GETDATE(), null, 0, 8);
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'starszy wyk�adowca', 1, GETDATE(), null, 0, 8);
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'docent', 1, GETDATE(), null, 0, 8);
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'asystent', 1, GETDATE(), null, 0, 8);
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'adiunkt', 1, GETDATE(), null, 0, 8);
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'prof. nadzyczajny', 1, GETDATE(), null, 0, 8);
insert into DictionaryValue values (isnull((select max(DV_ID)+1 from DictionaryValue), 0), 'prof. zwyczajny', 1, GETDATE(), null, 0, 8);