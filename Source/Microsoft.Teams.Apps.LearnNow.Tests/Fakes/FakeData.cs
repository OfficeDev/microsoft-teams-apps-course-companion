// <copyright file="FakeData.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Tests.Fakes
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Graph;
    using Microsoft.Teams.Apps.LearnNow.Infrastructure.Models;
    using Microsoft.Teams.Apps.LearnNow.Models;

    /// <summary>
    /// Class to fake repository data.
    /// </summary>
    public static class FakeData
    {
        /// <summary>
        /// Per page resource/learningModule count for lazy loading rendered on discover tab.
        /// </summary>
        public const string UserID = "1a1cce71-2833-4345-86e2-e9047f73e6af";

        /// <summary>
        /// Per page resource/learningModule count for lazy loading rendered on discover tab.
        /// </summary>
        public const string Id = "1a1cce71-2833-4345-86e2-e9047f73e6af";

        /// <summary>
        /// Make fake grade data for unit testing.
        /// </summary>
        /// <returns>Grade collection</returns>
        public static IEnumerable<Grade> GetGrades()
        {
            var grades = new List<Grade>();
            var grade1 = new Grade()
            {
                GradeName = "Grade 1",
                Id = Guid.Parse(Id),
                CreatedBy = Guid.Parse(UserID),
                UpdatedBy = Guid.Parse(UserID),
            };

            grades.Add(grade1);

            var grade2 = new Grade()
            {
                GradeName = "Grade 1",
                Id = Guid.Parse(Id),
                CreatedBy = Guid.Parse(UserID),
                UpdatedBy = Guid.Parse(UserID),
            };

            grades.Add(grade2);

            return grades;
        }

        /// <summary>
        /// Make fake subject data for unit testing.
        /// </summary>
        /// <returns>Subject collection</returns>
        public static IEnumerable<Subject> GetSubjects()
        {
            var subjects = new List<Subject>();
            var subject1 = new Subject()
            {
                SubjectName = "Subject 1",
                Id = Guid.Parse(Id),
                CreatedBy = Guid.Parse(UserID),
                UpdatedBy = Guid.Parse(UserID),
            };

            subjects.Add(subject1);

            var subject2 = new Subject()
            {
                SubjectName = "Subject 1",
                Id = Guid.Parse(UserID),
                CreatedBy = Guid.Parse(UserID),
                UpdatedBy = Guid.Parse(UserID),
            };

            subjects.Add(subject2);

            return subjects;
        }

        /// <summary>
        /// Make fake tag data for unit testing.
        /// </summary>
        /// <returns>Tag collection</returns>
        public static IEnumerable<Tag> GetTags()
        {
            var tags = new List<Tag>();
            var tag1 = new Tag()
            {
                TagName = "Tag 1",
                Id = Guid.Parse(Id),
                CreatedBy = Guid.Parse(UserID),
                UpdatedBy = Guid.Parse(UserID),
            };

            tags.Add(tag1);

            var tag2 = new Tag()
            {
                TagName = "Tag 1",
                Id = Guid.Parse(UserID),
                CreatedBy = Guid.Parse(UserID),
                UpdatedBy = Guid.Parse(UserID),
            };

            tags.Add(tag2);

            return tags;
        }

        /// <summary>
        /// Make fake ResourceVote data for unit testing.
        /// </summary>
        /// <returns>Tag collection</returns>
        public static IEnumerable<ResourceVote> GetResourceVotes()
        {
            IEnumerable<ResourceVote> resourceVotes = new List<ResourceVote>()
            {
                new ResourceVote()
                {
                    Id = Guid.NewGuid(),
                    ResourceId = FakeData.GetResource().Id,
                    UserId = Guid.Parse(UserID),
                },
                new ResourceVote()
                {
                    Id = Guid.NewGuid(),
                    ResourceId = FakeData.GetResource().Id,
                    UserId = Guid.NewGuid(),
                },
            };

            return resourceVotes;
        }

        /// <summary>
        /// Make fake Resource data for unit testing.
        /// </summary>
        /// <returns>Tag collection</returns>
        public static IEnumerable<Resource> GetResources()
        {
            var resources = new List<Resource>();
            var resourceTagss = new List<ResourceTag>();
            var resourceTag = new ResourceTag()
            {
                TagId = Guid.Parse(Id),
                ResourceId = Guid.Parse(Id),
            };
            resourceTagss.Add(resourceTag);
            var resource = new Resource()
            {
                Id = Guid.Parse(Id),
                Title = "test",
                CreatedBy = Guid.Parse(UserID),
                ResourceTag = resourceTagss,
                GradeId = Guid.Parse(Id),
                SubjectId = Guid.Parse(Id),
            };

            resources.Add(resource);

            return resources;
        }

        /// <summary>
        /// Make fake Resource data for non-administrator user unit testing.
        /// </summary>
        /// <returns>Tag collection</returns>
        public static Resource GetResource()
        {
            var resourceTags = new List<ResourceTag>();
            var resourceTag = new ResourceTag()
            {
                TagId = Guid.Parse(Id),
                ResourceId = Guid.Parse(Id),
            };
            resourceTags.Add(resourceTag);
            var resource = new Resource()
            {
                Id = Guid.Parse(Id),
                Title = "test",
                CreatedBy = Guid.Parse(UserID),
                ResourceTag = resourceTags,
                GradeId = Guid.Parse(Id),
                SubjectId = Guid.Parse(Id),
            };

            return resource;
        }

        /// <summary>
        /// Make fake Resource data for non-administrator user unit testing.
        /// </summary>
        /// <returns>Tag collection</returns>
        public static ResourceViewModel GetPayLoadResource()
        {
            var resourceTags = new List<ResourceTag>();
            var resourceTag = new ResourceTag()
            {
                TagId = Guid.Parse(Id),
                ResourceId = Guid.Parse(Id),
            };
            resourceTags.Add(resourceTag);
            var resource = new ResourceViewModel()
            {
                Id = Guid.Parse(Id),
                Title = "test",
                CreatedBy = Guid.Parse(UserID),
                ResourceTag = resourceTags,
                GradeId = Guid.Parse(Id),
                SubjectId = Guid.Parse(Id),
            };

            return resource;
        }

        /// <summary>
        /// Make fake LearningModule data for unit testing.
        /// </summary>
        /// <returns>Tag collection</returns>
        public static IEnumerable<LearningModule> GetLearningModules()
        {
            var resources = new List<LearningModule>();
            var resourceTagss = new List<LearningModuleTag>();
            var resourceTag = new LearningModuleTag()
            {
                TagId = Guid.Parse(Id),
                LearningModuleId = Guid.Parse(Id),
            };
            resourceTagss.Add(resourceTag);
            var resource = new LearningModule()
            {
                Id = Guid.Parse(Id),
                Title = "test",
                CreatedBy = Guid.Parse(UserID),
                LearningModuleTag = resourceTagss,
            };

            resources.Add(resource);

            return resources;
        }

        /// <summary>
        /// Make fake LearningModule data for different user.
        /// </summary>
        /// <returns>Tag collection</returns>
        public static LearningModule GetLearningModule()
        {
            var resourceTags = new List<LearningModuleTag>();
            var resourceTag = new LearningModuleTag()
            {
                TagId = Guid.Parse(Id),
                LearningModuleId = Guid.Parse(Id),
            };
            resourceTags.Add(resourceTag);
            var learningModule = new LearningModule()
            {
                Id = Guid.Parse(Id),
                Title = "test",
                CreatedBy = Guid.Parse(UserID),
                LearningModuleTag = resourceTags,
            };

            return learningModule;
        }

        /// <summary>
        /// Make fake LearningModule data for different user.
        /// </summary>
        /// <returns>Tag collection</returns>
        public static LearningModuleViewModel GetPayLoadLearningModule()
        {
            var resourceTags = new List<LearningModuleTag>();
            var resourceTag = new LearningModuleTag()
            {
                TagId = Guid.Parse(Id),
                LearningModuleId = Guid.Parse(Id),
            };
            resourceTags.Add(resourceTag);
            var learningModule = new LearningModuleViewModel()
            {
                Id = Guid.Parse(Id),
                Title = "test",
                CreatedBy = Guid.Parse(UserID),
                LearningModuleTag = resourceTags,
            };

            return learningModule;
        }

        /// <summary>
        /// Make fake ResourceVote data for unit testing.
        /// </summary>
        /// <returns>Tag collection</returns>
        public static IEnumerable<LearningModuleVote> GetLearningModuleVotes()
        {
            var resourceVotes = new List<LearningModuleVote>();
            var resourceVote = new LearningModuleVote()
            {
                Id = Guid.Parse(UserID),
                ModuleId = Guid.Parse(Id),
                UserId = Guid.Parse(UserID),
            };

            resourceVotes.Add(resourceVote);

            return resourceVotes;
        }

        /// <summary>
        /// Make fake Resource data for unit testing.
        /// </summary>
        /// <returns>Tag collection</returns>
        public static IEnumerable<string> GetUserIds()
        {
            var userIds = new List<string>();
            userIds.Add(UserID);

            return userIds;
        }

        /// <summary>
        /// Make fake Resource data for unit testing.
        /// </summary>
        /// <returns>Tag collection</returns>
        public static IEnumerable<User> GetUsers()
        {
            var users = new List<User>();
            var user = new User()
            {
                Id = UserID,
                DisplayName = "Test user",
            };

            users.Add(user);

            return users;
        }

        /// <summary>
        /// Make fake Resource data for unit testing.
        /// </summary>
        /// <returns>Tag collection</returns>
        public static Dictionary<Guid, string> GetUserDetails()
        {
            Dictionary<Guid, string> idToNameMap = new Dictionary<Guid, string>
            {
                { Guid.Parse(UserID), "Test user" },
            };
            return idToNameMap;
        }
    }
}