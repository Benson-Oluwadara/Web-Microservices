using AutoMapper;
using Mango.Services.OrderAPI.Models;
using Mango.Services.OrderAPI.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

//using Mango.MessageBus;
using Mango.Services.OrderAPI.Service.IService;
using Mango.Services.OrderAPI.Database.IDapperRepositorys;
using mango.messagebus;
using Mango.Services.OrderAPI.Models.DTO;
using System.Net;

namespace Mango.Services.OrderAPI.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class OrderAPIController : ControllerBase
    {

        private IMapper _mapper;
        private IProductService _productService;
        private readonly IOrderService _orderService;
        private readonly IMessageBus _messageBus;
        private readonly IConfiguration _configuration;
        public OrderAPIController(
            IProductService productService, IMapper mapper, IConfiguration configuration
            , IMessageBus messageBus, IOrderService orderService)
        {
            _messageBus = messageBus;
            _productService = productService;
            _mapper = mapper;
            _configuration = configuration;
            _orderService = orderService;
        }

        //[Authorize]
        [HttpGet("GetOrders")]
        public async Task<IActionResult> Get(string? userId = "")
        {
            try
            {
                IEnumerable<OrderHeader> orders;

                if (string.IsNullOrEmpty(userId) || User.IsInRole(SD.RoleAdmin))
                {
                    // Get all orders if userId is not provided or if the user is an admin
                    orders = (IEnumerable<OrderHeader>)await _orderService.GetAllOrdersAsync();
                }
                else
                {
                    // Get user-specific orders
                    orders = await _orderService.GetUserOrdersAsync(userId);
                }

                // Map orders to DTOs
                var orderDtos = _mapper.Map<IEnumerable<OrderHeaderDTO>>(orders);


                return Ok(new APIResponse(HttpStatusCode.OK, true, orderDtos));
            }
            catch (Exception ex)
            {
                // Handle exceptions
                // Log the exception or handle it as needed
                // For now, return an internal server error response
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new APIResponse(HttpStatusCode.InternalServerError, false, null, new List<string> { ex.Message }));
            }
        }


        [Authorize]
        [HttpGet("GetOrder/{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                // Call the service method to get the order by ID
                var orderHeader = await _orderService.GetOrderByIdAsync(id);

                if (orderHeader == null)
                {
                    return NotFound(); // Return 404 if order not found
                }

                // Map the order header to DTO
                var orderHeaderDto = _mapper.Map<OrderHeaderDTO>(orderHeader);

                // Return the mapped DTO
                return Ok(new APIResponse(HttpStatusCode.OK, true, orderHeaderDto));
            }
            catch (Exception ex)
            {
                // Handle exceptions and return appropriate error response
                return StatusCode(StatusCodes.Status500InternalServerError, new APIResponse(HttpStatusCode.InternalServerError, false, null, new List<string> { ex.Message }));
            }
        }



        [Authorize]
        [HttpPost("CreateOrder")]
        public async Task<IActionResult> CreateOrder([FromBody] CartDTO cartDto)
        {
            try
            {
                OrderHeaderDTO orderHeaderDto = _mapper.Map<OrderHeaderDTO>(cartDto.CartHeader);
                orderHeaderDto.OrderTime = DateTime.Now;
                orderHeaderDto.Status = SD.Status_Pending;
                orderHeaderDto.OrderDetails = _mapper.Map<IEnumerable<OrderDetailsDTO>>(cartDto.CartDetails);
                orderHeaderDto.OrderTotal = Math.Round(orderHeaderDto.OrderTotal, 2);

                OrderHeader orderHeader = _mapper.Map<OrderHeader>(orderHeaderDto);

                int insertedOrderHeaderId = await _orderService.InsertOrderHeaderAsync(orderHeader);

                orderHeaderDto.OrderHeaderId = insertedOrderHeaderId;
                return Ok(orderHeaderDto); // Return 200 OK with the created order header DTO
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message); // Return 500 Internal Server Error with the exception message
            }
        }
    }
}


//        [Authorize]
//        [HttpPost("CreateStripeSession")]
//        public async Task<ResponseDto> CreateStripeSession([FromBody] StripeRequestDto stripeRequestDto)
//        {
//            try
//            {

