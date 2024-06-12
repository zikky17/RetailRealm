using Microsoft.AspNetCore.Mvc.Rendering;
using ModelsLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelsLibrary.ViewModels
{
    public class UserVM
    {
        public ApplicationUser ApplicationUser { get; set; }
        public IEnumerable<SelectListItem> RoleList { get; set; }
        public IEnumerable<SelectListItem> CompanyList { get; set; }

    }
}
