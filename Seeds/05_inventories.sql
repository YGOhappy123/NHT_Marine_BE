USE NHT_Marine
GO

INSERT INTO dbo.Suppliers
    (Name, Address, ContactEmail, ContactPhone)
VALUES 
    (N'Trang trại cá Thanh Sơn', N'97 Man Thiện, phường Hiệp Phú, quận 9, thành phố Hồ Chí Minh', 'thanhson.farm@gmail.com', '0912345678'),
    (N'Bể cá Hoàng Mai', N'25 Trần Thái Tông, phường Dịch Vọng, quận Cầu Giấy, Hà Nội', 'hoangmai.aquarium@gmail.com', '0901135246'),
    (N'Trại cá Miền Tây', N'118 Quốc lộ 1A, phường Tân Tạo, quận Bình Tân, TP.HCM', 'mientayfishfarm@gmail.com', '0905123456'),
    (N'Trung tâm Cá cảnh Sài Gòn', N'123 Lê Văn Sỹ, phường 13, quận Phú Nhuận, TP.HCM', 'saigonfishcenter@gmail.com', '0916457890'),
    (N'Cơ sở Cá giống Cửu Long', N'30 Nguyễn Văn Linh, TP. Cần Thơ', 'cuulongfishbase@gmail.com', '0932789456');
GO

INSERT INTO dbo.StorageTypes
    (Name)
VALUES 
    (N'Nhà kho'),
    (N'Bể cá loại 1m2'),
    (N'Bể cá mini'),
    (N'Bể cá ngoài trời'),
    (N'Tủ trưng bày cá cảnh');
GO

INSERT INTO dbo.Storages
    (Name, TypeId)
VALUES
    (N'Kho A', 1),
    (N'Kho B', 1),
    (N'Kho C', 1),
    (N'Bể cá 1A', 2),
    (N'Bể cá 1B', 2),
    (N'Bể cá 1C', 2),
    (N'Bể mini 1', 3),
    (N'Bể mini 2', 3),
    (N'Bể mini 3', 3),
    (N'Bể ngoài trời 01', 4),
    (N'Bể ngoài trời 02', 4),
    (N'Bể ngoài trời 03', 4),
    (N'Tủ trưng bày A', 5),
    (N'Tủ trưng bày B', 5),
    (N'Tủ trưng bày C', 5),
	(N'Bể cá dự phòng 1', 2),
    (N'Bể cá dự phòng 2', 2),
    (N'Tủ trưng bày D', 5),
    (N'Tủ trưng bày E', 5),
    (N'Kho D', 1),
    (N'Kho E', 1),   
    (N'Kho F', 1),
    (N'Bể mini 4', 3),
    (N'Bể mini 5', 3),
    (N'Bể ngoài trời 04', 4);
GO

INSERT INTO dbo.DamageTypes
    (Name)
VALUES 
    (N'Cá bệnh'),
    (N'Thất lạc'),
    (N'Hết hạn'),
    (N'Bể cá hỏng'),
    (N'Nước nhiễm bẩn'),
    (N'Nhiệt độ không ổn định'),
    (N'Cá chết do sốc nhiệt'),
    (N'Cá bị đánh nhau'),
    (N'Thiết bị lọc hỏng'),
    (N'Mất điện đột ngột'),
    (N'Thiếu oxy trong bể'),
    (N'Lỗi vận chuyển');
GO

INSERT INTO dbo.Inventories
    (StorageId, ProductItemId, Quantity)
VALUES
    -- Kho 1
    (1, 1, 25), (1, 2, 50), (1, 3, 75),
    -- Kho 2
    (2, 4, 60), (2, 5, 30),
    -- Kho 3
    (3, 6, 90), (3, 7, 55), (3, 8, 20),
    -- Kho 4
    (4, 9, 35),
    -- Kho 5 (không có sản phẩm)
    -- Kho 6
    (6, 10, 80), (6, 11, 45),
    -- Kho 7
    (7, 12, 22), (7, 13, 67),
    -- Kho 8
    (8, 14, 40),
    -- Kho 9
    (9, 15, 88), (9, 16, 33), (9, 17, 19),
    -- Kho 10
    (10, 18, 66), (10, 19, 41),
    -- Kho 11
    (11, 20, 58),
    -- Kho 12 (không có sản phẩm)
    -- Kho 13
    (13, 1, 13),
    -- Kho 14
    (14, 2, 77), (14, 3, 39),
    -- Kho 15
    (15, 4, 51),
	-- Kho 16
    (16, 21, 40), (16, 22, 35), (16, 23, 50),
    -- Kho 17
    (17, 24, 28), (17, 25, 70), (17, 26, 15),
    -- Kho 18
    (18, 27, 60), (18, 28, 42), (18, 29, 55),
    -- Kho 19
    (19, 30, 20), (19, 31, 45), (19, 32, 18), (19, 33, 25),
    -- Kho 20
    (20, 34, 32), (20, 35, 27), (20, 36, 36), (20, 37, 48),
    -- Kho 21
    (21, 38, 22), (21, 39, 29), (21, 40, 40), (21, 41, 19),
    -- Kho 22
    (22, 42, 50), (22, 43, 60), (22, 44, 45), (22, 45, 33),
    -- Kho 23
    (23, 46, 20), (23, 47, 35), (23, 48, 55), (23, 49, 28),
    -- Kho 24
    (24, 50, 42), (24, 51, 31), (24, 52, 25), (24, 53, 38),
    -- Kho 25
    (25, 54, 27), (25, 55, 60), (25, 56, 22), (25, 57, 18),
    -- Quay lại Kho 16 để phân bổ tiếp
    (16, 58, 40), (16, 59, 35), (16, 60, 50),
    -- Kho 17 bổ sung
    (17, 61, 22), (17, 62, 28), (17, 63, 19),
    -- Kho 18 bổ sung
    (18, 64, 33), (18, 65, 27), (18, 66, 41),
    -- Kho 19 bổ sung
    (19, 67, 36), (19, 68, 29), (19, 69, 44),
    -- Kho 20 bổ sung
    (20, 70, 55), (20, 71, 48), (20, 72, 25),
    -- Kho 21 bổ sung
    (21, 73, 18), (21, 74, 30), (21, 75, 26);
GO