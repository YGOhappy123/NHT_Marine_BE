USE NHT_Marine
GO

INSERT INTO dbo.Coupons 
    (Code, Type, Amount, MaxUsage, IsActive, ExpiredAt, CreatedAt, CreatedBy)
VALUES 
    (N'CHAOHE2025', 0, 50000, 100, 1, '2025-08-31 23:59:59', '2025-06-01 08:00:00', 1),
    (N'SALESAPSAN', 1, 20, NULL, 1, '2025-12-31 23:59:59', '2025-07-01 09:30:00', 2),
    (N'TEAM100K', 0, 100000, 50, 1, '2025-07-31 23:59:59', '2025-06-15 10:15:00', 1),
    (N'KHACHVIP10', 1, 10, NULL, 1, '2025-12-31 23:59:59', '2025-05-10 14:00:00', 2),
    (N'HOTDEAL200', 0, 200000, 20, 0, '2025-12-31 23:59:59', '2025-04-20 11:45:00', 1),
    (N'MAXSALE99', 1, 99, 5, 1, '2025-10-15 23:59:59', '2025-07-10 08:20:00', 2),
    (N'FREESHIP50K', 0, 50000, NULL, 1, '2026-01-01 00:00:00', '2025-06-01 12:00:00', 1),
    (N'LOCKED25', 1, 25, 30, 0, '2025-09-01 00:00:00', '2025-05-30 15:00:00', 2);
GO

INSERT INTO dbo.Promotions
    (Name, Description, DiscountRate, StartDate, EndDate, IsActive, CreatedAt, CreatedBy)
VALUES
    (N'Siêu ưu đãi dòng cám cao cấp', N'Khuyến mãi 20% cho các sản phẩm AF- PRO CÁM, Arica cám và Boost Koi – Growth Color. Dành riêng cho mùa nuôi dưỡng cá tăng trưởng.', 20, '2025-08-01 00:00:00', '2025-08-10 23:59:59', 1, '2025-07-25 10:00:00', 1),
    (N'Guppy mùa lễ hội', N'Ưu đãi 25% cho dòng Cá 7 màu koi đỏ, koi đen và các loại cá Guppy khác. Mua càng nhiều càng giảm sâu!', 25, '2025-07-15 00:00:00', '2025-07-25 23:59:59', 1, '2025-07-10 09:00:00', 2),
    (N'Khuyến mãi combo sản phẩm tăng trưởng', N'Giảm 18% cho Boost Koi – Growth Color, AF- PRO CÁM và Arica cám khi mua combo 2 hoặc nhiều sản phẩm.', 18, '2025-08-12 00:00:00', '2025-08-20 23:59:59', 1, '2025-08-01 11:30:00', 2),
    (N'Mua cá tặng yêu thương', N'Chương trình giảm giá 15% khi mua bất kỳ sản phẩm trong nhóm: Cá 7 màu koi đỏ, Cá 7 màu koi đen, Ba ba. Áp dụng toàn quốc.', 15, '2025-07-20 00:00:00', '2025-07-30 23:59:59', 1, '2025-07-18 14:00:00', 1),
    (N'Ưu đãi dinh dưỡng mùa hè', N'Khuyến mãi 22% cho sản phẩm Boost Koi – Growth Color, AF- PRO CÁM, Arica cám trong tuần lễ chăm cá khỏe.', 22, '2025-08-18 00:00:00', '2025-08-24 23:59:59', 1, '2025-08-10 08:45:00', 2);
GO

INSERT INTO dbo.ProductsPromotions
    (PromotionId, ProductId)
VALUES
    -- Promotion 1: Siêu ưu đãi dòng cám cao cấp
    (11, 1), -- AF- PRO CÁM
    (11, 2), -- Arica cám
    (11, 3), -- Boost Koi – Growth Color
    -- Promotion 2: Guppy mùa lễ hội
    (12, 6), -- Cá 7 màu koi đỏ
    (12, 5), -- Cá 7 màu koi đen
    (12, 4), -- Ba ba
    -- Promotion 3: Khuyến mãi combo sản phẩm tăng trưởng
    (13, 3), -- Boost Koi – Growth Color
    (13, 1), -- AF- PRO CÁM
    (13, 2), -- Arica cám
    -- Promotion 4: Mua cá tặng yêu thương
    (14, 6), -- Cá 7 màu koi đỏ
    (14, 5), -- Cá 7 màu koi đen
    (14, 4), -- Ba ba
    -- Promotion 5: Ưu đãi dinh dưỡng mùa hè
    (15, 3), -- Boost Koi – Growth Color
    (15, 1), -- AF- PRO CÁM
    (15, 2); -- Arica cám
GO
