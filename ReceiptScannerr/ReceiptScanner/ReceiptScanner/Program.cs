using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ReceiptScanner
{
    public class Program
    {
        HttpClient client = new HttpClient();

        static async Task Main(string[] args)
        {
            Program program = new Program();
            await program.GetTodoItems();
        }

        private async Task GetTodoItems()
        {
            string response = await client.GetStringAsync(
               "https://interview-task-api.mca.dev/qr-scanner-codes/alpha-qr-gFpwhsQ8fkY1");

            List<Product> receipt = JsonConvert.DeserializeObject<List<Product>>(response);

            receipt.Sort((x, y) => string.Compare(x.Name, y.Name));

            int count = receipt.Count(x => x.Domestic);
            int countImported = receipt.Count(x => x.Domestic == false);

            double sumDomestic = receipt.Where(c => c.Domestic).Sum(c => c.Price);
            double sumImported = receipt.Where(x => x.Domestic == false).Sum(x => x.Price);

            IEnumerable<IGrouping<bool, Product>> groupByDomestic = receipt.GroupBy(x => x.Domestic);

            foreach (var item in groupByDomestic)
            {
                Console.WriteLine("---------------------------");

                if (item.Key == true)
                {
                    Console.WriteLine(". Domestic: ");
                }
                else if (item.Key == false)
                {
                    Console.WriteLine(". Imported: ");
                }

                foreach (var product in item)
                {

                    if (product.Description.Length > 10)
                    {
                        product.Description = product.Description.Substring(0, 10);
                    }

                    if (product.Weight == null)
                    {
                        Console.WriteLine("..." + product.Name + "\nPrice: $ " + product.Price + "\n" + product.Description + "..." + "\nWeight: N/A");
                    }
                    else if (product.Weight != null)
                    {
                        Console.WriteLine("..." + product.Name + "\nPrice: $ " + product.Price + "\n" + product.Description + "..." + "\nWeight: " + product.Weight);
                    }
                }
            }

            Console.WriteLine("---------------------------");
            Console.WriteLine("Imported cost: {0:C}", sumDomestic);
            Console.WriteLine("Imported cost: {0:C}", sumImported);
            Console.WriteLine("---------------------------");
            Console.WriteLine("Domestic count: " + count);
            Console.WriteLine("Imported count: " + countImported);


            Console.ReadLine();
        }

        public class Product
        {
            public string Name { get; set; }
            public bool Domestic { get; set; }
            public double Price { get; set; }
            public int? Weight { get; set; }
            public string Description { get; set; }
        }
    }
}
