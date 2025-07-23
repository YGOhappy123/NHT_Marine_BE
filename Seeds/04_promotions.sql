USE NHT_Marine
GO
INSERT INTO dbo.Coupons (Code, Type, Amount, MaxUsage, IsActive, ExpiredAt, CreatedAt, CreatedBy)
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
-- Tháng 7
(N'Hè rộn ràng, sale sập sàn!', 
 N'Chương trình khuyến mãi chào hè 2025 với nhiều sản phẩm Betta, Ali, Guppy... và cơ hội trúng quà hấp dẫn.', 
 20, '2025-07-01 00:00:00', '2025-07-10 23:59:59', 1, '2025-07-01 07:00:00', 1),

(N'Ưu đãi giữa tháng 7', 
 N'Giảm giá đặc biệt 15% cho tất cả các đơn hàng trong tuần thứ 2 của tháng.', 
 15, '2025-07-11 00:00:00', '2025-07-17 23:59:59', 1, '2025-07-05 09:00:00', 2),

(N'Tháng 7 săn sale', 
 N'Tặng ngay 10% cho các đơn hàng từ 500K trở lên khi mua trong tuần cuối tháng 7.', 
 10, '2025-07-24 00:00:00', '2025-07-31 23:59:59', 1, '2025-07-10 12:00:00', 1),

(N'Shock cuối tháng 7', 
 N'Áp dụng giảm 25% cho các sản phẩm dòng cá Ali & Betta size lớn.', 
 25, '2025-07-28 00:00:00', '2025-07-31 23:59:59', 1, '2025-07-15 15:30:00', 2),

-- Tháng 8
(N'Mở màn tháng 8', 
 N'Mở màn tháng 8 rực rỡ với ưu đãi 20% toàn shop trong 3 ngày đầu tháng.', 
 20, '2025-08-01 00:00:00', '2025-08-03 23:59:59', 1, '2025-07-30 09:00:00', 1),

(N'Flash sale 8.8', 
 N'Sự kiện sale siêu tốc 8/8 – 1 ngày duy nhất giảm đến 30%.', 
 30, '2025-08-08 00:00:00', '2025-08-08 23:59:59', 1, '2025-08-01 10:00:00', 2),

(N'Giữa tháng 8 siêu ưu đãi', 
 N'Giảm giá 18% cho các sản phẩm thủy sinh từ 12–18/8.', 
 18, '2025-08-12 00:00:00', '2025-08-18 23:59:59', 1, '2025-08-05 14:00:00', 1),

(N'Cuối tháng 8 rộn ràng', 
 N'Khuyến mãi 12% toàn bộ đơn hàng trong 5 ngày cuối tháng 8.', 
 12, '2025-08-27 00:00:00', '2025-08-31 23:59:59', 1, '2025-08-10 08:00:00', 2),

(N'Tháng 8 – Chốt đơn mạnh', 
 N'Ưu đãi 22% cho đơn hàng từ 1 triệu đồng trong tháng 8.', 
 22, '2025-08-01 00:00:00', '2025-08-31 23:59:59', 1, '2025-07-31 17:00:00', 1),

(N'Shock Sale Cá Guppy', 
 N'Sale 15% các dòng cá Guppy fancy, koi và full red.', 
 15, '2025-08-15 00:00:00', '2025-08-20 23:59:59', 1, '2025-08-10 10:30:00', 2);
GO
