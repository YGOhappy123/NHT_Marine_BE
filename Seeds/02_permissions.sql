USE NHT_Marine
GO

INSERT INTO dbo.AppPermissions
    (Code, Name)
VALUES
    -- Auth and roles related permissions
    ('ACCESS_ROLE_DASHBOARD_PAGE', N'Truy cập trang quản lý vai trò'), -- 1
    ('ADD_NEW_ROLE', N'Thêm vai trò mới'),
    ('UPDATE_ROLE', N'Chỉnh sửa vai trò'),
    ('REMOVE_ROLE', N'Xóa vai trò'),

    -- Advisory and customers related permissions
    ('ACCESS_CUSTOMER_DASHBOARD_PAGE', N'Truy cập trang quản lý khách hàng'), -- 5
    ('DEACTIVATE_CUSTOMER_ACCOUNT', N'Khóa tài khoản khách hàng'),
    ('ACCESS_ADVISORY_DASHBOARD_PAGE', N'Truy cập trang chăm sóc khách hàng'),
    ('CHAT_WITH_CUSTOMER', N'Nhắn tin tư vấn cho khách hàng'),
    
    -- Human resources related permissions 
    ('ACCESS_STAFF_DASHBOARD_PAGE', N'Truy cập trang quản lý nhân sự'), -- 9
    ('ADD_NEW_STAFF', N'Tạo nhân viên mới'),
    ('UPDATE_STAFF_INFORMATION', N'Cập nhật thông tin nhân viên'),
    ('CHANGE_STAFF_ROLE', N'Thay đổi vai trò của nhân viên'),
    ('DEACTIVATE_STAFF_ACCOUNT', N'Khóa tài khoản nhân viên'),
    ('MODIFY_PERSONAL_INFORMATION', N'Tự thay đổi thông tin cá nhân'),

    -- Categories and products related permissions
    ('ADD_NEW_PRODUCT_CATEGORY', N'Thêm danh mục sản phẩm mới'), -- 15
    ('UPDATE_PRODUCT_CATEGORY', N'Cập nhật danh mục sản phẩm'),
    ('DELETE_PRODUCT_CATEGORY', N'Xóa danh mục sản phẩm'),
    ('ADD_NEW_PRODUCT', N'Thêm sản phẩm mới'),
    ('UPDATE_PRODUCT_INFORMATION', N'Cập nhật thông tin sản phẩm'),
    ('UPDATE_PRODUCT_PRICE', N'Cập nhật giá sản phẩm'),
    ('DELETE_PRODUCT', N'Xóa sản phẩm'),

    -- Orders related permissions
    ('ACCESS_ORDER_DASHBOARD_PAGE', N'Truy cập trang quản lý đơn hàng'), -- 22
    ('PROCESS_ORDER', N'Xử lý đơn hàng'),
    ('ACCESS_ORDER_STATUS_DASHBOARD_PAGE', N'Truy cập trang quản lý trạng thái đơn hàng'),
    ('ADD_NEW_ORDER_STATUS', N'Thêm trạng thái đơn hàng mới'),
    ('UPDATE_ORDER_STATUS', N'Cập nhật trạng thái đơn hàng'),
    ('DELETE_ORDER_STATUS', N'Xóa trạng thái đơn hàng'),
    ('ACCESS_DELIVERY_SERVICE_DASHBOARD_PAGE', N'Truy cập trang quản lý đơn vị vận chuyển'),
    ('ADD_NEW_DELIVERY_SERVICE', N'Thêm đơn vị vận chuyển mới'),
    ('UPDATE_DELIVERY_SERVICE', N'Cập nhật đơn vị vận chuyển'),
    ('DELETE_DELIVERY_SERVICE', N'Xóa đơn vị vận chuyển'),

    -- Promotions and coupons related permissions
    ('ACCESS_PROMOTION_DASHBOARD_PAGE', N'Truy cập trang quản lý chương trình khuyến mãi'), --32
    ('ADD_NEW_PROMOTION', N'Thêm chương trình khuyến mãi mới'),
    ('UPDATE_PROMOTION', N'Cập nhật chương trình khuyến mãi'),
    ('DISABLE_PROMOTION', N'Dừng chương trình khuyến mãi'),
    ('ACCESS_COUPON_DASHBOARD_PAGE', N'Truy cập trang quản lý phiếu giảm giá'),
    ('ADD_NEW_COUPON', N'Thêm phiếu giảm giá mới'),
    ('UPDATE_COUPON', N'Cập nhật phiếu giảm giá'),
    ('DISABLE_COUPON', N'Khóa phiếu giảm giá'),

    -- Product imports related permissions
    ('ACCESS_SUPPLIER_DASHBOARD_PAGE', N'Truy cập trang quản lý nhà cung cấp'), -- 40
    ('ADD_NEW_SUPPLIER', N'Thêm nhà cung cấp mới'),
    ('UPDATE_SUPPLIER', N'Cập nhật nhà cung cấp'),
    ('DELETE_SUPPLIER', N'Xóa nhà cung cấp'),
    ('ACCESS_IMPORT_DASHBOARD_PAGE', N'Truy cập trang quản lý đơn nhập hàng'),
    ('ADD_NEW_IMPORT', N'Thêm đơn nhập hàng mới'),

    -- Stock management related permissions
    ('ACCESS_STORAGE_DASHBOARD_PAGE', N'Truy cập trang quản lý kho/ bể'), -- 46
    ('DISTRIBUTE_IMPORT_ITEM', N'Phân chia hàng nhập vào kho'),
    ('UPDATE_INVENTORY', N'Cập nhật hàng trong kho'),
    ('ADD_NEW_STORAGE', N'Thêm kho/ bể mới'),
    ('UPDATE_STORAGE', N'Cập nhật kho/ bể'),
    ('DELETE_STORAGE', N'Xóa kho/ bể'),
    ('ADD_NEW_STORAGE_TYPE', N'Thêm loại kho/ bể mới'),
    ('UPDATE_STORAGE_TYPE', N'Cập nhật loại kho/ bể'),
    ('DELETE_STORAGE_TYPE', N'Xóa loại kho/ bể'),
    ('ACCESS_DAMAGE_REPORT_DASHBOARD_PAGE', N'Truy cập trang quản lý báo cáo thiệt hại'),
    ('ADD_NEW_DAMAGE_REPORT', N'Thêm báo cáo thiệt hại mới'),
    ('UPDATE_DAMAGE_REPORT', N'Cập nhật báo cáo thiệt hại'),
    ('ADD_NEW_DAMAGE_CATEGORY', N'Thêm loại thiệt hại mới'),
    ('UPDATE_DAMAGE_CATEGORY', N'Cập nhật loại thiệt hại'),
    ('DELETE_DAMAGE_CATEGORY', N'Xóa loại thiệt hại'),

    -- Statistic related permissions
    ('ACCESS_STATISTIC_DASHBOARD_PAGE', N'Truy cập trang thống kê'), -- 61
    ('VIEW_PRODUCT_SALES_DATA', N'Xem các sản phẩm được bán ra'),
    ('VIEW_REVENUE_AND_STATISTIC_DATA', N'Xem doanh thu và thống kê');
