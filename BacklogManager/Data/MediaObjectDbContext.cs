using BacklogManager.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BacklogManager.Data
{
    public class MediaObjectDbContext : DbContext
    {
        public DbSet<MediaObject> MediaObjects { get; set; }

        public MediaObjectDbContext(DbContextOptions<MediaObjectDbContext> options) : base(options) { }
    }
}
