USE NHT_Marine
GO

INSERT INTO dbo.Categories 
    (Name, CreatedAt, CreatedBy, ParentId) 
VALUES
    (N'Cá cảnh', '2025-07-15 07:00:00', '1', NULL),
    (N'Vật nuôi', '2025-07-15 07:01:01', '1', NULL),
    (N'Thức ăn cho cá', '2025-07-15 07:02:02', '1', NULL),
    (N'Cá 7 màu', '2025-07-16 14:57:57', '2', '1'),
    (N'Ba ba', '2025-07-16 14:58:58', '2', '2'),
    (N'Cá 7 màu Koi', '2025-07-16 14:59:59', '2', '4');
GO

INSERT INTO dbo.RootProducts 
    (CategoryId, Name, Description, ImageUrl, CreatedAt, CreatedBy) 
VALUES
    ('3', N'AF- PRO CÁM', N'Thức ăn cá cảnh chất lượng', 'https://res.cloudinary.com/dagaqa0ly/image/upload/v1753192528/products/twjxp6ygofovrwewsxzf.jpg', '2025-07-17 08:00:00', '1'),
    ('3', N'Arica cám', N'Thức ăn cá cảnh giá rẻ', 'https://res.cloudinary.com/dagaqa0ly/image/upload/v1753192637/products/ch4g1sevvri7gnzwrbzo.jpg', '2025-07-17 08:01:01', '1'),
    ('3', N'Boost Koi – Growth Color', N'Thức ăn cho cá koi', 'https://res.cloudinary.com/dagaqa0ly/image/upload/v1753192786/products/opgrqifckgvnmzpnot4i.jpg', '2025-07-17 08:02:02', '1'),
    ('5', N'Ba ba', N'Ba ba gai và ba ba hoa là lựa chọn hoàn hảo để bắt đầu kinh doanh nuôi rùa da mềm: sinh trưởng nhanh, ăn uống dễ, sinh sản đều, thị trường đa dạng. Dù bạn làm trang trại lớn hay nuôi đơn giản, chỉ cần ao bể cơ bản và quản lý tốt nguồn thức ăn là có thể triển khai hiệu quả.', 'https://res.cloudinary.com/dagaqa0ly/image/upload/v1753192851/products/zs5cf3l51bhu4kvgd2ar.jpg', '2025-07-17 08:03:03', '1'),
    ('6', N'Cá 7 màu koi đen', N'Cá bảy màu koi đen hay còn gọi là Koi Balck Guppy là một trong những dòng cá 7 màu được người chơi cá cảnh ưa chuộng. Cá có màu đen và đỏ nổi bật được biết đến như phiên bản đối nghịch của cá bày màu koi đỏ. Cá 7 màu koi đen có khả năng thích nghi tốt và không kén người chơi. Chúng có thể sống tốt trong nhiều kiểu bể nuôi.', 'https://res.cloudinary.com/dagaqa0ly/image/upload/v1753192934/products/poz3y1wdjhtdxhttsfjq.png', '2025-07-17 08:04:04', '2'),
    ('6', N'Cá 7 màu koi đỏ', N'Cá bảy màu koi đỏ hay còn gọi là 7 màu koi red là 1 loài Gumpy đang rất được ưa chuộng của thế giới thủy sinh. 7 màu koi red phân bố chủ yếu ở Nam Mỹ. Phù hợp cho cả người mới chơi lẫn bậc thầy thủy sinh, Koi Đỏ chính là lựa chọn không thể bỏ qua.', 'https://res.cloudinary.com/dagaqa0ly/image/upload/v1753193009/products/obgyoqoosc68zxeoasbw.png', '2025-07-17 08:05:05', '2');
GO

INSERT INTO dbo.ProductVariants
    (RootProductId, Name, IsAdjustable) 
VALUES
    ('1', N'Khối lượng', '0'),
    ('2', N'Khối lượng', '0'),
    ('3', N'Khối lượng', '0'),
    ('4', N'Năm tuổi', '1'),
    ('5', N'Kích thước', '1'),
    ('5', N'Giống', '1'),
    ('6', N'Kích thước', '1'),
    ('6', N'Giống', '1');
GO

INSERT INTO dbo.VariantOptions 
    (VariantId, Value) 
VALUES
    ('1', '100g'),
    ('1', '500g'),
    ('1', '1kg'),
    ('2', '100g'),
    ('2', '500g'),
    ('2', '1kg'),
    ('3', '1.5kg'),
    ('3', '3,5kg'),
    ('3', '6,5kg'),
    ('4', N'<1 năm'),
    ('4', N'1-2 năm'),
    ('4', N'>2 năm'),
    ('5', '3 cm'),
    ('5', '5 cm'),
    ('6', N'có'),
    ('6', N'không'),
    ('7', '3 cm'),
    ('7', '5 cm'),
    ('8', N'có'),
    ('8', N'không');
GO

INSERT INTO dbo.ProductItems 
    (RootProductId, ImageUrl, Price, PackingGuide) 
