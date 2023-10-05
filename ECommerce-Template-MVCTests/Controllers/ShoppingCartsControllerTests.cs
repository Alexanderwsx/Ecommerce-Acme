using Microsoft.VisualStudio.TestTools.UnitTesting;
using ECommerce_Template_MVC.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce_Template_MVC.Models.ViewModel;
using ECommerce_Template_MVC.Models;

namespace ECommerce_Template_MVC.Controllers.Tests
{
    [TestClass()]
    public class ShoppingCartsControllerTests
    {
        private ShoppingCartsController _controller;

        [TestMethod()]
        public void CreateShipmentTest_EmptyCart()
        {
            var cartVM = new ShoppingCartVM();
            // Esperar una excepción si el carrito está vacío
            Assert.ThrowsException<NullReferenceException>(() => _controller.CreateShipment(cartVM));
        }

        [TestMethod()]
        public void CreateShipmentTest_MultipleProducts()
        {
            var cartVM = new ShoppingCartVM
            {
                ListCart = new List<ShoppingCart>
                {
                    new ShoppingCart
                    {
                        Product = new Product { Length = 20, Width = 10, Height = 5 }
                    },
                    new ShoppingCart
                    {
                        Product = new Product { Length = 25, Width = 20, Height = 10 }
                    }
                }
            };

            var shipment = _controller.CreateShipment(cartVM);
            Assert.IsNotNull(shipment);
        }

        [TestMethod()]
        public void CreateShipmentTest_NullOrderHeader()
        {
            var cartVM = new ShoppingCartVM
            {
                ListCart = new List<ShoppingCart>
                {
                    new ShoppingCart
                    {
                        Product = new Product { Length = 20, Width = 10, Height = 5 }
                    }
                },
                OrderHeader = null
            };

            // Asume que tu método requiere OrderHeader para ser no nulo
            Assert.ThrowsException<Exception>(() => _controller.CreateShipment(cartVM));
        }

        [TestMethod()]
        public void CreateShipmentTest_ProductExceedsBox()
        {
            var cartVM = new ShoppingCartVM
            {
                ListCart = new List<ShoppingCart>
                {
                    new ShoppingCart
                    {
                        Product = new Product { Length = 300, Width = 50, Height = 50 } // Dimensiones que exceden las cajas disponibles
                    }
                }
            };

            Assert.ThrowsException<Exception>(() => _controller.CreateShipment(cartVM));
        }

        [TestMethod()]
        public void CreateShipmentTest_SingleProduct()
        {
            var cartVM = new ShoppingCartVM
            {
                ListCart = new List<ShoppingCart>
                {
                    new ShoppingCart
                    {
                        Product = new Product { Length = 20, Width = 10, Height = 5 }
                    }
                }
            };

            var shipment = _controller.CreateShipment(cartVM);
            Assert.IsNotNull(shipment);
        }
    }
}