using Newtonsoft.Json;
using Spectre.Console;
using System;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace TwitchDropFarmBot
{
    class Functions
    {
        private static dynamic SID = System.Security.Principal.WindowsIdentity.GetCurrent().User;
        private static string passPhrase = "";
        public static bool IsPassSet = false;
        public static bool IsPassInvalid = false;

        public static void AskForCreds(bool changePass = false)
        {
            Console.Clear();
            if (changePass) {
                passPhrase = AnsiConsole.Prompt(
                    new TextPrompt<string>("Enter new password: ")
                        .PromptStyle("red")
                        .Secret());
            } else {
                passPhrase = AnsiConsole.Prompt(
                    new TextPrompt<string>("Enter your password (If it's your first time then put new pass): ")
                        .PromptStyle("red")
                        .Secret());
            }
            if (passPhrase == string.Empty)
                AskForCreds(changePass);
            IsPassSet = true;
            Console.Clear();
        }

        private static string initVector = SID.ToString().Substring(0,19);
        private static int keysize = 256;

        public static string EncryptString(string plainText)
        {
            byte[] initVectorBytes = Encoding.UTF8.GetBytes(initVector);
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, null);
            byte[] keyBytes = password.GetBytes(keysize / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
            cryptoStream.FlushFinalBlock();
            byte[] cipherTextBytes = memoryStream.ToArray();
            memoryStream.Close();
            cryptoStream.Close();
            Trace.WriteLine("Plain: " + plainText + " Cipher: " + Convert.ToBase64String(cipherTextBytes));
            return Convert.ToBase64String(cipherTextBytes);
        }
        public static string DecryptString(string cipherText, bool showText = true)
        {
            try
            {
                byte[] initVectorBytes = Encoding.UTF8.GetBytes(initVector);
                byte[] cipherTextBytes = Convert.FromBase64String(cipherText);
                PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, null);
                byte[] keyBytes = password.GetBytes(keysize / 8);
                RijndaelManaged symmetricKey = new RijndaelManaged();
                symmetricKey.Mode = CipherMode.CBC;
                ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);
                MemoryStream memoryStream = new MemoryStream(cipherTextBytes);
                CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
                byte[] plainTextBytes = new byte[cipherTextBytes.Length];
                int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                memoryStream.Close();
                cryptoStream.Close();
                return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
            } catch (Exception ex) {
                if (ex.Message.Contains("Padding is invalid and cannot be removed"))
                {
                    if (showText)
                    {
                        AnsiConsole.MarkupLine("[red]Invalid Password![/]");
                        Thread.Sleep(350);
                    }
                    IsPassInvalid = true;
                    return cipherText;
                }
                else {
                    AnsiConsole.MarkupLine("[red]Unable to decrypt![/]\n" +
                        "Exception: "+ex.Message);
                    Thread.Sleep(3000);
                    return cipherText;
                }
            }
            
        }
        public static dynamic LoadConfig()
        {
            using (StreamReader r = new StreamReader("config.json"))
            {
                string json = r.ReadToEnd();
                dynamic data = JsonConvert.DeserializeObject(json);
                return data;
            }
        }

        public static void SaveConfig()
        {
            //I know that all of this is a very shitty way of doing it but aslong as it does the job let it be

            if (Program.cfg.client_id.ToString().EndsWith("=")) {
                //Console.WriteLine("Client ID already encrypted. Unable to save. (Probably wrong password)");
                AnsiConsole.MarkupLine("[red]Client ID already encrypted. Unable to save. (Possibly wrong password)[/]");
            } else {
                Program.cfg.client_id = Functions.EncryptString(Program.cfg.client_id.ToString());
            }

            if (Program.cfg.client_secret.ToString().EndsWith("=")) {
                //Console.WriteLine("Client Secret already encrypted. Unable to save. (Probably wrong password)");
                AnsiConsole.MarkupLine("[red]Client Secret already encrypted. Unable to save. (Probably wrong password)[/]");
            }
            else {
                Program.cfg.client_secret = Functions.EncryptString(Program.cfg.client_secret.ToString());
            }

            if (Program.cfg.access_token.ToString().EndsWith("=")) {
                AnsiConsole.MarkupLine("[red]Access Token already encrypted. Unable to save. (Probably wrong password)[/]");
                //Console.WriteLine("Access Token already encrypted. Unable to save. (Probably wrong password)");
            } else {
                Program.cfg.access_token = Functions.EncryptString(Program.cfg.access_token.ToString());
            }
            Trace.WriteLine("Client ID, Client Secret, Access Token have been encrypted");

            File.WriteAllText("config.json", JsonConvert.SerializeObject(Program.cfg));
        }

        public static void GenerateConfigFile()
        {
            if (File.Exists("config.json")) File.Delete("config.json");

            var newCfgFile = File.Create("config.json");
            newCfgFile.Close();

            dynamic new_config = new ExpandoObject();
            new_config.client_id = "";
            new_config.client_secret = "";
            new_config.access_token = "";
            new_config.auto_open_stream = true;
            new_config.auto_close_stream = false;
            new_config.browser_proc_name = "";

            File.WriteAllText("config.json", JsonConvert.SerializeObject(new_config));
            //Console.WriteLine("Config has been generated.");
            AnsiConsole.MarkupLine("[green]Config has been generated.[/]");
        }
    }
}
