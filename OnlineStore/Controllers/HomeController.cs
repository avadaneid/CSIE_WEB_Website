using Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Drawing.Drawing2D;
using Contact;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;
using System.Windows.Forms;

namespace OnlineStore.Controllers
{
    class Email
    {
        public static void SendEmail(string toAddress, string fromAddress,string subject, StringBuilder message)
        {
            try
            {
                using (var mail = new MailMessage())
                {
                    const string email = "afacerielectronice10@gmail.com";
                   
                    var loginInfo = new NetworkCredential(email, ConfigurationManager.AppSettings["password"]);

                    mail.From = new MailAddress(fromAddress);
                    mail.To.Add(new MailAddress(toAddress));
                    mail.Subject = subject;
                    mail.Body = message.ToString();
                    mail.IsBodyHtml = true;

                    try
                    {
                        using (var smtpClient = new SmtpClient("smtp.gmail.com", 587))
                        {
                            smtpClient.EnableSsl = true;
                            smtpClient.UseDefaultCredentials = false;
                            smtpClient.Credentials = loginInfo;
                            smtpClient.Send(mail);
                        }
                    }

                    finally
                    {
                    
                        mail.Dispose();
                    }

                }
            }
            catch (SmtpFailedRecipientsException ex)
            {
                MessageBox.Show("Error : " + ex);
            }
          
        }
    }
   
    class Resize
    {
            public static Bitmap ResizeImage(Image image, int width, int height)
            {
                var destRect = new Rectangle(0, 0, width, height);
                var destImage = new Bitmap(width, height);

                destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

                using (var graphics = Graphics.FromImage(destImage))
                {
                    graphics.CompositingMode = CompositingMode.SourceCopy;
                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.SmoothingMode = SmoothingMode.HighQuality;
                    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                    using (var wrapMode = new ImageAttributes())
                    {
                        wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                        graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                    }
                }

                return destImage;
            }
        
    }

