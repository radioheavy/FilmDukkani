using Project.ENTITIES.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.MVCUI.VMClasses
{
    public class AppUserVM
    {
        public AppUser AppUser { get; set; }
        public AppUserProfile UserProfile { get; set; }
        public List<AppUser> AppUsers { get; set; }
        public List<AppUserProfile> Profiles { get; set; }


    }
}