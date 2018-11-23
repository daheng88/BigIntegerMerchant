using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigIntegerMerchant
{
    class Program
    {
        static void Main(string[] args)
        {

            var merchantId = 99;
            var memberId = 12;
            var content = ApiUtil.encryptMemberId(merchantId, memberId);
            Console.WriteLine(content);
            //AQza5zef7OL+/A==
            //var result = ApiUtil.decryptMemberId(merchantId, content);
           // Console.WriteLine(result);

           // var df = ApiUtil.GetMd5Hex(merchantId + "").Substring(16);


           //  Console.WriteLine(df);
            Console.ReadLine();
        }
    }
}
