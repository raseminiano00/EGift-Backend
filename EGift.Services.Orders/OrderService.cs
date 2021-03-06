﻿namespace EGift.Services.Orders
{
    using System;
    using System.Threading.Tasks;
    using EGift.Infrastructure.Common;
    using EGift.Infrastructure.Common.ResponseHandlers;
    using EGift.Services.Email;
    using EGift.Services.Email.Exceptions;
    using EGift.Services.Email.Messages;
    using EGift.Services.Orders.Data.Gateways;
    using EGift.Services.Orders.Exceptions;
    using EGift.Services.Orders.Extensions;
    using EGift.Services.Orders.Messages;

    public class OrderService : IOrderService
    {
        private IOrderDataGateway gateway;
        private IResponseHandlerFacade responseHandler;
        private IEmailService emailService;

        public OrderService(IOrderDataGateway gateway,IResponseHandlerFacade responseHandler,IEmailService emailService)
        {
            if (gateway == null || responseHandler == null)
            {
                throw new OrderServiceException(new Exception());
            }

            this.emailService = emailService;
            this.gateway = gateway;
            this.responseHandler = responseHandler;
        }

        public async Task<GetAllOrderResponse> GetAllOrderAsync()
        {
            GetAllOrderResponse result;
            try
            {
                result = await this.gateway.GetAllOrderAsync();

                result.Code = responseHandler.Handle(result);

                return result;
            }
            catch (Exception ex)
            {
                throw new OrderServiceException(ex);
            }
        }

        public async Task<NewOrderResponse> NewOrderAsync(NewOrderRequest request)
        {
            NewOrderResponse result;
            try
            {
                result = await this.gateway.NewOrderAsync(request);

                result.Code = responseHandler.Handle(result);

                if (result.Code == 200)
                {
                    var emailResponse = await this.emailService.SendEmailAsync(request.AsEmailRequest());

                    if (emailResponse.ApiResponse != "OK")
                        throw new EmailServiceException(new Exception());
                }


                return result;
            }
            catch (Exception ex)
            {
                throw new OrderServiceException(ex);
            }
        }

        public async Task<SearchOrderResponse> SearchOrderAsync(SearchOrderRequest request)
        {
            SearchOrderResponse result;
            try
            {
                result = await this.gateway.SearchOrderAsync(request);

                result.Code = responseHandler.Handle(result);

                return result;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                result = new SearchOrderResponse
                {
                    Code = 404
                };
                return result;
            }
        }
    }
}
