using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Vigilance.Web;


namespace OrchestratorAsset.Web.Controllers
{

    public class WalletRechargeController : Controller
    {
        // GET: WalletRecharge
        public ActionResult Recharge()
        {
            return View();
        }


      

        public async Task<String> RechargeWallet()
        {
            

            RequestClass obj = new RequestClass();
            obj.flag = "RECH";
            obj.etcCustId = "";
            obj.vrn = "KT01AA1152";
            obj.rechargeTxnid = "FAI000000000093674";
            obj.rechargeAmt = "500.00";

            string json = JsonConvert.SerializeObject(obj);
            var encryptedMessage = encryptMessage(json);

           // postData(encryptedMessage);
            // var decrypMessage = decryptMessage(encryptedMessage);


            string ContactUs = "https://125.19.66.195/kbxwnetc/RechargeCustomerAccount";
            string MyProxyHostString = "172.16.240.153";
            int MyProxyPort = 8080;
            //Bypass SSL Verification
            ServicePointManager.ServerCertificateValidationCallback +=
                delegate (object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors) { return true; };

            var request = (HttpWebRequest)WebRequest.Create(ContactUs);
            request.Proxy = new WebProxy(MyProxyHostString, MyProxyPort);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Accept = "application/json";

            APIFormat req = new APIFormat();
            req.reqdata = encryptedMessage;
            req.msgid = "0";
            string jsonReq = JsonConvert.SerializeObject(req);

            using ( var streamwriter=new StreamWriter(request.GetRequestStream()))
            {
                streamwriter.Write(jsonReq);
            }

            var response = (HttpWebResponse)request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader sr = new StreamReader(stream);
            string s = sr.ReadToEnd();

            string decrypt = decryptMessage(s);
     
            return null;

        }



        public void GetText()
        {


            var r = RechargeWallet();
           




        }

        

        public void CheckNumberIsValid()
        {
            //  var vehicleNumber = "";
            VehicleValidation();

        }

        public void checkBalance()
        {
            BalanceCheck();
        }


        public async Task<String> VehicleValidation()
        {


            RegisterClass obj = new RegisterClass();
            obj.flag = "VI";
            obj.etcCustId = "";
            obj.vrn = "KT01AA1152";
           

            string json = JsonConvert.SerializeObject(obj);
            var encryptedMessage = encryptMessage(json);
          
            string apiURL = "https://125.19.66.195/kbxwnetc/CustomerVehicleDetails";
            string MyProxyHostString = "172.16.240.153";
            int MyProxyPort = 8080;
            //Bypass SSL Verification
            ServicePointManager.ServerCertificateValidationCallback +=
                delegate (object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors) { return true; };

            var request = (HttpWebRequest)WebRequest.Create(apiURL);
            request.Proxy = new WebProxy(MyProxyHostString, MyProxyPort);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Accept = "application/json";

            APIFormat req = new APIFormat();
            req.reqdata = encryptedMessage;
            req.msgid = "0";
            string jsonReq = JsonConvert.SerializeObject(req);

            using (var streamwriter = new StreamWriter(request.GetRequestStream()))
            {
                streamwriter.Write(jsonReq);
            }

            var response = (HttpWebResponse)request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader sr = new StreamReader(stream);
            string s = sr.ReadToEnd();

            string decrypt = decryptMessage(s);

            var result = JsonConvert.DeserializeObject<VehicleInfo>(decrypt);

            return null;

        }



        public async Task<String> BalanceCheck()
        {


            RegisterClass obj = new RegisterClass();
            obj.flag = "CI";
            obj.etcCustId = "";
            obj.vrn = "KT01AA1152";
            try
            {

                string json = JsonConvert.SerializeObject(obj);
                var encryptedMessage = encryptMessage(json);

                string apiURL = "https://125.19.66.195/kbxwnetc/CustomerVehicleDetails";
                string MyProxyHostString = "172.16.240.153";
                int MyProxyPort = 8080;
                //Bypass SSL Verification
                ServicePointManager.ServerCertificateValidationCallback +=
                    delegate (object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors) { return true; };

                var request = (HttpWebRequest)WebRequest.Create(apiURL);
                request.Proxy = new WebProxy(MyProxyHostString, MyProxyPort);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Accept = "application/json";

                APIFormat req = new APIFormat();
                req.reqdata = encryptedMessage;
                req.msgid = "0";
                string jsonReq = JsonConvert.SerializeObject(req);

                using (var streamwriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamwriter.Write(jsonReq);
                }

                var response = (HttpWebResponse)request.GetResponse();
                Stream stream = response.GetResponseStream();
                StreamReader sr = new StreamReader(stream);
                string s = sr.ReadToEnd();

                string decrypt = decryptMessage(s);
                var result = JsonConvert.DeserializeObject<CustomerInfo>(decrypt);
                var a = 1;
            }
            catch(Exception e)
            {
                var k = e.Message;
            }
           

            return null;

        }





        public string generateKey(String password, byte[] saltBytes)
        {
           
            int iterations = 100;
            var rfc2898 =
            new System.Security.Cryptography.Rfc2898DeriveBytes(password, saltBytes, iterations);
            byte[] key = rfc2898.GetBytes(16);
            String keyB64 = Convert.ToBase64String(key);
            return keyB64;
        }

