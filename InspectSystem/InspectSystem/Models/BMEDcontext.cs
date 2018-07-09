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
        public DbSet<InspectFields> InspectFields { get; set; }
        public DbSet<ClassesOfAreas> ClassesOfAreas { get; set; }
        public DbSet<InspectPrecautions> InspectPrecautions { get; set; }
        public DbSet<InspectDocs> InspectDocs { get; set; }
        public DbSet<InspectDocDetails> InspectDocDetails { get; set; }
        public DbSet<InspectDocDetailsTemporary> InspectDocDetailsTemporary { get; set; }

    }
}