//                var options = new SessionCreateOptions
//                {
//                    SuccessUrl = stripeRequestDto.ApprovedUrl,
//                    CancelUrl = stripeRequestDto.CancelUrl,
//                    LineItems = new List<SessionLineItemOptions>(),
//                    Mode = "payment",

//                };

//                var DiscountsObj = new List<SessionDiscountOptions>()
//                {
//                    new SessionDiscountOptions
//                    {
//                        Coupon=stripeRequestDto.OrderHeader.CouponCode
//                    }
//                };

//                foreach (var item in stripeRequestDto.OrderHeader.OrderDetails)
//                {
//                    var sessionLineItem = new SessionLineItemOptions
//                    {
//                        PriceData = new SessionLineItemPriceDataOptions
//                        {
//                            UnitAmount = (long)(item.Price * 100), // $20.99 -> 2099
//                            Currency = "usd",
//                            ProductData = new SessionLineItemPriceDataProductDataOptions
//                            {
//                                Name = item.Product.Name
//                            }
//                        },
//                        Quantity = item.Count
//                    };

//                    options.LineItems.Add(sessionLineItem);
//                }

//                if (stripeRequestDto.OrderHeader.Discount > 0)
//                {
//                    options.Discounts = DiscountsObj;
//                }
//                var service = new SessionService();
//                Session session = service.Create(options);
//                stripeRequestDto.StripeSessionUrl = session.Url;
//                OrderHeader orderHeader = _db.OrderHeaders.First(u => u.OrderHeaderId == stripeRequestDto.OrderHeader.OrderHeaderId);
//                orderHeader.StripeSessionId = session.Id;
//                _db.SaveChanges();
//                _response.Result = stripeRequestDto;

//            }
//            catch (Exception ex)
//            {
//                _response.Message = ex.Message;
//                _response.IsSuccess = false;
//            }
//            return _response;
//        }


//        [Authorize]
//        [HttpPost("ValidateStripeSession")]
//        public async Task<ResponseDto> ValidateStripeSession([FromBody] int orderHeaderId)
//        {
//            try
//            {

//                OrderHeader orderHeader = _db.OrderHeaders.First(u => u.OrderHeaderId == orderHeaderId);

//                var service = new SessionService();
//                Session session = service.Get(orderHeader.StripeSessionId);

//                var paymentIntentService = new PaymentIntentService();
//                PaymentIntent paymentIntent = paymentIntentService.Get(session.PaymentIntentId);

//                if (paymentIntent.Status == "succeeded")
//                {
//                    //then payment was successful
//                    orderHeader.PaymentIntentId = paymentIntent.Id;
//                    orderHeader.Status = SD.Status_Approved;
//                    _db.SaveChanges();
//                    RewardsDto rewardsDto = new()
//                    {
//                        OrderId = orderHeader.OrderHeaderId,
//                        RewardsActivity = Convert.ToInt32(orderHeader.OrderTotal),
//                        UserId = orderHeader.UserId
//                    };
//                    string topicName = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic");
//                    await _messageBus.PublishMessage(rewardsDto, topicName);
//                    _response.Result = _mapper.Map<OrderHeaderDto>(orderHeader);
//                }

//            }
//            catch (Exception ex)
//            {
//                _response.Message = ex.Message;
//                _response.IsSuccess = false;
//            }
//            return _response;
//        }


//        [Authorize]
//        [HttpPost("UpdateOrderStatus/{orderId:int}")]
//        public async Task<ResponseDto> UpdateOrderStatus(int orderId, [FromBody] string newStatus)
//        {
//            try
//            {
//                OrderHeader orderHeader = _db.OrderHeaders.First(u => u.OrderHeaderId == orderId);
//                if (orderHeader != null)
//                {
//                    if (newStatus == SD.Status_Cancelled)
//                    {
//                        //we will give refund
//                        var options = new RefundCreateOptions
//                        {
//                            Reason = RefundReasons.RequestedByCustomer,
//                            PaymentIntent = orderHeader.PaymentIntentId
//                        };

//                        var service = new RefundService();
//                        Refund refund = service.Create(options);
//                    }
//                    orderHeader.Status = newStatus;
//                    _db.SaveChanges();
//                }
//            }
//            catch (Exception ex)
//            {
//                _response.IsSuccess = false;
//            }
//            return _response;
//        }
//    }
//}