        public static byte[] hexStringToByteArray(string hexString)
        {

         

            byte[] data = new byte[hexString.Length / 2];
            for (int index = 0; index < data.Length; index++)
            {
                string byteValue = hexString.Substring(index * 2, 2);
                data[index] = byte.Parse(byteValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }

            return data;


        }


        static Random random = new Random();
        public static string GetRandomHexNumber(int digits)
        {
            byte[] buffer = new byte[digits / 2];
            random.NextBytes(buffer);
            string result = String.Concat(buffer.Select(x => x.ToString("X2")).ToArray());
            if (digits % 2 == 0)
                return result;
            return result + random.Next(16).ToString("X");
        }


        public string encryptMessage(string message)
        {
           
            String combineData = "";
            try
            {
                
                string passphrase = "46ea428a97ba4c3094fc66e112d1d678";
                string saltHex = GetRandomHexNumber(32);
                string ivHex = GetRandomHexNumber(32);
                byte[] salt = hexStringToByteArray(saltHex);
                byte[] iv = hexStringToByteArray(ivHex);

                string sKey = generateKey(passphrase, salt);
                byte[] keyBytes = System.Convert.FromBase64String(sKey);
                AesManaged aesCipher = new AesManaged();
               
                aesCipher.KeySize = 128;
                aesCipher.BlockSize = 128;
                aesCipher.Mode = CipherMode.CBC;
                aesCipher.Padding = PaddingMode.PKCS7;
                aesCipher.Key = keyBytes;

                byte[] b = System.Text.Encoding.UTF8.GetBytes(message);
                ICryptoTransform encryptTransform = aesCipher.CreateEncryptor(keyBytes, iv);
                byte[] ctext = encryptTransform.TransformFinalBlock(b, 0, b.Length);

                var plainTextBytes = System.Convert.ToBase64String(ctext);
              
                combineData = saltHex + " " + ivHex + " " + plainTextBytes;
                
            }
            catch (Exception e)
            {
               
            }
            combineData = combineData.Replace("\n", "").Replace("\t", "").Replace("\r", "");
            return combineData;
        }


        public string decryptMessage(string str)
        {
           
            string myKey = "46ea428a97ba4c3094fc66e112d1d678";
            string decrypted = null;
            try
            {
                if ((str != null) && (str.Contains(' ')))
                {
                    string salt = str.Split(' ')[0];
                    string iv = str.Split(' ')[1];
                    String encryptedText = str.Split(' ')[2];
                 
                    decrypted = DecryptAlter(salt, iv, myKey, encryptedText);
                    return decrypted;
                }
                else
                {
                    decrypted = str;
                    return decrypted;
                }
            }
            catch (Exception e)
            {

            }

            return decrypted;

        }


        public string DecryptAlter(string salt, string iv, string passphrase, string EncryptedText)
        {
            string decryptedValue = null;
            try
            {
                byte[] saltBytes = hexStringToByteArray(salt);

                string sKey = generateKey(passphrase, saltBytes);
                byte[] ivBytes = hexStringToByteArray(iv);


                byte[] keyBytes = System.Convert.FromBase64String(sKey);


                AesManaged aesCipher = new AesManaged();
                aesCipher.IV = ivBytes;
                aesCipher.KeySize = 128;
                aesCipher.BlockSize = 128;
                aesCipher.Mode = CipherMode.CBC;
                aesCipher.Padding = PaddingMode.PKCS7;
                byte[] b = System.Convert.FromBase64String(EncryptedText);
                ICryptoTransform decryptTransform = aesCipher.CreateDecryptor(keyBytes, ivBytes);
                byte[] plainText = decryptTransform.TransformFinalBlock(b, 0, b.Length);

                var res = System.Text.Encoding.UTF8.GetString(plainText);
                return res;
            }
            catch (Exception e)
            {
                var k = e.Message;
            }
            return "";
        }

        

        public class RequestClass
        {
            public string flag { get; set; }
            public string vrn { get; set; }
            public string etcCustId { get; set; }
            public string rechargeTxnid { get; set; }
            public string rechargeAmt { get; set; }
        }

        public class APIFormat
        {
            public string reqdata { get; set; }
            public string msgid { get; set; }
        }

       public class RegisterClass
        {
            public string flag { get; set; }
            public string vrn { get; set; }
            public string etcCustId { get; set; }
        }




        public class VehicleInfo
        {
            public string respCode { get; set; }
            public string respMessage { get; set; }
            public string flag { get; set; }
            public List <data> data { get; set; }

        }
        public class data
        {
            public string vrn { get; set; }
            public string tagStatus { get; set; }
            public string barCode { get; set; }
        }


        public class CustomerInfo
        {
            public string respCode { get; set; }
            public string respMessage { get; set; }
            public string flag { get; set; }
            public List<CustData> data { get; set; }
        }

        public class CustData
        {
            public string walletBalance { get; set; }
            public string mobileNum { get; set; }
            public string vrn { get; set; }
            public string vehicleClass { get; set; }
            public string customerName { get; set; }
            public string masterBalance { get; set; }
            public string sdBalance { get; set; }
        }

    }


   






}

