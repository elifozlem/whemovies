using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Data.Entity;
using System.Data.Entity.Validation;

namespace VeriCekme
{
    public partial class Form1 : Form
    {
        MovieContext db = new MovieContext();
       
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
           // LoadMovies();
            LoadData();
        }
      
        public void LoadMovies()
        {

            //video tablosu oluştur. konu tür gibi bilgileri de çek. imdb.title adreslerini kes son kısmı al
            var web = new HtmlWeb();
           
          
            for (int i = 1; i <= 10; i++)
            {
                var doc = web.Load("http://gunlukfilm.com/page/" + i);
                var nodes = doc.DocumentNode.SelectNodes("//div[@class='film']/div[@class='film-baslik']/a");
               foreach (var item in nodes)
                {
                    var a = new Link();  
                    var lnk = item.Attributes["href"].Value;
                    var ad = item.InnerText;
                   
                    a.LINK1 = lnk;
                    a.LINKID = Guid.NewGuid();
                    a.NAME = ad;
                   var count =db.Links.Count(m => m.LINK1 == a.LINK1); //aynı verileri kaydetmesin.
                   if(count==0)
                   { 
                       db.Links.Add(a);
                       db.SaveChanges();
                   }
                    
                    

                }
              
            }
           
        } 
        public void LoadData()
        {
            
            var web = new HtmlWeb();
            db.Links.ToList();
            for (int i = 0; i < db.Links.Local.Count; i++)
            {
                var doc = web.Load(db.Links.Local[i].LINK1);
                var trailers = doc.DocumentNode.SelectNodes("//div[@class='KoD8 mrtrailer']/a");
                var imdb = doc.DocumentNode.SelectNodes("//div[@class='KoD8 imdb']/em/a");

                var video = doc.DocumentNode.SelectNodes("//div[@class='icerik']/div[@class='postTabs_divs']/p[iframe]"); //bütün olarak alıyor.//div[@class='postTabs_divs']/p[iframe]
                var topic = doc.DocumentNode.SelectNodes("//p");
              
                var mv = new Movie();
                var pt = new Part();
            
         
                foreach(var item in topic)
                {
                    
                    var tp = item.InnerText;
                    if (tp != "")
                    {
                        mv.TOPIC = tp;  //Yemek tarifleri çekiyor???????
                        
                    }
                  
                }

                if (trailers != null)
                {
                    foreach (var item in trailers)
                    {
                        var lnk = item.Attributes["href"].Value;
                        mv.TRAILER = lnk;
                       



                    }
                }
                else
                    mv.TRAILER = "";

                mv.NAME = db.Links.Local[i].NAME;
                if (imdb != null)
                {
                    foreach (var item in imdb)
                    {

                        var ımdb = item.Attributes["href"].Value;
                        mv.IMDB = ımdb.Substring(26);
                        mv.MOVIEID = db.Links.Local[i].LINKID;


                    }
                }
                else
                    mv.IMDB = "";

                pt.MOVIEID = mv.MOVIEID;
                if (video != null)
                {
                    foreach (var item in video)
                    {

                        var VideoLink = item.InnerHtml;
                        pt.Link = VideoLink;

                        var Partcount = db.Parts.Count(m => m.Link == pt.Link);
                        if (Partcount == 0)
                        {

                            db.Parts.Add(pt);
                            db.SaveChanges();


                        }
                    }
                }
                else
                {
                    pt.Link = "";
                    var Partcount1 = db.Parts.Count(m => m.Link == pt.Link);
                    if (Partcount1 == 0)
                    {

                        db.Parts.Add(pt);
                        db.SaveChanges();


                    }
                }
                    var count = db.Movies.Count(m => m.MOVIEID == mv.MOVIEID);
                if(count==0)
                { 
                db.Movies.Add(mv);
                
                db.SaveChanges();

                }
            }
           
          
           
            }
      }
    }
