﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ApplicationCore.Entities.OrderAggregate;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using Dama.API.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dama.API.Controllers
{
    [Produces("application/json")]
    [Route("api/v1/orders")]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IBasketRepository _repository;

        public OrderController(IOrderService orderService, IBasketRepository basketRepository)
        {
            _orderService = orderService;
            _repository = basketRepository;
        }

        [Route("all/{buyerId}")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Order>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetOrders(string buyerId)
        {
            var orders = await _orderService.GetOrdersAsync(buyerId);

            return Ok(orders);
        }

        [Route("{id}")]
        [HttpGet]
        [ProducesResponseType(typeof(Order), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetOrder(int id)
        {
            var order = await _orderService.GetOrderAsync(id);

            return Ok(order);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Order), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Post([FromBody]OrderViewModel model)
        {
            //Get Basket
            var basketSpec = new BasketWithItemsSpecification(model.BuyerId);
            var basket = (await _repository.ListAsync(basketSpec)).LastOrDefault();
            if (basket == null)
            {
                return NotFound();
            }

            //Create Address
            Address address = new Address(null, null, model.Street, model.City, model.Country, model.PostalCode);            
            var order = await _orderService.CreateOrderAsync(basket.Id,null,address,null,true,0);
            return Ok(order);
        }
    }
}