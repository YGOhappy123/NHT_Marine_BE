USE NHT_Marine
GO

INSERT INTO dbo.StaffRoles
	(Name, IsImmutable)
VALUES
	(N'Super Admin', 1),
	(N'Nhân Viên Quản Lý Kho', 0),
	(N'Nhân Viên Xử Lý Đơn Hàng', 0),
	(N'Nhân Viên Tư Vấn', 0),
    (N'Nhân Viên Marketing', 0);
GO

INSERT INTO dbo.Accounts
	(Username, Password, IsActive)
VALUES
	('customer0001', '$2a$11$nkC/M88KMyKWGmd44/aQzeSuuwq7.mfCFoRpb8OGHykQG0JnEFROC', 1),
	('customer0002', '$2a$11$KlwjhvWsKbfIi647k3qUr.ia9ZdQLZYcgITuEpueBO52AoyAhU9OK', 1),
	('customer0003', '$2a$11$0BZ2SnQREPsbPRhVOhaaVeeSQAlyzsFADL595pKc70ZFd1zHaCcAa', 1),
	('customer0004', '$2a$11$Oz.G5fwD8zq38K0lUCIYD.QZZ4B2cmqFkJuC47653Ev7PGqs71Zra', 1),
	('customer0005', '$2a$11$3P2dvTISOITxoKZERhT/je25XYkC5v7TDacTfGO2HUubRLtC/SL9q', 1),
	('staff0001', '$2a$11$ySwhpRk89aWrAvHI0iRtNuCpABHLbtgYym3ptSBA2ybwUy5H18.A2', 1),
	('staff0002', '$2a$11$tths/WMBffLQUWt33aNrkOPDi2l/m3L2MGOVjn5gaP.SaOpSWu3Xa', 1),
	('staff0003', '$2a$11$yjR8nOPp206keajtK2oupO.5tTICOhPHn/57tzN.MvcFShAQ9sZxe', 1),
	('staff0004', '$2a$11$7rJW6QKXD0s8/1FTSYjSeuTJlQmVIJjFX8Pp0nPepDqzejE8cnLq2', 1),
	('staff0005', '$2a$11$zqXsN0LPHcMoAKEe5/5pdOBsjnv/HH1fa0.E98pPhoIGkUCW2lj4u', 1),
	('staff0006', '$2a$11$2X2QD.jtfFtjPY4Fz/hmkuoXe.GjpZscjs82RtegrLItV9R92fboW', 1);
GO

INSERT INTO dbo.Customers
	(FullName, Email, Avatar, CreatedAt, AccountId)
VALUES
	(N'Trần Thị Vân Anh', 'vananhtt@gmail.com', 'https://res.cloudinary.com/dagaqa0ly/image/upload/v1748456563/avatar/uute8mhvnktvwyanwtef.jpg', GETDATE(), 1),
	(N'Phạm Thị Kiều Trang', 'kieutrangpt@gmail.com', 'https://res.cloudinary.com/dagaqa0ly/image/upload/v1737043282/avatar/istockphoto-1395009090-612x612_cz9cl1.jpg', GETDATE(), 2),
	(N'Hà Huyền My', 'huyenmyh@gmail.com', 'https://res.cloudinary.com/dagaqa0ly/image/upload/v1736128670/avatar/pexels-photo-573299_utp10s.jpg', GETDATE(), 3),
	(N'Lâm Chí Cường', 'chicuongl@gmail.com', 'https://res.cloudinary.com/dagaqa0ly/image/upload/v1736128669/avatar/pexels-photo-1516680_ku3mbb.jpg', GETDATE(), 4),
	(N'Nguyễn Văn Minh', 'vanminhn@gmail.com', 'https://res.cloudinary.com/dagaqa0ly/image/upload/v1736128668/avatar/pexels-photo-1680172_grg4ug.jpg', GETDATE(), 5);
GO

INSERT INTO dbo.Staffs
	(CreatedBy, FullName, Email, Avatar, RoleId, CreatedAt, AccountId)
VALUES
	(NULL, N'Nguyễn Văn Vũ', 'vanvun@gmail.com', 'https://res.cloudinary.com/dagaqa0ly/image/upload/v1752170940/avatar/photo-1742119897876-67e9935ac375_oiyq3t.avif', 1, GETDATE(), 6),
	(NULL, N'Huỳnh Thị Thu Nga', 'thungaht@gmail.com', 'https://res.cloudinary.com/dagaqa0ly/image/upload/v1752170987/avatar/photo-1676777455261-fcd8a99a9bca_z2t0de.jpg', 1, GETDATE(), 7),
	(1, N'Lê Thành Công', 'thanhcongl@gmail.com', 'https://res.cloudinary.com/dagaqa0ly/image/upload/v1752170940/avatar/photo-1718209881006-f6e313e2e109_qjex5e.avif', 2, GETDATE(), 8),
	(1, N'Nguyễn Trần Anh Quân', 'anhquannt@gmail.com', 'https://res.cloudinary.com/dagaqa0ly/image/upload/v1752426735/avatar/photo-1633332755192-727a05c4013d_p9ppsx.avif', 3, GETDATE(), 9),
	(2, N'Hà Ánh Tuyết', 'anhtuyeth@gmail.com', 'https://res.cloudinary.com/dagaqa0ly/image/upload/v1752170940/avatar/photo-1718999398032-8fc0a58ed9c7_nezkcx.avif', 4, GETDATE(), 10),
	(2, N'Phan Thu Hương', 'thuhuongp@gmail.com', 'https://res.cloudinary.com/dagaqa0ly/image/upload/v1752170940/avatar/photo-1596304250579-8cb49baae3f2_bepdku.avif', 5, GETDATE(), 11);
GO