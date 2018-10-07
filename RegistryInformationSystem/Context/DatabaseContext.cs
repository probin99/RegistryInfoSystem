using RegistryInformationSystem.Models.ComputerSystem;
using RegistryInformationSystem.Models.Login;
using RegistryInformationSystem.Models.Register;
using RegistryInformationSystem.Models.Software;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace RegistryInformationSystem.Context
{
    public class DatabaseContext: DbContext
    {
		public DatabaseContext(): base("name=DatabaseContext")
		{
		}
		public DbSet<Login> Logins { get; set; }
		public DbSet<Register> Registers { get; set; }
		public DbSet<ComputerSoftware> ComputerSoftwares { get; set; }
		public DbSet<ComputerSystem> ComputerSystems { get; set; }
	}
}