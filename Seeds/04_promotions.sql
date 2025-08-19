USE NHT_Marine
GO

INSERT INTO dbo.Coupons 
    (Code, Type, Amount, MaxUsage, IsActive, ExpiredAt, CreatedAt, CreatedBy)
VALUES 
    (N'CHAOHE2025', 'Fixed', 50000, 100, 1, '2025-08-31 23:59:59', '2025-06-01 08:00:00', 1),
    (N'SALESAPSAN', 'Percentage', 20, NULL, 1, '2025-12-31 23:59:59', '2025-07-01 09:30:00', 2),
    (N'TEAM100K', 'Fixed', 100000, 50, 1, '2025-07-31 23:59:59', '2025-06-15 10:15:00', 1),
    (N'KHACHVIP10', 'Percentage', 10, NULL, 1, '2025-12-31 23:59:59', '2025-05-10 14:00:00', 2),
    (N'HOTDEAL200', 'Fixed', 200000, 20, 1, '2025-12-31 23:59:59', '2025-04-20 11:45:00', 1),
    (N'MAXSALE99', 'Percentage', 99, 5, 1, '2025-10-15 23:59:59', '2025-07-10 08:20:00', 2),
    (N'FREESHIP30K', 'Fixed', 30000, NULL, 1, '2026-01-01 00:00:00', '2025-06-01 12:00:00', 1),
    (N'LOCKED25', 'Percentage', 25, 30, 0, '2025-09-01 00:00:00', '2025-05-30 15:00:00', 2);
GO

INSERT INTO dbo.Promotions
    (Name, Description, DiscountRate, StartDate, EndDate, IsActive, CreatedAt, CreatedBy)
VALUES
    (N'Siêu ưu đãi dòng cám cao cấp', N'Khuyến mãi 20% cho các sản phẩm AF - Pro cám, Arica cám và Boost Koi – Growth Color. Dành riêng cho mùa nuôi dưỡng cá tăng trưởng.', 20, '2025-08-01 00:00:00', '2025-08-30 23:59:59', 1, '2025-07-25 10:00:00', 1),
    (N'Guppy mùa lễ hội', N'Ưu đãi 25% cho dòng Cá 7 màu koi đỏ, koi đen và các loại cá Guppy khác. Mua càng nhiều càng giảm sâu!', 25, '2025-07-15 00:00:00', '2025-07-25 23:59:59', 1, '2025-07-10 09:00:00', 2),
    (N'Khuyến mãi combo sản phẩm tăng trưởng', N'Giảm 18% cho Boost Koi – Growth Color, AF - Pro cám và Arica cám khi mua combo 2 hoặc nhiều sản phẩm.', 18, '2025-08-12 00:00:00', '2025-08-25 23:59:59', 1, '2025-08-01 11:30:00', 2),
    (N'Mua cá tặng yêu thương', N'Chương trình giảm giá 15% khi mua bất kỳ sản phẩm trong nhóm: Cá 7 màu koi đỏ, Cá 7 màu koi đen, Ba ba. Áp dụng toàn quốc.', 15, '2025-07-20 00:00:00', '2025-07-30 23:59:59', 1, '2025-07-18 14:00:00', 1),
    (N'Ưu đãi dinh dưỡng mùa hè', N'Khuyến mãi 22% cho sản phẩm Boost Koi – Growth Color, AF - Pro cám, Arica cám trong tuần lễ chăm cá khỏe.', 22, '2025-08-18 00:00:00', '2025-08-24 23:59:59', 1, '2025-08-10 08:45:00', 2);
GO

INSERT INTO dbo.ProductsPromotions
    (PromotionId, ProductId)
VALUES
    -- Promotion 1: Siêu ưu đãi dòng cám cao cấp
    (1, 1), -- AF - Pro cám
    (1, 2), -- Arica cám
    (1, 3), -- Boost Koi – Growth Color
    -- Promotion 2: Guppy mùa lễ hội
    (2, 6), -- Cá 7 màu koi đỏ
    (2, 5), -- Cá 7 màu koi đen
    (2, 4), -- Ba ba
    -- Promotion 3: Khuyến mãi combo sản phẩm tăng trưởng
    (3, 3), -- Boost Koi – Growth Color
    (3, 1), -- AF - Pro cám
    (3, 2), -- Arica cám
    -- Promotion 4: Mua cá tặng yêu thương
    (4, 6), -- Cá 7 màu koi đỏ
    (4, 5), -- Cá 7 màu koi đen
    (4, 4), -- Ba ba
    -- Promotion 5: Ưu đãi dinh dưỡng mùa hè
    (5, 3), -- Boost Koi – Growth Color
    (5, 1), -- AF - Pro cám
    (5, 2); -- Arica cám
GO