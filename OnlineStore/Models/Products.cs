using System;
using System.Drawing;
using System.Web;

namespace Model
{
    public class Product
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }    
        public HttpPostedFileBase Picture { get; set; }
        public byte[] PictureR { get; set; }
    }


}