using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace WpfNoSqlJasenRekisteriApp.Tests
{
    [TestClass]
    public class UnitTest1
    {
        public string EndpointUri { get; set; } = "<URI>";
        private string PrimaryKey { get; set; } = "<PRIMARYKEY>";

        [TestMethod]
        public void TestGetDocuments()
        {
            //Arrange
            Uri uri = new Uri(EndpointUri);
            DocumentClient client = new DocumentClient(uri, PrimaryKey);
            Uri kokoelmaUrl = UriFactory.CreateDocumentCollectionUri("ToDoList", "Items");
            //Act
            var queryable = client.CreateDocumentQuery<Jasen>(kokoelmaUrl, new FeedOptions { MaxItemCount = 100 }).Where(b => b.PartitionKey > 0).AsDocumentQuery();
            var results = queryable.HasMoreResults;
            var total = client.CreateDocumentQuery<Jasen>(kokoelmaUrl, "SELECT * FROM C").ToList();
            //Assert
            if (results == false)
            {
                throw new Exception("Ei hakenut dokumenttia kannasta");
            }
            else
            {
                Assert.IsNotNull(results);
                Assert.AreEqual(5, total.Count);
            } 
        }


        [TestMethod]
        public void TestAddDocument()
        {
            //Arrange
            Uri uri = new Uri(EndpointUri);
            DocumentClient client = new DocumentClient(uri, PrimaryKey);
            Uri kokoelmaUrl = UriFactory.CreateDocumentCollectionUri("ToDoList", "Items");
            var results = client.CreateDocumentQuery<Jasen>(kokoelmaUrl, "SELECT * FROM C").ToList();
            Jasen jasen = new Jasen()
            {
                Id = "123456",
                PartitionKey = 123,
                Etunimi = "Teppo",
                Sukunimi = "Testaaja",
                Osoite = "Oikopolku 10",
                Puhelin = "1234567",
                Sahkoposti = "testi.testi@suomi.fi",
                Postinumero = 48100,
                Alkupvm = DateTime.Now
            };
            //Act
            var dokumentti = client.UpsertDocumentAsync(kokoelmaUrl, jasen);
            var results2 = client.CreateDocumentQuery<Jasen>(kokoelmaUrl, "SELECT * FROM C").ToList();
            //Assert
            if (dokumentti == null)
            {
                Assert.Fail("Käyttääjää ei luotu");
            }
            else
            {
                Assert.AreNotEqual(results.Count, results2.Count);
            }
        }

        [TestMethod]
        public async Task TestDeleteDocument()
        {
            //Arrange
            Uri uri = new Uri(EndpointUri);
            DocumentClient client = new DocumentClient(uri, PrimaryKey);
            string id = "123456";
            int avain = 123;
            //Act
            Uri dokumenttiUrl = UriFactory.CreateDocumentUri("ToDoList", "Items", id);
            _ = await client.DeleteDocumentAsync(dokumenttiUrl,
                new RequestOptions { PartitionKey = new PartitionKey(avain) });

            Uri kokoelmaUrl = UriFactory.CreateDocumentCollectionUri("ToDoList", "Items");
            var results = client.CreateDocumentQuery<Jasen>(kokoelmaUrl, "SELECT * FROM C").ToList();
            
            //Assert
            Assert.AreEqual(5, results.Count);
        }
    }
}
