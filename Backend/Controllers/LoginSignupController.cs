using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
using Backend.Data;
using Backend.Models;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Net.Mail;
using System.Net;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class LoginSignupController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly JwtSettings _jwtsettings;
        private readonly byte[] _key;
        private readonly byte[] _iv;

        public LoginSignupController(AppDbContext context, IOptions<JwtSettings> jwtSettings)
        {
            _context = context;
            _jwtsettings = jwtSettings.Value;
            _key = Encoding.ASCII.GetBytes(_jwtsettings.Key);
            _iv = new byte[16];

        }
        [HttpGet("{Id}")]
        public IActionResult GetById(int Id) {
        
            var client = _context.Clients.Find(Id);
            if (client == null)
            {
                return NotFound();
            }
            return Ok(client);
        }

        [HttpGet]
        public IActionResult Get()
        {

            var client = _context.Clients.ToList();
            if (client == null)
            {
                return NotFound();
            }
            return Ok(client);
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] Login login)
        {
            var user = _context.Clients.FirstOrDefault(u => u.Email == login.Email);
            if (user == null)
            {
                return NotFound("email or password Not Found");
            }
            string decryptedPassword = DecryptString(user.Password);
            if (decryptedPassword != login.Password)
            {
                return NotFound("Invalid Email or Password");
            }

            var tokenString = GenerateJwtToken(user);
            return Ok(new { tokenString,user.ClientId });
        }

        private string GenerateJwtToken(Client client)

        {

            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(_jwtsettings.Secret);



            var tokenDescriptor = new SecurityTokenDescriptor

            {

                Subject = new ClaimsIdentity(new Claim[]

                {

                    new Claim(ClaimTypes.Name, client.ClientId.ToString())

                }),

                Expires = DateTime.UtcNow.AddHours(1), // Token expiration time 

                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),

                Issuer = _jwtsettings.Issuer,

                Audience = _jwtsettings.Audience

            };



            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);

        }

        [HttpPost("signup")]

        public IActionResult Signup([FromBody] Client client)

        {
            client.Password = EncryptString(client.Password);
           

            _context.Clients.Add(client);

            _context.SaveChanges();

            return CreatedAtAction(nameof(Signup), new { id = client.ClientId }, client);
        }
        private string EncryptString(string plainText)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = _key;
                aes.IV = _iv;
                aes.Mode = CipherMode.CBC;
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                    }
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }
        private string DecryptString(string cipherText)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = _key;
                aes.IV = _iv;
                aes.Mode = CipherMode.CBC;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
        private string GetEmail([FromBody] Client client)
        {
            var user = _context.Clients.FirstOrDefault(u => u.Email == client.Email);
            if (user == null)
            {
                return "Email Not Found";
            }

            return (client.Email);
        }
       

        [HttpPost("forgotpassword")]
        public IActionResult GenerateOTP([FromQuery] string email, string content)
        {
            string[] saAllowedCharacters = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" };
            string sOTP = String.Empty;

            string sTempChars = String.Empty;

            Random rand = new Random();

            for (int i = 0; i < 6; i++)

            {

                int p = rand.Next(0, saAllowedCharacters.Length);

                sTempChars = saAllowedCharacters[rand.Next(0, saAllowedCharacters.Length)];

                sOTP += sTempChars;

            }

            string sender = "Priyastewartjan6@gmail.com";
            string senderPass = "uevp gqrz cfmz itce";
            string recieve = email;

            MailMessage mail = new MailMessage(sender, recieve);
            mail.Subject = "Forgot password";
            mail.Body = content;

            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
            smtpClient.Port = 587;
            smtpClient.Credentials = new NetworkCredential(sender, senderPass);
            smtpClient.EnableSsl = true;

            try
            {
                smtpClient.Send(mail);
                Console.WriteLine("Sent Successfully");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }

            return Ok(new { sOTP });
        }

        [HttpPost("email")]
        public IActionResult GenerateEmail([FromQuery] string email, string content)
        {
            string sender = "Priyastewartjan6@gmail.com";
            string senderPass = "uevp gqrz cfmz itce";
            string recieve = email;

            MailMessage mail = new MailMessage(sender, recieve);
            mail.Subject = "Booking Confirmation mail";
            mail.Body = content;

            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
            smtpClient.Port = 587;
            smtpClient.Credentials = new NetworkCredential(sender, senderPass);
            smtpClient.EnableSsl = true;

            try
            {
                smtpClient.Send(mail);
                Console.WriteLine("Sent Successfully");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }

            return Ok(new { content });
        }

        [HttpPut("forgotpassword(id)")]
        public IActionResult Update(int Id, Client updatingPassword)
        {
            var existingPassword = _context.Clients.Find(Id);
            if (existingPassword == null)
            {
                return NotFound("User not found");
            }
            existingPassword.Password = updatingPassword.Password;
            existingPassword.Email = updatingPassword.Email;

            _context.Clients.Add(existingPassword); 
            _context.SaveChanges();

            return Ok(existingPassword);
        }
    }
}

