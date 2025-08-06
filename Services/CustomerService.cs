using NHT_Marine_BE.Data.Dtos.Auth;
using NHT_Marine_BE.Data.Dtos.Order;
using NHT_Marine_BE.Data.Dtos.Response;
using NHT_Marine_BE.Data.Dtos.User;
using NHT_Marine_BE.Data.Queries;
using NHT_Marine_BE.Enums;
using NHT_Marine_BE.Interfaces.Repositories;
using NHT_Marine_BE.Interfaces.Services;
using NHT_Marine_BE.Models.User;
using NHT_Marine_BE.Utilities;

namespace NHT_Marine_BE.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepo;
        private readonly ICartRepository _cartRepo;
        private readonly IProductRepository _productRepo;
        private readonly IRoleRepository _roleRepo;
        private readonly IAccountRepository _accountRepo;

        public CustomerService(
            ICustomerRepository customerRepo,
            ICartRepository cartRepo,
            IProductRepository productRepo,
            IRoleRepository roleRepo,
            IAccountRepository accountRepo
        )
        {
            _customerRepo = customerRepo;
            _cartRepo = cartRepo;
            _productRepo = productRepo;
            _roleRepo = roleRepo;
            _accountRepo = accountRepo;
        }

        public async Task<ServiceResponse> UpdateCustomerProfile(UpdateUserDto updateDto, int authUserId)
        {
            var targetCustomer = await _customerRepo.GetCustomerById(authUserId);
            if (targetCustomer == null)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.NOT_FOUND,
                    Success = false,
                    Message = ErrorMessage.USER_NOT_FOUND,
                };
            }

            targetCustomer.FullName = updateDto.FullName;
            targetCustomer.Avatar = updateDto.Avatar;

            var customerWithThisEmail = await _customerRepo.GetCustomerByEmail(updateDto.Email);
            if (customerWithThisEmail != null && customerWithThisEmail.CustomerId != authUserId)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.CONFLICT,
                    Success = false,
                    Message = ErrorMessage.EMAIL_EXISTED,
                };
            }

            targetCustomer.Email = updateDto.Email;

            await _customerRepo.UpdateCustomer(targetCustomer);

            return new ServiceResponse
            {
                Status = ResStatusCode.OK,
                Success = true,
                Message = SuccessMessage.UPDATE_USER_SUCCESSFULLY,
            };
        }

        public async Task<ServiceResponse<CustomerCart?>> GetCustomerCart(int authUserId)
        {
            var cart = await _cartRepo.GetCustomerActiveCart(authUserId);

            return new ServiceResponse<CustomerCart?>
            {
                Status = ResStatusCode.OK,
                Success = true,
                Data = cart,
            };
        }

        public async Task<ServiceResponse> AddCartItem(AddCartItemDto addDto, int authUserId)
        {
            bool isCartItemAdded = false;
            var activeCart = await _cartRepo.GetCustomerActiveCart(authUserId);
            var currentStock = await _productRepo.GetProductItemCurrentStock(addDto.ProductItemId);

            if (activeCart == null)
            {
                if (addDto.Quantity > currentStock)
                {
                    return new ServiceResponse
                    {
                        Status = ResStatusCode.BAD_REQUEST,
                        Success = false,
                        Message = ErrorMessage.QUANTITY_EXCEED_CURRENT_STOCK,
                    };
                }

                var newCart = new CustomerCart { CustomerId = authUserId, Items = [] };
                newCart.Items.Add(new CartItem { ProductItemId = addDto.ProductItemId, Quantity = addDto.Quantity });

                await _cartRepo.AddCustomerCart(newCart);
                isCartItemAdded = true;
            }
            else
            {
                var existedCartItem = activeCart.Items.Find(ci => ci.ProductItemId == addDto.ProductItemId);
                if (existedCartItem == null)
                {
                    if (addDto.Quantity > currentStock)
                    {
                        return new ServiceResponse
                        {
                            Status = ResStatusCode.BAD_REQUEST,
                            Success = false,
                            Message = ErrorMessage.QUANTITY_EXCEED_CURRENT_STOCK,
                        };
                    }

                    activeCart.Items.Add(new CartItem { ProductItemId = addDto.ProductItemId, Quantity = addDto.Quantity });

                    await _cartRepo.UpdateCustomerCart(activeCart);
                    isCartItemAdded = true;
                }
                else
                {
                    if (addDto.Quantity > currentStock - existedCartItem.Quantity)
                    {
                        return new ServiceResponse
                        {
                            Status = ResStatusCode.BAD_REQUEST,
                            Success = false,
                            Message = ErrorMessage.QUANTITY_EXCEED_CURRENT_STOCK,
                        };
                    }

                    existedCartItem.Quantity += addDto.Quantity;

                    await _cartRepo.UpdateCartItem(existedCartItem);
                }

                activeCart.UpdatedAt = TimestampHandler.GetNow();
                await _cartRepo.UpdateCustomerCart(activeCart);
            }

            return new ServiceResponse
            {
                Status = isCartItemAdded ? ResStatusCode.CREATED : ResStatusCode.OK,
                Success = true,
                Message = SuccessMessage.ADD_TO_CART_SUCCESSFULLY,
            };
        }

        public async Task<ServiceResponse> UpdateCartItem(UpdateCartItemDto updateDto, int productItemId, int authUserId)
        {
            var activeCart = await _cartRepo.GetCustomerActiveCart(authUserId);
            if (activeCart == null)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.NOT_FOUND,
                    Success = false,
                    Message = ErrorMessage.CART_NOT_FOUND,
                };
            }

            var cartItem = activeCart.Items.Find(ci => ci.ProductItemId == productItemId);
            if (cartItem == null)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.NOT_FOUND,
                    Success = false,
                    Message = ErrorMessage.CART_ITEM_NOT_FOUND,
                };
            }

            if (updateDto.NewProductItemId != null && updateDto.NewProductItemId != productItemId)
            {
                var newProductStock = await _productRepo.GetProductItemCurrentStock((int)updateDto.NewProductItemId);
                if (updateDto.Quantity > newProductStock)
                {
                    return new ServiceResponse
                    {
                        Status = ResStatusCode.BAD_REQUEST,
                        Success = false,
                        Message = ErrorMessage.QUANTITY_EXCEED_CURRENT_STOCK,
                    };
                }

                var existedCartItem = activeCart.Items.Find(ci => ci.ProductItemId == updateDto.NewProductItemId);
                if (existedCartItem == null)
                {
                    activeCart.Items.Add(new CartItem { ProductItemId = updateDto.NewProductItemId, Quantity = updateDto.Quantity });
                }
                else
                {
                    existedCartItem.Quantity = updateDto.Quantity;
                    await _cartRepo.UpdateCartItem(existedCartItem);
                }

                activeCart.Items.Remove(cartItem);
            }
            else
            {
                var currentStock = await _productRepo.GetProductItemCurrentStock(productItemId);
                if (updateDto.Quantity > currentStock)
                {
                    return new ServiceResponse
                    {
                        Status = ResStatusCode.BAD_REQUEST,
                        Success = false,
                        Message = ErrorMessage.QUANTITY_EXCEED_CURRENT_STOCK,
                    };
                }

                cartItem.Quantity = updateDto.Quantity;

                await _cartRepo.UpdateCartItem(cartItem);
            }

            activeCart.UpdatedAt = TimestampHandler.GetNow();
            await _cartRepo.UpdateCustomerCart(activeCart);

            return new ServiceResponse
            {
                Status = ResStatusCode.OK,
                Success = true,
                Message = SuccessMessage.UPDATE_CART_SUCCESSFULLY,
            };
        }

        public async Task<ServiceResponse> RemoveCartItem(int productItemId, int authUserId)
        {
            var activeCart = await _cartRepo.GetCustomerActiveCart(authUserId);
            if (activeCart == null)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.NOT_FOUND,
                    Success = false,
                    Message = ErrorMessage.CART_NOT_FOUND,
                };
            }

            var cartItem = activeCart.Items.Find(ci => ci.ProductItemId == productItemId);
            if (cartItem == null)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.NOT_FOUND,
                    Success = false,
                    Message = ErrorMessage.CART_ITEM_NOT_FOUND,
                };
            }

            activeCart.UpdatedAt = TimestampHandler.GetNow();
            activeCart.Items.Remove(cartItem);
            await _cartRepo.UpdateCustomerCart(activeCart);

            return new ServiceResponse
            {
                Status = ResStatusCode.OK,
                Success = true,
                Message = SuccessMessage.UPDATE_CART_SUCCESSFULLY,
            };
        }

        public async Task<ServiceResponse> ResetCustomerCart(int authUserId)
        {
            var activeCart = await _cartRepo.GetCustomerActiveCart(authUserId);
            if (activeCart == null)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.NOT_FOUND,
                    Success = false,
                    Message = ErrorMessage.CART_NOT_FOUND,
                };
            }

            activeCart.UpdatedAt = TimestampHandler.GetNow();
            activeCart.Status = CartStatus.Abandoned;
            await _cartRepo.UpdateCustomerCart(activeCart);

            return new ServiceResponse
            {
                Status = ResStatusCode.OK,
                Success = true,
                Message = SuccessMessage.RESET_CART_SUCCESSFULLY,
            };
        }

        public async Task<ServiceResponse<List<Customer>>> GetAllCustomers(BaseQueryObject queryObject)
        {
            var (customers, total) = await _customerRepo.GetAllCustomers(queryObject);

            return new ServiceResponse<List<Customer>>
            {
                Status = ResStatusCode.OK,
                Success = true,
                Data = customers,
                Total = total,
                Took = customers.Count,
            };
        }

        public async Task<ServiceResponse> DeactivateCustomerAccount(int targetCustomerId, int authRoleId)
        {
            var hasDeactivateCustomerPermission = await _roleRepo.VerifyPermission(
                authRoleId,
                Permission.DEACTIVATE_CUSTOMER_ACCOUNT.ToString()
            );
            if (!hasDeactivateCustomerPermission)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.FORBIDDEN,
                    Success = false,
                    Message = ErrorMessage.NO_PERMISSION,
                };
            }

            var targetCustomer = await _customerRepo.GetCustomerById(targetCustomerId);
            if (targetCustomer == null)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.NOT_FOUND,
                    Success = false,
                    Message = ErrorMessage.USER_NOT_FOUND,
                };
            }

            var targetAccount = await _accountRepo.GetAccountByUserId(targetCustomerId, true);
            if (targetAccount == null)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.NOT_FOUND,
                    Success = false,
                    Message = ErrorMessage.USER_NOT_FOUND,
                };
            }

            targetAccount.IsActive = false;
            await _accountRepo.UpdateAccount(targetAccount);

            return new ServiceResponse
            {
                Status = ResStatusCode.OK,
                Success = true,
                Message = SuccessMessage.DEACTIVATE_CUSTOMER_SUCCESSFULLY,
            };
        }
    }
}
