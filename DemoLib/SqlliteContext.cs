using DemoLib.entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DemoLib
{
    public class SqlDbContext : DbContext
    {
        public SqlDbContext(DbContextOptions<SqlDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Triangle>().Ignore(tri => tri.Angles);
            modelBuilder.Entity<Triangle>().Ignore(tri => tri.Sides);
            modelBuilder.Entity<Point>().HasKey(p => p.ID);
            modelBuilder.Entity<Triangle>().HasKey(t => t.ID);
            modelBuilder.Entity<TriangleVertex>().HasKey(t => t.ID);
            modelBuilder.Entity<TriangleVertex>().
                    HasOne<Point>(tv => tv.Vertex).
                    WithMany().OnDelete(DeleteBehavior.Restrict);
            //HasForeignKey(tv=>tv.Vertex);
            //        WithOne().
            //        HasForeignKey<TriangleVertex>(tv => tv.Vertex);
            modelBuilder.Entity<Triangle>().
                HasMany<TriangleVertex>(t => t.Vertices).WithOne().
                HasForeignKey(tv => tv.ParentID).
                OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TriangleVertex>().HasIndex(tv => tv.PointID);//TODO test with and without index

        }
        public DbSet<Point> Points { get; set; }
        public DbSet<Triangle> Triangles { get; set; }
        public DbSet<TriangleVertex> TriangleVertices { get; set; }

    }
}
