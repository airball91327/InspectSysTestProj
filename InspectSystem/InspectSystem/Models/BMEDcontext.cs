using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace InspectSystem.Models
{
    public class BMEDcontext : DbContext
    {
        public BMEDcontext()
        : base("BMEDconnection")
        {
        }

        public DbSet<InspectAreas> InspectAreas { get; set; }
        public DbSet<InspectClasses> InspectClasses { get; set; }
        public DbSet<InspectItems> InspectItems { get; set; }
    }
}