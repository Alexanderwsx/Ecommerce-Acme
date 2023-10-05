using ECommerce_Template_MVC.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ECommerce_Template_MVC.Utility.Tests
{
    [TestClass]
    public class BoxTests
    {
        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void DetermineSuitableBox_WithAllOversizedProducts_ThrowsException()
        {
            // Arrange
            var products = new List<Product>
            {
            new Product { Length = 300, Width = 50, Height = 50, Weight = 5000 },
            new Product { Length = 280, Width = 45, Height = 45, Weight = 4000 }
            };

            // Act
            Box.DetermineSuitableBoxes(products);
        }

        [TestMethod]
        public void DetermineSuitableBox_WithMultipleProducts_ReturnsMultipleBoxes()
        {
            // Arrange
            var products = new List<Product>

            {
               new Product { Length = 10, Width = 5, Height = 5, Weight = 5000 },
               new Product { Length = 60, Width = 40, Height = 30, Weight = 10000 },
               new Product { Length = 25, Width = 20, Height = 10, Weight = 7000 }
            };

            // Act
            var resultBox = Box.DetermineSuitableBoxes(products);

            // Assert
            Assert.IsNotNull(resultBox);
            Assert.AreEqual(3, resultBox.Count); // Espera 3 cajas
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void DetermineSuitableBox_WithOversizedProduct_ThrowsException()
        {
            // Arrange
            var products = new List<Product>
                {
                        new Product { Length = 300, Width = 5, Height = 5, Weight = 5000 },  // Este producto es demasiado largo
		  };

            // Act
            Box.DetermineSuitableBoxes(products);
            // La prueba espera que se lance una excepción debido al tamaño del producto
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void DetermineSuitableBox_WithOverweightedProduct_ThrowsException()
        {
            // Arrange
            var products = new List<Product>
                            {
                                new Product { Length = 10, Width = 5, Height = 5, Weight = 69000 },  // Este producto es demasiado pesado
							};

            // Act
            Box.DetermineSuitableBoxes(products);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void DetermineSuitableBox_WithProductsExceedingGirth_ThrowsException()
        {
            // Arrange
            var products = new List<Product>
                {
                    new Product { Length = 100, Width = 30, Height = 30, Weight = 5000 }  // Circunferencia combinada es 220 que está bien, pero junto con la longitud excede 165 pulgadas
				};

            // Act
            Box.DetermineSuitableBoxes(products);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void DetermineSuitableBox_WithSomeOversizedProducts_ThrowsException()
        {
            // Arrange
            var products = new List<Product>
                {
                    new Product { Length = 10, Width = 5, Height = 5, Weight = 5000 },
                    new Product { Length = 280, Width = 45, Height = 45, Weight = 4000 }
                };

            // Act
            Box.DetermineSuitableBoxes(products);
        }

        [TestMethod]
        public void DetermineSuitableBox_WithValidProducts_ReturnsBox()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { Length = 10, Width = 5, Height = 5, Weight = 5000 },  // 5kg, fits in first box
				new Product { Length = 20, Width = 10, Height = 10, Weight = 6000 }, // 6kg, fits in second box
			};

            // Act
            var resultBox = Box.DetermineSuitableBoxes(products);

            // Assert
            Assert.IsNotNull(resultBox);
            // Puedes agregar más assertions según lo que quieras verificar
        }
    }
}