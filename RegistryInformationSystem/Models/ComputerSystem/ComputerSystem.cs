using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RegistryInformationSystem.Models.ComputerSystem
{
    public class ComputerSystem
    {        
        [Key]
        public int ComputerSystemId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public bool IsLocal { get; set; }
        public bool IsArray { get; set; }
        public string Origin { get; set; }
        public string Qualifiers { get; set; }
    }
}