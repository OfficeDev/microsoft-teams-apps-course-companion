---
page_type: sample
languages:
- csharp
products:
- office-teams
description: Course Companion lets educators organize learning resources and modules in a central place that offers a visual search-and-browse experience for students. The app makes it easy to save, share, and collaborate around learning content.
urlFragment: microsoft-teams-apps-course-companion
---

# Course Companion App Template

| [Documentation](https://github.com/OfficeDev/microsoft-teams-apps-course-companion/wiki/Home) | [Deployment guide](https://github.com/OfficeDev/microsoft-teams-apps-course-companion/wiki/Deployment-guide) | [Architecture](https://github.com/OfficeDev/microsoft-teams-apps-course-companion/wiki/Solution-Overview) |
| ---- | ---- | ---- |

Course Companion app is designed to enable educators to drive better student outcomes through structured learning content curated from varied sources.

Using the Course Companion app, educators can easily create, organize and share learning content to create an interactive and collaborative learning experience for students within Microsoft Teams. 

## Key benefits
- Enables continuous learning for students in a structured manner.
- Enables educators to simplify creation and categorization of learning resources for better learning outcomes.
- Encourages sharing of learning materials and collaboration among educators and students.


## User personas

### Configurators
 - People who need to maintain configurations like tags, grades, subjects, etc. at an organizational level should be given access to Course Companion-Configurator app and they will set up the app for use.
 - Configure settings like grades, subjects and tags which will be used by Educators to create learning resources. 
 
### Educators
- Create learning resources to supplement course learning material.
- Categorize the learning resources in a structured manner using metadata such as images and tags.
- Build learning modules with multiple learning resources to structure learning for a subject/topic.
- Share resources or learning modules with students using a messaging extension.

### Students
- Discover new resources and learning modules in a personal app.
- Discover relevant content based on curriculum or interest using filters available on the tab.
- Bookmark content for self-paced learning across subjects.
- Share resources or learning modules using a messaging extension. 

### Admins
- People with Admin permissions will have the ability to manage i.e. edit or delete resources and learning modules created by educators.

Simplified workflow of the app:
- Admins of the app will configure the grades, subjects, and tags which can be used by educators to create learning resources.
- Educators create learning resources by uploading files, adding external web links, etc. for reference. 
- Educators can then organize related resources in a learning module.
- Students will discover the available resources and learning modules in the personal app and bookmark them for their self-paced learning. 
- Students will be able to access the bookmarked content in Your learning tab.
- Educators can configure a channel tab by selecting a learning module for focused learning on relevant topics.   
- Educators and students can use the messaging extension to select a resource or learning module of interest and easily share it with the audience in a team, in group or personal chats.

Here are some of the workflows in action:

1. **Discover Tab**: Students will be able to discover new resources for learning shared by educators by using the app in the personal scope.
![Discover tab](https://github.com/OfficeDev/microsoft-teams-apps-course-companion/wiki/Images/DiscoverTab.png)

2. **Learning Modules**: Students will be able to view related resources in learning modules which are structured to provide a clear path of learning of relevant topics.
![Learning module details](https://github.com/OfficeDev/microsoft-teams-apps-course-companion/wiki/Images/LearningModuleDetails.png)


3. **Channel Tab**: Educators will be able to configure a channel tab by selecting a learning module for focused learning on relevant topics.
![Channel tab](https://github.com/OfficeDev/microsoft-teams-apps-course-companion/wiki/Images/ChannelTab.png)


4. **Your learning**: Students will be able to access the bookmarked content in Your learning tab.
![Your learning tab](https://github.com/OfficeDev/microsoft-teams-apps-course-companion/wiki/Images/Yourlearning.png)


5. **Messaging extension**: Educators and students will be able to use the messaging extension to share a resource or learning module.
![Messaging extension](https://github.com/OfficeDev/microsoft-teams-apps-course-companion/wiki/Images/MessagingExtension.png)




  

## Legal notice

This app template is provided under the [MIT License](https://github.com/OfficeDev/microsoft-teams-apps-course-companion/blob/master/LICENSE) terms.  In addition to these terms, by using this app template you agree to the following:

- You, not Microsoft, will license the use of your app to users or organization. 

- This app template is not intended to substitute your own regulatory due diligence or make you or your app compliant with respect to any applicable regulations, including but not limited to privacy, healthcare, employment, or financial regulations.

- You are responsible for complying with all applicable privacy and security regulations including those related to use, collection and handling of any personal data by your app. This includes complying with all internal privacy and security policies of your organization if your app is developed to be sideloaded internally within your organization. Where applicable, you may be responsible for data related incidents or data subject requests for data collected through your app.

- Any trademarks or registered trademarks of Microsoft in the United States and/or other countries and logos included in this repository are the property of Microsoft, and the license for this project does not grant you rights to use any Microsoft names, logos or trademarks outside of this repository. Microsoft’s general trademark guidelines can be found [here](https://www.microsoft.com/en-us/legal/intellectualproperty/trademarks/usage/general.aspx).

- If the app template enables access to any Microsoft Internet-based services (e.g., Office365), use of those services will be subject to the separately-provided terms of use. In such cases, Microsoft may collect telemetry data related to app template usage and operation. Use and handling of telemetry data will be performed in accordance with such terms of use.

- Use of this template does not guarantee acceptance of your app to the Teams app store. To make this app available in the Teams app store, you will have to comply with the [submission and validation process](https://docs.microsoft.com/en-us/microsoftteams/platform/concepts/deploy-and-publish/appsource/publish), and all associated requirements such as including your own privacy statement and terms of use for your app.


## Getting started

Begin with the [Getting started guide](https://github.com/OfficeDev/microsoft-teams-apps-course-companion/wiki/Getting-started) to read about what the app does and how it works.

When you're ready to try out Building Access app, or to use it in your own organization, follow the steps in the [Deployment guide](https://github.com/OfficeDev/microsoft-teams-apps-course-companion/wiki/Deployment-guide). 

## Contributing

This project welcomes contributions and suggestions.  Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.opensource.microsoft.com.

When you submit a pull request, a CLA bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., status check, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.
