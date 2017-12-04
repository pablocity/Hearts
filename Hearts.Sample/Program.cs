using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hearts.Model;
using System.IO;

namespace Hearts.Sample
{
    class Program
    {
        //Utility program to rename card's images files on HDD as it fit the name convention in resources
        static void Main(string[] args)
        {
            Deck deck = new Deck();

            //deck.Shuffle(new Random());

            string[] files = Directory.GetFiles(@"C:\Users\Paweł\Desktop\Hearts");
            List<FileInfo> fileList = Directory.GetFiles(@"C:\Users\Paweł\Desktop\Hearts").Select(f => new FileInfo(f)).OrderBy(f => f.LastWriteTime).ToList();
            int i = 0;

            for (int a = 0; a < 4; a++)
            {
                fileList.RemoveAt(0);
                
            }

            foreach (FileInfo file in fileList)
            {

                File.Move(file.FullName, $@"C:\Users\Paweł\Desktop\Hearts\{deck.cards[i].Name}.png");
                i++;
            }


            foreach (FileInfo s in Directory.GetFiles(@"C:\Users\Paweł\Desktop\Hearts").Select(f => new FileInfo(f)).OrderBy(f => f.LastWriteTime))
            {
                Console.WriteLine(s.Name);
            }

            Console.ReadKey();
        }
    }
}
