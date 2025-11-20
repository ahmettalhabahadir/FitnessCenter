-- Verileri kontrol et
USE FitnessCenterDb;
GO

SET QUOTED_IDENTIFIER ON;
GO

PRINT '=== SPOR SALONU ===';
SELECT Id, Name, Address, Phone, Email FROM Gyms;
GO

PRINT '=== HİZMETLER ===';
SELECT Id, Name, Price, DurationInMinutes FROM Services;
GO

PRINT '=== ANTRENÖRLER ===';
SELECT Id, FirstName, LastName, Email, Phone, Specializations FROM Trainers;
GO

PRINT '=== ANTRENÖR-HİZMET İLİŞKİLERİ ===';
SELECT ts.TrainerId, t.FirstName + ' ' + t.LastName AS TrainerName, s.Name AS ServiceName
FROM TrainerServices ts
INNER JOIN Trainers t ON ts.TrainerId = t.Id
INNER JOIN Services s ON ts.ServiceId = s.Id;
GO

PRINT '=== ANTRENÖR MÜSAİTLİK SAATLERİ ===';
SELECT ta.Id, t.FirstName + ' ' + t.LastName AS TrainerName, 
       CASE ta.DayOfWeek 
           WHEN 0 THEN 'Pazar'
           WHEN 1 THEN 'Pazartesi'
           WHEN 2 THEN 'Salı'
           WHEN 3 THEN 'Çarşamba'
           WHEN 4 THEN 'Perşembe'
           WHEN 5 THEN 'Cuma'
           WHEN 6 THEN 'Cumartesi'
       END AS DayName,
       ta.StartTime, ta.EndTime
FROM TrainerAvailabilities ta
INNER JOIN Trainers t ON ta.TrainerId = t.Id
ORDER BY t.Id, ta.DayOfWeek;
GO