VALUES
    ('1', 'https://res.cloudinary.com/dagaqa0ly/image/upload/v1753192528/products/twjxp6ygofovrwewsxzf.jpg', '30000', N'Sử dụng túi zip hoặc túi hút chân không nhỏ, đảm bảo loại bỏ hết không khí và niêm phong chặt. Đặt vào hộp nhỏ hoặc túi đệm khí để bảo vệ.'),
    ('1', 'https://res.cloudinary.com/dagaqa0ly/image/upload/v1753192528/products/twjxp6ygofovrwewsxzf.jpg', '60000', N'Dùng túi zip lớn hơn, túi hút chân không chịu lực hoặc hũ nhựa/lon thiếc có nắp kín. Loại bỏ không khí (nếu là túi) và đậy nắp thật chặt (nếu là hũ/lon). Chèn thêm vật liệu đệm trong thùng carton để cố định.'),
    ('1', 'https://res.cloudinary.com/dagaqa0ly/image/upload/v1753192528/products/twjxp6ygofovrwewsxzf.jpg', '90000', N'Ưu tiên túi hút chân không cỡ lớn, dày dặn hoặc thùng/lon nhựa cứng có nắp vặn/khóa chặt. Đảm bảo loại bỏ tối đa không khí và niêm phong chắc chắn. Sử dụng thùng carton chắc chắn với đầy đủ vật liệu chèn lót (xốp, bóng khí) để chống sốc và xê dịch trong quá trình vận chuyển.'),
    ('2', 'https://res.cloudinary.com/dagaqa0ly/image/upload/v1753192637/products/ch4g1sevvri7gnzwrbzo.jpg', '7000', N'Sử dụng túi zip hoặc túi hút chân không nhỏ, đảm bảo loại bỏ hết không khí và niêm phong chặt. Đặt vào hộp nhỏ hoặc túi đệm khí để bảo vệ.'),
    ('2', 'https://res.cloudinary.com/dagaqa0ly/image/upload/v1753192637/products/ch4g1sevvri7gnzwrbzo.jpg', '25000', N'Dùng túi zip lớn hơn, túi hút chân không chịu lực hoặc hũ nhựa/lon thiếc có nắp kín. Loại bỏ không khí (nếu là túi) và đậy nắp thật chặt (nếu là hũ/lon). Chèn thêm vật liệu đệm trong thùng carton để cố định.'),
    ('2', 'https://res.cloudinary.com/dagaqa0ly/image/upload/v1753192637/products/ch4g1sevvri7gnzwrbzo.jpg', '40000', N'Ưu tiên túi hút chân không cỡ lớn, dày dặn hoặc thùng/lon nhựa cứng có nắp vặn/khóa chặt. Đảm bảo loại bỏ tối đa không khí và niêm phong chắc chắn. Sử dụng thùng carton chắc chắn với đầy đủ vật liệu chèn lót (xốp, bóng khí) để chống sốc và xê dịch trong quá trình vận chuyển.'),
    ('3', 'https://res.cloudinary.com/dagaqa0ly/image/upload/v1753192786/products/opgrqifckgvnmzpnot4i.jpg', '280000', N'Ưu tiên túi hút chân không cỡ lớn, dày dặn hoặc thùng/lon nhựa cứng có nắp vặn/khóa chặt. Đảm bảo loại bỏ tối đa không khí và niêm phong chắc chắn. Sử dụng thùng carton chắc chắn với đầy đủ vật liệu chèn lót (xốp, bóng khí) để chống sốc và xê dịch trong quá trình vận chuyển.'),
    ('3', 'https://res.cloudinary.com/dagaqa0ly/image/upload/v1753192786/products/opgrqifckgvnmzpnot4i.jpg', '640000', N'Ưu tiên túi hút chân không cỡ lớn, dày dặn hoặc thùng/lon nhựa cứng có nắp vặn/khóa chặt. Đảm bảo loại bỏ tối đa không khí và niêm phong chắc chắn. Sử dụng thùng carton chắc chắn với đầy đủ vật liệu chèn lót (xốp, bóng khí) để chống sốc và xê dịch trong quá trình vận chuyển.'),
    ('3', 'https://res.cloudinary.com/dagaqa0ly/image/upload/v1753192786/products/opgrqifckgvnmzpnot4i.jpg', '1100000', N'Ưu tiên túi hút chân không cỡ lớn, dày dặn hoặc thùng/lon nhựa cứng có nắp vặn/khóa chặt. Đảm bảo loại bỏ tối đa không khí và niêm phong chắc chắn. Sử dụng thùng carton chắc chắn với đầy đủ vật liệu chèn lót (xốp, bóng khí) để chống sốc và xê dịch trong quá trình vận chuyển.'),
    ('4', 'https://res.cloudinary.com/dagaqa0ly/image/upload/v1753192851/products/zs5cf3l51bhu4kvgd2ar.jpg', '6000', N'Tối đa 1 con mỗi bịch, cho khoảng 200ml nước sạch, bơm nhiều oxi và đặt vào thùng xốp lớn, gói hàng không sớm hơn 1 giờ trước trước khi đơn vị vận chuyển đến lấy.'),
    ('4', 'https://res.cloudinary.com/dagaqa0ly/image/upload/v1753192851/products/zs5cf3l51bhu4kvgd2ar.jpg', '100000', N'Tối đa 1 con mỗi bịch, cho khoảng 200ml nước sạch, bơm nhiều oxi và đặt vào thùng xốp lớn, gói hàng không sớm hơn 1 giờ trước trước khi đơn vị vận chuyển đến lấy.'),
    ('4', 'https://res.cloudinary.com/dagaqa0ly/image/upload/v1753192851/products/zs5cf3l51bhu4kvgd2ar.jpg', '250000', N'Tối đa 1 con mỗi bịch, cho khoảng 200ml nước sạch, bơm nhiều oxi và đặt vào thùng xốp lớn, gói hàng không sớm hơn 1 giờ trước trước khi đơn vị vận chuyển đến lấy.'),
    ('5', 'https://res.cloudinary.com/dagaqa0ly/image/upload/v1753192934/products/poz3y1wdjhtdxhttsfjq.png', '14000', N'Tối đa 3 con mỗi bịch, cho khoảng 100ml nước sạch, bơm nhiều oxi và đặt vào thùng xốp lớn, gói hàng không sớm hơn 1 giờ trước trước khi đơn vị vận chuyển đến lấy.'),
    ('5', 'https://res.cloudinary.com/dagaqa0ly/image/upload/v1753192934/products/poz3y1wdjhtdxhttsfjq.png', '40000', N'Tối đa 2 con mỗi bịch, cho khoảng 100ml nước sạch, bơm nhiều oxi và đặt vào thùng xốp lớn, gói hàng không sớm hơn 1 giờ trước trước khi đơn vị vận chuyển đến lấy.'),
    ('5', 'https://res.cloudinary.com/dagaqa0ly/image/upload/v1753192934/products/poz3y1wdjhtdxhttsfjq.png', '7000', N'Tối đa 3 con mỗi bịch, cho khoảng 100ml nước sạch, bơm nhiều oxi và đặt vào thùng xốp lớn, gói hàng không sớm hơn 1 giờ trước trước khi đơn vị vận chuyển đến lấy.'),
    ('5', 'https://res.cloudinary.com/dagaqa0ly/image/upload/v1753192934/products/poz3y1wdjhtdxhttsfjq.png', '20000', N'Tối đa 2 con mỗi bịch, cho khoảng 100ml nước sạch, bơm nhiều oxi và đặt vào thùng xốp lớn, gói hàng không sớm hơn 1 giờ trước trước khi đơn vị vận chuyển đến lấy.'),
    ('6', 'https://res.cloudinary.com/dagaqa0ly/image/upload/v1753193009/products/obgyoqoosc68zxeoasbw.png', '14000', N'Tối đa 3 con mỗi bịch, cho khoảng 100ml nước sạch, bơm nhiều oxi và đặt vào thùng xốp lớn, gói hàng không sớm hơn 1 giờ trước trước khi đơn vị vận chuyển đến lấy.'),
    ('6', 'https://res.cloudinary.com/dagaqa0ly/image/upload/v1753193009/products/obgyoqoosc68zxeoasbw.png', '40000', N'Tối đa 2 con mỗi bịch, cho khoảng 100ml nước sạch, bơm nhiều oxi và đặt vào thùng xốp lớn, gói hàng không sớm hơn 1 giờ trước trước khi đơn vị vận chuyển đến lấy.'),
    ('6', 'https://res.cloudinary.com/dagaqa0ly/image/upload/v1753193009/products/obgyoqoosc68zxeoasbw.png', '7000', N'Tối đa 3 con mỗi bịch, cho khoảng 100ml nước sạch, bơm nhiều oxi và đặt vào thùng xốp lớn, gói hàng không sớm hơn 1 giờ trước trước khi đơn vị vận chuyển đến lấy.'),
    ('6', 'https://res.cloudinary.com/dagaqa0ly/image/upload/v1753193009/products/obgyoqoosc68zxeoasbw.png', '20000', N'Tối đa 2 con mỗi bịch, cho khoảng 100ml nước sạch, bơm nhiều oxi và đặt vào thùng xốp lớn, gói hàng không sớm hơn 1 giờ trước trước khi đơn vị vận chuyển đến lấy.');
GO

INSERT INTO dbo.ProductAttributes 
    (ProductItemId, OptionId) 
VALUES
    ('1', '1'),
    ('2', '2'),
    ('3', '3'),
    ('4', '4'),
    ('5', '5'),
    ('6', '6'),
    ('7', '7'),
    ('8', '8'),
    ('9', '9'),
    ('10', '10'),
    ('11', '11'),
    ('12', '12'),
    ('13', '13'),
    ('13', '15'),
    ('14', '14'),
    ('14', '15'),
    ('15', '13'),
    ('15', '16'),
    ('16', '14'),
    ('16', '16'),
    ('17', '17'),
    ('17', '19'),
    ('18', '18'),
    ('18', '19'),
    ('19', '17'),
    ('19', '20'),
    ('20', '18'),
    ('20', '20');
GO