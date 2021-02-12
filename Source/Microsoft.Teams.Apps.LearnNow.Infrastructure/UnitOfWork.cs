// <copyright file="UnitOfWork.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Infrastructure
{
    using System.Threading.Tasks;
    using Microsoft.Teams.Apps.LearnNow.Infrastructure.Repositories;

    /// <summary>
    /// A class which handles common operations with entity collection.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly LearnNowContext context;
        private ISubjectRepository subjectRepository;
        private IGradeRepository gradeRepository;
        private ITagRepository tagRepository;
        private IResourceRepository resourceRepository;
        private IResourceTagRepository resourceTagRepository;
        private IResourceVoteRepository resourceVoteRepository;
        private ILearningModuleRepository learningModuleRepository;
        private ILearningModuleVoteRepository learningModuleVoteRepository;
        private IResourceModuleRepository resourceModuleRepository;
        private ITabConfigurationRepository tabConfigurationRepository;
        private IUserResourceRepository userResourceRepository;
        private IUserLearningModuleRepository userLearningModuleRepository;
        private ILearningModuleTagRepository learningModuleTagRepository;
        private IUserSettingRepository userSettingRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWork"/> class.
        /// </summary>
        /// <param name="context">context to be used with database and can be used to query and to save instance of entities.</param>
        public UnitOfWork(LearnNowContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Gets repository instance for working with Grade entity.
        /// </summary>
        public IGradeRepository GradeRepository
        {
            get
            {
                if (this.gradeRepository == null)
                {
                    this.gradeRepository = new GradeRepository(this.context);
                }

                return this.gradeRepository;
            }
        }

        /// <summary>
        /// Gets repository instance for working with Subject entity.
        /// </summary>
        public ISubjectRepository SubjectRepository
        {
            get
            {
                if (this.subjectRepository == null)
                {
                    this.subjectRepository = new SubjectRepository(this.context);
                }

                return this.subjectRepository;
            }
        }

        /// <summary>
        /// Gets repository instance for working with Tag entity.
        /// </summary>
        public ITagRepository TagRepository
        {
            get
            {
                if (this.tagRepository == null)
                {
                    this.tagRepository = new TagRepository(this.context);
                }

                return this.tagRepository;
            }
        }

        /// <summary>
        /// Gets repository instance for working with Resource Tag entity.
        /// </summary>
        public IResourceTagRepository ResourceTagRepository
        {
            get
            {
                if (this.resourceTagRepository == null)
                {
                    this.resourceTagRepository = new ResourceTagRepository(this.context);
                }

                return this.resourceTagRepository;
            }
        }

        /// <summary>
        /// Gets repository instance for working with Resource entity.
        /// </summary>
        public IResourceRepository ResourceRepository
        {
            get
            {
                if (this.resourceRepository == null)
                {
                    this.resourceRepository = new ResourceRepository(this.context);
                }

                return this.resourceRepository;
            }
        }

        /// <summary>
        /// Gets repository instance for working with Resource Vote entity.
        /// </summary>
        public IResourceVoteRepository ResourceVoteRepository
        {
            get
            {
                if (this.resourceVoteRepository == null)
                {
                    this.resourceVoteRepository = new ResourceVoteRepository(this.context);
                }

                return this.resourceVoteRepository;
            }
        }

        /// <summary>
        /// Gets repository instance for working with Learning Module entity.
        /// </summary>
        public ILearningModuleRepository LearningModuleRepository
        {
            get
            {
                if (this.learningModuleRepository == null)
                {
                    this.learningModuleRepository = new LearningModuleRepository(this.context);
                }

                return this.learningModuleRepository;
            }
        }

        /// <summary>
        /// Gets repository instance for working with Learning Module vote entity.
        /// </summary>
        public ILearningModuleVoteRepository LearningModuleVoteRepository
        {
            get
            {
                if (this.learningModuleVoteRepository == null)
                {
                    this.learningModuleVoteRepository = new LearningModuleVoteRepository(this.context);
                }

                return this.learningModuleVoteRepository;
            }
        }

        /// <summary>
        /// Gets repository instance for working with Resource Learning Module mapping entity.
        /// </summary>
        public IResourceModuleRepository ResourceModuleRepository
        {
            get
            {
                if (this.resourceModuleRepository == null)
                {
                    this.resourceModuleRepository = new ResourceModuleRepository(this.context);
                }

                return this.resourceModuleRepository;
            }
        }

        /// <summary>
        /// Gets repository instance for working with User Resource entity.
        /// </summary>
        public IUserResourceRepository UserResourceRepository
        {
            get
            {
                if (this.userResourceRepository == null)
                {
                    this.userResourceRepository = new UserResourceRepository(this.context);
                }

                return this.userResourceRepository;
            }
        }

        /// <summary>
        /// Gets repository instance for working with User Learning Module entity.
        /// </summary>
        public IUserLearningModuleRepository UserLearningModuleRepository
        {
            get
            {
                if (this.userLearningModuleRepository == null)
                {
                    this.userLearningModuleRepository = new UserLearningModuleRepository(this.context);
                }

                return this.userLearningModuleRepository;
            }
        }

        /// <summary>
        /// Gets repository instance for working with Tab Configuration entity.
        /// </summary>
        public ITabConfigurationRepository TabConfigurationRepository
        {
            get
            {
                if (this.tabConfigurationRepository == null)
                {
                    this.tabConfigurationRepository = new TabConfigurationRepository(this.context);
                }

                return this.tabConfigurationRepository;
            }
        }

        /// <summary>
        /// Gets repository instance for working with Learning Module Tag entity.
        /// </summary>
        public ILearningModuleTagRepository LearningModuleTagRepository
        {
            get
            {
                if (this.learningModuleTagRepository == null)
                {
                    this.learningModuleTagRepository = new LearningModuleTagRepository(this.context);
                }

                return this.learningModuleTagRepository;
            }
        }

        /// <summary>
        /// Gets repository instance for working with user resource filter setting entity.
        /// </summary>
        public IUserSettingRepository UserSettingRepository
        {
            get
            {
                if (this.userSettingRepository == null)
                {
                    this.userSettingRepository = new UserSettingRepository(this.context);
                }

                return this.userSettingRepository;
            }
        }

        /// <summary>
        /// Saves all changes made in the context to the database.
        /// </summary>
        /// <returns>A task that represents the work queued to execute.</returns>
        public async Task SaveChangesAsync()
        {
            await this.context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}