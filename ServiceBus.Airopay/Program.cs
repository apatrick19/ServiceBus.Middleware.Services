using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Airopay
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Tesing");

            var input = new int[]{ 1, 2, 3, 4 };

          var result=  iszigZag(input);

            foreach (var item in result)
            {
                Console.WriteLine(item);
            }
           

            Console.ReadLine();
        }

        public static int[] iszigZag(int[] arr)
        {
            var list = new List<int>();

            for (int i = 0; i < arr.Length - 2; i++)
            {
                if (arr[i] < arr[i + 1] && arr[i + 1] > arr[i + 2])
                {
                    list.Add(1);
                }
                else if (arr[i] > arr[i + 1] && arr[i + 1] < arr[i + 2])
                {
                    list.Add(1);
                }
                else
                {
                    list.Add(0);
                }
            }
            return list.ToArray();

        }
    }
}
