// <copyright file="LearnNowContext.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Infrastructure.Repositories
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Teams.Apps.LearnNow.Infrastructure.Models;

    /// <summary>
    /// A class which represents session with learn now database and can be used to query and to save instances of entities.
    /// </summary>
    public partial class LearnNowContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LearnNowContext"/> class.
        /// </summary>
        public LearnNowContext()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LearnNowContext"/> class.
        /// </summary>
        /// <param name="options"> options to be used by DbContext.</param>
        public LearnNowContext(DbContextOptions<LearnNowContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Gets or sets Grade DbSet that can be used to query and save instances of Grade entity.
        /// </summary>
        public virtual DbSet<Grade> Grade { get; set; }

        /// <summary>
        /// Gets or sets Resource DbSet that can be used to query and save instances of Resource entity.
        /// </summary>
        public virtual DbSet<Resource> Resource { get; set; }

        /// <summary>
        /// Gets or sets ResourceTag DbSet that can be used to query and save instances of ResourceTag entity.
        /// </summary>
        public virtual DbSet<ResourceTag> ResourceTag { get; set; }

        /// <summary>
        /// Gets or sets ResourceVote DbSet that can be used to query and save instances of ResourceVote entity.
        /// </summary>
        public virtual DbSet<ResourceVote> ResourceVote { get; set; }

        /// <summary>
        /// Gets or sets Subject DbSet that can be used to query and save instances of Subject entity.
        /// </summary>
        public virtual DbSet<Subject> Subject { get; set; }

        /// <summary>
        /// Gets or sets Tag DbSet that can be used to query and save instances of Tag entity.
        /// </summary>
        public virtual DbSet<Tag> Tag { get; set; }

        /// <summary>
        /// Gets or sets LearningModule DbSet that can be used to query and save instances of LearningModule entity.
        /// </summary>
        public virtual DbSet<LearningModule> LearningModule { get; set; }

        /// <summary>
        /// Gets or sets LearningModuleVote DbSet that can be used to query and save instances of LearningModuleVote entity.
        /// </summary>
        public virtual DbSet<LearningModuleVote> LearningModuleVote { get; set; }

        /// <summary>
        /// Gets or sets TabConfiguration DbSet that can be used to query and save instances of TabConfiguration entity.
        /// </summary>
        public virtual DbSet<TabConfiguration> TabConfiguration { get; set; }

        /// <summary>
        /// Gets or sets ResourceModuleMapping DbSet that can be used to query and save instances of ResourceModuleMapping entity.
        /// </summary>
        public virtual DbSet<ResourceModuleMapping> ResourceModuleMapping { get; set; }

        /// <summary>
        /// Gets or sets UserResource DbSet that can be used to query and save instances of UserResource entity.
        /// </summary>
        public virtual DbSet<UserResource> UserResource { get; set; }

        /// <summary>
        /// Gets or sets UserLearningModule DbSet that can be used to query and save instances of UserLearningModule entity.
        /// </summary>
        public virtual DbSet<UserLearningModule> UserLearningModule { get; set; }

        /// <summary>
        /// Gets or sets Learning module tag DbSet that can be used to query and save instances of ResourceTag entity.
        /// </summary>
        public virtual DbSet<LearningModuleTag> LearningModuleTag { get; set; }

        /// <summary>
        /// Gets or sets UserSetting DbSet that can be used to query and save instances of UserSetting entity.
        /// </summary>
        public virtual DbSet<UserSettings> UserSetting { get; set; }

        /// <summary>
        ///  Method to configure the model that was discovered by convention
        ///  from the entity types exposed inDbSet properties on your derived context.
        ///  The resulting model may be cached and re-used for subsequent
        ///  instances of your derived context.
        /// </summary>
        /// <param name="modelBuilder"> provides simple API surface for configuring model that defines shape of entities,
        /// relationship between them and how they map to the database.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder = modelBuilder ?? throw new ArgumentNullException(nameof(modelBuilder));

            modelBuilder.Entity<Grade>(entity =>
            {
                entity.HasIndex(e => e.GradeName)
                    .HasName("ix_grade_GradeName");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnName("createdBy");

                entity.Property(e => e.CreatedOn)
                    .HasColumnName("createdOn")
                    .HasColumnType("datetimeoffset");

                entity.Property(e => e.GradeName)
                    .IsRequired()
                    .HasColumnName("gradeName")
                    .HasMaxLength(100);

                entity.Property(e => e.UpdatedBy)
                    .IsRequired()
                    .HasColumnName("updatedBy");

                entity.Property(e => e.UpdatedOn)
                    .HasColumnName("updatedOn")
                    .HasColumnType("datetimeoffset");
            });

            modelBuilder.Entity<Resource>(entity =>
            {
                entity.HasIndex(e => e.Title)
                    .HasName("resource_resource_Title")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.AttachmentUrl)
                    .HasColumnName("attachmentUrl")
                    .HasMaxLength(500);

                entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

                entity.Property(e => e.CreatedOn)
                    .HasColumnName("createdOn")
                    .HasColumnType("datetimeoffset");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("description")
                    .HasMaxLength(500);

                entity.Property(e => e.GradeId).HasColumnName("gradeId");

                entity.Property(e => e.ImageUrl)
                    .IsRequired()
                    .HasColumnName("imageUrl")
                    .HasMaxLength(500);

                entity.Property(e => e.LinkUrl)
                    .HasColumnName("linkUrl")
                    .HasMaxLength(500);

                entity.Property(e => e.ResourceType).HasColumnName("resourceType");

                entity.Property(e => e.SubjectId).HasColumnName("subjectId");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasColumnName("title")
                    .HasMaxLength(100);

                entity.Property(e => e.UpdatedBy).HasColumnName("updatedBy");

                entity.Property(e => e.UpdatedOn)
                    .HasColumnName("updatedOn")
                    .HasColumnType("datetimeoffset");

                entity.HasOne(d => d.Grade)
                    .WithMany(p => p.Resource)
                    .HasForeignKey(d => d.GradeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Resource_Grade");

                entity.HasOne(d => d.Subject)
                    .WithMany(p => p.Resource)
                    .HasForeignKey(d => d.SubjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Resource_subject");
            });

            modelBuilder.Entity<UserResource>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatedOn)
                    .HasColumnName("createdOn")
                    .HasColumnType("datetimeoffset");

                entity.Property(e => e.ResourceId).HasColumnName("resourceId");

                entity.Property(e => e.UserId).HasColumnName("userId");

                entity.HasOne(d => d.Resource)
                    .WithMany(p => p.UserResource)
                    .HasForeignKey(d => d.ResourceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserResource_Resource");
            });

            modelBuilder.Entity<UserLearningModule>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatedOn)
                    .HasColumnName("createdOn")
                    .HasColumnType("datetimeoffset");

                entity.Property(e => e.LearningModuleId).HasColumnName("learningModuleId");

                entity.Property(e => e.UserId).HasColumnName("userId");

                entity.HasOne(d => d.LearningModule)
                    .WithMany(p => p.UserLearningModule)
                    .HasForeignKey(d => d.LearningModuleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserLearningModule_LearningModule");
            });

            modelBuilder.Entity<ResourceTag>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.ResourceId).HasColumnName("resourceId");

                entity.Property(e => e.TagId).HasColumnName("tagId");

                entity.HasOne(d => d.Resource)
                    .WithMany(p => p.ResourceTag)
                    .HasForeignKey(d => d.ResourceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ResourceTag_Resource");

                entity.HasOne(d => d.Tag)
                    .WithMany(p => p.ResourceTag)
                    .HasForeignKey(d => d.TagId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ResourceTag_Tag");
            });

            modelBuilder.Entity<ResourceVote>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatedOn)
                    .HasColumnName("createdOn")
                    .HasColumnType("datetimeoffset");

                entity.Property(e => e.ResourceId).HasColumnName("resourceId");

                entity.Property(e => e.UserId).HasColumnName("userId");

                entity.HasIndex(e => new { e.UserId, e.ResourceId })
                    .HasName("unique_resourceVote")
                    .IsUnique();
            });

            modelBuilder.Entity<Subject>(entity =>
            {
                entity.ToTable("subject");

                entity.HasIndex(e => e.SubjectName)
                    .HasName("ix_subject_SubjectName");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnName("createdBy");

                entity.Property(e => e.CreatedOn)
                    .HasColumnName("createdOn")
                    .HasColumnType("datetimeoffset");

                entity.Property(e => e.SubjectName)
                    .IsRequired()
                    .HasColumnName("subjectName")
                    .HasMaxLength(100);

                entity.Property(e => e.UpdatedBy)
                    .IsRequired()
                    .HasColumnName("updatedBy");

                entity.Property(e => e.UpdatedOn)
                    .HasColumnName("updatedOn")
                    .HasColumnType("datetimeoffset");
            });

            modelBuilder.Entity<Tag>(entity =>
            {
                entity.HasIndex(e => e.TagName)
                    .HasName("ix_tag_TagName");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnName("createdBy");

                entity.Property(e => e.CreatedOn)
                    .HasColumnName("createdOn")
                    .HasColumnType("datetimeoffset");

                entity.Property(e => e.TagName)
                    .IsRequired()
                    .HasColumnName("tagName")
                    .HasMaxLength(100);

                entity.Property(e => e.UpdatedBy)
                    .IsRequired()
                    .HasColumnName("updatedBy");

                entity.Property(e => e.UpdatedOn)
                    .HasColumnName("updatedOn")
                    .HasColumnType("datetimeoffset");
            });

            modelBuilder.Entity<LearningModule>(entity =>
            {
                entity.HasIndex(e => e.Title)
                    .HasName("ix_learningModule_Title")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

                entity.Property(e => e.CreatedOn)
                    .HasColumnName("createdOn")
                    .HasColumnType("datetimeoffset");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("description")
                    .HasMaxLength(500)
                    .IsFixedLength();

                entity.Property(e => e.GradeId).HasColumnName("gradeId");

                entity.Property(e => e.ImageUrl)
                    .IsRequired()
                    .HasColumnName("imageUrl")
                    .HasMaxLength(500)
                    .IsFixedLength();

                entity.Property(e => e.SubjectId).HasColumnName("subjectId");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasColumnName("title")
                    .HasMaxLength(200)
                    .IsFixedLength();

                entity.Property(e => e.UpdatedBy).HasColumnName("updatedBy");

                entity.Property(e => e.UpdatedOn)
                    .HasColumnName("updatedOn")
                    .HasColumnType("datetimeoffset");

                entity.HasOne(d => d.Grade)
                    .WithMany(p => p.LearningModule)
                    .HasForeignKey(d => d.GradeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LearningModule_Grade");

                entity.HasOne(d => d.Subject)
                    .WithMany(p => p.LearningModule)
                    .HasForeignKey(d => d.SubjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LearningModule_subject");
            });

            modelBuilder.Entity<LearningModuleTag>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.LearningModuleId).HasColumnName("learningModuleId");

                entity.Property(e => e.TagId).HasColumnName("tagId");

                entity.HasOne(d => d.LearningModule)
                    .WithMany(p => p.LearningModuleTag)
                    .HasForeignKey(d => d.LearningModuleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LearningModuleTag_LearningModule");

                entity.HasOne(d => d.Tag)
                    .WithMany(p => p.LearningModuleTag)
                    .HasForeignKey(d => d.TagId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LearningModuleTag_Tag");
            });

            modelBuilder.Entity<LearningModuleVote>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatedOn)
                    .HasColumnName("createdOn")
                    .HasColumnType("datetimeoffset");

                entity.Property(e => e.ModuleId).HasColumnName("moduleId");

                entity.Property(e => e.UserId).HasColumnName("userId");

                entity.HasIndex(e => new { e.UserId, e.ModuleId })
                    .HasName("unique_learningModuleVote")
                    .IsUnique();
            });

            modelBuilder.Entity<LearningModuleTag>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.LearningModuleId).HasColumnName("learningModuleId");

                entity.Property(e => e.TagId).HasColumnName("tagId");

                entity.HasOne(d => d.LearningModule)
                    .WithMany(p => p.LearningModuleTag)
                    .HasForeignKey(d => d.LearningModuleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LearningModuleTag_LearningModule");

                entity.HasOne(d => d.Tag)
                    .WithMany(p => p.LearningModuleTag)
                    .HasForeignKey(d => d.TagId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LearningModuleTag_Tag");
            });

            modelBuilder.Entity<ResourceModuleMapping>(entity =>
            {
                entity.Property(e => e.LearningModuleId).HasColumnName("learningModuleId");

                entity.Property(e => e.ResourceId).HasColumnName("resourceId");

                entity.Property(e => e.CreatedOn).HasColumnName("createdOn").HasColumnType("datetimeoffset");

                entity.HasOne(d => d.LearningModule)
                    .WithMany()
                    .HasForeignKey(d => d.LearningModuleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ResourceModuleMapping_LearningModule");

                entity.HasOne(d => d.Resource)
                    .WithMany()
                    .HasForeignKey(d => d.ResourceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ResourceModuleMapping_Resource");
            });

            modelBuilder.Entity<TabConfiguration>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.ChannelId)
                    .IsRequired()
                    .HasColumnName("channelId")
                    .HasMaxLength(200);

                entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

                entity.Property(e => e.CreatedOn)
                    .HasColumnName("createdOn")
                    .HasColumnType("datetimeoffset");

                entity.Property(e => e.LearningModuleId).HasColumnName("learningModuleId");

                entity.Property(e => e.TeamId)
                    .IsRequired()
                    .HasColumnName("teamId")
                    .HasMaxLength(200);

                entity.Property(e => e.GroupId)
                   .IsRequired()
                   .HasColumnName("groupId")
                   .HasMaxLength(200);

                entity.Property(e => e.UpdatedBy).HasColumnName("updatedBy");

                entity.Property(e => e.UpdatedOn)
                    .HasColumnName("updatedOn")
                    .HasColumnType("datetimeoffset");
            });

            modelBuilder.Entity<ResourceModuleMapping>().HasKey(x => new { x.ResourceId, x.LearningModuleId });

            modelBuilder.Entity<UserSettings>(entity =>
            {
                entity.HasKey(e => e.UserId);

                entity.Property(e => e.UserId)
                    .HasColumnName("userId")
                    .ValueGeneratedNever();

                entity.Property(e => e.ModuleCreatedByObjectIds)
                    .HasColumnName("moduleAuthorIds")
                    .HasMaxLength(500);

                entity.Property(e => e.ModuleGradeIds)
                    .HasColumnName("moduleGradeIds")
                    .HasMaxLength(500);

                entity.Property(e => e.ModuleSubjectIds)
                    .HasColumnName("moduleSubjectIds")
                    .HasMaxLength(500);

                entity.Property(e => e.ModuleTagIds)
                    .HasColumnName("moduleTagIds")
                    .HasMaxLength(500);

                entity.Property(e => e.ResourceCreatedByObjectIds)
                    .HasColumnName("resourceAuthorIds")
                    .HasMaxLength(500);

                entity.Property(e => e.ResourceGradeIds)
                    .HasColumnName("resourceGradeIds")
                    .HasMaxLength(500);

                entity.Property(e => e.ResourceSubjectIds)
                    .HasColumnName("resourceSubjectIds")
                    .HasMaxLength(500);

                entity.Property(e => e.ResourceTagIds)
                    .HasColumnName("resourceTagIds")
                    .HasMaxLength(500);
            });

            this.OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
