using System;
using Microsoft.Azure.Documents;
using System.Linq;
using System.Windows;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using Microsoft.Azure.Documents.Linq;
using System.Configuration;

namespace WpfNoSqlJasenRekisteriApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly string EndpointUri = ConfigurationManager.AppSettings["EndPointUri"];

        private static readonly string PrimaryKey = ConfigurationManager.AppSettings["PrimaryKey"];
        public MainWindow()
        {
            InitializeComponent();
        }
        // Get all documents from database
        private async void btnGetDb_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri(EndpointUri);
            DocumentClient client = new DocumentClient(uri, PrimaryKey);

            try
            {
                Uri kokoelmaUrl = UriFactory.CreateDocumentCollectionUri("ToDoList", "Items");
                using (var queryable = client.CreateDocumentQuery<Jasen>(kokoelmaUrl,
                    new FeedOptions { MaxItemCount = 100 }).Where(b => b.PartitionKey > 0).AsDocumentQuery())
                {
                    while (queryable.HasMoreResults)
                    {
                        foreach (Jasen b in await queryable.ExecuteNextAsync<Jasen>())
                        {
                            txtJasenet.AppendText(b.Etunimi + " " + b.Sukunimi + "   Id: " + b.Id + "   PartitionKey: " + b.PartitionKey + '\r');
                        }
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Ei dokumentteja kannassa!");
            }
        }
        // Add document
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri(EndpointUri);
            DocumentClient client = new DocumentClient(uri, PrimaryKey);
            Random rnd = new Random();
            int avain = rnd.Next(0, 1000);
            try
            {
                Uri kokoelmaUrl = UriFactory.CreateDocumentCollectionUri("ToDoList", "Items");
                Jasen olio = new Jasen()
                {
                    Etunimi = txtEtu.Text,
                    Sukunimi = txtSuku.Text,
                    Osoite = txtOsoite.Text,
                    Postinumero = int.Parse(txtPostnum.Text),
                    Puhelin = txtPuh.Text,
                    Sahkoposti = txtSposti.Text,
                    Alkupvm = DateTime.Parse(txtAlkuPvm.Text),
                    PartitionKey = avain
                };
                ResourceResponse<Document> vastaus = await client.UpsertDocumentAsync(kokoelmaUrl, olio);

                txtEtu.Text = "";
                txtSuku.Text = "";
                txtOsoite.Text = "";
                txtPostnum.Text = "";
                txtPuh.Text = "";
                txtSposti.Text = "";
                txtAlkuPvm.Text = "";

                MessageBox.Show("Jäsen lisätty!");
                MessageBox.Show(vastaus.StatusCode + " " + vastaus.Resource.Id);
            }
            catch (Exception)
            {
                MessageBox.Show("Käyttäjää ei voitu lisätä!");
            }
        }
        // Get document by Id
        private async void btnTulosta_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri(EndpointUri);
            DocumentClient client = new DocumentClient(uri, PrimaryKey);

            try
            { 
                string id = txtID.Text;
                int avain = int.Parse(txtFindByName.Text);
                Uri dokumenttiUrl = UriFactory.CreateDocumentUri("ToDoList", "Items", id);
                ResourceResponse<Document> vastaus = await client.ReadDocumentAsync(dokumenttiUrl,
                    new RequestOptions { PartitionKey = new PartitionKey(avain) });

                string json = JsonConvert.SerializeObject(vastaus.Resource);
                txtJasenet.Text = json;

                Jasen olio = await client.ReadDocumentAsync<Jasen>(dokumenttiUrl,
                    new RequestOptions { PartitionKey = new PartitionKey(avain) });
                txtEtu.Text = olio.Etunimi;
                txtSuku.Text = olio.Sukunimi;
                txtOsoite.Text = olio.Osoite;
                txtPostnum.Text = olio.Postinumero.ToString();
                txtPuh.Text = olio.Puhelin;
                txtSposti.Text = olio.Sahkoposti;
                txtAlkuPvm.Text = olio.Alkupvm.ToString();

            }
            catch (Exception)
            {
                MessageBox.Show("Käyttäjää ei löydy!");
            }
        }
        // Delete document
        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                Uri uri = new Uri(EndpointUri);
                DocumentClient client = new DocumentClient(uri, PrimaryKey);

                string id = txtID.Text;
                int avain = int.Parse(txtFindByName.Text);
                Uri dokumenttiUrl = UriFactory.CreateDocumentUri("ToDoList", "Items", id);
                ResourceResponse<Document> vastaus = await client.DeleteDocumentAsync(dokumenttiUrl,
                    new RequestOptions { PartitionKey = new PartitionKey(avain) });
                MessageBox.Show("Käyttäjä poistettu " + id);
                txtJasenet.Text = "";
            }
            catch (Exception)
            {
                MessageBox.Show("Käyttäjää ei löydy!");
            }      
        }
        // Edit document
        private async void btnMuokkaa_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri(EndpointUri);
            DocumentClient client = new DocumentClient(uri, PrimaryKey);

            try
            {
                string id = txtID.Text;
                int avain = int.Parse(txtFindByName.Text);
                Uri dokumenttiUrl = UriFactory.CreateDocumentUri("ToDoList", "Items", id);
                ResourceResponse<Document> vastaus = await client.ReadDocumentAsync(dokumenttiUrl,
                    new RequestOptions { PartitionKey = new PartitionKey(avain) });

                Jasen olio = await client.ReadDocumentAsync<Jasen>(dokumenttiUrl,
                    new RequestOptions { PartitionKey = new PartitionKey(avain) });
                    {
                    olio.Id = olio.Id;
                    olio.PartitionKey = olio.PartitionKey;
                    olio.Etunimi = txtEtu.Text;
                    olio.Sukunimi = txtSuku.Text;
                    olio.Osoite = txtOsoite.Text;
                    olio.Postinumero = int.Parse(txtPostnum.Text);
                    olio.Puhelin = txtPuh.Text;
                    olio.Sahkoposti = txtSposti.Text;
                    olio.Alkupvm = DateTime.Parse(txtAlkuPvm.Text);
                    };

                Uri kokoelmaUrl = UriFactory.CreateDocumentCollectionUri("ToDoList", "Items");
                ResourceResponse<Document> muokkaus = await client.UpsertDocumentAsync(kokoelmaUrl, olio);

                MessageBox.Show("Käyttäjä muokattu! " + muokkaus.StatusCode + " " + muokkaus.Resource.Id);

            }
            catch (Exception)
            {
                MessageBox.Show("Käyttäjää ei muokattu!");
            }
        }
        // Empty fields
        private void btnNollaa_Click(object sender, RoutedEventArgs e)
        {
            txtEtu.Text = "";
            txtSuku.Text = "";
            txtOsoite.Text = "";
            txtOsoite.Text = "";
            txtPostnum.Text = "";
            txtPuh.Text = "";
            txtSposti.Text = "";
            txtAlkuPvm.Text = "";
            txtJasenet.Text = "";
            txtFindByName.Text = "";
            txtID.Text = "";
        }
    }
}
