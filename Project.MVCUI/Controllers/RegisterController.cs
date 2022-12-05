using Project.BLL.DesignPatterns.GenericRepository.ConcRep;
using Project.COMMON.Tools;
using Project.ENTITIES.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.MVCUI.Controllers
{
    public class RegisterController : Controller
    {

        AppUserRepository _apRep;
        ProfileRepository _proRep;

        public RegisterController()
        {
            _apRep = new AppUserRepository();
            _proRep = new ProfileRepository();
        }


        public ActionResult RegisterNow()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RegisterNow(AppUser appUser,AppUserProfile userProfile)
        {
            appUser.Password = DantexCrypt.Crypt(appUser.Password); //sifreyi kriptoladık

            if(_apRep.Any(x=>x.UserName == appUser.UserName))
            {
                ViewBag.ZatenVar = "Kullanıcı ismi daha önce alınmıs";
                return View();
            }
            else if(_apRep.Any(x=>x.Email == appUser.Email))
            {
                ViewBag.ZatenVar = "Email zaten kayıtlı";
                return View();
            }

            //Kullanıcı basarılı bir şekilde register işlemini tamamladıysa ona mail gönder...
            string gonderilecekEmail = "Tebrikler...Hesabınız olusturulmustur...Hesabınızı aktive etmek https://localhost:44306/Register/Activation/" + appUser.ActivationCode + " linkine tıklayabilirsiniz";

            MailService.Send(appUser.Email, body: gonderilecekEmail, subject: "Hesap AKtivasyon!!!");
            _apRep.Add(appUser); //siz kullanıcı yanında profili ekleyecek olsa bile öncelikle Repository'nin bu metodunu calıstırmalısınız.. Cünkü AppUser'in ID'si ilk basta olusmalı...CÜnkü bizim kurdugumuz birebir ilişkide AppUser zorunlu alan , Profile ise opsiyonel alandır...Dolayısıyla Profile'in ID'si identity degildir...O yüzden Profile eklenecekken ID belirlenmek zorundadır...Birebir ilişki oldugundan dolayı da Profile'in ID'si AppUser'a denk gelmelidir... İlk basta AppUser'in ID'si SaveChanges ile olusmalı ki sonra Profile'i rahatca ekleyebilelim...

            if(!string.IsNullOrEmpty(userProfile.FirstName.Trim()) || !string.IsNullOrEmpty(userProfile.LastName.Trim()))
            {
                userProfile.ID = appUser.ID;
                _proRep.Add(userProfile);
            }

            return View("RegisterOK");
        }

        public ActionResult Activation(Guid id)
        {
            AppUser aktifEdilecek = _apRep.FirstOrDefault(x => x.ActivationCode == id);
            if(aktifEdilecek != null)
            {
                aktifEdilecek.Active = true;
                _apRep.Update(aktifEdilecek);
                TempData["HesapAktifMi"] = "Hesabınız aktif hale getirildi";
                return RedirectToAction("Login", "Home");
            }

            //Supheli bir aktivite
            TempData["HesapAktifMi"] = "Hesabınız bulunamadı";
           
            return RedirectToAction("Login", "Home");
        }

        public ActionResult RegisterOK()
        {
            return View();
        }
    }
}