﻿using DemoLib.entity;
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

            ////modelBuilder.Ignore(typeof(double));



            //modelBuilder.Entity<Triangle>().Ignore(tri => tri.Sides);



            ////modelBuilder.Entity<Triangle>().HasMany<Point>(tri => tri.Vertices);
            ////modelBuilder.Entity<Triangle>().HasMany<Point>(t=>t.Vertices).WithOne().hasre
            ////HasMany(t=>t.Vertices).ToTable()
            //modelBuilder.Entity<Triangle>().HasMany<Point>(t => t.Vertices).
            //        WithOne().OnDelete(DeleteBehavior.Cascade);
            ////modelBuilder.Entity<Triangle>().OwnsMany<Point>(t => t.Vertices,(x)=> 
            ////{

            ////});//.HasForeignKey()

            ////base.OnModelCreating(modelBuilder);
        }
        public DbSet<Point> Points { get; set; }
        public DbSet<Triangle> Triangles { get; set; }
        public DbSet<TriangleVertex> TriangleVertices { get; set; }

    }
}