using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RegistryInformationSystem.Models.Software
{
    public class ComputerSoftware
    {
        [Key]
        public int SoftwareID { get; set; }
        public string Name { get; set; }
        public string Publisher { get; set; }
        public DateTime InstalledOn { get; set; }
        public int Size { get; set; }
        public string Version { get; set; }
        public int SystemComponent { get; set; }
        public string InstalledLocation { get; set; }
        public string Status { get; set; }
    }
}