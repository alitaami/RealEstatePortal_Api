using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Common.ViewModels
{
    public class RoleViewModel
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

 
        [StringLength(100)]
        public string Description { get; set; }

        public bool IsDelete { get; set; }
    }
}
