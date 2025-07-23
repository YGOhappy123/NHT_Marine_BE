USE NHT_Marine
GO

INSERT INTO dbo.DeliveryServices(name, ContactPhone)
VALUES 
(N'Giao Hàng Tiết Kiệm', '0987654321'),
(N'Shopee Express', '0901234567'),
(N'Giao Hàng Nhanh', '0934567890'),
(N'Viettel Post', '0978123456'),
(N'Ninja Van', '0967891234');
GO

INSERT INTO dbo.OrderStatuses (Name, Description, IsDefaultState, IsAccounted, IsUnfulfilled)
VALUES 
-- 1. Trạng thái mặc định khi khách đặt hàng
(N'Chờ xử lý', 
 N'Trạng thái mặc định khi khách hàng vừa đặt đơn. Nhân viên sẽ xem xét và quyết định tiếp nhận hoặc từ chối.', 
 1, 0, 0),

-- 2. Nhân viên tiếp nhận đơn
(N'Đã tiếp nhận', 
 N'Nhân viên đã xác nhận tiếp nhận đơn hàng và chuẩn bị xử lý.', 
 0, 0, 0),

-- 3. Đã đóng gói
(N'Đã đóng gói', 
 N'Sản phẩm đã được đóng gói và chờ đơn vị vận chuyển đến lấy hàng.', 
 0, 0, 0),

-- 4. Đang giao hàng
(N'Đang giao hàng', 
 N'Đơn hàng đang được vận chuyển đến tay khách hàng.', 
 0, 0, 0),

-- 5. Giao hàng thành công
(N'Giao hàng thành công', 
 N'Khách hàng đã nhận được hàng và thanh toán thành công.', 
 0, 1, 0),

-- 6. Đã thanh toán trước
(N'Đã thanh toán trước', 
 N'Khách hàng đã chuyển khoản hoặc thanh toán tiền trước khi giao hàng.', 
 0, 1, 0),

-- 7. Đơn bị từ chối
(N'Bị từ chối', 
 N'Nhân viên từ chối tiếp nhận đơn hàng (hết hàng, thông tin không rõ ràng, khách bom hàng nhiều lần...).', 
 0, 0, 1),

-- 8. Thất lạc
(N'Thất lạc', 
 N'Đơn hàng bị thất lạc trong quá trình vận chuyển hoặc không xác định được vị trí.', 
 0, 0, 1),

-- 9. Hư hỏng
(N'Hư hỏng', 
 N'Đơn hàng bị hư hại trong quá trình vận chuyển và không thể giao cho khách.', 
 0, 0, 1),

-- 10. Đã cọc nhưng không nhận hàng
(N'Đã cọc - Không nhận hàng', 
 N'Khách hàng đã cọc tiền nhưng từ chối nhận hàng khi giao tới. Cửa hàng vẫn ghi nhận phần doanh thu đã nhận.', 
 0, 1, 1);
GO
INSERT INTO dbo.StatusTransitions (FromStatusId, ToStatusId, TransitionLabel)
VALUES 
-- Chuyển từ "Chờ xử lý"
(1, 2, N'Chấp nhận đơn hàng'),
(1, 7, N'Từ chối đơn hàng'),

-- Chuyển từ "Đã tiếp nhận"
(2, 3, N'Đóng gói đơn hàng'),
(2, 7, N'Từ chối sau tiếp nhận'),

-- Chuyển từ "Đã đóng gói"
(3, 4, N'Chuyển cho đơn vị vận chuyển'),
(3, 8, N'Báo thất lạc'),
(3, 9, N'Báo hư hỏng'),

-- Chuyển từ "Đang giao hàng"
(4, 5, N'Xác nhận giao thành công'),
(4, 10, N'Khách đã cọc nhưng không nhận'),
(4, 8, N'Báo thất lạc khi giao'),
(4, 9, N'Báo hư hỏng khi giao'),

-- Chuyển từ "Đã thanh toán trước"
(6, 3, N'Đã thanh toán - tiếp tục đóng gói'),

-- Chuyển từ "Chờ xử lý"
(1, 6, N'Xác nhận đã thanh toán trước');
GO

