-- Test verileri ekleme script'i
USE FitnessCenterDb;
GO

SET QUOTED_IDENTIFIER ON;
GO

-- Önce admin kullanıcısının ID'sini ve GymId'sini alalım
DECLARE @AdminUserId NVARCHAR(450);
DECLARE @GymId INT;

SELECT @AdminUserId = Id, @GymId = GymId 
FROM AspNetUsers 
WHERE Email = 'B231210038@sakarya.edu.tr';

-- Eğer admin'in spor salonu yoksa, mevcut spor salonunu al veya yeni oluştur
IF @GymId IS NULL
BEGIN
    SELECT @GymId = Id FROM Gyms WHERE Id = 1;
    IF @GymId IS NULL
    BEGIN
        -- Yeni spor salonu oluştur
        INSERT INTO Gyms (Name, Address, Phone, Email, OpeningTime, ClosingTime, WorkingHours, Description)
        VALUES ('Ana Spor Salonu', 'Sakarya Üniversitesi Kampüsü', '0264 295 0000', 'sporsalonu@sakarya.edu.tr', '06:00', '22:00', 'Pazartesi - Pazar: 06:00 - 22:00', 'Ana spor salonu');
        SET @GymId = SCOPE_IDENTITY();
        
        -- Admin'e spor salonunu ata
        UPDATE AspNetUsers SET GymId = @GymId WHERE Id = @AdminUserId;
    END
END

-- Spor salonu bilgilerini güncelle (eğer varsa)
UPDATE Gyms 
SET Name = 'FitZone Spor Salonu',
    Address = 'Sakarya Üniversitesi Kampüsü, Esentepe Mahallesi',
    Phone = '0264 295 5000',
    Email = 'info@fitzone.com',
    OpeningTime = '06:00',
    ClosingTime = '23:00',
    WorkingHours = 'Pazartesi - Pazar: 06:00 - 23:00',
    Description = 'Modern ekipmanlar ve profesyonel antrenörlerle hizmet veren spor salonu'
WHERE Id = @GymId;

-- Hizmetler ekle
IF NOT EXISTS (SELECT 1 FROM Services WHERE GymId = @GymId)
BEGIN
    INSERT INTO Services (GymId, Name, Description, Price, DurationInMinutes) VALUES
    (@GymId, 'Kişisel Antrenörlük', 'Birebir kişisel antrenör eşliğinde özel program', 500.00, 60),
    (@GymId, 'Grup Fitness', 'Grup halinde fitness dersleri', 150.00, 45),
    (@GymId, 'Yoga', 'Hatha ve Vinyasa yoga dersleri', 120.00, 60),
    (@GymId, 'Pilates', 'Mat ve reformer pilates dersleri', 130.00, 50),
    (@GymId, 'Kardiyovasküler Antrenman', 'Koşu bandı, bisiklet ve eliptik antrenman', 80.00, 30),
    (@GymId, 'Ağırlık Antrenmanı', 'Serbest ağırlık ve makine antrenmanı', 100.00, 45),
    (@GymId, 'CrossFit', 'Yoğun fonksiyonel antrenman', 200.00, 60),
    (@GymId, 'Zumba', 'Eğlenceli dans fitness dersleri', 110.00, 45);
END

-- Antrenörler ekle
DECLARE @Trainer1Id INT, @Trainer2Id INT, @Trainer3Id INT, @Trainer4Id INT, @Trainer5Id INT;

IF NOT EXISTS (SELECT 1 FROM Trainers WHERE GymId = @GymId)
BEGIN
    INSERT INTO Trainers (GymId, FirstName, LastName, Email, Phone, Specializations, Biography) VALUES
    (@GymId, 'Ahmet', 'Yılmaz', 'ahmet.yilmaz@fitzone.com', '0532 111 2233', 'Kişisel Antrenörlük, Ağırlık Antrenmanı', '10 yıllık deneyime sahip sertifikalı kişisel antrenör. Vücut geliştirme ve güç antrenmanı konusunda uzman.'),
    (@GymId, 'Ayşe', 'Demir', 'ayse.demir@fitzone.com', '0532 222 3344', 'Yoga, Pilates', 'Yoga ve pilates eğitmeni. 8 yıldır grup ve özel dersler veriyor.'),
    (@GymId, 'Mehmet', 'Kaya', 'mehmet.kaya@fitzone.com', '0532 333 4455', 'CrossFit, Fonksiyonel Antrenman', 'CrossFit Level 2 sertifikalı antrenör. Rekabetçi sporcu geçmişi var.'),
    (@GymId, 'Zeynep', 'Şahin', 'zeynep.sahin@fitzone.com', '0532 444 5566', 'Grup Fitness, Zumba, Kardiyovasküler', 'Enerjik grup fitness eğitmeni. Zumba ve aerobik dersleri veriyor.'),
    (@GymId, 'Can', 'Öztürk', 'can.ozturk@fitzone.com', '0532 555 6677', 'Kişisel Antrenörlük, Beslenme Danışmanlığı', 'Beslenme uzmanı ve kişisel antrenör. Sağlıklı yaşam koçluğu yapıyor.');
    
    SET @Trainer1Id = SCOPE_IDENTITY() - 4;
    SET @Trainer2Id = SCOPE_IDENTITY() - 3;
    SET @Trainer3Id = SCOPE_IDENTITY() - 2;
    SET @Trainer4Id = SCOPE_IDENTITY() - 1;
    SET @Trainer5Id = SCOPE_IDENTITY();
