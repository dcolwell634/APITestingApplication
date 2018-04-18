using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BasicWebAPI1.Controllers;
using BasicWebAPI1.Models;
using System.Collections.Generic;

namespace BasicWebAPI1.Test
{
    [TestClass]
    public class UnitTest1
    {
        /// <summary>
        /// Test Initialization
        /// </summary>
        ProductsController controller;
        Product mazda = new Product() { Id = 6, Name = "Mazda 2", Description = "Small compact model" };

        [TestInitialize]
        public void TestInitialize()
        {
            controller = new ProductsController();            
        }

        /// <summary>
        /// Gets list of products
        /// </summary>
        [TestMethod]
        [TestCategory ("Unit")]
        public void GetProductReturnsProducts()
        {
            //Convert to list so we can count products returned
            List<Product> productList = (List<Product>)controller.GetProducts();
            //Assert we have 5 products
            Assert.IsTrue(productList.Count == 5, "Should have 5 products returned.");
        }

        /// <summary>
        /// Gets products by id passed and confirms correct product was returned
        /// </summary>
        [TestMethod]
        [TestCategory("Unit")]
        public void GetProductByIdReturnsCorrectProduct()
        {
            int id = 1;
            var hondaCivic = controller.GetProductById(id);

            Assert.IsTrue(hondaCivic.Id == 1, "Id returned should be 1");
            Assert.IsTrue(hondaCivic.Name == "Honda Civic", "Product with ID of 1 should be Honda Civic");
            Assert.IsTrue(hondaCivic.Description == "Luxury Model 2013", "Description should be \"Luxury Model 2013\"");
        }

        /// <summary>
        /// Searches for product based on description and verifies correct product was returned
        /// </summary>
        [TestMethod]
        [TestCategory("Unit")]
        public void GetProductBySearchReturnsCorrectProduct()
        {
            string searchText = "Luxury Model 2013";
            var productList = controller.GetProductsBySearch(searchText);

            try
            {
                //Probably not the most efficient way of doing this, but its an enumerable so I needed a way to iterate through. 
                //This does however introduce a scenario where the asserts could never be hit. Hence I wrapped it in an try catch
                foreach (Product product in productList)
                {
                    Assert.IsTrue(product.Name == "Honda Civic", "Incorrect car model returned");
                    Assert.IsTrue(product.Id == 1, "Incorrect car model returned");
                }
            }
            catch (Exception ex)
            {
                Assert.Fail();
            }
        }

        /// <summary>
        /// Adds product to the list
        /// </summary>
        [TestMethod]
        [TestCategory("Unit")]
        public void PostProductAddsProductToProducts()
        {
            var newProduct = controller.PostProduct(mazda);
            List<Product> productList = (List<Product>)controller.GetProducts();

            Assert.IsTrue(productList.Count == 6, "We should now have 6 products in our product list");
        }

        /// <summary>
        /// Update product and verify the updated description and name is correct
        /// </summary>
        [TestMethod]
        [TestCategory("Unit")]
        public void PutProductUpdatesProduct()
        {
            string newName = "Mazda 3";
            string newDescription = "Similar to Mazda 2 but with more features";
            mazda.Name = newName;
            mazda.Description = newDescription;
            var updateMazda = controller.PutProduct(mazda);
            var updatedMazda = controller.GetProductById(mazda.Id);

            Assert.IsTrue(updatedMazda.Id == 6, "Id returned should be 6");
            Assert.IsTrue(updatedMazda.Name == newName, "Id returned should be 6");
            Assert.IsTrue(updatedMazda.Description == newDescription, "Id returned should be 6");

        }

        /// <summary>
        /// Delete the previously added product
        /// </summary>
        [TestMethod]
        [TestCategory("Unit")]
        public void DeleteProductDeletsProduct()
        {
            int id = 6;
            var deletedProduct = controller.DeleteProduct(id);
            List<Product> productList = (List<Product>)controller.GetProducts();

            Assert.IsTrue(productList.Count == 5, "Should have 5 products returned.");
        }

        /// <summary>
        /// Confirm that a new product is added by checking the total Product count
        /// before and after creating a new Product
        /// Created by Sam Biondolillo
        /// </summary>
        [TestMethod]
        public void CreatingProductShouldIncreaseProductCount()
        {
            // Arrange
            var products = controller.GetProducts() as IList<Product>;
            var initialCount = products.Count;
            var newProduct = new Product { Id = 99, Name = "Test Car", Description = "This is just a test" };

            // Act
            controller.PostProduct(newProduct);
            products = controller.GetProducts() as IList<Product>;
            var finalCount = products.Count;

            // Assert
            Assert.AreEqual(finalCount, initialCount + 1);
            
        }

        /// <summary>
        /// Confirm that adding multiple products increases the total Product count
        /// by the number of new products we created
        /// Created by Sam Biondolillo
        /// </summary>
        [TestMethod]
        public void CreatingMultipleProductsShouldIncreaseProductCount()
        {
            // Arrange
            var products = controller.GetProducts() as IList<Product>;
            var initialCount = products.Count;
            var newProduct = new Product { Id = 99, Name = "Test Car", Description = "This is just a test" };
            var newProduct2 = new Product { Id = 999, Name = "Test Car 2", Description = "This is the second test" };
            var newProduct3 = new Product { Id = 9999, Name = "Test Car 3", Description = "This is the third test" };

            // Act
            controller.PostProduct(newProduct);
            controller.PostProduct(newProduct2);
            controller.PostProduct(newProduct3);
            products = controller.GetProducts() as IList<Product>;
            var finalCount = products.Count;

            // Assert
            Assert.AreEqual(finalCount, initialCount + 3);

        }

        /// <summary>
        /// Confirm that GetProductById returns correct product if new product was just added
        /// Created by Sam Biondolillo
        /// </summary>
        [TestMethod]
        public void GetProductByIdShouldReturnNewlyCreatedProduct()
        {
            // Arrange
            var newProduct = new Product { Id = 99, Name = "Test Car", Description = "This is just a test" };
            
            // Act
            controller.PostProduct(newProduct);
            var finalProduct = controller.GetProductById(newProduct.Id);

            // Assert
            Assert.IsTrue(finalProduct.Name.Equals(newProduct.Name));
            Assert.IsTrue(finalProduct.Description.Equals(newProduct.Description));

        }

        /// <summary>
        /// Confirm that GetProductById fails when id is not found
        /// Created by Sam Biondolillo
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(System.Web.Http.HttpResponseException))]
        public void GetProductByIdShouldFailWhenIdNotFound()
        {
            // Arrange
            
            // Act
            var product = controller.GetProductById(10101);

            // Assert
            Assert.IsNull(product);

        }

        /// <summary>
        /// Confirm that PostProduct returns bad request if product passed is null
        /// Created by Sam Biondolillo
        /// </summary>
        [TestMethod]
        public void PostProductShouldReturnBadRequestWhenPassedNullProduct()
        {
            // Arrange
            var expected = System.Net.HttpStatusCode.BadRequest;

            // Act
            var actual = controller.PostProduct(null).StatusCode;

            // Assert
            Assert.AreEqual(expected, actual);

        }

    }
}
