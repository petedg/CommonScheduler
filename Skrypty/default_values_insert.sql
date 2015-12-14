insert into department values (0, 'default', null, GETDATE(), null, 0);
insert into location values (0, 'default', '', '', '', null, 0, GETDATE(), null, 0);
insert into room values (0, 'Dodaj lokalizacjê specjaln¹', -1, 0, null, GETDATE(), 0);

insert into GlobalUser values (isnull((select max(id) + 1 from GlobalUser), 1), 'Konto administracyjne', '', 'sa', '', GETDATE(), null, 0, 1);

select * from GlobalUser





ALTER TABLE GlobalUser
ADD PASSWORD_TEMPORARY CHAR(1);

ALTER TABLE GlobalUser ALTER COLUMN PASSWORD_TEMPORARY char(1) not null

ALTER TABLE GLOBALUSER
DROP COLUMN PASSWORD_TEMPORATY;

SELECT * FROM GlobalUser

UPDATE GlobalUser SET PASSWORD_TEMPORARY = 1;
UPDATE GlobalUser SET PASSWORD_expiration = '2015-12-15';

delete from GlobalUser

select * from role