END
ELSE
BEGIN
    SELECT TOP 5 @Trainer1Id = Id FROM Trainers WHERE GymId = @GymId ORDER BY Id;
    SELECT TOP 1 @Trainer2Id = Id FROM Trainers WHERE GymId = @GymId AND Id > @Trainer1Id ORDER BY Id;
    SELECT TOP 1 @Trainer3Id = Id FROM Trainers WHERE GymId = @GymId AND Id > @Trainer2Id ORDER BY Id;
    SELECT TOP 1 @Trainer4Id = Id FROM Trainers WHERE GymId = @GymId AND Id > @Trainer3Id ORDER BY Id;
    SELECT TOP 1 @Trainer5Id = Id FROM Trainers WHERE GymId = @GymId AND Id > @Trainer4Id ORDER BY Id;
END

-- Antrenör-Hizmet ilişkileri (TrainerServices)
DECLARE @Service1Id INT, @Service2Id INT, @Service3Id INT, @Service4Id INT, @Service5Id INT, @Service6Id INT, @Service7Id INT, @Service8Id INT;

SELECT TOP 1 @Service1Id = Id FROM Services WHERE GymId = @GymId AND Name = 'Kişisel Antrenörlük';
SELECT TOP 1 @Service2Id = Id FROM Services WHERE GymId = @GymId AND Name = 'Grup Fitness';
SELECT TOP 1 @Service3Id = Id FROM Services WHERE GymId = @GymId AND Name = 'Yoga';
SELECT TOP 1 @Service4Id = Id FROM Services WHERE GymId = @GymId AND Name = 'Pilates';
SELECT TOP 1 @Service5Id = Id FROM Services WHERE GymId = @GymId AND Name = 'Kardiyovasküler Antrenman';
SELECT TOP 1 @Service6Id = Id FROM Services WHERE GymId = @GymId AND Name = 'Ağırlık Antrenmanı';
SELECT TOP 1 @Service7Id = Id FROM Services WHERE GymId = @GymId AND Name = 'CrossFit';
SELECT TOP 1 @Service8Id = Id FROM Services WHERE GymId = @GymId AND Name = 'Zumba';

-- Antrenörlere hizmetleri ata
IF @Trainer1Id IS NOT NULL AND @Service1Id IS NOT NULL AND NOT EXISTS (SELECT 1 FROM TrainerServices WHERE TrainerId = @Trainer1Id AND ServiceId = @Service1Id)
BEGIN
    INSERT INTO TrainerServices (TrainerId, ServiceId) VALUES
    (@Trainer1Id, @Service1Id), -- Ahmet - Kişisel Antrenörlük
    (@Trainer1Id, @Service6Id), -- Ahmet - Ağırlık Antrenmanı
    (@Trainer2Id, @Service3Id), -- Ayşe - Yoga
    (@Trainer2Id, @Service4Id), -- Ayşe - Pilates
    (@Trainer3Id, @Service7Id), -- Mehmet - CrossFit
    (@Trainer4Id, @Service2Id), -- Zeynep - Grup Fitness
    (@Trainer4Id, @Service8Id), -- Zeynep - Zumba
    (@Trainer4Id, @Service5Id), -- Zeynep - Kardiyovasküler
    (@Trainer5Id, @Service1Id); -- Can - Kişisel Antrenörlük
END

-- Antrenör müsaitlik saatleri (TrainerAvailabilities)
-- Pazartesi (1) - Cuma (5): 09:00 - 18:00
IF @Trainer1Id IS NOT NULL AND NOT EXISTS (SELECT 1 FROM TrainerAvailabilities WHERE TrainerId = @Trainer1Id)
BEGIN
    INSERT INTO TrainerAvailabilities (TrainerId, DayOfWeek, StartTime, EndTime) VALUES
    (@Trainer1Id, 1, '09:00', '18:00'), -- Pazartesi
    (@Trainer1Id, 2, '09:00', '18:00'), -- Salı
    (@Trainer1Id, 3, '09:00', '18:00'), -- Çarşamba
    (@Trainer1Id, 4, '09:00', '18:00'), -- Perşembe
    (@Trainer1Id, 5, '09:00', '18:00'), -- Cuma
    (@Trainer2Id, 1, '10:00', '19:00'),
    (@Trainer2Id, 2, '10:00', '19:00'),
    (@Trainer2Id, 3, '10:00', '19:00'),
    (@Trainer2Id, 4, '10:00', '19:00'),
    (@Trainer2Id, 5, '10:00', '19:00'),
    (@Trainer3Id, 1, '08:00', '17:00'),
    (@Trainer3Id, 2, '08:00', '17:00'),
    (@Trainer3Id, 3, '08:00', '17:00'),
    (@Trainer3Id, 4, '08:00', '17:00'),
    (@Trainer3Id, 5, '08:00', '17:00'),
    (@Trainer4Id, 1, '09:00', '20:00'),
    (@Trainer4Id, 2, '09:00', '20:00'),
    (@Trainer4Id, 3, '09:00', '20:00'),
    (@Trainer4Id, 4, '09:00', '20:00'),
    (@Trainer4Id, 5, '09:00', '20:00'),
    (@Trainer5Id, 1, '10:00', '18:00'),
    (@Trainer5Id, 2, '10:00', '18:00'),
    (@Trainer5Id, 3, '10:00', '18:00'),
    (@Trainer5Id, 4, '10:00', '18:00'),
    (@Trainer5Id, 5, '10:00', '18:00');
END

PRINT 'Test verileri başarıyla eklendi!';
GO

