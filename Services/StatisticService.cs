using System.Globalization;
using NHT_Marine_BE.Data.Dtos.Response;
using NHT_Marine_BE.Enums;
using NHT_Marine_BE.Extensions;
using NHT_Marine_BE.Interfaces.Repositories;
using NHT_Marine_BE.Interfaces.Services;
using NHT_Marine_BE.Models.Stock;
using NHT_Marine_BE.Models.Transaction;
using NHT_Marine_BE.Utilities;

namespace NHT_Marine_BE.Services
{
    public class StatisticService : IStatisticService
    {
        private readonly ICustomerRepository _customerRepo;
        private readonly IOrderRepository _orderRepo;
        private readonly IImportRepository _importRepo;
        private readonly IDamageReportRepository _damageReportRepo;
        private readonly IProductRepository _productRepo;
        private readonly IRoleRepository _roleRepo;

        public StatisticService(
            ICustomerRepository customerRepo,
            IOrderRepository orderRepo,
            IImportRepository importRepo,
            IDamageReportRepository damageReportRepo,
            IProductRepository productRepo,
            IRoleRepository roleRepo
        )
        {
            _customerRepo = customerRepo;
            _orderRepo = orderRepo;
            _importRepo = importRepo;
            _damageReportRepo = damageReportRepo;
            _productRepo = productRepo;
            _roleRepo = roleRepo;
        }

        private class ChartParams
        {
            public int Columns { get; set; } = 1;
            public string TimeUnit { get; set; } = string.Empty;
            public string Format { get; set; } = string.Empty;

            public ChartParams(int columns, string timeUnit, string format)
            {
                Columns = columns;
                TimeUnit = timeUnit;
                Format = format;
            }
        }

        private class ChartItem
        {
            public DateTime Date { get; set; } = DateTime.Now;
            public string Name { get; set; } = string.Empty;
            public decimal TotalImports { get; set; } = 0;
            public decimal TotalDamages { get; set; } = 0;
            public decimal TotalSales { get; set; } = 0;

            public ChartItem(DateTime date, string name, decimal totalImports, decimal totalDamages, decimal totalSales)
            {
                Date = date;
                Name = name;
                TotalImports = totalImports;
                TotalDamages = totalDamages;
                TotalSales = totalSales;
            }
        }

        public async Task<ServiceResponse<object>> GetSummaryStatistic(string type, int authRoleId)
        {
            var hasViewStatisticPermission = await _roleRepo.VerifyPermission(
                authRoleId,
                Permission.VIEW_REVENUE_AND_STATISTIC_DATA.ToString()
            );
            if (!hasViewStatisticPermission)
            {
                return new ServiceResponse<object>
                {
                    Status = ResStatusCode.FORBIDDEN,
                    Success = false,
                    Message = ErrorMessage.NO_PERMISSION,
                };
            }

            var currentTime = TimestampHandler.GetNow();
            var previousTime = TimestampHandler.GetPreviousTimeByType(currentTime, type);
            var startOfCurrentTime = TimestampHandler.GetStartOfTimeByType(currentTime, type);
            var startOfPreviousTime = TimestampHandler.GetStartOfTimeByType(previousTime, type);

            var currCustomers = await _customerRepo.GetAllCustomersInTimeRange(startOfCurrentTime, currentTime);
            var prevCustomers = await _customerRepo.GetAllCustomersInTimeRange(startOfPreviousTime, startOfCurrentTime);

            var currOrders = await _orderRepo.GetAllOrdersInTimeRange(startOfCurrentTime, currentTime);
            var prevOrders = await _orderRepo.GetAllOrdersInTimeRange(startOfPreviousTime, startOfCurrentTime);

            return new ServiceResponse<object>
            {
                Status = ResStatusCode.OK,
                Success = true,
                Data = new
                {
                    Customers = new { CurrentCount = currCustomers.Count, PreviousCount = prevCustomers.Count },
                    Orders = new { CurrentCount = currOrders.Count, PreviousCount = prevOrders.Count },
                    Revenues = new
                    {
                        CurrentCount = currOrders.Where(o => o.OrderStatus!.IsAccounted).Sum(o => o.TotalAmount),
                        PreviousCount = prevOrders.Where(o => o.OrderStatus!.IsAccounted).Sum(o => o.TotalAmount),
                    },
                },
            };
        }