GO

INSERT INTO dbo.RolesPermissions
    (RoleId, PermissionId)
VALUES
    (1, 1), (1, 2), (1, 3), (1, 4), (1, 5),
    (1, 6), (1, 7), (1, 8), (1, 9), (1, 10),
    (1, 11), (1, 12), (1, 13), (1, 14), (1, 15),
    (1, 16), (1, 17), (1, 18), (1, 19), (1, 20),
    (1, 21), (1, 22), (1, 23), (1, 24), (1, 25),
    (1, 26), (1, 27), (1, 28), (1, 29), (1, 30),
    (1, 31), (1, 32), (1, 33), (1, 34), (1, 35),
    (1, 36), (1, 37), (1, 38), (1, 39), (1, 40),
    (1, 41), (1, 42), (1, 43), (1, 44), (1, 45),
    (1, 46), (1, 47), (1, 48), (1, 49), (1, 50),
    (1, 51), (1, 52), (1, 53), (1, 54), (1, 55),
    (1, 56), (1, 57), (1, 58), (1, 59), (1, 60),
    (1, 61), (1, 62), (1, 63), (2, 44), (2, 45),
    (2, 46), (2, 47), (2, 48), (2, 55), (2, 56),
    (2, 57), (3, 22), (3, 23), (4, 5), (4, 7),
    (4, 8), (4,32), (4, 36), (5, 32), (5, 33),
    (5, 34), (5,35), (5, 36), (5, 37), (5, 38),
    (5, 39), (5, 61), (5, 62);
GO