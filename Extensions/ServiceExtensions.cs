using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NHT_Marine_BE.Data;
using NHT_Marine_BE.Interfaces.Repositories;
using NHT_Marine_BE.Interfaces.Services;
using NHT_Marine_BE.Repositories;
using NHT_Marine_BE.Services;

namespace NHT_Marine_BE.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddJsonOptionsForControllers(this IServiceCollection services)
        {
            services
                .AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            return services;
        }

        public static IServiceCollection AddCorsPolicies(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins", policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });

            return services;
        }

        public static IServiceCollection ConfigureApiBehavior(this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            return services;
        }

        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<JwtSecurityTokenHandler>();
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:AccessTokenSecret"]!)),
                        ClockSkew = TimeSpan.Zero,
                    };
                });
            services.AddAuthorization(options =>
            {
                options.AddPolicy(
                    "StaffOnly",
                    policy =>
                        policy
                            .RequireAuthenticatedUser()
                            .RequireAssertion(context =>
                                context.User.HasClaim(c => c.Type == ClaimTypes.Role && !string.IsNullOrEmpty(c.Value))
                            )
                );

                options.AddPolicy(
                    "CustomerOnly",
                    policy =>
                        policy
                            .RequireAuthenticatedUser()
                            .RequireAssertion(context => !context.User.HasClaim(c => c.Type == ClaimTypes.Role))
                );
            });

            return services;
        }

        public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(option =>
            {
                option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
                option.AddSecurityDefinition(
                    "Bearer",
                    new OpenApiSecurityScheme
                    {
                        In = ParameterLocation.Header,
                        Description = "Please enter a valid token",
                        Name = "Authorization",
                        Type = SecuritySchemeType.Http,
                        BearerFormat = "JWT",
                        Scheme = "Bearer",
                    }
                );
                option.AddSecurityRequirement(
                    new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" },
                            },
                            new string[] { }
                        },
                    }
                );
            });

            return services;
        }

        public static IServiceCollection AddDatabaseContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDBContext>(options =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("DbConnectionString"),
                    options =>
                    {
                        options.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                    }
                );
            });

            return services;
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Repository interfaces
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<ICartRepository, CartRepository>();
            services.AddScoped<IStaffRepository, StaffRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IPermissionRepository, PermissionRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IPromotionRepository, PromotionRepository>();
            services.AddScoped<ICouponRepository, CouponRepository>();
            services.AddScoped<IDamageTypeRepository, DamageTypeRepository>();
            services.AddScoped<IStorageTypeRepository, StorageTypeRepository>();
            services.AddScoped<ISupplierRepository, SupplierRepository>();
            services.AddScoped<IDeliveryServiceRepository, DeliveryServiceRepository>();
            services.AddScoped<IOrderStatusRepository, OrderStatusRepository>();
            services.AddScoped<IImportRepository, ImportRepository>();
            services.AddScoped<IDamageReportRepository, DamageReportRepository>();
            services.AddScoped<IStorageRepository, StorageRepository>();
            services.AddScoped<IStatusTransitionRepository, StatusTransitionRepository>();

            // Services interfaces
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IMailerService, MailerService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IStaffService, StaffService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IPromotionService, PromotionService>();
            services.AddScoped<IDamageTypeService, DamageTypeService>();
            services.AddScoped<IStorageTypeService, StorageTypeService>();
            services.AddScoped<ISupplierService, SupplierService>();
            services.AddScoped<IDeliveryServiceService, DeliveryServiceService>();
            services.AddScoped<IOrderStatusService, OrderStatusService>();
            services.AddScoped<IStatisticService, StatisticService>();
            services.AddScoped<IStorageService, StorageService>();
            services.AddScoped<IImportService, ImportService>();

            return services;
        }

        public static IServiceCollection AddBackgroundServices(this IServiceCollection services)
        {
            services.AddHostedService<AppBackgroundService>();

            return services;
        }
    }
}
