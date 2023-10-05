using ECommerce_Template_MVC.Models;
using System.Runtime.Intrinsics.X86;

namespace ECommerce_Template_MVC.Utility
{
    /*UPS Dimensions
    What is the maximum size combination for small package services?
    Packages can be up to 165 inches(419 cm) in length and girth combined.
    To accurately determine length and girth, use the formula: Length + 2x Width + 2x Height
    Remember, the longest side of your package will be your length.
    Packages can be up to 150 lb (68 kg).
    Packages can be up to 108 inches long (274 cm).
    */

    public class Box
    {
        public decimal Height { get; set; } //cm
        public decimal Length { get; set; } //cm

        public decimal TotalWeight { get; set; }  // Nuevo campo

        public decimal Width { get; set; }//cm

        public static List<Box> DetermineSuitableBoxes(List<Product> products)
        {
            if (products == null || !products.Any())
            {
                throw new ArgumentException("La lista de productos no puede estar vacía");
            }

            List<Box> standardBoxes = new List<Box>
            {
                new Box { Length = 20, Width = 15, Height = 10 },
                new Box { Length = 25, Width = 20, Height = 10 },
                new Box { Length = 30, Width = 20, Height = 15 },
                new Box { Length = 35, Width = 25, Height = 15 },
                new Box { Length = 40, Width = 30, Height = 25 },
                new Box { Length = 50, Width = 40, Height = 30 },
                new Box { Length = 60, Width = 40, Height = 40 },
                new Box { Length = 70, Width = 50, Height = 50 }
            };

            List<Box> boxesUsed = new List<Box>();

            // Copia de la lista original de productos para evitar la modificación de la lista original
            var productsToPack = new List<Product>(products);

            while (productsToPack.Any())
            {
                bool productPlaced = false;

                foreach (var box in standardBoxes)
                {
                    decimal boxVolumeRemaining = box.Volume();
                    decimal boxWeightRemaining = 68000; // 150 lb in grams

                    List<Product> productsAssignedToBox = new List<Product>();

                    foreach (var product in productsToPack.ToList()) // ToList() para crear una copia y evitar errores de modificación durante la iteración
                    {
                        decimal productVolume = product.Length * product.Width * product.Height;

                        if (product.Length <= box.Length &&
                            product.Width <= box.Width &&
                            product.Height <= box.Height &&
                            productVolume <= boxVolumeRemaining &&
                            product.Weight <= boxWeightRemaining)
                        {
                            boxVolumeRemaining -= productVolume;
                            boxWeightRemaining -= product.Weight;

                            // Aquí, añade el peso del producto al peso total de la caja
                            box.TotalWeight += product.Weight;

                            productsAssignedToBox.Add(product);
                            productsToPack.Remove(product);
                            productPlaced = true;
                        }
                    }

                    if (productsAssignedToBox.Any())
                    {
                        boxesUsed.Add(box);
                    }
                }

                // Si un producto no pudo ser colocado en ninguna caja, lanzar una excepción
                if (!productPlaced)
                {
                    throw new Exception("Uno o más productos no pueden ser empaquetados en las cajas disponibles.");
                }
            }

            return boxesUsed;
        }

        public decimal CalculateGirth()
        {
            return Length + 2 * Width + 2 * Height;
        }

        public bool IsLengthExceedingLimit()
        {
            return Length > 274; // 108 inches in cm
        }

        public decimal Volume()
        {
            return Length * Width * Height;
        }
    }
}