﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ApplicationCore.Entities.OrderAggregate;
using ApplicationCore.Interfaces;
using ApplicationCore.Services;
using ApplicationCore.Specifications;
using Dama.API.Interfaces;
using Dama.API.Services;
using Dama.API.ViewModel;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Dama.API.Controllers
{
    [Produces("application/json")]
    [Route("api/v1/orders")]
    [Route("api/grocery/orders")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderDamaService;
        private readonly IOrderGroceryService _orderGroceryService;
        private readonly IDamaRepository _damaRepository;
        private readonly IGroceryRepository _groceryRepository;
        //private readonly IBasketRepository _repository;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderController(IOrderService orderService, IOrderGroceryService orderGroceryService, IDamaRepository damaRepo,
           IGroceryRepository groceryRepo, UserManager<ApplicationUser> userManager)
        {
            _orderDamaService = orderService;
            _orderGroceryService = orderGroceryService;
            _damaRepository = damaRepo;
            _groceryRepository = groceryRepo;
            _userManager = userManager;
        }

        private IOrderService OrderService
        {
            get
            {
                return Request.Path.Value.Contains("grocery") ? _orderGroceryService : _orderDamaService;
            }
        }

        private IDamaRepository Repository
        {
            get
            {
                return Request.Path.Value.Contains("grocery") ? _groceryRepository : _damaRepository;
            }
        }

        [Route("all/{userId}")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Order>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetOrders(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            var orders = new List<Order>();
            if (user.UserName == "sue@damanojornal.com" || user.UserName == "jue@damanojornal.com" || user.UserName == "sonia@damanojornal.com")
            {
                orders = await OrderService.GetOrdersAsync("sue@damanojornal.com");
                orders.AddRange(await OrderService.GetOrdersAsync("jue@damanojornal.com"));
                orders.AddRange(await OrderService.GetOrdersAsync("sonia@damanojornal.com"));
            }
            else
            {
                orders = await OrderService.GetOrdersAsync(user.Email);
            }



            return Ok(orders);
        }

        [Route("{id}")]
        [HttpGet]
        [ProducesResponseType(typeof(Order), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetOrder(int id)
        {
            var order = await OrderService.GetOrderAsync(id);

            return Ok(order);
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> Post([FromBody]OrderViewModel model)
        {
            //Get Basket
            var basketSpec = new BasketWithItemsSpecification(model.BuyerId);
            var basket = (await Repository.ListAsync(basketSpec)).LastOrDefault();
            if (basket == null)
            {
                return NotFound();
            }

            //Create Address
            Address address = new Address(null, model.ShippingStreet, model.ShippingCity, model.ShippingCountry, model.ShippingZipCode);
            Address billAddress = new Address(model.BillingName, model.BillingStreet, model.BillingCity, model.BillingCountry, model.BillingPostalCode);
            var order = await OrderService.CreateOrderAsync(basket.Id, null, model.TaxNumber, address, billAddress, true, 0, model.CustomerEmail);

            //Update to Submitted
            await OrderService.UpdateOrderState(order.Id, OrderStateType.SUBMITTED);
            return Ok();
        }

        [Route("cancel")]
        [HttpPut]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CancelOrder([FromBody]CancelOrderViewModel command)
        {
            await OrderService.UpdateOrderState(command.OrderNumber, OrderStateType.CANCELED);
            return Ok();

        }

        [Route("place/{placeId}")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Order>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetOrdersByPlace(int placeId)
        {
            var street = PlaceIdToStreet(placeId);
            var orders = new List<Order>();
            orders = (await OrderService
                .GetOrdersAsync("sue@damanojornal.com"))
                .Where(o => o.ShipToAddress.Street.Equals(street))
                .ToList();
            orders.AddRange((await OrderService
                .GetOrdersAsync("jue@damanojornal.com"))
                .Where(o => o.ShipToAddress.Street.Equals(street))
                .ToList());
            orders.AddRange((await OrderService
                .GetOrdersAsync("sonia@damanojornal.com"))
                .Where(o => o.ShipToAddress.Street.Equals(street))
                .ToList());

            return Ok(orders);
        }

        private object PlaceIdToStreet(int placeId)
        {
            if (placeId == 1)
                return "Feira Popular de Loulé";
            if (placeId == 2)
                return "Feira da Serra de São Brás";
            return string.Empty;
        }
    }
}