    public class IO
    {
        public static byte[] NoPhoto()
        {
            byte[] result = new byte[0];
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["Database"]))
            {
                if(conn.State == 0)
                {
                    conn.Open();
                }

                string com = "select image from dbo.Product where Id = '566C06C8-C850-4CD2-8E54-CD321BCC711B'";

                using (SqlCommand sqlcom = new SqlCommand(com, conn))
                {
                    SqlDataReader read = sqlcom.ExecuteReader();
                    while (read.Read())
                    {
                        result = (byte[])read["Image"];
                    }
                }

            }
            return result;
        }

        public static byte[] ImageInsert(HttpPostedFileBase pf)
        {

            MemoryStream ms = new MemoryStream();
            System.Drawing.Image img = Resize.ResizeImage(System.Drawing.Image.FromStream(pf.InputStream), 190, 240);

            img.Save(ms, ImageFormat.Jpeg);
            byte[] bt = ms.ToArray();
            return bt;
        }

        public static byte[] ImageRead(byte[] image)
        {

            //System.Drawing.Image btm = System.Drawing.Image.FromStream(new MemoryStream(image));

            return image;
        }

        public static void Insert(Product pr)
        {
            
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.AppSettings["Database"]))
            {
                connection.Open();

                string s = "Insert into Product(Id,Name,Price,Description,Category,Image) values (@GUID,@Name,@Price,@Description,@Category,@Image)";
                
                using (SqlCommand cmd = new SqlCommand(s, connection))
                {
                    cmd.Parameters.Add("@GUID", SqlDbType.UniqueIdentifier).Value = Guid.NewGuid();
                    cmd.Parameters.Add("@Name", SqlDbType.NChar).Value = pr.Name;
                    cmd.Parameters.Add("@Price", SqlDbType.Int).Value = pr.Price;
                    cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = pr.Description;
                    cmd.Parameters.Add("@Category", SqlDbType.NVarChar).Value = pr.Description;
                    cmd.Parameters.Add("@Image", SqlDbType.VarBinary).Value = IO.ImageInsert(pr.Picture);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void InsertCOM(COM.Comanda cm)
        {

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.AppSettings["Database"]))
            {
                connection.Open();

                string s = "Insert into Comenzi(IdComanda,Nume,Prenume,AdresaLivrare,NumarTelefon,Email) values (@IdComanda,@Nume,@Prenume,@AdresaLivrare,@NumarTelefon,@Email)";

                using (SqlCommand cmd = new SqlCommand(s, connection))
                {
                    cmd.Parameters.Add("@IdComanda", SqlDbType.UniqueIdentifier).Value = Guid.NewGuid();
                    cmd.Parameters.Add("@Nume", SqlDbType.NChar).Value = cm.Nume;
                    cmd.Parameters.Add("@Prenume", SqlDbType.NChar).Value = cm.Prenume;
                    cmd.Parameters.Add("@AdresaLivrare", SqlDbType.NVarChar).Value = cm.AdresaLivrare;
                    cmd.Parameters.Add("@NumarTelefon", SqlDbType.NVarChar).Value = cm.NumarTelefon;
                    cmd.Parameters.Add("@Email", SqlDbType.NVarChar).Value = cm.Email;

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static List<Product> Read(Guid gd = default) {

            List<Product> lst;
            
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.AppSettings["Database"]))
            {
                string s = null;

                if (gd == new Guid())
                {
                    s = "select * from dbo.Product";
                }
                else
                {
                    s = $"select * from dbo.Product where Id = '{gd}'";
                }
                                         
                lst = new List<Product>();
                connection.Open();

                using (SqlCommand sqm = new SqlCommand(s, connection))
                {
                    using (SqlDataReader sdr = sqm.ExecuteReader())
                    {
                        while(sdr.Read())
                        {                        
                            lst.Add(new Product()
                            {
                                ID = (Guid)sdr?.GetValue(0),
                                Name = (string)sdr?.GetValue(1)??string.Empty,
                                Price = (int)sdr?.GetValue(2),
                                Description = (string)sdr?.GetValue(3),
                                PictureR = (sdr?.GetValue(4) == System.DBNull.Value) ? IO.NoPhoto() : (byte[])sdr.GetValue(4),
                                Category = (string)sdr?.GetValue(5),


                            });
                        }
                    }
                }
            }
            return lst;               
        }
               
    }


    public class HomeController : Controller
    {
        [HttpPost]
        public ActionResult Form(Product p)
        {
           
            IO.Insert(p);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult FinCOM(COM.Comanda c)
        {
           
                IO.InsertCOM(c);
                return RedirectToAction("FinalizareComanda");
            
         
        }

        public ActionResult FinalizareComanda()
        {
           
            return View();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Products()
        {
            ViewBag.List = IO.Read();
            return View();
        }

        public ActionResult Product(Guid Id)
        {
        
            ViewBag.Product = IO.Read(Id);
            return View();
        }

        public ActionResult Cart(Guid Id)
        {
            ViewBag.List = IO.Read(Id);
            return View();
        }

        public ActionResult About()
        {

            return View();
        }

        [HttpPost]
        public ActionResult SendEmail(ContactFormular contactFormular)
        {

            if (ModelState.IsValid)
            {

                var toAddress = "afacerielectronice10@gmail.com";
                var fromAddress = contactFormular.AdresaEmail.ToString();
                var subject = "Test"; 

                var message = new StringBuilder();
                message.Append("Nume: " +    contactFormular.Nume);
                message.Append("Prenume " +  contactFormular.Prenume);
                message.Append("Email: " +   contactFormular.AdresaEmail);
                message.Append("Telefon: " + contactFormular.NumarTelefon);
                message.Append("Mesaj: " +   contactFormular.Mesaj);

                Task.Run(() => Email.SendEmail(toAddress, fromAddress, subject, message));
                
            }
            return RedirectToAction("Contact");

        }

        public ActionResult Contact()
        {
            return View();
        }

    }
}