        public async Task<ServiceResponse<object>> GetPopularStatistic(string type, int authRoleId)
        {
            var hasViewStatisticPermission = await _roleRepo.VerifyPermission(
                authRoleId,
                Permission.VIEW_REVENUE_AND_STATISTIC_DATA.ToString()
            );
            if (!hasViewStatisticPermission)
            {
                return new ServiceResponse<object>
                {
                    Status = ResStatusCode.FORBIDDEN,
                    Success = false,
                    Message = ErrorMessage.NO_PERMISSION,
                };
            }

            var currentTime = TimestampHandler.GetNow();
            var startOfCurrentTime = TimestampHandler.GetStartOfTimeByType(currentTime, type);

            var highestOrderCountCustomers = await _customerRepo.GetCustomersWithHighestOrderCountInTimeRange(
                startOfCurrentTime,
                currentTime,
                5
            );
            var highestOrderAmountCustomers = await _customerRepo.GetCustomersWithHighestOrderAmountInTimeRange(
                startOfCurrentTime,
                currentTime,
                5
            );

            return new ServiceResponse<object>
            {
                Status = ResStatusCode.OK,
                Success = true,
                Data = new
                {
                    HighestOrderCountCustomers = highestOrderCountCustomers.Select(customer => new
                    {
                        customer.CustomerId,
                        customer.FullName,
                        customer.Email,
                        customer.Avatar,
                        customer.CreatedAt,
                        customer.OrderCount,
                        IsActive = customer.Account != null && customer.Account.IsActive,
                    }),
                    HighestOrderAmountCustomers = highestOrderAmountCustomers.Select(customer => new
                    {
                        customer.CustomerId,
                        customer.FullName,
                        customer.Email,
                        customer.Avatar,
                        customer.CreatedAt,
                        customer.OrderAmount,
                        IsActive = customer.Account != null && customer.Account.IsActive,
                    }),
                },
            };
        }

        public async Task<ServiceResponse<object>> GetRevenuesChart(string type, int authRoleId)
        {
            var hasViewStatisticPermission = await _roleRepo.VerifyPermission(
                authRoleId,
                Permission.VIEW_REVENUE_AND_STATISTIC_DATA.ToString()
            );
            if (!hasViewStatisticPermission)
            {
                return new ServiceResponse<object>
                {
                    Status = ResStatusCode.FORBIDDEN,
                    Success = false,
                    Message = ErrorMessage.NO_PERMISSION,
                };
            }

            var currentTime = TimestampHandler.GetNow();
            var startOfCurrentTime = TimestampHandler.GetStartOfTimeByType(currentTime, type);

            var orders = await _orderRepo.GetAllOrdersInTimeRange(startOfCurrentTime, currentTime);
            var imports = await _importRepo.GetAllImportsInTimeRange(startOfCurrentTime, currentTime);
            var reports = await _damageReportRepo.GetAllReportsInTimeRange(startOfCurrentTime, currentTime);

            var revenuesChart = CreateRevenuesChart(
                orders,
                imports,
                reports,
                startOfCurrentTime,
                PrepareCreateChartParams(type, startOfCurrentTime)
            );

            return new ServiceResponse<object>
            {
                Status = ResStatusCode.OK,
                Success = true,
                Data = revenuesChart.Select(ci => new
                {
                    Name = ci.Name.CapitalizeAllWords(),
                    ci.TotalSales,
                    ci.TotalImports,
                    ci.TotalDamages,
                }),
            };
        }

        private ChartParams PrepareCreateChartParams(string type, DateTime startDate)
        {
            return type.ToLower() switch
            {
                "daily" => new ChartParams(24, "hour", "HH:mm"),
                "weekly" => new ChartParams(7, "day", "dddd dd-MM"),
                "monthly" => new ChartParams(startDate.GetDaysInMonth(), "day", "dd"),
                "yearly" => new ChartParams(12, "month", "MMMM"),
                _ => throw new ArgumentException("Invalid type"),
            };
        }

        private List<ChartItem> CreateRevenuesChart(
            List<Order> orders,
            List<ProductImport> productImports,
            List<ProductDamageReport> damageReports,
            DateTime startDate,
            ChartParams chartParams
        )
        {
            CultureInfo cultureInfo = new CultureInfo("vi-VN");
            List<ChartItem> chartItems = [];

            for (int i = 0; i < chartParams.Columns; i++)
            {
                chartItems.Add(
                    new ChartItem(
                        startDate.AddByUnitAndAmount(i, chartParams.TimeUnit),
                        startDate.AddByUnitAndAmount(i, chartParams.TimeUnit).ToString(chartParams.Format, cultureInfo),
                        0,
                        0,
                        0
                    )
                );
            }

            foreach (var order in orders)
            {
                if (order.OrderStatus?.IsAccounted == true)
                {
                    var index = chartItems.FindIndex(item => TimestampHandler.IsSame(item.Date, order.CreatedAt, chartParams.TimeUnit));
                    chartItems[index].TotalSales += order.TotalAmount;
                }
            }

            foreach (var import in productImports)
            {
                var index = chartItems.FindIndex(item => TimestampHandler.IsSame(item.Date, import.TrackedAt, chartParams.TimeUnit));
                chartItems[index].TotalImports -= import.TotalCost;
            }

            foreach (var report in damageReports)
            {
                var index = chartItems.FindIndex(item => TimestampHandler.IsSame(item.Date, report.ReportedAt, chartParams.TimeUnit));
                chartItems[index].TotalDamages -= report.TotalExpectedCost;
            }

            return chartItems;
        }

        public async Task<ServiceResponse<object>> GetProductStatistic(int productId, int authRoleId)
        {
            var hasViewStatisticPermission = await _roleRepo.VerifyPermission(authRoleId, Permission.VIEW_PRODUCT_SALES_DATA.ToString());
            if (!hasViewStatisticPermission)
            {
                return new ServiceResponse<object>
                {
                    Status = ResStatusCode.FORBIDDEN,
                    Success = false,
                    Message = ErrorMessage.NO_PERMISSION,
                };
            }

            var currentTime = TimestampHandler.GetNow();
            var startOfCurrentDay = TimestampHandler.GetStartOfTimeByType(currentTime, "daily");
            var startOfCurrentWeek = TimestampHandler.GetStartOfTimeByType(currentTime, "weekly");
            var startOfCurrentMonth = TimestampHandler.GetStartOfTimeByType(currentTime, "monthly");
            var startOfCurrentYear = TimestampHandler.GetStartOfTimeByType(currentTime, "yearly");

            var thisDayStatistic = await _productRepo.GetProductStatisticInTimeRange(startOfCurrentDay, currentTime, productId);
            var thisWeekStatistic = await _productRepo.GetProductStatisticInTimeRange(startOfCurrentWeek, currentTime, productId);
            var thisMonthStatistic = await _productRepo.GetProductStatisticInTimeRange(startOfCurrentMonth, currentTime, productId);
            var thisYearStatistic = await _productRepo.GetProductStatisticInTimeRange(startOfCurrentYear, currentTime, productId);

            return new ServiceResponse<object>
            {
                Status = ResStatusCode.OK,
                Success = true,
                Data = new
                {
                    Daily = thisDayStatistic,
                    Weekly = thisWeekStatistic,
                    Monthly = thisMonthStatistic,
                    Yearly = thisYearStatistic,
                },
            };
        }
    }
}
